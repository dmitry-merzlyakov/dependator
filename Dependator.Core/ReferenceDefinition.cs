using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core
{
    public abstract class ReferenceDefinition
    {
        public ReferenceDefinition (ReferenceTypeEnum referenceType, ReferenceEndpoint referenceFrom, ReferenceEndpoint referenceTo)
        {
            if (referenceFrom == null)
                throw new ArgumentNullException("referenceFrom");
            if (referenceTo == null)
                throw new ArgumentNullException("referenceTo");

            if (referenceFrom.ReferenceKind != ReferenceKind.ReferencesTo)
                throw new ArgumentException("Incorrect reference kind for 'referenceFrom'");
            if (referenceTo.ReferenceKind != ReferenceKind.ReferencedBy)
                throw new ArgumentException("Incorrect reference kind for 'referenceTo'");

            ReferenceType = referenceType;
            ReferenceFrom = referenceFrom;
            ReferenceTo = referenceTo;

            ReferenceFrom.AddReference(this);
            ReferenceTo.AddReference(this);
        }

        public ReferenceTypeEnum ReferenceType { get; private set; }
        public ReferenceEndpoint ReferenceFrom { get; private set; }
        public ReferenceEndpoint ReferenceTo { get; private set; }

        public ReferenceDefinition Clone(ReferenceEndpoint referenceFrom, ReferenceEndpoint referenceTo)
        {
            var cloned = (ReferenceDefinition)MemberwiseClone();

            cloned.ReferenceFrom = referenceFrom;
            cloned.ReferenceTo = referenceTo;

            cloned.ReferenceFrom.AddReference(cloned);
            cloned.ReferenceTo.AddReference(cloned);

            return cloned;
        }

        public ReferenceEndpoint Inverse(ReferenceEndpoint endPoint)
        {
            if (ReferenceFrom == endPoint)
                return ReferenceTo;

            if (ReferenceTo == endPoint)
                return ReferenceFrom;

            throw new InvalidOperationException("Unknown endpoint");
        }
    }
}
