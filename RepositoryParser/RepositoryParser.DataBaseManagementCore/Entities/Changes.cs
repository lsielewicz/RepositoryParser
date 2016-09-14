namespace RepositoryParser.DataBaseManagementCore.Entities
{
    public class Changes : EntityBase
    {
        public virtual string Type { get; set; }
        public virtual string Path { get; set; }
        public virtual string ChangeContent { get; set; }
        public virtual Commit Commit { get;  set; }

        public Changes()
        {
            
        }

    }
}
