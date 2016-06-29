using System;
using RepositoryParser.Core.Enums;

namespace RepositoryParser.Core.Models
{
    public class ChangesColorModel
    {
        public string Line { get; set; }
        public ChangeType Color { get; set; }



        public ChangesColorModel()
        {
            this.Line = String.Empty;
            this.Color = ChangeType.Unchanged;
        }

        public ChangesColorModel(string line, ChangeType color)
        {
            this.Line = line;
            this.Color = color;
        }
    }
}
