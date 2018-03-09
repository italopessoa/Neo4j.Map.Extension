using Neo4j.Driver.V1;
using Neo4j.Map.Extension.Attributes;
using Neo4j.Map.Extension.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Neo4j.Map.Extension.Map
{
    /// <summary>
    /// 
    /// </summary>
    public static class NodeExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static T Map<T>(this object node) where T : Neo4jNode
        {
            T result = (T)Activator.CreateInstance(typeof(T));
            IDictionary<string, string> neo4jModelProperties = new Dictionary<string, string>();
            foreach (PropertyInfo propInfo in typeof(T).GetProperties())
            {
                IEnumerable<Neo4jPropertyAttribute> attrs = propInfo.GetCustomAttributes<Neo4jPropertyAttribute>(false);
                foreach (Neo4jPropertyAttribute attr in attrs)
                {
                    if (propInfo.PropertyType.IsEnum)
                    {
                        propInfo.PropertyType.IsEnumDefined("Maria");
                    }
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
    }
}
