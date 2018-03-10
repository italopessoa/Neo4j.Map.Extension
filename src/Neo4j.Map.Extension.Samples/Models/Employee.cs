using Neo4j.Map.Extension.Attributes;
using Neo4j.Map.Extension.Model;

namespace Neo4j.Map.Extension.Samples.Models
{
    [Neo4jLabel("Employee")]
    internal class Employee : Neo4jNode
    {
        [Neo4jProperty(Name = "name")]
        public string Name { get; set; }

        [Neo4jProperty(Name = "occupation")]
        public Ocuppation Ocuppation { get; set; }

        public override string ToString()
        {
            return $"Person {{Employee: '{UUID}', Name: '{Name}', Occupation: '{Ocuppation}'}}";
        }
    }
}
