using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibGit2Sharp;
using NHibernate;
using NHibernate.Util;
using RepositoryParser.Core.Interfaces;
using RepositoryParser.Core.Models;
using RepositoryParser.DataBaseManagementCore.Configuration;
using RepositoryParser.DataBaseManagementCore.Entities;
using Branch = RepositoryParser.DataBaseManagementCore.Entities.Branch;
using Commit = RepositoryParser.DataBaseManagementCore.Entities.Commit;
using GitRepository = LibGit2Sharp.Repository; 
using GitBranch = LibGit2Sharp.Branch;
using GitCommit = LibGit2Sharp.Commit;
using Repository = RepositoryParser.DataBaseManagementCore.Entities.Repository;

namespace RepositoryParser.Core.Services
{
    public class GitFilePersister : IVersionControlFilePersister
    {
        public string DirectoryPath { get; set; }
        public string UrlPath { get; set; }
        public bool IsCloned { get; set; }

        public GitFilePersister()
        {
            DirectoryPath = string.Empty;
        }

        public GitFilePersister(string path)
        {
            DirectoryPath = path;
        }

        public GitFilePersister(string path, bool clone)
        {
            {
                if (clone)
                    UrlPath = path;
                else
                    DirectoryPath = path;

                IsCloned = !clone;
                InitializeConnection();
            }
        }



        public void AddRepositoryToDataBase(ISessionFactory sessionFactory,string repositoryPath="")
        {
            if (string.IsNullOrEmpty(repositoryPath))
                repositoryPath = DirectoryPath;
            Repository repository = GetRepository(repositoryPath);
            FillDataBase(sessionFactory,repository);
        }

        public List<Commit> GetCommits(GitBranch branch, string repositoryPath)
        {
            List<Commit> commits = new List<Commit>();
            foreach (GitCommit gitCommitInfo in branch.Commits)
            {
                List<Changes> changes = GetChanges(gitCommitInfo, repositoryPath);
                Commit commit = new Commit()
                {
                    Revision = gitCommitInfo.Sha,
                    Author = gitCommitInfo.Author.Name,
                    Date = gitCommitInfo.Author.When.DateTime,
                    Email = gitCommitInfo.Author.Email,
                    Message = gitCommitInfo.MessageShort 
                };
                changes.ForEach(change=> commit.AddChanges(change));
                commits.Add(commit);
            }
            return commits;
        }

        public List<Changes> GetChanges(GitCommit commit, string repositoryPath)
        {
            List<Changes> changes = new List<Changes>();
            using (GitRepository gitRepositoryInfo = new GitRepository(repositoryPath))
            {
                bool isInitial = false;
                var firstCommit = commit.Parents.FirstOrDefault();
                if (firstCommit == null)
                {
                    isInitial = true;
                }
                Tree rootCommitTree = gitRepositoryInfo.Lookup<GitCommit>(commit.Id.ToString()).Tree;
                Patch patch;
                if (!isInitial)
                {
                    Tree commitTreeWithUpdatedFile =
                        gitRepositoryInfo.Lookup<GitCommit>(commit.Parents.FirstOrDefault().Sha).Tree;
                    patch = gitRepositoryInfo.Diff.Compare<Patch>(commitTreeWithUpdatedFile, rootCommitTree);
                }
                else
                {
                    patch = gitRepositoryInfo.Diff.Compare<Patch>(null, rootCommitTree);
                }
                foreach (var change in patch)
                {
                    changes.Add(new Changes()
                    {
                        Type = ConvertChangeKindToChangeType(change.Status),
                        Path = change.Path,
                        ChangeContent = change.Patch
                    });
                }

            }
            return changes;
        }

        public string GetNameFromPath(string path)
        {
            string output = path;
            string pattern = @"(.*)\\(.*)\\.git";
            Regex r = new Regex(pattern);
            Match m = r.Match(path);
            if (m.Success)
            {
                if (m.Groups.Count >= 2)
                {
                    output = m.Groups[2].Value;
                }
            }
            return output;
        }

        public void FillDataBase(ISessionFactory sessionFactory, EntityBase entity)
        {

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    if (entity != null)
                    {
                        session.SaveOrUpdate(entity);
                        transaction.Commit();
                    }
                }
            }
        }

        public Repository GetRepository(string repositoryPath)
        {
            Repository repository = new Repository();
            using (GitRepository gitRepositoryInfo = new GitRepository(repositoryPath))
            {
                List<Branch> branches = GetBranches(repositoryPath);

                repository.Type = RepositoryType.Git;
                repository.Name = GetNameFromPath(gitRepositoryInfo.Info.Path);
                repository.Url = DirectoryPath;

                branches.ForEach(branch => repository.AddBranch(branch));
            }
            return repository;
        }

        public List<Branch> GetBranches(string repositoryPath)
        {
            List<Branch> branches = new List<Branch>();
            using (GitRepository gitRepositoryInfo = new GitRepository(repositoryPath))
            {
                GitBranch headBranch = null;
                gitRepositoryInfo.Branches.ForEach(branch =>
                {
                    if (branch.IsRemote)
                        return;
                    if (headBranch == null || headBranch.Commits.Count() < branch.Commits.Count())
                        headBranch = branch;
                });

                List<Commit> commits = GetCommits(headBranch, repositoryPath);

                gitRepositoryInfo.Branches.ForEach(branch =>
                {
                    if (branch.IsRemote)
                        return;
                    Branch branchInstance = new Branch() { Name = branch.FriendlyName };

                    var uniqueCommits = branch.Commits.Where(c => commits.All(c1 => c1.Revision != c.Sha));
                    var enumerable = uniqueCommits as IList<GitCommit> ?? uniqueCommits.ToList();
                    if (enumerable.Any())
                    {
                        enumerable.ForEach(uCommit =>
                        {
                            var newCommit = GetCommitFromGitCommit(uCommit, repositoryPath);
                            branchInstance.AddCommit(newCommit);
                        });
                        
                    }

                    commits.ForEach(commit =>
                    {
                        if (branch.Commits.Any(c => c.Sha == commit.Revision))
                        {
                            branchInstance.AddCommit(commit);
                        }
                    });
                    branches.Add(branchInstance);
                });
            }
            return branches;
        }

        private Commit GetCommitFromGitCommit(GitCommit gitCommit, string repositoryPath)
        {
            List<Changes> changes = GetChanges(gitCommit, repositoryPath);
            Commit newCommit = new Commit()
            {
                Revision = gitCommit.Sha,
                Author = gitCommit.Author.Name,
                Date = gitCommit.Author.When.DateTime,
                Email = gitCommit.Author.Email,
                Message = gitCommit.MessageShort
            };
            changes.ForEach(change => newCommit.AddChanges(change));
            return newCommit;
        }

        private string ConvertChangeKindToChangeType(ChangeKind changeKind)
        {
            if (changeKind == ChangeKind.Modified)
                return ChangeType.Modified;
            if (changeKind == ChangeKind.Added)
                return ChangeType.Added;
            if (changeKind == ChangeKind.Deleted)
                return ChangeType.Deleted;

            return ChangeType.Unodified;
        }

        private void InitializeConnection()
        {
            if (!IsCloned && !string.IsNullOrEmpty(UrlPath))
            {
                CloneRepository();
            }


        }

        private void CloneRepository()
        {
            string urlPath = Regex.Replace(UrlPath, @"https?", "git");
            UrlPath = urlPath;
            GitCloneService cloneService = new GitCloneService(urlPath);
            cloneService.CloneRepository(true);
            IsCloned = true;
            DirectoryPath = cloneService.DirectoryPath;
        }

    }

    
}
