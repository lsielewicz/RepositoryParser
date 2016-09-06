using System.Collections.Generic;
using NHibernate;
using RepositoryParser.DataBaseManagementCore.Entities;
using GitRepository = LibGit2Sharp.Repository;
using GitBranch = LibGit2Sharp.Branch;
using GitCommit = LibGit2Sharp.Commit;

namespace RepositoryParser.Core.Interfaces
{
    public interface IVersionControlFilePersister
    {
        string DirectoryPath { get; set; }
        string UrlPath { get; set; }
        bool IsCloned { get; set; }
        Repository GetRepository(string repositoryPath);
        List<Branch> GetBranches(string repositoryPath);
        List<Commit> GetCommits(GitBranch branch, string repositoryPath);
        List<Changes> GetChanges(GitCommit commit, string repositoryPath);
        string GetNameFromPath(string path);
        void FillDataBase(ISessionFactory sessionFactory, EntityBase enity);
    }
}