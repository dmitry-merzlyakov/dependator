using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core.ReferenceDefinitions
{
    public sealed class BaseTypeReference : ReferenceDefinition
    {
        public BaseTypeReference(ReferenceEndpoint referenceFrom, ReferenceEndpoint referenceTo) : 
            base(ReferenceTypeEnum.BaseTypeInheritance, referenceFrom, referenceTo)
        { }

        public INamedTypeSymbol BaseType => ReferenceTo.Symbol;
        public INamedTypeSymbol DerivedType => ReferenceFrom.Symbol;
    }
}
