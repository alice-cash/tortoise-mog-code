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
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Tortoise.Server;
using Tortoise.Server.Text;
using Tortoise.Server.XML;
using Tortoise.Shared;
using Tortoise.Shared.Exceptions;
using Tortoise.Shared.IO;
using StormLib;
using StormLib.Module;
using Tortoise.Shared.Net;
using StormLib.Exceptions;

namespace Tortoise.Server.Module
{
	/// <summary>
	/// A username/password login module for testing/debuging.
	/// </summary>
	internal class Dbg_StaticLoginLoader : IModuleLoader
	{
		
		public Version Version {
			get {
				return new Version(1,0,0,0);
			}
		}
		
		public string Name {
			get {
				return "Debugging StaticLogin.";
			}
		}

		public void Load()
		{
			if(Login.LoginAttempt != null)
				throw new ModuleLoadException("Login.LoginAttempt has already been set!");
			
			//as this is executed during runtime, not start up, we don't want to use exceptions.
            Login.LoginAttempt = TestLogin;
		}

        private ExecutionState<bool> TestLogin(Connection Sender, string username, string HashedPassword)
        {
            var accountdata = ConnectionData.GetPlayerData(Sender);
            var data = Data.Tables.Account.GetAccountByUsername(username);
            if (!accountdata.ByteArrayValues.ContainsKey("AuthKey"))
                return ExecutionState<bool>.Failed("Connection has no AuthKey! Cannot recompute hash!");
            byte[] key = accountdata.ByteArrayValues["AuthKey"];

            if(data.Sucess == false)
                return ExecutionState<bool>.Succeeded(false);



            if (data.Result.Username != username)
                return ExecutionState<bool>.Succeeded(false);

            // For a standard user login system(mysql, etc) the password is an unsalted
            // md5 hash. Yes yes, unsalted is vulnerable to rainbow tables but if someone has
            // a database dump, you have other things to worry about.

            string _hpwd;
            _hpwd = MD5String(username + MD5String(data.Result.Password), key);

            if (_hpwd != HashedPassword)
                return ExecutionState<bool>.Succeeded(false);
            return ExecutionState<bool>.Succeeded(true);
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
			Array.Copy(orignal, salted,orignal.Length);
			Array.Copy(salt, 0, salted, orignal.Length, salted.Length);

			return BitConverter.ToString(md5.ComputeHash(salted));
		}

	}
}
