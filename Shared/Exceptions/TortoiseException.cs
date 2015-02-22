using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise.Shared.Exceptions
{
    public abstract class TortoiseException : Exception
    {
        public TortoiseException()
        {
        }

        public TortoiseException(string message)
            : base(message)
        {
        }

        public TortoiseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
