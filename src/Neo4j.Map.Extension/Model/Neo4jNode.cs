using Neo4j.Map.Extension.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Neo4j.Map.Extension.Model
{
    /// <summary>
    /// Specife the base properties of a node
    /// </summary>
    public abstract class Neo4jNode
    {
        /// <summary>
        /// Node Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Node UUID
        /// </summary>
        /// <remarks>https://github.com/graphaware/neo4j-uuid</remarks>
        [Neo4jProperty(Name = "uuid")]
        [JsonIgnore]
        public string UUID { get; set; }
    }
}
