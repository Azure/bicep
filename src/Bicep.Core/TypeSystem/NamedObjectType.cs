// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public class NamedObjectType : ObjectType
    {
        public NamedObjectType(string name, TypeSymbolValidationFlags validationFlags, IEnumerable<TypeProperty> properties, ITypeReference? additionalPropertiesType, TypePropertyFlags additionalPropertiesFlags = TypePropertyFlags.None, IEnumerable<FunctionOverload>? functions = null)
            : base(name)
        {
            this.ValidationFlags = validationFlags;
            this.Properties = properties.ToImmutableDictionary(property => property.Name, LanguageConstants.IdentifierComparer);
            this.MethodResolver = new FunctionResolver(this, functions);
            this.AdditionalPropertiesType = additionalPropertiesType;
            this.AdditionalPropertiesFlags = additionalPropertiesFlags;
        }

        public override TypeKind TypeKind => TypeKind.NamedObject;

        public override TypeSymbolValidationFlags ValidationFlags { get; }

        public override ImmutableDictionary<string, TypeProperty> Properties { get; }

        public override FunctionResolver MethodResolver { get; }

        public override ITypeReference? AdditionalPropertiesType { get; }

        public override TypePropertyFlags AdditionalPropertiesFlags { get; }
    }
}
