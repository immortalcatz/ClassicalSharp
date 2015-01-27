﻿using System;
using ClassicalSharp.Entities;
using ClassicalSharp.Util;
using ClassicalSharp.Window;
using OpenTK;

namespace ClassicalSharp.Network.Packets {
	
	public sealed class KeepAliveInbound : InboundPacket {
		
		public override void ReadData( NetReader reader ) {
		}
		
		public override void ReadCallback( Game game ) {
			game.Network.SendPacket( new KeepAliveOutbound() );
		}
	}
	
	public sealed class LoginRequestInbound : InboundPacket {
		int entityId;
		string unknown;
		long mapSeed;
		byte dimension;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			unknown = reader.ReadString();
			mapSeed = reader.ReadInt64();
			dimension = reader.ReadUInt8();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class HandshakeInbound : InboundPacket {
		string hash;
		
		public override void ReadData( NetReader reader ) {
			hash = reader.ReadString();
		}
		
		public override void ReadCallback( Game game ) {
			if( hash != "-" ) {
				game.SetNewScreen( new ErrorScreen( game, "Lost connection to server", "Cannot connect to online mode server" ) );
				game.Network.Dispose();
			} else {
				game.Network.SendPacket( new LoginRequestOutbound( game.Username ) );
			}
		}
	}
	
	public sealed class ChatInbound : InboundPacket {
		string text;
		
