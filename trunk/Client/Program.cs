/*
 * Copyright 2011 Matthew Cash. All rights reserved.
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
using System.Diagnostics;
using Tortoise.Client.Rendering;
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
		static Window MainWindow = null;

        public static void Main(string[] args)
		{

            Debug.Listeners.Clear();
            Trace.Listeners.Clear();


            Debug.Listeners.Add(new ConsoleTraceListener());
            Debug.Listeners.Add(new TortoiseConsoleTraceListiner());
            //Trace.Listeners.Add(new ConsoleTraceListener());
            //Trace.Listeners.Add(new TortoiseConsoleTraceListiner());
            
			//we are cheap and we just going to set it to the first language file for now
			//TODO: Make it select based on either a default or the systems current language.
			DefaultLanguage.Strings = LanguageStrings.GetAvalableLanguages()[0];
            ModuleInfo.LoadModules(Assembly.GetExecutingAssembly(), true);

            ServerConnection.Init();
			
			MainWindow = new Window();
			//This blocks until the window is closed.
			MainWindow.Run();
		}
	}
}