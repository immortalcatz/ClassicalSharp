﻿// ClassicalSharp copyright 2014-2016 UnknownShadow200 | Licensed under MIT
using System;
using System.IO;
using ClassicalSharp.Network;
using Launcher.Gui.Views;
using Launcher.Patcher;

namespace Launcher.Gui.Screens {	
	public sealed class ResourcesScreen : Screen {
		
		ResourceFetcher fetcher;
		ResourcesView view;
		public ResourcesScreen( LauncherWindow game ) : base( game ) {
			game.Window.Mouse.Move += MouseMove;
			game.Window.Mouse.ButtonDown += MouseButtonDown;
			view = new ResourcesView( game );
			widgets = view.widgets;
		}

		public override void Init() {
			view.Init();
			SetWidgetHandlers();
			Resize();
		}
		
		void SetWidgetHandlers() {
			widgets[view.yesIndex].OnClick = DownloadResources;
			widgets[view.noIndex].OnClick = (x, y) => GotoNextMenu();
			widgets[view.cancelIndex].OnClick = (x, y) => GotoNextMenu();
		}
		
		bool failed;
		public override void Tick() {
			if( fetcher == null || failed ) return;
			CheckCurrentProgress();
			
			if( !fetcher.Check( SetStatus ) )
				failed = true;
			
			if( !fetcher.Done ) return;
			if( ResourceList.Files.Count > 0 ) {
				ResourcePatcher patcher = new ResourcePatcher( fetcher );
				patcher.Run();
			}
			
			fetcher = null;
			GC.Collect();
			game.TryLoadTexturePack();
			GotoNextMenu();
		}
		
		public override void Resize() {
			view.DrawAll();
			game.Dirty = true;
		}
		
		void CheckCurrentProgress() {
			Request item = fetcher.downloader.CurrentItem;
			if( item == null ) { view.lastProgress = int.MinValue; return; }
			
			int progress = fetcher.downloader.CurrentItemProgress;
			if( progress == view.lastProgress ) return;
			view.lastProgress = progress;
			SetFetchStatus( progress );
		}
		
		void SetFetchStatus( int progress ) {
			if( progress >= 0 && progress <= 100 ) {
				view.DrawProgressBox( progress );
				game.Dirty = true;
			}
		}
		
		void DownloadResources( int mouseX, int mouseY ) {
			if( game.Downloader == null )
				game.Downloader = new AsyncDownloader( "null" );
			if( fetcher != null ) return;
			
			fetcher = game.fetcher;
			fetcher.DownloadItems( game.Downloader, SetStatus );
			selectedWidget = null;
			
			widgets[view.yesIndex].Visible = false;
			widgets[view.noIndex].Visible = false;
			widgets[view.textIndex].Visible = false;
			widgets[view.cancelIndex].Visible = true;
			widgets[view.sliderIndex].Visible = true;
			Resize();
		}
		
		void GotoNextMenu() {
			if( File.Exists( "options.txt" ) ) {
				game.SetScreen( new MainScreen( game ) );
			} else {
				game.SetScreen( new ChooseModeScreen( game, true ) );
			}
		}
		
		void SetStatus( string text ) {
			view.downloadingItems = true;
			view.RedrawStatus( text );
			game.Dirty = true;
		}
		
		public override void Dispose() {
			game.Window.Mouse.Move -= MouseMove;
			game.Window.Mouse.ButtonDown -= MouseButtonDown;
			view.Dispose();
		}
	}
}
