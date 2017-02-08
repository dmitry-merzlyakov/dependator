using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core
{
    public sealed class ProjectReader
    {
        public ProjectReader(string solutionFolder, string solutionName)
        {
            if (String.IsNullOrWhiteSpace(solutionFolder))
                throw new ArgumentNullException("solutionFolder");
            if (String.IsNullOrWhiteSpace(solutionName))
                throw new ArgumentNullException("solutionName");

            SolutionFolder = solutionFolder;
            SolutionName = solutionName;
        }

        public string SolutionFolder { get; private set; }
        public string SolutionName { get; private set; }

        public DependencyModel Read(Func<Project,bool> projectFilter = null)
        {
            Solution solution = MSBuildWorkspace.Create()
                        .OpenSolutionAsync(Path.Combine(SolutionFolder, SolutionName))
                        .Result;

            var projects = solution.Projects;
            if (projectFilter != null)
                projects = projects.Where(p => projectFilter(p));


            var list = new List<Tuple<Project, Compilation, IEnumerable<INamedTypeSymbol>>>();
            foreach(var project in projects)
            {
                var compilation = project.GetCompilationAsync().Result;
                var symbols = new NamedTypeSymbolFinder().GetSymbols(compilation).Where(s => s != null && s.CanBeReferencedByName).ToList();
                list.Add(new Tuple<Project, Compilation, IEnumerable<INamedTypeSymbol>>(project,compilation,symbols));
            }

            return new DependencyModel(list);
        }
    }
}
