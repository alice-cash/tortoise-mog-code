using System;
using Tortoise.Shared.Net;
using Tortoise.Shared.IO;
using StormLib.Module;
using Tortoise.Client.Net;
using Tortoise.Client.Exceptions;

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Tortoise.Client.Net
{
	public static class ServerConnection
    {
		public static Connection MainServerConnection;

        public static void Init()
        {
            _threadRunning = true;
			
			_listenThread = new Thread(WorkThread);
			_listenThread.Start();
        }

        private static Thread _listenThread;
        private static bool _threadRunning;



        private static void WorkThread()
		{
            while (Program.ThreadsRunning)
			{
                if (MainServerConnection != null && MainServerConnection.Connected)
                {
                    MainServerConnection.Poll();
                }
                Thread.Sleep(0);

			}

		}
    }
}
