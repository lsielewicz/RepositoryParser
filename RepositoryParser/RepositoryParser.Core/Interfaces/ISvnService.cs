using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryParser.Core.Models;
using SharpSvn;

namespace RepositoryParser.Core.Interfaces
{
    public interface ISvnService
    {
        string Path { get; set; }
        SvnClient Client { get; set; }
        SvnInfoEventArgs Info { get; set; }
        SqLiteService SqLiteInstance { get; set; }

        List<BranchTable> GetAllBranches();
        RepositoryTable GetRepository();
        List<CommitTable> GetCommits();
        List<ChangesTable> GetChanges();
         

    }
}
