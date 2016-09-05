using FluentNHibernate.Mapping;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.DataBaseManagementCore.Mappings
{
    class ChangesMap : ClassMap<Changes>
    {
        public ChangesMap()
        { 
            Id(x => x.Id);
            Map(x => x.Type);
            Map(x => x.Path);
            Map(x => x.ChangeContent);

            References<Commit>(x => x.Commit);
        }
    }
}
