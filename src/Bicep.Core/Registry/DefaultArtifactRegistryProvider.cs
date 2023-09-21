// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry
{
    public class DefaultArtifactRegistryProvider : IArtifactRegistryProvider
    {
        private readonly IFileResolver fileResolver;
        private readonly IContainerRegistryClientFactory clientFactory;
        private readonly ITemplateSpecRepositoryFactory templateSpecRepositoryFactory;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IConfigurationManager configurationManager;
        private readonly IServiceProvider serviceProvider;

        public DefaultArtifactRegistryProvider(IServiceProvider serviceProvider, IFileResolver fileResolver, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory, IFeatureProviderFactory featureProviderFactory, IConfigurationManager configurationManager)
        {
            this.fileResolver = fileResolver;
            this.clientFactory = clientFactory;
            this.templateSpecRepositoryFactory = templateSpecRepositoryFactory;
            this.featureProviderFactory = featureProviderFactory;
            this.configurationManager = configurationManager;
            this.serviceProvider = serviceProvider;
        }

        // NOTE(stephwe): The templateUri affects how module aliases are resolved (depending on whether the bicepconfig.json is located for the given template)
        public ImmutableArray<IArtifactRegistry> Registries(Uri templateUri)
        {
            var configuration = configurationManager.GetConfiguration(templateUri);
            var features = featureProviderFactory.GetFeatureProvider(templateUri);
            var builder = ImmutableArray.CreateBuilder<IArtifactRegistry>();

            // Using IServiceProvider instead of constructor injection due to a dependency cycle
            var compiler = this.serviceProvider.GetService<BicepCompiler>();
            builder.Add(new LocalModuleRegistry(this.fileResolver, templateUri, compiler));
            builder.Add(new OciModuleRegistry(this.fileResolver, this.clientFactory, features, configuration, templateUri));
            builder.Add(new TemplateSpecModuleRegistry(this.fileResolver, this.templateSpecRepositoryFactory, features, configuration, templateUri));

            return builder.ToImmutableArray();
        }
    }
}
