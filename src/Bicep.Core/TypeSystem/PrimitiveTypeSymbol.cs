using System;
using Bicep.Core.SemanticModel;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Represents a built-in primitive type.
    /// </summary>
    public class PrimitiveTypeSymbol : TypeSymbol
    {
        public PrimitiveTypeSymbol(string name) : base(name)
        {
        }

        public override TypeKind TypeKind => TypeKind.SimpleType;

        public override bool Equals(TypeSymbol other)
        {
            // for primitive types, type is equality is based on name of the type
            return string.Equals(this.Name, other.Name, StringComparison.Ordinal);
        }

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitPrimitiveTypeSymbol(this);
        }
    }
}