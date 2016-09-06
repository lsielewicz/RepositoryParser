using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;

namespace RepositoryParser.DataBaseManagementCore.Configuration
{
    public sealed class ChangeType
    {
        public static readonly ChangeType Added = new ChangeType("Added");
        public static readonly ChangeType Deleted = new ChangeType("Deleted");
        public static readonly ChangeType Modified = new ChangeType("Modified");

        #region Type-Safe enum pattern
        public string Name { get; private set; }

        private ChangeType(string name)
        {
            this.Name = name;
        }

        public static implicit operator string(ChangeType obj)
        {
            return obj.Name;
        }
        #endregion
    }
}
