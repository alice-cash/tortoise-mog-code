using System;
using System.Diagnostics;
using Tortoise.Client;
using Tortoise.Client.Module;
using Tortoise.Client.Net;
using Tortoise.Shared.Module;
using Tortoise.Shared.Localization;
using Tortoise.Shared.Diagnostics;
using System.Reflection;

namespace Tortoise.Client
{
    class Program
    {
        public const string GameName = "Tortoise Demo";

        public static bool ThreadsRunning { get; set; }

        private static System.Threading.Thread ConsoleThread;

        static void Main(string[] args)
        {
            ThreadsRunning = true;

            Debug.Listeners.Clear();
            Trace.Listeners.Clear();

            Shared.TConsole.Init();

            Debug.Listeners.Add(new ConsoleTraceListener());
            Debug.Listeners.Add(new TortoiseConsoleTraceListiner());
            //Trace.Listeners.Add(new ConsoleTraceListener());
            //Trace.Listeners.Add(new TortoiseConsoleTraceListiner());

            Trace.WriteLine(string.Format("Tortoise Version {0}.{1}.{2}.{3}", Program.Version.Major, Program.Version.Minor, Program.Version.Build, Program.Version.Revision));

            DefaultLanguage.InitDefault();
            ModuleInfo.LoadModules(Assembly.GetExecutingAssembly(), true);

            ServerConnection.Init();


            ConsoleThread = new System.Threading.Thread(ConsoleReader);
            ConsoleThread.Start();

            Screen.MainScreen MS = new Screen.MainScreen();
            MS.RunScreen();


        }

        private static void ConsoleReader()
        {

            while (ThreadsRunning)
            {
                Console.Write("$$>");
                string line = Console.ReadLine();
                var responce = Tortoise.Shared.TConsole.ProcessLine(line);
                Console.WriteLine(responce.Value);

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
