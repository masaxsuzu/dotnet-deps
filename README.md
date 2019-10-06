# dotnet-deps

dotnet-deps resolves dependency graph from a visual studio solution file and outputs it as dot format.

![deps](./docs/deps.png "dependency graph")
```
$ dotnet-deps -s ./Docs.sln
digraph G {                                                                                                 
    "Docs.Api"
    "Docs.Interfaces"
    "Docs.Client"
    "Docs.Api" -> "Docs.Interfaces"
    "Docs.Client" -> "Docs.Interfaces"
}
```