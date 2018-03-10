using Neo4j.Map.Extension.Attributes;
using System.ComponentModel;
using Neo4j.Map.Extension.Model;

namespace Neo4j.Map.Extension.Samples.Models
{
    [Neo4jLabel("Person")]
    internal class Person : Neo4jNode
    {
        [Neo4jProperty(Name = "name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Person {{UUID: '{UUID}', Name: '{Name}'}}";
        }
    }

    internal enum Ocuppation
    {
        [Description("Car Mechanic")]
        CarMechanic,
        Carpenter,
        [Description("Doctor")]
        Geneticist
    }
}
