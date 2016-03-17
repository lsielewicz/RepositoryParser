using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using GitSharp;
using RepositoryParser.Core.Interfaces;

namespace RepositoryParser.Core.Models
{
    public class GitRepositoryService : IGitRepositoryService
    {
        #region Variables
        public string RepoPath { get; set; }
        public string UrlRepoPath { get; set; }
        public Repository RepositoryInstance { get; set; }
        public SqLiteService SqLiteInstance { get; set; }
        public bool isCloned { get; set; }
        #endregion
        #region Constructors

        public GitRepositoryService()
        {
            RepoPath = "";
            RepositoryInstance = null;
            isCloned = false;
        }
        public GitRepositoryService(string path)
        {
            RepoPath = path;
            RepositoryInstance = null;
            isCloned = false;
        }
        #endregion

        #region Methods
        public void InitializeConnection()
        {
            try
            {
                if (isCloned)
                {
                    if (RepoPath.Length > 0)
                    {
                        RepositoryInstance = new Repository(RepoPath);
                    }
                }
                else
                {
                    if (UrlRepoPath.Length > 0)
                    {
                        int number = 0;
                        string directoryPath = "./Repository";
                        while (Directory.Exists(directoryPath))
                        {
                            directoryPath = "./Repository" + Convert.ToString(number);
                            number++;
                        }
                        string Result = UrlRepoPath;
                        try
                        {
                            Result = System.Text.RegularExpressions.Regex.Replace(UrlRepoPath, @"https?", "git");
                        }
                        catch (Exception Ex)
                        {
                            MessageBox.Show(Ex.Message);
                        }
                        UrlRepoPath = Result;


                        Git.Clone(UrlRepoPath, directoryPath);
                        isCloned = true;
                        RepositoryInstance = new Repository(directoryPath);
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
            GitCommits gitCommit = new GitCommits();
            BranchTable gitBranch = new BranchTable();
            GitRepositoryTable gitRepoTable = new GitRepositoryTable(RepositoryInstance.Directory);
            List<string> QuerysList = new List<string>();
            List<string> ChangesQuerys = new List<string>();
            List<BranchTable> gitBranchList = new List<BranchTable>();
            List<GitCommits> gitCommitsList = new List<GitCommits>();
            List<string> CommitsQueryLists = new List<string>();
            List<ChangesTable> changesList = new List<ChangesTable>();

            QuerysList.Add(GitRepositoryTable.InsertQuery(gitRepoTable));
            int lastRepoIndex = gitRepoTable.GetLastIndex(SqLiteInstance.Connection);
            foreach (KeyValuePair<string, Branch> branch in RepositoryInstance.Branches)
            {
                BranchTable tempBranch = new BranchTable(branch.Key, branch.Value);
                if (tempBranch.Name.Contains("/"))
                    continue;
                QuerysList.Add(BranchTable.InsertSqliteQuery(tempBranch));
                gitBranchList.Add(tempBranch);
            }
            int lastBranchIndex = gitBranch.GetLastIndex(SqLiteInstance.Connection);
            int StartBranch = lastBranchIndex + 1;

            foreach (BranchTable x in gitBranchList)
            {
                BranchForRepoTable temp = new BranchForRepoTable(lastRepoIndex + 1, StartBranch);
                QuerysList.Add(BranchForRepoTable.InsertQuery(temp));
                gitCommitsList.Clear();

                //add all commits to db
                int lastCommitIndex = gitCommit.GetLastIndex(SqLiteInstance.Connection);
                int startCommitIndex = lastCommitIndex + 1;

                int lastChangeIndex = ChangesTable.GetLastIndex(SqLiteInstance.Connection);
                int startChangeIndex = lastChangeIndex + 1;

                foreach (Commit commitz in RepositoryInstance.Get<Branch>(x.Name).CurrentCommit.Ancestors)
                {
                    gitCommit.Message = commitz.Message;
                    gitCommit.Author = commitz.Author.Name;
                    gitCommit.Date = Convert.ToString(commitz.AuthorDate);
                    gitCommit.Date = gitCommit.Date.Remove(19);
                    gitCommit.Date = SqLiteService.getDateTimeFormat(gitCommit.Date);
                    gitCommit.Email = commitz.Author.EmailAddress;
                    //SQLITE CORNER
                    CommitsQueryLists.Add(GitCommits.InsertSqliteQuery(gitCommit));
                    gitCommitsList.Add(gitCommit);
                    string tempChange = "";
                    // jezeli komit ma rodzica, to porownaj
                    if (commitz.HasParents)
                    {
                        Commit commitz2 = commitz.Parent;
                        CollectionViewSource aList = new CollectionViewSource();
                        foreach (Change change in commitz.CompareAgainst(commitz2))
                        {
                            tempChange += change.ChangeType + ": " + change.Path + "\n";

                            string TextA = "";
                            string TextB = "";
                            var a = (change.ReferenceObject != null
                                ? (change.ReferenceObject as Blob).RawData
                                : new byte[0]);
                            var b = (change.ComparedObject != null
                                ? (change.ComparedObject as Blob).RawData
                                : new byte[0]);
                            a = (Diff.IsBinary(a) == true ? Encoding.ASCII.GetBytes("Binary content\nFile size: " + a.Length) : a);
                            b = (Diff.IsBinary(b) == true ? Encoding.ASCII.GetBytes("Binary content\nFile size: " + b.Length) : b);
                            Diff diff = new Diff(a, b);
                            aList.Source = diff.Sections;
                            foreach (Diff.Section item in aList.View.SourceCollection)
                            {
                                TextA += item.TextA;
                                TextB += item.TextB;
                            }
                            //add changes to database
                            ChangesTable tempchanges = new ChangesTable(Convert.ToString(change.ChangeType), change.Path, TextA, TextB);
                            changesList.Add(tempchanges);
                            ChangesQuerys.Add(ChangesTable.InsertSqliteQuery(tempchanges));
                            ChangesQuerys.Add(ChangesForCommitTable.InsertQuery(new ChangesForCommitTable(startCommitIndex, startChangeIndex)));
                            startChangeIndex++;
                        }
                    }
                    startCommitIndex++;
                }

                lastCommitIndex = gitCommit.GetLastIndex(SqLiteInstance.Connection);
                startCommitIndex = lastCommitIndex + 1;

                gitCommitsList.ForEach(y =>
                {
                    CommitForBranchTable temp1 = new CommitForBranchTable(StartBranch, startCommitIndex);
                    CommitsQueryLists.Add(CommitForBranchTable.InsertQuery(temp1));
                    startCommitIndex++;
                });
                SqLiteInstance.ExecuteTransaction(CommitsQueryLists);
                SqLiteInstance.ExecuteTransaction(ChangesQuerys);

                CommitsQueryLists.Clear();
                ChangesQuerys.Clear();
                StartBranch++;
            }
            SqLiteInstance.ExecuteTransaction(QuerysList);

        }
        #endregion

        #region SQLITE

        public void ConnectRepositoryToDataBase(bool isNewFile)
        {
            string RepositoryName = "CommonRepositoryDataBase";
            int repoNumber = 0;
            while (File.Exists("./Databases/" + RepositoryName + ".sqlite"))
            {
                RepositoryName = "CommonRepositoryDataBase" + Convert.ToString(repoNumber);
                repoNumber++;
            }

            SqLiteInstance = new SqLiteService(RepositoryName);
            List<string> CreateTableQuerys = new List<string>
            {
                {GitRepositoryTable.createTable },
                {BranchForRepoTable.CreateTable},
                {BranchTable.CreateTable },
                {CommitForBranchTable.CreateTable },
                {GitCommits.SqliteQuery },
                {ChangesForCommitTable.CreateTable },
                {ChangesTable.CreateTableQuery }
            };

            SqLiteInstance.OpenConnection(CreateTableQuerys);
        }
        //working on one repository
        public void ConnectRepositoryToDataBase()
        {
            string RepositoryName = "CommonRepositoryDataBase";
            SqLiteInstance = new SqLiteService(RepositoryName);
            SqLiteInstance.OpenConnection();
        }

        public List<GitCommits> GetDataFromBase()
        {
            List<GitCommits> tempList = new List<GitCommits>();
            int idzz = 1;
            string query = "SELECT * FROM GitCommits " +
                           "INNER JOIN CommitForBranch on GitCommits.ID=CommitForBranch.NR_Commit " +
                           "INNER JOIN BRANCH on CommitForBranch.NR_Branch=Branch.ID " +
                           "WHERE Branch.Name='master'";
            SQLiteCommand command = new SQLiteCommand(query, SqLiteInstance.Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = idzz;
                idzz++;
                string message = Convert.ToString(reader["Message"]);
                string author = Convert.ToString(reader["Author"]);
                string date = Convert.ToString(reader["Date"]);
                date = SqLiteService.getDateTimeFormat(date);
                // date = date.Remove(19);
                string email = Convert.ToString(reader["Email"]);
                GitCommits tempInstance = new GitCommits(id, message, author, date, email);
                tempList.Add(tempInstance);
            }
            return tempList;
        }
        #endregion

    }
}
