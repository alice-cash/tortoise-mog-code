/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 7/20/2010
 * Time: 5:21 PM
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
using Client.Connections;
using Shared.Connections;

namespace Tortoise.Client.Connections
{
	//This part of the class deals with methods to read data from the server.
	partial class ServerConnection
	{
		
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
		
		
		
	}
}