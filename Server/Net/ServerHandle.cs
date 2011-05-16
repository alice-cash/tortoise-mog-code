/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 5/6/2010
 * Time: 12:38 PM
 * 
 * Copyright 2011 Matthew Cash. All rights reserved.
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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Tortoise.Server.XML;
using Tortoise.Shared.Net;

namespace Tortoise.Server.Connections
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

		private List<Connection> _clients;
		private Queue<Socket> _requests;
		
		private Thread _listenThread;
		private bool _threadRunning;
		private TcpListener _listiner;
		private TcpListener _secondaryListiner;
		private bool _secondaryListinerActive;
		
		private ServerHandle()
		{
			_clients = new List<Connection>();
			_requests = new Queue<Socket>();

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
			//If the Listen address is IPv6Any, then we possibly need to create a second listener for IPv4
			if (ServerConfig.Instance.ConvertedServerListenAddress == IPAddress.IPv6Any)
			{
				_secondaryListinerActive = true;
				_secondaryListiner = new TcpListener(IPAddress.Any, ServerConfig.Instance.ServerListenPort);
				_secondaryListiner.Start();
			}
			_listiner = new TcpListener(ServerConfig.Instance.ConvertedServerListenAddress, ServerConfig.Instance.ServerListenPort);
			_listiner.Start();
			
			_threadRunning = true;
			while (_threadRunning)
			{
				if (_listiner.Pending() || (_secondaryListinerActive && _secondaryListiner.Pending()))
				{
					//If the main listener didn't trigger this, then it has to have been the secondary one.
                    Socket Request = _listiner.Pending() ? _listiner.AcceptSocket() : _secondaryListiner.AcceptSocket();
					Connection Conn = new Connection(Request);

					//Check that its an accepted IP
					if (!ServerConfig.Instance.AcceptAnyAddress)
					{
						foreach (var address in ServerConfig.Instance.ConvertedAcceptedServerAddresses)
						{
							if (((IPEndPoint)Request.RemoteEndPoint).Address.Equals(address))
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
				
				_clients.ForEach((Connection sc) =>
				                 {
				                 	if (!sc.Connected)
				                 	{
				                 		_clients.Remove(sc);
				                 	}
				                 });
				foreach (var c in _clients)
				{
					c.Poll();
				}
			}
		}

        private void AcceptConnection(Socket client)
		{
			Connection Conn = new Connection(client);
			_clients.Add(Conn);
			Conn.DisconnectedEvent += delegate(object sender, EventArgs e)
			{
				//If its not a sender item, then it will be null
				//and a null should "NOT" be in the clients list
				if(_clients.Contains(sender as Connection))
					_clients.Remove(sender as Connection);
			};
		}
	}
}
