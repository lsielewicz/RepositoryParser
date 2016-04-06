using System.Collections.Generic;
using LibGit2Sharp;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Core.Interfaces
{
    public interface IGitService
    {
        string DirectoryPath { get; set; }
        string UrlPath { get; set; }
        bool IsCloned { get; set; }
        RepositoryTable GetRepository(string path);
        List<BranchTable> GetAllBranches(string path);
        List<CommitTable> GetAllCommits(Branch branch);
        List<ChangesTable> GetAllChanges(Commit commit);
        string GetNameFromPath(string path);
        void InitializeConnection();
        void ConnectRepositoryToDataBase(bool isNewFile = false);
        void FillDataBase();
    }
}
