using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core.ReferenceDefinitions
{
    public sealed class InterfaceReference : ReferenceDefinition
    {
        public InterfaceReference(ReferenceEndpoint referenceFrom, ReferenceEndpoint referenceTo) 
            : base(ReferenceTypeEnum.InterfaceImplementation, referenceFrom, referenceTo)
        { }

        public INamedTypeSymbol InterfaceType => ReferenceTo.Symbol;
        public INamedTypeSymbol InterfaceImplementor => ReferenceFrom.Symbol;
    }
}
