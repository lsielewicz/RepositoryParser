using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Models
{
    public class BranchForRepoTable
    {
        public int ID_BranchForRepo { get; set; }
        public int NR_GitRepository { get; set; }
        public int NR_GitBranch { get; set; }

        public BranchForRepoTable()
        {
            ID_BranchForRepo = 0;
            NR_GitBranch = 0;
            NR_GitRepository = 0;
        }

        public BranchForRepoTable(int id, int nrRepo, int nrBranch)
        {
            ID_BranchForRepo = id;
            NR_GitRepository = nrRepo;
            NR_GitBranch = nrBranch;
        }

        public BranchForRepoTable(int nrRepo, int nrBranch)
        {
            NR_GitRepository = nrRepo;
            NR_GitBranch = nrBranch;
        }

        #region Querys
        public static string CreateTable =
                                  "CREATE TABLE BranchForRepo(ID INTEGER PRIMARY KEY AUTOINCREMENT, NR_GitRepository INTEGER, NR_GitBranch INTEGER)";
        public static string deleteAllQuery = "DELETE FROM BranchForRepo";
        public static string InsertQuery(BranchForRepoTable obj)
        {
            string query = "INSERT INTO BranchForRepo (NR_GitRepository, NR_GitBranch) VALUES (" +
                           obj.NR_GitRepository + ", " + obj.NR_GitBranch + ")";
            return query;
        }
        public List<BranchForRepoTable> GetDataFromBase(SQLiteConnection Connection)
        {
            List<BranchForRepoTable> tempList = new List<BranchForRepoTable>();

            string query = "select * from BranchForRepo";
            SQLiteCommand command = new SQLiteCommand(query, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["ID"]);
                int nr_repo = Convert.ToInt32(reader["NR_GitRepository"]);
                int nr_branch = Convert.ToInt32(reader["NR_GitBranch"]);
                BranchForRepoTable temp = new BranchForRepoTable(id, nr_repo, nr_branch);
                tempList.Add(temp);
            }
            return tempList;
        }
        #endregion  
    }
}
