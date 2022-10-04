// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.UnitTests.Utils
{
    public static class ServiceBuilderExtensions
    {
        private static ServiceBuilder Register<TService>(ServiceBuilder serviceBuilder, TService service)
            where TService : class
            => serviceBuilder.WithRegistration(x => x.AddSingleton<TService>(service));

        public static ServiceBuilder WithFileResolver(this ServiceBuilder serviceBuilder, IFileResolver fileResolver)
            => Register(serviceBuilder, fileResolver);

        public static ServiceBuilder WithModuleDispatcher(this ServiceBuilder serviceBuilder, IModuleDispatcher moduleDispatcher)
            => Register(serviceBuilder, moduleDispatcher);

        public static ServiceBuilder WithWorkspace(this ServiceBuilder serviceBuilder, IWorkspace workspace)
            => Register(serviceBuilder, workspace);

        public static ServiceBuilder WithFeatureProvider(this ServiceBuilder serviceBuilder, IFeatureProvider featureProvider)
            => Register(serviceBuilder, featureProvider);

        public static ServiceBuilder WithNamespaceProvider(this ServiceBuilder serviceBuilder, INamespaceProvider namespaceProvider)
            => Register(serviceBuilder, namespaceProvider);

        public static ServiceBuilder WithConfigurationManager(this ServiceBuilder serviceBuilder, IConfigurationManager configurationManager)
            => Register(serviceBuilder, configurationManager);

        public static ServiceBuilder WithApiVersionProvider(this ServiceBuilder serviceBuilder, IApiVersionProvider apiVersionProvider)
            => Register(serviceBuilder, apiVersionProvider);

        public static ServiceBuilder WithBicepAnalyzer(this ServiceBuilder serviceBuilder, IBicepAnalyzer bicepAnalyzer)
            => Register(serviceBuilder, bicepAnalyzer);

        public static ServiceBuilder WithTestDefaults(this ServiceBuilder serviceBuilder)
            => serviceBuilder
                .WithFileResolver(BicepTestConstants.FileResolver)
                .WithModuleDispatcher(BicepTestConstants.ModuleDispatcher)
                .WithWorkspace(new Workspace())
                .WithFeatureProvider(BicepTestConstants.Features)
                .WithRegistration(x => x.AddSingleton<INamespaceProvider, DefaultNamespaceProvider>())
                .WithRegistration(x => x.AddSingleton<IAzResourceTypeLoader, AzResourceTypeLoader>())
                .WithConfigurationManager(BicepTestConstants.ConfigurationManager)
                .WithApiVersionProvider(BicepTestConstants.ApiVersionProvider)
                .WithBicepAnalyzer(BicepTestConstants.LinterAnalyzer);

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
                    provider.GetRequiredService<IFeatureProvider>(),
                    provider.GetRequiredService<INamespaceProvider>(),
                    sourceFileGrouping,
                    provider.GetRequiredService<IConfigurationManager>(),
                    provider.GetRequiredService<IApiVersionProvider>(),
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
