// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem
{
    public class NamedObjectType : ObjectType
    {
        public NamedObjectType(string name, IEnumerable<TypeProperty> properties, ITypeReference? additionalProperties, TypePropertyFlags additionalPropertiesFlags = TypePropertyFlags.None)
            : base(name)
        {
            this.Properties = properties.ToImmutableDictionary(property => property.Name, property => property, LanguageConstants.IdentifierComparer);
            this.AdditionalProperties = additionalProperties;
            this.AdditionalPropertiesFlags = additionalPropertiesFlags;
        }

        public override TypeKind TypeKind => TypeKind.NamedObject;

        public override ImmutableDictionary<string, TypeProperty> Properties { get; }

        public override ITypeReference? AdditionalProperties { get; }

        public override TypePropertyFlags AdditionalPropertiesFlags { get; }
    }
}
