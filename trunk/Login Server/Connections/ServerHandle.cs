﻿/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 5/6/2010
 * Time: 12:38 PM
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
using System.Net;
using System.Net.Sockets;
using System.Linq;


using LoginServer.XML;

namespace LoginServer.Connections
{
	/// <summary>
	/// Description of ServerHandle.
	/// </summary>
	public class ServerHandle
	{
		private static ServerHandle _instance;
		public static ServerHandle Instance
		{
			get { return _instance; }
		}

		private List<ServerConnection> _clients;
		private Queue<TcpClient> _requests;
		private bool _threadRunning;
		
		private Thread _listenThread;
		private bool _threadRunning;
		private TcpListener _listiner;
		private TcpListener _secondaryListiner;
		private bool _secondaryListinerActive;
		
		private ServerHandle()
		{
			_clients = new List<ServerConnection>();
			_requests = new Queue<TcpClient>();

			_listenThread = new Thread(WorkThread);
			_listenThread.Start();
			_secondaryListinerActive = false;
			
			_instance = this;
		}

		public static void CreateInstance()
		{
			if (Instance != null)
				return;
			_instance = new ServerHandle();
		}


		private void WorkThread()
		{
			//If the Listen address is IPv6Any, then we possibly need to create a second listiner for IPv4
			if (LoginServerConfig.Instance. == IPAddress.IPv6Any)
			{
				_secondaryListinerActive = true;
				_secondaryListiner = new TcpListener(IPAddress.Any, LoginServerConfig.Instance.ServerListenPort);
				_secondaryListiner.Start();
			}
			_listiner = new TcpListener(LoginServerConfig.Instance.ConvertedClientListenAddress, LoginServerConfig.Instance.ServerListenPort);
			_listiner.Start();
			
			_threadRunning = true;
			while (_threadRunning)
			{
				if (_listiner.Pending() || (_secondaryListinerActive && _secondaryListiner.Pending()))
				{
					//If the main listiner didn't trigger this, then it has to have been the secondary one.
					TcpClient Request = _listiner.Pending() ? _listiner.AcceptTcpClient() : _secondaryListiner.AcceptTcpClient();
					ServerConnection Conn = new ServerConnection(Request);

					//Check that its an accepted IP
					if (!LoginServerConfig.Instance.AcceptAnyAddress)
					{
						foreach (var address in LoginServerConfig.Instance.ConvertedAcceptedServerAddresses)
						{
							if (((IPEndPoint)Request.Client.RemoteEndPoint).Address.Equals(address))
							{
								_clients.Add(Conn);
							}
						}
					}
					else
					{
						_clients.Add(Conn);
					}
				}
				
				_clients.ForEach((ServerConnection sc) =>
				                 {
				                 	if (!sc.Connected)
				                 	{
				                 		sc.Disconnected();
				                 		_clients.Remove(sc);
				                 	}
				                 });
				foreach (var c in _clients)
				{
					c.Poll();
				}
			}
		}
		
		private void AcceptConnection(TcpClient client)
		{
			ServerConnection Conn = new ServerConnection(client);
			_clients.Add(Conn);
		}
	}
}
