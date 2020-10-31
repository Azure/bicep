// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem
{
    public sealed class NamespaceType : ObjectType
    {
        public NamespaceType(string name, IEnumerable<TypeProperty> properties, FunctionResolver methodResolver)
            : base(name)
        {
            this.Properties = properties.ToImmutableDictionary(property => property.Name, LanguageConstants.IdentifierComparer);
            this.MethodResolver = methodResolver;
        }

        public override TypeKind TypeKind => TypeKind.Namespace;

        public override TypeSymbolValidationFlags ValidationFlags => TypeSymbolValidationFlags.PreventAssignment;

        public override ImmutableDictionary<string, TypeProperty> Properties { get; }

        public override ITypeReference? AdditionalPropertiesType => null;

        public override FunctionResolver MethodResolver { get; }
    }
}