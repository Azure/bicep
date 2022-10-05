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

namespace Bicep.Core;

public interface IBicepService
{
    SourceFileGrouping BuildSourceFileGrouping(Uri entryFileUri, bool forceModulesRestore = false);

    SourceFileGrouping RebuildSourceFileGrouping(SourceFileGrouping current);

    Compilation BuildCompilation(SourceFileGrouping sourceFileGrouping, ImmutableDictionary<ISourceFile, ISemanticModel>? modelLookup = null);
}

public class ServiceBuilder
{
    private readonly IServiceCollection services;

    public ServiceBuilder(IServiceCollection? services = null)
    {
        if (services is null)
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

        this.services = services;
    }

    public ServiceBuilder WithRegistration(Action<IServiceCollection> registerAction)
    {
        registerAction(services);

        return this;
    }

    public IBicepService Build()
    {
        return new ServiceBuilderInternal(services.BuildServiceProvider());
    }

    private class ServiceBuilderInternal : IBicepService
    {
        private readonly IServiceProvider provider;

        public ServiceBuilderInternal(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public Compilation BuildCompilation(SourceFileGrouping sourceFileGrouping, ImmutableDictionary<ISourceFile, ISemanticModel>? modelLookup = null)
            => new(
                provider.GetRequiredService<IFeatureProviderFactory>(),
                provider.GetRequiredService<INamespaceProvider>(),
                sourceFileGrouping,
                provider.GetRequiredService<IConfigurationManager>(),
                provider.GetRequiredService<IApiVersionProviderFactory>(),
                provider.GetRequiredService<IBicepAnalyzer>(),
                modelLookup);

        public SourceFileGrouping BuildSourceFileGrouping(Uri entryFileUri, bool forceModulesRestore = false)
            => SourceFileGroupingBuilder.Build(
                provider.GetRequiredService<IFileResolver>(),
                provider.GetRequiredService<IModuleDispatcher>(),
                provider.GetRequiredService<IWorkspace>(),
                entryFileUri,
                forceModulesRestore);

        public SourceFileGrouping RebuildSourceFileGrouping(SourceFileGrouping current)
            => SourceFileGroupingBuilder.Rebuild(
                provider.GetRequiredService<IModuleDispatcher>(),
                provider.GetRequiredService<IWorkspace>(),
                current);
    }
}
