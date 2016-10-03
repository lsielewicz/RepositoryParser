using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Helpers
{
    public class LocalizationConstants
    {
        public static string ProductName
        {
            get { return "RepositoryAnalyser"; }
        }
        public static string AppDataPath
        {
            get
            {
                return Environment.ExpandEnvironmentVariables("%AppData%");
            }
        }

        public static string ProgramDataApplicationDirectory
        {
            get { return ZetaLongPaths.ZlpPathHelper.Combine(AppDataPath, ProductName); }
        }

        public static string ConfigFilePath
        {
            get { return ZetaLongPaths.ZlpPathHelper.Combine(AppDataPath, ProductName, "RepositoryAnalyser.config"); }
        }

        public static string DefaultRepositorySavingPath
        {
            get
            {
                return
                    ZetaLongPaths.ZlpPathHelper.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ProductName, "Repositories");
            }
        }
    }
}
