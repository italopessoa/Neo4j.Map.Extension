using Neo4j.Map.Extension.Attributes;
using Neo4j.Map.Extension.Model;
using Newtonsoft.Json;

namespace Neo4j.Map.Extension.Samples.Models
{
    [Neo4jLabel("Employee")]
    internal class Employee : Neo4jNode
    {
        [Neo4jProperty(Name = "name")]
        public string Name { get; set; }

        [Neo4jProperty(Name = "occupation")]
        [JsonProperty("hahaha")]
        public Ocuppation Ocuppation { get; set; }

        [Neo4jProperty(Name = "roles")]
        public string[] Roles { get; set; }
        public override string ToString()
        {
            return $"Person {{Employee: '{UUID}', Name: '{Name}', Occupation: '{Ocuppation}'}}";
        }
    }

    public class Relation
    {
        public Neo4jNode Origin { get; set; }

        public Neo4jNode Destiny { get; set; }

        public string Type { get; set; }
    }
}
