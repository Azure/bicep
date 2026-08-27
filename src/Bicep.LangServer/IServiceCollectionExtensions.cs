// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO.Abstractions;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Utils;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.LanguageServer.BicepConfig;
using Bicep.LanguageServer.ClientCapabilities;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Features.Custom.Deployments.Services;
using Bicep.LanguageServer.Features.Custom.InsertResource;
using Bicep.LanguageServer.Features.Custom.ModuleRestore;
using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Features.Language.Completion;
using Bicep.LanguageServer.Features.Language.Completion.Snippets;
using Bicep.LanguageServer.Features.Language.Definition;
using Bicep.LanguageServer.Options;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Utils;
using Bicep.Local.Deploy.Azure;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using LocalFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LanguageServer;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddServerDependencies(
        this IServiceCollection services,
        BicepLangServerOptions bicepLangServerOptions
    ) => services
        .AddBicepCore()
        .AddBicepDecompiler()
        .AddBicepLocalDeploy()
        .AddSingleton<IActiveSourceFileSet, ActiveSourceFileSet>()
        .AddSingleton<ISnippetsProvider, SnippetsProvider>()
        .AddSingleton<ICompilationManager, BicepCompilationManager>()
        .AddSingleton<ICompilationProvider, BicepCompilationProvider>()
        .AddSingleton<ISymbolResolver, BicepSymbolResolver>()
        .AddSingleton<ICompletionProvider, BicepCompletionProvider>()
        .AddSingleton<IModuleRestoreScheduler, ModuleRestoreScheduler>()
        .AddSingleton<IAzResourceProvider, AzResourceProvider>()
        .AddSingleton<IBicepConfigLifecycleManager, BicepConfigLifecycleManager>()
        .AddSingleton<IBicepConfigurationManager, BicepConfigurationManager>()
        .AddSingleton<IDeploymentCollectionProvider, DeploymentCollectionProvider>()
        .AddSingleton<IDeploymentOperationsCache, DeploymentOperationsCache>()
        .AddSingleton<IDeploymentFileCompilationCache, DeploymentFileCompilationCache>()
        .AddSingleton<IClientCapabilitiesProvider, ClientCapabilitiesProvider>()
        .AddSingleton<IModuleReferenceCompletionProvider, ModuleReferenceCompletionProvider>()
        .AddSingleton<IDeploymentHelper, DeploymentHelper>()
        .AddSingleton<ISettingsProvider, SettingsProvider>()
        .AddSingleton<IAzureContainerRegistriesProvider, AzureContainerRegistriesProvider>()
        .AddSingleton<IVisualGraphLayoutEngine, MsaglVisualGraphLayoutEngine>()
        .AddSingleton<IVisualResourceCreationService, VisualResourceCreationService>()
        .AddSingleton(bicepLangServerOptions)
        .AddSingleton<DocumentSelectorFactory>();
}
