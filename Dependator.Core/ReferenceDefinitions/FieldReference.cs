using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core.ReferenceDefinitions
{
    public sealed class FieldReference : ReferenceDefinition
    {
        public FieldReference(ReferenceEndpoint referenceFrom, ReferenceEndpoint referenceTo) : 
            base(ReferenceTypeEnum.FieldType, referenceFrom, referenceTo)
        { }
    }
}
