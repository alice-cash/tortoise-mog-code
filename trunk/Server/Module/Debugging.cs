using System;
using System.Collections.Generic;
using Tortoise.Shared.Net;
using Tortoise.Shared.Module;
using Tortoise.Shared;

namespace Tortoise.Server.Module
{
    class Debugging : ModuleLoader
    {
        public static void SyncError(Connection Sender, Dictionary<String, Object> ErrorLog0, Dictionary<String, Object> ErrorLog1, Dictionary<String, Object> ErrorLog2)
        {
            string level = TConsole.GetValue("Debugging_Level").Value;
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

        public static void LogError(Connection Sender, Dictionary<String, Object> ErrorLog0, Dictionary<String, Object> ErrorLog1, Dictionary<String, Object> ErrorLog2)
        {

        }

        public override Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }

        public override string Name
        {
            get { return "Debugging"; }
        }

        public override void Load()
        {
            TConsole.SetValue("Debugging_Level", new ConsoleVarable() { ValidCheck = CheckConsoleInput, Value = "0" });
        }

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
