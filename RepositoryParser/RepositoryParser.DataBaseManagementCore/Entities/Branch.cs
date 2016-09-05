using System.Collections.Generic;

namespace RepositoryParser.DataBaseManagementCore.Entities
{
    public class Branch : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual IList<Commit> Commits { get; set; }
        public virtual Repository Repository { get; set; }

        public Branch()
        {
            Commits = new List<Commit>();
        }

        public virtual void AddCommit(Commit commit)
        {
            commit.Branches.Add(this);
            Commits.Add(commit);
        }
    }
}
