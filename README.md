# dotnet-deps

dotnet-deps resolves dependency graph from a visual studio solution file and outputs it as dot format.

![deps](./docs/deps.png "dependency graph")
```
$ dotnet-deps -s ./Docs.sln
digraph G {
    "Docs.Api"
    "Docs.Interfaces"
    "Docs.Client"
    "Prism"
    "Docs.Api" -> "Prism"
    "System.ValueTuple"
    "Docs.Api" -> "System.ValueTuple"
    "System"
    "Docs.Interfaces" -> "System"
    "System.Core"
    "Docs.Interfaces" -> "System.Core"
    "System.Xml.Linq"
    "Docs.Interfaces" -> "System.Xml.Linq"
    "System.Data.DataSetExtensions"
    "Docs.Interfaces" -> "System.Data.DataSetExtensions"
    "Microsoft.CSharp"
    "Docs.Interfaces" -> "Microsoft.CSharp"
    "System.Data"
    "Docs.Interfaces" -> "System.Data"
    "System.Net.Http"
    "Docs.Interfaces" -> "System.Net.Http"
    "System.Xml"
    "Docs.Interfaces" -> "System.Xml"
    "System"
    "Docs.Client" -> "System"
    "System.Core"
    "Docs.Client" -> "System.Core"
    "System.Xml.Linq"
    "Docs.Client" -> "System.Xml.Linq"
    "System.Data.DataSetExtensions"
    "Docs.Client" -> "System.Data.DataSetExtensions"
    "Microsoft.CSharp"
    "Docs.Client" -> "Microsoft.CSharp"
    "System.Data"
    "Docs.Client" -> "System.Data"
    "System.Net.Http"
    "Docs.Client" -> "System.Net.Http"
    "System.Xml"
    "Docs.Client" -> "System.Xml"
    "Docs.Api" -> "Docs.Interfaces"
    "Docs.Client" -> "Docs.Interfaces"
}
```