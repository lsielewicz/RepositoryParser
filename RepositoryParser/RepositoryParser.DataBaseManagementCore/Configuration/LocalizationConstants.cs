using System;

namespace RepositoryParser.DataBaseManagementCore.Configuration
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

        public static string ProgramDataDataBaseDirectoryPath
        {
            get
            {
                return ZetaLongPaths.ZlpPathHelper.Combine(ProgramDataRepositoryParserPath, "Database");
            }
        }


        public static string ProgramDataDataBaseLocationPath
        {
            get
            {
                return ZetaLongPaths.ZlpPathHelper.Combine(ProgramDataRepositoryParserPath, "Database",
                    "RepositoryAnalyserData.sqlite");
            }
        }


}
}
