// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Oci;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bicep.Core.Registry
{
    public class DefaultArtifactRegistryProvider : ArtifactRegistryProvider
    {
        public DefaultArtifactRegistryProvider(
            IOciRegistryTransportFactory transportFactory,
            ITemplateSpecRepositoryFactory templateSpecRepositoryFactory,
            IPublicModuleMetadataProvider publicModuleMetadataProvider,
            ILogger<OciArtifactRegistry>? logger = null)
            : base(new IArtifactRegistry[]
                {
                    new LocalModuleRegistry(),
                    new OciArtifactRegistry(
                        transportFactory,
                        publicModuleMetadataProvider,
                        logger ?? NullLogger<OciArtifactRegistry>.Instance),
                    new TemplateSpecModuleRegistry(templateSpecRepositoryFactory),
                })
        {
        }
    }
}
