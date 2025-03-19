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

            if (members.Length > 0)
            {
                var flagsHeldByAll = ~TypeSymbolValidationFlags.Default;
                var taintFlags = TypeSymbolValidationFlags.Default;
                for (int i = 0; i < members.Length; i++)
                {
                    flagsHeldByAll &= members[i].Type.ValidationFlags;
                    taintFlags |= (members[i].Type.ValidationFlags & LanguageConstants.TaintFlags);
                }

                ValidationFlags = flagsHeldByAll | taintFlags;
            }
            else
            {
                ValidationFlags = TypeSymbolValidationFlags.Default;
            }
        }

        public override TypeKind TypeKind => Members.IsEmpty ? TypeKind.Never : TypeKind.Union;

        public override TypeSymbolValidationFlags ValidationFlags { get; }

        public ImmutableArray<ITypeReference> Members { get; }

        public override string FormatNameForCompoundTypes() => TypeKind == TypeKind.Never ? Name : WrapTypeName();

        public override bool Equals(object? other) => other is UnionType otherUnion &&
            otherUnion.ValidationFlags == ValidationFlags &&
            Members.SequenceEqual(otherUnion.Members);

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
