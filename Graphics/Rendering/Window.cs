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
using GorgonLibrary.Input;
using GorgonLibrary.Graphics;

namespace Tortoise.Graphics.Rendering
{
    public class Window : Invokable, IRender
    {

        public static Window CreateWindow(String name)
        {
            
        }

        private string _name;
        public string Name { get { return _name; } }

        public Window(string Name)
        {

        }

        /// <summary>
        /// Initialize the class.
        /// </summary>
        public override void Initialize()
        {
            _invoker = new Invoker();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }

        public void Tick(TickEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Render()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


    }
}


//namespace Tortoise.Graphics.Rendering
//{
//    /// <summary>
//    /// The main window.
//    /// </summary>
//    public class Window : GameWindow, IInvokable 
//    {
//        //This is called automatically when any static property is accessed
//        //or an instance of the class is created
//        static Window()
//        {
//            AvailableScreens = new Dictionary<string, Screen>();
//        }

//        TickEventArgs tickEventData;
//        Timer frameTimer;
//        Timer TotalTimer;
//        LimitedList<double> lastFrameTimes;

//        FontInfo DebugFont;

//        private Surface MainSurface;

//        public const int ScreenHeight = 600, ScreenWidth = 800;

//       /* public int BestBitsPerPixle
//        {
//            get { return Video.BestBitsPerPixel(ScreenWidth, ScreenHeight, false); }
//        }*/

//        public static Dictionary<string, Screen> AvailableScreens { get; private set; }
//        public static Window Instance { get; private set; }


//        public event EventHandler ScreenChanged;

//        private int _bufferID;

//        private ThreadSafetyEnforcer _threadSafety;
//        private Invoker _invoker;

//        public bool GameRunning { get; set; }
//        public Screen CurrentScreen { get; set; }

//        protected override void OnLoad(EventArgs e)
//        {
//            GL.ClearColor(Color.Black);
//            Instance = this;

//            _threadSafety = new ThreadSafetyEnforcer("Main Window Class");
//            _invoker = new Invoker(_threadSafety);

//            //This selects our first screen, and Loads it.
//            if (!AvailableScreens.ContainsKey("MainMenu"))
//                throw new Exception("No module has set a MainMenu screen!");
//            CurrentScreen = AvailableScreens["MainMenu"];

//            //Width, Height, BitsPerPixle, resizable, openGL, fullscreen, hardware, frame
//            //Video.SetVideoMode(ScreenWidth, ScreenHeight, BestBitsPerPixle, true, false, false, true, true);
//            //Video.WindowCaption = "Tortoise Demo";

//            MainSurface = GenerateSurface(Size);

//            CurrentScreen.Load();

//            DebugFont = FontInfo.GetInstance(10, FontTypes.Sans_Mono);

//            Mouse.ButtonDown += new EventHandler<MouseButtonEventArgs>(Mouse_ButtonDown);
//            Mouse.ButtonUp += new EventHandler<MouseButtonEventArgs>(Mouse_ButtonUp);
//            Mouse.Move += new EventHandler<MouseMoveEventArgs>(Mouse_Move);

//            KeyDown += new EventHandler<KeyboardKeyEventArgs>(Window_KeyDown);
//            KeyUp += new EventHandler<KeyboardKeyEventArgs>(Window_KeyUp);
//            KeyPress += new EventHandler<KeyPressEventArgs>(Window_KeyPress);

//            Resize += new EventHandler<EventArgs>(Window_Resize);

//            Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(Window_Closing);


//            tickEventData = new TickEventArgs();
//            frameTimer = new Timer(true);
//            TotalTimer = new Timer(true);
//            lastFrameTimes = new LimitedList<double>(30, 0);


//            TConsole.SetIfNotExsistValue("gf_RenderUpdateRec", ConsoleVarable.OnOffVarable("Draw Boxes showing Updated Areas"));
//            TConsole.SetIfNotExsistValue("gf_ShowFPS", ConsoleVarable.OnOffVarable("Displays FPS information"));


//        }






//        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
//        {
//            GameRunning = false;
//            foreach (Screen screen in AvailableScreens.Values)
//                if (screen.Loaded)
//                    screen.Unload();

//            Program.ThreadsRunning = false;
//            //Code to run when everything else is unloaded should go here 
//        }

//        void Window_Resize(object sender, EventArgs e)
//        {
//            CurrentScreen.OnResize();
//        }

//        void Window_KeyUp(object sender, KeyboardKeyEventArgs e)
//        {
//            CurrentScreen.OnKeyboardUp(new KeyEventArgs(e));
//        }

//        void Window_KeyDown(object sender, KeyboardKeyEventArgs e)
//        {
//            CurrentScreen.OnKeyboardDown(new KeyEventArgs(e));
//        }

//        void Window_KeyPress(object sender, KeyPressEventArgs e)
//        {
//            CurrentScreen.OnKeyboardPress(e);
//        }

//        void Mouse_Move(object sender, MouseMoveEventArgs e)
//        {
//            CurrentScreen.OnMouseMove(new MouseEventArgs(e));
//        }

//        void Mouse_ButtonUp(object sender, MouseButtonEventArgs e)
//        {
//            CurrentScreen.OnMouseUp(new MouseEventArgs(e));
//        }

//        void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
//        {
//            CurrentScreen.OnMouseDown(new MouseEventArgs(e));
//        }


//        public void Run()
//        {
//            if (GameRunning) return;
//            GameRunning = true;
//        }

//        public Surface GenerateSurface(Size size)
//        { return GenerateSurface(size.Width, size.Height); }
//        public Surface GenerateSurface(int width, int height)
//        {
//            Surface s = new Surface(width, height);
//            return s;
//        }
//        /*
//        public Surface GenerateTransparentSurface(int width, int height)
//        { return GenerateTransparentSurface(new Size(width, height)); }
//        public Surface GenerateTransparentSurface(Size size)
//        {
//            return new Surface(GfxResource.t).CreateStretchedSurface(size);
//        }*/

//        protected override void OnRenderFrame(FrameEventArgs e)
//        {
//            if (!GameRunning)
//                return;
//            _invoker.PollInvokes();
//            CurrentScreen.Tick(tickEventData); 


//            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
//            GL.MatrixMode(MatrixMode.Modelview);
//            GL.LoadIdentity();

//            GL.Color3(Color.Red);
//            GL.Begin(BeginMode.Triangles);						// Drawing Using TrianGL.es
//            CurrentScreen.Render();
//            GL.End();


//            //Video.Screen.Blit(MainSurface);

//            //if (TConsole.GetValue("gf_RenderUpdateRec").Value == "1")
//            //{
//           //     foreach (Rectangle rec in updateAreas)
//           //         Video.Screen.Draw(new Box(rec.Location, rec.Size), Color.Red);
//            //}



//           // if (TConsole.GetValue("gf_ShowFPS").Value == "1")
//           // {
//           //     Video.Screen.Blit(DebugFont.Font.Render(tickEventData.FPS.ToString("f2") + " fps - " + tickEventData.AverageFrameTime.ToString("f2") + " ms", Color.Red), new Point(10, 10));
//            //}

//            //Video.Screen.Update(); // (updateAreas);

//            SwapBuffers();

//            //All of this is just for calculating the FPS and stuff.
//            tickEventData.LastFrameTime = frameTimer.ElapsedMilliseconds;
//            //this resets the timer's time, but doesn't stop it.
//            frameTimer.Reset();
//            frameTimer.Start();
//            lastFrameTimes.Add(tickEventData.LastFrameTime);
//            tickEventData.AverageFrameTime = CalculateAverage(lastFrameTimes);
//            tickEventData.FPS = 1000 / tickEventData.AverageFrameTime;
//            tickEventData.TotalSeconds = TotalTimer.ElapsedSeconds;
//            tickEventData.TotalMilliseconds = TotalTimer.ElapsedMilliseconds;
//        }




//        /// <summary>
//        /// Change to the screen name listed. The screenName should be an item in the AvailableScreens.
//        /// </summary>
//        public void ChangeToScreen(string screenName)
//        {
//            //The lamba expression keeps the name in scope
//            //so no worried there, i hope...
//            //The InvokeMethod will run it right away if this is the correct thread.
//            System.Action<object> id = (object nothing) =>
//            {
//                if (!AvailableScreens.ContainsKey(screenName))
//                    throw new Exception(string.Format("{0} does not exist!", screenName));
//                CurrentScreen.Unload();
//                CurrentScreen = AvailableScreens[screenName];
//                CurrentScreen.Load();
//                if (ScreenChanged != null)
//                    ScreenChanged(this, EventArgs.Empty);

//            };
//            InvokeMethod(id, null);
//        }



//        /// <summary>
//        /// Either adds the specified thread to the invoke list, or calls it now if its in the parent thread.
//        /// </summary>
//        /// <param name="methodToInvoke">A method or delegate to call.</param>
//        /// <param name="userData">An object with information sent to the method.</param>
//        public void InvokeMethod(System.Action<object> methodToInvoke, object userData)
//        {
//            _invoker.InvokeMethod(methodToInvoke, userData);
//        }

//        /// <summary>
//        /// Returns true if we need to invoke a method.
//        /// </summary>
//        public bool InvokeRequired()
//        {
//            return _invoker.InvokeRequired();
//        }

//        /// <summary>
//        /// Calculates the average number in a LimitedList<double>. If allowZero is false, 0 will be replaced with 1(for use in division)
//        /// </summary>
//        private double CalculateAverage(LimitedList<double> items, bool allowZero = false)
//        {
//            double count = 0, total = 0, result = 0;
//            foreach (var v in items)
//            {
//                if (v == 0) continue;
//                count++;
//                total += v;
//            }

//            if (count == 0) return allowZero ? 0 : 1;

//            result = total / count;
//            return !allowZero && result == 0 ? 1 : result;
//        }



//    }
//}
