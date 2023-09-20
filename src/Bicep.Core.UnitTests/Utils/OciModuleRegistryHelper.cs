// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Registry;
using FluentAssertions;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Bicep.Core.Features;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.WindowsAzure.ResourceStack.Common.Memory;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Utils
{
    public static class OciModuleRegistryHelper
    {
        public static OciModuleReference CreateModuleReferenceMock(
            string registry,
            string repository,
            Uri parentModuleUri,
            string? digest,
            string? tag)
        {
            var artifactReferenceMock = StrictMock.Of<IOciArtifactReference>();
            artifactReferenceMock.SetupGet(m => m.Registry).Returns(registry);
            artifactReferenceMock.SetupGet(m => m.Repository).Returns(repository);
            artifactReferenceMock.SetupGet(m => m.Digest).Returns(digest);
            artifactReferenceMock.SetupGet(m => m.Tag).Returns(tag);
            artifactReferenceMock.SetupGet(m => m.ArtifactId).Returns($"{registry}/{repository}:{tag ?? digest}");

            return new OciModuleReference(artifactReferenceMock.Object, parentModuleUri);
        }

        public static OciModuleReference CreateModuleReference(string registry, string repository, string? tag, string? digest)
        {
            OciModuleReference.TryParse(null, $"{registry}/{repository}:{tag}", BicepTestConstants.BuiltInConfiguration, new Uri("file:///main.bicep")).IsSuccess(out var moduleReference).Should().BeTrue();
            return moduleReference!;
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

        // public a new (real) OciModuleRegistry instance with an empty on-disk cache that can push and pull modules
        public static (OciModuleRegistry, MockRegistryBlobClient) CreateModuleRegistry(
            Uri parentModuleUri,
            IFeatureProvider featureProvider)
        {
            IContainerRegistryClientFactory ClientFactory = StrictMock.Of<IContainerRegistryClientFactory>().Object;

            var blobClient = new MockRegistryBlobClient();
            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns(blobClient);

            var registry = new OciModuleRegistry(
                BicepTestConstants.FileResolver,
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
                config: new StreamDescriptor(new TextByteArray(configContents ?? string.Empty).ToStream(), BicepMediaTypes.BicepModuleConfigV1),
                layers: (layers.Select(layer => new StreamDescriptor(TextByteArray.TextToStream(layer.contents), layer.mediaType))),
                new OciManifestAnnotationsBuilder()
            );

            return (client, clientFactory);
        }

    }
}
