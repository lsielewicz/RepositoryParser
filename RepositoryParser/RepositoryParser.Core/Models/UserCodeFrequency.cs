namespace RepositoryParser.Core.Models
{
    public class UserCodeFrequency
    {
        public string User { get; set; }
        public int AddedLines { get; set; }
        public int DeletedLines { get; set; }

        public UserCodeFrequency(string u, int a, int d)
        {
            this.User = u;
            this.AddedLines = a;
            this.DeletedLines = d;
        }
    }
}
