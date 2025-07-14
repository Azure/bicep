// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Azure.Bicep.Types.Az;
using Azure.Identity;
using Bicep.Core;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Registry.Catalog.Implementation;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.McpServer.ResourceProperties;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Environment = Bicep.Core.Utils.Environment;
using LocalFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.McpServer;

public static class IServiceCollectionExtensions
{
    public static IMcpServerBuilder AddBicepMcpServer(this IServiceCollection services)
    {
        services
            .AddSingleton<ILogger<ResourceVisitor>>(NullLoggerFactory.Instance.CreateLogger<ResourceVisitor>())
            .AddSingleton<AzResourceTypeLoader>(provider => new(new AzTypeLoader()))
            .AddSingleton<ResourceVisitor>()
            .AddBicepCore();

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddArmClient("00000000-0000-0000-0000-000000000000");
            clientBuilder.UseCredential(new ChainedTokenCredential(
                new AzureCliCredential(),
                new AzurePowerShellCredential()));
        });

        return services.AddMcpServer(options =>
        {
            options.ServerInstructions = Constants.ServerInstructions;
        })
        .WithTools<BicepTools>();
    }
        

    public static IServiceCollection AddBicepCore(this IServiceCollection services) => services
        .AddSingleton<INamespaceProvider, NamespaceProvider>()
        .AddSingleton<IResourceTypeProviderFactory, ResourceTypeProviderFactory>()
        .AddSingleton<IContainerRegistryClientFactory, ContainerRegistryClientFactory>()
        .AddSingleton<ITemplateSpecRepositoryFactory, TemplateSpecRepositoryFactory>()
        .AddSingleton<IModuleDispatcher, ModuleDispatcher>()
        .AddSingleton<IArtifactRegistryProvider, DefaultArtifactRegistryProvider>()
        .AddSingleton<ITokenCredentialFactory, TokenCredentialFactory>()
        .AddSingleton<IFileResolver, FileResolver>()
        .AddSingleton<IEnvironment, Environment>()
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
}
