///*
// * Copyright 2012 Matthew Cash. All rights reserved.
// * 
// * Redistribution and use in source and binary forms, with or without modification, are
// * permitted provided that the following conditions are met:
// * 
// *    1. Redistributions of source code must retain the above copyright notice, this list of
// *       conditions and the following disclaimer.
// * 
// *    2. Redistributions in binary form must reproduce the above copyright notice, this list
// *       of conditions and the following disclaimer in the documentation and/or other materials
// *       provided with the distribution.
// * 
// * THIS SOFTWARE IS PROVIDED BY Matthew Cash ``AS IS'' AND ANY EXPRESS OR IMPLIED
// * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Matthew Cash OR
// * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
// * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// * 
// * The views and conclusions contained in the software and documentation are those of the
// * authors and should not be interpreted as representing official policies, either expressed
// * or implied, of Matthew Cash.
// */

//using System;
//using System.Drawing;
//using Tortoise.Client.Exceptions;
//using Tortoise.Client.Rendering;
//using Tortoise.Client.Rendering.GUI;
//using Tortoise.Shared.Module;
//using Tortoise.Shared.IO;
//using Tortoise.Shared.Exceptions;
//using Tortoise.Client.Module.GUI.FrameBuilder;
//using Tortoise.Shared.Localization;

//namespace Tortoise.Client.Module
//{

//    /// <summary>
//    /// Load up the Main Menu Module.
//    /// </summary>
//    internal class MainMenuLoader : ModuleLoader
//    {
//        public override Version Version
//        {
//            get
//            {
//                return new Version(1, 0, 0, 0);
//            }
//        }

//        public override string Name
//        {
//            get
//            {
//                return "Tortoise Main Menu and About Screen.";
//            }
//        }

//        public override void Load()
//        {
//            if (Window.AvailableScreens.ContainsKey("MainMenu") || Window.AvailableScreens.ContainsKey("About"))
//                throw new ModuleLoadException("The Current screen has already been set!");
//            Window.AvailableScreens.Add("MainMenu", new MainMenu());
//            Window.AvailableScreens.Add("About", new About());
//        }
//    }

//    /// <summary>
//    ///  The Main Menu.
//    /// </summary>
//    class MainMenu : Screen
//    {

//        public static Action<string, string> LoginRequest;

//        public MainMenu()
//        {
//        }



//        /// <summary>
//        /// Initiates the Class. This should only be called from the Load() function's Base code in the Control function.
//        /// </summary>
//        public override void Init()
//        {
//            if (_inited) return;
//            //This is called when the window is initially created.
//            base.Init();

//            BackgroundColor = Color.Wheat;

//            Label title = new Label("_title", Program.GameName, new Point(10, 10), new Size(Window.ScreenWidth - 20, 40), FontInfo.GetInstance(24, FontTypes.Sans));
//            title.TextAlignement = TextAlignement.Center;
//            title.BackgroundColor = Color.Transparent;

//            string aboutText = DefaultLanguage.Strings.GetFormatedString("Main_Menu_Credits");
//            Point aboutPos = new Point(Window.ScreenWidth - 60, Window.ScreenHeight - 25);

//            Surface aboutBackground = FrameBuilder.Frame1.CreateFrame(new Size(60, 25));
//            Surface aboutOver = FrameBuilder.Frame4.CreateFrame(new Size(60, 25));
//            Surface aboutDown = FrameBuilder.Frame5.CreateFrame(new Size(60, 25));

//            Button about = new Button("_about", aboutText, aboutPos, new Size(60, 25), FontInfo.GetInstance(10, FontTypes.Sans));
//            about.BackgroundImage = aboutBackground;
//            about.TextAlignement = TextAlignement.Center;
//            about.MouseOverTexture = aboutOver;
//            about.MouseDownTexture = aboutDown;

//            about.BackgroundColor = Color.Transparent;
//            about.MouseUp += delegate
//            {
//                Window.Instance.ChangeToScreen("About");
//            };

//            string versionText = string.Format("{0}.{1}.{2}.{3}", Program.Version.Major, Program.Version.Minor, Program.Version.Build, Program.Version.Revision);
//            Point versionPos = new Point(5, Window.ScreenHeight - 15);

//            Label version = new Label("_version", versionText, versionPos, new Size(120, 12), FontInfo.GetInstance(10, FontTypes.Sans_Mono));
//            version.BackgroundColor = Color.Transparent;


//            string exitText = DefaultLanguage.Strings.GetFormatedString("Main_Menu_Exit");
//            Point exitPos = new Point(Window.ScreenWidth / 2 - 30, 500);

//            Surface exitBackground = FrameBuilder.Frame1.CreateFrame(new Size(60, 25));
//            Surface exitOver = FrameBuilder.Frame4.CreateFrame(new Size(60, 25));
//            Surface exitDown = FrameBuilder.Frame5.CreateFrame(new Size(60, 25));

//            Button exit = new Button("_exit", exitText, exitPos, new Size(60, 25), FontInfo.GetInstance(10, FontTypes.Sans));
//            exit.BackgroundImage = exitBackground;
//            exit.TextAlignement = TextAlignement.Center;
//            exit.MouseOverTexture = exitOver;
//            exit.MouseDownTexture = exitDown;

