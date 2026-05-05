// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Registry.Catalog;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry
{
    public class DefaultArtifactRegistryProvider : ArtifactRegistryProvider
    {
        public DefaultArtifactRegistryProvider(RegistryConfiguration registryConfiguration, IPublicModuleMetadataProvider publicModuleMetadataProvider, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory)
            : base(new IArtifactRegistry[]
                {
                    new LocalModuleRegistry(),
                    new OciArtifactRegistry(registryConfiguration, clientFactory, publicModuleMetadataProvider),
                    new TemplateSpecModuleRegistry(templateSpecRepositoryFactory),
                })
        {
        }
    }
}
