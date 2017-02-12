using Microsoft.CodeAnalysis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core
{
    public class ReferenceEndpoint
    {
        public ReferenceEndpoint(ReferenceKind referenceKind, INamedTypeSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException("symbol");

            ReferenceKind = referenceKind;
            Symbol = symbol;
        }

        public ReferenceKind ReferenceKind { get; private set; }
        public INamedTypeSymbol Symbol { get; private set; }
        public IEnumerable<ReferenceDefinition> References => ReferenceList;

        public void AddReference(ReferenceDefinition referenceDefinition)
        {
            if (referenceDefinition == null)
                throw new ArgumentNullException("referenceDefinition");

            if (References.Contains(referenceDefinition))
                throw new ArgumentException("Reference definition is already added");

            ReferenceList.Add(referenceDefinition);
        }

        public override string ToString()
        {
            return String.Format("{0} [{1}] {2} references", ReferenceKind, Symbol, References.Count());
        }

        private readonly BlockingCollection<ReferenceDefinition> ReferenceList = new BlockingCollection<ReferenceDefinition>();
    }
}
