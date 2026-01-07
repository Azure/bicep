// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.Net;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;
using Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Environment = Bicep.Core.Utils.Environment;
using LocalFileSystem = System.IO.Abstractions.FileSystem;

namespace Microsoft.Extensions.DependencyInjection;

public static class BicepCoreServiceCollectionExtensions
{
    public static IServiceCollection AddBicepCore(this IServiceCollection services)
    {
        services.TryAddSingleton<INamespaceProvider, NamespaceProvider>();
        services.TryAddSingleton<IResourceTypeProviderFactory, ResourceTypeProviderFactory>();
        services.TryAddSingleton<IContainerRegistryClientFactory, ContainerRegistryClientFactory>();
        services.TryAddSingleton<ITemplateSpecRepositoryFactory, TemplateSpecRepositoryFactory>();
        services.TryAddSingleton<IArmClientProvider, ArmClientProvider>();
        services.TryAddSingleton<IModuleDispatcher, ModuleDispatcher>();
        services.TryAddSingleton<IArtifactRegistryProvider, DefaultArtifactRegistryProvider>();
        services.TryAddSingleton<ITokenCredentialFactory, TokenCredentialFactory>();
        services.TryAddSingleton<IEnvironment, Environment>();
        services.TryAddSingleton<IFileSystem, LocalFileSystem>();
        services.TryAddSingleton<IFileExplorer, FileSystemFileExplorer>();
        services.TryAddSingleton<IAuxiliaryFileCache, AuxiliaryFileCache>();
        services.TryAddSingleton<IConfigurationManager, ConfigurationManager>();
        services.TryAddSingleton<IBicepAnalyzer, LinterAnalyzer>();
        services.TryAddSingleton<IFeatureProviderFactory, FeatureProviderFactory>();
        services.TryAddSingleton<ILinterRulesProvider, LinterRulesProvider>();
        services.TryAddSingleton<ISourceFileFactory, SourceFileFactory>();
        services.AddBicepRegistryCatalogServices();
        services.TryAddSingleton<BicepCompiler>();

        return services;
    }

    public static IServiceCollection AddBicepRegistryCatalogServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IPublicModuleMetadataProvider, PublicModuleMetadataProvider>();
        services.TryAddSingleton<IRegistryModuleCatalog, RegistryModuleCatalog>();
        services.TryAddSingleton<IPrivateAcrModuleMetadataProviderFactory, PrivateAcrModuleMetadataProviderFactory>();

        // using type based registration for Http clients so dependencies can be injected automatically
        // without manually constructing up the graph, see https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#typed-clients
        services
            .AddHttpClient<IPublicModuleIndexHttpClient, PublicModuleMetadataHttpClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

        return services;
    }
}
