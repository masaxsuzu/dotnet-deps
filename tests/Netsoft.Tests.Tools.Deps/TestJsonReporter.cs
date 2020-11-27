using Microsoft.CodeAnalysis;
using Netsoft.Tools.Deps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Netsoft.Tests.Tools.Deps
{
    public class TestJsonReporter
    {
        [Fact]
        public void ShouldCloseArray()
        {
            using (var m = new MemoryStream())
            {
                using (var sw = new StreamWriter(m))
                {
                    using (var reporter = new JsonReporter(sw))
                    {

                    }
                    sw.Flush();
                }

                Assert.Equal(@"[

]
".Replace("\r\n", "\n"), System.Text.Encoding.UTF8.GetString(m.ToArray()).Replace("\r\n","\n"));
            }
        }
        [Fact]
        public void ShouldShowSortedOrder()
        {
            using (var workspace = new AdhocWorkspace())
            {
                workspace.AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(),VersionStamp.Create(), "c:\\ws\\app\\app.sln"));

                var p1 = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "A", "A.dll", "C#", "c:\\ws\\app\\src\\A\\A.csproj");
                var p2 = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "B", "Y.dll", "C#", "c:\\ws\\app\\src\\B\\B.csproj");
                
                using (var m = new MemoryStream())
                {
                    using (var sw = new StreamWriter(m))
                    {
                        using (var reporter = new JsonReporter(sw))
                        {
                            var a = workspace.AddProject(p1);
                            var b = workspace.AddProject(p2);
                            reporter.Report(a);
                            reporter.Report(b);
                        }
                        sw.Flush();
                    }

                    Assert.Equal(@"[
  { ""Name"": ""A"", ""Order"": 0, ""AssemblyName"": ""A.dll"", ""FilePathFromSolutionDir"": ""src\\A\\A.csproj"" },
  { ""Name"": ""B"", ""Order"": 1, ""AssemblyName"": ""Y.dll"", ""FilePathFromSolutionDir"": ""src\\B\\B.csproj"" }
]
".Replace("\r\n", "\n"), System.Text.Encoding.UTF8.GetString(m.ToArray()).Replace("\r\n","\n"));
                }
            }
        }
    }
}
