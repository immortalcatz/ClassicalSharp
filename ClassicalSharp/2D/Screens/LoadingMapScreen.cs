﻿// ClassicalSharp copyright 2014-2016 UnknownShadow200 | Licensed under MIT
using System;
using System.Drawing;
using ClassicalSharp.Events;
using ClassicalSharp.GraphicsAPI;
using ClassicalSharp.Gui.Widgets;
using ClassicalSharp.Model;
using ClassicalSharp.TexturePack;
using OpenTK.Input;

namespace ClassicalSharp.Gui.Screens {
	public class LoadingMapScreen : Screen {
		
		readonly Font font;
		public LoadingMapScreen( Game game, string name, string motd ) : base( game ) {
			serverName = name;
			serverMotd = motd;
			font = new Font( game.FontName, 16 );
		}
		
		string serverName, serverMotd;
		float progress;
		ChatTextWidget titleWidget, messageWidget;
		const int progWidth = 220, progHeight = 10;
		readonly FastColour backCol = new FastColour( 128, 128, 128 );
		readonly FastColour progressCol = new FastColour( 128, 255, 128 );
		
		public override void Render( double delta ) {
			api.Texturing = true;
			DrawBackground();
			titleWidget.Render( delta );
			messageWidget.Render( delta );
			api.Texturing = false;
			
			int progX = game.Width / 2 - progWidth / 2;
			int progY = game.Height / 2 - progHeight / 2;
			api.Draw2DQuad( progX, progY, progWidth, progHeight, backCol );
			api.Draw2DQuad( progX, progY, progWidth * progress, progHeight, progressCol );
		}
		
		void DrawBackground() {
			VertexP3fT2fC4b[] vertices = game.ModelCache.vertices;
			int index = 0, atlasIndex = 0;
			int drawnY = 0, height = game.Height;
			FastColour col = new FastColour( 64, 64, 64 );
			
			int texLoc = game.BlockInfo.GetTextureLoc( Block.Dirt, Side.Top );
			TerrainAtlas1D atlas = game.TerrainAtlas1D;
			Texture tex = new Texture( 0, 0, 0, game.Width, 64, 
			                          atlas.GetTexRec( texLoc, 1, out atlasIndex ) );
			tex.U2 = (float)game.Width / 64;
			bool bound = false;
			
			while( drawnY < height ) {
				tex.Y1 = drawnY;
				IGraphicsApi.Make2DQuad( ref tex, col, vertices, ref index );
				if( index >= vertices.Length )
					DrawBackgroundVertices( ref index, atlasIndex, ref bound );
				drawnY += 64;
			}
			DrawBackgroundVertices( ref index, atlasIndex, ref bound );
		}
		
		void DrawBackgroundVertices( ref int index, int atlasIndex, ref bool bound ) {
			if( index == 0 ) return;
			if( !bound ) {
				bound = true;
				api.BindTexture( game.TerrainAtlas1D.TexIds[atlasIndex] );
			}
					
			ModelCache cache = game.ModelCache;
			api.SetBatchFormat( VertexFormat.P3fT2fC4b );
			api.UpdateDynamicIndexedVb( DrawMode.Triangles, cache.vb, cache.vertices, index, index * 6 / 4 );
			index = 0;
		}
		
		public override void Init() {
			api.Fog = false;
			SetTitle( serverName );
			SetMessage( serverMotd );
			game.WorldEvents.MapLoading += MapLoading;
		}
		
		public void SetTitle( string title ) {
			if( titleWidget != null )
				titleWidget.Dispose();
			titleWidget = ChatTextWidget.Create( game, 0, -80, title, Anchor.Centre, Anchor.Centre, font );
		}
		
		public void SetMessage( string message ) {
			if( messageWidget != null )
				messageWidget.Dispose();
			messageWidget = ChatTextWidget.Create( game, 0, -30, message, Anchor.Centre, Anchor.Centre, font );
		}
		
		public void SetProgress( float progress ) {
			this.progress = progress;
		}

		void MapLoading( object sender, MapLoadingEventArgs e ) {
			progress = e.Progress;
		}
		
		public override void Dispose() {
			font.Dispose();
			messageWidget.Dispose();
			titleWidget.Dispose();
			game.WorldEvents.MapLoading -= MapLoading;
		}
		
		public override void OnResize( int width, int height ) {
			messageWidget.OnResize( width, height );
			titleWidget.OnResize( width, height );
		}
		
		public override bool BlocksWorld { get { return true; } }
		
		public override bool HandlesAllInput { get { return true; } }
		
		public override bool RenderHudAfter { get { return true; } }
		
		public override bool HandlesKeyDown( Key key ) {
			if( key == Key.Tab ) return true;
			return game.Gui.hudScreen.HandlesKeyDown( key );
		}
		
		public override bool HandlesKeyPress( char key )  {
			return game.Gui.hudScreen.HandlesKeyPress( key );
		}
		
		public override bool HandlesKeyUp( Key key ) {
			if( key == Key.Tab ) return true;
			return game.Gui.hudScreen.HandlesKeyUp( key );
		}
		
		public override bool HandlesMouseClick( int mouseX, int mouseY, MouseButton button ) { return true; }
		
		public override bool HandlesMouseMove( int mouseX, int mouseY ) { return true; }
		
		public override bool HandlesMouseScroll( int delta )  { return true; }
		
		public override bool HandlesMouseUp( int mouseX, int mouseY, MouseButton button ) { return true; }
	}
}
