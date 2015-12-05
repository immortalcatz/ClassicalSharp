﻿// Based on:
// https://github.com/UnknownShadow200/ClassicalSharp/wiki/Minecraft-Classic-map-generation-algorithm
// Thanks to Jerralish for originally reverse engineering classic's algorithm, then preparing a high level overview of the algorithm.
// I believe this process adheres to clean room reverse engineering.
using System;
using System.Collections.Generic;

namespace ClassicalSharp.Generator {
	
	// TODO: figure out how noise functions differ, probably based on rnd.
	public sealed partial class NotchyGenerator {
		
		int width, height, length;
		int waterLevel, oneY;
		byte[] blocks;
		short[] heightmap;
		Random rnd;
		
		public byte[] GenerateMap( int width, int height, int length ) {
			this.width = width;
			this.height = height;
			this.length = length;
			oneY = width * length;
			waterLevel = height / 2;
			blocks = new byte[width * height * length];
			
			rnd = new Random( 0x5553200 );
			CreateHeightmap();
			CreateStrata();
			CarveCaves();
			CarveOreVeins( 0.9f, (byte)Block.CoalOre );
			CarveOreVeins( 0.7f, (byte)Block.IronOre );
			CarveOreVeins( 0.5f, (byte)Block.GoldOre );
			
			FloodFillWaterBorders();
			FloodFillWater();
			FloodFillLava();			
			return blocks;
		}
		
		void CreateHeightmap() {
			Noise n1 = new CombinedNoise(
				new OctaveNoise( 8, rnd ), new OctaveNoise( 8, rnd ) );
			Noise n2 = new CombinedNoise(
				new OctaveNoise( 8, rnd ), new OctaveNoise( 8, rnd ) );
			Noise n3 = new OctaveNoise( 6, rnd );
			int index = 0;
			short[] hMap = new short[width * length];
			
			for( int z = 0; z < length; z++ ) {
				for( int x = 0; x < width; x++ ) {
					double hLow = n1.Compute( x * 1.3f, z * 1.3f ) / 6 - 4;
					double hHigh = n2.Compute( x * 1.3f, z * 1.3f ) / 5 + 6;
					
					double height = n3.Compute( x, z ) > 0 ? hLow : Math.Max( hLow, hHigh );
					if( height < 0 ) height *= 0.8f;
					hMap[index++] = (short)(height + waterLevel);
				}
			}
			heightmap = hMap;
		}
		
		void CreateStrata() {
			Noise n = new OctaveNoise( 8, rnd );
			//Noise n = new ImprovedNoise( rnd );
			
			int hMapIndex = 0;
			for( int z = 0; z < length; z++ ) {
				for( int x = 0; x < width; x++ ) {
					int dirtThickness = (int)(n.Compute( x, z ) / 24 - 4);
					int dirtHeight = heightmap[hMapIndex++];

					int stoneHeight = dirtHeight + dirtThickness;
					int mapIndex = z * width + x;
					
					blocks[mapIndex] = (byte)Block.Lava;
					mapIndex += oneY;
					for( int y = 1; y < height; y++ ) {
						byte type = 0;
						if( y <= stoneHeight ) type = (byte)Block.Stone;
						else if( y <= dirtHeight ) type = (byte)Block.Dirt;
						blocks[mapIndex] = type;
						mapIndex += oneY;
					}
				}
			}
		}
		
