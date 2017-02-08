using Microsoft.CodeAnalysis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core
{
    public class ReferenceModel
    {
        public IEnumerable<ReferenceEndpoint> ReferenceFrom => LazyReferenceFrom.Value.Values;
        public IEnumerable<ReferenceEndpoint> ReferenceTo => LazyReferenceTo.Value.Values;
        public IEnumerable<ReferenceDefinition> References => LazyReferenceDefinitionList.Value;

        public ReferenceEndpoint GetOrAddReferenceFrom(INamedTypeSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException("symbol");

            return LazyReferenceFrom.Value.GetOrAdd(symbol, s => new ReferenceEndpoint(ReferenceKind.ReferencesTo, s));
        }

        public ReferenceEndpoint GetOrAddReferenceTo(INamedTypeSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException("symbol");

            return LazyReferenceTo.Value.GetOrAdd(symbol, s => new ReferenceEndpoint(ReferenceKind.ReferencedBy, s));
        }


        public void AddReferenceDefinition(ReferenceDefinition referenceDefinition)
        {
            if (referenceDefinition == null)
                throw new ArgumentNullException("referenceDefinition");

            LazyReferenceDefinitionList.Value.Add(referenceDefinition);
        }

        public void CloneReferenceDefinition(ReferenceDefinition referenceDefinition)
        {
            if (referenceDefinition == null)
                throw new ArgumentNullException("referenceDefinition");

            var referenceFrom = GetOrAddReferenceFrom(referenceDefinition.ReferenceFrom.Symbol);
            var referenceTo = GetOrAddReferenceTo(referenceDefinition.ReferenceTo.Symbol);
            AddReferenceDefinition(referenceDefinition.Clone(referenceFrom, referenceTo));
        }

        private readonly Lazy<ConcurrentDictionary<INamedTypeSymbol, ReferenceEndpoint>> LazyReferenceFrom = new Lazy<ConcurrentDictionary<INamedTypeSymbol, ReferenceEndpoint>>(true);
        private readonly Lazy<ConcurrentDictionary<INamedTypeSymbol, ReferenceEndpoint>> LazyReferenceTo = new Lazy<ConcurrentDictionary<INamedTypeSymbol, ReferenceEndpoint>>(true);
        private readonly Lazy<BlockingCollection<ReferenceDefinition>> LazyReferenceDefinitionList = new Lazy<BlockingCollection<ReferenceDefinition>>(true);
    }
}
