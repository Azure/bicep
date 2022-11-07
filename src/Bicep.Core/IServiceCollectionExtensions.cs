// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO.Abstractions;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Workspaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBicepCore(this IServiceCollection services)
    {
        services.TryAddSingleton<INamespaceProvider, DefaultNamespaceProvider>();
        services.TryAddSingleton<IAzResourceTypeLoader, AzResourceTypeLoader>();
        services.TryAddSingleton<IContainerRegistryClientFactory, ContainerRegistryClientFactory>();
        services.TryAddSingleton<ITemplateSpecRepositoryFactory, TemplateSpecRepositoryFactory>();
        services.TryAddSingleton<IModuleDispatcher, ModuleDispatcher>();
        services.TryAddSingleton<IModuleRegistryProvider, DefaultModuleRegistryProvider>();
        services.TryAddSingleton<ITokenCredentialFactory, TokenCredentialFactory>();
        services.TryAddSingleton<IFileResolver, FileResolver>();
        services.TryAddSingleton<IConfigurationManager, ConfigurationManager>();
        services.TryAddSingleton<IApiVersionProviderFactory, ApiVersionProviderFactory>();
        services.TryAddSingleton<IBicepAnalyzer, LinterAnalyzer>();
        services.TryAddSingleton<IFileSystem, IOFileSystem>();
        services.TryAddSingleton<IFeatureProviderFactory, FeatureProviderFactory>();
        services.TryAddSingleton<ILinterRulesProvider, LinterRulesProvider>();
        services.TryAddSingleton<IBicepCompiler, BicepCompiler>();

        return services;
    }
}
