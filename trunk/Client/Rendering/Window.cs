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
using AgateLib;
using AgateLib.DisplayLib;
using AgateLib.Geometry;
using AgateLib.InputLib;

using Tortoise.Client.Collection;

namespace Tortoise.Client.Rendering
{
	/// <summary>
	/// Description of Screen.
	/// </summary>
	public class Window
	{
		public static DisplayWindow MainWindow{get; private set;}
		public static Window Instance {get; private set;}
		
		public IScreen CurrentScreen{get; set;}
		public Window()
		{
			Instance = this;
		}
		
		public void Run()
		{
			AgateSetup AS = new AgateSetup();

			
			AS.Initialize(true, false, true);
			if (AS.WasCanceled)
				return;

			MainWindow = DisplayWindow.CreateWindowed ("Tortoise MOG", 800, 600);
			
			CurrentScreen = new MainMenuScreen();
			CurrentScreen.Init();
			CurrentScreen.Load();
			
			
			TickEventArgs tickEventData = new TickEventArgs();
		
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
			
			Timing.StopWatch frameTimer = new Timing.StopWatch(true);
			Timing.StopWatch TotalTimer = new Timing.StopWatch(true);
			LimitedList<double> lastFrameTimes = new LimitedList<double>(30,0);
			
			

			while (MainWindow.IsClosed == false)
			{	
				CurrentScreen.Tick(tickEventData);
				
				if(Display.RenderTarget != MainWindow.FrameBuffer)
					Display.RenderTarget = MainWindow.FrameBuffer;
				Display.BeginFrame();
				Display.Clear(Color.Black);
				
				CurrentScreen.Render();

				Display.EndFrame();
				
				
				MainWindow.Title = tickEventData.FPS.ToString("f2") + " fps - " +  tickEventData.AverageFrameTime.ToString("f2") + " ms";
				
				Core.KeepAlive();
				
				tickEventData.LastFrameTime = frameTimer.TotalMilliseconds;
				//this resets the timers time, but doesn't stop it.
				frameTimer.Reset();
				lastFrameTimes.Add(tickEventData.LastFrameTime);
				tickEventData.AverageFrameTime = CalculateAverage(lastFrameTimes);
				tickEventData.FPS = 1000 / tickEventData.AverageFrameTime;
				tickEventData.TotalSeconds = TotalTimer.TotalSeconds;
				tickEventData.TotalMilliseconds = TotalTimer.TotalMilliseconds;
			}
		}
		
		/// <summary>
		/// Calculates the average number in a LimitedList<double>. If allowZero is false, 0 will be replaced with 1
		/// </summary>
		/// <param name="frameTimes"></param>
		/// <returns></returns>
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
