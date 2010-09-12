using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tortoise.Server.Collection
{
    abstract class CachedData<T>
    {
        DateTime _lastUse;
        public bool Loaded { get; private set; }
        private T _value;
        public T Value
        {
            get
            {
                if (!Loaded) Load();
                return _value;
            }
            set
            {
                if (!Loaded) Load();
                _lastUse = DateTime.Now;
                _value = value;
            }
        }


        public virtual void Poll()
        {
            if ((DateTime.Now - _lastUse).TotalSeconds > 60)
            {
                Unload();
            }
        }

        public virtual void Load();
        public virtual void Unload();

    }
}