		public override void ReadData( NetReader reader ) {
			text = reader.ReadString();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class TimeUpdateInbound : InboundPacket {
		long worldAge;
		
		public override void ReadData( NetReader reader ) {
			worldAge = reader.ReadInt64();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class EntityEquipmentInbound : InboundPacket {
		int entityId;
		short slotId;
		Slot slot;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			slotId = reader.ReadInt16();
			slot = reader.ReadSlot();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class SpawnPositionInbound : InboundPacket {
		Vector3I spawnPos;
		
		public override void ReadData( NetReader reader ) {
			spawnPos.X = reader.ReadInt32();
			spawnPos.Y = reader.ReadInt32();
			spawnPos.Z = reader.ReadInt32();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class UpdateHealthInbound : InboundPacket {
		short health;
		
		public override void ReadData( NetReader reader ) {
			health = reader.ReadInt16();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class RespawnInbound : InboundPacket {
		byte dimension;
		
		public override void ReadData( NetReader reader ) {
			dimension = reader.ReadUInt8();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class PlayerPosAndLookInbound : InboundPacket {
		float x, y, z, stanceY;
		float yaw, pitch;
		
		public override void ReadData( NetReader reader ) {
			x = (float)reader.ReadFloat64();
			stanceY = (float)reader.ReadFloat64();
			y = (float)reader.ReadFloat64();
			z = (float)reader.ReadFloat64();
			yaw = reader.ReadFloat32();
			pitch = reader.ReadFloat32();
		}
		
		public override void ReadCallback( Game game ) {
			LocationUpdate update = LocationUpdate.MakePosAndOri( x, y, z, yaw, pitch, false );
			game.Network.receivedFirstPosition = true;
			game.LocalPlayer.SetLocation( update, false );
		}
	}
	
	public sealed class UseBedInbound : InboundPacket {
		int entityId;
		byte unknown;
		int x, y, z;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			unknown = reader.ReadUInt8();
			x = reader.ReadInt32();
			y = reader.ReadInt32();
			z = reader.ReadInt32();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class AnimationInbound : InboundPacket {
		int entityId;
		byte anim;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			anim = reader.ReadUInt8();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class SpawnPlayerInbound : InboundPacket {
		int entityId;
		string playerName;
		float x, y, z;
		float yaw, pitch;
		short currentItem;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			playerName = reader.ReadString();
			x = reader.ReadInt32() / 32f;
			y = reader.ReadInt32() / 32f;
			z = reader.ReadInt32() / 32f;
			yaw = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
			pitch = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
			currentItem = reader.ReadInt16();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class SpawnPickupInbound : InboundPacket {
		int entityId;
		short itemId;
		byte itemsCount;
		short itemDamage;
		float x, y, z;
		float yaw, pitch, roll;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			itemId = reader.ReadInt16();
			itemsCount = reader.ReadUInt8();
			itemDamage = reader.ReadInt16();
			x = reader.ReadInt32() / 32f;
			y = reader.ReadInt32() / 32f;
			z = reader.ReadInt32() / 32f;
			yaw = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
			pitch = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
			roll = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class CollectItemInbound : InboundPacket {
		int collectedId;
		int collectorId;
		
		public override void ReadData( NetReader reader ) {
			collectedId = reader.ReadInt32();
			collectorId = reader.ReadInt32();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class SpawnObjectInbound : InboundPacket {
		int entityId;
		byte type;
		float x, y, z;
		int objData;
		float velX, velY, velZ;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			type = reader.ReadUInt8();
			x = reader.ReadInt32() / 32f;
			y = reader.ReadInt32() / 32f;
			z = reader.ReadInt32() / 32f;
			objData = reader.ReadInt32();
			if( objData > 0 ) {
				velX = reader.ReadInt16() / 8000f;
				velY = reader.ReadInt16() / 8000f;
				velZ = reader.ReadInt16() / 8000f;
			}
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class SpawnMobInbound : InboundPacket {
		int entityId;
		byte type;
		float x, y, z;
		float yaw, pitch;
		EntityMetadata meta;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			type = reader.ReadUInt8();
			x = reader.ReadInt32() / 32f;
			y = reader.ReadInt32() / 32f;
			z = reader.ReadInt32() / 32f;
			yaw = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
			pitch = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
			meta = EntityMetadata.ReadFrom( reader );
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class SpawnPaintingInbound : InboundPacket {
		int entityId;
		string title;
		int x, y, z;
		int direction;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			title = reader.ReadString();
			x = reader.ReadInt32();
			y = reader.ReadInt32();
			z = reader.ReadInt32();
			direction = reader.ReadInt32();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class EntityVelocityInbound : InboundPacket {
		int entityId;
		float velX, velY, velZ;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			velX = reader.ReadInt16() / 8000f;
			velY = reader.ReadInt16() / 8000f;
			velZ = reader.ReadInt16() / 8000f;
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class DestroyEntityInbound : InboundPacket {
		int entityId;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class EntityInbound : InboundPacket {
		int entityId;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class EntityRelativeMoveInbound : InboundPacket {
		int entityId;
		float dx, dy, dz;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			dx = reader.ReadInt8() / 32f;
			dy = reader.ReadInt8() / 32f;
			dz = reader.ReadInt8() / 32f;
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class EntityLookInbound : InboundPacket {
		int entityId;
		float yaw, pitch;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			yaw = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
			pitch = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class EntityLookAndRelativeMoveInbound : InboundPacket {
		int entityId;
		float dx, dy, dz;
		float yaw, pitch;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			dx = reader.ReadInt8() / 32f;
			dy = reader.ReadInt8() / 32f;
			dz = reader.ReadInt8() / 32f;
			yaw = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
			pitch = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class EntityTeleportInbound : InboundPacket {
		int entityId;
		float x, y, z;
		float yaw, pitch;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			x = reader.ReadInt32() / 32f;
			y = reader.ReadInt32() / 32f;
			z = reader.ReadInt32() / 32f;
			yaw = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
			pitch = (float)Utils.PackedToDegrees( reader.ReadUInt8() );
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class EntityStatusInbound : InboundPacket {
		int entityId;
		byte status;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			status = reader.ReadUInt8();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class AttachEntityInbound : InboundPacket {
		int collectedId;
		int collectorId;
		
		public override void ReadData( NetReader reader ) {
			collectedId = reader.ReadInt32();
			collectorId = reader.ReadInt32();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class EntityMetadataInbound : InboundPacket {
		int entityId;
		EntityMetadata meta;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			meta = EntityMetadata.ReadFrom( reader );
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class PrepareChunkInbound : InboundPacket {
		int chunkX, chunkZ;
		bool load;
		
		public override void ReadData( NetReader reader ) {
			chunkX = reader.ReadInt32();
			chunkZ = reader.ReadInt32();
			load = reader.ReadBool();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class MapChunkInbound : InboundPacket {
		int x, y, z;
		int sizeX, sizeY, sizeZ;
		byte[] compressedData;
		
		public override void ReadData( NetReader reader ) {
			x = reader.ReadInt32();
			y = reader.ReadInt16();
			z = reader.ReadInt32();
			sizeX = reader.ReadUInt8() + 1;
			sizeY = reader.ReadUInt8() + 1;
			sizeZ = reader.ReadUInt8() + 1;
			int compressedSize = reader.ReadInt32();
			compressedData = reader.ReadRawBytes( compressedSize );
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class MultiBlockChangeInbound : InboundPacket {
		int chunkX, chunkZ;
		ushort[] posMasks;
		byte[] blockTypes;
		byte[] blockMetas;
		
		public override void ReadData( NetReader reader ) {
			chunkX = reader.ReadInt32();
			chunkZ = reader.ReadInt32();
			int count = reader.ReadInt16();
			posMasks = new ushort[count];
			for( int i = 0; i < count; i++ ) {
				posMasks[i] = reader.ReadUInt16();
			}
			blockTypes = reader.ReadRawBytes( count );
			blockMetas = reader.ReadRawBytes( count );
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class BlockChangeInbound : InboundPacket {
		int x, y, z;
		byte blockType, blockMeta;
		
		public override void ReadData( NetReader reader ) {
			x = reader.ReadInt32();
			y = reader.ReadUInt8();
			z = reader.ReadInt32();
			blockType = reader.ReadUInt8();
			blockMeta = reader.ReadUInt8();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class BlockActionInbound : InboundPacket {
		int x, y, z;
		byte data1, data2;
		
		public override void ReadData( NetReader reader ) {
			x = reader.ReadInt32();
			y = reader.ReadInt16();
			z = reader.ReadInt32();
			data1 = reader.ReadUInt8();
			data2 = reader.ReadUInt8();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class ExplosionInbound : InboundPacket {
		float x, y, z;
		float radius;
		Record[] records;
		
		struct Record {
			public sbyte X, Y, Z;
			
			public Record( sbyte x, sbyte y, sbyte z ) {
				X = x;
				Y = y;
				Z = z;
			}
		}
		
		public override void ReadData( NetReader reader ) {
			x = (float)reader.ReadFloat64();
			y = (float)reader.ReadFloat64();
			z = (float)reader.ReadFloat64();
			radius = reader.ReadFloat32();
			records = new Record[reader.ReadInt32()];
			for( int i = 0; i < records.Length; i++ ) {
				records[i] = new Record( reader.ReadInt8(), reader.ReadInt8(), reader.ReadInt8() );
			}
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class SoundEffectInbound : InboundPacket {
		int effectId;
		int x, y, z;
		int data;
		
		public override void ReadData( NetReader reader ) {
			effectId = reader.ReadInt32();
			x = reader.ReadInt32();
			y = reader.ReadInt8();
			z = reader.ReadInt32();
			data = reader.ReadInt32();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class ChangeGameStateInbound : InboundPacket {
		byte state;
		
		public override void ReadData( NetReader reader ) {
			state = reader.ReadUInt8();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class ThunderboltInbound : InboundPacket {
		int entityId;
		byte unknown;
		float x, y, z;
		
		public override void ReadData( NetReader reader ) {
			entityId = reader.ReadInt32();
			unknown = reader.ReadUInt8();
			x = reader.ReadInt32() / 32f;
			y = reader.ReadInt32() / 32f;
			z = reader.ReadInt32() / 32f;
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class OpenWindowInbound : InboundPacket {
		byte windowId;
		string inventoryType;
		string windowTitle;
		byte numSlots;
		
		public override void ReadData( NetReader reader ) {
			windowId = reader.ReadUInt8();
			inventoryType = reader.ReadString();
			windowTitle = reader.ReadString();
			numSlots = reader.ReadUInt8();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class CloseWindowInbound : InboundPacket {
		byte windowId;
		
		public override void ReadData( NetReader reader ) {
			windowId = reader.ReadUInt8();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class SetSlotInbound : InboundPacket {
		byte windowId;
		short slotId;
		Slot slot;
		
		public override void ReadData( NetReader reader ) {
			windowId = reader.ReadUInt8();
			slotId = reader.ReadInt16();
			slot = reader.ReadSlot();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class WindowItemsInbound : InboundPacket {
		byte windowId;
		Slot[] slots;
		
		public override void ReadData( NetReader reader ) {
			windowId = reader.ReadUInt8();
			slots = new Slot[reader.ReadInt16()];
			for( int i = 0; i < slots.Length; i++ ) {
				slots[i] = reader.ReadSlot();
			}
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class WindowPropertyInbound : InboundPacket {
		byte windowId;
		short property;
		short value;
		
		public override void ReadData( NetReader reader ) {
			windowId = reader.ReadUInt8();
			property = reader.ReadInt16();
			value = reader.ReadInt16();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class ConfirmTransactionInbound : InboundPacket {
		byte windowId;
		short actionNum;
		bool accepted;
		
		public override void ReadData( NetReader reader ) {
			windowId = reader.ReadUInt8();
			actionNum = reader.ReadInt16();
			accepted = reader.ReadBool();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class UpdateSignInbound : InboundPacket {
		int x, y, z;
		string line1, line2, line3, line4;
		
		public override void ReadData( NetReader reader ) {
			x = reader.ReadInt32();
			y = reader.ReadInt16();
			z = reader.ReadInt32();
			line1 = reader.ReadString();
			line2 = reader.ReadString();
			line3 = reader.ReadString();
			line4 = reader.ReadString();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class MapsInbound : InboundPacket {
		short itemType;
		short itemDamage;
		byte[] data;
		
		public override void ReadData( NetReader reader ) {
			itemType = reader.ReadInt16();
			itemDamage = reader.ReadInt16();
			data = reader.ReadRawBytes( reader.ReadUInt8() );
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class StatisticsInbound : InboundPacket {
		int statId;
		byte amount;
		
		public override void ReadData( NetReader reader ) {
			statId = reader.ReadInt32();
			amount = reader.ReadUInt8();
		}
		
		public override void ReadCallback( Game game ) {
			throw new NotImplementedException();
		}
	}
	
	public sealed class DisconnectInbound : InboundPacket {
		string reason;
		
		public override void ReadData( NetReader reader ) {
			reason = reader.ReadString();
		}
		
		public override void ReadCallback( Game game ) {
			game.SetNewScreen( new ErrorScreen( game, "Lost connection to server", reason ) );
			game.Network.Dispose();
		}
	}
}