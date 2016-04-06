using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using RepositoryParser.Core.Models;

namespace RepositoryParser.Core.Messages
{
    public class DataMessageToChartOfChanges :MessageBase
    {
        public List<ChangesColorModel> ChildChangesList { get; set; }

        public DataMessageToChartOfChanges(List<ChangesColorModel> childList)
        {
            this.ChildChangesList = childList;
        } 
    }
}
