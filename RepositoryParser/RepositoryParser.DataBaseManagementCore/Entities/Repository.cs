using System.Collections.Generic;

namespace RepositoryParser.DataBaseManagementCore.Entities
{
    public class Repository : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual string Type { get; set; }
        public virtual string Url { get; set; }
        public virtual IList<Branch> Branches { get; set; }

        public Repository()
        {
            Branches = new List<Branch>();
        }

        public virtual void AddBranch(Branch branch)
        {
            branch.Repository = this;
            Branches.Add(branch);
        }
    }
}
