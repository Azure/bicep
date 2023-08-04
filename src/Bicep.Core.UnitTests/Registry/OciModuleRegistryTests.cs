// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Linq;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class OciModuleRegistryTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        #region GetDocumentationUri

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

            var result = ociModuleRegistry.TryGetDocumentationUri(ociArtifactModuleReference);

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

            var result = ociModuleRegistry.TryGetDocumentationUri(ociArtifactModuleReference);

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

            var result = ociModuleRegistry.TryGetDocumentationUri(ociArtifactModuleReference);

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

            var result = ociModuleRegistry.TryGetDocumentationUri(ociArtifactModuleReference);

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
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var documentation = ociModuleRegistry.TryGetDocumentationUri(ociArtifactModuleReference);
            documentation.Should().BeNull();

            var description = await ociModuleRegistry.TryGetDescription(ociArtifactModuleReference);
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
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var documentation = ociModuleRegistry.TryGetDocumentationUri(ociArtifactModuleReference);
            documentation.Should().BeNull();

            var description = await ociModuleRegistry.TryGetDescription(ociArtifactModuleReference);
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
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = ociModuleRegistry.TryGetDocumentationUri(ociArtifactModuleReference);

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

            var result = ociModuleRegistry.TryGetDocumentationUri(ociArtifactModuleReference);

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
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = ociModuleRegistry.TryGetDocumentationUri(ociArtifactModuleReference);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetDescription_WithNonExistentManifestFile_ShouldReturnNull()
        {
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                "some_manifest_text",
                "test.azurecr.io",
                "bicep/modules/storage",
                digest: "sha:12345",
                cacheRootDirectory: false);

            var result = await ociModuleRegistry.TryGetDescription(ociArtifactModuleReference);

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
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = await ociModuleRegistry.TryGetDescription(ociArtifactModuleReference);

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
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = await ociModuleRegistry.TryGetDescription(ociArtifactModuleReference);

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
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = await ociModuleRegistry.TryGetDescription(ociArtifactModuleReference);

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
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var result = await ociModuleRegistry.TryGetDescription(ociArtifactModuleReference);

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
            (OciModuleRegistry ociModuleRegistry, OciArtifactModuleReference ociArtifactModuleReference) = GetOciModuleRegistryAndOciArtifactModuleReference(
                "output myOutput string = 'hello!'",
                manifestFileContents,
                "test.azurecr.io",
                "bicep/modules/storage",
                "sha:12345");

            var actualDocumentationUri = ociModuleRegistry.TryGetDocumentationUri(ociArtifactModuleReference);

            actualDocumentationUri.Should().NotBeNull();
            actualDocumentationUri.Should().BeEquivalentTo(documentationUri);

            var actualDescription = await ociModuleRegistry.TryGetDescription(ociArtifactModuleReference);

            actualDescription.Should().NotBeNull();
            actualDescription.Should().BeEquivalentTo(description.Replace("\\", "")); // unencode json
        }

        #endregion GetDescription

        #region PublishModule

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
        public async Task PublishModuleWithSources_ShouldAttachSourceToModuleManifest(bool publishSource)
        {
            string registry = "myregistry.azurecr.io";
            string repository = "bicep/myrepo";
            var moduleReference = new OciArtifactModuleReference(registry, repository, "v1", null, new Uri("file://fakebicepfile.bicep", UriKind.Absolute));

            var (blobClient, ociRegistry) = CreateMocks();

            var template = new TextBytes(jsonContentsV1);
            var sources = publishSource ? new TextBytes("This is a test. This is only a test. If this were a real source archive, it would have been binary.") : null;

            await ociRegistry.PublishModule(moduleReference, template.ToStream(), sources?.ToStream(), "http://documentation", "description");

            blobClient.Should().HaveModule("v1", template.ToStream());
            if (publishSource)
            {
                blobClient.Should().HaveSourceAttachedToModule("v1", sources!.ToArray());
            }
            else
            {
                blobClient.Should().NotHaveAnyAttachments("v1");
            }
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task PublishModuleWithSources_AtMultipleVersions_ShouldAttachSourceToModuleManifest(bool publishSource)
        {
            string registry = "myregistry.azurecr.io";
            string repository = "bicep/myrepo";
            var moduleReferenceV1 = new OciArtifactModuleReference(registry, repository, "v1", null, new Uri("file://fakebicepfile.bicep", UriKind.Absolute));
            var moduleReferenceV2 = new OciArtifactModuleReference(registry, repository, "v2", null, new Uri("file://fakebicepfile.bicep", UriKind.Absolute));


            var (blobClient, ociRegistry) = CreateMocks();

            var templateV1 = new TextBytes(jsonContentsV1);
            var sourcesV1 = publishSource ? new TextBytes("This is a test. This is only a test. If this were a real source archive, it would have been binary.") : null;
            await ociRegistry.PublishModule(moduleReferenceV1, templateV1.ToStream(), sourcesV1?.ToStream(), "http://documentation", "description");

            var templateV2 = new TextBytes(jsonContentsV2);
            var sourcesV2 = publishSource ? new TextBytes("This is a test V2. This is only a test. If this were a real source archive, it would have been binary.") : null;
            await ociRegistry.PublishModule(moduleReferenceV2, templateV2.ToStream(), sourcesV2?.ToStream(), "http://documentation", "description");

            blobClient.Should().HaveModule("v1", templateV1.ToStream());
            blobClient.Should().HaveModule("v2", templateV2.ToStream());

            if (publishSource)
            {
                blobClient.Should().HaveSourceAttachedToModule("v1", sourcesV1!.ToArray());
                blobClient.Should().HaveSourceAttachedToModule("v2", sourcesV2!.ToArray());
            }
            else
            {
                blobClient.Should().NotHaveAnyAttachments("v1");
                blobClient.Should().NotHaveAnyAttachments("v2");
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
        public async Task PublishModuleWithSources_AtMultipleVersions_ShouldAttachSourceToModuleManifest(string jsonContentsV1, string? sourceContentsV1, string jsonContentsV2, string? sourceContentsV2)
        {
            string registry = "myregistry.azurecr.io";
            string repository = "bicep/myrepo";
            var moduleReferenceV1 = new OciArtifactModuleReference(registry, repository, "v1", null, new Uri("file://fakebicepfile.bicep", UriKind.Absolute));
            var moduleReferenceV2 = new OciArtifactModuleReference(registry, repository, "v2", null, new Uri("file://fakebicepfile.bicep", UriKind.Absolute));


            var (blobClient, ociRegistry) = CreateMocks();

            var templateV1 = new TextBytes(jsonContentsV1);
            var sourcesV1 = sourceContentsV1 == null ? null : new TextBytes(sourceContentsV1);
            await ociRegistry.PublishModule(moduleReferenceV1, templateV1.ToStream(), sourcesV1?.ToStream(), "http://documentation", "description");

            var templateV2 = new TextBytes(jsonContentsV2);
            var sourcesV2 = sourceContentsV2 == null ? null : new TextBytes(sourceContentsV2);
            await ociRegistry.PublishModule(moduleReferenceV2, templateV2.ToStream(), sourcesV2?.ToStream(), "http://documentation", "description");

            if (sourcesV1 != null)
            {
                blobClient.Should().HaveModuleWithSource("v1", templateV1.ToStream(), sourcesV1.ToArray());
            }
            else
            {
                blobClient.Should().HaveModuleWithoutSource("v1", templateV1.ToStream());
            }
            if (sourcesV2 != null)
            {
                blobClient.Should().HaveModuleWithSource("v2", templateV1.ToStream(), sourcesV2.ToArray());
            }
            else
            {
                blobClient.Should().HaveModuleWithoutSource("v2", templateV2.ToStream());
            }
        }

        private (
            MockRegistryBlobClient blobClient,
            OciModuleRegistry ociModuleRegistry
        ) CreateMocks()
        {
            IContainerRegistryClientFactory ClientFactory = StrictMock.Of<IContainerRegistryClientFactory>().Object;

            var blobClient = new MockRegistryBlobClient();
            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns(blobClient);

            var ociModuleRegistry = CreateOciModuleRegistry(new Uri("file:///caller.bicep", UriKind.Absolute), null, clientFactory.Object);

            return (blobClient, ociModuleRegistry);
        }

        #endregion

        #region Helpers

        private OciModuleRegistry CreateOciModuleRegistry(
            Uri parentModuleUri,
            string? cacheRootDirectory,
            IContainerRegistryClientFactory? containerRegistryClientFactory = null)
        {
            return new OciModuleRegistry(
                BicepTestConstants.FileResolver,
                containerRegistryClientFactory ?? BicepTestConstants.ClientFactory,
                GetFeatures(cacheRootDirectory is not null, cacheRootDirectory ?? string.Empty),
                BicepTestConstants.BuiltInConfiguration,
                parentModuleUri);
        }

        private (OciModuleRegistry, OciArtifactModuleReference) GetOciModuleRegistryAndOciArtifactModuleReference(
            string parentBicepFileContents, // The bicep file which references the module
            string manifestFileContents,
            string registory,
            string repository,
            string? digest = null,
            string? tag = null,
            bool cacheRootDirectory = true,
            IContainerRegistryClientFactory? containerRegistryClientFactory = null)
        {
            string testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepPath = FileHelper.SaveResultFile(TestContext, "input.bicep", parentBicepFileContents, testOutputPath);
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

            var ociModuleRegistry = CreateOciModuleRegistry(
                parentModuleUri,
                cacheRootDirectory ? testOutputPath : null,
                containerRegistryClientFactory);

            return (ociModuleRegistry, ociArtifactModuleReference);
        }

        private IFeatureProvider GetFeatures(bool cacheRootDirectory, string rootDirectory)
        {
            var features = StrictMock.Of<IFeatureProvider>();

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

        #endregion Helpers
    }
}
