// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.AzureApi;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Utils;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using BicepConfig = Bicep.Core.Configuration;
using Environment = Bicep.Core.Utils.Environment;

namespace Bicep.Wasm;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBicepCore(this IServiceCollection services) => services
        .AddSingleton<INamespaceProvider, NamespaceProvider>()
        .AddSingleton<IResourceTypeProviderFactory, ResourceTypeProviderFactory>()
        .AddSingleton<IArmClientProvider, ArmClientProvider>()
        .AddSingleton<IModuleDispatcher, ModuleDispatcher>()
        .AddSingleton<IArtifactRegistryProvider, EmptyModuleRegistryProvider>()
        .AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>()
        .AddSingleton<IEnvironment, Environment>()
        .AddSingleton<IFileSystem, MockFileSystem>()
        .AddSingleton<IFileExplorer, FileSystemFileExplorer>()
        .AddSingleton<IAuxiliaryFileCache, AuxiliaryFileCache>()
        .AddSingleton<BicepConfig.IConfigurationManager, BicepConfig.ConfigurationManager>()
        .AddSingleton<IBicepAnalyzer, LinterAnalyzer>()
        .AddSingleton<IFeatureProviderFactory, FeatureProviderFactory>()
        .AddSingleton<ILinterRulesProvider, LinterRulesProvider>()
        .AddSingleton<ISourceFileFactory, SourceFileFactory>()
        .AddRegistryCatalogServices()
        .AddSingleton<BicepCompiler>();

    public static IServiceCollection AddBicepDecompiler(this IServiceCollection services) => services
        .AddSingleton<BicepDecompiler>();
}
