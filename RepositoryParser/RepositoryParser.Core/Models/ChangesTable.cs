using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Models
{
    public class ChangesTable
    {
        #region Variables
        public int ID { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
        public string TextA { get; set; }
        public string TextB { get; set; }
        public int NR_Commit { get; set; }
        #endregion

      
        #region Constructors

        public ChangesTable(int id, string type, string path, string texta, string textb)
        {
            this.ID = id;
            this.Type = type;
            this.Path = path;
            this.TextA = texta;
            this.TextB = textb;
        }
        public ChangesTable(string type, string path, string texta, string textb)
        {
            this.Type = type;
            this.Path = path;
            this.TextA = texta;
            this.TextB = textb;
        }
        public ChangesTable(string type, string path, string texta, string textb, int nr_commit)
        {
            this.Type = type;
            this.Path = path;
            this.TextA = texta;
            this.TextB = textb;
            this.NR_Commit = nr_commit;
        }
        #endregion

        #region querys
        public static string CreateTableQuery = "create table Changes(ID INTEGER PRIMARY KEY AUTOINCREMENT, Type varchar(50), Path varchar(60), TextA varchar(4000), TextB varchar(4000))";

        public static string InsertSqliteQuery(string type, string path, string texta, string textb)
        {
            if (texta.Contains("'"))
                texta = SqLiteService.StripSlashes(texta);
            if (textb.Contains("'"))
                textb = SqLiteService.StripSlashes(texta);

            string query = "Insert into Changes (Type, Path, TextA, TextB) values (" +
                           "'" + type + "'" +
                           ", '" + path + "'" +
                           ", '" + texta + "'" +
                           ", '" + textb + "')";
            return query;
        }
        public static string InsertSqliteQuery(ChangesTable obj)
        {
            if (obj.TextA.Contains("'"))
                obj.TextA = SqLiteService.StripSlashes(obj.TextA);
            if (obj.TextB.Contains("'"))
                obj.TextB = SqLiteService.StripSlashes(obj.TextB);

            string query = "Insert into Changes (Type, Path, TextA, TextB) values (" +
                           "'" + obj.Type + "'" +
                           ", '" + obj.Path + "'" +
                           ", '" + obj.TextA + "'" +
                           ", '" + obj.TextB + "')";
            return query;
        }
        public static int GetLastIndex(SQLiteConnection Connection)
        {
            int id = 0;
            string query = "select ID from Changes";
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                id = Convert.ToInt32(reader["ID"]);
            }
            return id;
        }



        public List<ChangesTable> GetDataFromBase(SQLiteConnection Connection)
        {
            List<ChangesTable> tempList = new List<ChangesTable>();

            string query = "select * from Changes";
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["ID"]);
                string type = Convert.ToString(reader["Type"]);
                string path = Convert.ToString(reader["Path"]);
                string texta = Convert.ToString(reader["TextA"]);
                string textb = Convert.ToString(reader["TextB"]);
                ChangesTable tempInstance = new ChangesTable(id, type, path, texta, textb);
                tempList.Add(tempInstance);
            }
            return tempList;
        }

        public static string deleteAllQuery = "DELETE FROM Changes";
        #endregion
    }
}
