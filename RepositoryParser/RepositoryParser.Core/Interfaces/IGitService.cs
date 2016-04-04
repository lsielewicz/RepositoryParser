using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Core.Interfaces
{
    public interface IGitService
    {
        string RepositoryPath { get; set; }
        RepositoryTable GetRepository(string path);
        List<BranchTable> GetAllBranches(string path);
        List<CommitTable> GetAllCommits(Branch branch);
        List<ChangesTable> GetAllChanges(Commit commit);
        string GetNameFromPath(string path);
    }
}
