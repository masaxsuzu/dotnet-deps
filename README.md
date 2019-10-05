# dotnet-deps

dotnet-deps resolves dependency graph from a visual studio solution file and show it as dot format.

```
$ dotnet-deps -s 'path-to-solution'
digraph G {
    "Netsoft.Tests.Tools.Deps"
    "Netsoft.Tests.Tools.Deps"
    "Netsoft.Tests.Tools.Deps" -> "Netsoft.Tools.Deps"
}
```