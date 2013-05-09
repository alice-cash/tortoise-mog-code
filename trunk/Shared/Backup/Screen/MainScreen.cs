
using System;
using System.Collections.Generic;
using System.Text;

using GorgonLibrary;

namespace Tortoise.Client.Screen
{
    class MainScreen
    {
        TScreen PrimaryForm;
        GorgonLibrary.GUI.Desktop PrimaryDesktop;
        GorgonLibrary.InputDevices.Input PrimaryInput;


        public void RunScreen()
        {
            Gorgon.Initialize(false, false);

            PrimaryForm = new TScreen();




            Gorgon.SetMode(PrimaryForm);

            Gorgon.Idle += new GorgonLibrary.Graphics.FrameEventHandler(Idle);
            

            PrimaryDesktop = new GorgonLibrary.GUI.Desktop(PrimaryInput, new GorgonLibrary.GUI.GUISkin());
            PrimaryDesktop.Windows.Add(new GorgonLibrary.GUI.GUIWindow("Test", 2, 2, 100, 100));
            

            Gorgon.Go();

        }

        void Idle(object sender, GorgonLibrary.Graphics.FrameEventArgs e)
        {

            Gorgon.Screen.Clear();
            PrimaryDesktop.Update(e.FrameDeltaTime);
            PrimaryDesktop.Draw();
            

        }
    }
}
