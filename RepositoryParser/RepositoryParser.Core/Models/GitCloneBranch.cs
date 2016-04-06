using System;

namespace RepositoryParser.Core.Models
{
    public class GitCloneBranch
    {
        public string BranchName { get; set; }
        public string OriginName { get; set; }

        public GitCloneBranch()
        {
            this.BranchName = String.Empty;
            this.OriginName = String.Empty;

        }

       public GitCloneBranch(string branchName, string originName)
        {
            this.BranchName = branchName;
            this.OriginName = originName;
        }
    }
}
