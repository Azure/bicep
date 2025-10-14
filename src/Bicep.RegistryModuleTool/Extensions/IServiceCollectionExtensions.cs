// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Oci.Oras;
using Bicep.Core.Registry.Providers;
using Bicep.Core.AzureApi;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using Environment = Bicep.Core.Utils.Environment;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddBicepCompiler(this IServiceCollection services) => services
            .AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton<IFileExplorer, FileSystemFileExplorer>()
            .AddSingleton<IAuxiliaryFileCache, AuxiliaryFileCache>()
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
            .AddSingleton<IEnvironment, Environment>()
            .AddSingleton<IConfigurationManager, ConfigurationManager>()
            .AddSingleton<IBicepAnalyzer, LinterAnalyzer>()
            .AddSingleton<IFeatureProviderFactory, FeatureProviderFactory>()
            .AddSingleton<ILinterRulesProvider, LinterRulesProvider>()
            .AddSingleton<ISourceFileFactory, SourceFileFactory>()
            .AddRegistryCatalogServices()
            .AddSingleton<BicepCompiler>();
    }
}
