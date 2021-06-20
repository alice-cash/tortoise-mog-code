/*
 * Copyright 2014 Matthew Cash. All rights reserved.
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

/*
 * File: Tortoise.Client.Module.Login.cs
 * Description : Provides Authorization and Login functionality to a remote server.
 * Dependencies: None
 * Console Variables: None
 * Console Functions: None
 */

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
//using System.Drawing;

using Tortoise.Client.Net;

using Tortoise.Graphics.Rendering;
using Tortoise.Graphics.Rendering.GUI;

using StormLib;
using StormLib.Exceptions;
using Tortoise.Shared.IO;
using StormLib.Module;
using Tortoise.Shared.Net;
using StormLib.Localization;
using Tortoise.Graphics;
using Tortoise.Shared.Drawing;

namespace Tortoise.Client.Module
{

    internal class LoginLoader : IModuleLoader
    {
        public const ushort ClientModuleComID = 10010;
        public const ushort ServerModuleComID = 20010;

        public Version Version
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        public string Name
        {
            get
            {
                return "Tortoise Login Manager.";
            }
        }
        static Login _instance;
        public void Load()
        {
            _instance = new Login();
            Connection.AddModuleHandle(ClientModuleComID, _instance);
            if (MainMenuScreen.LoginRequest != null)
                throw new ModuleLoadException("MainMenu.LoginRequest has already been set!");
            MainMenuScreen.LoginRequest = _instance.LoginRequest;

            if (Program.GameLogic.Graphics.Window.AvailableScreens.ContainsKey("LoginStatusScreen"))
                throw new ModuleLoadException("A LoginStatusScreen screen has already been set!");
            LoginStatusScreen.Instance = new LoginStatusScreen(Program.GameLogic.Graphics);
            Program.GameLogic.Graphics.Window.AvailableScreens.Add("LoginStatusScreen", LoginStatusScreen.Instance);
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
        private const byte _packet_AuthKey = 200;
        private const byte _packet_LoginRequest = 202;
        private const byte _packet_LoginResponce = 207;
        private const byte _packet_Version = 209;

        private string serverIP = "127.0.0.1";
        private int serverPort = 9974;

        private string username = "", password = "";


        public byte[] AuthKey;



        public Login()
        {
        }

        public void Communication(Connection Sender, ByteReader data)
        {
            var ComID = data.ReadByte();
            if (!ComID)
            {
                Sender.SyncError("Could not read Packet ID");
                return;
            }
            switch (ComID.Result)
            {
                case _packet_AuthKey:
                    if (!ReadAuthKey(Sender, data))
                        return;
                    SendLogin();
                    break;
                case _packet_LoginResponce:
                    if (!ReadLoginResponce(Sender, data))
                        return;

                    break;
            }
        }

        private ExecutionState ReadLoginResponce(Connection Sender, ByteReader data)
        {
            var sucess = data.ReadBoolean();
            if (!sucess)
            {
                var dbglvl0 = new Dictionary<string, object>
                {
                    { "Location", "Tortoise.Client.Module.Login.ReadLoginResponce.1" }
                };
                var dbglvl1 = new Dictionary<string, object>(dbglvl0)
                {
                    { "length Error", sucess.Reason }
                };
                var dbglvl2 = new Dictionary<string, object>(dbglvl1)
                {
                    { "ByteReader Dump", data.DumpDebugInfo() }
                };
                Debugging.SyncError(Sender, dbglvl0, dbglvl1, dbglvl2);
                return ExecutionState.Failed();
            }

            if (sucess.Result)
                LoginStatusScreen.Instance.Text = DefaultLanguage.Strings.GetFormatedString("Login_Status_Sucess");
            else
                LoginStatusScreen.Instance.Text = DefaultLanguage.Strings.GetFormatedString("Login_Status_Failure");


            return ExecutionState.Succeeded();
        }

        private ExecutionState ReadAuthKey(Connection Sender, ByteReader data)
        {
            var length = data.ReadUShort();
            if (!length)
            {
                var dbglvl0 = new Dictionary<string, object>
                {
                    { "Location", "Tortoise.Client.Module.Login.ReadAuthKey.1" }
                };
                var dbglvl1 = new Dictionary<string, object>(dbglvl0)
                {
                    { "length Error", length.Reason }
                };
                var dbglvl2 = new Dictionary<string, object>(dbglvl1)
                {
                    { "ByteReader Dump", data.DumpDebugInfo() }
                };
                Debugging.SyncError(Sender, dbglvl0, dbglvl1, dbglvl2);
                return ExecutionState.Failed();
            }
            var key = data.ReadBytes(length.Result);
            if (!key)
            {
                var dbglvl0 = new Dictionary<string, object>
                {
                    { "Location", "Tortoise.Client.Module.Login.ReadAuthKey.2" }
                };
                var dbglvl1 = new Dictionary<string, object>(dbglvl0)
                {
                    { "key Error", key.Reason }
                };
                var dbglvl2 = new Dictionary<string, object>(dbglvl1)
                {
                    { "ByteReader Dump", data.DumpDebugInfo() }
                };
                Debugging.SyncError(Sender, dbglvl0, dbglvl1, dbglvl2);
                return ExecutionState.Failed();
            }

            AuthKey = key.Result;
            return ExecutionState.Succeeded();
        }

        private void SendLogin()
        {
            LoginStatusScreen.Instance.Text = DefaultLanguage.Strings.GetFormatedString("Login_Status_Authenticating");

            string hashedPassword;
            hashedPassword = MD5String(username + MD5String(password), AuthKey);

            ByteWriter bw = new ByteWriter();
            bw.Write(_packet_LoginRequest);
            bw.Write(username);
            bw.Write(hashedPassword);

            ServerConnection.MainServerConnection.WriteModulePacket(bw.GetArray(), LoginLoader.ServerModuleComID);
        }

        private string MD5String(string text)
        {
            byte[] orignal;
            MD5 md5;

            md5 = new MD5CryptoServiceProvider();
            orignal = ASCIIEncoding.Default.GetBytes(text);

            return BitConverter.ToString(md5.ComputeHash(orignal));
        }

        private string MD5String(string text, byte[] salt)
        {
            byte[] orignal, salted;
            MD5 md5;

            md5 = new MD5CryptoServiceProvider();
            orignal = ASCIIEncoding.Default.GetBytes(text);
            salted = new Byte[orignal.Length + salt.Length];
            Array.Copy(orignal, salted, orignal.Length);
            Array.Copy(salt, 0, salted, orignal.Length, salted.Length);

            return BitConverter.ToString(md5.ComputeHash(salted));
        }

        public void LoginRequest(string Username, string Password)
        {
            Program.GameLogic.Graphics.Window.ChangeToScreen("LoginStatusScreen");
            UpdateStatusText(DefaultLanguage.Strings.GetFormatedString("Login_Status_Connecting"));

            ServerConnection.MainServerConnection = new Connection(serverIP, serverPort);

            ServerConnection.MainServerConnection.MessageEvent += delegate (object sender, MessageEventArgs e)
            {
                switch (e.Message)
                {
                    case MessageID.Null:
                        UpdateStatusText(DefaultLanguage.Strings.GetFormatedString("Server_Message_Null"));
                        break;
                    case MessageID.OutOfDate:
                        UpdateStatusText(DefaultLanguage.Strings.GetFormatedString("Server_Message_OutOfDate"));
                        break;
                    case MessageID.SyncError:
                        UpdateStatusText(DefaultLanguage.Strings.GetFormatedString("Server_Message_SyncError"));
                        break;

                }
            };
            ServerConnection.MainServerConnection.DisconnectedEvent += delegate (object sender, EventArgs e)
            {

            };

            UpdateStatusText(DefaultLanguage.Strings.GetFormatedString("Login_Status_Connected"));

            ByteWriter bw = new ByteWriter();
            bw.Write(_packet_Version);
            bw.Write(Convert.ToByte(Program.Version.Major));
            bw.Write(Convert.ToByte(Program.Version.Minor));
            bw.Write(Convert.ToUInt16(Program.Version.Build));
            bw.Write(Convert.ToUInt16(Program.Version.Revision));

            username = Username;
            password = Password;



            ServerConnection.MainServerConnection.WriteModulePacket(bw.GetArray(), LoginLoader.ServerModuleComID);
        }

        private void UpdateStatusText(string message)
        {
            if (LoginStatusScreen.Instance.InvokeRequired())
            {
                LoginStatusScreen.Instance.InvokeMethod((nulldata) =>
                {
                    LoginStatusScreen.Instance.Text = message;
                }, null);
            }
            else
            {
                LoginStatusScreen.Instance.Text = message;
            }
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

        public LoginStatusScreen(TGraphics graphics) : base(graphics)
        {
            
            BackgroundColor = System.Drawing.Color.Wheat;
            Label cointents = new Label(Program.GameLogic.Graphics, "_contents", "", new Point(0, 0), new Size(Program.GameLogic.Graphics.ScreenSize.Width, Program.GameLogic.Graphics.ScreenSize.Height))
            {
                TextAlignement = TextAlignement.Center
            };
            Controls.Add(10, cointents);

        }

        public new void OnResize()
        {
            base.OnResize();
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

        public override void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
