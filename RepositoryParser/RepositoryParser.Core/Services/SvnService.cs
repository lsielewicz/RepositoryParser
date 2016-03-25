using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryParser.Core.Interfaces;
using RepositoryParser.Core.Models;
using SharpSvn;

namespace RepositoryParser.Core.Services
{
    public class SvnService : ISvnService
    {
        public string Path { get; set; }
        public SvnClient Client { get; set; }
        public SvnInfoEventArgs Info { get; set; }
        public SqLiteService SqLiteInstance { get; set; }
        public List<BranchTable> GetAllBranches()
        {
            throw new NotImplementedException();
        }

        public RepositoryTable GetRepository()
        {
            throw new NotImplementedException();
        }

        public List<CommitTable> GetCommits()
        {
            throw new NotImplementedException();
        }

        public List<ChangesTable> GetChanges()
        {
            throw new NotImplementedException();
        }
    }
}
