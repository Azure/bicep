// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Providers;
using Bicep.Core.SourceCode;
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

    public static async Task PublishModuleToRegistry(IContainerRegistryClientFactory clientFactory, string moduleName, string target, string moduleSource, bool publishSource, string? documentationUri = null)
    {
        var featureProviderFactory = BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(PublishSourceEnabled: publishSource));
        var dispatcher = ServiceBuilder.Create(s => s.WithDisabledAnalyzersConfiguration()
            .AddSingleton(clientFactory)
            .AddSingleton(BicepTestConstants.TemplateSpecRepositoryFactory)
            .AddSingleton(featureProviderFactory)
            ).Construct<IModuleDispatcher>();

        var targetReference = dispatcher.TryGetArtifactReference(ArtifactType.Module, target, RandomFileUri()).IsSuccess(out var @ref) ? @ref
            : throw new InvalidOperationException($"Module '{moduleName}' has an invalid target reference '{target}'. Specify a reference to an OCI artifact.");

        var result = CompilationHelper.Compile(moduleSource);
        if (result.Template is null)
        {
            throw new InvalidOperationException($"Module {moduleName} failed to produce a template.");
        }

        var features = featureProviderFactory.GetFeatureProvider(result.BicepFile.FileUri);
        BinaryData? sourcesStream = publishSource ? BinaryData.FromStream(SourceArchive.PackSourcesIntoStream(dispatcher, result.Compilation.SourceFileGrouping, features.CacheRootDirectory)) : null;
        await dispatcher.PublishModule(targetReference, BinaryData.FromString(result.Template.ToString()), sourcesStream, documentationUri);
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

        var targetReference = dispatcher.TryGetArtifactReference(ArtifactType.Provider, target, new Uri("file:///main.bicep")).Unwrap();

        await dispatcher.PublishProvider(targetReference, tgzData);
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
