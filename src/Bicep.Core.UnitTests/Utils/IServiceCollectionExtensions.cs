// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Configuration;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.UnitTests.Utils;

public static class IServiceCollectionExtensions
{
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
