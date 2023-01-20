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
        public void TryGetDocumentationUrl_WithInvalidManifestContents_ShouldReturnFalse(string manifestFileContents)
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"output myOutput string = 'hello!'", testOutputPath);
            var parentModuleUri = DocumentUri.FromFileSystemPath(bicepPath).ToUri();

            var ociArtifactModuleReference = GetModuleReferenceAndSaveManifestFile(
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345",
                manifestFileContents,
                testOutputPath,
                parentModuleUri);
            var ociModuleRegistry = new OciModuleRegistry(BicepTestConstants.FileResolver, BicepTestConstants.ClientFactory, GetFeatures(testOutputPath), BicepTestConstants.BuiltInConfiguration, parentModuleUri);

            ociModuleRegistry.TryGetDocumentationUrl(ociArtifactModuleReference, out _).Should().BeFalse();
        }

        [TestMethod]
        public void TryGetDocumentationUrl_WithManifestFileAndNoAnnotations_ShouldReturnFalse()
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"output myOutput string = 'hello!'", testOutputPath);
            var parentModuleUri = DocumentUri.FromFileSystemPath(bicepPath).ToUri();

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
            var ociArtifactModuleReference = GetModuleReferenceAndSaveManifestFile(
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345",
                manifestFileContents,
                testOutputPath,
                parentModuleUri);
            var ociModuleRegistry = new OciModuleRegistry(BicepTestConstants.FileResolver, BicepTestConstants.ClientFactory, GetFeatures(testOutputPath), BicepTestConstants.BuiltInConfiguration, parentModuleUri);

            ociModuleRegistry.TryGetDocumentationUrl(ociArtifactModuleReference, out _).Should().BeFalse();
        }

        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public void TryGetDocumentationUrl_WithAnnotationsInManifestFileAndInvalidDocumentationUrl_ShouldReturnFalse(string documentationUrl)
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"output myOutput string = 'hello!'", testOutputPath);
            var parentModuleUri = DocumentUri.FromFileSystemPath(bicepPath).ToUri();

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
    ""documentation"": """+ documentationUrl + @"""
  }
}";
            var ociArtifactModuleReference = GetModuleReferenceAndSaveManifestFile(
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345",
                manifestFileContents,
                testOutputPath,
                parentModuleUri);
            var ociModuleRegistry = new OciModuleRegistry(BicepTestConstants.FileResolver, BicepTestConstants.ClientFactory, GetFeatures(testOutputPath), BicepTestConstants.BuiltInConfiguration, parentModuleUri);

            ociModuleRegistry.TryGetDocumentationUrl(ociArtifactModuleReference, out _).Should().BeFalse();
        }

        [TestMethod]
        public void TryGetDocumentationUrl_WithValidDocumentationUrlInManifestFile_ShouldReturnTrue()
        {
            var documentationUrl = @"https://github.com/Azure/bicep-registry-modules/blob/main/modules/samples/hello-world/README.md";
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", @"output myOutput string = 'hello!'", testOutputPath);
            var parentModuleUri = DocumentUri.FromFileSystemPath(bicepPath).ToUri();

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
    ""documentation"": """+ documentationUrl + @"""
  }
}";
            var ociArtifactModuleReference = GetModuleReferenceAndSaveManifestFile(
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345",
                manifestFileContents,
                testOutputPath,
                parentModuleUri);
            var ociModuleRegistry = new OciModuleRegistry(BicepTestConstants.FileResolver, BicepTestConstants.ClientFactory, GetFeatures(testOutputPath), BicepTestConstants.BuiltInConfiguration, parentModuleUri);

            ociModuleRegistry.TryGetDocumentationUrl(ociArtifactModuleReference, out string? result).Should().BeTrue();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(documentationUrl);
        }

        private IFeatureProvider GetFeatures(string rootDirectory)
        {
            var features = StrictMock.Of<IFeatureProvider>();
            features.Setup(m => m.RegistryEnabled).Returns(true);
            features.Setup(m => m.CacheRootDirectory).Returns(rootDirectory);

            return features.Object;
        }

        private OciArtifactModuleReference GetModuleReferenceAndSaveManifestFile(string registory, string repository, string digest, string manifestFileContents, string testOutputPath, Uri parentModuleUri)
        {
            var manifestFilePath = Path.Combine(testOutputPath, "br", registory, repository.Replace("/", "$"), digest.Replace(":", "#"));
            FileHelper.SaveResultFile(TestContext, "manifest", manifestFileContents, manifestFilePath);

            return new OciArtifactModuleReference(registory, repository, null, digest, parentModuleUri);
        }
    }
}
