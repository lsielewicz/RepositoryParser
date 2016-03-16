using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Core.Messages
{
    public class DataMessageToAnalisys : MessageBase
    {
        public GitRepository GitRepoInstance { get; set; }

        public DataMessageToAnalisys(GitRepository gitRepo)
        {
            GitRepoInstance = gitRepo;
        }
    }
}
