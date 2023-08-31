// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using System.Collections.Generic;

namespace Bicep.Core.TypeSystem
{
    public record NamespaceSettings(
        bool IsSingleton,
        string BicepProviderName,
        ObjectType? ConfigurationType,
        string ArmTemplateProviderName,
        string ArmTemplateProviderVersion);

    public sealed class NamespaceType : ObjectType
    {
        public NamespaceType(
            string aliasName,
            NamespaceSettings settings,
            IEnumerable<TypeTypeProperty> properties,
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

        public ObjectType? ConfigurationType => Settings.ConfigurationType;

        public TypeTypeProperty? TryGetTypeProperty(string name) => Properties.TryGetValue(name, out var property) && property is TypeTypeProperty typeProperty
            ? typeProperty
            : null;
    }
}
