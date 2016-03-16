using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Models
{
    public class CommitForBranchTable
    {
        public int ID_CommitForBranch { get; set; }
        public int NR_Branch { get; set; }
        public int NR_Commit { get; set; }

        public static string CreateTable =
            "CREATE TABLE CommitForBranch(ID INTEGER PRIMARY KEY AUTOINCREMENT, NR_Branch INTEGER, NR_Commit INTEGER)";
        public static string deleteAllQuery = "DELETE FROM CommitForBranch";

        public static string InsertQuery(CommitForBranchTable obj)
        {
            string query = "INSERT INTO CommitForBranch (NR_Branch, NR_Commit) VALUES (" +
                           obj.NR_Branch + ", " + obj.NR_Commit + ")";
            return query;
        }
        public List<CommitForBranchTable> GetDataFromBase(SQLiteConnection Connection)
        {
            List<CommitForBranchTable> tempList = new List<CommitForBranchTable>();

            string query = "select * from CommitForBranch";
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["ID"]);
                int nr_repo = Convert.ToInt32(reader["NR_Branch"]);
                int nr_branch = Convert.ToInt32(reader["NR_Commit"]);
                CommitForBranchTable temp = new CommitForBranchTable(id, nr_repo, nr_branch);
                tempList.Add(temp);
            }
            return tempList;
        }
        public CommitForBranchTable()
        {
            ID_CommitForBranch = 0;
            NR_Branch = 0;
            NR_Commit = 0;
        }

        public CommitForBranchTable(int id, int nrBranch, int nrCommit)
        {
            ID_CommitForBranch = id;
            NR_Branch = nrBranch;
            NR_Commit = nrCommit;
        }

        public CommitForBranchTable(int nrBranch, int nrCommit)
        {
            NR_Branch = nrBranch;
            NR_Commit = nrCommit;
        }
    }
}
