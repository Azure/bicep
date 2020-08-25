// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem
{
    public class NamedObjectType : ObjectType
    {
        public NamedObjectType(string name, IEnumerable<TypeProperty> properties, TypeSymbol? additionalPropertiesType, TypePropertyFlags additionalPropertiesFlags = TypePropertyFlags.None)
            : base(name)
        {
            this.Properties = properties.ToImmutableDictionary(property => property.Name, property => property, LanguageConstants.IdentifierComparer);
            this.AdditionalPropertiesType = additionalPropertiesType;
            this.AdditionalPropertiesFlags = additionalPropertiesFlags;
        }

        public override TypeKind TypeKind => TypeKind.NamedObject;

        public override ImmutableDictionary<string, TypeProperty> Properties { get; }

        public override TypeSymbol? AdditionalPropertiesType { get; }

        public override TypePropertyFlags AdditionalPropertiesFlags { get; }
    }
}