		void CarveCaves() {
			int cavesCount = blocks.Length / 8192;
			for( int i = 0; i < cavesCount; i++ ) {
				double caveX = rnd.Next( width );
				double caveY = rnd.Next( height );
				double caveZ = rnd.Next( length );
				
				int caveLen = (int)(rnd.NextDouble() * rnd.NextDouble() * 200);
				double theta = rnd.NextDouble() * 2 * Math.PI, deltaTheta = 0;
				double phi = rnd.NextDouble() * 2 * Math.PI, deltaPhi = 0;
				double caveRadius = rnd.NextDouble() * rnd.NextDouble();
				
				for( int j = 0; j < caveLen; j++ ) {
					caveX += Math.Sin( theta ) * Math.Cos( phi );
					caveY += Math.Cos( theta ) * Math.Cos( phi );
					caveZ += Math.Sin( phi );
					
					theta = deltaTheta * 0.2;
					deltaTheta = deltaTheta * 0.9 + rnd.NextDouble() - rnd.NextDouble();
					phi = phi / 2 + deltaPhi / 4;
					deltaPhi = deltaPhi * 0.75 + rnd.NextDouble() - rnd.NextDouble();
					if( rnd.NextDouble() < 0.25 ) continue;
					
					int cenX = (int)(caveX + (rnd.Next( 4 ) - 2) * 0.2);
					int cenY = (int)(caveY + (rnd.Next( 4 ) - 2) * 0.2);
					int cenZ = (int)(caveZ + (rnd.Next( 4 ) - 2) * 0.2);
					double radius = (height - cenY) / (double)height;
					radius = 1.2 + (radius * 3.5 + 1) * caveRadius;
					radius = radius + Math.Sin( j * Math.PI / caveLen );
					FillOblateSpheroid( cenX, cenY, cenZ, (int)radius, (byte)Block.Air );
				}
			}
		}
		
		void CarveOreVeins( float abundance, byte block ) {
			int numVeins = (int)(blocks.Length * abundance / 16384);
			for( int i = 0; i < numVeins; i++ ) {
				double veinX = rnd.Next( width );
				double veinY = rnd.Next( height );
				double veinZ = rnd.Next( length );
				
				int veinLen = (int)(rnd.NextDouble() * rnd.NextDouble() * 75 * abundance);
				double theta = rnd.NextDouble() * 2 * Math.PI, deltaTheta = 0;
				double phi = rnd.NextDouble() * 2 * Math.PI, deltaPhi = 0;
				
				for( int j = 0; j < veinLen; j++ ) {
					veinX += Math.Sin( theta ) * Math.Cos( phi );
					veinY += Math.Cos( theta ) * Math.Cos( phi );
					veinZ += Math.Sin( phi );
					
					theta = deltaTheta * 0.2;
					deltaTheta = deltaTheta * 0.9 + rnd.NextDouble() - rnd.NextDouble();
					phi = phi / 2 + deltaPhi / 4;
					deltaPhi = deltaPhi * 0.9 + rnd.NextDouble() - rnd.NextDouble();
					
					int radius = (int)(abundance * Math.Sin( j * Math.PI / veinLen ) + 1);
					FillOblateSpheroid( (int)veinX, (int)veinY, (int)veinZ, radius, block );
				}
			}
		}
		
		void FloodFillWaterBorders() {
			int waterY = waterLevel - 1;
			int index1 = (waterY * length + 0) * width + 0;
			int index2 = (waterY * length + (length - 1)) * width + 0;			
			for( int x = 0; x < width; x++ ) {
				FloodFill( index1, (byte)Block.Water );
				FloodFill( index2, (byte)Block.Water );
				index1++; index2++;
			}	
			
			index1 = (waterY * length + 0) * width + 0;
			index2 = (waterY * length + 0) * width + (width - 1);		
			for( int z = 0; z < length; z++ ) {
				FloodFill( index1, (byte)Block.Water );
				FloodFill( index2, (byte)Block.Water );
				index1 += width; index2 += width;
			}		
		}
		
		void FloodFillWater() {
			int numSources = width * length / 800;
			for( int i = 0; i < numSources; i++ ) {
				int x = rnd.Next( width ), z = rnd.Next( length );
				int y = waterLevel - rnd.Next( 1, 3 );
				FloodFill( (y * length + z) * width + x, (byte)Block.Water );
			}
		}
		
		void FloodFillLava() {
			int numSources = width * length / 20000;
			for( int i = 0; i < numSources; i++ ) {
				int x = rnd.Next( width ), z = rnd.Next( length );
				int y = (int)((waterLevel - 3) * rnd.NextDouble() * rnd.NextDouble());
				FloodFill( (y * length + z) * width + x, (byte)Block.Lava );
			}
		}
		
		void CreateSurfaceLayer() {
			Noise n1 = new OctaveNoise( 8, rnd ), n2 = new OctaveNoise( 8, rnd );
			
		}
	}
}