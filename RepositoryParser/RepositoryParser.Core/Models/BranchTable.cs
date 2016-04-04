using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitSharp;

namespace RepositoryParser.Core.Models
{
    public class BranchTable
    {
        public int ID_Branch { get; set; }
        public string Name { get; set; }
        public LibGit2Sharp.Branch Value { get; set; }
        public string Path { get; set; }



        public BranchTable()
        {
            ID_Branch = 0;
            Name = "";
        }
        public BranchTable(int id, string name)
        {
            ID_Branch = id;
            Name = name;
        }

        public BranchTable(string name)
        {
            Name = name;
        }

        public BranchTable(string name, LibGit2Sharp.Branch value)
        {
            Name = name;
            Value = value;
        }
        //svn
        public BranchTable(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
        #region Querys

        public static string CreateTable = "CREATE TABLE Branch(ID INTEGER PRIMARY KEY AUTOINCREMENT, Name varchar(40))";
        public static string deleteAllQuery = "DELETE FROM Branch";
        public static string InsertSqliteQuery(BranchTable obj)
        {
            return "Insert into Branch (Name) values (" +
                           "'" + obj.Name + "')";
        }


        public List<BranchTable> GetDataFromBase(SQLiteConnection Connection, string q = "")
        {
            List<BranchTable> tempList = new List<BranchTable>();

            string query = "select * from Branch";
            if (!string.IsNullOrEmpty(q))
            {
                query = q;
            }
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["ID"]);
                string name = Convert.ToString(reader["Name"]);
                BranchTable temp = new BranchTable(id, name);
                tempList.Add(temp);
            }
            return tempList;
        }
        public int GetLastIndex(SQLiteConnection Connection)
        {
            int lastIndex = 0;

            string query = "select ID from Branch";
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                lastIndex = Convert.ToInt32(reader["ID"]);
            }
            return lastIndex;
        }

        public static string FixRemoteRepositoryName(string remoteRepository)
        {
            string Result = remoteRepository;
            try
            {
                Result = System.Text.RegularExpressions.Regex.Replace(remoteRepository, @"(.*/)", string.Empty);
            }
            catch (Exception Ex)
            {
                // handle any exception here
                Console.WriteLine(Ex.Message);
            }

            return Result;
        }
        #endregion
    }
}
