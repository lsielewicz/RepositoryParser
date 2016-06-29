using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Messages
{
    public class RefreshMessageToFiltering : RefreshMessageToPresentation
    {
        public RefreshMessageToFiltering(bool refresh) : base(refresh)
        {
        }
    }
}
