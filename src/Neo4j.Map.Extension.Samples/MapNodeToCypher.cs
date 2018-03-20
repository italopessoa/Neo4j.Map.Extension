using Neo4j.Map.Extension.Map;
using Neo4j.Map.Extension.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4j.Map.Extension.Samples
{
    internal class MapNodeToCypher
    {
        public string CreationQuery(Neo4jNode node)
        {
            return node.MapToCypher(CypherQueryType.Create);
        }
    }
}
