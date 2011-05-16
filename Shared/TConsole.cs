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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using C5;
using Tortoise.Shared;
using Tortoise.Shared.Collection;
using Tortoise.Shared.Localization;
using Tortoise.Shared.Exceptions;

namespace Tortoise.Shared
{
        public enum ConsoleCommandSucess
    {
        Sucess,
        Failure
    }
    public struct ConsoleResponce
    {
        public string Value;
        public ConsoleCommandSucess Sucess;
    }
    public struct ConsoleVarable
    {
        public string Value;
        public Func<string, ExecutionState> ValidCheck;
        public override string ToString()
        {
            return Value;
        }
    }
    /// <summary>
    /// This is the backend for the Console system.
    /// </summary>
    public static class TConsole
    {

        private static HashDictionary<string, ConsoleVarable> _varables;
        private static HashDictionary<string, System.Func<string[], ConsoleResponce>> _functions;

        private static LimitedList<string> _consoleBacklog;
        private static string _backlogLine;

        static TConsole()
        {
            Init();
        }

        public static void Init(int backlogLength = 500)
        {
            _varables = new HashDictionary<string, ConsoleVarable>();
            _functions = new HashDictionary<string, System.Func<string[], ConsoleResponce>>();
            _consoleBacklog = new LimitedList<string>(backlogLength, string.Empty);
            _backlogLine = "";
        }

        public static void WriteToBacklog(string message, bool newline)
        {
            if (!newline)
                _backlogLine += message;
            if (newline && _backlogLine == "")
                _consoleBacklog.Add(message);
            else
            {
                _consoleBacklog.Add(_backlogLine + message);
                _backlogLine = "";
            }
        }

        public static ConsoleVarable GetValue(string name)
        {
            lock (_varables)
                if (ValueContains(name))
                {
                    return _varables[name];
                }
            //this is called by normal code, so throw a normal exception!
            throw new InvalidVarableNameExceptions(string.Format("{0} does not exist", name));
        }


        public static void SetValue(string name, ConsoleVarable value)
        {
            lock (_varables)
                if (_varables.Contains(name))
                {
                    _varables[name] = value;
                }
                else
                {
                    _varables.Add(name, value);
                }
        }

        public static bool ValueContains(string name)
        {
            lock (_varables)
                return _varables.Contains(name);
        }

        public static ConsoleResponce ExecuteFunc(string name, params string[] args)
        {
            lock (_varables)
            {
                ConsoleResponce cr = new ConsoleResponce();
                cr.Sucess = ConsoleCommandSucess.Failure;
                cr.Value = "";
                try
                {
                    if (_functions.Contains(name))
                    {
                        return _functions[name](args);
                    }
                }
                catch (ConsoleException ex)
                {
                    cr.Value = DefaultLanguage.Strings.GetFormatedString("Console_Function_Exception", ex);
                }
                return cr;
            }
        }



        public static void SetFunc(string name, System.Func<string[], ConsoleResponce> func)
        {
            lock (_varables)
                if (_varables.Contains(name))
                {
                    _functions[name] = func;
                }
                else
                {
                    _functions.Add(name, func);
                }
        }

        public static bool FuncContains(string name)
        {
            lock (_varables)
                return _functions.Contains(name);
        }

        public static ConsoleResponce ProcessLine(string line)
        {

            lock (_varables)
            {
                ConsoleResponce cr = new ConsoleResponce();
                cr.Sucess = ConsoleCommandSucess.Sucess;
                if (line == "")
                {
                    cr.Value = "";
                    return cr;
                }

                string[] split = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string name = split[0], value = "";
                Array.Copy(split, 1, split, 0, split.Length - 1);

                if (_varables.Contains(name) && split.Length == 0)
                {
                    cr.Value = string.Format("{0} = {1}\n", name, _varables[name]);
                    return cr;
                }
                value = String.Join(" ", split);
                if (_varables.Contains(name))
                {
                    if (value.StartsWith("=") || value.StartsWith("\\"))
                        value.Remove(0, 1);
                    if (_varables[name].ValidCheck(value))
                    {
                        ConsoleVarable cv = _varables[name];
                        cv.Value = value;
                        _varables[name] = cv;
                    }
                    else
                    {
                        cr.Sucess = ConsoleCommandSucess.Failure;
                        cr.Value = DefaultLanguage.Strings.GetFormatedString("Console_Validation_Failure", name, value);
                        return cr;
                    }
                }
                if (_functions.Contains(name))
                {
                    return ExecuteFunc(name, ArgSplit(value, true));
                }

                return cr;
            }
        }

        public static void ProcessFile(string fileName)
        {
            lock (_varables)
            {
                if (!File.Exists(fileName))
                    throw new FileNotFoundException("Could not find {0}", fileName);

                string line, varableName, VarableArguments;
                int pos;
                foreach (string l in File.ReadAllLines(fileName))
                {
                    line = l.Trim();
                    if (line.StartsWith("#")) continue;
                    if (!line.Contains(" ") && !line.Contains("\t")) continue;

                    //We need to stop at the first space or \t.
                    for (pos = 0; pos < line.Length; pos++)
                    {
                        if (line[pos] == ' ' || line[pos] == '\t')
                            break;
                    }

                    varableName = line.Substring(0, pos);

                    VarableArguments = line.Substring(pos);
                    VarableArguments = VarableArguments.Trim();
                    if (_varables.Contains(varableName))
                    {
                        if (_varables[varableName].ValidCheck != null && _varables[varableName].ValidCheck(VarableArguments).Sucess)
                        {
                            ConsoleVarable cv = _varables[varableName];
                            cv.Value = VarableArguments;
                            _varables[varableName] = cv;
                        }

                    }
                    else if (_functions.Contains(varableName))
                    {
                        var result = ExecuteFunc(varableName, ArgSplit(VarableArguments, true));
                        if (result.Sucess == ConsoleCommandSucess.Failure)
                            System.Diagnostics.Debug.WriteLine(result.Value);
                    }
                }

            }
        }

        private static string[] ArgSplit(string args, bool ignoreEmprt = false)
        {
            bool instring = false;
            int escape = 0;
            List<string> tmpArgs = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (char c in args)
            {
                if (escape > 0) escape--;
                if (c == ' ' && !instring)
                {
                    if (sb.Length != 0 && ignoreEmprt == true || ignoreEmprt == false)
                    {
                        tmpArgs.Add(sb.ToString());
                    }
                    sb = new StringBuilder();
                    continue;
                }
                if (c == '\\')
                {
                    escape += 2;
                    continue;
                }
                if (c == '"' && escape == 0)
                {
                    instring = !instring;
                    continue;
                }

                sb.Append(c);

            }

            if (sb.Length != 0 && ignoreEmprt == true || ignoreEmprt == false)
            {
                tmpArgs.Add(sb.ToString());
            }

            return tmpArgs.ToArray();
        }


        public static string GetBacklog(bool IncludeEmptyLines = true)
        {
            StringBuilder sb = new StringBuilder(_consoleBacklog.Limit);
            foreach (string s in _consoleBacklog)
            {
                if (!IncludeEmptyLines && s != String.Empty)
                    sb.AppendLine(s);
            }
            return sb.ToString();
        }
    }
}
