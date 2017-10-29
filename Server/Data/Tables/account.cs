using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using StormLib;

namespace Tortoise.Server.Data.Tables
{
    class Account
    {
        private static string _getAccountByUsernameQuery = "Select ID, username, password FROM accounts where username = @username";
        public static ExecutionState<Account> GetAccountByUsername(string username)
        {
            
            DataTable sqlData = Data.Mysql.Instance.RunQuery(_getAccountByUsernameQuery, KVP("username", username));
            if (sqlData.Rows.Count == 0)
                return new ExecutionState<Account>(false, null);
            return Initalize(sqlData);
        }

        public int ID { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        private Account()
        {

        }

        private static ExecutionState<Account> Initalize(DataTable data)
        {
            if (!data.Columns.Contains("ID") || !data.Columns.Contains("username") || !data.Columns.Contains("password"))
                return new ExecutionState<Account>(false, null);

            Account acc = new Account()
            {
                ID = (int)data.Rows[0]["ID"],
                Username = data.Rows[0]["username"] as string,
                Password = data.Rows[0]["password"] as string
            };
            return new ExecutionState<Account>(true, acc);
        }

        private static ExecutionState<List<Account>> InitalizeRows(DataTable data)
        {
            if (data.Columns.Contains("ID") || data.Columns.Contains("username") || data.Columns.Contains("password"))
                return new ExecutionState<List<Account>>(false, null);
            List<Account> accList = new List<Account>();
            foreach (DataRow dr in data.Rows)
            {
                Account acc = new Account()
                {
                    ID = (int)dr["ID"],
                    Username = dr["username"] as string,
                    Password = dr["password"] as string
                };
                accList.Add(acc);
            }
            return new ExecutionState<List<Account>>(true, accList);

        }


        private static KeyValuePair<string, object> KVP(string Key, object obj)
        {
            return new KeyValuePair<string, object>(Key, obj);
        }

    }
}
