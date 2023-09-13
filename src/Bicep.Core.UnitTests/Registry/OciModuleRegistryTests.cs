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
using Bicep.Core.Registry.Oci;

namespace Bicep.Core.UnitTests.Registry
 {
     [TestClass]
     public class OciModuleRegistryTests
     {
         [NotNull]
         public TestContext? TestContext { get; set; }

        #region PublishArtifact

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


        // >>>>>>>>>>>>>>>>>>>> RUN THIS TEST
        [DataTestMethod]
        [DataRow(true)]
        public async Task PublishArtifactWithSources_AtMultipleVersions_ShouldAttachAsdfgSourceToModuleManifest(bool publishSource)
        {
            string registry = "myregistry.azurecr.io";
            string repository = "bicep/myrepo";
            var moduleReferenceV1 = CreateModuleReference(registry, repository, "v1", null);
            var moduleReferenceV2 = CreateModuleReference(registry, repository, "v2", null);


            var (blobClient, ociRegistry) = CreateMocks();

            var templateV1 = new TextByteArray(jsonContentsV1);
            var sourcesV1 = publishSource ? new TextByteArray("This is a test. This is only a test. If this were a real source archive, it would have been binary.") : null;
            await ociRegistry.PublishArtifact(moduleReferenceV1, templateV1.ToStream(), sourcesV1?.ToStream(), "http://documentation", "description");

            var templateV2 = new TextByteArray(jsonContentsV2);
            var sourcesV2 = publishSource ? new TextByteArray("This is a test V2. This is only a test. If this were a real source archive, it would have been binary.") : null;
            await ociRegistry.PublishArtifact(moduleReferenceV2, templateV2.ToStream(), sourcesV2?.ToStream(), "http://documentation", "description");

            blobClient.Should().HaveModule("v1", templateV1.ToStream());
            blobClient.Should().HaveModule("v2", templateV2.ToStream());
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

        private OciModuleReference CreateModuleReference(string registry, string repository, string? tag, string? digest) {
            OciModuleReference.TryParse(null, $"{registry}/{repository}:{tag}", BicepTestConstants.BuiltInConfiguration, new Uri("file:///main.bicep")).IsSuccess(out var moduleReference).Should().BeTrue();
            return moduleReference!;
        }

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
