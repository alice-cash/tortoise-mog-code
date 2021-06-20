using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tortoise.Graphics.Input
{
    public abstract class InputState
    {
        public abstract bool Poll();

        internal T[] GetNewItems<T>(T[] data, T[] reference)
        {
            if (reference.Length == 0)
                return new T[0];

            List<T> newKeys = new List<T>();
            foreach (T val in reference)
            {
                if (!data.Contains(val))
                    newKeys.Add(val);
            }

            return newKeys.ToArray();
        }

        internal T[] GetMissingItems<T>(T[] data, T[] reference)
        {
            if (data.Length == 0)
                return new T[0];

            List<T> newKeys = new List<T>(data);
            foreach (T val in data)
            {
                if (reference.Contains(val))
                    newKeys.Remove(val);
            }

            return newKeys.ToArray();
        }
    }
}
