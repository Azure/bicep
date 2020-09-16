// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public class DiscriminatedObjectType : TypeSymbol
    {
        public DiscriminatedObjectType(string name, string discriminatorKey, IEnumerable<NamedObjectType> unionMembers)
            : base(name)
        {
            var unionMembersByKey = new Dictionary<string, NamedObjectType>();
            var unionKeyTypes = new List<StringLiteralType>();
            foreach (var member in unionMembers)
            {
                if (!member.Properties.TryGetValue(discriminatorKey, out var discriminatorProp))
                {
                    throw new ArgumentException("Missing discriminator field on member");
                }

                if (!(discriminatorProp.Type is StringLiteralType stringLiteral))
                {
                    throw new ArgumentException($"Invalid discriminator field type {discriminatorProp.Type.Name} on member");
                }

                unionMembersByKey.Add(stringLiteral.Name, member);
                unionKeyTypes.Add(stringLiteral);
            }

            this.UnionMembersByKey = unionMembersByKey.ToImmutableDictionary();
            this.DiscriminatorKey = discriminatorKey;
            this.DiscriminatorKeysUnionType = UnionType.Create(unionKeyTypes);
        }

        public override TypeKind TypeKind => TypeKind.DiscriminatedObject;

        public ImmutableDictionary<string, NamedObjectType> UnionMembersByKey { get; }

        public string DiscriminatorKey { get; }

        public TypeSymbol DiscriminatorKeysUnionType { get; }
    }
}
