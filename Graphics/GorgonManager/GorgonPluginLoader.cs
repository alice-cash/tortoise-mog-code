using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using GorgonLibrary;
using GorgonLibrary.Input;

namespace Tortoise.Graphics.GorgonManager
{
    class GorgonPluginLoader
    {
        public static void LoadPlugins()
        {

            var files = Directory.GetFiles("./", "*.dll");

            if (files.Length == 0)
            {
                return;
            }

            // Find our plug-ins in the DLLs.
            foreach (var file in files)
            {
                // Get the assembly name.  
                // This is the preferred method of loading a plug-in assembly.
                // It keeps us from going into DLL hell because it'll contain
                // version information, public key info, etc...  
                // We wrap this in this exception handler because if a DLL is
                // a native DLL, then it'll throw an exception.  And since
                // we can't load native DLLs as our plug-in, then we should
                // skip it.
                AssemblyName name;
                try
                {
                    name = AssemblyName.GetAssemblyName(file);


                }
                catch (BadImageFormatException)
                {
                    // This happens if we try and load a DLL that's not a .NET assembly.
                    continue;
                }


                // Skip any assemblies that aren't a plug-in assembly.
                try
                {
                    if (!Gorgon.PlugIns.IsPlugInAssembly(name))
                    {
                        continue;
                    }
                }
                catch(FileLoadException ex)
                {
                    continue;
                }
                // Load the assembly DLL.
                // This will not only load the assembly DLL into the application
                // domain (if it's not already loaded), but will also enumerate
                // the plug-in types.  If there are none, an exception will be
                // thrown.  This is why we do a check with IsPlugInAssembly before
                // we load the assembly.
                Gorgon.PlugIns.LoadPlugInAssembly(name);

            }


        }
    }
}
