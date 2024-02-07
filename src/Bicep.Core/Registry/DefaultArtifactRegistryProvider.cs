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
    public class DefaultArtifactRegistryProvider(IServiceProvider serviceProvider, IFileResolver fileResolver, IFileSystem fileSystem, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory, IFeatureProviderFactory featureProviderFactory, IConfigurationManager configurationManager) : IArtifactRegistryProvider
    {
        private readonly IFileResolver fileResolver = fileResolver;
        private readonly IFileSystem fileSystem = fileSystem;
        private readonly IContainerRegistryClientFactory clientFactory = clientFactory;
        private readonly ITemplateSpecRepositoryFactory templateSpecRepositoryFactory = templateSpecRepositoryFactory;
        private readonly IFeatureProviderFactory featureProviderFactory = featureProviderFactory;
        private readonly IConfigurationManager configurationManager = configurationManager;
        private readonly IServiceProvider serviceProvider = serviceProvider;

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
            var compiler = this.serviceProvider.GetService<BicepCompiler>();
            builder.Add(new LocalModuleRegistry(this.fileResolver, templateUri, compiler));
            builder.Add(new OciArtifactRegistry(this.fileResolver, this.fileSystem, this.clientFactory, features, configuration, templateUri));
            builder.Add(new TemplateSpecModuleRegistry(this.fileResolver, this.fileSystem, this.templateSpecRepositoryFactory, features, configuration, templateUri));

            return builder.ToImmutableArray();
        }
    }
}
