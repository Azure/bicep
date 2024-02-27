// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Azure.Bicep.Types.Index;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.ThirdParty;
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

        //We might not need name
        public static NamespaceType Create(string name, string aliasName, IResourceTypeProvider resourceTypeProvider)
        {
            var thirdPartyResourceTypeProvider = (ThirdPartyResourceTypeProvider)resourceTypeProvider;

            return new NamespaceType(
                aliasName,
                new NamespaceSettings(
                    IsSingleton: thirdPartyResourceTypeProvider.IsSingleton,
                    BicepProviderName: thirdPartyResourceTypeProvider.Name,
                    ConfigurationType: null,
                    ArmTemplateProviderName: thirdPartyResourceTypeProvider.Name,
                    ArmTemplateProviderVersion: resourceTypeProvider.Version),
                ImmutableArray<TypeProperty>.Empty,
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                resourceTypeProvider);
        }
    }
}
