using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using Tortoise.Client;
using Tortoise.Client.Module;
using Tortoise.Client.Net;
using Tortoise.Shared.Module;
using Tortoise.Shared.Localization;
using Tortoise.Shared.Diagnostics;
using System.Reflection;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;



using Tortoise.Graphics.Rendering;
using Tortoise.Graphics;

using Tortoise.Shared;

using TGraphics = Tortoise.Graphics.TGraphics;

namespace Tortoise.Client
{
    internal class Game
    {

        public TGraphics Graphics { get; private set; }
        public MainForm MainForm { get; private set; }

        public Game(MainForm Target)
        {
            MainForm = Target;
            Graphics = TGraphics.CreateGraphics(Target);
        }


        public bool GameLoop()
        {

            while (Program.ThreadsRunning)
            {
                Application.DoEvents();
                Graphics.DoTick();
                Graphics.DoRender();
            }
            return true;
        }

        public void Quit()
        {
            Program.ThreadsRunning = false;
            Graphics.Window.Unload();
            MainForm.Close();
            CleanUp();
        }

        internal void CleanUp()
        {

        }
    }
}
