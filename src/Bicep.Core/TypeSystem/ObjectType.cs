using System;
using System.Collections.Immutable;
using Bicep.Core.SemanticModel;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Represents an object with any property of any type.
    /// </summary>
    public class ObjectType : TypeSymbol
    {
        public ObjectType(string name) : base(name)
        {
        }

        public override TypeKind TypeKind => TypeKind.Object;

        public virtual ImmutableDictionary<string, TypeProperty> Properties => ImmutableDictionary<string, TypeProperty>.Empty;

        public virtual TypeSymbol? AdditionalPropertiesType => LanguageConstants.Any;
    }
}