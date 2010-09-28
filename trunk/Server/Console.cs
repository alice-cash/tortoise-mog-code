using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tortoise.Server
{
    class Console
    {
        public static void WriteLine()
        {
            WriteLine("");
        }
        public static void WriteLine(string Line)
        {
            WriteLine("", "");
        }
        public static void WriteLine(string line, params string[] args)
        {
            System.Diagnostics.Trace.WriteLine(string.Format(line, args));
        }
        public static void WriteLine(string line, object arg)
        {
            System.Diagnostics.Trace.WriteLine(string.Format(line, arg));
        }
        public static void WriteLine(string line, params object[] args)
        {
            System.Diagnostics.Trace.WriteLine(string.Format(line, args));

        }
    }
}
