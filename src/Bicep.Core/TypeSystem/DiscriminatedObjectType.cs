// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem
{
    public class DiscriminatedObjectType : TypeSymbol
    {
        public DiscriminatedObjectType(string name, TypeSymbolValidationFlags validationFlags, string discriminatorKey, IEnumerable<ITypeReference> unionMembers)
            : base(name)
        {
            var unionMembersByKey = new Dictionary<string, ITypeReference>();
            var unionKeyTypes = new List<StringLiteralType>();
            foreach (var member in unionMembers)
            {
                if (!(member.Type is NamedObjectType namedObject))
                {
                    throw new ArgumentException($"Invalid member of type {member.Type.GetType()}");
                }

                if (!namedObject.Properties.TryGetValue(discriminatorKey, out var discriminatorProp))
                {
                    throw new ArgumentException("Missing discriminator field on member");
                }

                if (!(discriminatorProp.TypeReference.Type is StringLiteralType stringLiteral))
                {
                    throw new ArgumentException($"Invalid discriminator field type {discriminatorProp.TypeReference.Type.Name} on member");
                }

                unionMembersByKey.Add(stringLiteral.Name, member);
                unionKeyTypes.Add(stringLiteral);
            }

            this.UnionMembersByKey = unionMembersByKey.ToImmutableDictionary();
            this.ValidationFlags = validationFlags;
            this.DiscriminatorKeysUnionType = UnionType.Create(unionKeyTypes);
            this.DiscriminatorProperty = new TypeProperty(discriminatorKey, this.DiscriminatorKeysUnionType, TypePropertyFlags.Required);
        }

        public override TypeKind TypeKind => TypeKind.DiscriminatedObject;

        public ImmutableDictionary<string, ITypeReference> UnionMembersByKey { get; }

        public override TypeSymbolValidationFlags ValidationFlags { get; }

        public TypeProperty DiscriminatorProperty { get; }

        public string DiscriminatorKey => this.DiscriminatorProperty.Name;

        public TypeSymbol DiscriminatorKeysUnionType { get; }
    }
}
