using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LibGit2Sharp;
using RepositoryParser.Core.Interfaces;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Core.Services
{
    public class GitService : IGitService
    {
        public string RepositoryPath { get; set; }

        public GitService()
        {
            this.RepositoryPath = String.Empty;
        }
        public GitService(string path)
        {
            this.RepositoryPath = path;
        }


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
                        branchList.Add(new BranchTable(0,branch.FriendlyName));
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
                commitList.Add(new CommitTable(0,commit.MessageShort,commit.Author.Name,date,commit.Author.Email));
            }
            return commitList;
        }

        public List<ChangesTable> GetAllChanges(Commit commit)
        {
            List<ChangesTable> changesList = new List<ChangesTable>();
            var firstOrDefault = commit.Parents.FirstOrDefault();
            if (firstOrDefault == null)
                return changesList;

            using (Repository repository = new Repository(this.RepositoryPath))
            {
                Tree rootCommitTree = repository.Lookup<Commit>(commit.Id.ToString()).Tree;
                Tree commitTreeWithUpdatedFile = repository.Lookup<Commit>(commit.Parents.FirstOrDefault().Id.ToString()).Tree;

                var changes = repository.Diff.Compare<Patch>(rootCommitTree, commitTreeWithUpdatedFile);
                var changes2 = repository.Diff.Compare<Patch>(commitTreeWithUpdatedFile, rootCommitTree);
                List<string> pathes = changes.Select(change => change.Path).ToList();

                if (changes.Count() == changes2.Count())
                {
                    foreach (string p in pathes)
                    {
                        ChangesTable temp = new ChangesTable();
                        temp.Path = changes[p].Path;
                        if (changes[p].Status == ChangeKind.Deleted)
                            temp.Type = "Deleted";
                        else if (changes[p].Status == ChangeKind.Added)
                            temp.Type = "Added";
                        else if (changes[p].Status == ChangeKind.Modified)
                            temp.Type = "Modified";
                        temp.TextA = changes[p].Patch;
                        temp.TextB = changes2[p].Patch;
                        changesList.Add(temp);
                    }
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
    }
}
