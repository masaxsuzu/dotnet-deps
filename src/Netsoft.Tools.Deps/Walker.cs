using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Netsoft.Tools.Deps
{
    public class SolutionWalker
    {
        public void Walk(Solution solution, IProgress<Project> nodeReporter, IProgress<(Project, Project)> edgeReporter)
        {
            var graph = solution.GetProjectDependencyGraph();
            var projects = solution.ProjectIds.Select(id => solution.GetProject(id))
                .ToArray();
            var dependencies = projects
                .Select(p => new { From = p, DependsOn = graph.GetProjectsThatThisProjectDirectlyDependsOn(p.Id) })
                .ToArray();

            foreach (var project in projects)
            {
                nodeReporter.Report(project);
            }

            foreach (var dependency in dependencies)
            {
                foreach (var to in dependency.DependsOn)
                {
                    edgeReporter.Report((dependency.From, solution.GetProject(to)));
                }
            }
        }
    }
}
