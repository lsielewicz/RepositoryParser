using System;

namespace RepositoryParser.Core.Models
{
    public class ChangesColorModel
    {
        public string Line { get; set; }
        public ChangeType Color { get; set; }

        public enum ChangeType
        {
            Unchanged,
            Added,
            Modified,
            Deleted
        }

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
