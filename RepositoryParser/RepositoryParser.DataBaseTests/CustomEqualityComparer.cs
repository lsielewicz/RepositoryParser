using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Data;
using RepositoryParser.DataBaseManagementCore.Entities;

namespace RepositoryParser.DataBaseTests
{
    public class CustomEqualityComparer : IEqualityComparer
    {
        public bool Equals(object x, object y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            EntityBase xEntity = x as EntityBase;
            EntityBase yEntity = y as EntityBase;

            if (xEntity == null || yEntity == null)
            {
                return x.Equals(y);
            }

            return xEntity.Id == yEntity.Id;
        }

        public int GetHashCode(object obj)
        {
            EntityBase e = obj as EntityBase;

            if (e != null)
                return e.Id;
            else
                return base.GetHashCode();
        }
    }
}
