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
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBicepCompiler(this IServiceCollection services) => services
            .AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton<INamespaceProvider, DefaultNamespaceProvider>()
            .AddSingleton<IAzResourceTypeLoader, AzResourceTypeLoader>()
            .AddSingleton<IAzResourceTypeLoaderFactory, AzResourceTypeLoaderFactory>()
            .AddSingleton<IContainerRegistryClientFactory, ContainerRegistryClientFactory>()
            .AddSingleton<ITemplateSpecRepositoryFactory, TemplateSpecRepositoryFactory>()
            .AddSingleton<IModuleDispatcher, ModuleDispatcher>()
            .AddSingleton<IArtifactRegistryProvider, DefaultArtifactRegistryProvider>()
            .AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>()
            .AddSingleton<IFileResolver, FileResolver>()
            .AddSingleton<IConfigurationManager, ConfigurationManager>()
            .AddSingleton<IBicepAnalyzer, LinterAnalyzer>()
            .AddSingleton<IFeatureProviderFactory, FeatureProviderFactory>()
            .AddSingleton<ILinterRulesProvider, LinterRulesProvider>()
            .AddSingleton<BicepCompiler>();
    }
}
