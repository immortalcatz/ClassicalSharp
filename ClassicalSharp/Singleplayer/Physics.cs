﻿// ClassicalSharp copyright 2014-2016 UnknownShadow200 | Licensed under MIT
using System;
using System.Collections.Generic;
using ClassicalSharp.Map;
using ClassicalSharp.Events;

namespace ClassicalSharp.Singleplayer {

	public class Physics {
		Game game;
		World map;
		Random rnd = new Random();
		BlockInfo info;
		int width, length, height, oneY;
		
		public const uint tickMask = 0xF8000000;
		public const uint posMask =  0x07FFFFFF;
		public const int tickShift = 27;
		FallingPhysics falling;
		TNTPhysics tnt;
		FoilagePhysics foilage;
		LiquidPhysics liquid;
		OtherPhysics other;
		
		bool enabled = true;
		public bool Enabled {
			get { return enabled; }
			set { enabled = value; liquid.Clear(); }
		}
		
		public Action<int, byte>[] OnActivate = new Action<int, byte>[256];
		public Action<int, byte>[] OnRandomTick = new Action<int, byte>[256];
		public Action<int, byte>[] OnPlace = new Action<int, byte>[256];
		public Action<int, byte>[] OnDelete = new Action<int, byte>[256];
		
		public Physics( Game game ) {
			this.game = game;
			map = game.World;
			info = game.BlockInfo;
			game.WorldEvents.OnNewMapLoaded += ResetMap;
			game.UserEvents.BlockChanged += BlockChanged;
			enabled = Options.GetBool( OptionsKey.SingleplayerPhysics, true );
			
			falling = new FallingPhysics( game, this );
			tnt = new TNTPhysics( game, this );
			foilage = new FoilagePhysics( game, this );
			liquid = new LiquidPhysics( game, this );
			other = new OtherPhysics( game, this );
		}
		
		internal static bool CheckItem( Queue<uint> queue, out int posIndex ) {
			uint packed = queue.Dequeue();
			int tickDelay = (int)((packed & tickMask) >> tickShift);
			posIndex = (int)(packed & posMask);

			if( tickDelay > 0 ) {
				tickDelay--;
				queue.Enqueue( (uint)posIndex | ((uint)tickDelay << tickShift) );
				return false;
			}
			return true;
		}
		
		int tickCount = 0;
		public void Tick() {
			if( !Enabled || game.World.IsNotLoaded ) return;
			
			//if( (tickCount % 5) == 0 ) {
			liquid.TickLava();
			liquid.TickWater();
			//}
			tickCount++;
			TickRandomBlocks();
		}
		
		void BlockChanged( object sender, BlockChangedEventArgs e ) {
			if( !Enabled ) return;
			Vector3I p = e.Coords;
			int index = (p.Y * length + p.Z) * width + p.X;
			byte block = e.Block;
			
			if( block == Block.Air && IsEdgeWater( p.X, p.Y, p.Z ) ) { 
				block = Block.StillWater; 
				game.UpdateBlock( p.X, p.Y, p.Z, Block.StillWater );
			}
			
			if( e.Block == 0 ) {
				Action<int, byte> delete = OnDelete[e.OldBlock];
				if( delete != null ) delete( index, e.OldBlock );
			} else {
				Action<int, byte> place = OnPlace[block];
				if( place != null ) place( index, block );
			}
			
			if( p.X > 0 ) Activate( index - 1 );
			if( p.X < map.Width - 1 ) Activate( index + 1 );
			if( p.Z > 0 ) Activate( index - map.Width );
			if( p.Z < map.Length - 1 ) Activate( index + map.Width );
			if( p.Y > 0 ) Activate( index - oneY );
			if( p.Y < map.Height - 1 ) Activate( index + oneY );
		}
		
		void Activate( int index ) {
			byte block = map.blocks[index];
			Action<int, byte> activate = OnActivate[block];
			if( activate != null ) activate( index, block );
		}
		
		bool IsEdgeWater( int x, int y, int z ) {
			WorldEnv env = map.Env;
			if( !(env.EdgeBlock == Block.Water || env.EdgeBlock == Block.StillWater) )
				return false;
			
			return y >= env.SidesHeight && y < env.EdgeHeight 
				&& (x == 0 || z == 0 || x == (map.Width - 1) || z == (map.Length - 1) );
		}
		
		void ResetMap( object sender, EventArgs e ) {
			falling.ResetMap();
			liquid.ResetMap();
			width = map.Width;
			height = map.Height;
			length = map.Length;
			oneY = width * length;
		}
		
		public void Dispose() {
			game.WorldEvents.OnNewMapLoaded -= ResetMap;
			game.UserEvents.BlockChanged -= BlockChanged;
		}
		
		void TickRandomBlocks() {
			int xMax = width - 1, yMax = height - 1, zMax = length - 1;
			for( int y = 0; y < height; y += 16 )
				for( int z = 0; z < length; z += 16 )
					for( int x = 0; x < width; x += 16 )
			{
				int lo = (y * length + z) * width + x;
				int hi = (Math.Min( yMax, y + 15 ) * length + Math.Min( zMax, z + 15 ))
					* width + Math.Min( xMax, x + 15 );
				
				// Inlined 3 random ticks for this chunk
				int index = rnd.Next( lo, hi );
				byte block = map.blocks[index];
				Action<int, byte> tick = OnRandomTick[block];
				if( tick != null ) tick( index, block );
				
				index = rnd.Next( lo, hi );
				block = map.blocks[index];
				tick = OnRandomTick[block];
				if( tick != null ) tick( index, block );
				
				index = rnd.Next( lo, hi );
				block = map.blocks[index];
				tick = OnRandomTick[block];
				if( tick != null ) tick( index, block );
			}
		}
	}
}