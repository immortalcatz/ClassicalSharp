﻿// ClassicalSharp copyright 2014-2016 UnknownShadow200 | Licensed under MIT
using System;
using ClassicalSharp.Events;
using ClassicalSharp.GraphicsAPI;
using OpenTK;

namespace ClassicalSharp.Renderers {

	public unsafe class StandardEnvRenderer : EnvRenderer {
		
		int cloudsVb = -1, cloudVertices, skyVb = -1, skyVertices;
		internal bool legacy;
		
		public override void UseLegacyMode( bool legacy ) {
			this.legacy = legacy;
			ResetSky();
			ResetClouds();
		}
		
		public override void Render( double deltaTime ) {
			if( skyVb == -1 || cloudsVb == -1 ) return;
			if( !game.SkyboxRenderer.ShouldRender )
				RenderMainEnv( deltaTime );
			UpdateFog();
		}
		
		void RenderMainEnv( double deltaTime ) {
			Vector3 pos = game.CurrentCameraPos;
			float normalY = map.Height + 8;
			float skyY = Math.Max( pos.Y + 8, normalY );
			
			graphics.SetBatchFormat( VertexFormat.P3fC4b );
			graphics.BindVb( skyVb );
			if( skyY == normalY ) {
				graphics.DrawIndexedVb( DrawMode.Triangles, skyVertices * 6 / 4, 0 );
			} else {
				Matrix4 m = Matrix4.Identity;
				m.Row3.Y = skyY - normalY; // Y translation matrix
				
				graphics.PushMatrix();
				graphics.MultiplyMatrix( ref m );
				graphics.DrawIndexedVb( DrawMode.Triangles, skyVertices * 6 / 4, 0 );
				graphics.PopMatrix();
			}
			RenderClouds( deltaTime );
		}
		
		protected override void EnvVariableChanged( object sender, EnvVarEventArgs e ) {
			if( e.Var == EnvVar.SkyColour ) {
				ResetSky();
			} else if( e.Var == EnvVar.FogColour ) {
				UpdateFog();
			} else if( e.Var == EnvVar.CloudsColour ) {
				ResetClouds();
			} else if( e.Var == EnvVar.CloudsLevel ) {
				ResetSky();
				ResetClouds();
			}
		}
		
		public override void Init( Game game ) {
			base.Init( game );
			graphics.SetFogStart( 0 );
			graphics.Fog = true;
			ResetAllEnv( null, null );
			game.Events.ViewDistanceChanged += ResetAllEnv;
			game.SetViewDistance(game.UserViewDistance, false);
		}
		
		public override void OnNewMap( Game game ) {
			graphics.Fog = false;
			graphics.DeleteVb( ref skyVb );
			graphics.DeleteVb( ref cloudsVb );
		}
		
		public override void OnNewMapLoaded( Game game ) {
			graphics.Fog = true;
			ResetAllEnv( null, null );
		}
		
		void ResetAllEnv( object sender, EventArgs e ) {
			UpdateFog();
			ResetSky();
			ResetClouds();
		}
		
		public override void Dispose() {
			base.Dispose();
			game.Events.ViewDistanceChanged -= ResetAllEnv;
			graphics.DeleteVb( ref skyVb );
			graphics.DeleteVb( ref cloudsVb );
		}
		
		void RenderClouds( double delta ) {
			if( game.World.Env.CloudHeight < -2000 ) return;
			double time = game.accumulator;
			float offset = (float)( time / 2048f * 0.6f * map.Env.CloudsSpeed );
			graphics.SetMatrixMode( MatrixType.Texture );
			Matrix4 matrix = Matrix4.Identity; matrix.Row3.X = offset; // translate X axis
			graphics.LoadMatrix( ref matrix );
			graphics.SetMatrixMode( MatrixType.Modelview );
			
			graphics.AlphaTest = true;
			graphics.Texturing = true;
			graphics.BindTexture( game.CloudsTex );
			graphics.SetBatchFormat( VertexFormat.P3fT2fC4b );
			graphics.BindVb( cloudsVb );
			graphics.DrawIndexedVb_TrisT2fC4b( cloudVertices * 6 / 4, 0 );
			graphics.AlphaTest = false;
			graphics.Texturing = false;
			
			graphics.SetMatrixMode( MatrixType.Texture );
			graphics.LoadIdentityMatrix();
			graphics.SetMatrixMode( MatrixType.Modelview );
		}
		
