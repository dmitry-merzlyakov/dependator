using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core.ReferenceDefinitions
{
    public sealed class PropertyReference : ReferenceDefinition
    {
        public PropertyReference(ReferenceEndpoint referenceFrom, ReferenceEndpoint referenceTo) : 
            base(ReferenceTypeEnum.PropertyType, referenceFrom, referenceTo)
        {   }
    }
}
