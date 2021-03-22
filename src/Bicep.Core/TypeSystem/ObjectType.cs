// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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
            : base(name)
        {
            this.ValidationFlags = validationFlags;
            this.Properties = properties.ToImmutableDictionary(property => property.Name, LanguageConstants.IdentifierComparer);
            this.MethodResolver = new FunctionResolver(this, functions);
            this.AdditionalPropertiesType = additionalPropertiesType;
            this.AdditionalPropertiesFlags = additionalPropertiesFlags;
        }

        public override TypeKind TypeKind => TypeKind.Object;

        public override TypeSymbolValidationFlags ValidationFlags { get; }

        public ImmutableDictionary<string, TypeProperty> Properties { get; }

        public FunctionResolver MethodResolver { get; }

        public ITypeReference? AdditionalPropertiesType { get; }

        public TypePropertyFlags AdditionalPropertiesFlags { get; }
    }
}
