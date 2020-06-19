using System;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Represents a built-in primitive type.
    /// </summary>
    internal class PrimitiveType : TypeSymbol
    {
        public PrimitiveType(string name) : base(name)
        {
        }

        public override TypeKind TypeKind => TypeKind.SimpleType;

        public override bool Equals(TypeSymbol other)
        {
            // for primitive types, type is equality is based on name of the type
            return string.Equals(this.Name, other.Name, StringComparison.Ordinal);
        }
    }
}