using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Core.Messages
{
    public class DataMessageToDisplay : MessageBase
    {
        public List<CommitTable> CommitList { get; set; }

        public DataMessageToDisplay(List<CommitTable> list)
        {
            CommitList = list;
        }
    }
}
