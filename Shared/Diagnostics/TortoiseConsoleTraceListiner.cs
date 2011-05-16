using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
 
namespace Tortoise.Shared.Diagnostics
{
    public class TortoiseConsoleTraceListiner : TraceListener
    {

        public override void Write(string message)
        {
            TConsole.WriteToBacklog(message, false);
        }

        public override void WriteLine(string message)
        {
            TConsole.WriteToBacklog(message, true);
        }
    }
}
