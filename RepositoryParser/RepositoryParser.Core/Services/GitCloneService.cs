using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LibGit2Sharp;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Core.Services
{
    public class GitCloneService
    {
        private List<GitCloneBranch> _branches;
        private List<string> _remotes; 
        public string UrlAdress { get; set; }




        public string getRepositoryNameFromUrl(string url)
        {
            string repositoryName = url;
            //TODO regex that will gain name from url. .*/

            return repositoryName;
        }

        public void FillBranches()
        {
            _branches=new List<GitCloneBranch>();
            try
            {   
                Repository repository = new Repository(this.UrlAdress);
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
                    string tempName = branch.Name;

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
                MessageBox.Show(ex.Message);
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

            Repository repository = new Repository(this.UrlAdress);
            foreach (GitCloneBranch branch in _branches)
            {
                Branch remoteBranch = repository.Branches[branch.OriginName];
                Branch newLoaclBranch = repository.CreateBranch(branch.BranchName, remoteBranch.Tip);

                repository.Branches.Update(newLoaclBranch, b => b.TrackedBranch = remoteBranch.CanonicalName);

            }

        }

        public void CloneRepository()
        {
            string repositoryPath = "./" + getRepositoryNameFromUrl(UrlAdress);
            Repository.Clone(UrlAdress, repositoryPath);
        }

        private bool IsAddressCorrect()
        {
            //TODO VALIDATION
            return false;
        }
    }
}
