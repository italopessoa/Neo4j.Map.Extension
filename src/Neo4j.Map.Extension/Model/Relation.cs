using Neo4j.Map.Extension.Attributes;
using Neo4j.Map.Extension.Map;
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
        protected RelationNode()
        {
            Properties = new Dictionary<string, object>();
        }

        protected RelationNode(O origin, D destiny)
        {
            Properties = new Dictionary<string, object>();
            Origin = origin;
            Destiny = destiny;
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
    }

    //public abstract class RelationNode : RelationNode<Neo4jNode, Neo4jNode>
    //{
    //}
}
