﻿// ClassicalSharp copyright 2014-2016 UnknownShadow200 | Licensed under MIT
using System;
using System.Collections.Generic;
using System.Text;
using ClassicalSharp.Renderers;
using OpenTK.Input;

namespace ClassicalSharp.Commands {
	
	/// <summary> Command that displays the list of all currently registered client commands. </summary>
	public sealed class CommandsCommand : Command {
		
		public CommandsCommand() {
			Name = "Commands";
			Help = new [] {
				"&a/client commands",
				"&ePrints a list of all usable commands"
			};
		}
		
		public override void Execute( CommandReader reader ) {
			game.CommandList.PrintDefinedCommands( game );
		}
	}
	
	/// <summary> Command that displays information about an input client command. </summary>
	public sealed class HelpCommand : Command {
		
		public HelpCommand() {
			Name = "Help";
			Help = new [] {
				"&a/client help [command name]",
				"&eDisplays the help for the given command.",
			};
		}
		
		public override void Execute( CommandReader reader ) {
			string cmdName = reader.Next();
			if( cmdName == null ) {
				game.Chat.Add( "&eList of client commands:" );
				game.CommandList.PrintDefinedCommands( game );
				game.Chat.Add( "&eTo see a particular command's help, type /client help [cmd name]" );
			} else {
				Command cmd = game.CommandList.GetMatch( cmdName );
				if( cmd == null ) return;
				string[] help = cmd.Help;
				for( int i = 0; i < help.Length; i++ )
					game.Chat.Add( help[i] );
			}
		}
	}
	
	/// <summary> Command that displays information about the user's GPU. </summary>
	public sealed class GpuInfoCommand : Command {
		
		public GpuInfoCommand() {
			Name = "GpuInfo";
			Help = new [] {
				"&a/client gpuinfo",
				"&eDisplays information about your GPU.",
			};
		}
		
		public override void Execute( CommandReader reader ) {
			string[] lines = game.Graphics.ApiInfo;
			for( int i = 0; i < lines.Length; i++ )
				game.Chat.Add( "&a" + lines[i] );
		}
	}
	
	public sealed class InfoCommand : Command {
		
		public InfoCommand() {
			Name = "Info";
			Help = new [] {
				"&a/client info [property]",
				"&bproperties: &epos, target, dimensions, jumpheight",
			};
		}
		
		public override void Execute( CommandReader reader ) {
			string property = reader.Next();
			if( property == null ) {
				game.Chat.Add( "&e/client: &cYou didn't specify a property." );
			} else if( Utils.CaselessEquals( property, "pos" ) ) {
				game.Chat.Add( "Feet: " + game.LocalPlayer.Position );
				game.Chat.Add( "Eye: " + game.LocalPlayer.EyePosition );
				Vector3I p = Vector3I.Floor( game.LocalPlayer.Position );
				game.Chat.Add( game.World.GetLightHeight( p.X, p.Z ).ToString() );
			} else if( Utils.CaselessEquals( property, "target" ) ) {
				PickedPos pos = game.SelectedPos;
				if( !pos.Valid ) {
					game.Chat.Add( "Currently not targeting a block" );
				} else {
					game.Chat.Add( "Currently targeting at: " + pos.BlockPos );
					game.Chat.Add( "ID of block targeted: " + game.World.SafeGetBlock( pos.BlockPos ) );
				}
			} else if( Utils.CaselessEquals( property, "dimensions" ) ) {
				game.Chat.Add( "map width: " + game.World.Width );
				game.Chat.Add( "map height: " + game.World.Height );
				game.Chat.Add( "map length: " + game.World.Length );
			} else {
				game.Chat.Add( "&e/client: Unrecognised property: \"&f" + property + "&e\"." );
			}
		}
	}
	
	public sealed class RenderTypeCommand : Command {
		
		public RenderTypeCommand() {
			Name = "RenderType";
			Help = new [] {
				"&a/client rendertype [normal/legacy/legacyfast]",
				"&bnormal: &eDefault renderer, with all environmental effects enabled.",
				"&blegacy: &eMay be slightly slower than normal, but produces the same environmental effects.",
				"&blegacyfast: &eSacrifices clouds, fog and overhead sky for faster performance.",
				"&bnormalfast: &eSacrifices clouds, fog and overhead sky for faster performance.",
			};
		}
		
		public override void Execute( CommandReader reader ) {
			string property = reader.Next();
			if( property == null ) {
				game.Chat.Add( "&e/client: &cYou didn't specify a new render type." );
			} else if( game.SetRenderType( property ) ) {
				game.Chat.Add( "&e/client: &fRender type is now " + property + "." );
			} else {
				game.Chat.Add( "&e/client: &cUnrecognised render type &f\"" + property + "\"&c." );
			}
		}
	}
}
