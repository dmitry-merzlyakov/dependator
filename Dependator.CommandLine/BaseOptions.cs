using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.CommandLine
{
    public abstract class BaseOptions
    {
        [Option('s', "solution", Required = true, HelpText = "The path and name to a solution to analyze")]
        public string SolutionPathName { get; set; }

        [Option('f', "project-filter", HelpText = "Regex expression to filter projects to analyze")]
        public string ProjectFilter { get; set; }

        public abstract void Run();
    }
}
