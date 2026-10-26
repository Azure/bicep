// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Oci;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bicep.Core.Registry
{
    public class DefaultArtifactRegistryProvider : ArtifactRegistryProvider
    {
        public DefaultArtifactRegistryProvider(
            RegistryConfiguration registryConfiguration,
            IOciRegistryTransportFactory transportFactory,
            IPublicModuleMetadataProvider publicModuleMetadataProvider,
            ITemplateSpecRepositoryFactory templateSpecRepositoryFactory,
            IFileExplorer fileExplorer,
            ILogger<OciArtifactRegistry>? logger = null)
            : base(new IArtifactRegistry[]
                {
                    new LocalModuleRegistry(),
                    new OciArtifactRegistry(
                        registryConfiguration,
                        transportFactory,
                        publicModuleMetadataProvider,
                        fileExplorer,
                        logger ?? NullLogger<OciArtifactRegistry>.Instance),
                    new OciArtifactMockedRegistry(),
                    new TemplateSpecModuleRegistry(templateSpecRepositoryFactory),
                })
        {
        }
    }
}
