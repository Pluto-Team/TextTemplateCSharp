using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTemplate
{
    class TypedData
    {
        public string type;
        public string missingValue;
        public string key;
        public List<object> list;
        public string defaultIndent;
        public object parts;
        public string bullet;
        public TypedData(string type, string missingValue = null, string key = null, List<object> list = null, string defaultIndent = null, object parts = null, string bullet = "")
        {
            this.type = type;
            this.missingValue = missingValue;
            this.key = key;
            this.list = list;
            this.defaultIndent = defaultIndent;
            this.bullet = bullet;
            this.parts = parts;
        }
    }
}