		void UpdateFog() {
			if( map.IsNotLoaded ) return;
			FastColour fogCol = FastColour.White;
			float fogDensity = 0;
			byte block = BlockOn( out fogDensity, out fogCol );
			
			if( fogDensity != 0 ) {
				graphics.SetFogMode( Fog.Exp );
				graphics.SetFogDensity( fogDensity );
			} else {
				graphics.SetFogMode( Fog.Linear );
				graphics.SetFogEnd( game.ViewDistance );
			}
			graphics.ClearColour( fogCol );
			graphics.SetFogColour( fogCol );
		}
		
		void ResetClouds() {
			if( map.IsNotLoaded ) return;
			graphics.DeleteVb( ref cloudsVb );
			ResetClouds( (int)game.ViewDistance, legacy ? 128 : 65536 );
		}
		
		void ResetSky() {
			if( map.IsNotLoaded ) return;
			graphics.DeleteVb( ref skyVb );
			ResetSky( (int)game.ViewDistance, legacy ? 128 : 65536 );
		}
		
		void ResetClouds( int extent, int axisSize ) {
			extent = Utils.AdjViewDist( extent );
			int x1 = -extent, x2 = map.Width + extent;
			int z1 = -extent, z2 = map.Length + extent;
			cloudVertices = Utils.CountVertices( x2 - x1, z2 - z1, axisSize );
			
			VertexP3fT2fC4b[] vertices = new VertexP3fT2fC4b[cloudVertices];
			DrawCloudsY( x1, z1, x2, z2, map.Env.CloudHeight, axisSize, map.Env.CloudsCol.Pack(), vertices );
			cloudsVb = graphics.CreateVb( vertices, VertexFormat.P3fT2fC4b, cloudVertices );
		}
		
		void ResetSky( int extent, int axisSize ) {
			extent = Utils.AdjViewDist( extent );
			int x1 = -extent, x2 = map.Width + extent;
			int z1 = -extent, z2 = map.Length + extent;
			skyVertices = Utils.CountVertices( x2 - x1, z2 - z1, axisSize );
			
			VertexP3fC4b[] vertices = new VertexP3fC4b[skyVertices];
			int height = Math.Max( map.Height + 2 + 6, map.Env.CloudHeight + 6);
			
			DrawSkyY( x1, z1, x2, z2, height, axisSize, map.Env.SkyCol.Pack(), vertices );
			skyVb = graphics.CreateVb( vertices, VertexFormat.P3fC4b, skyVertices );
		}
		
		void DrawSkyY( int x1, int z1, int x2, int z2, int y, int axisSize, int col, VertexP3fC4b[] vertices ) {
			int endX = x2, endZ = z2, startZ = z1;
			int i = 0;
			
			for( ; x1 < endX; x1 += axisSize ) {
				x2 = x1 + axisSize;
				if( x2 > endX ) x2 = endX;
				z1 = startZ;
				for( ; z1 < endZ; z1 += axisSize ) {
					z2 = z1 + axisSize;
					if( z2 > endZ ) z2 = endZ;
					
					vertices[i++] = new VertexP3fC4b( x1, y, z1, col );
					vertices[i++] = new VertexP3fC4b( x1, y, z2, col );
					vertices[i++] = new VertexP3fC4b( x2, y, z2, col );
					vertices[i++] = new VertexP3fC4b( x2, y, z1, col );
				}
			}
		}
		
		void DrawCloudsY( int x1, int z1, int x2, int z2, int y, int axisSize, int col, VertexP3fT2fC4b[] vertices ) {
			int endX = x2, endZ = z2, startZ = z1;
			// adjust range so that largest negative uv coordinate is shifted to 0 or above.
			float offset = Utils.CeilDiv( -x1, 2048 );
			int i = 0;
			
			for( ; x1 < endX; x1 += axisSize ) {
				x2 = x1 + axisSize;
				if( x2 > endX ) x2 = endX;
				z1 = startZ;
				for( ; z1 < endZ; z1 += axisSize ) {
					z2 = z1 + axisSize;
					if( z2 > endZ ) z2 = endZ;
					
					vertices[i++] = new VertexP3fT2fC4b( x1, y + 0.1f, z1, x1 / 2048f + offset, z1 / 2048f + offset, col );
					vertices[i++] = new VertexP3fT2fC4b( x1, y + 0.1f, z2, x1 / 2048f + offset, z2 / 2048f + offset, col );
					vertices[i++] = new VertexP3fT2fC4b( x2, y + 0.1f, z2, x2 / 2048f + offset, z2 / 2048f + offset, col );
					vertices[i++] = new VertexP3fT2fC4b( x2, y + 0.1f, z1, x2 / 2048f + offset, z1 / 2048f + offset, col );
				}
			}
		}
	}
}