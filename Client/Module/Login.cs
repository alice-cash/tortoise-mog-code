/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 8/7/2010
 * Time: 10:01 PM
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
using Tortoise.Shared.Net;
using Tortoise.Shared.IO;
using Tortoise.Shared.Module;
using Tortoise.Client.Exceptions;
using Tortoise.Client.Rendering;
using Tortoise.Client.Rendering.GUI;

using AgateLib.Geometry;
using AgateLib.DisplayLib;

namespace Tortoise.Client.Module
{
	
	internal class LoginLoader : ModuleLoader
	{
		public const ushort ClientModuleComID = 10010;
		public const ushort ServerModuleComID = 20010;
		
		public override Version Version {
			get {
				return new Version(1,0,0,0);
			}
		}
		
		public override string Name {
			get {
				return "Tortoise Login Manager.";
			}
		}		
		static Login _instance;
		public override void Load()
		{
			_instance = new Login();
			Connection.AddModuleHandle(ClientModuleComID, _instance);
            if (MainMenu.LoginRequest != null)
                throw new ModuleLoadException("MainMenu.LoginRequest has already been set!");
            MainMenu.LoginRequest = _instance.LoginRequest;

            if (Window.AvailableScreens.ContainsKey("LoginStatusScreen"))
                throw new ModuleLoadException("A LoginStatusScreen screen has already been set!");
            LoginStatusScreen.Instance = new LoginStatusScreen();
            Window.AvailableScreens.Add("LoginStatusScreen", LoginStatusScreen.Instance);
        }
		

	}
	
	/// <summary>
	/// Description of Login.
	/// </summary>
	class Login : IComModule
	{
		//These are IDs used by the packet. The random numbers
		//should help catch sync issues.
		//TODO: Change these to different values for your game, make sure they match in the server.
		private const byte _packet_AuthKey = 			200;
		private const byte _packet_LoginRequest = 	202;
		private const byte _packet_LoginResponce = 	207;
		private const byte _packet_Version =			209;


		public Login()
		{
		}
		
		public void Communication(Connection Sender, ByteReader data)
		{
			
		}

        public void LoginRequest(string username, string password)
        {
            Window.Instance.ChangeToScreen("LoginStatusScreen");
            LoginStatusScreen.Instance.Text = localization.Default.Strings.GetFormatedString("Login_Status_Connecting");
        }
	}

    class LoginStatusScreen : Screen
    {
        public static LoginStatusScreen Instance;
        public string Text
        {
            get { return (Controls["_contents"] as Label).Text; }
            set
            {
                _threadSafety.EnforceThreadSafety();
                (Controls["_contents"] as Label).Text = value;
            }
        }
        public override void Init()
        {
            BackgroundColor = Color.Wheat;
            Label cointents = new Label("_contents", "", new Point(0, 0), new Size(Window.ScreenWidth, Window.ScreenHeight), FontSurface.AgateSans10);
            cointents.TextAlignement = TextAlignement.Center;
            Controls.Add(10, cointents);

        }

        public void OnResize()
        {
            Size = Window.MainWindow.Size;
        }

        public override void Load()
        {
            base.Load();
            //This is called right before the window becomes the focused screen.
            //Any code to run should go here.
        }

        public override void Unload()
        {
            base.Unload();
            //This is called when the window is no longer the focused screen.
            //Any code to run should go here.
        }



    }
}
