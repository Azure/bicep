using System;
using Bicep.Core.SemanticModel;

namespace Bicep.Core.TypeSystem
{
    public abstract class TypeSymbol : Symbol, IEquatable<TypeSymbol>
    {
        protected TypeSymbol(string name) : base(name)
        {
        }

        public override SymbolKind Kind => SymbolKind.Type;

        public abstract TypeKind TypeKind { get; }

        public abstract bool Equals(TypeSymbol other);

        public static bool Equals(TypeSymbol? first, TypeSymbol? second)
        {
            if (first != null && second != null)
            {
                return first.Equals(second);
            }

            if (first == null && second == null)
            {
                // null equals null
                return true;
            }

            return false;
        }
    }
}