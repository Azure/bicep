// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry.PublicRegistry;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry
{
    public class DefaultArtifactRegistryProvider : ArtifactRegistryProvider
    {
        public DefaultArtifactRegistryProvider(IServiceProvider serviceProvider, IFileResolver fileResolver, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory)
            : base(new IArtifactRegistry[]
                {
                    new LocalModuleRegistry(fileResolver),
                    new OciArtifactRegistry(fileResolver, clientFactory, serviceProvider.GetRequiredService<IPublicModuleMetadataProvider>()),
                    new TemplateSpecModuleRegistry(fileResolver, templateSpecRepositoryFactory),
                })
        {
        }
    }
}
