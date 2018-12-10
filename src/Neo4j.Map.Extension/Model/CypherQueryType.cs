using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4j.Map.Extension.Model
{
    public enum CypherQueryType
    {
        Create,
        Merge,
        Delete,
        Match,
        MatchLike
    }
}
