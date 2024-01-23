// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem.Types
{
    public class UnionType : TypeSymbol
    {
        public UnionType(string name, ImmutableArray<ITypeReference> members)
            : base(name)
        {
            Members = members;
        }

        public override TypeKind TypeKind => Members.IsEmpty ? TypeKind.Never : TypeKind.Union;

        public ImmutableArray<ITypeReference> Members { get; }

        public override string FormatNameForCompoundTypes() => TypeKind == TypeKind.Never ? Name : WrapTypeName();

        public override bool Equals(object? other) => other is UnionType otherUnion && Members.SequenceEqual(otherUnion.Members);

        public override int GetHashCode()
        {
            HashCode hashCode = new();

            hashCode.Add(Name);
            hashCode.Add(TypeKind);
            foreach (var member in Members)
            {
                hashCode.Add(member);
            }

            return hashCode.ToHashCode();
        }
    }
}
