using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependator.Core
{
    public enum ReferenceTypeEnum
    {
        BaseTypeInheritance,
        InterfaceImplementation,
        MethodGenericTypeArgument,
        MethodParameter,
        MethodResult,
        MethodBody,
        PropertyType,
        FieldType,
    }
}
