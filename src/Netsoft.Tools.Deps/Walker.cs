using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Netsoft.Tools.Deps
{
    public class SolutionWalker
    {
        public void Walk(Solution solution,
            IProgress<Project> nodeReporter,
            IProgress<(Project, Project)> profectRefercenceReporter)
        {
            var graph = solution.GetProjectDependencyGraph();
            var projects = graph.GetTopologicallySortedProjects()
                .Select(id => solution.GetProject(id))
                .ToArray();
            var projectReferences = projects
                .Select(p => new { From = p, DependsOn = graph.GetProjectsThatThisProjectDirectlyDependsOn(p.Id) })
                .ToArray();

            foreach (var project in projects)
            {
                nodeReporter.Report(project);
            }

            foreach (var dependency in projectReferences)
            {
                foreach (var to in dependency.DependsOn)
                {
                    profectRefercenceReporter.Report((dependency.From, solution.GetProject(to)));
                }
            }
        }

        private async Task<(Project,string[])> PostMetadataReferences(Project project, IHttpClientFactory client)
        {
            using (var fs = new FileStream(project.FilePath,
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var sr = new StreamReader(fs))
                {
                    var body = new Dictionary<string, string>()
                    {
                        { "contents", sr.ReadToEnd() },
                    };

                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8080/dependencies")
                    {
                        Content = new FormUrlEncodedContent(body),
                    };
                    var message = await client.CreateClient()
                        .SendAsync(request);
                    string json = await message.Content.ReadAsStringAsync();

                    var response = System.Text.Json.JsonSerializer.Deserialize<Response>(json);

                    return (project, response.dependencies);
                }
            }
        }
    }
}
