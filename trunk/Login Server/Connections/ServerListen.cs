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
using System.Net;
using System.Net.Sockets;
using System.Threading;

using LoginServer.XML;

namespace LoginServer.Connections
{
    /// <summary>
    /// Description of ServerListen.
    /// </summary>
    public class ServerListen
    {
        public static ServerListen _instance;
        public static ServerListen Instance
        { get { return _instance; } }

        private Thread _listenThread;
        private bool _threadRunning;
        private TcpListener _listiner;
        private TcpListener _secondaryListiner;
        private bool _secondaryListinerActive;

        private ServerListen()
        {
        	_listenThread = new Thread(Listener);
        	_listenThread.Start();
        	_secondaryListinerActive = false;
        	_threadRunning = true;

            ServerHandle.CreateInstance();
        }

        public static void CreateInstance()
        {
            if (Instance != null)
                return;
            _instance = new ServerListen();
        }

        private void Listener()
        {
            //If the Listen address is IPv6Any, then we possibly need to create a second listiner for IPv4
            if (LoginServerConfig.Instance.ConvertedServerListenAddress == IPAddress.IPv6Any)
            {
                _secondaryListinerActive = true;
                _secondaryListiner = new TcpListener(IPAddress.Any, LoginServerConfig.Instance.ServerListenPort);
                _secondaryListiner.Start();
            }
            _listiner = new TcpListener(LoginServerConfig.Instance.ConvertedServerListenAddress, LoginServerConfig.Instance.ServerListenPort);
            _listiner.Start();

            while (_threadRunning)
            {
                if (_listiner.Pending())
                {
                    ServerHandle.Instance.EnqueConnection(_listiner.AcceptTcpClient());
                }

                if (_secondaryListinerActive && _secondaryListiner.Pending())
                {
                    ServerHandle.Instance.EnqueConnection(_secondaryListiner.AcceptTcpClient());
                }
            }

        }

    }
}
