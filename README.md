# dotnet-deps

dotnet-deps resolves a dependency graph from a given visual studio solution file and outputs topologically sorted projects as json.

```
PS> dotnet-deps -s .\Docs.sln | ConvertFrom-Json
Name            Order AssemblyName    FilePathFromSolutionDir                    
----            ----- ------------    -----------------------                    
Docs.Interfaces     0 Docs.Interfaces docs\Docs.Interfaces\Docs.Interfaces.csproj
Docs.Api            1 Docs.Api        docs\Docs.Api\Docs.Api.csproj              
Docs.Client         2 Docs.Client     docs\Docs.Client\Docs.Client.csproj        
```
