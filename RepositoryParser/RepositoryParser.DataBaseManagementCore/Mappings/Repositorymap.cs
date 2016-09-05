using FluentNHibernate.Mapping;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.DataBaseManagementCore.Mappings
{
    class Repositorymap : ClassMap<Repository>
    {
        public Repositorymap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Type);
            Map(x => x.Url);

            HasMany<Branch>(x => x.Branches).Inverse().Cascade.AllDeleteOrphan();
        }
    }
}
