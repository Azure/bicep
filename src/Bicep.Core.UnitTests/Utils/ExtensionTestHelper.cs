// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;

namespace Bicep.Core.UnitTests.Utils;

public static class ExtensionTestHelper
{
    public static ServiceBuilder GetServiceBuilder(
        IFileSystem fileSystem,
        string registryHost,
        string repositoryPath,
        FeatureProviderOverrides featureOverrides)
    {
        var clientFactory = RegistryHelper.CreateMockRegistryClient(registryHost, repositoryPath);

        return new ServiceBuilder()
            .WithFeatureOverrides(featureOverrides)
            .WithFileSystem(fileSystem)
            .WithContainerRegistryClientFactory(clientFactory);
    }

    public static Task<ServiceBuilder> GetServiceBuilderWithPublishedExtension(BinaryData tgzData, FeatureProviderOverrides features, IFileSystem? fileSystem = null)
        => GetServiceBuilderWithPublishedExtension(tgzData, "example.azurecr.io/extensions/foo:1.2.3", features, fileSystem);

    public static async Task<ServiceBuilder> GetServiceBuilderWithPublishedExtension(BinaryData tgzData, string target, FeatureProviderOverrides features, IFileSystem? fileSystem = null)
    {
        var reference = OciArtifactReference.TryParseModule(target).Unwrap();

        fileSystem ??= new MockFileSystem();
        var services = GetServiceBuilder(fileSystem, reference.Registry, reference.Repository, features);

        await RegistryHelper.PublishExtensionToRegistryAsync(services.Build(), reference.FullyQualifiedReference, tgzData);

        return services;
    }
}
