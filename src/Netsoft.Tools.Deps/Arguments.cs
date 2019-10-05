using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netsoft.Tools.Deps
{
    class Arguments
    {
        [Option('s', "solution", Required = true, HelpText = "Solution path to analyze.")]
        public string SolutionFilePath { get; set; }
    }
}
