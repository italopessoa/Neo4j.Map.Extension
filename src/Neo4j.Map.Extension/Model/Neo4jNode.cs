using Neo4j.Map.Extension.Attributes;
using System.Collections.Generic;

namespace Neo4j.Map.Extension.Model
{
    public abstract class Neo4jNode
    {
        public IReadOnlyDictionary<string, object> Properties
        {
            get; set;
        }


        public long Id
        {
            get; set;
        }

        [Neo4jProperty(Name = "uuid")]
        public string UUID { get; set; }
    }
}
