// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class ThirdPartyNamespaceType
    {
        public static NamespaceSettings Settings { get; } = new(
            IsSingleton: true,
            BicepProviderName: string.Empty,
            ConfigurationType: null,
            ArmTemplateProviderName: string.Empty,
            ArmTemplateProviderVersion: string.Empty);

        public static NamespaceType Create(string name, string aliasName, IResourceTypeProvider resourceTypeProvider)
        {
            return new NamespaceType(
                aliasName,
                new NamespaceSettings(
                    IsSingleton: true,
                    BicepProviderName: name,
                    ConfigurationType: null,
                    ArmTemplateProviderName: name,
                    ArmTemplateProviderVersion: resourceTypeProvider.Version),
                ImmutableArray<TypeProperty>.Empty,
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                resourceTypeProvider);
        }
    }
}
