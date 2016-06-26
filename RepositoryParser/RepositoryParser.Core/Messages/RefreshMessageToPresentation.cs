using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace RepositoryParser.Core.Messages
{
    public class RefreshMessageToPresentation : MessageBase
    {
        public bool Refresh { get; private set; }

        public RefreshMessageToPresentation(bool refresh)
        {
            Refresh = refresh;
        }
    }
}
