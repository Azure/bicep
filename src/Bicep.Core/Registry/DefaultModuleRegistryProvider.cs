// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using System;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry
{
    public class DefaultModuleRegistryProvider : IModuleRegistryProvider
    {
        private readonly IFileResolver fileResolver;
        private readonly IContainerRegistryClientFactory clientFactory;
        private readonly ITemplateSpecRepositoryFactory templateSpecRepositoryFactory;
        private readonly IFeatureProviderFactory featureProviderFactory;
        private readonly IConfigurationManager configurationManager;
        private readonly IServiceProvider serviceProvider;

        public DefaultModuleRegistryProvider(IServiceProvider serviceProvider, IFileResolver fileResolver, IContainerRegistryClientFactory clientFactory, ITemplateSpecRepositoryFactory templateSpecRepositoryFactory, IFeatureProviderFactory featureProviderFactory, IConfigurationManager configurationManager)
        {
            this.fileResolver = fileResolver;
            this.clientFactory = clientFactory;
            this.templateSpecRepositoryFactory = templateSpecRepositoryFactory;
            this.featureProviderFactory = featureProviderFactory;
            this.configurationManager = configurationManager;
            this.serviceProvider = serviceProvider;
        }

        public ImmutableArray<IModuleRegistry> Registries(Uri templateUri)
        {
            var configuration = configurationManager.GetConfiguration(templateUri);
            var features = featureProviderFactory.GetFeatureProvider(templateUri);
            var builder = ImmutableArray.CreateBuilder<IModuleRegistry>();

            // Using IServiceProvider instead of constructor injection due to a dependency cycle
            var compiler = this.serviceProvider.GetRequiredService<BicepCompiler>();
            builder.Add(new LocalModuleRegistry(compiler, this.fileResolver, templateUri));

            if (features.RegistryEnabled)
            {
                builder.Add(new OciModuleRegistry(this.fileResolver, this.clientFactory, features, configuration, templateUri));
                builder.Add(new TemplateSpecModuleRegistry(this.fileResolver, this.templateSpecRepositoryFactory, features, configuration, templateUri));
            }

            return builder.ToImmutableArray();
        }
    }
}
