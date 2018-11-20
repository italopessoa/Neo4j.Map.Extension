using Neo4j.Driver.V1;
using Neo4j.Map.Extension.Map;
using Neo4j.Map.Extension.Model;
using Neo4j.Map.Extension.Samples.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neo4j.Map.Extension.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Creating sample data");
            //CreateSampleNodes();
            //Sample1().GetAwaiter();
            //Sample2().GetAwaiter();
            //Console.ReadKey();
            //Console.WriteLine("Removing sample data");
            //DeleteSampleNodes();
            //Console.WriteLine("Done");
            //Console.ReadKey();
            //Console.WriteLine("Generate cypher query");
            //Sample3().GetAwaiter();
            //Sample4().GetAwaiter();
            //Sample5().GetAwaiter();
            new MapRelation().Find();
            Console.ReadKey();

        }

        private static async Task Sample1()
        {
            MapNodeToCustomClassSample sample1 = new MapNodeToCustomClassSample();
            var nodes = await sample1.Find();
            foreach (var node in nodes)
            {
                Console.WriteLine(node);
            }
        }

        private static async Task Sample2()
        {
            MapNodeToCustomClassWithEnumPropertySample sample2 = new MapNodeToCustomClassWithEnumPropertySample();
            var nodes = await sample2.Find();
            foreach (var node in nodes)
            {
                Console.WriteLine(node);
            }
        }

        private static async Task Sample3()
        {
            MapNodeToCypher sample3 = new MapNodeToCypher();
            Employee employee = new Employee
            {
                Name = "employee name",
                Ocuppation = Ocuppation.Carpenter
            };
            Console.WriteLine(sample3.CreationQuery(employee));
        }

        private static async Task Sample4()
        {
            MapNodeToCustomClassWithEnumPropertySample sample4 = new MapNodeToCustomClassWithEnumPropertySample();
            List<Employee> nodes = await sample4.Find();
            foreach (var node in nodes)
            {
                Console.WriteLine(node);
                Console.WriteLine(node.MapToCypher(CypherQueryType.Delete));
            }
        }

        private static async Task Sample5()
        {
            MapNodeToCustomClassWithEnumPropertySample sample4 = new MapNodeToCustomClassWithEnumPropertySample();
            List<Employee> nodes = await sample4.Find();
            foreach (var node in nodes)
            {
                Console.WriteLine(node);
                node.UUID = null;
                Console.WriteLine(node.MapToCypher(Model.CypherQueryType.Match));
            }
        }

        static void CreateSampleNodes()
        {
            IDriver driver = GraphDatabase.Driver("bolt://127.0.0.1:7687", AuthTokens.None);
            using (ISession session = driver.Session(AccessMode.Write))
            {
                session.Run("CREATE (:Person {name:'Person 1'})");
                session.Run("CREATE (:Person {name:'Person 2'})");
                session.Run("CREATE (:Person {name:'Person 3'})");
                session.Run("CREATE (:Person {name:'Person 4'})");

                session.Run("CREATE (:Employee {name:'Employee 1', occupation:'Car Mechanic' })");
                session.Run("CREATE (:Employee {name:'Employee 2', occupation:'Doctor'})");
                session.Run("CREATE (:Employee {name:'Employee 3', occupation:'Carpenter'})");

            }
            using (ISession session = driver.Session(AccessMode.Write))
            {
                IStatementResult result = session.Run("match (n {uuid: 'b6d1eca0-2fe4-11e8-bcfe-2cd05a628834'}) detach delete n");
            } 
        }

        static void DeleteSampleNodes()
        {
            IDriver driver = GraphDatabase.Driver("bolt://127.0.0.1:7687", AuthTokens.None);
            using (ISession session = driver.Session(AccessMode.Write))
            {
                session.Run("MATCH (p:Person) DETACH DELETE p");
                session.Run("MATCH (c:Car) DETACH DELETE c");
            }
        }
    }
}
