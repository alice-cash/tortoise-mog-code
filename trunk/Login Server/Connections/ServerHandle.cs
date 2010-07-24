/*
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

        private ServerHandle()
        {
            _clients = new List<ServerConnection>();
            _requests = new Queue<TcpClient>();
            _threadRunning = true;

            _instance = this;
        }

        public static void CreateInstance()
        {
            if (Instance != null)
                return;
            _instance = new ServerHandle();
        }

        public void EnqueConnection(TcpClient Client)
        {
            lock (_requests)
            {
                _requests.Enqueue(Client);
            }

        }

        private void WorkThread()
        {
            while (_threadRunning == true)
            {
                lock (_requests)
                {
                    if (_requests.Count > 0)
                    {
                        TcpClient Request = _requests.Dequeue();
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
                }
                _clients.ForEach((ServerConnection sc) =>
                {
                    if (!sc.Connected) _clients.Remove(sc);
                });
                foreach (var c in _clients)
                {
                    c.Poll();
                }
            }
        }
    }
}
