using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;

namespace RepositoryParser.Core.Messages
{
    public class DataMessageToCharts : MessageBase
    {
        public List<string> AuthorsList { get; private set; }
        public string FilteringQuery { get; private set; }
        public bool IsFromContent { get; private set; }

        public DataMessageToCharts(List<string> list)
        {
            AuthorsList = list;
        }

        public DataMessageToCharts(string generatedQuery)
        {
            FilteringQuery = generatedQuery;
        }
        public DataMessageToCharts(List<string> authorsList, string generatedQuery)
        {
            FilteringQuery = generatedQuery;
            AuthorsList = authorsList;
        }
        public DataMessageToCharts(List<string> authorsList, string generatedQuery, bool isToContent)
        {
            FilteringQuery = generatedQuery;
            AuthorsList = authorsList;
            IsFromContent = isToContent;
        }
    }
}
