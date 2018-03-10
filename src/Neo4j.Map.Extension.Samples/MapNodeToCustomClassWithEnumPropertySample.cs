using Neo4j.Driver.V1;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Map.Extension.Map;
using Neo4j.Map.Extension.Samples.Models;

namespace Neo4j.Map.Extension.Samples
{
    internal class MapNodeToCustomClassWithEnumPropertySample
    {
        internal async Task<List<Employee>> Find()
        {
            IDriver driver = GraphDatabase.Driver("bolt://127.0.0.1:7687", AuthTokens.None);
            List<Employee> nodes = new List<Employee>();
            using (ISession session = driver.Session(AccessMode.Read))
            {
                IStatementResultCursor result = await session.RunAsync("MATCH (e:Employee) return e");
                await result.ForEachAsync(r =>
                {
                    nodes.Add(r[r.Keys[0]].Map<Employee>());
                });
            }

            return nodes;
        }
    }
}
