// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Features;
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

    public static Task<ServiceBuilder> GetServiceBuilderWithPublishedExtension(BinaryData tgzData, FeatureProviderOverrides features, IFileSystem? fileSystem = null)
        => GetServiceBuilderWithPublishedExtension(new ExtensionPackage(tgzData, false, []), "example.azurecr.io/extensions/foo:1.2.3", features, fileSystem);

    public static Task<ServiceBuilder> GetServiceBuilderWithPublishedExtension(ExtensionPackage package, FeatureProviderOverrides features, IFileSystem? fileSystem = null)
        => GetServiceBuilderWithPublishedExtension(package, "example.azurecr.io/extensions/foo:1.2.3", features, fileSystem);

    public static async Task<ServiceBuilder> GetServiceBuilderWithPublishedExtension(ExtensionPackage package, string target, FeatureProviderOverrides features, IFileSystem? fileSystem = null)
    {
        var reference = OciArtifactReference.TryParse(BicepTestConstants.DummyBicepFile, ArtifactType.Module, null, target).Unwrap();

        fileSystem ??= new MockFileSystem();
        var services = GetServiceBuilder(fileSystem, reference.Registry, reference.Repository, features);

        await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), reference.FullyQualifiedReference, package);

        return services;
    }

    public static async Task<ServiceBuilder> AddMockMsGraphExtension(ServiceBuilder services, TestContext testContext)
    {
        var indexJsonBeta = FileHelper.SaveResultFile(testContext, "types/index-beta.json", BicepTestConstants.GetMsGraphIndexJson(BicepTestConstants.MsGraphVersionBeta));
        var indexJsonV10 = FileHelper.SaveResultFile(testContext, "types/index-v1.0.json", BicepTestConstants.GetMsGraphIndexJson(BicepTestConstants.MsGraphVersionV10));

        var cacheRoot = FileHelper.GetCacheRootDirectory(testContext).EnsureExists();

        services = services
            .WithFeaturesOverridden(f => f with { CacheRootDirectory = cacheRoot })
            .WithContainerRegistryClientFactory(RegistryHelper.CreateOciClientForMsGraphExtension());

        await RegistryHelper.PublishMsGraphExtension(services.Build(), indexJsonBeta, "beta", BicepTestConstants.MsGraphVersionBeta);
        await RegistryHelper.PublishMsGraphExtension(services.Build(), indexJsonV10, "v1", BicepTestConstants.MsGraphVersionV10);

        return services;
    }
}
