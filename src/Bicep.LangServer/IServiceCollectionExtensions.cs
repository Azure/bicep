// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO.Abstractions;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using Bicep.Decompiler;
using Microsoft.Extensions.DependencyInjection;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LanguageServer;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBicepCore(this IServiceCollection services) => services
        .AddSingleton<INamespaceProvider, DefaultNamespaceProvider>()
        .AddSingleton<IAzResourceTypeLoader, AzResourceTypeLoader>()
        .AddSingleton<IAzResourceTypeLoaderFactory, AzResourceTypeLoaderFactory>()
        .AddSingleton<IContainerRegistryClientFactory, ContainerRegistryClientFactory>()
        .AddSingleton<ITemplateSpecRepositoryFactory, TemplateSpecRepositoryFactory>()
        .AddSingleton<IArtifactDispatcher, ArtifactDispatcher>()
        .AddSingleton<IModuleRegistryProvider, DefaultModuleRegistryProvider>()
        .AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>()
        .AddSingleton<IFileResolver, FileResolver>()
        .AddSingleton<IFileSystem, IOFileSystem>()
        .AddSingleton<IConfigurationManager, ConfigurationManager>()
        .AddSingleton<IBicepAnalyzer, LinterAnalyzer>()
        .AddSingleton<IFeatureProviderFactory, FeatureProviderFactory>()
        .AddSingleton<ILinterRulesProvider, LinterRulesProvider>()
        .AddSingleton<BicepCompiler>();

    public static IServiceCollection AddBicepDecompiler(this IServiceCollection services) => services
        .AddSingleton<BicepDecompiler>();
    
    public static IServiceCollection AddBicepparamDecompiler(this IServiceCollection services) => services
        .AddSingleton<BicepparamDecompiler>();
}
