/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 7/18/2010
 * Time: 4:00 PM
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
using Shared.Connections;

using SharedServerLib.Exceptions;

namespace LoginServer.Connections
{
	//Handles writing to the client
	partial class ClientConnection
	{
		
		
		public void Write_ServerMessage(MessageID reason)
		{
			//2 for ID, 2 for message ID
			ushort length = 4;
			_sw.Write(length);
			_sw.Write(PacketID.ServerMessage.Value());
			_sw.Write(reason.Value());
			_sw.Flush();
		}
		
		public void Write_TempAuthKey(string key)
		{
			//this should never occure, but if it does, its mostlikley due to a horrible,
			//horrable bug, and it should bring the server down and burn their house down.
			//This still needs to be here due to the explicid conversion and my coding rules for this project.
			if(key.Length + 2 >= ushort.MaxValue)
				throw new TortoiseGeneralException("Invalid key length!");
			
			ushort length = (ushort)(2 + key.Length);
			_sw.Write(length);
			_sw.Write(PacketID.Authintication.Value());
			_sw.Write(key);
			_sw.Flush();
		}
		
		private void Write_LoginSucess(bool status)
		{
			//2 for ID, 1 for bool
			ushort length = 3;
		    _sw.Write(length);
		    _sw.Write(PacketID.LoginSucess.Value());
			_sw.Write(status);
			_sw.Flush();
		}
	}							
}
