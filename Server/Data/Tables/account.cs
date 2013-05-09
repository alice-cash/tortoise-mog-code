using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Tortoise.Shared;

namespace Tortoise.Server.Data.Tables
{
    class account
    {
        private static string _getAccountByUsernameQuery = "Select ID, username, password FROM accounts where username = @username";
        public static ExecutionState<account> GetAccountByUsername(string username)
        {
            
            DataTable sqlData = Data.Mysql.Instance.RunQuery(_getAccountByUsernameQuery, KVP("username", username));
            if (sqlData.Rows.Count == 0)
                return new ExecutionState<account>(false, null);
            return initalize(sqlData);
        }

        public int ID { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        private account()
        {

        }

        private static ExecutionState<account> initalize(DataTable data)
        {
            if (!data.Columns.Contains("ID") || !data.Columns.Contains("username") || !data.Columns.Contains("password"))
                return new ExecutionState<account>(false, null);

            account acc = new account();
            acc.ID = (int)data.Rows[0]["ID"];
            acc.Username = data.Rows[0]["username"] as string;
            acc.Password = data.Rows[0]["password"] as string;
            return new ExecutionState<account>(true, acc);
        }

        private static ExecutionState<List<account>> initalizeRows(DataTable data)
        {
            if (data.Columns.Contains("ID") || data.Columns.Contains("username") || data.Columns.Contains("password"))
                return new ExecutionState<List<account>>(false, null);
            List<account> accList = new List<account>();
            foreach (DataRow dr in data.Rows)
            {
                account acc = new account();
                acc.ID = (int)dr["ID"];
                acc.Username = dr["username"] as string;
                acc.Password = dr["password"] as string;
                accList.Add(acc);
            }
            return new ExecutionState<List<account>>(true, accList);

        }


        private static KeyValuePair<string, object> KVP(string Key, object obj)
        {
            return new KeyValuePair<string, object>(Key, obj);
        }

    }
}
