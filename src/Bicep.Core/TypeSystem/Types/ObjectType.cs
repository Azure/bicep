// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem.Types
{
    /// <summary>
    /// Represents an object with any property of any type.
    /// </summary>
    public class ObjectType : TypeSymbol
    {
        public ObjectType(string name, TypeSymbolValidationFlags validationFlags, IEnumerable<TypeProperty> properties, ITypeReference? additionalPropertiesType, TypePropertyFlags additionalPropertiesFlags = TypePropertyFlags.None, IEnumerable<FunctionOverload>? functions = null)
            : this(name, validationFlags, properties, additionalPropertiesType, additionalPropertiesFlags, owner => new FunctionResolver(owner, functions ?? ImmutableArray<FunctionOverload>.Empty))
        {
        }

        public ObjectType(string name, TypeSymbolValidationFlags validationFlags, IEnumerable<TypeProperty> properties, ITypeReference? additionalPropertiesType, TypePropertyFlags additionalPropertiesFlags, Func<ObjectType, FunctionResolver> methodResolverBuilder)
            : base(name)
        {
            ValidationFlags = validationFlags;
            Properties = properties.ToImmutableSortedDictionary(property => property.Name, property => property, LanguageConstants.IdentifierComparer);
            MethodResolver = methodResolverBuilder(this);
            AdditionalPropertiesType = additionalPropertiesType;
            AdditionalPropertiesFlags = additionalPropertiesFlags;
        }

        public override TypeKind TypeKind => TypeKind.Object;

        public override TypeSymbolValidationFlags ValidationFlags { get; }

        public ImmutableSortedDictionary<string, TypeProperty> Properties { get; }

        public ITypeReference? AdditionalPropertiesType { get; }

        public TypePropertyFlags AdditionalPropertiesFlags { get; }

        public bool HasExplicitAdditionalPropertiesType =>
            AdditionalPropertiesType != null && !AdditionalPropertiesFlags.HasFlag(TypePropertyFlags.FallbackProperty);

        public FunctionResolver MethodResolver { get; }

        public ObjectType With(
            TypeSymbolValidationFlags? validationFlags = null,
            IEnumerable<TypeProperty>? properties = null,
            Tuple<ITypeReference?>? additionalPropertiesType = null,
            TypePropertyFlags? additionalPropertiesFlags = null,
            Func<ObjectType, FunctionResolver>? methodResolverBuilder = null) => new(
                Name,
                validationFlags ?? ValidationFlags,
                properties ?? Properties.Values,
                additionalPropertiesType is not null ? additionalPropertiesType.Item1 : AdditionalPropertiesType,
                additionalPropertiesFlags ?? AdditionalPropertiesFlags,
                methodResolverBuilder ?? MethodResolver.CopyToObject);
    }
}
