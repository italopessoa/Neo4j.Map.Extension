using System;

namespace Neo4j.Map.Extension.Attributes
{
    /// <summary>
    /// Specifies a Label for a class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Neo4jLabelAttribute : Attribute
    {
        /// <summary>
        /// The Label name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Node label</param>
        public Neo4jLabelAttribute(string name)
        {
            Name = name;
        }
    }
}
