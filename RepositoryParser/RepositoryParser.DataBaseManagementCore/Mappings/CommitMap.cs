using FluentNHibernate.Mapping;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.DataBaseManagementCore.Mappings
{
    class CommitMap : ClassMap<Commit>
    {
        public CommitMap()
        {
            Id(x => x.Id);
            Map(x => x.Author);
            Map(x => x.Date);
            Map(x => x.Email);
            Map(x => x.Message);
            Map(x => x.Revision);


            HasMany<Changes>(x => x.Changes).Cascade.All().Inverse();
            HasManyToMany<Branch>(x => x.Branches).Cascade.All().Inverse().Table("CommitsBranches");
        }
    }
}
