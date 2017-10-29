﻿/*
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

/*
 * File: Tortoise.Client.Module.Debugging.cs
 * Description : Provides Logging and Debugging information which can be relayed to the remote server. 
 * Dependencies: None
 * Console Variables: 
 *     Debugging_Level
 * Console Functions: None
 */

using System;
using System.Collections.Generic;
using Tortoise.Shared.Net;

using Tortoise.Shared;
using Console = StormLib.Console;
using StormLib;
using StormLib.Module;

namespace Tortoise.Client.Module
{
    class Debugging : IModuleLoader
    {

        /// <summary>
        /// Relays Sync errors to the remote server and disconnects from the server. 3 levels of data are supplied and one is chosen based on the current debugging level.
        /// </summary>
        /// <param name="Sender">Network connection reference</param>
        /// <param name="ErrorLog0">Debugging Data used when Debugging is set to 0</param>
        /// <param name="ErrorLog1">Debugging Data used when Debugging is set to 1</param>
        /// <param name="ErrorLog2">Debugging Data used when Debugging is set to 2</param>
        public static void SyncError(Connection Sender, Dictionary<String, Object> ErrorLog0, Dictionary<String, Object> ErrorLog1, Dictionary<String, Object> ErrorLog2)
        {
            string level = Console.GetValue("Debugging_Level").Value;
            switch (level)
            {
                default:
                case "0":
                    Sender.SyncError(ErrorLog0);
                    break;
                case "1":
                    Sender.SyncError(ErrorLog1);
                    break;
                case "2":
                    Sender.SyncError(ErrorLog2);
                    break;
            }
        }

        /// <summary>
        /// Relays Errors to the remote server. 3 levels of data are supplied and one is chosen based on the current debugging level.
        /// </summary>
        /// <param name="Sender">Network connection reference</param>
        /// <param name="ErrorLog0">Debugging Data used when Debugging is set to 0</</param>
        /// <param name="ErrorLog1">Debugging Data used when Debugging is set to 1</param>
        /// <param name="ErrorLog2">Debugging Data used when Debugging is set to 2</param>
        public static void LogError(Connection Sender, Dictionary<String, Object> ErrorLog0, Dictionary<String, Object> ErrorLog1, Dictionary<String, Object> ErrorLog2)
        {

        }

        public Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }

        public string Name
        {
            get { return "Debugging"; }
        }

        public void Load()
        {
            Console.SetValue("Debugging_Level", new ConsoleVarable() { ValidCheck = CheckConsoleInput, Value = "0" });
        }

        /// <summary>
        /// Verify the input for Debugging_Level is a valid level.
        /// </summary>
        /// <param name="input">Entered level</param>
        /// <returns>Returned status regarding success or failure of input</returns>
        ExecutionState CheckConsoleInput(string input)
        {
            input = input.Trim();
            switch (input)
            {
                case "0":
                case "1":
                case "2":
                    return ExecutionState.Succeeded();
                default:
                    return ExecutionState.Failed("Input must be 0, 1, or 2");
            }
        }
    }
}
