/*
 * Copyright 2014 Matthew Cash. All rights reserved.
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
using System.Drawing;
using System.Diagnostics;
using Tortoise.Client;
using Tortoise.Client.Module;
using Tortoise.Client.Net;
using Tortoise.Shared.Module;
using Tortoise.Shared.Localization;
using Tortoise.Shared.Diagnostics;
using System.Reflection;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

using GorgonLibrary;
using GorgonLibrary.Diagnostics;
using GorgonLibrary.IO;
using GorgonLibrary.Math;
using GorgonLibrary.Renderers;
using GorgonLibrary.UI;
using SlimMath;


using System.Runtime.InteropServices;


using Tortoise.Client.Screen;
using Tortoise.Shared;
using Tortoise.Graphics;

namespace Tortoise.Client
{
    /// <summary>
    /// Entry point to the application.
    /// </summary>
    static class Program
    {


        /// <summary>
        /// Return the application name.
        /// </summary>
        public static string GameName { get { return "Tortoise Demo"; } }

        /// <summary>
        /// Determin if threads are running. Any threads should run when this is true.
        /// When set false all threads should cease execution. You should only set when you
        /// wish to kill the appliication.
        /// </summary>
        public static bool ThreadsRunning { get; set; }

        /// <summary>
        /// Thread which houses our console reader.
        /// </summary>
        private static System.Threading.Thread ConsoleThread;


        /// <summary>
        /// Determin if the console is visisble.
        /// </summary>
        private static bool _consoleVisible;

        /// <summary>
        /// Stores the application's primary MainForm instance.
        /// </summary>
        private static MainForm mainForm;

        /// <summary>
        /// Stores the application's primary Game instance.
        /// </summary>
        private static Game _gameLogic;

        /// <summary>
        /// Retrive the application's primary Game instance.
        /// </summary>
        public static Game GameLogic { get { return _gameLogic; } }

        /// <summary>
        /// Primary entry point of the application.
        /// </summary>
        static void Main(string[] args)
        {
            _consoleVisible = false;

#if DEBUG
            _consoleVisible = true;
#endif

            foreach (string arg in args)
                if (string.Compare(arg, "-c", true) == 0 ||
                    string.Compare(arg, "--console", true) == 0)
                    _consoleVisible = true;


            if (_consoleVisible)
            {
                ShowConsoleWindow();
            }


            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Initialize();

                Gorgon.Run(mainForm, _gameLogic.GameLoop);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                _gameLogic.CleanUp();
            }

        }

        /// <summary>
        /// Function to initialize the application.
        /// </summary>
        private static void Initialize()
        {

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

            mainForm = new MainForm();
            _gameLogic = new Game(mainForm);

        }


        /// <summary>
        /// Start reading the console. This should be put into its own thread.
        /// </summary>
        private static void ConsoleReader()
        {

            while (ThreadsRunning)
            {
                if (_consoleVisible)
                {
                    Console.Write("$$>");
                    string line = Console.ReadLine();
                    var responce = Tortoise.Shared.TConsole.ProcessLine(line);
                    Console.WriteLine(responce.Value);
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Return the applicaion's version.
        /// </summary>
        public static Version Version
        {
            get
            {
                return typeof(Program).Assembly.GetName().Version;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;


        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
            _consoleVisible = true;

        }

        public static void HideConsoleWindow()
        {
            _consoleVisible = false;
            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);
        }

    }

}
