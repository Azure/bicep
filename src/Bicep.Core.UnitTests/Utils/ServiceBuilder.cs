// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Linq;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.DependencyInjection;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.UnitTests.Utils
{
    public static class ServiceBuilderExtensions
    {
        private static ServiceBuilder Register<TService>(ServiceBuilder serviceBuilder, TService service)
            where TService : class
            => serviceBuilder.WithRegistration(x => x.AddSingleton<TService>(service));

        public static ServiceBuilder WithFileResolver(this ServiceBuilder serviceBuilder, IFileResolver fileResolver)
            => Register(serviceBuilder, fileResolver);

        public static ServiceBuilder WithWorkspace(this ServiceBuilder serviceBuilder, IWorkspace workspace)
            => Register(serviceBuilder, workspace);

        public static ServiceBuilder WithFeatureProviderFactory(this ServiceBuilder serviceBuilder, IFeatureProviderFactory featureProviderFactory)
            => Register(serviceBuilder, featureProviderFactory);

        public static ServiceBuilder WithNamespaceProvider(this ServiceBuilder serviceBuilder, INamespaceProvider namespaceProvider)
            => Register(serviceBuilder, namespaceProvider);

        public static ServiceBuilder WithConfigurationManager(this ServiceBuilder serviceBuilder, IConfigurationManager configurationManager)
            => Register(serviceBuilder, configurationManager);

        public static ServiceBuilder WithApiVersionProviderFactory(this ServiceBuilder serviceBuilder, IApiVersionProviderFactory apiVersionProviderFactory)
            => Register(serviceBuilder, apiVersionProviderFactory);

        public static ServiceBuilder WithBicepAnalyzer(this ServiceBuilder serviceBuilder, IBicepAnalyzer bicepAnalyzer)
            => Register(serviceBuilder, bicepAnalyzer);

        public static ServiceBuilder WithTestDefaults(this ServiceBuilder serviceBuilder)
            => serviceBuilder
                .WithRegistration(service => service
                    .AddSingleton<INamespaceProvider, DefaultNamespaceProvider>()
                    .AddSingleton<IAzResourceTypeLoader, AzResourceTypeLoader>()
                    .AddSingleton<IContainerRegistryClientFactory, ContainerRegistryClientFactory>()
                    .AddSingleton<ITemplateSpecRepositoryFactory, TemplateSpecRepositoryFactory>()
                    .AddSingleton<IModuleDispatcher, ModuleDispatcher>()
                    .AddSingleton<IModuleRegistryProvider, DefaultModuleRegistryProvider>()
                    .AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>()
                    .AddSingleton<IFileResolver, FileResolver>()
                    .AddSingleton<IConfigurationManager, ConfigurationManager>()
                    .AddSingleton<IApiVersionProviderFactory, ApiVersionProviderFactory>()
                    .AddSingleton<IBicepAnalyzer, LinterAnalyzer>()
                    .AddSingleton<IFileSystem, IOFileSystem>()
                    .AddSingleton<IWorkspace, Workspace>())
                .WithFeatureProviderFactory(BicepTestConstants.FeatureProviderFactory);

        public static ServiceBuilder WithAzResources(this ServiceBuilder serviceBuilder, IEnumerable<ResourceTypeComponents> resourceTypes)
            => serviceBuilder.WithRegistration(x => x.AddSingleton<IAzResourceTypeLoader>(TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(resourceTypes)));

        public static ServiceBuilder WithEmptyAzResources(this ServiceBuilder serviceBuilder)
            => serviceBuilder.WithAzResources(Enumerable.Empty<ResourceTypeComponents>());

        public static ServiceBuilder WithWorkspaceFiles(this ServiceBuilder serviceBuilder, IReadOnlyDictionary<Uri, string> fileContentsByUri)
        {
            var workspace = new Workspace();
            var sourceFiles = fileContentsByUri.Select(kvp => SourceFileFactory.CreateSourceFile(kvp.Key, kvp.Value));
            workspace.UpsertSourceFiles(sourceFiles);

            return serviceBuilder.WithWorkspace(workspace);
        }
    }

    public class ServiceBuilder
    {
        private readonly IServiceCollection services;

        public ServiceBuilder()
        {
            services = new ServiceCollection();
        }

        public ServiceBuilder WithRegistration(Action<IServiceCollection> registerAction)
        {
            registerAction(services);

            return this;
        }

        public CompilationBuilderInternal Compilation => new(services.BuildServiceProvider());

        public SourceFileGroupingBuilderInternal SourceFileGrouping => new(services.BuildServiceProvider());

        public class CompilationBuilderInternal
        {
            private readonly IServiceProvider provider;

            public CompilationBuilderInternal(IServiceProvider provider)
            {
                this.provider = provider;
            }

            public Compilation Build(SourceFileGrouping sourceFileGrouping, ImmutableDictionary<ISourceFile, ISemanticModel>? modelLookup = null)
            {
                return new(
                    provider.GetRequiredService<IFeatureProviderFactory>(),
                    provider.GetRequiredService<INamespaceProvider>(),
                    sourceFileGrouping,
                    provider.GetRequiredService<IConfigurationManager>(),
                    provider.GetRequiredService<IApiVersionProviderFactory>(),
                    provider.GetRequiredService<IBicepAnalyzer>(),
                    modelLookup);
            }
        }

        public class SourceFileGroupingBuilderInternal
        {
            private readonly IServiceProvider provider;

            public SourceFileGroupingBuilderInternal(IServiceProvider provider)
            {
                this.provider = provider;
            }

            public SourceFileGrouping Build(Uri entryFileUri, bool forceModulesRestore = false)
            {
                return SourceFileGroupingBuilder.Build(
                    provider.GetRequiredService<IFileResolver>(),
                    provider.GetRequiredService<IModuleDispatcher>(),
                    provider.GetRequiredService<IWorkspace>(),
                    entryFileUri,
                    forceModulesRestore);
            }

            public SourceFileGrouping Rebuild(SourceFileGrouping current)
            {
                return SourceFileGroupingBuilder.Rebuild(
                    provider.GetRequiredService<IModuleDispatcher>(),
                    provider.GetRequiredService<IWorkspace>(),
                    current);
            }
        }
    }
}
