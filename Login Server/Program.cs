/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 5/2/2010
 * Time: 12:21 AM
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
 * */
using System;

using SharedServerLib.Exceptions;
#if LINUX
using System.Threading;
using Mono.Unix;
#endif
namespace LoginServer 
{
	class Program
	{
#if LINUX
        static UnixSignal[] signals = new UnixSignal[] {
            new UnixSignal (Mono.Unix.Native.Signum.SIGTERM)
        };

        static Thread signal_thread = new Thread(delegate()
        {
            while (true)
            {
                // Wait for a signal to be delivered
                int index = UnixSignal.WaitAny(signals, -1);

                Mono.Unix.Native.Signum signal = signals[index].Signum;
                if (signal == Mono.Unix.Native.Signum.SIGTERM)
                    RunServer = false;

            }
        });
#endif
		public static bool RunServer{get;set;}
		
		public static void Main(string[] args)
		{
			#if LINUX
			signal_thread.Start();
			#endif
			//Load up the configeration file
			try{
				XML.LoginServerConfig.LoadConfig();
			} catch(TortusMissingResourceException ex)
			{
				Console.WriteLine("The server could not load the embeded resource. Please check following embeded resource: {0}", ex.Data["ResourceName"]);
                return;
            }
            catch (TortusFileException ex)
			{
				Console.WriteLine("The server could not load or create the configeration file. More information: {0}", ex.InnerException.ToString());
                return;
            }
            catch (InvalidOperationException ex)
			{
				Console.WriteLine("An uknown error occured during deserialization. More information: {0}", ex.InnerException.ToString());
                return;
            }
			if(XML.LoginServerConfig.Instance.MysqlUser == "{EDIT ME}" ||
			   XML.LoginServerConfig.Instance.MysqlPass == "{EDIT ME}")
			{
				Console.WriteLine("Edit the LoginConfig.xml file");
                return;
			}
            if (XML.LoginServerConfig.Instance.AcceptAnyAddress)
            {
                Console.WriteLine("Warning: This server is set to accept server connections from any IP address. ANY server with the secrets can connect.");
            }
			
            //Start the various Listiners.
            LoginServer.Connections.ServerListen.CreateInstance();

            LoginServer.Connections.ClientListen.CreateInstance();
#if DEBUG
			Console.WriteLine("Press a key...");
			Console.ReadKey(true);
#endif
		}
	}
}