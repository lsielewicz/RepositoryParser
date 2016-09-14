using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.DataBaseManagementCore.Entities
{
    public class EntityBase
    {
        public virtual int Id { get; protected set; }
    }
}
