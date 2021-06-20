using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tortoise.Server.Collection
{
    abstract class CachedData<T>
    {
        protected DateTime _lastUse;
        protected TimeSpan _timeout;
        protected bool _indefinite;
        public bool Loaded { get; private set; }
        public bool Indefinite { get { return _indefinite; } set { _indefinite = value; } }
        public TimeSpan Timeout { get { return _timeout; } set { _timeout = value; } }

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
            if (_indefinite) return;

            if ((DateTime.Now - _lastUse) > _timeout)
            {
                Unload();
            }
        }

        public abstract void Load();
        public abstract void Unload();
        public abstract void Save();

    }
}
