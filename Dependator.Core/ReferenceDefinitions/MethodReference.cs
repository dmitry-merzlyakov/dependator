using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core.ReferenceDefinitions
{
    public abstract class MethodReference : ReferenceDefinition
    {
        public MethodReference(ReferenceTypeEnum referenceType, ReferenceEndpoint referenceFrom, ReferenceEndpoint referenceTo, IMethodSymbol methodSymbol) 
            : base(referenceType, referenceFrom, referenceTo)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException("methodSymbol");

            MethodSymbol = methodSymbol;
        }

        public IMethodSymbol MethodSymbol { get; private set; }
    }

    public class MethodGenericReference : MethodReference
    {
        public MethodGenericReference(ReferenceEndpoint referenceFrom, ReferenceEndpoint referenceTo, IMethodSymbol methodSymbol) 
            : base(ReferenceTypeEnum.MethodGenericTypeArgument, referenceFrom, referenceTo, methodSymbol)
        { }
    }

    public class MethodParameterReference : MethodReference
    {
        public MethodParameterReference(ReferenceEndpoint referenceFrom, ReferenceEndpoint referenceTo, IMethodSymbol methodSymbol)
            : base(ReferenceTypeEnum.MethodParameter, referenceFrom, referenceTo, methodSymbol)
        { }
    }

    public class MethodResultReference : MethodReference
    {
        public MethodResultReference(ReferenceEndpoint referenceFrom, ReferenceEndpoint referenceTo, IMethodSymbol methodSymbol)
            : base(ReferenceTypeEnum.MethodResult, referenceFrom, referenceTo, methodSymbol)
        { }
    }

    public class MethodBodyReference : MethodReference
    {
        public MethodBodyReference(ReferenceEndpoint referenceFrom, ReferenceEndpoint referenceTo, IMethodSymbol methodSymbol)
            : base(ReferenceTypeEnum.MethodResult, referenceFrom, referenceTo, methodSymbol)
        { }
    }

}
