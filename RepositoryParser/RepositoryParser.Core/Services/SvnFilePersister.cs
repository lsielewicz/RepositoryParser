using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate;
using NHibernate.Util;
using RepositoryParser.Core.Interfaces;
using RepositoryParser.DataBaseManagementCore.Configuration;
using RepositoryParser.DataBaseManagementCore.Entities;
using SharpSvn;
using Branch = RepositoryParser.DataBaseManagementCore.Entities.Branch;
using Commit = RepositoryParser.DataBaseManagementCore.Entities.Commit;
using Repository = RepositoryParser.DataBaseManagementCore.Entities.Repository;

namespace RepositoryParser.Core.Services
{
    public class SvnFilePersister : ISvnFilePersister
    {
        public string DirectoryPath { get; set; }
        public Repository GetRepository()
        {
            Repository repository = new Repository()
            {
                Name = GetNameFromPath(DirectoryPath),
                Type = RepositoryType.Svn
            };
            List<Branch> branches = GetBranches(DirectoryPath);
            branches.ForEach(branch=> repository.AddBranch(branch));

            return repository;
        }

        public SvnFilePersister(string path)
        {
            DirectoryPath = CheckPath(path);
        }

        public void AddRepositoryToDataBase(ISessionFactory sessionFactory)
        {
            Repository repository = GetRepository();
            FillDataBase(sessionFactory, repository);
        }


        public List<Branch> GetBranches(string path)
        {
            List<Branch> branches = new List<Branch>();
            string branchesPath;
            if (path.EndsWith("/"))
                branchesPath = path + "branches";
            else
                branchesPath = path + "/branches";
            //key-key : path, key-value : fullPath, value: count of revisions
            Dictionary<KeyValuePair<string,string>, int> branchesPathes = new Dictionary<KeyValuePair<string,string>, int>();
            using (SvnClient svnClient = new SvnClient())
            {
                Collection<SvnListEventArgs> contents;
                if (svnClient.GetList(new Uri(branchesPath), out contents))
                {
                    contents.ForEach(content =>
                    {
                        string name = !string.IsNullOrEmpty(content.Path) ? content.Path : "trunk";
                        string fullPath = ZetaLongPaths.ZlpPathHelper.Combine(branchesPath, content.Path);
                        branchesPathes.Add(new KeyValuePair<string, string>(name, fullPath), GetCountOfCommitsInBranch(fullPath));
                    });
                }

                Dictionary<KeyValuePair<string, string>, int> orderedPathes =
                    branchesPathes.OrderByDescending(p => p.Value)
                        .ToDictionary(p => new KeyValuePair<string, string>(p.Key.Key, p.Key.Value), p => p.Value);
                if (orderedPathes.Any())
                {
                    string headPath = orderedPathes.First().Key.Value;
                    List<Commit> headCommits = GetCommits(headPath);
                    foreach (var branch in branchesPathes)
                    {
                        List<Commit> commitItems = GetCommits(branch.Key.Value, headCommits);
                        Branch branchItem = new Branch()
                        {
                            Name = branch.Key.Key,
                            Path = branch.Key.Value
                        };
                        commitItems.ForEach(commitItem=>branchItem.AddCommit(commitItem));
                        branches.Add(branchItem);
                    }
                }
            }

            return branches;
        }

        private int GetCountOfCommitsInBranch(string path)
        {
            using (SvnClient svnClient = new SvnClient())
            {
                Collection<SvnLogEventArgs> logEventArgs;
                svnClient.GetLog(new Uri(path), out logEventArgs);
                if (logEventArgs != null)
                    return logEventArgs.Count;
                return 0;
            }
        }

        public List<Commit> GetCommits(string path, List<Commit> alreadyDeclaredCommits = null)
        {
            List<Commit> commits = new List<Commit>();
            using (SvnClient svnClient = new SvnClient())
            {
                System.Collections.ObjectModel.Collection<SvnLogEventArgs> logEventArgs;
                svnClient.GetLog(new Uri(path), out logEventArgs);
                logEventArgs.ForEach(commit =>
                {
                    if (alreadyDeclaredCommits != null &&
                        alreadyDeclaredCommits.Any(
                            declaredCommit => declaredCommit.Revision == commit.Revision.ToString()))
                    {
                        commits.Add(alreadyDeclaredCommits.Find(c=>c.Revision == commit.Revision.ToString()));
                        return;
                    }
                    List<Changes> changes = GetChanges(commit.Revision, path);
                    Commit commitItem = new Commit()
                    {
                        Author = commit.Author,
                        Date = commit.Time.Date,
                        Email = " - ",
                        Message = commit.LogMessage,
                        Revision = commit.Revision.ToString()
                    };
                    changes.ForEach(change=>commitItem.AddChanges(change));
                    commits.Add(commitItem);
                });
            }

            return commits;
        }

        public List<Changes> GetChanges(long revision, string path)
        {
            List<Changes> changes = new List<Changes>();
            using (SvnClient svnClient = new SvnClient())
                svnClient.Log(
                    new Uri(path),
                    new SvnLogArgs
                    {
                        Range = new SvnRevisionRange(revision, revision - 1)
                    },
                    (o, e) =>
                    {
                        e.ChangedPaths.ForEach(change =>
                        {
                            Changes item = new Changes()
                            {
                                ChangeContent = GetPatchOfChange(revision,change.Path),
                                Path = change.Path,
                                Type = SvnChangeActionToChangeType(change.Action)
                            };
                        });
                    });
            
            return changes;
        }

        public string GetNameFromPath(string path)
        {
            string output = path;
            if (output.EndsWith(@"/") || output.EndsWith(@"\"))
                output = output.Remove(output.Length - 1, 1);
            string pattern = @".*(\/|\\).*(\/|\\)([a-zA-Z0-9]*)";
            Regex r = new Regex(pattern);
            Match m = r.Match(output);
            if (m.Success)
            {
                if (m.Groups.Count >= 3)
                {
                    output = m.Groups[3].Value;
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

        private string GetPatchOfChange(long revision,string path)
        {
            if (revision == 1)
                return "";
            path = this.DirectoryPath + path;
            SvnRevisionRange range;

            range = new SvnRevisionRange(revision - 1, revision);
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
                        double size = diff.Length / 500;
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

        private string SvnChangeActionToChangeType(SvnChangeAction changeAction)
        {
            if(changeAction == SvnChangeAction.Add)
                return ChangeType.Added;
            if (changeAction == SvnChangeAction.Modify)
                return ChangeType.Modified;
            if (changeAction == SvnChangeAction.Delete)
                return ChangeType.Deleted;
            return ChangeType.Unmodified;
        }

        private string CheckPath(string path)
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
    }
}
