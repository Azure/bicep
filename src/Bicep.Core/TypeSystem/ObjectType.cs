// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
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
            this.ValidationFlags = validationFlags;
            this.Properties = properties.ToImmutableSortedDictionary(property => property.Name, property => property, LanguageConstants.IdentifierComparer);
            this.MethodResolver = methodResolverBuilder(this);
            this.AdditionalPropertiesType = additionalPropertiesType;
            this.AdditionalPropertiesFlags = additionalPropertiesFlags;
        }

        public override TypeKind TypeKind => TypeKind.Object;

        public override TypeSymbolValidationFlags ValidationFlags { get; }

        public ImmutableSortedDictionary<string, TypeProperty> Properties { get; }

        public ITypeReference? AdditionalPropertiesType { get; }

        public TypePropertyFlags AdditionalPropertiesFlags { get; }

        public FunctionResolver MethodResolver { get; }
    }
}
