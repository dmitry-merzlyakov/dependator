using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dependator.Core.Controlers
{
    public sealed class PrintIndirectReferencesController
    {
        public void Run(string solutionPathName, string projectFilterRegex = null)
        {
            if (String.IsNullOrWhiteSpace(solutionPathName))
                throw new ArgumentNullException("solutionPathName");

            var solutionPath = Path.GetDirectoryName(solutionPathName);
            var solutionName = Path.GetFileName(solutionPathName);

            Console.WriteLine("Running");
            var reader = new ProjectReader(solutionPath, solutionName);

            Console.WriteLine(String.Format("Reading files for the solution {0} (path {1})", solutionName, solutionPath));

            var regexProjectFilter = !String.IsNullOrWhiteSpace(projectFilterRegex) ? new Regex(projectFilterRegex, RegexOptions.IgnoreCase) : null;
            var model = regexProjectFilter == null ? reader.Read() : reader.Read(p => regexProjectFilter.IsMatch(p.Name));
            Console.WriteLine(String.Format("Loaded '{0}' project(s); '{1}' symbol(s)", model.SolutionNamespaces.Count(), model.Symbols.Count()));

            Console.WriteLine("Building dependency tree");
            new ReferenceBuilder().Build(model);

            Console.WriteLine("Collecting indirect references");
            var final = new DependencyAnalyzer().SelectIndirectReferences(model);

            Console.WriteLine("Writing a report file");
            var report = new Reporter().PrintFromToEndpoints(final);

            Console.WriteLine();
            Console.WriteLine(report);
        }
    }
}
