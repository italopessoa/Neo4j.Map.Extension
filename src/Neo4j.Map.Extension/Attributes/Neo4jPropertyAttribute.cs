using System;

namespace Neo4j.Map.Extension.Attributes
{
    /// <summary>
    /// Specified a node property name
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Neo4jPropertyAttribute : Attribute
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Specified a node property name
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Neo4jRelationPropertyAttribute : Attribute
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; set; }
    }
}
