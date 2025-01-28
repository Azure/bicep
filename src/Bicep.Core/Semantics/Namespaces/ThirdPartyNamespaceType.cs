// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Registry;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.ThirdParty;
using Bicep.Core.TypeSystem.Types;
using static Bicep.Core.TypeSystem.Providers.ThirdParty.ThirdPartyResourceTypeLoader;

namespace Bicep.Core.Semantics.Namespaces
{
    public static class ThirdPartyNamespaceType
    {
        public static NamespaceType Create(string? aliasName, IResourceTypeProvider resourceTypeProvider, ArtifactReference? artifact)
        {
            if (resourceTypeProvider is ThirdPartyResourceTypeProvider thirdPartyProvider &&
                thirdPartyProvider.GetNamespaceConfiguration() is NamespaceConfiguration namespaceConfig)
            {
                return new NamespaceType(
                    aliasName ?? namespaceConfig.Name,
                    new NamespaceSettings(
                        IsSingleton: namespaceConfig.IsSingleton,
                        BicepExtensionName: namespaceConfig.Name,
                        ConfigurationType: namespaceConfig.ConfigurationObject,
                        TemplateExtensionName: namespaceConfig.Name,
                        TemplateExtensionVersion: namespaceConfig.Version),
                    ImmutableArray<NamedTypeProperty>.Empty,
                    ImmutableArray<FunctionOverload>.Empty,
                    ImmutableArray<BannedFunction>.Empty,
                    ImmutableArray<Decorator>.Empty,
                    resourceTypeProvider,
                    artifact);
            }

            throw new ArgumentException($"Please provide the following required Settings properties: Name, Version, & IsSingleton.");
        }
    }
}
