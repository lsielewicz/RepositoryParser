using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace RepositoryParser.Core.Models
{
    public class RepositoryTable
    {
        public int ID_Repository { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public RepositoryTable()
        {
            ID_Repository = 0;
        }

        public RepositoryTable(int rnumber)
        {
            ID_Repository = rnumber;
        }

        public RepositoryTable(string name, string type)
        {
            Name = name;
            Type = type;
        }
        public RepositoryTable(string name)
        {
            Name = name;
            Type = "GIT";
        }

        public RepositoryTable(int ID, string name)
        {
            ID_Repository = ID;
            Name = name;
        }
        public RepositoryTable(int ID, string name, string type)
        {
            ID_Repository = ID;
            Name = name;
            Type = type;
        }

        #region querys
        public static string createTable = "CREATE TABLE Repository(ID INTEGER PRIMARY KEY AUTOINCREMENT, Name varchar(40), Type varchar(40))";
        public static string deleteAllQuery = "DELETE FROM Repository";

        public static string InsertQuery(RepositoryTable obj)
        {
            obj.Name = RepositoryTable.FixName(obj.Name);
            string query = "INSERT INTO Repository (Name,Type) Values ('" + obj.Name + "', '"+ obj.Type+"')";
            return query;
        }

        public List<RepositoryTable> GetDataFromBase(SQLiteConnection Connection)
        {
            List<RepositoryTable> tempList = new List<RepositoryTable>();

            string query = "select * from Repository";
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["ID"]);
                string name = Convert.ToString(reader["Name"]);
                string type = Convert.ToString(reader["Type"]);
                RepositoryTable temp = new RepositoryTable(id, name, type);
                tempList.Add(temp);
            }
            return tempList;
        }

        public int GetLastIndex(SQLiteConnection Connection)
        {
            int id = 0;
            string query = "select ID from Repository";
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                id = Convert.ToInt32(reader["ID"]);
            }
            return id;
        }

        public static string FixName(string Repository)
        {
            string output = Repository;
           // string pattern = @"((.*)\\(.*)\\)";
            string pattern = @"(.*)\\(.*)\\.git";
            Regex r = new Regex(pattern);
            Match m = r.Match(Repository);
            if (m.Success)
            {
                if (m.Groups.Count >= 2)
                {
                    output = m.Groups[2].Value;
                }
            }
            return output;
        }
        #endregion
    }
}
