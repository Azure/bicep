// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Catalog;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry
{
    public class DefaultArtifactRegistryProvider : ArtifactRegistryProvider
    {
        public DefaultArtifactRegistryProvider(IServiceProvider serviceProvider, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory)
            : base(new IArtifactRegistry[]
                {
                    new LocalModuleRegistry(),
                    new OciArtifactRegistry(clientFactory, serviceProvider.GetRequiredService<IPublicModuleMetadataProvider>()),
                    new TemplateSpecModuleRegistry(templateSpecRepositoryFactory),
                })
        {
        }
    }
}
