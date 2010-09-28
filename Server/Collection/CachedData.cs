using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tortoise.Server.Collection
{
    abstract class CachedData<T>
    {
        protected DateTime _lastUse;
        protected bool _indefinite;
        public bool Loaded { get; private set; }
        protected T _value;
        public virtual T Value
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

        public abstract void Load();
        public abstract void Unload();
        public abstract void Save();

    }
}
