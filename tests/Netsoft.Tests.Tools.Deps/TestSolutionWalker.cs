using Microsoft.CodeAnalysis;
using Netsoft.Tools.Deps;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Netsoft.Tests.Tools.Deps
{
    public class TestSolutionWalker
    {
        [Fact]
        public void ShouldReportAllProjects()
        {
            using (var workspace = new AdhocWorkspace())
            {
                var p1 = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "project1", "project1", "C#");
                var p2 = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "project2", "project2", "C#");

                var sln = workspace.AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create()))
                    .AddProject(p1)
                    .AddProject(p2);

                var walker = new SolutionWalker();

                var got = new Progress();
                walker.Walk(sln, got, got);

                Xunit.Assert.Equal(new string[] { "project1", "project2" }, got.Projects.Select(p => p.Name).ToArray());
            }
        }
        [Fact]
        public void ShouldReportAllProjectDependencies()
        {
            using (var workspace = new AdhocWorkspace())
            {

                var p1 = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "project1", "project1", "C#");
                var p2 = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "project2", "project2", "C#");
                var p3 = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "project3", "project3", "C#");

                var sln = workspace.AddSolution(SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create()))
                    .AddProject(p1)
                    .AddProject(p2)
                    .AddProject(p3);

                sln = sln.AddProjectReference(p1.Id, new ProjectReference(p2.Id));
                sln = sln.AddProjectReference(p2.Id, new ProjectReference(p3.Id));
                sln = sln.AddProjectReference(p1.Id, new ProjectReference(p3.Id));

                var walker = new SolutionWalker();

                var got = new Progress();
                walker.Walk(sln, got, got);

                Xunit.Assert.Equal(
                    new string[][] {
                    new string[] {"project1","project2" },
                    new string[] {"project1","project3" },
                    new string[] {"project2","project3" },
                    },
                    got.DepensOn
                    .Select(p => new string[] { p.Item1.Name, p.Item2.Name })
                    .OrderBy(k => string.Join('+', k))
                    .ToArray());
            }
        }
    }

    internal class Progress : IProgress<Project>, IProgress<(Project,Project)>
    {
        public List<Project> Projects { get; private set; }
        public List<(Project, Project)> DepensOn { get; private set; }

        public Progress()
        {
            Projects = new List<Project>();
            DepensOn = new List<(Project, Project)>();
        }
        public void Report(Project value)
        {
            Projects.Add(value);
        }

        public void Report((Project, Project) value)
        {
            DepensOn.Add(value);
        }
    }
}
