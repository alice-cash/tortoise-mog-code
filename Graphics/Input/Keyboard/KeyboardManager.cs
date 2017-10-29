using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Tortoise.Graphics.Input.Keyboard
{
    public static class KeyboardManager
    {
        public static IKeyboard[] AvaliableKeyboards { get; private set; }
        public static IKeyboard CurrentKeyboard { get; private set; }

        static KeyboardManager()
        {
            AvaliableKeyboards = new IKeyboard[0];
        }

        public static void LoadKeyboard(Type kbType)
        {
            foreach (IKeyboard kb in AvaliableKeyboards)
                if (kb.GetType() == kbType)
                {
                    CurrentKeyboard = kb;
                    return;
                }
            throw new ArgumentException(kbType + " is not a loaded Keyboard.");
        }

        public static void LoadKeyboard(String Name)
        {
            foreach(IKeyboard kb in AvaliableKeyboards)
                if(kb.Name == Name)
                {
                    CurrentKeyboard = kb;
                    return;
                }
            throw new ArgumentException(Name + " is not a loaded Keyboard.");
        }

        public static void LoadKeyboards(Assembly LookIn, bool SharedLibrary)
        {
            LoadAssemblyModules(LookIn);
            if (SharedLibrary)
            {
                LoadAssemblyModules(Assembly.GetExecutingAssembly());
            }
        }

        public static void LoadKeyboards()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asp in loadedAssemblies)
                LoadAssemblyModules(asp);
        }

        private static void LoadAssemblyModules(Assembly target)
        {
            List<IKeyboard> keyboards = new List<IKeyboard>(AvaliableKeyboards);
            //we simply use reflection to look for all keyboard modules.
            foreach (Type type in target.GetTypes())
            {
                if (type.GetInterfaces().Contains(typeof(IKeyboard)))
                {
                    IKeyboard kb = Activator.CreateInstance(type) as IKeyboard;
                    if (kb == null) continue;
                    keyboards.Add(kb);
                    Debug.WriteLine(string.Format("Found Keyboard \"{0}\", Display Name \"{1}\"", kb.Name, kb.DisplayName));
                }
            }
            AvaliableKeyboards = keyboards.ToArray();
        }
    }
}
