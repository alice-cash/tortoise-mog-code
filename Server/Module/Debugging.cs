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
            TConsole.SetValue("Debugging_Level", new ConsoleVarable() { ValidCheck = CheckConsoleInput, Value = "0", HelpInfo = Shared.Localization.DefaultLanguage.Strings.GetString("DEBUGGING_LEVEL_HELP") });

            Shared.TConsole.SetFunc("debug_db_grabusr", new ConsoleFunction()
            {
                Function = new Func<string[], Shared.ConsoleResponce>((string[] debug_args) =>
                    {
                        if (debug_args.Length == 0 || debug_args.Length > 1)
                            return Shared.ConsoleResponce.NewFailure(Shared.Localization.DefaultLanguage.Strings.GetString("DEBUG_DB_GRABUSR_HELP"));
                        if (debug_args.Length == 1)
                        {
                            var v = Data.Tables.account.GetAccountByUsername(debug_args[0]);
                            if (v.Sucess == false)
                                return Shared.ConsoleResponce.NewFailure(Shared.Localization.DefaultLanguage.Strings.GetFormatedString("DEBUG_DB_GRABUSR_NOFIND", debug_args[0]));

                            return Shared.ConsoleResponce.NewSucess(Shared.Localization.DefaultLanguage.Strings.GetFormatedString("DEBUG_DB_GRABUSR_FIND", v.Result.ID, v.Result.Username, v.Result.Password));
                        }
                        else
                        {

                        }

                        return Shared.ConsoleResponce.NewSucess("");
                    }),
                HelpInfo = Shared.Localization.DefaultLanguage.Strings.GetString("DEBUG_DB_GRABUSR_HELP")
            });
        
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
