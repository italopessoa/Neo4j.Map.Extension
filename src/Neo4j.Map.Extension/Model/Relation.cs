using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4j.Map.Extension.Model
{
    public class RelationNode
    {
        public string Name { get; }

        public RelationNode(string name)
        {
            Name = name;
        }

        public Neo4jNode Origin { get; set; }

        public Neo4jNode Destiny { get; set; }

        public Dictionary<string, object> Properties { get; set; }
    }
}
