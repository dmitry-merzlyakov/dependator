using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core
{
    public class DependencyAnalyzer
    {
        public ReferenceModel SelectIndirectReferences(ReferenceModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var indirectReferences = model.References.Where(r => !IsDirectReference(r.ReferenceFrom.Symbol, r.ReferenceTo.Symbol));

            var result = new ReferenceModel();
            Parallel.ForEach(indirectReferences, reference => result.CloneReferenceDefinition(reference));

            return result;
        }

        public static bool IsDirectReference(INamedTypeSymbol from, INamedTypeSymbol to)
        {
            var nspaceFrom = from.ContainingNamespace.ToDisplayString();
            var nspaceTo = to.ContainingNamespace.ToDisplayString();

            return nspaceFrom.StartsWith(nspaceTo);
        }
    }
}
