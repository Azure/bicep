// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public class UnionType : TypeSymbol
    {
        public UnionType(string name, ImmutableArray<ITypeReference> members, string? discriminatorPropertyName = null)
            : base(name)
        {
            this.Members = members;
            this.DiscriminatorPropertyName = discriminatorPropertyName;
        }

        public override TypeKind TypeKind => this.Members.IsEmpty ? TypeKind.Never : TypeKind.Union;

        public ImmutableArray<ITypeReference> Members { get; }

        public string? DiscriminatorPropertyName { get; }

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

        public static UnionType GetModifiedUnionType(UnionType inputType, string? discriminatorPropertyName) => new(inputType.Name, inputType.Members, discriminatorPropertyName);
    }
}
