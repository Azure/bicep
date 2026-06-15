// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Registry.Catalog;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry
{
    public class DefaultArtifactRegistryProvider : ArtifactRegistryProvider
    {
        public DefaultArtifactRegistryProvider(RegistryConfiguration registryConfiguration, IPublicModuleMetadataProvider publicModuleMetadataProvider, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory, IFileExplorer fileExplorer)
            : base(new IArtifactRegistry[]
                {
                    new LocalModuleRegistry(),
                    new OciArtifactRegistry(registryConfiguration, clientFactory, publicModuleMetadataProvider, fileExplorer),
                    new OciArtifactMockedRegistry(),
                    new TemplateSpecModuleRegistry(templateSpecRepositoryFactory),
                })
        {
        }
    }
}
