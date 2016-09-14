using System;
using RepositoryParser.DataBaseManagementCore.Configuration;

namespace RepositoryParser.Core.Models
{
    public class ChangesColorModel
    {
        public string Line { get; set; }
        public ChangeType Color { get; set; }



        public ChangesColorModel()
        {
            this.Line = String.Empty;
            this.Color = ChangeType.Unmodified;
        }

        public ChangesColorModel(string line, ChangeType color)
        {
            this.Line = line;
            this.Color = color;
        }
    }
}
