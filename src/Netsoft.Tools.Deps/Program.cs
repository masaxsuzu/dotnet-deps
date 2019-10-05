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
using CommandLine;
namespace Netsoft.Tools.Deps
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var parsed = CommandLine.Parser.Default.ParseArguments<Arguments>(args);

            if (parsed.Tag == ParserResultType.NotParsed)
            {
                return 1;
            }

            string solutionPath = (parsed as Parsed<Arguments>)
                .Value
                .SolutionFilePath;

            if (!File.Exists(solutionPath))
            {
                Console.Error.WriteLine($"Not found solution file '{solutionPath}'.");
                return 2;
            }

            // Attempt to set the version of MSBuild.
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();

            if (visualStudioInstances.Length < 0)
            {
                Console.Error.WriteLine($"Not found Visual Studio instance.");
                return 3;
            }

            var instance = visualStudioInstances[0];

            MSBuildLocator.RegisterInstance(instance);
            using (var workspace = MSBuildWorkspace.Create())
            {
                var solution = await workspace.OpenSolutionAsync(solutionPath);

                var walker = new SolutionWalker();

                using (var progress = new DotReporter(Console.Out))
                {
                    walker.Walk(solution, progress, progress);
                }
            }

            return 0;
        }
    }
}
