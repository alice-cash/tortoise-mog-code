/*
 * Copyright 2012 Matthew Cash. All rights reserved.
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
using System.Diagnostics;
using Tortoise.Server.Exceptions;
using StormLib.Module;
using System.Reflection;


#if LINUX
using System.Threading;
using Mono.Unix;
#endif
namespace Tortoise.Server
{
    class Program
    {
        public static bool DEBUG;
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

        public static bool RunServer { get; set; }

        public static void Main(string[] args)
        {

#if DEBUG
            DEBUG = true;
#else
			DEBUG = false;
#endif
#if LINUX
			signal_thread.Start();
#endif
            StormLib.Console.Init();

            //add the diagnostic debug output to console.
            Debug.Listeners.Add(new ConsoleTraceListener());
            Debug.Listeners.Add(new StormLib.Diagnostics.ConsoleTraceListiner());
            //Trace.Listeners.Add(new ConsoleTraceListener());
            //Trace.Listeners.Add(new TortoiseConsoleTraceListiner());
            //Load up the configuration file
            try
            {
                XML.ServerConfig.LoadConfig();
            }
            catch (TortoiseMissingResourceException ex)
            {
                Debug.WriteLine(string.Format("The server could not load the embedded resource. Please check following embedded resource: {0}", ex.Data["ResourceName"]));
                return;
            }
            catch (TortoiseFileException ex)
            {
                Debug.WriteLine(string.Format("The server could not load or create the configuration file. More information: {0}", ex.InnerException.ToString()));
                return;
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(string.Format("An unknown error occurred during deserialization. More information: {0}", ex.InnerException.ToString()));
                return;
            }
            if (XML.ServerConfig.Instance.MysqlUser == "{EDIT ME}" ||
               XML.ServerConfig.Instance.MysqlPass == "{EDIT ME}")
            {
                Debug.WriteLine("Edit the LoginConfig.xml file");
               // return;
            }
            if (XML.ServerConfig.Instance.AcceptAnyAddress)
            {
                Debug.WriteLine("Warning: This server is set to accept server connections from any IP address. ANY server with the secrets can connect.");
            }

            
            

            ModuleInfo.LoadModules(Assembly.GetExecutingAssembly(), true);

            //Start the various Listeners.
            Server.Connections.ServerHandle.CreateInstance();

            Server.Connections.ClientHandle.CreateInstance();


            while (true)
            {
                Console.Write("$$>");
                Console.WriteLine(StormLib.Console.ProcessLine(Console.ReadLine()).Value);
            }
        }

        public static Version Version
        {
            get
            {
                return typeof(Program).Assembly.GetName().Version;
            }
        }
    }
}