using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Models
{
    public class ChangesForCommitTable
    {
        public int ID { get; set; }
        public int NR_Commit { get; set; }
        public int NR_Change { get; set; }

        public static string CreateTable =
            "CREATE TABLE ChangesForCommit(ID INTEGER PRIMARY KEY AUTOINCREMENT, NR_Commit INTEGER, NR_Change INTEGER)";
        public static string deleteAllQuery = "DELETE FROM ChangesForCommit";

        public static string InsertQuery(ChangesForCommitTable obj)
        {
            string query = "INSERT INTO ChangesForCommit (NR_Commit, NR_Change) VALUES (" +
                           obj.NR_Commit + ", " + obj.NR_Change + ")";
            return query;
        }

        public List<ChangesForCommitTable> GetDataFromBase(SQLiteConnection Connection)
        {
            List<ChangesForCommitTable> tempList = new List<ChangesForCommitTable>();

            string query = "select * from ChangesForCommit";
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["ID"]);
                int nr_repo = Convert.ToInt32(reader["NR_Commit"]);
                int nr_branch = Convert.ToInt32(reader["NR_Change"]);
                ChangesForCommitTable temp = new ChangesForCommitTable(id, nr_repo, nr_branch);
                tempList.Add(temp);
            }
            return tempList;
        }

        public ChangesForCommitTable(int id, int nr_commit, int nr_change)
        {
            this.ID = id;
            this.NR_Commit = nr_commit;
            this.NR_Change = nr_change;
        }

        public ChangesForCommitTable(int nr_commit, int nr_change)
        {
            this.NR_Commit = nr_commit;
            this.NR_Change = nr_change;
        }
    }
}
