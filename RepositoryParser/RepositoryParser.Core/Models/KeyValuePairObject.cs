using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryParser.Core.Models
{
    public class KeyValuePairObject<T1,T2>
    {
        public T1 Key { get;  set; }
        public T2 Value { get;  set; }

        public KeyValuePairObject(T1 key, T2 value)
        {
            Key = key;
            Value = value;
        }

        public KeyValuePairObject(KeyValuePairObject<T1,T2> copy )
        {
            this.Key = copy.Key;
            this.Value = copy.Value;
        }

    }
}
