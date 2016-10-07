using System;
using System.Xml.Serialization;

namespace RepositoryParser.Configuration
{
    [Serializable]
    public class RepositoryAnalyserConfiguration
    {
        [XmlElement("CurrentLanguage")]
        public string CurrentLanguage { get; set; }

        [XmlElement("DynamicFiltering")]
        public bool DynamicFiltering { get; set; }

        [XmlElement("CloneAllBranches")]
        public bool CloneAllBranches { get; set; }

        [XmlElement("SavingRepositoryPath")]
        public string SavingRepositoryPath { get; set; }

        
    }
}
