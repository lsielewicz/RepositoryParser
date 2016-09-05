using System.IO;
using NHibernate;
using RepositoryParser.DataBaseManagementCore.Configuration;
using RepositoryParser.DataBaseManagementCore.Interfaces;

namespace RepositoryParser.DataBaseManagementCore.Services
{
    public class DbService : IDbService
    {
        #region Singleton
        private static DbService _instance;
        public static DbService Instance
        {
            get { return _instance ?? (_instance = new DbService()); }
        }

        private DbService()
        {
            DataBaseLocalizationPath = LocalizationConstants.ProgramDataDataBaseLocationPath;
            DatabaseDirectoryPath = LocalizationConstants.ProgramDataDataBaseDirectoryPath;

            if (File.Exists(DataBaseLocalizationPath))
                _dbHelper = new HibernateConfigurationHelper(DataBaseLocalizationPath);
            else
                CreateDataBase();

            SessionFactory = _dbHelper.SessionFactory;
        }
        #endregion

        private HibernateConfigurationHelper _dbHelper;

        public string DataBaseLocalizationPath { get; private set; }
        public string DatabaseDirectoryPath { get; private set; }
        public ISessionFactory SessionFactory { get; private set; }

        public void CreateDataBase()
        {
            if (!Directory.Exists(DatabaseDirectoryPath))
            {
                Directory.CreateDirectory(DatabaseDirectoryPath);
            }
            _dbHelper = new HibernateConfigurationHelper(DataBaseLocalizationPath, true);
        }

        public void ChangeDataBaseLocation(string dataBaselocation, string dataBaseDirectory)
        {
            this.DataBaseLocalizationPath = dataBaselocation;
            this.DatabaseDirectoryPath = dataBaseDirectory;

            if (File.Exists(DataBaseLocalizationPath))
                _dbHelper = new HibernateConfigurationHelper(DataBaseLocalizationPath);
            else
                CreateDataBase();

            SessionFactory = _dbHelper.SessionFactory;
        }
    }
}
