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

                //1. Do we have to do a null check
                //Do we have to return like this, is there a better way? 
                if(namespaceConfig != null)
                {
                    return new NamespaceType(
                    aliasName,
                    new NamespaceSettings(
                        IsSingleton: namespaceConfig.IsSingleton,
                        BicepProviderName: namespaceConfig.Name,
                        ConfigurationType: (ObjectType?)namespaceConfig.ConfigurationObject,
                        ArmTemplateProviderName: namespaceConfig.Name,
                        ArmTemplateProviderVersion: namespaceConfig.Version),
                    ImmutableArray<TypeProperty>.Empty,
                    ImmutableArray<FunctionOverload>.Empty,
                    ImmutableArray<BannedFunction>.Empty,
                    ImmutableArray<Decorator>.Empty,
                    resourceTypeProvider);
                }
            }

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
