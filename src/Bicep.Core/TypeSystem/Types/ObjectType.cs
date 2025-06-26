// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem.Types
{
    /// <summary>
    /// Represents an object with any property of any type.
    /// </summary>
    public class ObjectType : ObjectLikeType
    {
        public ObjectType(string name, TypeSymbolValidationFlags validationFlags, IEnumerable<NamedTypeProperty> properties, TypeProperty? additionalProperties = null, IEnumerable<FunctionOverload>? functions = null)
            : this(name, validationFlags, properties, additionalProperties, owner => new FunctionResolver(owner, functions ?? ImmutableArray<FunctionOverload>.Empty))
        {
        }

        public ObjectType(string name, TypeSymbolValidationFlags validationFlags, IEnumerable<NamedTypeProperty> properties, TypeProperty? additionalProperties, Func<ObjectType, FunctionResolver> methodResolverBuilder)
            : base(name, validationFlags)
        {
            Properties = properties.ToImmutableSortedDictionary(property => property.Name, property => property, LanguageConstants.IdentifierComparer);
            MethodResolver = methodResolverBuilder(this);
            AdditionalProperties = additionalProperties;
        }

        public override TypeKind TypeKind => TypeKind.Object;

        public ImmutableSortedDictionary<string, NamedTypeProperty> Properties { get; }

        public TypeProperty? AdditionalProperties { get; }

        public bool HasExplicitAdditionalPropertiesType =>
            AdditionalProperties is { } additionalProperties && !additionalProperties.Flags.HasFlag(TypePropertyFlags.FallbackProperty);

        public FunctionResolver MethodResolver { get; }

        public ObjectType With(
            TypeSymbolValidationFlags? validationFlags = null,
            IEnumerable<NamedTypeProperty>? properties = null,
            TypeProperty? additionalProperties = null,
            Func<ObjectType, FunctionResolver>? methodResolverBuilder = null) => new(
                Name,
                validationFlags ?? ValidationFlags,
                properties ?? Properties.Values,
                additionalProperties ?? AdditionalProperties,
                methodResolverBuilder ?? MethodResolver.CopyToObject);
    }
}
