using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise.Shared.Threading
{
    [Flags]
    public enum ThreadSafeFlags
    {
        ThreadSafe,
        ThreadSafeEnforced,
        ThreadUnsafe
    }
}
