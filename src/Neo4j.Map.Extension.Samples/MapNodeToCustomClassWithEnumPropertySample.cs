using Neo4j.Driver.V1;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Map.Extension.Map;
using Neo4j.Map.Extension.Samples.Models;
using System;
using Neo4j.Map.Extension.Model;

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
                string origin = "User";
                string relation = "HAS_FAILED";
                string destiny = "Question";
                //IStatementResultCursor result = await session.RunAsync($"MATCH (origin:{origin})-[relation:{relation}]-(destiny:{destiny}) return origin,destiny,relation");
                IStatementResultCursor result = await session.RunAsync("MATCH (e:Employee) return e");

                await result.ForEachAsync(r =>
                {
                    nodes.Add(r[r.Keys[0]].Map<Employee>());
                });
            }

            return nodes;
        }
    }

    internal class MapRelation
    {
        internal async void Find()
        {
            IDriver driver = GraphDatabase.Driver("bolt://127.0.0.1:7687", AuthTokens.Basic("neo4j", "bitcoinshow"));
            List<Person> nodes = new List<Person>();
            List<RelationErrouNode> relations = new List<RelationErrouNode>();
            using (ISession session = driver.Session(AccessMode.Read))
            {
                string origin = "User";
                string relation = "";
                string destiny = "Question";

                IStatementResultCursor result = await session.RunAsync($"MATCH (origin:{origin})-[relation{relation}]->(destiny:{destiny}) return origin,destiny,relation");

                await result.ForEachAsync(r =>
                {
                    var x = r.Values.MapRelation<RelationErrouNode, Person, Question>();// <RelationErrouNode>();
                    relations.Add(x);
                });
            }
        }
    }
}
