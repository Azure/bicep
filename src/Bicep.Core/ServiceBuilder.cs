// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bicep.Core;

public interface IBicepService
{
    SourceFileGrouping BuildSourceFileGrouping(Uri entryFileUri, bool forceModulesRestore = false);

    Compilation BuildCompilation(SourceFileGrouping sourceFileGrouping, ImmutableDictionary<ISourceFile, ISemanticModel>? modelLookup = null);
}

public class ServiceBuilder
{
    private readonly IServiceCollection services;

    public ServiceBuilder(IServiceCollection? services = null)
    {
        if (services is null)
        {
            services = new ServiceCollection();
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
        services.AddBicepCore();
        services.TryAddSingleton<IWorkspace, Workspace>();

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
    }
}
