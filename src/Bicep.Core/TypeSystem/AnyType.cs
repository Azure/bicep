using System;
using Bicep.Core.SemanticModel;

namespace Bicep.Core.TypeSystem
{
    public class AnyType : TypeSymbol
    {
        public AnyType()
            : base("any")
        {
        }

        public override TypeKind TypeKind => TypeKind.Any;
    }
}