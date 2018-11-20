using Neo4j.Map.Extension.Attributes;
using System.ComponentModel;
using Neo4j.Map.Extension.Model;

namespace Neo4j.Map.Extension.Samples.Models
{
    [Neo4jLabel("User")]
    internal class Person : Neo4jNode
    {
        [Neo4jProperty(Name = "name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Person {{UUID: '{UUID}', Name: '{Name}'}}";
        }
    }

    [Neo4jLabel("Question")]
    internal class Question : Neo4jNode
    {
        [Neo4jProperty(Name = "title")]
        public string Title { get; set; }

        public override string ToString()
        {
            return $"Person {{UUID: '{UUID}', Title: '{Title}'}}";
        }
    }

    internal class RelationErrouNode : RelationNode<Person, Question>
    {
        private long _a;
        private long _b;
        private long _c;
        private long _d;

        [Neo4jProperty(Name = "a")]
        public long A
        {
            get { return _a; }
            set
            {
                SetPropertyValue(value);
                _a = value;
            }
        }


        [Neo4jProperty(Name = "b")]
        public long B
        {
            get { return _b; }
            set
            {
                SetPropertyValue(value);
                _b = value;
            }
        }


        [Neo4jProperty(Name = "c")]
        public long C
        {
            get { return _c; }
            set
            {
                SetPropertyValue(value);
                _c = value;
            }
        }


        [Neo4jProperty(Name = "d")]
        public long D
        {
            get { return _d; }
            set
            {
                SetPropertyValue(value);
                _d = value;
            }
        }

        public RelationErrouNode() : base()
        {
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

    internal enum Relations
    {
        [Description("ERROU")]
        Errou,
        [Description("ACERTOU")]
        Acertou
    }
}
