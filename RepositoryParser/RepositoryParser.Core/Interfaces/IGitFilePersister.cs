using System.Collections.Generic;
using RepositoryParser.DataBaseManagementCore.Entities;
using GitRepository = LibGit2Sharp.Repository;
using GitBranch = LibGit2Sharp.Branch;
using GitCommit = LibGit2Sharp.Commit;

namespace RepositoryParser.Core.Interfaces
{
    public interface IGitFilePersister : IVersionControlFilePersister
    {
        string UrlPath { get; }
        bool IsCloned { get; }
        List<Branch> GetBranches(string repositoryPath);
        List<Commit> GetCommits(GitBranch branch, string repositoryPath);
        List<Changes> GetChanges(GitCommit commit, string repositoryPath);
    }
}