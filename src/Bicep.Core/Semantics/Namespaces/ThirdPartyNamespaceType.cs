// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Registry.Oci;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.ThirdParty;
using Bicep.Core.TypeSystem.Types;
using static Bicep.Core.TypeSystem.Providers.ThirdParty.ThirdPartyResourceTypeLoader;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class ThirdPartyNamespaceType
    {
        public static NamespaceType Create(string name, string aliasName, IResourceTypeProvider resourceTypeProvider, OciArtifactReference? artifact)
        {
            // NamespaceConfig is not null
            if (resourceTypeProvider is ThirdPartyResourceTypeProvider thirdPartyProvider && thirdPartyProvider.GetNamespaceConfiguration() is NamespaceConfiguration namespaceConfig && namespaceConfig != null)
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
                    resourceTypeProvider,
                    artifact);
            }

            // NamespaceConfig is required to be set for 3PProviders
            throw new ArgumentException($"Please provide the following required Settings properties: Name, Version, & IsSingleton.");

        }
    }
}
