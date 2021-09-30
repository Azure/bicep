// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Semantics;

namespace Bicep.Core.TypeSystem
{
    public record NamespaceSettings(
        bool IsSingleton,
        string BicepProviderName,
        ITypeReference? ConfigurationType,
        string ArmTemplateProviderName,
        string ArmTemplateProviderVersion);

    public sealed class NamespaceType : ObjectType
    {
        public NamespaceType(
            string aliasName,
            NamespaceSettings settings,
            IEnumerable<TypeProperty> properties,
            IEnumerable<FunctionOverload> functionOverloads,
            IEnumerable<BannedFunction> bannedFunctions,
            IEnumerable<Decorator> decorators,
            IResourceTypeProvider resourceTypeProvider)
            : base(aliasName, TypeSymbolValidationFlags.PreventAssignment, properties, null, TypePropertyFlags.None, obj => new FunctionResolver(obj, functionOverloads, bannedFunctions))
        {
            Settings = settings;
            ResourceTypeProvider = resourceTypeProvider;
            DecoratorResolver = new DecoratorResolver(this, decorators);
        }

        public override TypeKind TypeKind => TypeKind.Namespace;

        public DecoratorResolver DecoratorResolver { get; }

        public NamespaceSettings Settings { get; }

        public IResourceTypeProvider ResourceTypeProvider { get; }

        public string ProviderName => Settings.BicepProviderName;

        public ITypeReference? ConfigurationType => Settings.ConfigurationType;
    }
}
