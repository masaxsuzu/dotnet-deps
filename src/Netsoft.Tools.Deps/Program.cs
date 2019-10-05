using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace Netsoft.Tools.Deps
{
    class Program
    {
        static async Task Main()
        {
            // Attempt to set the version of MSBuild.
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            var instance = visualStudioInstances[0];

            MSBuildLocator.RegisterInstance(instance);
            using (var workspace = MSBuildWorkspace.Create())
            {
                string solutionPath = @"..\..\Netsoft.Tools.Deps.sln";

                var solution = await workspace.OpenSolutionAsync(solutionPath);

                var walker = new SolutionWalker();

                using (var progress = new GraphVizReporter(Console.Out))
                {
                    walker.Walk(solution, progress, progress);
                }
            }
        }
    }
}
