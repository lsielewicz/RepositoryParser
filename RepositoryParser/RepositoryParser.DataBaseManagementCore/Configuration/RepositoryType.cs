namespace RepositoryParser.DataBaseManagementCore.Configuration
{
    public sealed class RepositoryType
    {
        public static readonly RepositoryType Git = new RepositoryType("Git");
        public static readonly RepositoryType Svn = new RepositoryType("Svn");

        #region Type-Safe enum pattern
        public string Name { get; private set; }

        private RepositoryType(string name)
        {
            this.Name = name;
        }

        public static implicit operator string(RepositoryType obj)
        {
            return obj.Name;
        }
        #endregion
    }
}