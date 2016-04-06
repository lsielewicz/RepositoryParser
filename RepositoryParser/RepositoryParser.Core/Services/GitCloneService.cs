using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Core.Services
{
    public class GitCloneService
    {
        private List<GitCloneBranch> _branches;
        private List<string> _remotes;
        public string UrlAdress { get; set; }
        public string DirectoryPath { get; set; }

        public GitCloneService()
        {
            UrlAdress = String.Empty;
        }

        public GitCloneService(string url)
        {
            UrlAdress = url;
        }

        public string GetRepositoryNameFromUrl(string url)
        {
            string repositoryName = url;
            string rg = @"(.*\/)(.*)\.git";
            Regex regex = new Regex(rg);
            Match match = regex.Match(url);
            if (match.Success)
            {
                if (match.Groups.Count >= 3)
                {
                    repositoryName = match.Groups[2].Value;
                }
            }

            return repositoryName;
        }

        public void FillBranches()
        {
            _branches = new List<GitCloneBranch>();
            _remotes = new List<string>();
            try
            {
                Repository repository = new Repository("./" + GetRepositoryNameFromUrl(UrlAdress));
                List<GitCloneBranch> tempBranches = new List<GitCloneBranch>();
                foreach (Branch branch in repository.Branches)
                {
                    GitCloneBranch temp = new GitCloneBranch();
                    if (!branch.IsRemote || branch.IsCurrentRepositoryHead || branch.FriendlyName == "origin/master" ||
                    branch.FriendlyName == "origin/HEAD")
                    {
                        if (!branch.IsRemote)
                            _remotes.Add(branch.FriendlyName);
                        continue;
                    }
                    // Display the branch name
                    string tempName = branch.FriendlyName;

                    if (tempName.Contains("origin"))
                    {
                        tempName = tempName.Remove(0, 7);
                    }
                    temp.BranchName = tempName;
                    temp.OriginName = branch.FriendlyName;
                    tempBranches.Add(temp);
                }

                tempBranches.ForEach(branch =>
                {
                    if (!isBranchCloned(_remotes, branch.BranchName))
                    {
                        _branches.Add(branch);
                    }
                });

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }


        private bool isBranchCloned(List<string> remotes, string branch)
        {
            int index = remotes.FindIndex(x => x == branch);
            if (index >= 0)
                return true;
            else
                return false;
        }

        public void CloneAllBranches()
        {
            if (_branches == null || _branches.Count == 0)
                return;


            Repository repository = new Repository("./" + GetRepositoryNameFromUrl(UrlAdress));
            foreach (GitCloneBranch branch in _branches)
            {
                Branch remoteBranch = repository.Branches[branch.OriginName];
                Branch newLoaclBranch = repository.CreateBranch(branch.BranchName, remoteBranch.Tip);

                repository.Branches.Update(newLoaclBranch, b => b.TrackedBranch = remoteBranch.CanonicalName);

            }

        }
        private void DeleteDirectory(string path, bool recursive)
        {
            if (recursive)
            {
                var subfolders = Directory.GetDirectories(path);
                foreach (var s in subfolders)
                {
                    DeleteDirectory(s, true);
                }
            }
            var files = Directory.GetFiles(path);
            foreach (var f in files)
            {
                var attr = File.GetAttributes(f);
                if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)      
                    File.SetAttributes(f, attr ^ FileAttributes.ReadOnly);
          
                File.Delete(f);
            }
            Directory.Delete(path);
        }


        public void CloneRepository(bool cloneWithAllBranches = false)
        {
            string repositoryPath = "./" + GetRepositoryNameFromUrl(UrlAdress);
            if (!Directory.Exists(repositoryPath))
            {
                Directory.CreateDirectory(repositoryPath);
            }
            else
            {
                DeleteDirectory(repositoryPath,true);
            }
            Repository.Clone(UrlAdress, repositoryPath);
            DirectoryPath = repositoryPath;
            if (cloneWithAllBranches)
            {
                FillBranches();
                CloneAllBranches();
            }
        }

        private bool IsAddressCorrect()
        {
            //TODO VALIDATION
            return false;
        }
    }
}
