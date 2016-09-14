using System.Collections.Generic;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.Core.Interfaces
{
    public interface ISvnFilePersister : IVersionControlFilePersister
    {
        List<Branch> GetBranches(string path);
        List<Commit> GetCommits(string path, List<Commit> alreadyDeclaredCommits = null);
        List<Changes> GetChanges(long revision, string path);
    }
}