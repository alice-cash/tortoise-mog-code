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



namespace Tortoise.Shared.Connections
{
	/// <summary>
	/// Base connection for conitivity between 2 hosts.
	/// </summary>
	abstract class Connection
	{
		internal static Dictionary<ushort, IModule> _moduleActions = new Dictionary<ushort, IModule>();
		public static void AddModuleHandle(ushort ID,IModule module)
		{
			if(_moduleActions.ContainsKey(ID))
				throw new Exception("ID already exsists!");
			_moduleActions.Add(ID, module);
		}
		
		
		internal TcpClient _client;
		internal BinaryReader _sr;
		internal BinaryWriter _sw;
		//private byte[] _data;
		internal ushort _length;
		internal DateTime _recived;

		internal string _authKey;
		
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
			//If we are waiting for data.
			if(_length > 0)
			{
				//if we still don't have it.
				if(_client.Available < _length)
				{
					//if its been more than a second, call a sync error.
					if(_recived + TimeSpan.FromMilliseconds(1000) >= DateTime.Now)
					{
						SyncError();
					}
					return;
				}
				
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
		
		/// <summary>
		/// When overriden in a base class, handles input from the Connection.
		/// </summary>
		/// <param name="packetID"></param>
		internal abstract void HandleInput(ushort length, ushort packetID);
		
		/// <summary>
		/// Disconnects the client without any reason.
		/// </summary>
		public virtual void Disconnect()
		{
			_client.Close();
		}	
		
		private void Disconnected()
		{
			if(Connected || _DisconnectedEventCalled) return; //UUh, we are not disconnected or this has been called already.
			if(DisconnectedEvent != null)
				DisconnectedEvent(this, EventArgs.Empty);
			_DisconnectedEventCalled = true;
		}

		/// <summary>
		/// Calls a Sync Error, Usually when the reciving datastream contains data the program isn't expecting.
		/// </summary>
		public virtual void SyncError()
		{
			SyncError(new Dictionary<String, Object>());
		}
		
		public virtual void SyncError(Dictionary<String, Object> data)
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

			Disconnect();
		}

	}
}
