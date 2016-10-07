namespace RepositoryParser.Configuration
{
    public sealed class ApplicationLanguage
    {
        public static readonly ApplicationLanguage Polish = new ApplicationLanguage("pl-PL");
        public static readonly ApplicationLanguage English = new ApplicationLanguage("en-EN");

        #region Type-Safe enum pattern
        public string Name { get; private set; }

        private ApplicationLanguage(string name)
        {
            this.Name = name;
        }

        public static implicit operator string(ApplicationLanguage obj)
        {
            return obj.Name;
        }
        #endregion
    }
}
