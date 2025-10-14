// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Auth;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Oci;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bicep.Core.Registry
{
    public class DefaultArtifactRegistryProvider : ArtifactRegistryProvider
    {
        public DefaultArtifactRegistryProvider(
            IServiceProvider serviceProvider,
            IOciRegistryTransportFactory transportFactory,
            ITemplateSpecRepositoryFactory templateSpecRepositoryFactory)
            : base(new IArtifactRegistry[]
                {
                    new LocalModuleRegistry(),
                    new OciArtifactRegistry(
                        transportFactory,
                        serviceProvider.GetRequiredService<IPublicModuleMetadataProvider>(),
                        serviceProvider.GetRequiredService<ICredentialChain>(),
                        serviceProvider.GetService<ILogger<OciArtifactRegistry>>() ?? NullLogger<OciArtifactRegistry>.Instance),
                    new TemplateSpecModuleRegistry(templateSpecRepositoryFactory),
                })
        {
        }
    }
}