//            exit.BackgroundColor = Color.Transparent;
//            exit.MouseUp += delegate
//            {
//                Window.Instance.GameRunning = false;
//            };



//            string loginText =
//            Point loginPos = new Point(Window.ScreenWidth / 2 - 30, 400);

//            Surface loginBackground = FrameBuilder.Frame1.CreateFrame(new Size(60, 25));
//            Surface loginOver = FrameBuilder.Frame4.CreateFrame(new Size(60, 25));
//            Surface loginDown = FrameBuilder.Frame5.CreateFrame(new Size(60, 25));

//            Button login = new Button("_login", loginText, loginPos, new Size(60, 25), FontInfo.GetInstance(10, FontTypes.Sans));
//            login.BackgroundImage = loginBackground;
//            login.TextAlignement = TextAlignement.Center;
//            login.MouseOverTexture = loginOver;
//            login.MouseDownTexture = loginDown;

//            login.BackgroundColor = Color.Transparent;

//            Point usernamePos = new Point(Window.ScreenWidth / 2 - 100, 300);
//            TextBox username = new TextBox("_username", usernamePos, new Size(200, 25));
//            Point passwordPos = new Point(Window.ScreenWidth / 2 - 100, 350);
//            TextBox password = new TextBox("_password", passwordPos, new Size(200, 25));
//            password.PasswordCharacter = '*';
//            password.UsePasswordCharacter = true;


//            login.MouseUp += delegate
//            {
//                if (LoginRequest == null)
//                    throw new GUIException("LoginRequest was never assigned anything!");
//                LoginRequest(username.Text, password.Text);
//            };





//            Controls.Add(10, login);
//            Controls.Add(10, username);
//            Controls.Add(10, password);

//            Controls.Add(10, version);
//            Controls.Add(10, title);
//            Controls.Add(10, exit);
//            Controls.Add(10, about);

//        }

//        public override void OnResize()
//        {
//            Size = Window.Instance.Size;
//        }

//        public override void Load()
//        {
//            base.Load();
//            //This is called right before the window becomes the focused screen.
//            //Any code to run should go here.
//            Controls["_username"].HasFocus = true;
//        }

//        public override void Unload()
//        {
//            base.Unload();
//            //This is called when the window is no longer the focused screen.
//            //Any code to run should go here.
//            (Controls["_password"] as TextBox).Text = "";
//        }
//    }
//    /// <summary>
//    /// If you make your own About screen, please leave the "Made with the Tortoise MOG Framework." In it.
//    /// </summary>
//    class About : Screen
//    {


//        public About()
//        {
//        }

//        /// <summary>
//        /// Initiates the Class. This should only be called from the Load() function's Base code in the Control function.
//        /// </summary>
//        public override void Init()
//        {
//            if (_inited) return;
//            //This is called when the window is initially created.
//            base.Init();

//            BackgroundColor = Color.Wheat;

//            Label title = new Label("_title", Program.GameName, new Point(10, 10), new Size(Window.ScreenWidth - 20, 40), FontInfo.GetInstance(24, FontTypes.Sans));

//            string creditText = DefaultLanguage.Strings.GetFormatedString("Credits_Text");
//            Label cointents = new Label("_contents", creditText, new Point(0, 0), new Size(Window.ScreenWidth, Window.ScreenHeight), FontInfo.GetInstance(10, FontTypes.Sans));

//            Point returnPos = new Point(Window.ScreenWidth - 250, Window.ScreenHeight - 25);
//            Surface returnBackground = FrameBuilder.Frame1.CreateFrame(new Size(250, 25));
//            Surface returnOver = FrameBuilder.Frame4.CreateFrame(new Size(250, 25));
//            Surface returnDown = FrameBuilder.Frame5.CreateFrame(new Size(250, 25));
//            string returnText = DefaultLanguage.Strings.GetFormatedString("Credits_Return");
//            Button returnButton = new Button("_return", returnText, returnPos, new Size(250, 25), FontInfo.GetInstance(10, FontTypes.Sans));
//            returnButton.BackgroundImage = returnBackground;
//            returnButton.TextAlignement = TextAlignement.Center;
//            returnButton.MouseDownTexture = returnDown;
//            returnButton.MouseOverTexture = returnOver;

//            title.TextAlignement = TextAlignement.Center;
//            title.BackgroundColor = Color.Transparent;

//            cointents.TextAlignement = TextAlignement.Center;
//            cointents.BackgroundColor = Color.Transparent;

//            returnButton.BackgroundColor = Color.FromArgb(40, Color.Gray);

//            returnButton.MouseUp += delegate
//            {
//                Window.Instance.ChangeToScreen("MainMenu");
//            };

//            Controls.Add(10, title);
//            Controls.Add(15, cointents);
//            Controls.Add(10, returnButton);
//        }

//        public override void OnResize()
//        {
//            Size = Window.Instance.Size;
//        }

//        public override void Load()
//        {
//            base.Load();
//            //This is called right before the window becomes the focused screen.
//            //Any code to run should go here.
//        }

//        public override void Unload()
//        {
//            base.Unload();
//            //This is called when the window is no longer the focused screen.
//            //Any code to run should go here.
//        }
//    }
//}

