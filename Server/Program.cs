using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Server.Exceptions;
using System.Reflection;

using StormLib.Module;
using StormLib.XML;

#if LINUX
using System.Threading;
#endif
namespace Server
{
    class Program
    {
        public static bool DEBUG;
        public static bool RunServer { get; set; }
        public static IConfig LoginServerConfig;
        public static void Main(string[] args)
        {

#if DEBUG
            DEBUG = true;
#else
			DEBUG = false;
#endif
            AppDomain.CurrentDomain.ProcessExit += (s,e) => {
                RunServer = false;
            };

            StormLib.Console.Init();

            //add the diagnostic debug output to console.
            Trace.Listeners.Add(new StormLib.Diagnostics.ConsoleTraceListiner());
            //Load up the configuration file
            try
            {
                LoginServerConfig = new XML.LoginServerConfig();
                LoginServerConfig.LoadConfig();
            }
            catch (MissingResourceException ex)
            {
                Trace.WriteLine(string.Format("The server could not load the embedded resource. Please check following embedded resource: {0}", ex.Data["ResourceName"]));
                return;
            }
            catch (FileException ex)
            {
                Trace.WriteLine(string.Format("The server could not load or create the configuration file. More information: {0}", ex.InnerException.ToString()));
                return;
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(string.Format("An unknown error occurred during deserialization. More information: {0}", ex.InnerException.ToString()));
                return;
            }

            if(!LoginServerConfig.ValidateConfig()){
                return;
            }


            
            

            ModuleInfo.LoadModules(Assembly.GetExecutingAssembly(), true);

            //Start the various Listeners.
            //Server.Connections.ServerHandle.CreateInstance();

           // Server.Connections.ClientHandle.CreateInstance();


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