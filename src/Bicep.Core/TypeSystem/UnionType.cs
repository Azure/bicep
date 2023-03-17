// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public class UnionType : TypeSymbol
    {
        public UnionType(string name, ImmutableArray<ITypeReference> members)
            : base(name)
        {
            this.Members = members;
        }

        public override TypeKind TypeKind => this.Members.IsEmpty ? TypeKind.Never : TypeKind.Union;

        public ImmutableArray<ITypeReference> Members { get; }

        public override string FormatNameForCompoundTypes() => TypeKind == TypeKind.Never ? Name : WrapTypeName();

        public override bool Equals(object? other) => other is UnionType otherUnion && Members.SequenceEqual(otherUnion.Members);

        public override int GetHashCode()
        {
            int hashCode = TypeKind.GetHashCode();

            // follow the Java algorithm (hashCode(<set>) == the sum of the hashcodes of set members) so that hash code is not dependent on set identity or iteration order
            foreach (var member in Members)
            {
                hashCode = unchecked(hashCode + member.GetHashCode());
            }

            return hashCode;
        }
    }
}
