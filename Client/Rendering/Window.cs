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

using Tortoise.Client.Collection;

namespace Tortoise.Client.Rendering
{
	/// <summary>
	/// Description of Screen.
	/// </summary>
	public class Window
	{
		public Screen CurrentScreen{get; set;}
		public DisplayWindow MainWindow{get; private set;}
		public Window()
		{
		}
		
		public void Run()
		{
			AgateSetup AS = new AgateSetup();
			AS.Initialize(true, false, false);
			if (AS.WasCanceled)
				return;

			MainWindow = DisplayWindow.CreateWindowed ("Tortoise MOG", 800, 600);
			
			CurrentScreen = new MainMenuScreen();
			CurrentScreen.Init();
			CurrentScreen.Load();
			
			
			TickEventArgs tickEventData = new TickEventArgs();
			Timing.StopWatch frameTimer = new Timing.StopWatch(true);
			LimitedList<int> last30FrameTimes = new LimitedList<int>(30,0);
			while (MainWindow.IsClosed == false)
			{
				
				CurrentScreen.Tick(tickEventData);
				
				
				if(Display.RenderTarget != MainWindow)
					Display.RenderTarget = MainWindow;
				Display.BeginFrame();
				Display.Clear(Color.Black);
				
				CurrentScreen.Render();
				
				Display.EndFrame();
				
				MainWindow.Title = tickEventData.GetFPS.ToString() + " fps - " + (tickEventData.LastFrameTime / 1000).ToString() + " ms";
				
				Core.KeepAlive();
				
				tickEventData.LastFrameTime = (int)frameTimer.TotalMilliseconds * 1000;
				//this resets the timers time, but doesn't stop it.
				frameTimer.Reset();
				last30FrameTimes.Add(tickEventData.LastFrameTime);
				tickEventData.GetFPS = CalculateFPS(last30FrameTimes);
			}
		}
		
		/// <summary>
		/// Calculates the average frame rate for the past 30 frames.
		/// </summary>
		/// <param name="frameTimes"></param>
		/// <returns></returns>
		private int CalculateFPS(LimitedList<int> frameTimes)
		{
			int count = 0, total = 0;
			foreach(var v in frameTimes)
			{
				if(v == 0) continue;
				count++;
				total += v;
			}
			if(count == 0) return 0;
			
			return 1000000 / (total / count);
		}
	}
}
