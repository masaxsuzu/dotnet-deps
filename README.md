# dotnet-deps

dotnet-deps resolves dependency graph from a visual studio solution file and outputs it as dot format.

![deps](./docs/deps.png "dependency graph")
```
$ dotnet-deps -s ./Docs.sln
digraph G {
    "Docs.Types"
    "mscorlib.dll"
    "Docs.Types" -> "mscorlib.dll"
    "System.Core.dll"
    "Docs.Types" -> "System.Core.dll"
    "System.ValueTuple.dll"
    "Docs.Types" -> "System.ValueTuple.dll"
    "Docs.Api"
    "mscorlib.dll"
    "Docs.Api" -> "mscorlib.dll"
    "Prism.dll"
    "Docs.Api" -> "Prism.dll"
    "System.Core.dll"
    "Docs.Api" -> "System.Core.dll"
    "System.ValueTuple.dll"
    "Docs.Api" -> "System.ValueTuple.dll"
}
```
