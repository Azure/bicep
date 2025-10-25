// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO.Abstractions;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources.Mocking;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Configuration;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Mock.Registry;
using Bicep.Core.UnitTests.Mock.Registry.Catalog;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Providers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using LocalFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.UnitTests;

public static class IServiceCollectionExtensions
{
    private static IServiceCollection Register<TService>(IServiceCollection services, TService service)
        where TService : class
        => services.AddSingleton(service);

    public static IServiceCollection WithFileExplorer(this IServiceCollection services, IFileExplorer fileExplorer)
        => Register(services, fileExplorer);

    public static IServiceCollection WithFileSystem(this IServiceCollection services, IFileSystem fileSystem)
        => Register(services, fileSystem);

    public static IServiceCollection WithContainerRegistryClientFactory(this IServiceCollection services, IContainerRegistryClientFactory containerRegistryClientFactory)
        => Register(services, containerRegistryClientFactory);

    public static IServiceCollection WithTemplateSpecRepositoryFactory(this IServiceCollection services, ITemplateSpecRepositoryFactory factory)
        => Register(services, factory);

    public static IServiceCollection WithWorkspace(this IServiceCollection services, IActiveSourceFileSet workspace)
        => Register(services, workspace);

    public static IServiceCollection WithFeatureOverrides(this IServiceCollection services, FeatureProviderOverrides overrides)
        => Register(services, overrides)
            .AddSingleton<FeatureProviderFactory>()
            .AddSingleton<IFeatureProviderFactory, OverriddenFeatureProviderFactory>();

    public static IServiceCollection WithEnvironmentVariables(this IServiceCollection services, params (string key, string? value)[] variables)
        => WithEnvironment(services, TestEnvironment.Default.WithVariables(variables));

    public static IServiceCollection WithEnvironment(this IServiceCollection services, IEnvironment environment)
        => Register(services, environment);

    public static IServiceCollection WithNamespaceProvider(this IServiceCollection services, INamespaceProvider namespaceProvider)
        => Register(services, namespaceProvider);

    public static IServiceCollection WithFeatureProviderFactory(this IServiceCollection services, IFeatureProviderFactory featureProviderFactory)
        => Register(services, featureProviderFactory);

    public static IServiceCollection WithModuleDispatcher(this IServiceCollection services, IModuleDispatcher moduleDispatcher)
        => Register(services, moduleDispatcher);

    public static IServiceCollection WithCompilationManager(this IServiceCollection services, ICompilationManager compilationManager)
        => Register(services, compilationManager);

    public static IServiceCollection WithConfigurationManager(this IServiceCollection services, IConfigurationManager configurationManager)
        => Register(services, configurationManager);

    public static IServiceCollection WithConfigurationPatch(this IServiceCollection services, Func<RootConfiguration, RootConfiguration> patchFunc)
        => Register(services, patchFunc)
            .AddSingleton<ConfigurationManager>()
            .AddSingleton<IConfigurationManager, PatchingConfigurationManager>();

    public static IServiceCollection WithDisabledAnalyzersConfiguration(this IServiceCollection services)
        => services.WithConfigurationPatch(c => c.WithAllAnalyzersDisabled());

    public static IServiceCollection WithAnalyzersCodesToDisableConfiguration(this IServiceCollection services, params string[] analyzerCodesToDisable)
        => services.WithConfigurationPatch(c => c.WithAllAnalyzers().WithAnalyzersDisabled(analyzerCodesToDisable));

    public static IServiceCollection WithConfiguration(this IServiceCollection services, RootConfiguration configuration)
        => services.WithConfigurationPatch(c => configuration);

    public static IServiceCollection WithBicepAnalyzer(this IServiceCollection services, IBicepAnalyzer bicepAnalyzer)
        => Register(services, bicepAnalyzer);

    public static IServiceCollection WithAzResources(this IServiceCollection services, IEnumerable<ResourceTypeComponents> resourceTypes)
        => services.WithAzResourceTypeLoaderFactory(TestTypeHelper.CreateResourceTypeLoaderWithTypes(resourceTypes));

    public static IServiceCollection WithAzResourceTypeLoaderFactory(this IServiceCollection services, IResourceTypeLoader loader)
    {
        var provider = new AzResourceTypeProvider(loader);
        return Register(services, TestTypeHelper.CreateResourceTypeLoaderFactory(provider));
    }

    public static IServiceCollection WithAzResourceProvider(this IServiceCollection services, IAzResourceProvider azResourceProvider)
        => Register(services, azResourceProvider);

    public static IServiceCollection WithArmClientProvider(this IServiceCollection services, IArmClientProvider armClientProvider)
        => Register(services, armClientProvider);

    public static IServiceCollection WithDeploymentHelper(this IServiceCollection services, IDeploymentHelper deploymentHelper)
        => Register(services, deploymentHelper);

    public static IServiceCollection WithEmptyAzResources(this IServiceCollection services)
        => services.WithAzResources([]);

    public static IServiceCollection AddSingletonIfNotNull<TService>(this IServiceCollection services, TService? instance)
        where TService : class
    {
        if (instance is not null)
        {
            return services.AddSingleton(instance);
        }

        return services;
    }

    public static IServiceCollection AddMockHttpClient<TClient>(this IServiceCollection services, TClient? httpClient) where TClient : class
    {
        return AddMockHttpClientIfNotNull(services, httpClient);
    }

    public static IServiceCollection AddMockHttpClientIfNotNull<TClient>(IServiceCollection services, TClient? httpClient) where TClient : class
    {
        if (!typeof(TClient).IsInterface)
        {
            throw new ArgumentException($"TClient must be an interface type, found: {typeof(TClient).FullName}");
        }

        if (httpClient is { })
        {
            services.AddHttpClient(typeof(TClient).FullName!, httpClient =>
            {
            })
                .AddTypedClient(c => httpClient);
        }

        return services;
    }

    public static IServiceCollection AddMockArmClient(this IServiceCollection services, MockableResourcesArmClient armClient)
        => AddMockArmClient(services, _ => armClient);

    public static IServiceCollection AddMockArmClient(this IServiceCollection services, Func<RootConfiguration, MockableResourcesArmClient> armClient)
    {
        var clientProvider = StrictMock.Of<IArmClientProvider>();
        clientProvider.Setup(x => x.CreateArmClient(It.IsAny<RootConfiguration>(), It.IsAny<string?>()))
            .Returns<RootConfiguration, string?>((config, _) =>
            {
                var clientMock = StrictMock.Of<ArmClient>();

                clientMock.Setup(x => x.GetCachedClient(It.IsAny<Func<ArmClient, MockableResourcesArmClient>>()))
                    .Returns(armClient(config));

                return clientMock.Object;
            });

        return services.AddSingleton(clientProvider.Object);
    }
}
