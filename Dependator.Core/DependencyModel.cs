using Microsoft.CodeAnalysis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core
{
    public sealed class DependencyModel : ReferenceModel
    {
        public DependencyModel(IEnumerable<Tuple<Project,Compilation,IEnumerable<INamedTypeSymbol>>> symbols)
        {
            if (symbols == null)
                throw new ArgumentNullException("symbols");

            Symbols = symbols.SelectMany(s => s.Item3).ToList();
            SolutionNamespaces = symbols.Select(s => s.Item1.Name).ToList();

            LazySolutionSymbols = new Lazy<IEnumerable<INamedTypeSymbol>>(() => Symbols.Where(s => SolutionNamespaces.Contains(s.ContainingAssembly.Name)).ToList(), true);
            LazyCompilations = new Lazy<IDictionary<string, Compilation>>(() => GetCompilations(symbols));
        }

        public IEnumerable<INamedTypeSymbol> Symbols { get; private set; }
        public IEnumerable<string> SolutionNamespaces { get; private set; }
        public IEnumerable<INamedTypeSymbol> SolutionSymbols => LazySolutionSymbols.Value;

        public bool IsSolutionSymbol(INamedTypeSymbol symbol)
        {
            return LazySolutionSymbols.Value.Contains(symbol);
        }

        public Compilation GetCompilation(INamedTypeSymbol symbol)
        {
            return LazyCompilations.Value[symbol.ToString()];
        }

        private static IDictionary<string, Compilation> GetCompilations(IEnumerable<Tuple<Project, Compilation, IEnumerable<INamedTypeSymbol>>> symbols)
        {
            var compilations = new Dictionary<string, Compilation>();

            // The order to add projects is essential
            foreach (var project in symbols)
            {
                foreach (var symbol in project.Item3)
                {
                    if (!compilations.ContainsKey(symbol.ToString()))
                        compilations.Add(symbol.ToString(), project.Item2);
                }
            }

            return compilations;
        }

        private readonly Lazy<IEnumerable<INamedTypeSymbol>> LazySolutionSymbols;
        private readonly Lazy<IDictionary<string,Compilation>> LazyCompilations;
    }
}
