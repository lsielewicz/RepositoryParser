using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
