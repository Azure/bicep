using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem
{
    public class NamedObjectType : ObjectType
    {
        public NamedObjectType(string name, IEnumerable<TypeProperty> properties, TypeSymbol? additionalPropertiesType)
            : base(name)
        {
            this.Properties = properties.ToImmutableDictionary(property => property.Name, property => property, StringComparer.Ordinal);
            this.AdditionalPropertiesType = additionalPropertiesType;
        }

        public override ImmutableDictionary<string, TypeProperty> Properties { get; }

        public override TypeSymbol? AdditionalPropertiesType { get; }
    }
}