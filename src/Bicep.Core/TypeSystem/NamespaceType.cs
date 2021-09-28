// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public sealed class NamespaceType : ObjectType
    {
        public NamespaceType(
            string aliasName,
            string providerName,
            IEnumerable<TypeProperty> properties,
            IEnumerable<FunctionOverload> functionOverloads,
            IEnumerable<BannedFunction> bannedFunctions,
            IEnumerable<Decorator> decorators,
            IResourceTypeProvider resourceTypeProvider,
            ITypeReference? configurationType,
            bool isSingleton)
            : base(aliasName, TypeSymbolValidationFlags.PreventAssignment, properties, null, TypePropertyFlags.None, obj => new FunctionResolver(obj, functionOverloads, bannedFunctions))
        {
            this.DecoratorResolver = new DecoratorResolver(this, decorators);
            ProviderName = providerName;
            ResourceTypeProvider = resourceTypeProvider;
            ConfigurationType = configurationType;
            IsSingleton = isSingleton;
        }

        public override TypeKind TypeKind => TypeKind.Namespace;

        public DecoratorResolver DecoratorResolver { get; }

        public string ProviderName { get; }

        public IResourceTypeProvider ResourceTypeProvider { get; }

        public ITypeReference? ConfigurationType { get; }

        public bool IsSingleton { get; }
    }
}
