using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tortoise.Graphics.Input
{
    public abstract class InputState
    {
        public abstract bool Poll();

        internal T[] GetNewItems<T>(T[] source, T[] reference)
        {
            if (reference.Length == 0)
                return new T[0];

            List<T> newKeys = new List<T>();
            foreach (T val in source)
            {
                if (!reference.Contains(val))
                    newKeys.Add(val);
            }

            return newKeys.ToArray();
        }

        internal T[] GetMissingItems<T>(T[] source, T[] reference)
        {
            if (reference.Length == 0)
                return new T[0];

            List<T> newKeys = new List<T>(reference);
            foreach (T val in reference)
            {
                if (!source.Contains(val))
                    newKeys.Remove(val);
            }

            return newKeys.ToArray();
        }
    }
}
