using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryParser.Helpers;

namespace RepositoryParser.Configuration
{
    public class ConfigurationService
    {
        #region Singleton
        private static ConfigurationService _instance;

        public static ConfigurationService Instance
        {
            get { return _instance ?? (_instance = new ConfigurationService()); }
        }

        private ConfigurationService()
        {
            
        }
        #endregion

        private CultureInfo _cultureInfo;

        public CultureInfo CultureInfo
        {
            get { return _cultureInfo ?? (_cultureInfo = new CultureInfo(this.Configuration.CurrentLanguage)); }
            set { _cultureInfo = value; }
        }
            
        private RepositoryAnalyserConfiguration _configuration;

        public RepositoryAnalyserConfiguration Configuration
        {
            get { return _configuration ?? (_configuration = LoadConfigurationFromFile()); }
        }

        public void SaveChanges()
        {
            XmlSerializeHelper<RepositoryAnalyserConfiguration>.Serialize(Configuration, LocalizationConstants.ConfigFilePath);
        }

        private RepositoryAnalyserConfiguration LoadConfigurationFromFile()
        {
            try
            {
                RepositoryAnalyserConfiguration configuration;
                if (ZetaLongPaths.ZlpIOHelper.FileExists(LocalizationConstants.ConfigFilePath))
                {
                    configuration = XmlSerializeHelper<RepositoryAnalyserConfiguration>.Deserialize(
                        LocalizationConstants.ConfigFilePath);
                }
                else
                {
                    configuration = CreateEmptyConfiguration();
                }

                return configuration;
            }
            catch (Exception)
            {
                //todo loging
                return this.CreateEmptyConfiguration();
            }
        }

        private RepositoryAnalyserConfiguration CreateEmptyConfiguration()
        {
            RepositoryAnalyserConfiguration configuration = new RepositoryAnalyserConfiguration();
            XmlSerializeHelper<RepositoryAnalyserConfiguration>.InitEmptyProperties(configuration);

            configuration.CurrentLanguage = "en-EN";
            configuration.CloneAllBranches = true;
            configuration.DynamicFiltering = true;
            configuration.SavingRepositoryPath = LocalizationConstants.DefaultRepositorySavingPath;

            if (!ZetaLongPaths.ZlpIOHelper.DirectoryExists(LocalizationConstants.ProgramDataApplicationDirectory))
            {
                ZetaLongPaths.ZlpIOHelper.CreateDirectory(LocalizationConstants.ProgramDataApplicationDirectory);
            }

            XmlSerializeHelper<RepositoryAnalyserConfiguration>.Serialize(configuration, LocalizationConstants.ConfigFilePath);

            return configuration;
        }

    }
}
