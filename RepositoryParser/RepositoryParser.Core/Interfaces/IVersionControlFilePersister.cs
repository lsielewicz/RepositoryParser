using NHibernate;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.Core.Interfaces
{
    public interface IVersionControlFilePersister
    {
        string DirectoryPath { get; set; }
        Repository GetRepository();
        string GetNameFromPath(string path);
        void FillDataBase(ISessionFactory sessionFactory, EntityBase enity);
        void AddRepositoryToDataBase(ISessionFactory sessionFactory);
    }
}