using NHibernate;

namespace RepositoryParser.DataBaseManagementCore.Interfaces
{
    public interface IDbService
    {
        void CreateDataBase();
        ISessionFactory SessionFactory { get; }
    }
}