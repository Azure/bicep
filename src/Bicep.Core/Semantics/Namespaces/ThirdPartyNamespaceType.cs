// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
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

        public static NamespaceType Create(string name, string aliasName, IResourceTypeProvider resourceTypeProvider)
        {
            var namespaceSettings = new NamespaceSettings(
                    IsSingleton: true,
                    BicepProviderName: name,
                    ConfigurationType: null,
                    ArmTemplateProviderName: name,
                    ArmTemplateProviderVersion: resourceTypeProvider.Version);

            if (resourceTypeProvider is ThirdPartyResourceTypeProvider thirdPartyProvider)
            {
                var namespaceConfig = thirdPartyProvider.GetNamespaceConfiguration();

                if (namespaceConfig is not null)
                {
                    namespaceSettings = new NamespaceSettings(
                        IsSingleton: namespaceConfig.IsSingleton,
                        BicepProviderName: namespaceConfig.Name,
                        ConfigurationType: (ObjectType?)namespaceConfig.ConfigurationObject,
                        ArmTemplateProviderName: namespaceConfig.Name,
                        ArmTemplateProviderVersion: namespaceConfig.Version);

                }
            }

            return new NamespaceType(
                aliasName,
                namespaceSettings,
                ImmutableArray<TypeProperty>.Empty,
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                resourceTypeProvider);
        }
    }
}
