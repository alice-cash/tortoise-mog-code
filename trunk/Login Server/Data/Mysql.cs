/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 7/19/2010
 * Time: 2:24 PM
 * 
 * Copyright 2010 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *	1. Redistributions of source code must retain the above copyright notice, this list of
 *	   conditions and the following disclaimer.
 * 
 *	2. Redistributions in binary form must reproduce the above copyright notice, this list
 *	   of conditions and the following disclaimer in the documentation and/or other materials
 *	   provided with the distribution.
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
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using SharedServerLib.Exceptions;
namespace LoginServer.Data
{
	/// <summary>
	/// Description of Mysql.
	/// </summary>
	public class Mysql
	{
		private static Mysql _instance;
		public static Mysql Instance
		{
			get
			{
				if(_instance != null)
					_instance = new Mysql();
				return _instance;
			}
		}
		
		private MySqlConnection _connection;
		private MySqlCommand _command;
		//private MySqlDataReader Reader;

		// MyConString = "Database=spacetraders;Data Source=localhost;User=root;Password=";
		string _myConString = "Database=Tortus;Data Source=localhost;User=Tortus;Password=BrCFWATJ7Z2uqvNS";


		public Mysql()
		{
		}
		
		~Mysql()
	   {
			if (_connection != null && _connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
	   }
		 
		private int RunCommnd(string query, params KeyValuePair<string, object>[] param)
		{
			//string FormatedQuery = string.Format(Query, param);

			CheckConnection();
			_command = _connection.CreateCommand();
			_command.CommandText = query;


			foreach (KeyValuePair<string, object> Obj in param)
			{
				MySqlParameter oParam = _command.Parameters.Add(Obj.Key, GetType(Obj.Value));
				oParam.Value = Obj.Value;
			}

			return _command.ExecuteNonQuery();
		}

		public MySqlDbType GetType(object obj)
		{

			Type T = obj.GetType();

			if (T == typeof(string))
				return MySqlDbType.String;

			if (T == typeof(byte))
				return MySqlDbType.Byte;

			if (T == typeof(int))
				return MySqlDbType.Int32;

			if (T == typeof(short))
				return MySqlDbType.Int16;

			if (T == typeof(long))
				return MySqlDbType.Int64;

			if (T == typeof(DateTime))
				return MySqlDbType.DateTime;

			if (T == typeof(Enum))
				return MySqlDbType.Enum;

			if (T == typeof(byte[]))
				return MySqlDbType.Blob;

			return MySqlDbType.String;

		}

		private System.Data.DataTable RunQuery(string query, params KeyValuePair<string, object>[] param)
		{

			//string FormatedQuery = string.Format(Query, param);

			MySqlDataReader reader = null;
			DataTable DT = new System.Data.DataTable();
			CheckConnection();

			_command = _connection.CreateCommand();
			_command.CommandText = query;

			foreach (KeyValuePair<string, object> Obj in param)
			{
				MySqlParameter oParam = _command.Parameters.Add(Obj.Key, GetType(Obj.Value));
				oParam.Value = Obj.Value;
			}
			try
			{
				reader = _command.ExecuteReader();

				DT.Load(reader);
				_command.Dispose();
				reader.Close();
			}
			finally
			{
				if (reader != null)
					reader.Close();

				_command.Dispose();

			}
			return DT;
		}
		
		private static KeyValuePair<string, object> KVP(string Key, object obj)
		{
			return new KeyValuePair<string, object>(Key, obj);
		}
			 
		public void CheckConnection()
		{
			if (_connection == null || _connection.State == ConnectionState.Closed)
			{
				_connection = new MySqlConnection(_myConString);
				try{
				_connection.Open();
				}catch(MySqlException ex)
				{
					throw new TortusGeneralException("Error with MySql. See innerException.", ex);
				}
				return;
			}
		}
		
		public LoginsTable.LoginsDataTable Query_GetLoginInfo(string login)
		{
			 string command = "SELECT * FROM  `Logins` WHERE `Login` = ?login;";

			 return new LoginsTable.LoginsDataTable(RunQuery(command, KVP("?login", login)));
		}
	}
}
