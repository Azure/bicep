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
using Bicep.Core.Registry.Auth;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Oci.Oras;
using Bicep.Core.Registry.Providers;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Utils;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Configuration;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Options;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Registry;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Snippets;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using Bicep.Local.Deploy.Azure;
using Bicep.Local.Deploy.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using LocalFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LanguageServer;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBicepCore(this IServiceCollection services) => services
        .AddSingleton<INamespaceProvider, NamespaceProvider>()
        .AddSingleton<IResourceTypeProviderFactory, ResourceTypeProviderFactory>()
        .AddSingleton<IContainerRegistryClientFactory, ContainerRegistryClientFactory>()
        .AddSingleton<AzureContainerRegistryManager>()
        .AddSingleton<DockerCredentialProvider>()
        .AddSingleton<DockerCredentialSource>()
        .AddSingleton<ICredentialSource>(sp => sp.GetRequiredService<DockerCredentialSource>())
        .AddSingleton<ICredentialChain, CompositeCredentialChain>()
        .AddSingleton<OrasOciRegistryTransport>()
        .AddSingleton<IRegistryProvider, AcrRegistryProvider>()
        .AddSingleton<IRegistryProvider, GenericOciRegistryProvider>()
        .AddSingleton<RegistryProviderFactory>()
        .AddSingleton<IOciRegistryTransportFactory>(services =>
        {
            var providerFactory = services.GetRequiredService<RegistryProviderFactory>();
            return new OciRegistryTransportFactory(providerFactory);
        })
        .AddSingleton<ITemplateSpecRepositoryFactory, TemplateSpecRepositoryFactory>()
        .AddSingleton<IArmClientProvider, ArmClientProvider>()
        .AddSingleton<IModuleDispatcher, ModuleDispatcher>()
        .AddSingleton<IArtifactRegistryProvider, DefaultArtifactRegistryProvider>()
        .AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>()
        .AddSingleton<IEnvironment, Core.Utils.Environment>()
        .AddSingleton<IFileSystem, LocalFileSystem>()
        .AddSingleton<IFileExplorer, FileSystemFileExplorer>()
        .AddSingleton<IAuxiliaryFileCache, AuxiliaryFileCache>()
        .AddSingleton<IConfigurationManager, ConfigurationManager>()
        .AddSingleton<IBicepAnalyzer, LinterAnalyzer>()
        .AddSingleton<IFeatureProviderFactory, FeatureProviderFactory>()
        .AddSingleton<ILinterRulesProvider, LinterRulesProvider>()
        .AddSingleton<ISourceFileFactory, SourceFileFactory>()
        .AddRegistryCatalogServices()
        .AddSingleton<BicepCompiler>();

    public static IServiceCollection AddBicepDecompiler(this IServiceCollection services) => services
        .AddSingleton<BicepDecompiler>();

    public static IServiceCollection AddLocalDeploy(this IServiceCollection services) => services
        .AddSingleton<LocalExtensionDispatcherFactory>()
        .AddSingleton<IArmDeploymentProvider, ArmDeploymentProvider>()
        .AddSingleton<ILocalExtensionFactory, GrpcLocalExtensionFactory>();

    public static IServiceCollection AddServerDependencies(
        this IServiceCollection services,
        BicepLangServerOptions bicepLangServerOptions
    ) => services
        .AddBicepCore()
        .AddBicepDecompiler()
        .AddLocalDeploy()
        .AddSingleton<IActiveSourceFileSet, ActiveSourceFileSet>()
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
        .AddSingleton<IDeploymentHelper, DeploymentHelper>()
        .AddSingleton<ISettingsProvider, SettingsProvider>()
        .AddSingleton<IAzureContainerRegistriesProvider, AzureContainerRegistriesProvider>()
        .AddSingleton(bicepLangServerOptions)
        .AddSingleton<DocumentSelectorFactory>();
}
