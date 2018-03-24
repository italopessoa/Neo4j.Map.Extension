# Neo4j.Map.Extension

[![Build status](https://ci.appveyor.com/api/projects/status/65kgw70j31ra7hlt?svg=true)](https://ci.appveyor.com/project/italopessoa/neo4j-map-extension) [![NuGet](https://img.shields.io/badge/nuget-v1.0.1-blue.svg)](https://www.nuget.org/packages/Neo4j.Map.Extension/)

### What is Neo4j.Map.Extension?
Neo4j.Map.Extension is a simple tool that was born from an experiment while I was learning how to use Neo4j.Driver to execute queries in Neo4j. After some repetitive query parses, I tried to make it more abstract for me, using some attributes to configure a graph node as a custom C# class.
### Where can I get it?

You can install Neo4j.Map.Extension using
#### Package Manager
`PM> Install-Package Neo4j.Map.Extension`

#### Paket CLI
`> paket add Neo4j.Map.Extension`
#### .NET CLI
`> dotnet add package Neo4j.Map.Extension`

#### How to use it?
There are two attributes that you can use to set up your custom class, ``Neo4jLabelAttribute`` to map your classes and ``Neo4jPropertyAttribute`` to map your properties.  If you have any class which you want its properties values to be bind by ``Neo4j.Map.Extension``, it must inherit from ``Neo4jNode`` class.

```C#
[Neo4jLabel("Employee")]
class Employee : Neo4jNode
{
    [Neo4jProperty(Name = "name")]
    public string Name { get; set; }

    [Neo4jProperty(Name = "occupation")]
    public Ocuppation Ocuppation { get; set; }

    public override string ToString()
    {
        return $"Person {{Employee: '{UUID}', Name: '{Name}', Occupation: '{Ocuppation}'}}";
    }
}
```
``UUID`` is a default property inherited from the ``Neo4jNode`` class. UUID is a value generated to guarantee that all your nodes have one unique identity value. You can read more about it on [neo4j-uuid](https://github.com/graphaware/neo4j-uuid)

#### Map() - Binding node values to properties

Once you get the return from Neo4j server, you can use the ``Map`` extension method to check and bind all the values of you graph node into your custom class.

```C#
IDriver driver = GraphDatabase.Driver("bolt://127.0.0.1:7687", AuthTokens.None);
List<Employee> nodes = new List<Employee>();
using (ISession session = driver.Session(AccessMode.Read))
{
    IStatementResultCursor result = await session.RunAsync("MATCH (n:Employee) return n");
    await result.ForEachAsync(r =>
    {
        nodes.Add(r[r.Keys[0]].Map<Employee>());
    });
}
```
