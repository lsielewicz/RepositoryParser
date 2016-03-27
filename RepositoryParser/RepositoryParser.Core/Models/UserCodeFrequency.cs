using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Models
{
    public class UserCodeFrequency
    {
        public string User { get; set; }
        public int AddedLines { get; set; }
        public int DeletedLines { get; set; }
        public int ModifiedLines { get; set; }

        public UserCodeFrequency(string u, int a, int d, int m)
        {
            this.User = u;
            this.AddedLines = a;
            this.DeletedLines = d;
            this.ModifiedLines = m;
        }
    }
}
