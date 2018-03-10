using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Neo4j.Map.Extension.Map
{
    [Serializable]
    internal class Neo4jMappingException : Exception
    {
        public Neo4jMappingException()
        {
        }

        public Neo4jMappingException(string message) : base(message)
        {
        }

        public Neo4jMappingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected Neo4jMappingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
