using System;
using System.Linq;
using Tortoise.Shared;

namespace Tortoise.Server
{
    static class ObjectExtension
    {
        public static ExecutionState<object> Select(this object source, object obja)
        {
            if (source == obja)
                return new ExecutionState<object>(true, source);
            return ExecutionState<object>.Failed();
        }

        public static ExecutionState<object> Select(this object source, object obja, object objb)
        {
            if (source.Equals(obja) || source.Equals(objb))
                return new ExecutionState<object>(true, source);
            return ExecutionState<object>.Failed();
        }

        public static ExecutionState<object> Select(this object source, params object[] objs)
        {
            if (objs.Contains(source))
                return new ExecutionState<object>(true, source);
            return ExecutionState<object>.Failed();
        }
    }
}
