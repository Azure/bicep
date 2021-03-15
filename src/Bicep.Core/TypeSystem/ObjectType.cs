// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Represents an object with any property of any type.
    /// </summary>
    public class ObjectType : TypeSymbol
    {
        public ObjectType(string name) : base(name)
        {
            AdditionalPropertiesType = LanguageConstants.Any;
            MethodResolver = new FunctionResolver(this, null);
        }

        public override TypeKind TypeKind => TypeKind.Primitive;

        public virtual ImmutableDictionary<string, TypeProperty> Properties => ImmutableDictionary<string, TypeProperty>.Empty;

        public virtual FunctionResolver MethodResolver { get; }

        public virtual ITypeReference? AdditionalPropertiesType { get; }

        public virtual TypePropertyFlags AdditionalPropertiesFlags => TypePropertyFlags.None;
    }
}
