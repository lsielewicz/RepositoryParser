using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.DataBaseTests.Configuration
{
    public class LocalizationConstants
    {
        private static string ApplicationName = "RepositoryAnalyser";

        private static string ProgramDataPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData); }
        }

        public static string ProgramDataRepositoryParserPath
        {
            get { return ZetaLongPaths.ZlpPathHelper.Combine(ProgramDataPath, ApplicationName); }
        }

        public static string TestDbDirectory
        {
            get
            {
                return ZetaLongPaths.ZlpPathHelper.Combine(ProgramDataRepositoryParserPath, "Database");
            }
        }


        public static string TestDbLocalization
        {
            get
            {
                return ZetaLongPaths.ZlpPathHelper.Combine(ProgramDataRepositoryParserPath, "Database",
                    "TestDB.sqlite");
            }
        }
    }
}
