/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 7/26/2010
 * Time: 9:36 PM
 * 
 * Copyright 2010 Matthew Cash. All rights reserved.
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
using System.Collections.Generic;
using AgateLib;
using AgateLib.DisplayLib;
using AgateLib.Geometry;
using AgateLib.InputLib;

using Tortoise.Client.Collection;
using Tortoise.Shared.Threading;

namespace Tortoise.Client.Rendering
{
	/// <summary>
	/// The main window.
	/// </summary>
	public class Window : IInvokable
	{
		//This is called autmaticly when any static property is accessed
		//or an instance of the class is created
		static Window()
		{
			AvailableScreens = new Dictionary<string, Screen>();
		}
		
		public static Dictionary<string, Screen> AvailableScreens{get; private set;}
		public static DisplayWindow MainWindow{get; private set;}
		public static Window Instance {get; private set;}
		
		public event EventHandler ScreenChanged;
		
		
		private ThreadSafetyEnforcer _threadSafety;
		private Invoker _invoker;
		
		public Screen CurrentScreen{get; set;}
		public Window()
		{
			Instance = this;
		}
		
		public void Run()
		{
			//This is the main entry for the program.
			//We create the window here
			//and then loop to draw each frame.
			
			//Thread safety is a very important part since we use multiple threads
			//and random people will use this and add more random stuff
			_threadSafety = new ThreadSafetyEnforcer("Main Window Class");
			_invoker = new Invoker(_threadSafety);
			
			//Set up the Agate backend system, and send it the command line arguments
			AgateSetup AS = new AgateSetup(Environment.GetCommandLineArgs());
			
			//Display, Sound, Input
			AS.Initialize(true, true, true);
			//If they run the "--choose" command, and cancel their selection
			//then we just quit.
			if (AS.WasCanceled)
				return;
			
			// Resizing is broken in the current agatelib revision.
			MainWindow = DisplayWindow.CreateWindowed ("Tortoise MOG",  Program.ScreenWidth,  Program.ScreenHeight, false);
			
			MainWindow.Resize += delegate(object sender, EventArgs e)
			{
				CurrentScreen.OnResize();
			};
			
			//This selects our first screen, and Loads it.
			if(!AvailableScreens.ContainsKey("MainMenu"))
				throw new Exception("No module has set a MainMenu screen!");
			CurrentScreen = AvailableScreens["MainMenu"];
			CurrentScreen.Load();
			
			
			//This catches and sends Input events to the current screen.
			//We filter mouse and keybord events into 2 logical classes
			//as we should not check for mouse values in a keyboard event
			Mouse.MouseDown += delegate(InputEventArgs e)
			{
				CurrentScreen.OnMouseDown(new MouseEventArgs(e));
			};

			Mouse.MouseUp += delegate(InputEventArgs e)
			{
				CurrentScreen.OnMouseUp(new MouseEventArgs(e));
			};

			Mouse.MouseMove += delegate(InputEventArgs e)
			{
				CurrentScreen.OnMouseMove(new MouseEventArgs(e));
			};
			
			Keyboard.KeyDown += delegate(InputEventArgs e)
			{
				CurrentScreen.OnKeyboardDown(new KeyEventArgs(e));
			};
			
			Keyboard.KeyUp += delegate(InputEventArgs e)
			{
				CurrentScreen.OnKeyboardUp(new KeyEventArgs(e));
			};
			
			
			
			//Now we just create some varables used by the loop.
			//The TickEventArgs object is reused each frame.
			//No reason to recreate it each frame.
			TickEventArgs tickEventData = new TickEventArgs();
			Timing.StopWatch frameTimer = new Timing.StopWatch(true);
			Timing.StopWatch TotalTimer = new Timing.StopWatch(true);
			LimitedList<double> lastFrameTimes = new LimitedList<double>(30,0);
			
			

			while (MainWindow.IsClosed == false)
			{
				_invoker.PollInvokes();
				CurrentScreen.Tick(tickEventData);
				
				//I should throw an exception, but we may have issues figuring out
				//what forgot to change it back to the main window.
				if(Display.RenderTarget != MainWindow.FrameBuffer)
					Display.RenderTarget = MainWindow.FrameBuffer;
				Display.BeginFrame();
				
				//I like black. Background should be done in the Current Screen.
				Display.Clear(Color.Black);
				
				CurrentScreen.Render();

				Display.EndFrame();
				
				//Pureley for debuging purposes ATM
				//TODO: Remove and replace with text based info in the window.
				MainWindow.Title = tickEventData.FPS.ToString("f2") + " fps - " +  tickEventData.AverageFrameTime.ToString("f2") + " ms";
				
				//has somthing to do with the agatelib, probably to poll events for the window and such
				Core.KeepAlive();
				
				//All of this is just for calculating the FPS and stuff.
				tickEventData.LastFrameTime = frameTimer.TotalMilliseconds;
				//this resets the timer's time, but doesn't stop it.
				frameTimer.Reset();
				lastFrameTimes.Add(tickEventData.LastFrameTime);
				tickEventData.AverageFrameTime = CalculateAverage(lastFrameTimes);
				tickEventData.FPS = 1000 / tickEventData.AverageFrameTime;
				tickEventData.TotalSeconds = TotalTimer.TotalSeconds;
				tickEventData.TotalMilliseconds = TotalTimer.TotalMilliseconds;
			}
		}
		

		/// <summary>
		/// Change to the screen name listed. The screenName should be an item in the AvailableScreens.
		/// </summary>
		public void ChangeToScreen(string screenName)
		{
			//The lamba expression keeps the name in scope
			//so no worried there, i hope...
			System.Action<object> id = (object nothing) =>
			{
				if(!AvailableScreens.ContainsKey(screenName))
					throw new Exception(string.Format("{0} does not exsist!", screenName));
                CurrentScreen.Unload();
				CurrentScreen = AvailableScreens[screenName];
				CurrentScreen.Load();
				if(ScreenChanged!= null)
					ScreenChanged(this, EventArgs.Empty);

			};
			InvokeMethod(id, null);
		}
		
		/// <summary>
		/// Either adds the specified thread to the invoke list, or calls it now if its in the parent thread.
		/// </summary>
		/// <param name="methodToInvoke">A method or deligate to call.</param>
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
		/// Calculates the average number in a LimitedList<double>. If allowZero is false, 0 will be replaced with 1(for use in division)
		/// </summary>
		private double CalculateAverage(LimitedList<double> items, bool allowZero = false)
		{
			double count = 0, total = 0, result = 0;
			foreach(var v in items)
			{
				if(v == 0) continue;
				count++;
				total += v;
			}
			
			if(count == 0) return allowZero ? 0 : 1;
			
			result = total / count;
			return !allowZero && result == 0 ? 1 : result;
		}
	}
}
