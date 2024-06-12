// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Providers;
using Bicep.Core.SourceCode;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.UnitTests.Utils;

public static class RegistryHelper
{
    public static IContainerRegistryClientFactory CreateMockRegistryClient(string registry, string repository)
    {
        return new TestContainerRegistryClientFactoryBuilder()
            .RegisterMockRepositoryBlobClient(registry, repository)
            .Build().clientFactory;
    }

    public static (IContainerRegistryClientFactory factoryMock, ImmutableDictionary<(Uri, string), MockRegistryBlobClient> blobClientMocks) CreateMockRegistryClients(params (string, string)[] clients)
    {
        var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();

        foreach (var (registryHost, repository) in clients)
        {
            containerRegistryFactoryBuilder.RegisterMockRepositoryBlobClient(registryHost, repository);

        }

        return containerRegistryFactoryBuilder.Build();
    }

    // Example target: br:mockregistry.io/test/module1:v1
    public static async Task PublishModuleToRegistryAsync(
        IContainerRegistryClientFactory clientFactory,
        IFileSystem fileSystem,
        string moduleName,
        string target,
        string moduleSource,
        bool publishSource,
        string? documentationUri = null)
    {
        var featureProviderFactory = BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides());

        var services = new ServiceBuilder()
            .WithDisabledAnalyzersConfiguration()
            .WithContainerRegistryClientFactory(clientFactory)
            .WithFileSystem(fileSystem)
            .WithTemplateSpecRepositoryFactory(BicepTestConstants.TemplateSpecRepositoryFactory)
            .WithFeatureProviderFactory(featureProviderFactory);

        var dispatcher = services.Build().Construct<IModuleDispatcher>();

        var targetReference = dispatcher.TryGetArtifactReference(ArtifactType.Module, target, RandomFileUri()).IsSuccess(out var @ref) ? @ref
            : throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{target}'. Specify a reference to an OCI artifact.");

        var result = await CompilationHelper.RestoreAndCompile(services, moduleSource);
        if (result.Template is null)
        {
            throw new InvalidOperationException($"Module {moduleName} failed to produce a template.");
        }

        var features = featureProviderFactory.GetFeatureProvider(result.BicepFile.FileUri);
        BinaryData? sourcesStream = publishSource ? BinaryData.FromStream(SourceArchive.PackSourcesIntoStream(dispatcher, result.Compilation.SourceFileGrouping, features.CacheRootDirectory)) : null;
        await dispatcher.PublishModule(targetReference, BinaryData.FromString(result.Template.ToString()), sourcesStream, documentationUri);
    }

    // Example target: br:mockregistry.io/test/module1:v1
    // Module name is automatically extracted from target (in this case, "module1")
    public static async Task PublishModuleToRegistryAsync(IContainerRegistryClientFactory clientFactory, IFileSystem fileSystem, string target, string source, bool withSource)
    {
        await PublishModuleToRegistryAsync(
              clientFactory,
              fileSystem,
              target.Substring(target.LastIndexOf('/')),
              target,
              source,
              publishSource: withSource);
    }

    // Creates a new registry client factory and publishes the specified modules to the registry.
    // Example usage:
    // var clientFactory = await PublishModules([                
    //    ("br:mockregistry.io/test/module1:v1", "param p1 bool", withSource: true),
    //    ("br:mockregistry.io/test/module2:v1", "param p2 string", withSource: true),
    //    ("br:mockregistry.io/test/module1:v2", "param p12 string", withSource: false),
    // ]);
    public static async Task<IContainerRegistryClientFactory> CreateMockRegistryClientWithPublishedModulesAsync(
        IFileSystem fileSystem,
        params (string target, string source, bool withSource)[] modules)
    {
        var repos = new List<(string registry, string repo)>();

        foreach (var module in modules)
        {
            var (registry, repo) = module.target.ExtractRegexGroups(
                "^br:(?<registry>.+?)/(?<repo>.+?)[:@](?<tag>.+?)$",
                ["registry", "repo"]);

            if (!repos.Contains((registry, repo)))
            {
                repos.Add((registry, repo));
            }
        }

        var clientFactory = CreateMockRegistryClients(repos.ToArray()).factoryMock;

        foreach (var module in modules)
        {
            await PublishModuleToRegistryAsync(
                  clientFactory,
                  fileSystem,
                  module.target,
                  module.source,
                  module.withSource);
        }

        return clientFactory;
    }

    public static async Task PublishProviderToRegistryAsync(IDependencyHelper services, string pathToIndexJson, string target)
    {
        var fileSystem = services.Construct<IFileSystem>();

        var tgzData = await TypesV1Archive.GenerateProviderTarStream(fileSystem, pathToIndexJson);

        await PublishProviderToRegistryAsync(services, target, tgzData);
    }

    public static async Task PublishProviderToRegistryAsync(IDependencyHelper services, string target, BinaryData tgzData)
    {
        var dispatcher = services.Construct<IModuleDispatcher>();

        var targetProviderUri = PathHelper.FilePathToFileUrl(PathHelper.ResolvePath("dummy"));
        if (!target.StartsWith("br:"))
        {
            // convert to a relative path, as this is the only format supported for the local filesystem
            targetProviderUri = PathHelper.FilePathToFileUrl(PathHelper.ResolvePath(target));
            target = Path.GetFileName(targetProviderUri.LocalPath);
        }

        if (!dispatcher.TryGetArtifactReference(ArtifactType.Provider, target, targetProviderUri).IsSuccess(out var targetReference, out var errorBuilder))
        {
            throw new InvalidOperationException($"Failed to get reference '{errorBuilder(DiagnosticBuilder.ForDocumentStart()).Message}'.");
        }

        await dispatcher.PublishProvider(targetReference, new(tgzData, false, []));
    }

    private static Uri RandomFileUri() => PathHelper.FilePathToFileUrl(Path.GetTempFileName());

    public static async Task PublishAzProvider(IDependencyHelper services, string pathToIndexJson)
    {
        var version = BicepTestConstants.BuiltinAzProviderVersion;
        var repository = "bicep/providers/az";
        await PublishProviderToRegistryAsync(services, pathToIndexJson, $"br:{LanguageConstants.BicepPublicMcrRegistry}/{repository}:{version}");
    }

    public static IContainerRegistryClientFactory CreateOciClientForAzProvider()
        => CreateMockRegistryClients((LanguageConstants.BicepPublicMcrRegistry, $"bicep/providers/az")).factoryMock;
}
