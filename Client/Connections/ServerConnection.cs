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
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
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


		private string _authKey;
		
		public System.EventHandler<ServerMessageEventArgs> ServerMessageEvent; 

		public ServerConnection(string dest, int port)
		{
			_client = new TcpClient();
			_client.Connect(dest, port);
			_sr = new BinaryReader(_client.GetStream());
			_sw = new BinaryWriter(_client.GetStream());
		}
		public void Poll()
		{
			if(_length > 0)
			{
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
		   if (_client.Available > 4)
		   {
		   		_length = _sr.ReadUInt16();
		   		if(_client.Available < _length)
		   		{
		   			//if theres not enough data, go on.
		   			_recived = DateTime.Now;
		   			return;
		   		}
		   		
		   		ushort pTempID = _sr.ReadUInt16();
		   		PacketID pID = PacketID.Null;
		   		if(!pID.TryParse(pTempID))
		   		{
		   			Disconnect(MessageID.SyncError);
		   			return;
		   		}

		   		switch(pID)
		   		{
		   			case PacketID.Null:
		   				break;
		   			case PacketID.Authintication: Read_AuthKey();
		   				break;
		   			case PacketID.ClientInfo:
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
	}
}
