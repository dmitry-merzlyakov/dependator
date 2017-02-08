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
            LazyCompilations = new Lazy<IDictionary<INamedTypeSymbol, Compilation>>(() => symbols.
                SelectMany(s => s.Item3.Select(m => new Tuple<INamedTypeSymbol, Compilation>(m, s.Item2))).
                ToDictionary(t => t.Item1, t => t.Item2));
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
            return LazyCompilations.Value[symbol];
        }

        private readonly Lazy<IEnumerable<INamedTypeSymbol>> LazySolutionSymbols;
        private readonly Lazy<IDictionary<INamedTypeSymbol,Compilation>> LazyCompilations;
    }
}
