/*
 * Copyright 2012 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 * 
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY Matthew Cash ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Matthew Cash OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Matthew Cash.
 */
using System;
using System.Drawing;
using System.Collections.Generic;


using Timer = Tortoise.Shared.Timer;

using Tortoise.Shared;
using Tortoise.Shared.Collection;
using Tortoise.Shared.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Tortoise.Graphics;

using Tortoise.Graphics.Input;

//namespace Tortoise.Graphics.Rendering
//{
//    public class Window : Invokable, IRender
//    {
//        /*
//        public static Window CreateWindow(String name)
//        {
//            Window result = new Window(name);

//            return result;
//        }*/

//        private string _name;
//        public string Name { get { return _name; } }

//        public Window(string Name)
//        {
//            _invoker = new Invoker();

//        }


//        public void Initialize()
//        {
//        }

//        public void Load()
//        {
//            throw new NotImplementedException();
//        }

//        public void Unload()
//        {
//            throw new NotImplementedException();
//        }

//        public void Tick(TickEventArgs e)
//        {
//            throw new NotImplementedException();
//        }

//        public void Render()
//        {
//            throw new NotImplementedException();
//        }

//        public void Dispose()
//        {
//            throw new NotImplementedException();
//        }


//    }
//}


namespace Tortoise.Graphics.Rendering
{
    /// <summary>
    /// The main window.
    /// </summary>
    public class Window
    {

        TickEventArgs tickEventData;
        Timer frameTimer;
        Timer TotalTimer;
        LimitedList<double> lastFrameTimes;

        FontInfo DebugFont;

        //private Surface MainSurface;

        /* public int BestBitsPerPixle
         {
             get { return Video.BestBitsPerPixel(ScreenWidth, ScreenHeight, false); }
         }*/

        public Dictionary<string, Screen> AvailableScreens { get; private set; }
        //public static Window Instance { get; private set; }


        public event EventHandler ScreenChanged;

        private int _bufferID;

        private ThreadSafetyEnforcer _threadSafety;
        private Invoker _invoker;

        //public bool GameRunning { get; set; }
        public Screen CurrentScreen { get; set; }

        public TGraphics Graphics { get; private set; }

        KeyboardState _keyState;
        MouseState _pointerState;


        public Window(TGraphics graphics)
        {
            AvailableScreens = new Dictionary<string, Screen>();
            Graphics = graphics;
        }

        internal void Initialize()
        {


            _threadSafety = new ThreadSafetyEnforcer("Main Window Class");
            _invoker = new Invoker(_threadSafety);

            //This selects our first screen, and Loads it.


            //Width, Height, BitsPerPixle, resizable, openGL, fullscreen, hardware, frame
            //Video.SetVideoMode(ScreenWidth, ScreenHeight, BestBitsPerPixle, true, false, false, true, true);
            //Video.WindowCaption = "Tortoise Demo";

            //MainSurface = GenerateSurface();


            //DebugFont = FontInfo.GetInstance(Graphics, 10, FontTypes.Sans_Mono);

            TMouseState mouse = Graphics.InputManager.MouseStateManager;
            TKeyState keyboard = Graphics.InputManager.KeyStateManager;


            mouse.MouseDownEvent += Mouse_ButtonDown;
            mouse.MouseUpEvent += Mouse_ButtonUp;
            mouse.MouseMoveEvent += Mouse_Move;

            keyboard.KeyboardKeyPressEvent += Window_KeyDown;
            keyboard.KeyboardKeyReleaseEvent += Window_KeyUp;

            //_keyboard.KeyPress += new EventHandler<KeyEventArgs>(Window_KeyPress);

            Graphics.Control.Resize += new EventHandler(Window_Resize);


            tickEventData = new TickEventArgs();
            frameTimer = new Timer(true);
            TotalTimer = new Timer(true);
            lastFrameTimes = new LimitedList<double>(30, 0);


            TConsole.SetIfNotExsistValue("gf_RenderUpdateRec", ConsoleVarable.OnOffVarable("Draw Boxes showing Updated Areas"));
            TConsole.SetIfNotExsistValue("gf_ShowFPS", ConsoleVarable.OnOffVarable("Displays FPS information"));


        }

        public void Unload()
        {
            //GameRunning = false;
            foreach (Screen screen in AvailableScreens.Values)
                if (screen.Loaded)
                    screen.Unload();
        }

        void Window_Resize(object sender, EventArgs e)
        {
            if (ScreenLoaded())
                CurrentScreen.OnResize();
        }

        void Window_KeyUp( KeyEventArgs e)
        {
            if (ScreenLoaded())
            {
                CurrentScreen.OnKeyboardUp(e);

                CurrentScreen.OnKeyboardPress(e);
            }
        }

        void Window_KeyDown(KeyEventArgs e)
        {
            if (ScreenLoaded())
                CurrentScreen.OnKeyboardDown(e);
        }

