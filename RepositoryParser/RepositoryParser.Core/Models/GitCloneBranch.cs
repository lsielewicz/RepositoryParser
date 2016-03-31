using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Models
{
    public class GitCloneBranch
    {
        public string BranchName { get; set; }
        public string OriginName { get; set; }
        public string RemoteName { get; set; }
        public bool IsAlreadyCloned { get; set; }

        GitCloneBranch(string branchName, string originName, string remoteName, bool isCloned)
        {
            this.BranchName = branchName;
            this.OriginName = originName;
            this.RemoteName = remoteName;
            this.IsAlreadyCloned = isCloned;
        }
    }
}
