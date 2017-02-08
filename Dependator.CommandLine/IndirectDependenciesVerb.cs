using CommandLine;
using Dependator.Core.Controlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.CommandLine
{
    [Verb("indirect-dependencies", HelpText = "Collect all indirect dependencies in the solution and prints a report")]
    public class IndirectDependenciesVerb : BaseOptions
    {
        public override void Run()
        {
            new PrintIndirectReferencesController().Run(SolutionPathName, ProjectFilter);
        }
    }
}
