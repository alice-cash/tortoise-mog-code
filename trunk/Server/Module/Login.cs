/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 8/8/2010
 * Time: 3:55 PM
 * 
 * Copyright 2011 Matthew Cash. All rights reserved.
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

using Tortoise.Shared;
using Tortoise.Server;
using Tortoise.Server.Text;
using Tortoise.Shared.Module;
using Tortoise.Shared.IO;
using Tortoise.Shared.Net;
using Tortoise.Server.XML;

namespace Tortoise.Server.Module
{
    class LoginLoader : ModuleLoader
    {
        public const ushort ClientModuleComID = 10010;
        public const ushort ServerModuleComID = 20010;

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
                return "Tortoise Login and Client handle Module.";
            }
        }

        static Login _instance;
        public override void Load()
        {
            _instance = new Login();
            Connection.AddModuleHandle(ServerModuleComID, _instance);

        }
    }


    /// <summary>
    /// Description of Login.
    /// </summary>
    class Login : IComModule
    {

        //Instances of this class are single instances stored in the static variable in the Connection class
        //Because of this, we pass around a reference to what we are actively using, instead of creating a 
        //new instance. That saves memory since we don't need to create a new instance of every class for each connection.

        //These are IDs used by the packet. The random numbers
        //should help catch sync issues.
        //TODO: Change these to different values for your game, make sure they match in the client.
        private const byte _packet_AuthKey = 200;
        private const byte _packet_LoginRequest = 202;
        private const byte _packet_LoginResponce = 207;
        private const byte _packet_Version = 209;

        public delegate ExecutionState<bool> LoginAttemptDelegate(Connection Sender, string username, string HashedPassword);
        public static LoginAttemptDelegate LoginAttempt;

        public Login()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="data"></param>
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
                case _packet_Version:
                    if (!ReadClientInfo(Sender, data))
                        return;

                    SendAuthKey(Sender);
                    break;
                case _packet_LoginRequest:

                    if (!ReadLoginRequest(Sender, data))
                        return;

                    break;
                default:
                    Sender.SyncError("Invalid Packet ID");
                    break;
            }
            
            

        }


        ExecutionState ReadLoginRequest(Connection Sender, ByteReader data)
        {
            if (LoginAttempt == null)
                throw new Exception("LoginAttempt was never assigned!");

            var username = data.ReadString();
            var password = data.ReadString();

            //Error point 1
            if (!username || !password)
            {
                var dbglvl0 = new Dictionary<string, object>();
                dbglvl0.Add("Location", "Tortoise.Server.Module.Login.ReadLoginRequest.1");
                var dbglvl1 = new Dictionary<string, object>(dbglvl0);
                dbglvl1.Add("Username Error", username.Reason);
                dbglvl1.Add("Password Error", username.Reason);
                var dbglvl2 = new Dictionary<string, object>(dbglvl1);
                dbglvl2.Add("ByteReader Dump", data.DumpDebugInfo());

                Debugging.SyncError(Sender, dbglvl0, dbglvl1, dbglvl2);
                return ExecutionState.Failed();
            }
            var result = LoginAttempt(Sender, username.Result, password.Result);
            //Error point 2
            if (!result)
            {
                var dbglvl0 = new Dictionary<string, object>();
                dbglvl0.Add("Location", "Tortoise.Server.Module.Login.ReadLoginRequest.2");
                var dbglvl1 = new Dictionary<string, object>(dbglvl0);
                dbglvl1.Add("LoginAttempt result Error", result.Reason);
                var dbglvl2 = new Dictionary<string, object>(dbglvl1);
                dbglvl2.Add("ByteReader Dump", data.DumpDebugInfo());

                Debugging.SyncError(Sender, dbglvl0, dbglvl1, dbglvl2);
                return ExecutionState.Failed();
            }
            ByteWriter bw = new ByteWriter();
            bw.Write(_packet_LoginResponce);
            bw.Write(result.Result);
            if (!ConnectionData.GetPlayerData(Sender).UIntValues.ContainsKey("UserID"))
                ConnectionData.GetPlayerData(Sender).UIntValues.Add("UserID", 0);
            bw.Write(ConnectionData.GetPlayerData(Sender).UIntValues["UserID"]);

            Sender.WriteModulePacket(bw.GetArray(), LoginLoader.ClientModuleComID);
            return ExecutionState.Succeeded();
        }

        ExecutionState ReadClientInfo(Connection Sender, ByteReader data)
        {
            //(byte major, byte minor, ushort revision)

            var major = data.ReadByte();
            var minor = data.ReadByte();
            var revision = data.ReadUShort();
            if (!major || !minor || !revision)
            {
                Sender.SyncError("Version Data");
                return ExecutionState.Failed();
            }

            if (revision.Result != XML.ServerConfig.Instance.ClientRevision ||
               minor.Result != XML.ServerConfig.Instance.CLientMinor ||
               major.Result != XML.ServerConfig.Instance.ClientMajor)
            {
                Sender.Disconnect(MessageID.OutOfDate);
                return ExecutionState.Failed();
            }
            //Write_TempAuthKey(_authKey);
            return ExecutionState.Succeeded();
        }

        void SendAuthKey(Connection Sender)
        {
            var data = ConnectionData.GetPlayerData(Sender);
            if (!data.ByteArrayValues.ContainsKey("AuthKey"))
            {
                GenerateAuthKey(Sender);
            }
            byte[] key = data.ByteArrayValues["AuthKey"];
            //it doesn't need to be longer than 512 bytes, thats a 4096 bit key
            //this also 
            if (key.Length > 512)
            {
                byte[] tmp = new byte[512];
                Array.Copy(key, tmp, 512);
                key = tmp;
                data.ByteArrayValues["AuthKey"] = key;
            }

            ByteWriter bw = new ByteWriter();
            bw.Write(_packet_AuthKey);
            bw.Write((ushort)key.Length);
            bw.Write(key);

            Sender.WriteModulePacket(bw.GetArray(), LoginLoader.ClientModuleComID);

        }

        /// <summary>
        /// Generates an AuthKey for password hashing or security keys or whatever else the client needs.
        /// </summary>
        /// <param name="Sender"></param>
        void GenerateAuthKey(Connection Sender)
        {
            //we will generate a 512 bit, or 64 byte key. Suitable for hashing and encryption.
            var data = ConnectionData.GetPlayerData(Sender);
            byte[] key = new byte[64];
            Random R = new Random();
            R.NextBytes(key);
            if (data.ByteArrayValues.ContainsKey("AuthKey"))
                data.ByteArrayValues["AuthKey"] = key;
            else
                data.ByteArrayValues.Add("AuthKey", key);
        }



        /*
                 void Write_Message(MessageID reason)
        {
            //2 for ID, 2 for message ID
            ushort length = 4;
            _sw.Write(length);
            _sw.Write(PacketID.ServerMessage.Value());
            _sw.Write(reason.Value());
            _sw.Flush();
        }
		
        public void Write_TempAuthKey(string key)
        {
            //this should never occure, but if it does, its mostlikley due to a horrible,
            //horrable bug, and it should bring the server down and burn their house down.
            //This still needs to be here due to the explicid conversion and my coding rules for this project.
            if(key.Length + 2 >= ushort.MaxValue)
                throw new TortoiseGeneralException("Invalid key length!");
			
            ushort length = (ushort)(2 + key.Length);
            _sw.Write(length);
            _sw.Write(PacketID.Authintication.Value());
            _sw.Write(key);
            _sw.Flush();
        }
		
        private void Write_LoginSucess(bool status)
        {
            //2 for ID, 1 for bool
            ushort length = 3;
            _sw.Write(length);
            _sw.Write(PacketID.LoginSucess.Value());
            _sw.Write(status);
            _sw.Flush();
        }*/


    }
}
