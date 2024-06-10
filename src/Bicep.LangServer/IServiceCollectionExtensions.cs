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
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;
using Bicep.Decompiler;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Configuration;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Registry;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Snippets;
using Bicep.LanguageServer.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LanguageServer;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBicepCore(this IServiceCollection services) => services
        .AddSingleton<INamespaceProvider, NamespaceProvider>()
        .AddSingleton<IResourceTypeProviderFactory, ResourceTypeProviderFactory>()
        .AddSingleton<IContainerRegistryClientFactory, ContainerRegistryClientFactory>()
        .AddSingleton<ITemplateSpecRepositoryFactory, TemplateSpecRepositoryFactory>()
        .AddSingleton<IModuleDispatcher, ModuleDispatcher>()
        .AddSingleton<IArtifactRegistryProvider, DefaultArtifactRegistryProvider>()
        .AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>()
        .AddSingleton<IFileResolver, FileResolver>()
        .AddSingleton<IEnvironment, Core.Utils.Environment>()
        .AddSingleton<IFileSystem, IOFileSystem>()
        .AddSingleton<IConfigurationManager, ConfigurationManager>()
        .AddSingleton<IBicepAnalyzer, LinterAnalyzer>()
        .AddSingleton<IFeatureProviderFactory, FeatureProviderFactory>()
        .AddSingleton<ILinterRulesProvider, LinterRulesProvider>()
        .AddSingleton<BicepCompiler>();

    public static IServiceCollection AddBicepDecompiler(this IServiceCollection services) => services
        .AddSingleton<BicepDecompiler>();

    public static IServiceCollection AddServerDependencies(this IServiceCollection services) => services
        .AddBicepCore()
        .AddBicepDecompiler()
        .AddSingleton<IWorkspace, Workspace>()
        .AddSingleton<ISnippetsProvider, SnippetsProvider>()
        .AddSingleton<ITelemetryProvider, TelemetryProvider>()
        .AddSingleton<ICompilationManager, BicepCompilationManager>()
        .AddSingleton<ICompilationProvider, BicepCompilationProvider>()
        .AddSingleton<ISymbolResolver, BicepSymbolResolver>()
        .AddSingleton<ICompletionProvider, BicepCompletionProvider>()
        .AddSingleton<IModuleRestoreScheduler, ModuleRestoreScheduler>()
        .AddSingleton<IAzResourceProvider, AzResourceProvider>()
        .AddSingleton<IBicepConfigChangeHandler, BicepConfigChangeHandler>()
        .AddSingleton<IDeploymentCollectionProvider, DeploymentCollectionProvider>()
        .AddSingleton<IDeploymentOperationsCache, DeploymentOperationsCache>()
        .AddSingleton<IDeploymentFileCompilationCache, DeploymentFileCompilationCache>()
        .AddSingleton<IClientCapabilitiesProvider, ClientCapabilitiesProvider>()
        .AddSingleton<IModuleReferenceCompletionProvider, ModuleReferenceCompletionProvider>()
        .AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>()
        .AddSingleton<IArmClientProvider, ArmClientProvider>()
        .AddSingleton<IDeploymentHelper, DeploymentHelper>()
        .AddSingleton<ISettingsProvider, SettingsProvider>()
        .AddSingleton<IAzureContainerRegistriesProvider, AzureContainerRegistriesProvider>()
        .AddSingleton<IPublicRegistryModuleMetadataProvider, PublicRegistryModuleMetadataProvider>()
        ;
}
