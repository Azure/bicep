// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Memory;
using Moq;

namespace Bicep.Core.UnitTests.Utils
{
    public static class OciRegistryHelper
    {
        public static OciArtifactReference CreateModuleReferenceMock(string registry, string repository, Uri parentModuleUri, string? digest, string? tag)
            => new(ArtifactType.Module, registry, repository, tag, digest, parentModuleUri);


        public static OciArtifactReference ParseModuleReference(string moduleId /* with or without br: */, Uri? parentModuleUri = null)
        {
            if (moduleId.StartsWith(OciArtifactReferenceFacts.SchemeWithColon))
            {
                moduleId = moduleId.Substring(OciArtifactReferenceFacts.SchemeWithColon.Length);
            }

            OciArtifactReference.TryParse(
                ArtifactType.Module,
                null,
                moduleId,
                BicepTestConstants.BuiltInConfiguration,
                parentModuleUri ?? new Uri("file:///main.bicep"))
                    .IsSuccess(out var moduleReference).Should().BeTrue();
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
        public static (OciArtifactRegistry, MockRegistryBlobClient) CreateModuleRegistry(
            Uri parentModuleUri,
            IFeatureProvider featureProvider)
        {
            IContainerRegistryClientFactory ClientFactory = StrictMock.Of<IContainerRegistryClientFactory>().Object;

            var blobClient = new MockRegistryBlobClient();
            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns(blobClient);

            var registry = new OciArtifactRegistry(
                BicepTestConstants.FileResolver,
                BicepTestConstants.FileSystem,
                clientFactory.Object,
                featureProvider,
                BicepTestConstants.BuiltInConfiguration,
                parentModuleUri);

            return (registry, blobClient);
        }

        public static async Task<(MockRegistryBlobClient, Mock<IContainerRegistryClientFactory>)> PublishArtifactLayersToMockClient(string tempDirectory, string registry, Uri registryUri, string repository, string? mediaType, string? artifactType, string? configContents, IEnumerable<(string mediaType, string contents)> layers)
        {
            var client = new MockRegistryBlobClient();

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory.Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), registryUri, repository)).Returns(client);

            var templateSpecRepositoryFactory = BicepTestConstants.TemplateSpecRepositoryFactory;

            Directory.CreateDirectory(tempDirectory);

            var containerRegistryManager = new AzureContainerRegistryManager(clientFactory.Object);

            var fs = new MockFileSystem();
            var configurationManager = new ConfigurationManager(fs);
            var parentUri = new Uri("http://test.bicep", UriKind.Absolute);
            var configuration = configurationManager.GetConfiguration(parentUri);

            using var compiledStream = new BufferedMemoryStream();

            var moduleReference = CreateModuleReference(registry, repository, "v1", null);
            await containerRegistryManager.PushArtifactAsync(
                configuration: configuration,
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
