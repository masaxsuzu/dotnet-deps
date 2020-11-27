using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Netsoft.Tools.Deps
{
    public class DotReporter : IDisposable,
        IProgress<Project>, 
        IProgress<(Project, Project)>,
        IProgress<(Project, object)>
    {
        private readonly TextWriter _writer;
        public DotReporter(TextWriter writer)
        {
            _writer = writer;
            _writer.WriteLine("digraph G {");
        }

        public void Report(Project value)
        {
            _writer.WriteLine($"    \"{value.Name}\"");
        }

        public void Report((Project, Project) value)
        {
            _writer.WriteLine($"    \"{value.Item1.Name}\" -> \"{value.Item2.Name}\"");
        }

        public void Report((Project, object) value)
        {
            _writer.WriteLine($"    \"{value.Item2}\"");
            _writer.WriteLine($"    \"{value.Item1.Name}\" -> \"{value.Item2}\"");
        }

        #region IDisposable Support
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _writer.WriteLine("}");
                }

                _disposedValue = true;
            }
        }

        // ~DotProgress()
        // {
        //   Dispose(false);
        // }

        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class JsonReporter : IDisposable,
        IProgress<Project>, 
        IProgress<(Project, Project)>,
        IProgress<(Project, object)>
    {
        private readonly TextWriter _writer;
        private readonly List<Project> _projects;
        public JsonReporter(TextWriter writer)
        {
            _writer = writer;
            _projects = new List<Project>();
        }

        public void Report(Project value)
        {
            _projects.Add(value);
        }

        public void Report((Project, Project) value)
        {
        }

        public void Report((Project, object) value)
        {
        }

        private string GetFilePathFromSolution(Project project)
        {
            var slnDir = new DirectoryInfo(project.Solution.FilePath);
            return project.FilePath
                .Replace(slnDir.Parent.FullName, "")
                .TrimStart('\\')
                .Replace("\\", "\\\\");
        }

        #region IDisposable Support
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _writer.WriteLine("[");
                    _writer.WriteLine(string.Join(
                        ",\n",
                        _projects
                        .Select(p => $"  {{ \"Name\": \"{p.Name}\", \"AssemblyName\": \"{p.AssemblyName}\", \"FilePathFromSolutionDir\": \"{GetFilePathFromSolution(p)}\" }}")));
                    _writer.WriteLine("]");
                }

                _disposedValue = true;
            }
        }

        // ~DotProgress()
        // {
        //   Dispose(false);
        // }

        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

}
