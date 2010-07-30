/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 5/6/2010
 * Time: 6:26 PM
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
using Shared.Connections;

namespace LoginServer.Connections
{
	/// <summary>
	/// Description of ServerConnection.
	/// </summary>
	class ServerConnection : Connection
	{

		public ServerConnection(TcpClient connection) : base(connection)
		{

		}
		internal override void HandleInput(ushort packetID)
		{
			//Make sure its a valid Enum Number
			PacketID pID = PacketID.Null;
			if(!pID.TryParse(packetID))
			{
				SyncError();
				return;
			}

			//Switch through all of the items, even if we throw a SyncError.
			//Otherwise each method should call a Read_{DescritiveInfo}()
			Dictionary<String, Object> debugData;
			switch(pID)
			{
				case PacketID.Null:
					debugData = new Dictionary<String, Object>();
					debugData.Add("PacketID", PacketID.Null);
					SyncError(debugData);
					break;
				case PacketID.Authintication:
					
					break;
				case PacketID.ClientInfo:
					
					break;
			}
		}
		
		

	}
}
