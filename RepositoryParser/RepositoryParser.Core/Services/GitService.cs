using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using GitSharp.Commands;
using LibGit2Sharp;
using RepositoryParser.Core.Interfaces;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Core.Services
{
    public class GitService : IGitService
    {
        #region Fields
        public string DirectoryPath { get; set; }
        public string UrlPath { get; set; }
        public bool IsCloned { get; set; }
        public SqLiteService SqLiteInstance { get; set; }
        #endregion

        #region Constructors
        public GitService()
        {
            DirectoryPath = String.Empty;
        }
        public GitService(string path)
        {
            DirectoryPath = path;
        }
        public GitService(string path, bool clone)
        {
            if (clone)
                UrlPath = path;
            else
                DirectoryPath = path;

            IsCloned = !clone;
            InitializeConnection();
        }
        #endregion

        #region Methods
        public RepositoryTable GetRepository(string path)
        {
            RepositoryTable repositoryTable = new RepositoryTable();
            using (Repository repository = new Repository(path))
            {
                repositoryTable.Type = "GIT";
                repositoryTable.Name = FixName(repository.Info.Path);
            }
            return repositoryTable;
        }

        public List<BranchTable> GetAllBranches(string path)
        {
            List<BranchTable> branchList = new List<BranchTable>();
            using (Repository repository = new Repository(path))
            {
                foreach (Branch branch in repository.Branches)
                {
                    if (!branch.IsRemote)
                    {
                        branchList.Add(new BranchTable(branch.FriendlyName));
                        
                    }
                }
            }
            return branchList;
        }

        public List<CommitTable> GetAllCommits(Branch branch)
        {
            List<CommitTable> commitList = new List<CommitTable>();
            foreach (var commit in branch.Commits)
            {
                string date = commit.Author.When.ToString();
                date = SqLiteService.getDateTimeFormat(date);
                date = date.Remove(19);
                commitList.Add(new CommitTable(commit.MessageShort,commit.Author.Name,date,commit.Author.Email,commit.Sha));
            }
            return commitList;
        }

        public List<ChangesTable> GetAllChanges(Commit commit)
        {
            List<ChangesTable> changesList = new List<ChangesTable>();
            using (Repository repository = new Repository(DirectoryPath))
            {
                bool isInitial = false;
                var firstOrDefault = commit.Parents.FirstOrDefault();
                if (firstOrDefault == null)
                {
                    isInitial = true;
                }
                Tree rootCommitTree = repository.Lookup<Commit>(commit.Id.ToString()).Tree;
                Patch changes;
                if (!isInitial)
                {
                    Tree commitTreeWithUpdatedFile = repository.Lookup<Commit>(commit.Parents.FirstOrDefault().Sha).Tree;
                    changes = repository.Diff.Compare<Patch>(commitTreeWithUpdatedFile, rootCommitTree);
                }
                else
                {
                     changes = repository.Diff.Compare<Patch>(null, rootCommitTree);
                }
                foreach (var change in changes)
                {
                    ChangesTable temp = new ChangesTable();
                    temp.Path = change.Path;
                    if (change.Status == ChangeKind.Deleted)
                        temp.Type = "Deleted";
                    else if (change.Status == ChangeKind.Added)
                        temp.Type = "Added";
                    else
                        temp.Type = "Modified";

                    temp.TextA = change.Patch;
                    temp.TextB = String.Empty;
                    changesList.Add(temp);
                }   
            }     
            return changesList;
        }

        public string GetNameFromPath(string path)
        {
            return FixName(path);
        }

        private string FixName(string name)
        {
            string output = name;
            string pattern = @"(.*)\\(.*)\\.git";
            Regex r = new Regex(pattern);
            Match m = r.Match(name);
            if (m.Success)
            {
                if (m.Groups.Count >= 2)
                {
                    output = m.Groups[2].Value;
                }
            }
            return output;
        }

        public void ConnectRepositoryToDataBase(bool isNewFile = false)
        {
            if (isNewFile)
            {
                string repositoryName = "CommonRepositoryDataBase";
                int repoNumber = 0;
                while (File.Exists("./Databases/" + repositoryName + ".sqlite"))
                {
                    repositoryName = "CommonRepositoryDataBase" + Convert.ToString(repoNumber);
                    repoNumber++;
                }

                SqLiteInstance = SqLiteService.GetInstance();
                SqLiteInstance.DBName = repositoryName;
                List<string> createTableQuerys = new List<string>
                {
                    {RepositoryTable.createTable},
                    {BranchForRepoTable.CreateTable},
                    {BranchTable.CreateTable},
                    {CommitForBranchTable.CreateTable},
                    {CommitTable.SqliteQuery},
                    {ChangesForCommitTable.CreateTable},
                    {ChangesTable.CreateTableQuery}
                };

                SqLiteInstance.OpenConnection(createTableQuerys);
            }
            else
            {
                string RepositoryName = "CommonRepositoryDataBase";
                SqLiteInstance = SqLiteService.GetInstance();
                SqLiteInstance.DBName = RepositoryName;
                SqLiteInstance.OpenConnection();
            }
        }

        public void InitializeConnection()
        {
            try
            {
                if(!IsCloned)
                {
                    if (UrlPath.Length > 0)
                    {
                        string urlPath = Regex.Replace(UrlPath, @"https?", "git");
                        UrlPath = urlPath;
                        GitCloneService cloneService = new GitCloneService(urlPath);
                        cloneService.CloneRepository(true);
                        IsCloned = true;
                        DirectoryPath = cloneService.DirectoryPath;
                    }
                }
                //SQLITE
                string repoPath = "./DataBases/CommonRepositoryDataBase.sqlite";
                if (!File.Exists(repoPath))
                    ConnectRepositoryToDataBase(true);
                else
                    ConnectRepositoryToDataBase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void FillDataBase()
        {
            try
            {
                using (Repository repository = new Repository(DirectoryPath))
                {
                    List<string> transactions = new List<string>();
                    List<string> changesTransactions = new List<string>();
                    RepositoryTable repositoryTable = GetRepository(DirectoryPath);
                    List<BranchTable> branches = GetAllBranches(DirectoryPath);
                    BranchTable tempBranch = new BranchTable();
                    CommitTable tempCommit = new CommitTable();
                    transactions.Add(RepositoryTable.InsertQuery(repositoryTable));

                    int startRepoIndex = repositoryTable.GetLastIndex(SqLiteInstance.Connection) + 1;
                    int startBranchIndex = tempBranch.GetLastIndex(SqLiteInstance.Connection) + 1;
                    int startCommitIndex = tempCommit.GetLastIndex(SqLiteInstance.Connection) + 1;
                    int startChangeIndex = ChangesTable.GetLastIndex(SqLiteInstance.Connection) + 1;

                    foreach (BranchTable branch in branches)
                    {
                        transactions.Add(BranchTable.InsertSqliteQuery(branch));
                        transactions.Add(
                            BranchForRepoTable.InsertQuery(new BranchForRepoTable(startRepoIndex, startBranchIndex)));

                        List<CommitTable> commits = GetAllCommits(repository.Branches[branch.Name]);
                        foreach (CommitTable commit in commits)
                        {
                            transactions.Add(CommitTable.InsertSqliteQuery(commit));
                            transactions.Add(
                                CommitForBranchTable.InsertQuery(new CommitForBranchTable(startBranchIndex,
                                    startCommitIndex)));

                            List<ChangesTable> changes = GetAllChanges(repository.Lookup<Commit>(commit.Sha));
                            foreach (ChangesTable change in changes)
                            {
                                changesTransactions.Add(ChangesTable.InsertSqliteQuery(change));
                                changesTransactions.Add(
                                    ChangesForCommitTable.InsertQuery(new ChangesForCommitTable(startCommitIndex,
                                        startChangeIndex)));
                                startChangeIndex++;
                            }
                            startCommitIndex++;
                        }
                        startBranchIndex++;
                    }
                    SqLiteInstance.ExecuteTransaction(transactions);
                    SqLiteInstance.ExecuteTransaction(changesTransactions);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SqLiteInstance.CloseConnection();
            }   
        }

        public List<CommitTable> GetDataFromBase()
        {
            List<CommitTable> tempList = new List<CommitTable>();
            int idzz = 1;
            string query = "SELECT * FROM Commits " +
                           "INNER JOIN CommitForBranch on Commits.ID=CommitForBranch.NR_Commit " +
                           "INNER JOIN BRANCH on CommitForBranch.NR_Branch=Branch.ID " +
                           "WHERE Branch.Name='master' OR Branch.Name='trunk'";
            SQLiteCommand command = new SQLiteCommand(query, SqLiteInstance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = idzz;
                idzz++;
                string message = Convert.ToString(reader["Message"]);
                string author = Convert.ToString(reader["Author"]);
                string date=String.Empty;
                if (reader["Date"] != null)
                date = Convert.ToString(reader["Date"]);
                date = SqLiteService.getDateTimeFormat(date);
                // date = date.Remove(19);
                string email = Convert.ToString(reader["Email"]);
                CommitTable tempInstance = new CommitTable(id, message, author, date, email);
                tempList.Add(tempInstance);
            }
            return tempList;
        }
        #endregion

    }
}
