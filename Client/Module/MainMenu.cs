/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 8/5/2010
 * Time: 11:46 PM
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
using AgateLib.Geometry;
using AgateLib.DisplayLib;
using AgateLib.Resources;
using Tortoise.Client.Exceptions;
using Tortoise.Client.Rendering;
using Tortoise.Client.Rendering.GUI;
using Tortoise.Shared.Module;
using Tortoise.Shared.IO;
using Tortoise.Client.Module.GUI.FrameBuilder;

namespace Tortoise.Client.Module
{
	
	/// <summary>
	/// Load up the Main Menu Module.
	/// </summary>
	internal class MainMenuLoader : ModuleLoader
	{
		public override Version Version {
			get {
				return new Version(1,0,0,0);
			}
		}
		
		public override string Name {
			get {
				return "Tortoise Main Menu and About Screen.";
			}
		}

		public override void Load()
		{
			if(Window.AvailableScreens.ContainsKey("MainMenu") || Window.AvailableScreens.ContainsKey("About"))
				throw new ModuleLoadException("The Current screen has already been set!");
			Window.AvailableScreens.Add("MainMenu",new MainMenu());
			Window.AvailableScreens.Add("About",new About());
		}
	}
	
	/// <summary>
	///  The Main Menu.
	/// </summary>
	class MainMenu : Control, IScreen
	{

		Container _renderItems;
		
		public MainMenu():base("_Screen",Point.Empty,Size.Empty)
		{
			_renderItems  = new Container("_parent", 0,0,  Program.ScreenWidth, Program.ScreenHeight);
			_renderItems._backgroundColor = Color.Wheat;
		}
		
		public override void Dispose()
		{
			_renderItems.Dispose();
		}
		
		public override void Init()
		{
			_renderItems.Init();
			
			Label title = new Label("_title", Program.GameName, new Point(10,10), new Size(Program.ScreenWidth - 20, 40), FontSurface.AgateSans24);
			
			string aboutText = localization.Default.Strings.GetFormatedString("Main_Menu_Credits");
			Point buttonPos = new Point(Program.ScreenWidth - 80, Program.ScreenHeight - 40);

            Surface aboutBackground = FrameBuilder.Frame1.CreateFrame(new Size(60,25));

			Button about = new Button("_about", aboutText, buttonPos, new Size(60,25), FontSurface.AgateSans10);
            about.BackgroundImage = aboutBackground;

			title.TextAlignement = TextAlignement.Center;
			title.BackgroundColor = Color.Transparent;
			
			about.BackgroundColor = Color.FromArgb(40,Color.Gray);
			about.MouseUp+= delegate
			{
				Window.Instance.ChangeToScreen("About");
			};
			
			_renderItems.Controls.Add(10, title);
			_renderItems.Controls.Add(10, about);

		}
		
		public override void Load()
		{
			_renderItems.Load();
		}
		
		public override void Unload()
		{
			_renderItems.Unload();
		}
		
		public new void Render()
		{
			_renderItems.Render();
		}
		
		public new void Tick(TickEventArgs e)
		{
			_renderItems.Tick(e);
		}
		
		public new void OnMouseUp(MouseEventArgs e)
		{
			_renderItems.OnMouseUp(e);
		}
		
		public new void OnMouseMove(MouseEventArgs e)
		{
			_renderItems.OnMouseMove(e);
		}
		
		public new void OnMouseDown(MouseEventArgs e)
		{
			_renderItems.OnMouseDown(e);
		}
		
		public new void OnKeyboardUp(KeyEventArgs e)
		{
			_renderItems.OnKeyboardUp(e);
		}
		
		public new void OnKeyboardDown(KeyEventArgs e)
		{
			_renderItems.OnKeyboardDown(e);
		}
		
		public void OnResize()
		{
			_renderItems.Size = Window.MainWindow.Size;
		}

	}
	/// <summary>
	/// If you make your own About screen, please leave the "Made with the Tortoise MOG Framework." In it.
	/// </summary>
	class About : Control, IScreen
	{
		
		Container _renderItems;
		
		public About():base("_Screen",Point.Empty,Size.Empty)
		{
			_renderItems  = new Container("_parent", 0,0,  Program.ScreenWidth, Program.ScreenHeight);
			_renderItems._backgroundColor = Color.Wheat;
		}

		public override void Dispose()
		{
			_renderItems.Dispose();
		}

		public override void Init()
		{
			_renderItems.Init();
			
			Label title = new Label("_title", Program.GameName, new Point(10,10), new Size(Program.ScreenWidth - 20, 40), FontSurface.AgateSans24);
			
			string creditText = localization.Default.Strings.GetFormatedString("Credits_Text");
			Label cointents = new Label("_contents", creditText, new Point(0,0), new Size(Program.ScreenWidth,Program.ScreenHeight), FontSurface.AgateSans10);
			
			Point buttonPos = new Point(Program.ScreenWidth - 250, Program.ScreenHeight - 20);
			string returnText = localization.Default.Strings.GetFormatedString("Credits_Return");
			Button returnButton = new Button("_return", returnText,buttonPos,new Size(250,20),FontSurface.AgateSans10);

			title.TextAlignement = TextAlignement.Center;
			title.BackgroundColor = Color.Transparent;
			
			cointents.TextAlignement = TextAlignement.Center;
			cointents.BackgroundColor = Color.Transparent;
			
			returnButton.BackgroundColor = Color.FromArgb(40,Color.Gray);

			returnButton.MouseUp+= delegate
			{
				Window.Instance.ChangeToScreen("MainMenu");
			};

			_renderItems.Controls.Add(10, title);
			_renderItems.Controls.Add(15, cointents);
			_renderItems.Controls.Add(10, returnButton);
		}
		
		
		public override void Load()
		{
			_renderItems.Load();
		}
		
		public override void Unload()
		{
			_renderItems.Unload();
		}
		
		public new void Render()
		{
			_renderItems.Render();
		}
		
		public new void Tick(TickEventArgs e)
		{
			_renderItems.Tick(e);
		}
		
		public new void OnMouseUp(MouseEventArgs e)
		{
			_renderItems.OnMouseUp(e);
		}
		
		public new void OnMouseMove(MouseEventArgs e)
		{
			_renderItems.OnMouseMove(e);
		}
		
		public new void OnMouseDown(MouseEventArgs e)
		{
			_renderItems.OnMouseDown(e);
		}
		
		public new void OnKeyboardUp(KeyEventArgs e)
		{
			_renderItems.OnKeyboardUp(e);
		}
		
		public new void OnKeyboardDown(KeyEventArgs e)
		{
			_renderItems.OnKeyboardDown(e);
		}
		
		public void OnResize()
		{
			_renderItems.Size = Window.MainWindow.Size;
		}
	}
}

