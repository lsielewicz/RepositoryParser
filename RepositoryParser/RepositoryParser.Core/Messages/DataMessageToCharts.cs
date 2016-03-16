using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Core.Messages
{
    public class DataMessageToCharts : MessageBase
    {
        public GitRepository RepoInstance { get; set; }
        public List<string> AuthorsList { get; set; }
        public string FilteringQuery { get; set; }

        public DataMessageToCharts(GitRepository temp, List<string> list)
        {
            RepoInstance = temp;
            AuthorsList = list;
        }

        public DataMessageToCharts(GitRepository repo, string generatedQuery)
        {
            RepoInstance = repo;
            FilteringQuery = generatedQuery;
        }
        public DataMessageToCharts(GitRepository repo, List<string> authorsList, string generatedQuery)
        {
            RepoInstance = repo;
            FilteringQuery = generatedQuery;
            AuthorsList = authorsList;
        }
    }
}
