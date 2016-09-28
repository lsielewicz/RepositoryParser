using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.Core.Messages
{
    public class DataMessageToDisplay : MessageBase
    {
        public List<Commit> CommitList { get; set; }

        public DataMessageToDisplay(List<Commit> list)
        {
            CommitList = list;
        }
    }
}
