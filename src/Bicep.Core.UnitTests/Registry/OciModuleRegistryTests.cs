// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceCode;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class OciArtifactRegistryTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [NotNull]
        private string? TestOutputPath { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            TestOutputPath = FileHelper.GetUniqueTestOutputPath(this.TestContext);
        }

        #region GetDocumentationUri

        [DataRow("")]
        [DataRow("    ")]
        [DataRow(null)]
        [DataTestMethod]
        public void GetDocumentationUri_WithInvalidManifestContents_ShouldReturnNull(string manifestFileContents)
        {
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = OciArtifactRegistry.TryGetDocumentationUri(OciArtifactReference);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetDocumentationUri_WithNonExistentManifestFile_ShouldReturnNull()
        {
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                null,
                "test.azurecr.io",
                "bicep/modules/storage",
                digest: "sha:12345");

            var result = OciArtifactRegistry.TryGetDocumentationUri(OciArtifactReference);

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
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = OciArtifactRegistry.TryGetDocumentationUri(OciArtifactReference);

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
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = OciArtifactRegistry.TryGetDocumentationUri(OciArtifactReference);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetDocumentationUri_WithAnnotationsInManifestFile_ButEmpty_ShouldReturnNullDocumentationAndDescription()
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
   }
 }";
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var documentation = OciArtifactRegistry.TryGetDocumentationUri(OciArtifactReference);
            documentation.Should().BeNull();

            var description = await OciArtifactRegistry.TryGetModuleDescription(null!, OciArtifactReference);
            description.Should().BeNull();
        }

        [TestMethod]
        public async Task GetDocumentationUri_WithAnnotationsInManifestFile_ButOnlyHasOtherProperties_ShouldReturnNullDocumentationAndDescription()
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
      ""org.opencontainers.image.notdocumentation"": """ + "documentationUri" + @""",
      ""org.opencontainers.image.notdescription"": """ + "description" + @"""
   }
 }";
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var documentation = OciArtifactRegistry.TryGetDocumentationUri(OciArtifactReference);
            documentation.Should().BeNull();

            var description = await OciArtifactRegistry.TryGetModuleDescription(null!, OciArtifactReference);
            description.Should().BeNull();
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
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = OciArtifactRegistry.TryGetDocumentationUri(OciArtifactReference);

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
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                bicepFileContents,
                manifestFileContents,
                LanguageConstants.BicepPublicMcrRegistry,
                "bicep/app/dapr-containerapps-environment/bicep/core",
                tag: "1.0.1");

            var result = OciArtifactRegistry.TryGetDocumentationUri(OciArtifactReference);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo("https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapps-environment/bicep/core/1.0.1/modules/app/dapr-containerapps-environment/bicep/core/README.md");
        }

        #endregion GetDocumentationUri

        #region GetDescription

        [DataRow("")]
        [DataRow("    ")]
        [DataRow(null)]
        [DataTestMethod]
        public void GetDescription_WithInvalidManifestContents_ShouldReturnNull(string manifestFileContents)
        {
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = OciArtifactRegistry.TryGetDocumentationUri(OciArtifactReference);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetDescription_WithNonExistentManifestFile_ShouldReturnNull()
        {
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                null,
                "test.azurecr.io",
                "bicep/modules/storage",
                digest: "sha:12345");

            var result = await OciArtifactRegistry.TryGetModuleDescription(null!, OciArtifactReference);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetDescription_WithManifestFileAndNoAnnotations_ShouldReturnNull()
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
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = await OciArtifactRegistry.TryGetModuleDescription(null!, OciArtifactReference);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetDescription_WithManifestFileAndJustDocumentationUri_ShouldReturnNull()
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
       ""annotations"": {
         ""org.opencontainers.image.documentation"": ""https://github.com/Azure/bicep-registry-modules/blob/main/modules/samples/hello-world/README.md""
       }
     }
   ]
 }";
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = await OciArtifactRegistry.TryGetModuleDescription(null!, OciArtifactReference);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetDescription_WithValidDescriptionInManifestFile_ShouldReturnDescription()
        {
            var description = @"My description is this: https://github.com/Azure/bicep-registry-modules/blob/main/modules/samples/hello-world/README.md";
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
     ""org.opencontainers.image.description"": """ + description + @"""
   }
 }";
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = await OciArtifactRegistry.TryGetModuleDescription(null!, OciArtifactReference);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(description);
        }

        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public async Task GetDescription_WithAnnotationsInManifestFileAndInvalidDescription_ShouldReturnNull(string description)
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
     ""org.opencontainers.image.description"": """ + description + @"""
   }
 }";
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = await OciArtifactRegistry.TryGetModuleDescription(null!, OciArtifactReference);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetDescription_WithValidDescriptionAndDocumentationUriInManifestFile_ShouldReturnDescriptionAndDocumentationUri()
        {
            var documentationUri = @"https://github.com/Azure/bicep-registry-modules/blob/main/modules/samples/hello-world/README.md";
            var description = "This is my \\\"description\\\"";
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
     ""org.opencontainers.image.documentation"": """ + documentationUri + @""",
     ""org.opencontainers.image.description"": """ + description + @"""
   }
 }";
            (OciArtifactRegistry OciArtifactRegistry, OciArtifactReference OciArtifactReference) = CreateModuleRegistryWithCachedModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var actualDocumentationUri = OciArtifactRegistry.TryGetDocumentationUri(OciArtifactReference);

            actualDocumentationUri.Should().NotBeNull();
            actualDocumentationUri.Should().BeEquivalentTo(documentationUri);

            var actualDescription = await OciArtifactRegistry.TryGetModuleDescription(null!, OciArtifactReference);

            actualDescription.Should().NotBeNull();
            actualDescription.Should().BeEquivalentTo(description.Replace("\\", "")); // unencode json
        }

        #endregion GetDescription

        #region Publish

        private const string jsonContentsV1 = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.19.5.34762"",
      ""templateHash"": ""6661241730999253120""
    }
  },
  ""resources"": [],
  ""outputs"": {
    ""myOutput"": {
      ""type"": ""string"",
      ""value"": ""hello!""
    }
  }
}";
        private const string jsonContentsV2 = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.19.5.34762"",
      ""templateHash"": ""6661241730999253120""
    }
  },
  ""resources"": [],
  ""outputs"": {
    ""myOutput"": {
      ""type"": ""string"",
      ""value"": ""hello! V2""
    }
  }
}";

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task PublishModuleWithSource_ShouldHaveSource(bool publishSource)
        {
            string registry = "myregistry.azurecr.io";
            string repository = "bicep/myrepo";

            var (ociRegistry, blobClient, bicepFile) = CreateModuleRegistryAndBicepFile(null);

            var moduleReference = CreateModuleReference(bicepFile, registry, repository, "v1", null);

            var template = BinaryData.FromString(jsonContentsV1);
            var sources = publishSource ? BinaryData.FromString("This is a test. This is only a test. If this were a real source archive, it would have been binary.") : null;

            await ociRegistry.PublishModule(moduleReference, template, sources, "http://documentation", "description");

            if (publishSource)
            {
                blobClient.Should().HaveModuleWithSource("v1", template, sources);
            }
            else
            {
                blobClient.Should().HaveModuleWithNoSource("v1", template);
            }
        }

        [DataTestMethod]
        // No sources at all
        [DataRow(jsonContentsV1, null, jsonContentsV2, null)]
        // Sources for only one version
        [DataRow(jsonContentsV1, "sources v1", jsonContentsV2, null)]
        [DataRow(jsonContentsV1, null, jsonContentsV2, "sources v2")]
        // Sources for both versions
        [DataRow(jsonContentsV1, "sources v1", jsonContentsV2, "sources v2")]
        // Template changed, but sources did not (perhaps imported text source changed)
        [DataRow(jsonContentsV1, "sources", jsonContentsV2, "sources")]
        // Sources changed, but compiled template did not
        [DataRow(jsonContentsV1, "sources v1", jsonContentsV1, "sources v2")]
        public async Task PublishArtifactWithSource_AtMultipleVersions_ShouldHaveRespectivelyPublishedSource(string jsonContentsV1, string? sourceContentsV1, string jsonContentsV2, string? sourceContentsV2)
        {
            string registry = "myregistry.azurecr.io";
            string repository = "bicep/myrepo";
            var (ociRegistry, blobClient, bicepFile) = CreateModuleRegistryAndBicepFile(null);
            var moduleReferenceV1 = CreateModuleReference(bicepFile, registry, repository, "v1", null);
            var moduleReferenceV2 = CreateModuleReference(bicepFile, registry, repository, "v2", null);

            var templateV1 = BinaryData.FromString(jsonContentsV1);
            var sourcesV1 = sourceContentsV1 == null ? null : BinaryData.FromString(sourceContentsV1);
            await ociRegistry.PublishModule(moduleReferenceV1, templateV1, sourcesV1, "http://documentation", "description");

            var templateV2 = BinaryData.FromString(jsonContentsV2);
            var sourcesV2 = sourceContentsV2 == null ? null : BinaryData.FromString(sourceContentsV2);
            await ociRegistry.PublishModule(moduleReferenceV2, templateV2, sourcesV2, "http://documentation", "description");

            if (sourcesV1 != null)
            {
                blobClient.Should().HaveModuleWithSource("v1", templateV1, sourcesV1);
            }
            else
            {
                blobClient.Should().HaveModuleWithNoSource("v1", templateV1);
            }

            if (sourcesV2 != null)
            {
                blobClient.Should().HaveModuleWithSource("v2", templateV2, sourcesV2);
            }
            else
            {
                blobClient.Should().HaveModuleWithNoSource("v2", templateV2);
            }
        }

        #endregion

        #region Pull modules

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task RestoreModuleWithSource_ShouldRestoreSourceToDisk(bool publishSource)
        {
            string registry = "myregistry.azurecr.io";
            string repository = "bicep/myrepo";

            var (ociRegistry, blobClient, bicepFile) = CreateModuleRegistryAndBicepFile(null);

            var moduleReference = CreateModuleReference(bicepFile, registry, repository, "v1", null);

            var template = BinaryData.FromString(jsonContentsV1);

            var featureProviderFactoryMock = StrictMock.Of<IFeatureProviderFactory>();
            featureProviderFactoryMock.Setup(x => x.GetFeatureProvider(bicepFile.Uri)).Returns(bicepFile.Features);

            BinaryData? sources = null;
            if (publishSource)
            {
                var uri = InMemoryFileResolver.GetFileUri("/path/to/bicep.bicep");
                var sourceFileFactory = new SourceFileFactory(BicepTestConstants.ConfigurationManager, featureProviderFactoryMock.Object);
                sources = new SourceArchiveBuilder(sourceFileFactory)
                    .WithBicepFile(uri, "// contents")
                    .BuildBinaryData();
            }

            await ociRegistry.PublishModule(moduleReference, template, sources, "http://documentation", "description");

            if (publishSource)
            {
                blobClient.Should().HaveModuleWithSource("v1", template, sources);
            }
            else
            {
                blobClient.Should().HaveModuleWithNoSource("v1", template);
            }

            await RestoreModule(ociRegistry, moduleReference);

            var modules = CachedModules.GetCachedModules(BicepTestConstants.FileSystem, bicepFile.Features.CacheRootDirectory);
            modules.Should().HaveCountGreaterThan(0);

            if (publishSource)
            {
                modules.Should().AllSatisfy(m => m.HasSourceLayer.Should().Be(publishSource));
            }

            var actualSourceResult = ociRegistry.TryGetSource(moduleReference);

            if (sources is { })
            {
                actualSourceResult.UnwrapOrThrow().Should().BeEquivalentTo(SourceArchive.UnpackFromStream(sources.ToStream()).UnwrapOrThrow());
            }
            else
            {
                actualSourceResult.IsSuccess().Should().BeFalse();
            }
        }

        #endregion

        #region Helpers

        private async Task RestoreModule(OciArtifactRegistry ociRegistry, OciArtifactReference reference)
        {
            var (_, failureBuilder) = (await ociRegistry.RestoreArtifacts(new[] { reference })).SingleOrDefault();
            if (failureBuilder is { })
            {
                var builder = new DiagnosticBuilderInternal(new Core.Parsing.TextSpan());
                var diagnostic = failureBuilder(builder);
                if (diagnostic is { })
                {
                    throw new Exception(diagnostic.Message);
                }
            }
        }

        private OciArtifactReference CreateModuleReference(BicepSourceFile referencingFile, string registry, string repository, string? tag, string? digest)
        {
            OciArtifactReference.TryParse(referencingFile, ArtifactType.Module, null, $"{registry}/{repository}:{tag}").IsSuccess(out var moduleReference).Should().BeTrue();
            return moduleReference!;
        }

        private (OciArtifactRegistry, FakeRegistryBlobClient, BicepFile parentModuleFile) CreateModuleRegistryAndBicepFile(
            string? parentBicepFileContents // The bicep file which references a module
        )
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", parentBicepFileContents ?? "", TestOutputPath);
            var parentModuleUri = new Uri(bicepPath);

            var featureProviderMock = StrictMock.Of<IFeatureProvider>();
            var cacheRootDirectory = BicepTestConstants.FileExplorer.GetDirectory(IOUri.FromLocalFilePath(TestOutputPath));
            featureProviderMock.Setup(m => m.CacheRootDirectory).Returns(cacheRootDirectory);

            var featureProviderFactoryMock = StrictMock.Of<IFeatureProviderFactory>();
            featureProviderFactoryMock.Setup(m => m.GetFeatureProvider(parentModuleUri)).Returns(featureProviderMock.Object);

            var parentModuleFile = new BicepFile(
                parentModuleUri,
                [],
                SyntaxFactory.EmptyProgram,
                BicepTestConstants.ConfigurationManager,
                featureProviderFactoryMock.Object,
                EmptyDiagnosticLookup.Instance,
                EmptyDiagnosticLookup.Instance);

            var (registry, blobClient) = OciRegistryHelper.CreateModuleRegistry();
            return (registry, blobClient, parentModuleFile);
        }

        // Creates a new (real) OciArtifactRegistry and sets it up so that it has an on-disk cached module
        //   reference as if it had been restored from the registry. This can be used to test scenarios
        //   where a module has already been restored.
        private (OciArtifactRegistry, OciArtifactReference) CreateModuleRegistryWithCachedModuleReference(
            string parentBicepFileContents, // The bicep file which references the module
            string? manifestFileContents,
            string registry,
            string repository,
            string? digest = null,
            string? tag = null)
        {
            var (OciArtifactRegistry, _, parentModuleFile) = CreateModuleRegistryAndBicepFile(parentBicepFileContents);

            OciArtifactReference? OciArtifactReference = OciRegistryHelper.CreateModuleReferenceMock(
                parentModuleFile,
                registry,
                repository,
                digest,
                tag);

            if (manifestFileContents is not null)
            {
                OciRegistryHelper.SaveManifestFileToModuleRegistryCache(
                    TestContext,
                    registry,
                    repository,
                    manifestFileContents,
                    TestOutputPath,
                    digest,
                    tag);
            }

            return (OciArtifactRegistry, OciArtifactReference);
        }
        #endregion Helpers
    }
}
