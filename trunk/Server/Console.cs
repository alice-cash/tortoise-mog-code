//using System;
//using System.Collections.Generic;
//using C5;
//using Tortoise.Shared;

//namespace Tortoise.Server
//{
//    public struct ConsoleVarable
//    {
//        public string Value;
//        public Func<string, ExecutionState> ValidCheck;
//        public override string ToString()
//        {
//            return Value;
//        }
//    }
//    class Console
//    {
//        private static HashDictionary<string, ConsoleVarable> _varables;
//        public static void WriteLine()
//        {
//            WriteLine("");
//        }
//        public static void WriteLine(string Line)
//        {
//            WriteLine("", "");
//        }
//        public static void WriteLine(string line, params string[] args)
//        {
//            System.Diagnostics.Trace.WriteLine(string.Format(line, args));
//        }
//        public static void WriteLine(string line, object arg)
//        {
//            System.Diagnostics.Trace.WriteLine(string.Format(line, arg));
//        }
//        public static void WriteLine(string line, params object[] args)
//        {
//            System.Diagnostics.Trace.WriteLine(string.Format(line, args));

//        }

//        public static ConsoleVarable GetValue(string name)
//        {
//            lock (_varables)
//                if (ValueContains(name))
//                {
//                    return _varables[name];
//                }
//            //this is called by normal code, so throw a normal exception!
//            throw new ArgumentException(string.Format("{0} does not exist", name));
//        }


//        public static void SetValue(string name, ConsoleVarable value)
//        {
//            lock (_varables)
//                if (_varables.Contains(name))
//                {
//                    _varables[name] = value;
//                }
//                else
//                {
//                    _varables.Add(name, value);
//                }
//        }

//        public static ExecutionState SetValueChecked(string name, string value)
//        {
//            lock (_varables)
//                if (_varables.Contains(name))
//                {
//                    ExecutionState result = _varables[name].ValidCheck(value);

//                    if (!result)
//                        return ExecutionState.Failed(string.Format("{0} is not valid! {1}", value, result.Reason));
//                    var tmp = _varables[name];
//                    tmp.Value = value;
//                    _varables[name] = tmp;
                        
//                }
//                else
//                {
//                    return ExecutionState.Failed(string.Format("{0} does not exists!", name));
//                }
//            return ExecutionState.Succeeded();

//        }

//        public static bool ValueContains(string name)
//        {
//            lock (_varables)
//                return _varables.Contains(name);
//        }
//    }
//}
