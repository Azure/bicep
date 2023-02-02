// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class OciModuleRegistryTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataRow("")]
        [DataRow("    ")]
        [DataRow(null)]
        [DataTestMethod]
        public void GetDocumentationUri_WithInvalidManifestContents_ShouldReturnNull(string manifestFileContents)
        {
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = ociModuleRegistry.GetDocumentationUri(ociArtifactModuleReference);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetDocumentationUri_WithNonExistentManifestFile_ShouldReturnNull()
        {
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                "some_manifest_text",
                "test.azurecr.io",
                "bicep/modules/storage",
                digest: "sha:12345",
                cacheRootDirectory: false);

            var result = ociModuleRegistry.GetDocumentationUri(ociArtifactModuleReference);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetDocumentationUri_WithManifestFileAndNoAnnotations_ShouldReturnNull()
        {
            var manifestFileContents = @"{
  ""schemaVersion"": 2,
  ""artifactType"": ""application/vnd.ms.bicep.module.artifact"",
  ""config"": {
    ""mediaType"": ""application/vnd.ms.bicep.module.config.v1+json"",
    ""digest"": ""sha256:e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"",
    ""size"": 0,
    ""annotations"": {}
  },
  ""layers"": [
    {
      ""mediaType"": ""application/vnd.ms.bicep.module.layer.v1+json"",
      ""digest"": ""sha256:9846dcfde47a4b2943be478754d1169ece3adc6447c9596d9ba48e2579c24173"",
      ""size"": 735131,
      ""annotations"": {}
    }
  ]
}";
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = ociModuleRegistry.GetDocumentationUri(ociArtifactModuleReference);

            result.Should().BeNull();
        }

        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public void GetDocumentationUri_WithAnnotationsInManifestFileAndInvalidDocumentationUri_ShouldReturnNull(string documentationUri)
        {
            var manifestFileContents = @"{
  ""schemaVersion"": 2,
  ""artifactType"": ""application/vnd.ms.bicep.module.artifact"",
  ""config"": {
    ""mediaType"": ""application/vnd.ms.bicep.module.config.v1+json"",
    ""digest"": ""sha256:e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"",
    ""size"": 0,
    ""annotations"": {}
  },
  ""layers"": [
    {
      ""mediaType"": ""application/vnd.ms.bicep.module.layer.v1+json"",
      ""digest"": ""sha256:9846dcfde47a4b2943be478754d1169ece3adc6447c9596d9ba48e2579c24173"",
      ""size"": 735131,
      ""annotations"": {}
    }
  ],
  ""annotations"": {
    ""org.opencontainers.image.documentation"": """ + documentationUri + @"""
  }
}";
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = ociModuleRegistry.GetDocumentationUri(ociArtifactModuleReference);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetDocumentationUri_WithValidDocumentationUriInManifestFile_ShouldReturnDocumentationUri()
        {
            var documentationUri = @"https://github.com/Azure/bicep-registry-modules/blob/main/modules/samples/hello-world/README.md";
            var manifestFileContents = @"{
  ""schemaVersion"": 2,
  ""artifactType"": ""application/vnd.ms.bicep.module.artifact"",
  ""config"": {
    ""mediaType"": ""application/vnd.ms.bicep.module.config.v1+json"",
    ""digest"": ""sha256:e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"",
    ""size"": 0,
    ""annotations"": {}
  },
  ""layers"": [
    {
      ""mediaType"": ""application/vnd.ms.bicep.module.layer.v1+json"",
      ""digest"": ""sha256:9846dcfde47a4b2943be478754d1169ece3adc6447c9596d9ba48e2579c24173"",
      ""size"": 735131,
      ""annotations"": {}
    }
  ],
  ""annotations"": {
    ""org.opencontainers.image.documentation"": """ + documentationUri + @"""
  }
}";
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = ociModuleRegistry.GetDocumentationUri(ociArtifactModuleReference);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(documentationUri);
        }

        [TestMethod]
        public void GetDocumentationUri_WithMcrModuleReferenceAndNoDocumentationUriInManifestFile_ShouldReturnDocumentationUriThatPointsToReadmeLink()
        {
            var manifestFileContents = @"{
  ""schemaVersion"": 2,
  ""artifactType"": ""application/vnd.ms.bicep.module.artifact"",
  ""config"": {
    ""mediaType"": ""application/vnd.ms.bicep.module.config.v1+json"",
    ""digest"": ""sha256:e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"",
    ""size"": 0,
    ""annotations"": {}
  },
  ""layers"": [
    {
      ""mediaType"": ""application/vnd.ms.bicep.module.layer.v1+json"",
      ""digest"": ""sha256:9846dcfde47a4b2943be478754d1169ece3adc6447c9596d9ba48e2579c24173"",
      ""size"": 735131,
      ""annotations"": {}
    }
  ]
}";
            var bicepFileContents = @"module myenv 'br:mcr.microsoft.com/bicep/app/dapr-containerapps-environment:1.0.1' = {
  name: 'state'
  params: {
    location: 'eastus'
    nameseed: 'stateSt1'
    applicationEntityName: 'appdata'
    daprComponentType: 'state.azure.blobstorage'
    daprComponentScopes: [
      'nodeapp'
    ]
  }
}";
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                bicepFileContents,
                manifestFileContents,
                "mcr.microsoft.com",
                "bicep/app/dapr-containerapps-environment/bicep/core",
                tag: "1.0.1");

            var result = ociModuleRegistry.GetDocumentationUri(ociArtifactModuleReference);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo("https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapps-environment/bicep/core/1.0.1/modules/app/dapr-containerapps-environment/bicep/core/README.md");
        }

        private (OciModuleRegistry, OciArtifactModuleReference) GetOciModuleRegistryAndOciArtifactModuleReference(
            string bicepFileContents,
            string manifestFileContents,
            string registory,
            string repository,
            string? digest= null,
            string? tag = null,
            bool cacheRootDirectory = true)
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);
            var parentModuleUri = DocumentUri.FromFileSystemPath(bicepPath).ToUri();

            var ociArtifactModuleReference = OciArtifactModuleReferenceHelper.GetModuleReferenceAndSaveManifestFile(
                TestContext,
                registory,
                repository,
                manifestFileContents,
                testOutputPath,
                parentModuleUri,
                digest,
                tag);

            var ociModuleRegistry = new OciModuleRegistry(BicepTestConstants.FileResolver, BicepTestConstants.ClientFactory, GetFeatures(cacheRootDirectory, testOutputPath), BicepTestConstants.BuiltInConfiguration, parentModuleUri);

            return (ociModuleRegistry, ociArtifactModuleReference);
        }

        private IFeatureProvider GetFeatures(bool cacheRootDirectory, string rootDirectory)
        {
            var features = StrictMock.Of<IFeatureProvider>();
            features.Setup(m => m.RegistryEnabled).Returns(true);

            if (cacheRootDirectory)
            {
                features.Setup(m => m.CacheRootDirectory).Returns(rootDirectory);
            }
            else
            {
                features.Setup(m => m.CacheRootDirectory).Returns(string.Empty);
            }

            return features.Object;
        }
    }
}