        /*
        void Window_KeyPress(object sender, KeyEventArgs e)
        {
            CurrentScreen.OnKeyboardPress(e);
        }*/

        void Mouse_Move(MouseEventArgs e)
        {
            if (ScreenLoaded())
                CurrentScreen.OnMouseMove(e);
        }

        void Mouse_ButtonUp(MouseEventArgs e)
        {
            if (ScreenLoaded())
                CurrentScreen.OnMouseUp(e);
        }

        void Mouse_ButtonDown(MouseEventArgs e)
        {
            if (ScreenLoaded())
                CurrentScreen.OnMouseDown(e);
        }


        /*   public void Run()
           {
               if (GameRunning) return;
               GameRunning = true;
           }*/

        public Surface GenerateSurface(Size size)
        { return GenerateSurface(size.Width, size.Height); }
        public Surface GenerateSurface(int width, int height)
        {
            Surface result = Surface.CreateBlankSurface(Graphics, width, height);
            return result;
        }
        /*
        public Surface GenerateTransparentSurface(int width, int height)
        { return GenerateTransparentSurface(new Size(width, height)); }
        public Surface GenerateTransparentSurface(Size size)
        {
            return new Surface(GfxResource.t).CreateStretchedSurface(size);
        }*/

            internal void Tick()
        {
            _invoker.PollInvokes();
            if (ScreenLoaded())
            {
                CurrentScreen.Tick(tickEventData);
            }
        }

        internal void Render()
        {
            if (ScreenLoaded())
            {
                CurrentScreen.Render();
            }


            //Video.Screen.Blit(MainSurface);

            //if (TConsole.GetValue("gf_RenderUpdateRec").Value == "1")
            //{
            //     foreach (Rectangle rec in updateAreas)
            //         Video.Screen.Draw(new Box(rec.Location, rec.Size), Color.Red);
            //}



            // if (TConsole.GetValue("gf_ShowFPS").Value == "1")
            // {
            //     Video.Screen.Blit(DebugFont.Font.Render(tickEventData.FPS.ToString("f2") + " fps - " + tickEventData.AverageFrameTime.ToString("f2") + " ms", Color.Red), new Point(10, 10));
            //}

            //Video.Screen.Update(); // (updateAreas);


            //All of this is just for calculating the FPS and stuff.
            tickEventData.LastFrameTime = frameTimer.ElapsedMilliseconds;
            //this resets the timer's time, but doesn't stop it.
            frameTimer.Reset();
            frameTimer.Start();
            lastFrameTimes.Add(tickEventData.LastFrameTime);
            tickEventData.AverageFrameTime = CalculateAverage(lastFrameTimes);
            tickEventData.FPS = 1000 / tickEventData.AverageFrameTime;
            tickEventData.TotalSeconds = TotalTimer.ElapsedSeconds;
            tickEventData.TotalMilliseconds = TotalTimer.ElapsedMilliseconds;
        }




        /// <summary>
        /// Change to the screen name listed. The screenName should be an item in the AvailableScreens.
        /// </summary>
        public void ChangeToScreen(string screenName)
        {
            //The lambda expression keeps the name in scope
            //so no worried there, i hope...
            //The InvokeMethod will run it right away if this is the correct thread.
            System.Action<object> id = (object nothing) =>
            {
                if (!AvailableScreens.ContainsKey(screenName))
                    throw new Exception(string.Format("{0} does not exist!", screenName));
                if(ScreenLoaded()) 
                    CurrentScreen.Unload();
                CurrentScreen = AvailableScreens[screenName];
                CurrentScreen.Load();
                if (ScreenChanged != null)
                    ScreenChanged(this, EventArgs.Empty);

            };
            InvokeMethod(id, null);
        }


        public bool ScreenLoaded()
        {
            return CurrentScreen == null ? false : CurrentScreen.Loaded;
        }


        /// <summary>
        /// Either adds the specified thread to the invoke list, or calls it now if its in the parent thread.
        /// </summary>
        /// <param name="methodToInvoke">A method or delegate to call.</param>
        /// <param name="userData">An object with information sent to the method.</param>
        public void InvokeMethod(System.Action<object> methodToInvoke, object userData)
        {
            _invoker.InvokeMethod(methodToInvoke, userData);
        }

        /// <summary>
        /// Returns true if we need to invoke a method.
        /// </summary>
        public bool InvokeRequired()
        {
            return _invoker.InvokeRequired();
        }

        /// <summary>
        /// Calculates the average number in a LimitedList<double>. If allowZero is false, 0 will be replaced with nonZero(for use in division)
        /// </summary>
        private double CalculateAverage(LimitedList<double> items, bool allowZero = false, int nonZero = 1)
        {
            double count = 0, total = 0, result = 0;
            foreach (var v in items)
            {
                if (v == 0) continue;
                count++;
                total += v;
            }

            if (count == 0) return allowZero ? 0 : nonZero;

            result = total / count;
            return !allowZero && result == 0 ? nonZero : result;
        }


    }
}
