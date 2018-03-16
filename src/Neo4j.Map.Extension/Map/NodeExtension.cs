using Neo4j.Driver.V1;
using Neo4j.Map.Extension.Attributes;
using Neo4j.Map.Extension.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Neo4j.Map.Extension.Map
{
    /// <summary>
    /// Neo4j extension methods
    /// </summary>
    public static class NodeExtension
    {
        /// <summary>
        /// Map Neo4j node to a custom classe
        /// </summary>
        /// <typeparam name="T">Custom class. The class should inherit from <see cref="Neo4jNode"/></typeparam>
        /// <param name="node">Neo4j node <see cref="INode"/></param>
        /// <returns>Mapped object</returns>
        public static T Map<T>(this object node) where T : Neo4jNode
        {
            T result = (T)Activator.CreateInstance(typeof(T));
            IDictionary<string, string> neo4jModelProperties = new Dictionary<string, string>();
            foreach (PropertyInfo propInfo in typeof(T).GetProperties())
            {
                IEnumerable<Neo4jPropertyAttribute> attrs = propInfo.GetCustomAttributes<Neo4jPropertyAttribute>(false);
                foreach (Neo4jPropertyAttribute attr in attrs)
                {
                    string propName = propInfo.Name;
                    string neo4jAttr = attr.Name;
                    neo4jModelProperties.Add(neo4jAttr, propName);
                }
            }

            INode nodeAux = node as INode;
            foreach (KeyValuePair<string, string> property in neo4jModelProperties)
            {
                if (nodeAux.Properties.ContainsKey(property.Key))
                {
                    if (!nodeAux.Properties.ContainsKey(property.Key))
                        throw new InvalidOperationException($"There is not property named \"{property.Key}\". Check your mapping class and your database schema definition.");

                    PropertyInfo propertyInfo = result.GetType().GetProperty(property.Value);
                    object currentPropertyValue = nodeAux.Properties[property.Key];
                    if (propertyInfo.PropertyType.IsEnum)
                    {
                        currentPropertyValue = TryGetEnumValue(propertyInfo, currentPropertyValue);
                    }
                    else
                        currentPropertyValue = nodeAux.Properties[property.Key];

                    propertyInfo.SetValue(result, currentPropertyValue);
                }
            }
            PropertyInfo propertyInfoId = result.GetType().GetProperty("Id");
            propertyInfoId.SetValue(result, nodeAux.Id);

            return result;
        }

        /// <summary>
        /// Generate cypher query from object model
        /// </summary>
        /// <typeparam name="T">Custom class type</typeparam>
        /// <param name="node">Current node</param>
        /// <param name="queryType">The type of cypher query to be generated <see cref="CypherQueryType"/> </param>
        /// <returns>Cypher query</returns>
        public static string MapToCypher<T>(this T node, CypherQueryType queryType) where T : Neo4jNode
        {
            string query = String.Empty;
            switch (queryType)
            {
                case CypherQueryType.Create:
                    query = CreationQuery(node);
                    break;
                case CypherQueryType.Delete:
                    break;
            }
            return query;
        }

        /// <summary>
        /// Generate cypher CREATE query
        /// </summary>
        /// <param name="node">Node oject</param>
        /// <returns>CREATE query</returns>
        private static string CreationQuery(Neo4jNode node)
        {
            string labelName = string.Empty;
            Neo4jLabelAttribute label = node.GetType().GetCustomAttribute<Neo4jLabelAttribute>();
            if (label != null)
            {
                labelName = label.Name;
            }
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (PropertyInfo propInfo in node.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                Neo4jPropertyAttribute attr = propInfo.GetCustomAttribute<Neo4jPropertyAttribute>();
                if (attr != null)
                {
                    if (propInfo.PropertyType.IsEnum)
                        values.Add(attr.Name, TryGetEnumValueDescription(propInfo, propInfo.GetValue(node)));
                    else
                        values.Add(attr.Name, propInfo.GetValue(node));
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.Append($"CREATE (:{labelName} {{");
            string comma = "";
            foreach (KeyValuePair<string, object> keyValue in values)
            {
                sb.Append($"{comma}{keyValue.Key}: {JsonConvert.SerializeObject(keyValue.Value)}");
                comma = ", ";
            }
            sb.Append("})");

            string cypher = sb.ToString().Replace("\"", "'");
            return cypher;
        }

        /// <summary>
        /// Extract Enum value description
        /// </summary>
        /// <param name="propertyInfo">Custom class enum property</param>
        /// <param name="currentPropertyValue">Neo4j node property value</param>
        /// <returns>Enum value</returns>
        private static object TryGetEnumValue(PropertyInfo propertyInfo, object currentPropertyValue)
        {
            foreach (var enumValue in propertyInfo.PropertyType.GetEnumValues())
            {
                MemberInfo enumInfo = propertyInfo.PropertyType.GetMember(enumValue.ToString())[0];
                DescriptionAttribute descriptionAttribute = enumInfo.GetCustomAttribute<DescriptionAttribute>();
                if ((descriptionAttribute != null && descriptionAttribute.Description.Equals(currentPropertyValue))
                    || enumInfo.Name.Equals(currentPropertyValue.ToString()))
                {
                    currentPropertyValue = enumValue;
                    break;
                }
            }
            if (currentPropertyValue == null)
                throw new Neo4jMappingException($"\"{currentPropertyValue}\" is not a valid value for {propertyInfo.PropertyType.FullName}");
            return currentPropertyValue;
        }

        /// <summary>
        /// Get enum value description
        /// </summary>
        /// <param name="propertyInfo">Object propoerty</param>
        /// <param name="currentPropertyValue">Property value</param>
        /// <returns>Enum description</returns>
        private static object TryGetEnumValueDescription(PropertyInfo propertyInfo, object currentPropertyValue)
        {
            foreach (var enumValue in propertyInfo.PropertyType.GetEnumValues())
            {
                MemberInfo enumInfo = propertyInfo.PropertyType.GetMember(enumValue.ToString())[0];
                DescriptionAttribute descriptionAttribute = enumInfo.GetCustomAttribute<DescriptionAttribute>();
                if (descriptionAttribute != null && descriptionAttribute.Description.Equals(currentPropertyValue))
                    return descriptionAttribute.Description;
                else if (enumInfo.Name.Equals(currentPropertyValue.ToString()))
                    return enumInfo.Name;
            }
            return null;
        }
    }
}
