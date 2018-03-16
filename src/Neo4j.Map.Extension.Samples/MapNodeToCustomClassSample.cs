using Neo4j.Driver.V1;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Map.Extension.Map;
using Neo4j.Map.Extension.Samples.Models;

namespace Neo4j.Map.Extension.Samples
{
    internal class MapNodeToCustomClassSample
    {
        internal async Task<List<Person>> Find()
        {
            IDriver driver = GraphDatabase.Driver("bolt://127.0.0.1:7687", AuthTokens.None);
            List<Person> nodes = new List<Person>();
            using (ISession session = driver.Session(AccessMode.Read))
            {
                IStatementResultCursor result = await session.RunAsync("MATCH (n:Person) return n");
                await result.ForEachAsync(r =>
                {
                    nodes.Add(r[r.Keys[0]].Map<Person>());
                });
            }
            return nodes;
        }
    }
}
