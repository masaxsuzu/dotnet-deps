using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netsoft.Tools.Deps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace Netsoft.Tests.Tools.Deps
{
    public class Fixture:IDisposable
    {
        private readonly IServiceScope _scope;
        public IHttpClientFactory ClientFactory { get; set; }
        public Fixture()
        {
            MSBuildLocator.RegisterDefaults();

            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) => services.AddHttpClient()).UseConsoleLifetime();

            var host = builder.Build();

            _scope = host.Services.CreateScope();
            ClientFactory = _scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
    public class TestSolutionWalker: IClassFixture<Fixture>
    {
        private readonly Fixture _fixture;
        public TestSolutionWalker(Fixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReportAllProjectsAsync()
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                var sln = workspace.OpenSolutionAsync("..\\..\\..\\..\\..\\Docs.sln").Result;
                var walker = new SolutionWalker();

                var got = new Progress();
                await walker.WalkAsync(sln,_fixture.ClientFactory , got, got, got);

                Xunit.Assert.Equal(
                    new string[] { "Docs.Api", "Docs.Interfaces", "Docs.Client" },
                    got.Projects.Select(p => p.Name).ToArray());
            }
        }
        [Fact]
        public async System.Threading.Tasks.Task ShouldReportAllProjectDependenciesAsync()
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                var sln = workspace.OpenSolutionAsync("..\\..\\..\\..\\..\\Docs.sln").Result;

                var walker = new SolutionWalker();

                var got = new Progress();
                await walker.WalkAsync(sln, _fixture.ClientFactory, got, got, got);

                Xunit.Assert.Equal(
                    new string[][] {
                        new string[] { "Docs.Api", "Docs.Interfaces" },
                        new string[] { "Docs.Client","Docs.Interfaces" },
                    },
                    got.DepensOn
                    .Select(p => new string[] { p.Item1.Name, p.Item2.Name })
                    .OrderBy(k => string.Join('+', k))
                    .ToArray());
            }
        }

        [Fact]
        public async System.Threading.Tasks.Task ShouldReportAllMetadataDependenciesAsync()
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                var sln = workspace.OpenSolutionAsync("..\\..\\..\\..\\..\\Docs.sln").Result;

                var walker = new SolutionWalker();

                var got = new Progress();
                await walker.WalkAsync(sln, _fixture.ClientFactory, got, got, got);

                string[][] metadata = got.Metadata
                    .Select(p => new string[] { p.Item1.Name, p.Item2.ToString() })
                    .OrderBy(k => string.Join('+', k))
                    .ToArray();
                Xunit.Assert.Equal(
                    new string[][] {
                        new string[] { "Docs.Api", "Prism" },
                        new string[] { "Docs.Api", "System.ValueTuple" },
                        new string[] { "Docs.Client", "Microsoft.CSharp" },
                        new string[] { "Docs.Client", "System" },
                        new string[] { "Docs.Client", "System.Core" },
                        new string[] { "Docs.Client", "System.Data" },
                        new string[] { "Docs.Client", "System.Data.DataSetExtensions" },
                        new string[] { "Docs.Client", "System.Net.Http" },
                        new string[] { "Docs.Client", "System.Xml" },
                        new string[] { "Docs.Client", "System.Xml.Linq" },
                        new string[] { "Docs.Interfaces", "Microsoft.CSharp" },
                        new string[] { "Docs.Interfaces", "System" },
                        new string[] { "Docs.Interfaces", "System.Core" },
                        new string[] { "Docs.Interfaces", "System.Data" },
                        new string[] { "Docs.Interfaces", "System.Data.DataSetExtensions" },
                        new string[] { "Docs.Interfaces", "System.Net.Http" },
                        new string[] { "Docs.Interfaces", "System.Xml" },
                        new string[] { "Docs.Interfaces", "System.Xml.Linq" },
                    }, metadata);
            }
        }
    }

    internal class Progress : 
        IProgress<Project>, 
        IProgress<(Project,Project)>, 
        IProgress<(Project, object)>
    {
        public List<Project> Projects { get; private set; }
        public List<(Project, Project)> DepensOn { get; private set; }
        public List<(Project, object)> Metadata { get; private set; }

        public Progress()
        {
            Projects = new List<Project>();
            DepensOn = new List<(Project, Project)>();
            Metadata = new List<(Project, object)>();
        }
        public void Report(Project value)
        {
            Projects.Add(value);
        }

        public void Report((Project, Project) value)
        {
            DepensOn.Add(value);
        }

        public void Report((Project, object) value)
        {
            Metadata.Add(value);
        }
    }
}
