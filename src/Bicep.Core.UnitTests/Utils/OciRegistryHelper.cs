// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Memory;
using Moq;

namespace Bicep.Core.UnitTests.Utils
{
    public static class OciRegistryHelper
    {
        public static OciArtifactReference CreateModuleReferenceMock(BicepSourceFile referencingFile, string registry, string repository, string? digest, string? tag)
            => new(referencingFile, ArtifactType.Module, registry, repository, tag, digest);

        public static OciArtifactReference ParseModuleReference(string moduleId /* with or without br: */, Uri? parentModuleUri = null)
        {
            if (moduleId.StartsWith(OciArtifactReferenceFacts.SchemeWithColon))
            {
                moduleId = moduleId[OciArtifactReferenceFacts.SchemeWithColon.Length..];
            }
            OciArtifactReference.TryParse(BicepTestConstants.DummyBicepFile, ArtifactType.Module, null, moduleId).IsSuccess(out var moduleReference).Should().BeTrue();
            return moduleReference!;
        }

        public static OciArtifactReference CreateModuleReference(string registry, string repository, string? tag, string? digest, Uri? parentModuleUri = null)
        {
            return ParseModuleReference($"{registry}/{repository}" + (tag is null ? $"@{digest}" : $":{tag}"), parentModuleUri);
        }

        public static void SaveManifestFileToModuleRegistryCache(
            TestContext testContext,
            string registry,
            string repository,
            string manifestFileContents,
            string testOutputPath,
            string? digest,
            string? tag)
        {
            string? manifestFileRelativePath = null;

            if (digest is not null)
            {
                manifestFileRelativePath = Path.Combine("br", registry, repository.Replace("/", "$"), digest.Replace(":", "#"));
            }
            else if (tag is not null)
            {
                manifestFileRelativePath = Path.Combine("br", registry, repository.Replace("/", "$"), tag + "$");
            }

            if (!string.IsNullOrWhiteSpace(manifestFileRelativePath))
            {
                FileHelper.SaveResultFile(testContext, Path.Join(manifestFileRelativePath, "manifest"), manifestFileContents, testOutputPath);
            }
        }

        // create a new (real) OciArtifactRegistry instance with an empty on-disk cache that can push and pull modules
        public static (OciArtifactRegistry, FakeRegistryBlobClient) CreateModuleRegistry()
        {
            IContainerRegistryClientFactory ClientFactory = StrictMock.Of<IContainerRegistryClientFactory>().Object;

            var blobClient = new FakeRegistryBlobClient();
            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<CloudConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns(blobClient);

            var registry = new OciArtifactRegistry(clientFactory.Object, StrictMock.Of<IPublicModuleMetadataProvider>().Object);

            return (registry, blobClient);
        }

        public static async Task<(FakeRegistryBlobClient, Mock<IContainerRegistryClientFactory>)> PublishArtifactLayersToMockClient(string registry, Uri registryUri, string repository, string? mediaType, string? artifactType, string? configContents, IEnumerable<(string mediaType, string contents)> layers)
        {
            var client = new FakeRegistryBlobClient();

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory.Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<CloudConfiguration>(), registryUri, repository)).Returns(client);

            var containerRegistryManager = new AzureContainerRegistryManager(clientFactory.Object);
            var configuration = IConfigurationManager.GetBuiltInConfiguration();

            using var compiledStream = new BufferedMemoryStream();

            var moduleReference = CreateModuleReference(registry, repository, "v1", null);
            await containerRegistryManager.PushArtifactAsync(
                configuration.Cloud,
                artifactReference: moduleReference,
                mediaType: mediaType,
                artifactType: artifactType,
                config: new OciDescriptor(configContents ?? string.Empty, BicepMediaTypes.BicepModuleConfigV1),
                layers: layers.Select(layer => new OciDescriptor(layer.contents, layer.mediaType)),
                new OciManifestAnnotationsBuilder()
            );

            return (client, clientFactory);
        }

    }
}
