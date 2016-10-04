using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Models
{
    public class CodeFrequencyDataRow
    {
        public int AddedLines { get; set; }
        public int DeletedLines { get; set; }
        public string Repository { get; set; }
        public string ChartKey { get; set; }
    }
}
