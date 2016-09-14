using System;
using System.Collections.Generic;

namespace RepositoryParser.DataBaseManagementCore.Entities
{
    public class Commit : EntityBase
    {
        public virtual string Revision { get; set; }
        public virtual string Author { get; set; }
        public virtual string Message { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Email { get; set; }

        public virtual IList<Branch> Branches { get; set; }
        public virtual IList<Changes> Changes { get; set; }

        public Commit()
        {
            Branches = new List<Branch>();
            Changes = new List<Changes>();
        }

        public virtual void AddBranch(Branch branch)
        {
            branch.Commits.Add(this);
            Branches.Add(branch);
        }

        public virtual void AddChanges(Changes changes)
        {
            changes.Commit = this;
            Changes.Add(changes);
        }

    }
}
