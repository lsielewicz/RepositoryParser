using System.Collections.Generic;
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

        void InitializeConnection();
        List<BranchTable> GetAllBranches();
        RepositoryTable GetRepository();
        List<CommitTable> GetCommits(string path);
        List<ChangesTable> GetChanges(long revision, string path);
         

    }
}
