/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 7/18/20010
 * Time: 12:20 PM
 * 
 * Copyright 2010 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *	1. Redistributions of source code must retain the above copyright notice, this list of
 *	   conditions and the following disclaimer.
 * 
 *	2. Redistributions in binary form must reproduce the above copyright notice, this list
 *	   of conditions and the following disclaimer in the documentation and/or other materials
 *	   provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY Matthew Cash ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Matthew Cash OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Matthew Cash.
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Client.Connections;
using Tortoise.Client.Connections;
using Tortoise.Shared.Connections;
using System.Diagnostics;

namespace Tortoise.Client.Connections
{
	/// <summary>
	/// 
	/// </summary>
	class ServerConnection : Connection
	{
		
		public static ConnectionState ConnectionState = ConnectionState.NotConnected;
		
		private bool _readyForData;
		
		public string AuthKey
		{
			get{return _authKey;}
		}

		public bool ReadyForData
		{
			get{return _readyForData;}
		}
		
		
		public System.EventHandler<ServerMessageEventArgs> ServerMessageEvent;
		public System.EventHandler ReadyForDataEvent;

		public ServerConnection(string dest, int port):base(dest, port)
		{
			
		}

		
		internal override void HandleInput(ushort length, ushort packetID)
		{
			PacketID pID = PacketID.Null;
			if(!pID.TryParse(packetID))
			{
				SyncError();
				return;
			}

			//Switch through all of the items, even if we throw a SyncError.
			//Otherwise each ID should call a Read_{DescritiveInfo}()
			//The reason for the empty SyncError() for a release is we don't care about
			//reasons. We can assume the end developer has
			Dictionary<String, Object> debugData;
			switch(pID)
			{
				case PacketID.Null:
					debugData = new Dictionary<String, Object>();
					debugData.Add("PacketID", PacketID.Null);
					SyncError(debugData);

					break;
				//case PacketID.Authintication:
				//	Read_AuthKey();
			//		break;
				case PacketID.ClientInfo:
					debugData = new Dictionary<String, Object>();
					debugData.Add("PacketID", PacketID.ClientInfo);
					SyncError(debugData);

					break;
				case PacketID.ServerMessage:
					Read_ServerMessage();
					break;
				case PacketID.ModulePacket:
					Read_ModulePacket(length);
					break;
			}
		}
		
		void Read_AuthKey()
		{
			//(string key)
			_authKey = _sr.ReadString();
			_readyForData = true;
			if(ReadyForDataEvent !=null)
				ReadyForDataEvent(this, EventArgs.Empty);
		}
		
		void Read_ServerMessage()
		{
			//(MessageID reason)
			//Check for a valid Enum Item.
			ushort rTmp;
			rTmp = _sr.ReadUInt16();
			MessageID mID = MessageID.Null;
			if(!mID.TryParse(rTmp))
			{
				Disconnect(MessageID.SyncError);
				return;
			}
			if(ServerMessageEvent != null)
				ServerMessageEvent(this, new ServerMessageEventArgs(mID));
		}
		
		void Read_ModulePacket(ushort length)
		{
			ushort moduleID = _sr.ReadUInt16();
			if(!_moduleActions.ContainsKey(moduleID))
			{
				SyncError();
				throw new Exception("moduleID not regesterd!");
			}
			//remove 2 from the length because we just read 2 off
			byte[] data = _sr.ReadBytes(length - 2);
			
		}
		
		public void Write_Version(byte major, byte minor, ushort revision)
		{
			//2 for PacketID, 1 for major, 1 for minor, 2 for revision
			ushort length = 6;
			_sw.Write(length);
			_sw.Write((ushort)PacketID.ClientInfo);
			_sw.Write(major);
			_sw.Write(minor);
			_sw.Write(revision);
			_sw.Flush();
		}
		
		public void Write_ServerMessage(MessageID reason)
		{
			//2 for ID, 2 for message ID
			ushort length = 4;
			_sw.Write(length);
			_sw.Write((ushort)PacketID.ServerMessage);
			_sw.Write((ushort)reason);
			_sw.Flush();
		}
		
		/// <summary>
		/// Disconnects the client with the specified reason.
		/// </summary>
		public void Disconnect(MessageID reason)
		{
			Write_ServerMessage(reason);
			Disconnect();
		}
		
		public override void SyncError(Dictionary<string, object> data)
		{
			Write_ServerMessage(MessageID.SyncError);
			base.SyncError(data);
		}
	}
}
