using Neo4j.Driver.V1;
using Neo4j.Map.Extension.Attributes;
using Neo4j.Map.Extension.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            IDictionary<string, string> neo4jModelProperties = GetNeo4jNodeProperties<T>();

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
                    else if (propertyInfo.PropertyType.IsArray)
                    {
                        if (propertyInfo.PropertyType.GetElementType() == typeof(string))
                            currentPropertyValue = JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(nodeAux.Properties[property.Key]));
                        else if (propertyInfo.PropertyType.GetElementType() == typeof(int))
                            currentPropertyValue = JsonConvert.DeserializeObject<int[]>(JsonConvert.SerializeObject(nodeAux.Properties[property.Key]));
                        else if (propertyInfo.PropertyType.GetElementType() == typeof(long))
                            currentPropertyValue = JsonConvert.DeserializeObject<long[]>(JsonConvert.SerializeObject(nodeAux.Properties[property.Key]));
                    }
                    propertyInfo.SetValue(result, currentPropertyValue);
                }
            }
            PropertyInfo propertyInfoId = result.GetType().GetProperty("Id");
            propertyInfoId.SetValue(result, nodeAux.Id);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="O"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static T MapRelation<T, O, D>(this object node)
            where O : Neo4jNode
            where D : Neo4jNode
            where T : Neo4jNode//TODO: fix
        {
            IDictionary<string, string> neo4jModelProperties = GetNeo4jNodeProperties<T>();

            T result = (T)Activator.CreateInstance(typeof(T));

            IRelationship nodeAux = node as IRelationship;
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

                    try
                    {
                        propertyInfo.SetValue(result, currentPropertyValue);

                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
            }
            PropertyInfo propertyInfoId = result.GetType().GetProperty("Id");
            propertyInfoId.SetValue(result, nodeAux.Id);

            PropertyInfo propertyInfoType = result.GetType().GetProperty("RelationType");
            propertyInfoType.SetValue(result, nodeAux.Type);

            return result;
        }

        public static T MapRelation<T, O, D>(this IReadOnlyDictionary<string, object> values)
            where T : Neo4jNode
            where O : Neo4jNode
            where D : Neo4jNode
        {
            IDictionary<string, string> neo4jModelProperties = GetNeo4jNodeProperties<T>();

            O originValue = values["origin"].Map<O>();
            D destinyValue = values["destiny"].Map<D>();
            T relation = values["relation"].MapRelation<T, O, D>();

            PropertyInfo originPropertyInfo = relation.GetType().GetProperty("Origin");
            PropertyInfo destinyPropertyInfo = relation.GetType().GetProperty("Destiny");

            originPropertyInfo.SetValue(relation, originValue);
            destinyPropertyInfo.SetValue(relation, destinyValue);
            return relation;
        }

        private static IDictionary<string, string> GetNeo4jNodeProperties<T>()
            where T : Neo4jNode
        {
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

            return neo4jModelProperties;
        }

        /// <summary>
        /// Generate cypher query from object model
        /// </summary>
        /// <typeparam name="T">Custom class type</typeparam>
        /// <param name="node">Current node</param>
        /// <param name="queryType">The type of cypher query to be generated <see cref="CypherQueryType"/> </param>
        /// <returns>Cypher query</returns>
        public static string MapToCypher<T>(this T node, CypherQueryType queryType, object parameters = null) where T : Neo4jNode
        {
            string query = string.Empty;
            switch (queryType)
            {
                case CypherQueryType.Create:
                    query = CreationQuery(node);
                    break;
                case CypherQueryType.Merge:
                    query = CreationQuery(node, true);
                    break;
                case CypherQueryType.Delete:
                    query = DeleteQuery(node);
                    break;
                case CypherQueryType.Match:
                    query = MatchQuery(node);
                    break;
                case CypherQueryType.MatchLike:
                    query = MatchQuery(node);
                    break;
                case CypherQueryType.MatchProperties:
                    if (parameters is string[])
                        query = MatchPropertiesQuery(node, parameters as string[]);
                    else
                        throw new ArgumentException(nameof(parameters), "Must be string[].");
                    break;
            }
            return query;
        }

        public static string GetLabel(this Neo4jNode node)
        {
            Neo4jLabelAttribute label = node.GetType().GetCustomAttribute<Neo4jLabelAttribute>();
            if (label != null)
            {
                return label.Name;
            }

            return string.Empty;
        }

        private static string MatchPropertiesQuery<T>(T node, string[] properties) where T : Neo4jNode
        {
            string labelName = node.GetLabel();
            string cypher = string.Empty;

            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            if (properties.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(properties), "At least one property must be informed");

            List<string> propertiesFilter = new List<string>();
            PropertyInfo[] nodeProperties = node.GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = nodeProperties.FirstOrDefault(p => p.Name.Equals(properties[i], StringComparison.InvariantCultureIgnoreCase));
                Neo4jPropertyAttribute attr = property.GetCustomAttribute<Neo4jPropertyAttribute>();
                if (property == null)
                    throw new InvalidOperationException($"Property \"{properties[i]}\" not found.");

                var value = property.GetValue(node);

                if (attr != null && value != null)
                {
                    if (property.PropertyType.IsEnum)
                    {
                        propertiesFilter.Add($"{properties[i]}: {TryGetEnumValueDescription(property, value)}");
                    }
                    else if (property.PropertyType.IsArray)
                    {
                        string[] arrayValue = (string[])value;
                        //values.Add(attr.Name, $"[{arrayValue.Select( string.Join(", ", arrayValue)}]'");
                    }
                    else
                    {
                        if (int.TryParse(value.ToString(), out int x) && int.TryParse(value.ToString().Substring(0, 1), out int z))
                            propertiesFilter.Add($"{attr.Name}: {value.ToString()}");
                        else
                            propertiesFilter.Add($"{attr.Name}: '{value.ToString().Replace("'", @"\'")}'");
                    }
                }
            }
            return propertiesFilter.Count > 0 ? $"MATCH (n:{labelName} {{{string.Join(", ", propertiesFilter)}}}) RETURN n;" : string.Empty;
        }

        private static string MatchQuery<T>(T node) where T : Neo4jNode
        {
            string cypher = string.Empty;
            string labelName = node.GetLabel();

            var uuidProp = node.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals("UUID", StringComparison.InvariantCultureIgnoreCase));
            var idProp = node.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(uuidProp?.GetValue(node)?.ToString()))
            {
                cypher = $"MATCH (n:{labelName} {{uuid: '{uuidProp.GetValue(node)}'}}) RETURN n;";
            }
            else if (!string.IsNullOrEmpty(idProp?.GetValue(node)?.ToString()))
            {
                cypher = $"MATCH (n:{labelName}) WHERE ID(n) = {idProp.GetValue(node)} RETURN n;";
            }
            else
            {
                StringBuilder properties = new StringBuilder();
                Dictionary<string, object> values = new Dictionary<string, object>();
                foreach (PropertyInfo propInfo in node.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    Neo4jPropertyAttribute attr = propInfo.GetCustomAttribute<Neo4jPropertyAttribute>();
                    var value = propInfo.GetValue(node);

                    if (attr != null && value != null)
                    {
                        if (propInfo.PropertyType.IsEnum)
                        {
                            values.Add(attr.Name, TryGetEnumValueDescription(propInfo, value));
                        }
                        else if (propInfo.PropertyType.IsArray)
                        {
                            string[] arrayValue = (string[])value;
                            //values.Add(attr.Name, $"[{arrayValue.Select( string.Join(", ", arrayValue)}]'");
                        }
                        else
                        {
                            values.Add(attr.Name, value);
                        }
                    }
                }

                foreach (KeyValuePair<string, object> keyValue in values)
                {
                    if (properties.Length > 0)
                        properties.Append(", ");

                    if (int.TryParse(keyValue.Value.ToString(), out int x) && int.TryParse(keyValue.Value.ToString().Substring(0, 1), out int z))
                        properties.Append($"{keyValue.Key}:{keyValue.Value}");
                    else
                    {
                        properties.Append($"{keyValue.Key}:'{keyValue.Value.ToString().Replace("'", @"\'")}'");
                    }
                }
                cypher = $"MATCH (n:{labelName} {{{properties.ToString()}}}) RETURN n;";
            }

            return cypher;
        }

        private static string MatchLikeQuery<T>(T node) where T : Neo4jNode
        {
            string labelName = node.GetLabel();
            string cypher = string.Empty;
            var uuidProp = node.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals("UUID", StringComparison.InvariantCultureIgnoreCase));
            if (!String.IsNullOrEmpty(uuidProp?.GetValue(node)?.ToString()))
            {
                cypher = $"MATCH (n:{labelName} {{uuid: '{uuidProp.GetValue(node)}'}}) RETURN n";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                Dictionary<string, object> values = new Dictionary<string, object>();
                foreach (PropertyInfo propInfo in node.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    Neo4jPropertyAttribute attr = propInfo.GetCustomAttribute<Neo4jPropertyAttribute>();
                    var value = propInfo.GetValue(node);
                    if (attr != null && value != null)
                    {
                        if (propInfo.PropertyType.IsEnum)
                            values.Add(attr.Name, TryGetEnumValueDescription(propInfo, value));
                        else
                            values.Add(attr.Name, value);
                    }
                }
                foreach (KeyValuePair<string, object> keyValue in values)
                {
                    if (int.TryParse(keyValue.Value.ToString(), out int x))
                        sb.Append($" {(sb.Length > 0 ? " AND " : string.Empty)} n.{keyValue.Key}={keyValue.Value} ");
                    else sb.Append($" {(sb.Length > 0 ? " AND " : string.Empty)} n.{keyValue.Key}=~'(?i).*{keyValue.Value.ToString().Replace("'", @"\'")}.*' ");
                }
                cypher = $"MATCH (n:{labelName}) WHERE {sb.ToString()} RETURN n";
            }

            return cypher;
        }

        /// <summary>
        /// Generate cypher CREATE query
        /// </summary>
        /// <param name="node">Node object</param>
        /// /// <param name="useMerge">Indicate whether it sould use MERGE instead of CREATE or not. </param>
        /// <returns>CREATE query</returns>
        private static string CreationQuery(Neo4jNode node, bool useMerge = false)
        {
            string labelName = node.GetLabel();

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

            List<string> properties = new List<string>();
            foreach (KeyValuePair<string, object> keyValue in values)
            {
                if (keyValue.Value != null)
                {
                    //if (keyValue.Value.GetType() == typeof(Int64))
                    //    properties.Add($"{keyValue.Key}: {keyValue.Value}");
                    //else if (int.TryParse(keyValue.Value.ToString(), out int x) && int.TryParse(keyValue.Value.ToString().Substring(0, 1), out int z))
                    //    properties.Add($"{keyValue.Key}: {keyValue.Value}");
                    //else if (keyValue.Value.GetType() == typeof(string))
                    //    properties.Add($"{keyValue.Key}: \"{keyValue.Value.ToString().Replace("\"", "\\\"")}\"");
                    //else if (keyValue.Value.GetType().IsArray)
                    //    properties.Add($"{keyValue.Key}: {JsonConvert.SerializeObject(keyValue.Value)}");

                    //if (int.TryParse(keyValue.Value.ToString(), out int x) && !int.TryParse(keyValue.Value.ToString().Substring(0, 1), out int z))
                    //    throw new Exception("NODE WITH ERROR");
                    //else
                        properties.Add($"{keyValue.Key}: {JsonConvert.SerializeObject(keyValue.Value)}");

                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"{(useMerge ? "MERGE" : "CREATE")}");
            sb.Append($"(n:{labelName} {{{string.Join(", ", properties)}}}) RETURN n;");

            return sb.ToString();
        }


        public static string CreateRelationQuery<O, D>(this RelationNode<O, D> relationNode)
             where O : Neo4jNode
            where D : Neo4jNode
        {
            string originLabel = string.Empty;
            string destinyLabel = string.Empty;

            Neo4jLabelAttribute label = relationNode.Origin.GetType().GetCustomAttribute<Neo4jLabelAttribute>();
            if (label != null) originLabel = label.Name;

            label = relationNode.Destiny.GetType().GetCustomAttribute<Neo4jLabelAttribute>();
            if (label != null) destinyLabel = label.Name;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"MATCH (o:{originLabel}), (d:{destinyLabel})");
            sb.AppendLine("WHERE ");

            if (relationNode.Origin.Id > 0 && string.IsNullOrEmpty(relationNode.Origin.UUID))
                sb.Append($"ID (o) = {relationNode.Origin.Id}");
            else
                sb.Append($"o.uuid = '{relationNode.Origin.UUID}'");

            sb.Append(" AND ");

            if (string.IsNullOrEmpty(relationNode.Destiny.UUID) && relationNode.Destiny.Id > 0)
                sb.Append($"ID (d) = {relationNode.Destiny.Id}");
            else
                sb.Append($"d.uuid = '{relationNode.Destiny.UUID}'");

            sb.AppendLine($" CREATE (o)-[r:{relationNode.RelationType}]->(d) ");
            sb.AppendLine("RETURN o,d,r ");

            return sb.ToString();
        }
        /// <summary>
        /// Generate cypher DELETE query
        /// </summary>
        /// <param name="node">Node object</param>
        /// <returns>DELETE query</returns>
        private static string DeleteQuery(Neo4jNode node)
        {
            string labelName = node.GetLabel();
            Dictionary<string, object> values = new Dictionary<string, object>();
            List<PropertyInfo> properties = node.GetType().GetProperties().ToList();
            var uuid = properties.FirstOrDefault(p => p.Name.Equals("UUID", StringComparison.InvariantCultureIgnoreCase));
            var id = properties.FirstOrDefault(p => p.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase));
            if (uuid != null)
            {
                return $"MATCH (n:{labelName} {{uuid:'{uuid.GetValue(node)}'}} DETACH DELETE n";
            }
            else if (id != null)
            {
                return $"MATCH (n:{labelName} {{id:'{id.GetValue(node)}'}} DETACH DELETE n";
            }

            throw new Neo4jMappingException("No node identity found.", new Exception("Check your custom class attributes."));
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
                if (descriptionAttribute != null && enumInfo.Name.Equals(currentPropertyValue.ToString()))
                    return descriptionAttribute.Description;
                else if (enumInfo.Name.Equals(currentPropertyValue.ToString()))
                    return enumInfo.Name;
            }
            return null;
        }
    }
}
