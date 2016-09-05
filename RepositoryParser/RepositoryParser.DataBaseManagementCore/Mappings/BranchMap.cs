using FluentNHibernate.Mapping;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.DataBaseManagementCore.Mappings
{
    class BranchMap : ClassMap<Branch>
    {
        public BranchMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);

            References<Repository>(x => x.Repository);
            HasManyToMany<Commit>(x => x.Commits).Cascade.All().Table("CommitsBranches");
        }
    }
}
