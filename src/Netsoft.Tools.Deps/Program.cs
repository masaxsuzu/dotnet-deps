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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

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

            MSBuildLocator.RegisterDefaults();

            using (var workspace = MSBuildWorkspace.Create())
            {
#if DEBUG
                //System.Diagnostics.Debugger.Launch();
#endif
                using (var serviceScope = HostHttpClient())
                {
                    var services = serviceScope.Services;
                    var client = services.GetRequiredService<IHttpClientFactory>();
                    try
                    {
                        await Walk(solutionPath, workspace);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                        return 3;
                    }
                }
            }

            return 0;
        }

        public static async Task Walk(string solutionPath, MSBuildWorkspace workspace)
        {
            var solution = await workspace.OpenSolutionAsync(solutionPath);

            using (var progress = new JsonReporter(Console.Out))
            {
                var walker = new SolutionWalker();
                walker.Walk(solution, progress, progress);
            }
        }

        private static IHost HostHttpClient()
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) => services.AddHttpClient()).UseConsoleLifetime();
            return builder.Build();
        }
    }
}
