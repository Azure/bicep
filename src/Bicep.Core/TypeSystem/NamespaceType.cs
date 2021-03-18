// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public sealed class NamespaceType : ObjectType
    {
        public NamespaceType(string name, IEnumerable<TypeProperty> properties, IEnumerable<FunctionOverload> functionOverloads, IEnumerable<BannedFunction> bannedFunctions, IEnumerable<Decorator> decorators)
            : base(name)
        {
            this.Properties = properties.ToImmutableDictionary(property => property.Name, LanguageConstants.IdentifierComparer);
            this.MethodResolver = new FunctionResolver(this, functionOverloads, bannedFunctions);
            this.DecoratorResolver = new DecoratorResolver(this, decorators);
        }

        public override TypeKind TypeKind => TypeKind.Namespace;

        public override TypeSymbolValidationFlags ValidationFlags => TypeSymbolValidationFlags.PreventAssignment;

        public override ImmutableDictionary<string, TypeProperty> Properties { get; }

        public override ITypeReference? AdditionalPropertiesType => null;

        public override FunctionResolver MethodResolver { get; }

        public DecoratorResolver DecoratorResolver { get; }
    }
}