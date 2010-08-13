/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 7/29/2010
 * Time: 10:28 PM
 * 
 * Copyright 2010 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 * 
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
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
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using C5;

using Tortoise.Shared.Module;
using Tortoise.Shared.IO;
using Tortoise.Shared.Net;


namespace Tortoise.Shared.Net
{
	/// <summary>
	/// Base connection for conitivity between 2 hosts.
	/// </summary>
	class Connection
	{
		protected static Dictionary<ushort, IComModule> _moduleActions = new Dictionary<ushort, IComModule>();
		public static void AddModuleHandle(ushort ID,IComModule module)
		{
			if(_moduleActions.ContainsKey(ID))
				throw new Exception("ID already exsists!");
			_moduleActions.Add(ID, module);
		}
		
		private Tortoise.Shared.Collection.SortedList<Packet> _packetQue = new Tortoise.Shared.Collection.SortedList<Packet>(new PacketSorter());
		
		public System.EventHandler<MessageEventArgs> MessageEvent;
		public System.EventHandler ReadyForDataEvent;

		public static ConnectionState ConnectionState = ConnectionState.NotConnected;
		
		private bool _readyForData;
		
		public bool ReadyForData
		{
			get{return _readyForData;}
		}

		protected TcpClient _client;
		protected BinaryReader _sr;
		protected BinaryWriter _sw;
		//private byte[] _data;
		protected ushort _length;
		protected DateTime _recived;

		protected string _authKey;
		
		public string AuthKey
		{
			get{return _authKey;}
		}
		
		public EventHandler DisconnectedEvent;
		
		private bool _DisconnectedEventCalled;

		public bool Connected
		{
			get { return _client == null ? false : _client.Connected; }
		}
		public Connection(string dest, int port)
		{
			_client = new TcpClient();
			_client.Connect(dest, port);
			Init();
		}
		public Connection(TcpClient connection)
		{
			_client = connection;
			Init();
		}
		
		private void Init()
		{
			_sr = new BinaryReader(_client.GetStream());
			_sw = new BinaryWriter(_client.GetStream());
			_DisconnectedEventCalled = false;
			_authKey = "";
		}
		
		
		public void Poll()
		{
			if(!Connected)
			{
				Disconnected();
				return;
			}
			Poll_Read();
			Poll_Write();
			
			
		}
		
		private void Poll_Read()
		{
			//If we are waiting for data.
			if(_length >= _client.Available)
			{
				//if its been more than a second, call a sync error.
				//Snipits of data should not take more than a second to recive.
				if(_recived + TimeSpan.FromMilliseconds(1000) >= DateTime.Now)
				{
					SyncError();
				}
				return;
			}
			//if enough data is avalable to read the ushort
			if (_client.Available > 2)
			{
				//If its 0, then we are not waiting.
				//If it isn't 0, then we are waiting for data, and reading
				//bytes will break stuff.
				if(_length == 0) _length = _sr.ReadUInt16();
				//If _length was set earlier, we assume its long enough
				//otherwise it would of failed the test earlier.
				if(_client.Available < _length)
				{
					//if there isn't enough data, go on.
					_recived = DateTime.Now;
					return;
				}
				
				ushort pTempID = _sr.ReadUInt16();
				//remove 2 from the length that we just read from
				HandleInput(Convert.ToUInt16(_length - 2), pTempID);
			}
			_length = 0;
		}
		
		private void Poll_Write()
		{
			lock(_packetQue)
			{
				if(_packetQue.Count > 0)
				{
					Packet p = _packetQue.Dequeue();
					_sw.Write(p.Data);
				}
			}
		}
		
		private void AddPacket(byte[] data, int priority)
		{
			lock(_packetQue)
			{
				_packetQue.Enqueue(new Packet(data, priority));
			}
		}

		protected void HandleInput(ushort length, ushort packetID)
		{
			PacketID pID = PacketID.Null;
			if(!pID.TryParse(packetID))
			{
				SyncError();
				return;
			}

			//Switch through all of the items, even if we need to throw a SyncError.
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
				case PacketID.Message:
					Read_Message();
					break;
				case PacketID.ModulePacket:
					Read_ModulePacket(length);
					break;
			}
		}


		void Read_Message()
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
			if(MessageEvent != null)
				MessageEvent(this, new MessageEventArgs(mID));
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
			ByteReader data = new ByteReader(_sr, length - 2);
			_moduleActions[moduleID].Communication(this, data);
		}
		
		void Write_ModulePacket(byte[] data, ushort moduleID)
		{
			Write_ModulePacket(data, moduleID, 0);
		}
		
		void Write_ModulePacket(byte[] data, ushort moduleID, int priority)
		{
			//2 for ID, 2 for module ID, x for data length
			ushort length = Convert.ToUInt16(4 + data.Length);
			ByteWriter bw = new ByteWriter();

			bw.Write(length);
			bw.Write(PacketID.ModulePacket.Value());
			bw.Write(moduleID);
			bw.Write(data);
			AddPacket(bw.GetArray(), priority);
		}
		
		public void Write_Message(MessageID reason)
		{
			//2 for ID, 2 for message ID
			ushort length = 4;
			ByteWriter bw = new ByteWriter();

			bw.Write(length);
			bw.Write(PacketID.Message.Value());
			bw.Write(reason.Value());
			
			//Messages are generally important, so slight priority change.
			AddPacket(bw.GetArray(), -1);
		}
		
		/// <summary>
		/// Disconnects the client without any reason.
		/// </summary>
		public void Disconnect()
		{
			_client.Close();
			Disconnected();
		}
		
		private void Disconnected()
		{
			if(Connected || _DisconnectedEventCalled) return; //UUh, we are not disconnected or this has been called already.
			if(DisconnectedEvent != null)
				DisconnectedEvent(this, EventArgs.Empty);
			_DisconnectedEventCalled = true;
		}

		/// <summary>
		/// Disconnects the client with the specified reason.
		/// </summary>
		public void Disconnect(MessageID reason)
		{
			Write_Message(reason);
			Disconnect();
		}
		
		
		/// <summary>
		/// Calls a Sync Error, Usually when the reciving datastream contains data the program isn't expecting.
		/// </summary>
		public  void SyncError()
		{
			SyncError(new Dictionary<String, Object>());
		}
		
		public  void SyncError(string debugData)
		{
			Dictionary<String, Object> d = new Dictionary<String, Object>();
			d.Add("Debug",debugData);
			SyncError(new Dictionary<String, Object>());
		}
		
		public  void SyncError(Dictionary<String, Object> data)
		{
			StackTrace stackTrace = new StackTrace();
			
			if(System.Diagnostics.Debugger.IsAttached)
				System.Diagnostics.Debugger.Break();
			else
			{
				System.Diagnostics.Debug.WriteLine(String.Format("SyncError!"));
				foreach(var kvp in data)
					System.Diagnostics.Debug.WriteLine(String.Format("{0} = {1}", kvp.Key, kvp.Value));
				
				System.Diagnostics.Debug.WriteLine("Stack:");
				System.Diagnostics.Debug.WriteLine(stackTrace.ToString());
			}

			Write_Message(MessageID.SyncError);
			Disconnect();
		}

	}
}
