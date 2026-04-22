// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Catalog;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry
{
    public class DefaultArtifactRegistryProvider : ArtifactRegistryProvider
    {
        public DefaultArtifactRegistryProvider(IFileExplorer fileExplorer, IServiceProvider serviceProvider, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory)
            : base(new IArtifactRegistry[]
                {
                    new LocalModuleRegistry(fileExplorer),
                    new OciArtifactRegistry(fileExplorer, clientFactory, serviceProvider.GetRequiredService<IPublicModuleMetadataProvider>()),
                    new TemplateSpecModuleRegistry(fileExplorer, templateSpecRepositoryFactory),
                })
        {
        }
    }
}
