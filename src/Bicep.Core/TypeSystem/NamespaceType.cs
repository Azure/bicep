// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public sealed class NamespaceType : ObjectType
    {
        public NamespaceType(string name, IEnumerable<TypeProperty> properties, FunctionResolver methodResolver, DecoratorResolver decoratorResolver)
            : base(name)
        {
            this.Properties = properties.ToImmutableDictionary(property => property.Name, LanguageConstants.IdentifierComparer);
            this.MethodResolver = methodResolver;
            this.DecoratorResolver = decoratorResolver;
        }

        public override TypeKind TypeKind => TypeKind.Namespace;

        public override TypeSymbolValidationFlags ValidationFlags => TypeSymbolValidationFlags.PreventAssignment;

        public override ImmutableDictionary<string, TypeProperty> Properties { get; }

        public override ITypeReference? AdditionalPropertiesType => null;

        public override FunctionResolver MethodResolver { get; }

        public DecoratorResolver DecoratorResolver { get; }
    }
}