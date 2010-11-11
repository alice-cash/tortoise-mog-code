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
using System.Threading;

using Tortoise.Shared.Net;

namespace Tortoise.Server.Connections
{
    /// <summary>
    /// Description of ClientHandle.
    /// </summary>
    public class ClientHandle
    {
        public static ClientHandle _instance;
        public static ClientHandle Instance
        { get { return _instance; } }

        private List<Connection> _clients;

        public int ConnectedClients
        {
            get { return _clients.Count; }
        }

        //This would be very expensive, i think at least
        public ulong Last15PacketSpeed
        {
            get
            {
                ulong count = 0;
                foreach (var c in _clients)
                    count += c.Last15PacketAverage;
                return count;
            }
        }

        public ulong LastPacketSpeed
        {
            get
            {
                ulong count = 0;
                foreach (var c in _clients)
                    count += c.LastPacketSpeed;
                return count;
            }
        }

        private Thread _listenThread;
        private bool _threadRunning;
        private TcpListener _listiner;

        private ClientHandle()
        {
            _clients = new List<Connection>();
            _threadRunning = true;

            _listenThread = new Thread(WorkThread);
            _listenThread.Start();

            _instance = this;
        }

        public static void CreateInstance()
        {
            if (Instance != null)
                return;
            _instance = new ClientHandle();
        }


        private void WorkThread()
        {
            _listiner = new TcpListener(IPAddress.Parse("10.255.255.1"), 9999);
            _listiner.Start();

            _threadRunning = true;
            while (_threadRunning)
            {
                if (_listiner.Pending())
                {
                    AcceptConnection(_listiner.AcceptSocket());
                }

                foreach (var c in _clients)
                {
                    c.Poll();
                }
                Thread.Sleep(0);
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
                if (_clients.Contains(sender as Connection))
                    _clients.Remove(sender as Connection);
            };
        }

    }
}
