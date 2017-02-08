using Microsoft.CodeAnalysis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core
{
    public sealed class NamedTypeSymbolFinder
    {
        public IEnumerable<INamedTypeSymbol> GetSymbols(Compilation compilation)
        {
            var visitor = new NamedTypeSymbolVisitor();
            visitor.Visit(compilation.GlobalNamespace);
            return visitor.AllTypeSymbols;
        }

        private class NamedTypeSymbolVisitor : SymbolVisitor
        {
            public BlockingCollection<INamedTypeSymbol> AllTypeSymbols { get; } = new BlockingCollection<INamedTypeSymbol>();

            public override void VisitNamespace(INamespaceSymbol symbol)
            {
                Parallel.ForEach(symbol.GetMembers(), s => s.Accept(this));
            }

            public override void VisitNamedType(INamedTypeSymbol symbol)
            {
                AllTypeSymbols.Add(symbol);

                foreach (var childSymbol in symbol.GetTypeMembers())
                {
                    base.Visit(childSymbol);
                }
            }
        }

    }
}
