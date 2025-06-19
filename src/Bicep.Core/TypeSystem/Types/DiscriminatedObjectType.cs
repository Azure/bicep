// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem.Types
{
    public class DiscriminatedObjectType : ObjectLikeType
    {
        public DiscriminatedObjectType(string name, TypeSymbolValidationFlags validationFlags, string discriminatorKey, IEnumerable<ITypeReference> unionMembers)
            : base(name, validationFlags)
        {
            var unionMembersByKey = new Dictionary<string, ObjectType>();
            var unionKeyTypes = new List<StringLiteralType>();

            // start with required and we will aggregate in everything else
            var discriminatorPropertyFlags = TypePropertyFlags.Required;

            foreach (var member in unionMembers)
            {
                if (member.Type is not ObjectType objectType)
                {
                    throw new ArgumentException($"Invalid member of type {member.Type.GetType()}");
                }

                if (!objectType.Properties.TryGetValue(discriminatorKey, out var discriminatorProp))
                {
                    throw new ArgumentException("Missing discriminator field on member");
                }

                discriminatorPropertyFlags |= discriminatorProp.Flags;

                if (discriminatorProp.TypeReference.Type is not StringLiteralType stringLiteral)
                {
                    throw new ArgumentException($"Invalid discriminator field type {discriminatorProp.TypeReference.Type.Name} on member");
                }

                unionMembersByKey.Add(stringLiteral.Name, objectType);
                unionKeyTypes.Add(stringLiteral);
            }

            // NOTE(kylealbert): keys are bicep string literals (ex: "'a'" and not "a")
            UnionMembersByKey = unionMembersByKey.ToImmutableDictionary();
            DiscriminatorKeysUnionType = TypeHelper.CreateTypeUnion(unionKeyTypes);
            DiscriminatorProperty = new NamedTypeProperty(discriminatorKey, DiscriminatorKeysUnionType, discriminatorPropertyFlags);
        }

        private DiscriminatedObjectType(DiscriminatedObjectType other, ImmutableDictionary<string, ObjectType> unionMembersByKey)
            : base(other.Name, other.ValidationFlags)
        {
            DiscriminatorProperty = other.DiscriminatorProperty;
            DiscriminatorKeysUnionType = other.DiscriminatorKeysUnionType;
            UnionMembersByKey = unionMembersByKey;
        }

        public override TypeKind TypeKind => TypeKind.DiscriminatedObject;

        public override string FormatNameForCompoundTypes() => Name.IndexOf(' ') > -1 ? WrapTypeName() : Name;

        public ImmutableDictionary<string, ObjectType> UnionMembersByKey { get; }

        public NamedTypeProperty DiscriminatorProperty { get; }

        public string DiscriminatorKey => DiscriminatorProperty.Name;

        public TypeSymbol DiscriminatorKeysUnionType { get; }

        /// <summary>
        /// Returns the discriminator property if the given property key matches the discriminator key.
        /// </summary>
        public TypeProperty? TryGetDiscriminatorProperty(string? propertyKey)
        {
            if (propertyKey is null)
            {
                return null;
            }

            if (LanguageConstants.IdentifierComparer.Equals(propertyKey, DiscriminatorKey))
            {
                return DiscriminatorProperty;
            }

            return null;
        }

        public DiscriminatedObjectType WithModifiedMembers(Func<ObjectType, ObjectType> modifyMemberFn) =>
            new(this, UnionMembersByKey.ToImmutableDictionary(kvp => kvp.Key, kvp => modifyMemberFn(kvp.Value)));
    }
}
