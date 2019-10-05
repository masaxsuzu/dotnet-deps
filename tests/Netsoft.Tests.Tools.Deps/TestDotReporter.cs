using Microsoft.CodeAnalysis;
using Netsoft.Tools.Deps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Netsoft.Tests.Tools.Deps
{
    public class TestDotReporter
    {
        [Fact]
        public void ShouldCloseGraph()
        {
            using (var m = new MemoryStream())
            {
                using (var sw = new StreamWriter(m))
                {
                    using (var reporter = new DotReporter(sw))
                    {

                    }
                    sw.Flush();
                }

                Assert.Equal(@"digraph G {
}
", System.Text.Encoding.UTF8.GetString(m.ToArray()));
            }
        }

        [Fact]
        public void ShouldDoubleQuote()
        {
            using (var workspace = new AdhocWorkspace())
            {
                var p1 = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "A", "A", "C#");
                var p2 = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "B.C", "B.C", "C#");
               
                using (var m = new MemoryStream())
                {
                    using (var sw = new StreamWriter(m))
                    {
                        using (var reporter = new DotReporter(sw))
                        {
                            var a = workspace.AddProject(p1);
                            var b = workspace.AddProject(p2);
                            reporter.Report(a);
                            reporter.Report(b);

                            reporter.Report((a, b));
                        }
                        sw.Flush();
                    }

                    Assert.Equal(@"digraph G {
    ""A""
    ""B.C""
    ""A"" -> ""B.C""
}
", System.Text.Encoding.UTF8.GetString(m.ToArray()));
                }
            }
        }
    }
}
