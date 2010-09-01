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
        public override Version Version
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        public override string Name
        {
            get
            {
                return "Tortoise Main Menu and About Screen.";
            }
        }

        public override void Load()
        {
            if (Window.AvailableScreens.ContainsKey("MainMenu") || Window.AvailableScreens.ContainsKey("About"))
                throw new ModuleLoadException("The Current screen has already been set!");
            Window.AvailableScreens.Add("MainMenu", new MainMenu());
            Window.AvailableScreens.Add("About", new About());
        }
    }

    /// <summary>
    ///  The Main Menu.
    /// </summary>
    class MainMenu : Screen
    {

        public static Action<string, string> LoginRequest;

        public MainMenu()
        {
        }

        /// <summary>
        /// Initiates the Class. This should only be called from the Load() function's Base code in the Control function.
        /// </summary>
        public override void Init()
        {
            if (_inited) return;
            //This is called when the window is initially created.
            base.Init();

            BackgroundColor = Color.Wheat;

            Label title = new Label("_title", Program.GameName, new Point(10, 10), new Size(Program.ScreenWidth - 20, 40), FontSurface.AgateSans24);
            title.TextAlignement = TextAlignement.Center;
            title.BackgroundColor = Color.Transparent;

            string aboutText = localization.Default.Strings.GetFormatedString("Main_Menu_Credits");
            Point aboutPos = new Point(Program.ScreenWidth - 60, Program.ScreenHeight - 25);

            Surface aboutBackground = FrameBuilder.Frame1.CreateFrame(new Size(60, 25));
            Surface aboutOver = FrameBuilder.Frame4.CreateFrame(new Size(60, 25));
            Surface aboutDown = FrameBuilder.Frame5.CreateFrame(new Size(60, 25));

            Button about = new Button("_about", aboutText, aboutPos, new Size(60, 25), FontSurface.AgateSans10);
            about.BackgroundImage = aboutBackground;
            about.TextAlignement = TextAlignement.Center;
            about.MouseOverTexture = aboutOver;
            about.MouseDownTexture = aboutDown;

            about.BackgroundColor = Color.Transparent;
            about.MouseUp += delegate
            {
                Window.Instance.ChangeToScreen("About");
            };



            string exitText = localization.Default.Strings.GetFormatedString("Main_Menu_Exit");
            Point exitPos = new Point(Program.ScreenWidth / 2 - 30, 500);

            Surface exitBackground = FrameBuilder.Frame1.CreateFrame(new Size(60, 25));
            Surface exitOver = FrameBuilder.Frame4.CreateFrame(new Size(60, 25));
            Surface exitDown = FrameBuilder.Frame5.CreateFrame(new Size(60, 25));

            Button exit = new Button("_exit", exitText, exitPos, new Size(60, 25), FontSurface.AgateSans10);
            exit.BackgroundImage = exitBackground;
            exit.TextAlignement = TextAlignement.Center;
            exit.MouseOverTexture = exitOver;
            exit.MouseDownTexture = exitDown;

            exit.BackgroundColor = Color.Transparent;
            exit.MouseUp += delegate
            {
                Window.Instance.GameRunning = false;
            };



            string loginText = localization.Default.Strings.GetFormatedString("Main_Menu_Login");
            Point loginPos = new Point(Program.ScreenWidth / 2 - 30, 400);

            Surface loginBackground = FrameBuilder.Frame1.CreateFrame(new Size(60, 25));
            Surface loginOver = FrameBuilder.Frame4.CreateFrame(new Size(60, 25));
            Surface loginDown = FrameBuilder.Frame5.CreateFrame(new Size(60, 25));

            Button login = new Button("_login", loginText, loginPos, new Size(60, 25), FontSurface.AgateSans10);
            login.BackgroundImage = loginBackground;
            login.TextAlignement = TextAlignement.Center;
            login.MouseOverTexture = loginOver;
            login.MouseDownTexture = loginDown;

            login.BackgroundColor = Color.Transparent;

            Point usernamePos = new Point(Program.ScreenWidth / 2 - 100, 300);
            TextBox username = new TextBox("_username", usernamePos, new Size(200, 25));
            Point passwordPos = new Point(Program.ScreenWidth / 2 - 100, 350);
            TextBox password = new TextBox("_password", passwordPos, new Size(200, 25));
            password.PasswordCharacter = '*';
            password.UsePasswordCharacter = true;


            login.MouseUp += delegate
            {
                if (LoginRequest == null)
                    throw new GUIException("LoginRequest was never assigned anything!");
                LoginRequest(username.Text, password.Text);
            };





            Controls.Add(10, login);
            Controls.Add(10, username);
            Controls.Add(10, password);

            Controls.Add(10, title);
            Controls.Add(10, exit);
            Controls.Add(10, about);

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
            Controls["_username"].HasFocus = true;
        }

        public override void Unload()
        {
            base.Unload();
            //This is called when the window is no longer the focused screen.
            //Any code to run should go here.
            (Controls["_password"] as TextBox).Text = "";
        }
    }
    /// <summary>
    /// If you make your own About screen, please leave the "Made with the Tortoise MOG Framework." In it.
    /// </summary>
    class About : Screen
    {


        public About()
        {
        }

        /// <summary>
        /// Initiates the Class. This should only be called from the Load() function's Base code in the Control function.
        /// </summary>
        public override void Init()
        {
            if (_inited) return;
            //This is called when the window is initially created.
            base.Init();

            BackgroundColor = Color.Wheat;

            Label title = new Label("_title", Program.GameName, new Point(10, 10), new Size(Program.ScreenWidth - 20, 40), FontSurface.AgateSans24);

            string creditText = localization.Default.Strings.GetFormatedString("Credits_Text");
            Label cointents = new Label("_contents", creditText, new Point(0, 0), new Size(Program.ScreenWidth, Program.ScreenHeight), FontSurface.AgateSans10);

            Point returnPos = new Point(Program.ScreenWidth - 250, Program.ScreenHeight - 25);
            Surface returnBackground = FrameBuilder.Frame1.CreateFrame(new Size(250, 25));
            Surface returnOver = FrameBuilder.Frame4.CreateFrame(new Size(250, 25));
            Surface returnDown = FrameBuilder.Frame5.CreateFrame(new Size(250, 25));
            string returnText = localization.Default.Strings.GetFormatedString("Credits_Return");
            Button returnButton = new Button("_return", returnText, returnPos, new Size(250, 25), FontSurface.AgateSans10);
            returnButton.BackgroundImage = returnBackground;
            returnButton.TextAlignement = TextAlignement.Center;
            returnButton.MouseDownTexture = returnDown;
            returnButton.MouseOverTexture = returnOver;

            title.TextAlignement = TextAlignement.Center;
            title.BackgroundColor = Color.Transparent;

            cointents.TextAlignement = TextAlignement.Center;
            cointents.BackgroundColor = Color.Transparent;

            returnButton.BackgroundColor = Color.FromArgb(40, Color.Gray);

            returnButton.MouseUp += delegate
            {
                Window.Instance.ChangeToScreen("MainMenu");
            };

            Controls.Add(10, title);
            Controls.Add(15, cointents);
            Controls.Add(10, returnButton);
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

