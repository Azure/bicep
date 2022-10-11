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
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBicepCore(this IServiceCollection services)
        => services
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
            .AddSingleton<IFeatureProviderFactory, FeatureProviderFactory>()
            .AddSingleton<ILinterRulesProvider, LinterRulesProvider>();
}
