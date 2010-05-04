﻿/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 5/2/2010
 * Time: 2:24 AM
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
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
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
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace LoginServer.XML
{
	/// <summary>
	/// Configeration Database for Login Server
	/// </summary>
	[Serializable]
	public class LoginServer
	{
		
		public static LoginServer Instance;
		
		
		/// <summary>
		/// This is the name that the server uses when sharing info about its self.
		/// </summary>
		public string ServerName
		{ get; set; }
		
		
		/// <summary>
		/// This is the port the server listen on for clients.
		/// </summary>
		public int ClientListenPort
		{ get; set; }
		
		/// <summary>
		/// this is the address the server listens on for clients.
		/// </summary>
		public string ClientListenAddress
		{ get; set; }
		
		
		/// <summary>
		/// This is the port the server listen on for servers.
		/// </summary>
		public int ServerListenPort
		{ get; set; }
		
		/// <summary>
		/// this is the address the server listens on for clients.
		/// </summary>
		public string ServerListenAddress
		{ get; set; }
		
		public string[] ServerListenAcceptedAddresses
		{ get; set; }
		
		
		
		/// <summary>
		/// This is the Port for the Mysql Database.
		/// </summary>
		public int MysqlPort
		{ get; set; }
		
		/// <summary>
		/// This is the address for the Mysql Database.
		/// </summary>
		public string MysqlAddress
		{ get; set; }
		
		/// <summary>
		/// This is the Account Databse for the Mysql Database.
		/// </summary>
		public string MysqlAccountDatabse
		{ get; set; }

		/// <summary>
		/// This is the Server Databse for the Mysql Database.
		/// </summary>	
		public string MysqlServerDatabse
		{ get; set; }

		/// <summary>
		/// This is the User for the Mysql Database.
		/// </summary>
		public string MysqlUser
		{ get; set; }
		
		/// <summary>
		/// This is the Password for the Mysql Database.
		/// </summary>
		public string MysqlPass
		{ get; set; }
		
		/// <summary>
		/// This is the number of threads that the server will use to handle Clients.
		/// </summary>
		public int ClientListenThreads
		{ get; set; }

		/// <summary>
		/// This is the number of Clients each thread will handle.
		/// </summary>
		public int MaxUsersPerThread
		{ get; set; }

		
		
		
		public string SyncKey
		{	
			get
			{
				return _SyncKey;
			}
			set
			{
				_Key = SharedServerLib.Misc.ByteStringConverter.StringToBytes(value);
				_SyncKey = value;
			}
		}
		public string SyncVector
		{	
			get
			{
				return _SyncKey;
			}
			set
			{
				_Key = SharedServerLib.Misc.ByteStringConverter.StringToBytes(value);	
				_SyncKey = value;
			}
		}
		
		
		public byte[] Key
		{	
			get
			{
				return _Key;
			}
		}

		public byte[] Vector
		{	
			get
			{
				return _Vector;
			}
		}
		
		
		
		
		
		private string _SyncKey;
		private string _SyncVector;
		private	byte[] _Key;
		private	byte[] _Vector;
		
		 
		/// <summary>
		/// This creates a default Config and saves it.
		/// </summary>
		public static void CreateDefault()
		{
			

			
		
		}
		
		public static void LoadConfig()
		{
			if(!File.Exists("./LoginConfig.xml"))
			{
				CreateDefault();
			}
			TextReader reader = new StreamReader("./LoginConfig.xml");
         	XmlSerializer serializer = new XmlSerializer(typeof(LoginServer));
         	LoginServer.Instance = (LoginServer)serializer.Deserialize(reader);
         	reader.Close();
				
			
		
		}
	}
	
	
	
}
