using System;
using NHibernate;
using NHibernate.Dialect.Function;

namespace RepositoryParser.DataBaseManagementCore.Interfaces
{
    public interface IDbService
    {
        void CreateDataBase();
        ISessionFactory SessionFactory { get; }
    }
}