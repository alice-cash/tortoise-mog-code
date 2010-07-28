﻿/*
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
using Shared.Connections;
using System.Diagnostics;

namespace Client.Connections
{
	/// <summary>
	/// 
	/// </summary>
	partial class ServerConnection
	{
		private TcpClient _client;
		private BinaryReader _sr;
		private BinaryWriter _sw;
		
		private int _length;
		private DateTime _recived;


		private string _authKey = "";
		
		private bool _readyForData;
		public bool ReadyForData
		{
			get{return _readyForData;}
		}
		
		public System.EventHandler<ServerMessageEventArgs> ServerMessageEvent; 
		public System.EventHandler ReadyForDataEvent; 

		public ServerConnection(string dest, int port)
		{
			_client = new TcpClient();
			_client.Connect(dest, port);
			_sr = new BinaryReader(_client.GetStream());
			_sw = new BinaryWriter(_client.GetStream());
		}
		public void Poll()
		{
			//If we are waiting for data.
			if(_length > 0)
			{
				//if we still don't have it.
				if(_client.Available < _length)
		   		{
		   			//if its been more than a second, call a sync error.
		   			if(_recived + TimeSpan.FromMilliseconds(1000) >= DateTime.Now)
		   			{
		   				Disconnect(MessageID.SyncError);
		   			}
		   			return;
		   		}
			
			}
			//if enough data is avalable to read the ushort
		   if (_client.Available > 2)
		   {
		   		_length = _sr.ReadUInt16();
		   		if(_client.Available < _length)
		   		{
		   			//if theres not enough data, go on.
		   			_recived = DateTime.Now;
		   			return;
		   		}

				//Make sure its a valid Enum Number		   		
		   		ushort pTempID = _sr.ReadUInt16();
		   		PacketID pID = PacketID.Null;
		   		if(!pID.TryParse(pTempID))
		   		{
		   			Disconnect(MessageID.SyncError);
		   			return;
		   		}

		   		//Switch through all of the items, even if we throw a SyncError.
		   		//Otherwise each ID should call a Read_{DescritiveInfo}()
		   		//The reason for the empty SyncError() for a release is we don't care about
		   		//reasons. We can assume the end developer has 
		   		#if DEBUG
		   		Dictionary<String, Object> debugData;
		   		#endif
		   		switch(pID)
		   		{
		   			case PacketID.Null:
		   				debugData = new Dictionary<String, Object>();
		   				debugData.Add("PacketID", PacketID.Null);
		   				SyncError(debugData);

		   				break;
		   			case PacketID.Authintication: Read_AuthKey();
		   				break;
		   			case PacketID.ClientInfo:
		   				debugData = new Dictionary<String, Object>();
		   				debugData.Add("PacketID", PacketID.ClientInfo);
		   				SyncError(debugData);

		   				break;
		   			case PacketID.ServerMessage: Read_ServerMessage();
		   				break;
		   		}
		   		
		   }
		   
		   _length = 0;
		}
		
		public void Disconnect(MessageID reason)
		{
			Write_ServerMessage(reason);
			_client.Close();					
		}
		

		/// <summary>
		/// Calls a Sync Error, Usually when the reciving datastream contains data the program isn't expecting.
		/// </summary>
		public void SyncError()
		{
			SyncError(new Dictionary<String, Object>());
		}
		
		public void SyncError(Dictionary<String, Object> data)
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

			Disconnect(MessageID.SyncError);
		}
		
		
	}
}
