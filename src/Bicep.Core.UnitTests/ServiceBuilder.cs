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
using Bicep.Decompiler;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.UnitTests;

public interface IDependencyHelper
{
    public TService Construct<TService>()
        where TService : class;
}

public static class IDependencyHelperExtensions
{
    public static Compilation BuildCompilation(this IDependencyHelper helper, SourceFileGrouping sourceFileGrouping, ImmutableDictionary<ISourceFile, ISemanticModel>? modelLookup = null)
        => new(
            helper.Construct<IFeatureProviderFactory>(),
            helper.Construct<INamespaceProvider>(),
            sourceFileGrouping,
            helper.Construct<IConfigurationManager>(),
            helper.Construct<IBicepAnalyzer>(),
            modelLookup);

    public static SourceFileGrouping BuildSourceFileGrouping(this IDependencyHelper helper, Uri entryFileUri, bool forceModulesRestore = false)
        => SourceFileGroupingBuilder.Build(
            helper.Construct<IFileResolver>(),
            helper.Construct<IModuleDispatcher>(),
            helper.Construct<IWorkspace>(),
            entryFileUri,
            forceModulesRestore);

    public static BicepCompiler GetCompiler(this IDependencyHelper helper)
        => helper.Construct<BicepCompiler>();

    public static BicepDecompiler GetDecompiler(this IDependencyHelper helper)
        => helper.Construct<BicepDecompiler>();
}

public class ServiceBuilder
{
    private readonly IServiceCollection services;

    public ServiceBuilder()
    {
        this.services = new ServiceCollection()
            .AddBicepCore()
            .AddBicepDecompiler()
            .WithWorkspace(new Workspace());
    }

    public static IDependencyHelper Create(Action<IServiceCollection>? registerAction = null)
    {
        registerAction ??= services => {};

        return new ServiceBuilder().WithRegistration(registerAction).Build();
    }

    public ServiceBuilder WithRegistration(Action<IServiceCollection> registerAction)
    {
        registerAction(services);

        return this;
    }

    public IDependencyHelper Build()
    {
        return new ServiceBuilderInternal(services.BuildServiceProvider());
    }

    private class ServiceBuilderInternal : IDependencyHelper
    {
        private readonly IServiceProvider provider;

        public ServiceBuilderInternal(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public TService Construct<TService>()
            where TService : class
        {
            return provider.GetRequiredService<TService>();
        }
    }
}
