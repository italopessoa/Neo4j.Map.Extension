using Neo4j.Map.Extension.Attributes;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Neo4j.Map.Extension.Model
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class RelationNode<O, D> : Neo4jNode
        where O : Neo4jNode
        where D : Neo4jNode
    {

        public RelationNode()
        {
            Properties = new Dictionary<string, object>();
        }
        /// <summary>
        /// 
        /// </summary>
        public O Origin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public D Destiny { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Neo4jProperty(Name = "type")]
        public string RelationType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="memberName"></param>
        protected void SetPropertyValue(object value, [CallerMemberName] string memberName = "")
        {
            string propKey = memberName.ToLower();
            if (Properties.ContainsKey(propKey))
                Properties[propKey] = value;
            else
                Properties.Add(propKey, value);
        }

        public override string ToString()
        {
            return $"({Origin.UUID})-[:{RelationType}]->({Destiny.UUID})";
        }
    }

    public abstract class RelationNode : RelationNode<Neo4jNode, Neo4jNode>
    {
    }
}
