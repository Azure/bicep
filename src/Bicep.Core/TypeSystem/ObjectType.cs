// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Represents an object with any property of any type.
    /// </summary>
    public class ObjectType : TypeSymbol
    {
        public ObjectType(string name) : base(name)
        {
            AdditionalProperties = LanguageConstants.Any;
        }

        public override TypeKind TypeKind => TypeKind.Primitive;

        public virtual ImmutableDictionary<string, TypeProperty> Properties => ImmutableDictionary<string, TypeProperty>.Empty;

        public virtual ITypeReference? AdditionalProperties { get; }

        public virtual TypePropertyFlags AdditionalPropertiesFlags => TypePropertyFlags.None;
    }
}
