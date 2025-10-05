// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Features;
using Bicep.IO.Abstraction;
using Bicep.TextFixtures.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.Core.UnitTests.Utils;

public static class ExtensionTestHelper
{
    public static ServiceBuilder GetServiceBuilder(
        IFileSystem fileSystem,
        string registryHost,
        string repositoryPath,
        FeatureProviderOverrides featureOverrides)
    {
        var clientFactory = RegistryHelper.CreateMockRegistryClient(new RepoDescriptor(registryHost, repositoryPath, ["tag"]));

        return new ServiceBuilder()
            .WithFeatureOverrides(featureOverrides)
            .WithFileSystem(fileSystem)
            .WithContainerRegistryClientFactory(clientFactory);
    }

    public static Task<ServiceBuilder> GetServiceBuilderWithPublishedExtension(BinaryData tgzData, FeatureProviderOverrides features, IFileSystem? fileSystem = null, string? artifactTarget = null)
        => GetServiceBuilderWithPublishedExtension(new ExtensionPackage(tgzData, false, []), artifactTarget ?? "example.azurecr.io/extensions/foo:1.2.3", features, fileSystem);

    public static Task<ServiceBuilder> GetServiceBuilderWithPublishedExtension(ExtensionPackage package, FeatureProviderOverrides features, IFileSystem? fileSystem = null, string? artifactTarget = null)
        => GetServiceBuilderWithPublishedExtension(package, artifactTarget ?? "example.azurecr.io/extensions/foo:1.2.3", features, fileSystem);

    public static async Task<ServiceBuilder> GetServiceBuilderWithPublishedExtension(ExtensionPackage package, string target, FeatureProviderOverrides features, IFileSystem? fileSystem = null)
    {
        var reference = OciArtifactReference.TryParse(BicepTestConstants.DummyBicepFile, ArtifactType.Module, null, target).Unwrap();

        fileSystem ??= new MockFileSystem();
        var services = GetServiceBuilder(fileSystem, reference.Registry, reference.Repository, features);

        await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), reference.FullyQualifiedReference, package);

        return services;
    }

    public static MockExtensionData CreateMockExtensionMockData(string name, string version, string repoVersion, CustomExtensionTypeFactoryDelegates typeFactoryDelegates)
        => new(name, version, repoVersion, ExtensionResourceTypeHelper.CreateTypesTgzBytesForCustomExtension(name, version, typeFactoryDelegates));

    public static async Task<ServiceBuilder> AddMockExtension(ServiceBuilder services, MockExtensionData MockExtensionData)
    {
        await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), MockExtensionData.ExtensionRepoReference, MockExtensionData.TypesTgzData);

        return services;
    }

    public static async Task<ServiceBuilder> AddMockExtensions(ServiceBuilder services, TestContext testContext, params MockExtensionData[] extensionMocks)
    {
        var clientFactory = RegistryHelper.CreateMockRegistryClient(
            extensionMocks.Select(ext => new RepoDescriptor(ext.Registry, ext.RepoPath, ext.Tags)).ToArray());

        services = services
            .WithFeaturesOverridden(f => f with { CacheRootDirectory = ExtensionTestHelper.GetCacheRootDirectory(testContext) })
            .WithContainerRegistryClientFactory(clientFactory);

        foreach (var ext in extensionMocks)
        {
            await ExtensionTestHelper.AddMockExtension(services, ext);
        }

        return services;
    }

    public static async Task<ServiceBuilder> AddMockMsGraphExtension(ServiceBuilder services, TestContext testContext)
    {
        var indexJsonBeta = FileHelper.SaveResultFile(testContext, "types/index-beta.json", BicepTestConstants.GetMsGraphIndexJson(BicepTestConstants.MsGraphVersionBeta));
        var indexJsonV10 = FileHelper.SaveResultFile(testContext, "types/index-v1.0.json", BicepTestConstants.GetMsGraphIndexJson(BicepTestConstants.MsGraphVersionV10));

        services = services
            .WithFeaturesOverridden(f => f with { CacheRootDirectory = GetCacheRootDirectory(testContext) })
            .WithContainerRegistryClientFactory(RegistryHelper.CreateOciClientForMsGraphExtension());

        await RegistryHelper.PublishMsGraphExtension(services.Build(), indexJsonBeta, "beta", BicepTestConstants.MsGraphVersionBeta);
        await RegistryHelper.PublishMsGraphExtension(services.Build(), indexJsonV10, "v1", BicepTestConstants.MsGraphVersionV10);

        return services;
    }

    public static IDirectoryHandle GetCacheRootDirectory(TestContext testContext) => FileHelper.GetCacheRootDirectory(testContext).EnsureExists();
}
