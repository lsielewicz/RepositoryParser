using System.IO;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.DataBaseManagementCore.Configuration
{
    public class HibernateConfigurationHelper
    {
        private ISessionFactory _sessionFactory;
        private readonly string _dbPath;

        public HibernateConfigurationHelper(string dbPath, bool createSchema = false)
        {
            _dbPath = dbPath;
            _sessionFactory = CreateSessionFactory(dbPath, createSchema);         
        }

        public ISessionFactory SessionFactory
        {
            get { return _sessionFactory ?? (_sessionFactory = CreateSessionFactory(_dbPath)); }
        }

        private ISessionFactory CreateSessionFactory(string dbPath, bool createSchema=false)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            if (createSchema)
            {
                return Fluently.Configure().Database(SQLiteConfiguration.Standard.UsingFile(dbPath)).Mappings(m=>m.FluentMappings.AddFromAssemblyOf<EntityBase>())
                .ExposeConfiguration(BuildSchema).BuildSessionFactory();
            }

            return Fluently.Configure().Database(SQLiteConfiguration.Standard.UsingFile(dbPath))
                .Mappings(m => m.FluentMappings.AddFromAssembly(assembly)).BuildSessionFactory();
        }

        private void BuildSchema(NHibernate.Cfg.Configuration config)
        {
            if (File.Exists(_dbPath))
                File.Delete(_dbPath);

            new SchemaExport(config)
              .Create(false, true);
        }

    }
}
