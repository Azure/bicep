// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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

    public static IServiceCollection WithFeatureProvider(this IServiceCollection services, IFeatureProvider featureProvider)
        => Register(services, IFeatureProviderFactory.WithStaticFeatureProvider(featureProvider));

    public static IServiceCollection WithFeatureProviderFactory(this IServiceCollection services, IFeatureProviderFactory featureProviderFactory)
        => Register(services, featureProviderFactory);

    public static IServiceCollection WithNamespaceProvider(this IServiceCollection services, INamespaceProvider namespaceProvider)
        => Register(services, namespaceProvider);

    public static IServiceCollection WithConfigurationManager(this IServiceCollection services, IConfigurationManager configurationManager)
        => Register(services, configurationManager);

    public static IServiceCollection WithConfiguration(this IServiceCollection services, RootConfiguration configuration)
        => Register(services, IConfigurationManager.WithStaticConfiguration(configuration));

    public static IServiceCollection WithDisabledAnalyzersConfiguration(this IServiceCollection services)
        => services.WithConfiguration(BicepTestConstants.BuiltInConfigurationWithAllAnalyzersDisabled);

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
}
