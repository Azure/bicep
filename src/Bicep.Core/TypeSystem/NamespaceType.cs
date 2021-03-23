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
            : base(name, TypeSymbolValidationFlags.PreventAssignment, properties, null, TypePropertyFlags.None, new FunctionResolver(functionOverloads, bannedFunctions))
        {
            this.DecoratorResolver = new DecoratorResolver(decorators);
        }

        public override TypeKind TypeKind => TypeKind.Namespace;

        public DecoratorResolver DecoratorResolver { get; }
    }
}