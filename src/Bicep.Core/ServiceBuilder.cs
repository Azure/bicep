// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.IO.Abstractions;
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
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.DependencyInjection;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core
{
    public class ServiceBuilder
    {
        private readonly IServiceCollection services;

        public ServiceBuilder()
        {
            services = new ServiceCollection()
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
                .AddSingleton<IWorkspace, Workspace>()
                .AddSingleton<IFeatureProviderFactory, FeatureProviderFactory>();
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
