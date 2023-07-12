// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public class UnionType : TypeSymbol
    {
        public UnionType(string name, ImmutableArray<ITypeReference> members, TypeProperty? discriminatorProperty = null)
            : base(name)
        {
            this.Members = members;
            this.DiscriminatorProperty = discriminatorProperty;
        }

        public override TypeKind TypeKind => this.Members.IsEmpty ? TypeKind.Never : TypeKind.Union;

        public ImmutableArray<ITypeReference> Members { get; }

        public TypeProperty? DiscriminatorProperty { get; }

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

        public static UnionType GetModifiedUnionType(UnionType inputType, TypeProperty? discriminatorProperty) => new(inputType.Name, inputType.Members, discriminatorProperty);
    }
}
