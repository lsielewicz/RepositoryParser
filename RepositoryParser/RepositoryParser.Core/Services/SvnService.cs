using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using RepositoryParser.Core.Interfaces;
using RepositoryParser.Core.Models;
using SharpSvn;

namespace RepositoryParser.Core.Services
{
    public class SvnService : ISvnService
    {
        #region Fields
        public string Path { get; set; }
        public SvnClient Client { get; set; }
        public SvnInfoEventArgs Info { get; set; }
        public SqLiteService SqLiteInstance { get; set; }
        #endregion

        #region Constructors
        public SvnService()
        {
            Path = String.Empty;
            Client = null;
            Info = null;
            SqLiteInstance = null;
        }

        public SvnService(string path)
        {
            path = checkPath(path);
            this.Path = path;
            InitializeConnection();
        }
        #endregion

        #region Methods
        private string checkPath(string path)
        {
            using (SvnClient client = new SvnClient())
            {
                var checker = client.GetUriFromWorkingCopy(path);
                if (checker != null)
                {
                    return checker.ToString();
                }
            }
            return path;

        }


        public void InitializeConnection()
        {
            try
            {
                if (!String.IsNullOrEmpty(Path) && !Path.Contains("file:///"))
                    Path = "file:///" + Path;
                else if (String.IsNullOrEmpty(Path))
                {
                    return;
                }

                this.Client = new SvnClient();
                SvnInfoEventArgs svnInfoEventArgs;
                Client.GetInfo(new Uri(Path), out svnInfoEventArgs);
                Info = svnInfoEventArgs;

                string repoPath = "./DataBases/CommonRepositoryDataBase.sqlite";
                if (!File.Exists(repoPath))
                    ConnectRepositoryToDataBase(true);
                else
                    ConnectRepositoryToDataBase();
            }
            catch (SvnRepositoryIOException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public List<BranchTable> GetAllBranches()
        {
            string branchesPath;
            if (Path.EndsWith("/"))
                branchesPath = Path+"branches";
            else
                branchesPath = Path + "/branches";

            List<string> files = new List<string>();
            using (SvnClient svnClient = new SvnClient())
            {
                Collection<SvnListEventArgs> contents;
                if (svnClient.GetList(new Uri(branchesPath), out contents))
                {
                    files.AddRange(contents.Select(item => item.Path));
                }
            }
            List<BranchTable> branches = new List<BranchTable>();
            files.ForEach(file =>
            {
                branches.Add(new BranchTable(file, branchesPath+'/'+file));
            });
            return branches;
        }

        public RepositoryTable GetRepository()
        {
            RepositoryTable repoTable=new RepositoryTable();
            if (!String.IsNullOrEmpty(Path))
            {
                repoTable.Name = FixName(this.Path);
                repoTable.Type = "SVN";
            }
            return repoTable;    
        }

        public List<CommitTable> GetCommits(string path)
        {
            List<CommitTable> commitsList = new List<CommitTable>();
            using (SvnClient svnClient = new SvnClient())
            {
                System.Collections.ObjectModel.Collection<SvnLogEventArgs> logEventArgs;
                svnClient.GetLog(new Uri(path), out logEventArgs);
                foreach (var arg in logEventArgs)
                {
                    CommitTable tempTable= new CommitTable();
                    tempTable.Author = arg.Author;
                    tempTable.Date = Convert.ToString(arg.Time);
                   // tempTable.Date = tempTable.Date.Remove(19);
                    tempTable.Date = SqLiteService.getDateTimeFormat(tempTable.Date);
                    tempTable.Message = arg.LogMessage;
                    tempTable.Email = "-";
                    tempTable.Revision = arg.Revision;
                    commitsList.Add(tempTable);
                }
            }
            return commitsList;
        }


        public List<ChangesTable> GetChanges(long revision, string path)
        {
            List<ChangesTable> changesList = new List<ChangesTable>();

            using (SvnClient svnClient = new SvnClient())
                svnClient.Log(
                    new Uri(path),
                    new SvnLogArgs
                    {
                        Range = new SvnRevisionRange(revision, revision-1)
                    },
                    (o, e) =>
                    {
                        foreach (var changeItem in e.ChangedPaths)
                        {
                            string action = String.Empty;
                            if(changeItem.Action == SvnChangeAction.None)
                                continue;
                            else if (changeItem.Action == SvnChangeAction.Modify)
                                action = "Modified";
                            else if (changeItem.Action == SvnChangeAction.Add)
                                action = "Deleted";
                            else if (changeItem.Action == SvnChangeAction.Delete)
                                action = "Added";
                            else
                                action = Convert.ToString(changeItem.Action);
                                                  
                            changesList.Add(new ChangesTable(action,
                                                            changeItem.Path,
                                                            GetDifferences(revision,changeItem.Path,false), 
                                                            GetDifferences(revision, changeItem.Path, true)));
                        }
                    });

        return changesList;
        }

        private string GetDifferences(long revision, string path, bool isParent=false)
        {
            if (revision == 1)
                return "";
            path = this.Path + path;
            SvnRevisionRange range;
            if (!isParent)
                range = new SvnRevisionRange(revision-1, revision);
            else
                range = new SvnRevisionRange(revision, revision- 1);


            MemoryStream diffResult = new MemoryStream();
            string theFile = String.Empty;
            int counter = 0;
            bool descFlag = false;
            using (SvnClient client = new SvnClient())
            {
                if (client.Diff(new SvnUriTarget(path), range, diffResult))
                {
                    diffResult.Position = 0;
                    StreamReader strReader = new StreamReader(diffResult);
                    string diff = strReader.ReadToEnd();
                    diff = diff.Insert(diff.Length, "\0");
                    if (diff.Length >= 50000)
                    {
                        double size = diff.Length/500;
                        return String.Format("File content: {0} kb", size);
                    }
                    foreach (char c in diff)
                    {
                        counter++;
                        if (c != '@' && descFlag == false) //getting past the first description part
                            continue;
                        else if (c == '@' && descFlag == false) //we know we are towards the end of it
                        {
                            if (diff.Substring(counter, 1) == "\n" || diff.Substring(counter, 1) == "\r") //at the end of the line with the '@' symbols
                            {
                                descFlag = true;
                            }
                            else
                                continue;
                        }
                        else if (descFlag == true) //now reading the actual file
                        {
                            theFile += diff.Substring(counter);
                            break;
                        }
                    }
                }
            }
            return theFile;
        }


        public void ConnectRepositoryToDataBase(bool isNewFile=false)
        {
            if (isNewFile == true)
            {
                string RepositoryName = "CommonRepositoryDataBase";
                int repoNumber = 0;
                while (File.Exists("./Databases/" + RepositoryName + ".sqlite"))
                {
                    RepositoryName = "CommonRepositoryDataBase" + Convert.ToString(repoNumber);
                    repoNumber++;
                }

                SqLiteInstance = SqLiteService.GetInstance();
                SqLiteInstance.DBName = RepositoryName;
                List<string> CreateTableQuerys = new List<string>
                {
                    {RepositoryTable.createTable},
                    {BranchForRepoTable.CreateTable},
                    {BranchTable.CreateTable},
                    {CommitForBranchTable.CreateTable},
                    {CommitTable.SqliteQuery},
                    {ChangesForCommitTable.CreateTable},
                    {ChangesTable.CreateTableQuery}
                };

                SqLiteInstance.OpenConnection(CreateTableQuerys);
            }
            else
            {
                string RepositoryName = "CommonRepositoryDataBase";
                SqLiteInstance = SqLiteService.GetInstance();
                SqLiteInstance.DBName = RepositoryName;
                SqLiteInstance.OpenConnection();
            }
        }


        private string FixName(string Repository)
        {
            string output = Repository;
            string pattern = @"((.*)\\(.*)\\)";
            Regex r = new Regex(pattern);
            Match m = r.Match(Repository);
            if (m.Success)
            {
                if (m.Groups.Count >= 3)
                {
                    output = m.Groups[3].Value;
                }
            }
            return output;
        }


        public void FillDataBase()
        {
            List<string> transactions=new List<string>();
            RepositoryTable repository = GetRepository();
            List<BranchTable> branches = GetAllBranches();
            BranchTable tempBranch = new BranchTable();
            CommitTable tempCommit = new CommitTable();

            transactions.Add(RepositoryTable.InsertQuery(repository));

            int startRepoIndex = repository.GetLastIndex(SqLiteInstance.Connection) + 1;
            int startBranchIndex = tempBranch.GetLastIndex(SqLiteInstance.Connection) + 1;
            int startCommitIndex = tempCommit.GetLastIndex(SqLiteInstance.Connection) + 1;
            int startChangeIndex = ChangesTable.GetLastIndex(SqLiteInstance.Connection) + 1;

            foreach (BranchTable branch in branches)
            {
                if (String.IsNullOrWhiteSpace(branch.Name))
                {
                    branch.Name = "trunk";
                    if (branch.Path.EndsWith("/") && Path.EndsWith("/"))
                            branch.Path = Path + "trunk/";     
                    else
                        branch.Path = Path + "/trunk/";

                }

                transactions.Add(BranchTable.InsertSqliteQuery(branch));
                transactions.Add(BranchForRepoTable.InsertQuery(new BranchForRepoTable(startRepoIndex, startBranchIndex)));

                List<CommitTable> commits = GetCommits(branch.Path);
                foreach (CommitTable commit in commits)
                {
                    transactions.Add(CommitTable.InsertSqliteQuery(commit));
                    transactions.Add(
                        CommitForBranchTable.InsertQuery(new CommitForBranchTable(startBranchIndex, startCommitIndex)));
                        List<ChangesTable> changes = GetChanges(commit.Revision, branch.Path);
                        foreach (ChangesTable change in changes)
                        {
                            transactions.Add(ChangesTable.InsertSqliteQuery(change));
                            transactions.Add(
                                ChangesForCommitTable.InsertQuery(new ChangesForCommitTable(startCommitIndex,
                                    startChangeIndex)));

                            startChangeIndex++;
                        }
                    startCommitIndex++;
                }
                startBranchIndex++;
            }
            SqLiteInstance.ExecuteTransaction(transactions);
        }
        #endregion
    }
}
