using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;

namespace RepositoryParser.Core.Messages
{
    public class DataMessageToCharts : MessageBase
    {
        public List<string> AuthorsList { get; set; }
        public string FilteringQuery { get; set; }

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
    }
}
