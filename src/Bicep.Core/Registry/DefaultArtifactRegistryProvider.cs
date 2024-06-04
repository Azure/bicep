// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry
{
    public class DefaultArtifactRegistryProvider : IArtifactRegistryProvider
    {
        private readonly IFileResolver fileResolver;
        private readonly IFileSystem fileSystem;
        private readonly IContainerRegistryClientFactory clientFactory;
        private readonly ITemplateSpecRepositoryFactory templateSpecRepositoryFactory;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IConfigurationManager configurationManager;
        private readonly IServiceProvider serviceProvider;

        public DefaultArtifactRegistryProvider(IServiceProvider serviceProvider, IFileResolver fileResolver, IFileSystem fileSystem, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory, IFeatureProviderFactory featureProviderFactory, IConfigurationManager configurationManager)
        {
            this.fileResolver = fileResolver;
            this.fileSystem = fileSystem;
            this.clientFactory = clientFactory;
            this.templateSpecRepositoryFactory = templateSpecRepositoryFactory;
            this.featureProviderFactory = featureProviderFactory;
            this.configurationManager = configurationManager;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the registries available for module references inside a given template URI.
        /// </summary>
        /// <param name="templateUri">URI of the Bicep template source code which contains the module references.
        /// This is needed to determine the appropriate bicepconfig.json (which contains module alias definitions) and features provider to bind to</param>
        /// <returns></returns>
        public ImmutableArray<IArtifactRegistry> Registries(Uri templateUri)
        {
            var configuration = configurationManager.GetConfiguration(templateUri);
            var features = featureProviderFactory.GetFeatureProvider(templateUri);
            var builder = ImmutableArray.CreateBuilder<IArtifactRegistry>();

            // Using IServiceProvider instead of constructor injection due to a dependency cycle
            builder.Add(new LocalModuleRegistry(fileResolver, fileSystem, features, templateUri));
            builder.Add(new OciArtifactRegistry(this.fileResolver, this.fileSystem, this.clientFactory, features, configuration, templateUri));
            builder.Add(new TemplateSpecModuleRegistry(this.fileResolver, this.fileSystem, this.templateSpecRepositoryFactory, features, configuration, templateUri));

            return builder.ToImmutableArray();
        }
    }
}
