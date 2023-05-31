// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
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
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Configuration;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;
using Bicep.LanguageServer;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Providers;
using Microsoft.Extensions.DependencyInjection;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.UnitTests;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBicepCore(this IServiceCollection services) => services
        .AddSingleton<INamespaceProvider, DefaultNamespaceProvider>()
        .AddSingleton<IAzResourceTypeLoader, AzResourceTypeLoader>()
        .AddSingleton<IContainerRegistryClientFactory, ContainerRegistryClientFactory>()
        .AddSingleton<ITemplateSpecRepositoryFactory, TemplateSpecRepositoryFactory>()
        .AddSingleton<IModuleDispatcher, ModuleDispatcher>()
        .AddSingleton<IModuleRegistryProvider, DefaultModuleRegistryProvider>()
        .AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>()
        .AddSingleton<IFileResolver, FileResolver>()
        .AddSingleton<IFileSystem, IOFileSystem>()
        .AddSingleton<IConfigurationManager, ConfigurationManager>()
        .AddSingleton<IApiVersionProviderFactory, ApiVersionProviderFactory>()
        .AddSingleton<IBicepAnalyzer, LinterAnalyzer>()
        .AddSingleton<IFeatureProviderFactory, FeatureProviderFactory>()
        .AddSingleton<ILinterRulesProvider, LinterRulesProvider>()
        .AddSingleton<BicepCompiler>();

    public static IServiceCollection AddBicepDecompiler(this IServiceCollection services) => services
        .AddSingleton<BicepDecompiler>();

    private static IServiceCollection Register<TService>(IServiceCollection services, TService service)
        where TService : class
        => services.AddSingleton<TService>(service);

    public static IServiceCollection WithFileResolver(this IServiceCollection services, IFileResolver fileResolver)
        => Register(services, fileResolver);

    public static IServiceCollection WithWorkspace(this IServiceCollection services, IWorkspace workspace)
        => Register(services, workspace);

    public static IServiceCollection WithFeatureOverrides(this IServiceCollection services, FeatureProviderOverrides overrides)
        => Register(services, overrides)
            .AddSingleton<FeatureProviderFactory>()
            .AddSingleton<IFeatureProviderFactory, OverriddenFeatureProviderFactory>();

    public static IServiceCollection WithNamespaceProvider(this IServiceCollection services, INamespaceProvider namespaceProvider)
        => Register(services, namespaceProvider);

    public static IServiceCollection WithFeatureProviderFactory(this IServiceCollection services, IFeatureProviderFactory featureProviderFactory)
        => Register(services, featureProviderFactory);

    public static IServiceCollection WithModuleDispatcher(this IServiceCollection services, IModuleDispatcher moduleDispatcher)
        => Register(services, moduleDispatcher);

    public static IServiceCollection WithCompilationManager(this IServiceCollection services, ICompilationManager compilationManager)
        => Register(services, compilationManager);

    public static IServiceCollection WithConfigurationPatch(this IServiceCollection services, Func<RootConfiguration, RootConfiguration> patchFunc)
        => Register(services, patchFunc)
            .AddSingleton<ConfigurationManager>()
            .AddSingleton<IConfigurationManager, PatchingConfigurationManager>();

    public static IServiceCollection WithDisabledAnalyzersConfiguration(this IServiceCollection services)
        => services.WithConfigurationPatch(c => c.WithAllAnalyzersDisabled());

    public static IServiceCollection WithApiVersionProviderFactory(this IServiceCollection services, IApiVersionProviderFactory apiVersionProviderFactory)
        => Register(services, apiVersionProviderFactory);

    public static IServiceCollection WithApiVersionProvider(this IServiceCollection services, IApiVersionProvider apiVersionProvider)
        => services.WithApiVersionProviderFactory(IApiVersionProviderFactory.WithStaticApiVersionProvider(apiVersionProvider));

    public static IServiceCollection WithBicepAnalyzer(this IServiceCollection services, IBicepAnalyzer bicepAnalyzer)
        => Register(services, bicepAnalyzer);

    public static IServiceCollection WithAzResources(this IServiceCollection services, IEnumerable<ResourceTypeComponents> resourceTypes)
        => services.WithAzResourceTypeLoader(TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(resourceTypes));

    public static IServiceCollection WithAzResourceTypeLoader(this IServiceCollection services, IAzResourceTypeLoader azResourceTypeLoader)
        => Register(services, azResourceTypeLoader);

    public static IServiceCollection WithAzResourceProvider(this IServiceCollection services, IAzResourceProvider azResourceProvider)
        => Register(services, azResourceProvider);

    public static IServiceCollection WithArmClientProvider(this IServiceCollection services, IArmClientProvider armClientProvider)
        => Register(services, armClientProvider);

    public static IServiceCollection WithEmptyAzResources(this IServiceCollection services)
        => services.WithAzResources(Enumerable.Empty<ResourceTypeComponents>());

    public static IServiceCollection WithWorkspaceFiles(this IServiceCollection services, IReadOnlyDictionary<Uri, string> fileContentsByUri)
    {
        var workspace = new Workspace();
        var sourceFiles = fileContentsByUri.Select(kvp => SourceFileFactory.CreateSourceFile(kvp.Key, kvp.Value));
        workspace.UpsertSourceFiles(sourceFiles);

        return services.WithWorkspace(workspace);
    }

    public static IServiceCollection AddSingletonIfNonNull<TService>(this IServiceCollection services, TService? instance)
        where TService : class
    {
        if (instance is not null)
        {
            return services.AddSingleton(instance);
        }

        return services;
    }
}
