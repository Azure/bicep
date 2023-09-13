
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Memory;
using Moq;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class AzureContainerRegistryManagerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private OciModuleReference CreateModuleReference(string registry, string repository, string? tag, string? digest)
        {
            OciModuleReference.TryParse(null, $"{registry}/{repository}:{tag}", BicepTestConstants.BuiltInConfiguration, new Uri("file:///main.bicep")).IsSuccess(out var moduleReference).Should().BeTrue();
            return moduleReference!;
        }

        private async Task<(MockRegistryBlobClient, Mock<IContainerRegistryClientFactory>)> PublishArtifactLayersToMockClient(string tempDirectory, string registry, Uri registryUri, string repository, string? mediaType, string? artifactType, string? configContents, IEnumerable<(string mediaType, string contents)> layers)
        {
            var client = new MockRegistryBlobClient();

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
            clientFactory.Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), registryUri, repository)).Returns(client);

            var templateSpecRepositoryFactory = BicepTestConstants.TemplateSpecRepositoryFactory;

            Directory.CreateDirectory(tempDirectory);

            var containerRegistryManager = new AzureContainerRegistryManager(clientFactory.Object);

            //asdfg?
            var fs = new MockFileSystem();
            //var fr = new FileResolver(fs);
            var configurationManager = new ConfigurationManager(fs);
            var parentUri = new Uri("http://test.bicep", UriKind.Absolute);
            //var featureProvider = BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(PublishSourceEnabled: false/*asdfg*/), configurationManager)
            //    .GetFeatureProvider(parentUri);
            var configuration = configurationManager.GetConfiguration(parentUri);

            //var ociModuleRegistry = new OciModuleRegistry(fr, clientFactory.Object, featureProvider, configuration, parentUri);


            using var compiledStream = new BufferedMemoryStream();

            var moduleReference = CreateModuleReference(registry, repository, "v1", null);
            await containerRegistryManager.PushArtifactAsync(
                configuration: configuration,
                artifactReference: moduleReference,
                mediaType: mediaType,
                artifactType: artifactType,
                config: new StreamDescriptor(new TextByteArray(configContents ?? string.Empty).ToStream(), BicepMediaTypes.BicepModuleConfigV1),
                layers: (layers.Select(layer => new StreamDescriptor(TextByteArray.TextToStream(layer.contents), layer.mediaType)))
                );

            /*
             * TODO: Publish via code
             */

            return (client, clientFactory);
        }

        //private async Task<(MockRegistryBlobClient, Mock<IContainerRegistryClientFactory>)> PublishArtifactLayersToMockClientasdfg(string tempDirectory, string registry, Uri registryUri, string repository, string armTemplate, string? mediaType, string? artifactType, string? configContents, IEnumerable<(string mediaType, string contents)> layers)
        //{
        //    var client = new MockRegistryBlobClient();

        //    var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();
        //    clientFactory.Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), registryUri, repository)).Returns(client);

        //    var templateSpecRepositoryFactory = BicepTestConstants.TemplateSpecRepositoryFactory;

        //    Directory.CreateDirectory(tempDirectory);

        //    var containerRegistryManager = new AzureContainerRegistryManager(clientFactory.Object);

        //    //asdfg?
        //    var fs = new MockFileSystem();
        //    //var fr = new FileResolver(fs);
        //    var configurationManager = new ConfigurationManager(fs);
        //    var parentUri = new Uri("http://test.bicep", UriKind.Absolute);
        //    //var featureProvider = BicepTestConstants.CreateFeatureProviderFactory(new FeatureProviderOverrides(PublishSourceEnabled: false/*asdfg*/), configurationManager)
        //    //    .GetFeatureProvider(parentUri);
        //    var configuration = configurationManager.GetConfiguration(parentUri);

        //    //var ociModuleRegistry = new OciModuleRegistry(fr, clientFactory.Object, featureProvider, configuration, parentUri);


        //    using var compiledStream = new BufferedMemoryStream();
        //    OciModuleReference.TryParse(null, $"{registry}/{repository}:v1", configuration, new Uri("file:///main.bicep")).IsSuccess(out var artifactReference).Should().BeTrue();

        //    compiledStream.Write(TemplateEmitter.UTF8EncodingWithoutBom.GetBytes(armTemplate));
        //    compiledStream.Position = 0;

        //    await containerRegistryManager.PushArtifactAsync(
        //        configuration: configuration,
        //        artifactReference: artifactReference!,
        //        mediaType: mediaType,
        //        artifactType: artifactType,
        //        config: new StreamDescriptor(new TextByteArray(configContents ?? string.Empty).ToStream(), BicepMediaTypes.BicepModuleConfigV1),
        //        layers: (layers.Select((mt, contents) => new StreamDescriptor(compiledStream, mt)))
        //            .Concat(bicepSources is { } ?
        //                new StreamDescriptor[] { new StreamDescriptor(bicepSources, "application/vnd.ms.bicep.module.source.v1+zip") } :
        //                Enumerable.Empty<StreamDescriptor>())
        //            .ToArray()
        //        );

        //    /*
        //     * TODO: Publish via code
        //     */

        //    return (client, clientFactory);
        //}


        //[DataTestMethod]
        //[DynamicData(nameof(GetAllDataSets), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetTestDisplayName))]
        //public async Task Restore_ShouldSucceedWithAnonymousClient(string testName, DataSet dataSet, bool publishSource)
        //{
        //    //asdfg
        //    //TestContext.WriteLine(testName);

        //    //var clientFactory = dataSet.CreateMockRegistryClients().Object;
        //    //var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
        //    //var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
        //    //await dataSet.PublishModulesToRegistryAsync(clientFactory, publishSource);

        //    //var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);

        //    //// create client that mocks missing az or PS login
        //    //var clientWithCredentialUnavailable = StrictMock.Of<ContainerRegistryClient>();
        //    //clientWithCredentialUnavailable
        //    //    .Setup(m => m.GetManifestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        //    //    .ThrowsAsync(new CredentialUnavailableException("Mock credential unavailable exception"));

        //    //// authenticated client creation will produce a client that will fail due to missing login
        //    //// this will force fallback to the anonymous client
        //    //var clientFactoryForRestore = StrictMock.Of<IContainerRegistryClientFactory>();
        //    //clientFactoryForRestore
        //    //    .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
        //    //    .Returns(clientWithCredentialUnavailable.Object);

        //    //// anonymous client creation will redirect to the working client factory containing mock published modules
        //    //clientFactoryForRestore
        //    //    .Setup(m => m.CreateAnonymousBlobClient(It.IsAny<RootConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
        //    //    .Returns<RootConfiguration, Uri, string>(clientFactory.CreateAnonymousBlobClient);

        //    //var settings = new InvocationSettings(new(TestContext, RegistryEnabled: dataSet.HasExternalModules, PublishSourceEnabled: publishSource), clientFactoryForRestore.Object, templateSpecRepositoryFactory);
        //    //TestContext.WriteLine($"Cache root = {settings.FeatureOverrides.CacheRootDirectory}");
        //    //var (output, error, result) = await Bicep(settings, "restore", bicepFilePath);

        //    //using (new AssertionScope())
        //    //{
        //    //    result.Should().Be(0);
        //    //    output.Should().BeEmpty();
        //    //    error.Should().BeEmpty();
        //    //}

        //    //if (dataSet.HasExternalModules)
        //    //{
        //    //    // ensure something got restored
        //    //    settings.FeatureOverrides.Should().HaveValidCachedModules(withSources: publishSource);
        //    //}
        //}

        // asdfg bool publishSource
        // Validates that we can restore a module published by an older version of Bicep that had artifactType as null in the OCI manifest,
        //   or mediaType as null, or an empty config, or newer versions that have a non-empty config
        //
        //
        // No errors
        [DataTestMethod]
        [DataRow("application/vnd.oci.image.manifest.v1+json", "application/vnd.ms.bicep.module.une2xpected", null,
    // expected error:
    "Error BCP192: Unable to restore.*but found 'application/vnd.ms.bicep.module.unexpected'.*newer version of Bicep might be required")]
        [DataRow(null, null, null)]
        [DataRow(null, "application/vnd.ms.bicep.module.artifact", null)]
        [DataRow("application/vnd.oci.image.manifest.v1+json", null, null)]
        [DataRow("application/vnd.oci.image.manifest.v1+json", "application/vnd.ms.bicep.module.artifact", null)]
        // We should ignore any unrecognized layers and any data written into a module's config, for future compatibility
        // Expecting no errors
        [DataRow(null, null, "{}", null)]
        [DataRow("application/vnd.oci.image.manifest.v1+json", "application/vnd.ms.bicep.module.artifact", "{\"whatever\": \"your heart desires as long as it's JSON\"}")]
        // These are just invalid. It's possible they might change in the future, but they would have to be breaking changes,
        //   current clients can't be expected to ignore these.
        // Expecting errors
        [DataRow("application/vnd.oci.image.manifest.v1+json", "application/vnd.ms.bicep.module.unexpected", null,
            // expected error:
            "Error BCP192: Unable to restore.*but found 'application/vnd.ms.bicep.module.unexpected'.*newer version of Bicep might be required")]
        public async Task Restore_Artifacts_BackwardsAndForwardsCompatibility(string? mediaType, string? artifactType, string? configContents, string? expectedErrorRegex = null)
        {
            var registry = "example.com";
            var registryUri = new Uri("https://" + registry);
            var repository = "hello/there";
            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);

            var (client, clientFactory) = await PublishArtifactLayersToMockClient(
                tempDirectory,
                registry,
                registryUri,
                repository,
                mediaType,
                artifactType,
                configContents,
                new (string mediaType, string contents)[] { (BicepMediaTypes.BicepModuleLayerV1Json, "layer contents") });

            /* asdfg
                        var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);

                        var containerRegistryManager = new AzureContainerRegistryManager(clientFactory.Object, BicepTestConstants.Features);
                        var configuration = BicepTestConstants.BuiltInConfiguration;

                        using (var compiledStream = new BufferedMemoryStream())
                        {
                            OciArtifactModuleReference.TryParse(null, $"{registry}/{repository}:v1", configuration, new Uri("file:///main.bicep"), out var moduleReference, out _).Should().BeTrue();

                            compiledStream.Write(TemplateEmitter.UTF8EncodingWithoutBom.GetBytes(dataSet.Compiled!));
                            compiledStream.Position = 0;

                            await containerRegistryManager.PushModuleAsync(
                                configuration: configuration,
                                moduleReference: moduleReference!,
                                // intentionally setting artifactType to null to simulate a publish done by an older version of Bicep
                                artifactType: null,
                                bicepSources: publishSource ? TextBytes.TextToStream("sources") : null,
                                config: new StreamDescriptor(Stream.Null, BicepMediaTypes.BicepModuleConfigV1),
                                description: null,
                                documentationUri: null,
                                layers: new StreamDescriptor[] { new StreamDescriptor(compiledStream, BicepMediaTypes.BicepModuleLayerV1Json) });
                        }
            */

            client.BlobUploads.Should().Be(2);
            client.Manifests.Should().HaveCount(1);
            client.ManifestTags.Should().HaveCount(1);
            client.ManifestObjects.Single().Value.Layers.Should().HaveCount(1);

            string digest = client.Manifests.Single(m => m.Value.Text.Contains(BicepMediaTypes.BicepModuleConfigV1)).Key;

            var bicep = $@"
module empty 'br:{registry}/{repository}@{digest}' = {{
  name: 'empty'
}}
";

            //asdfg?
            //var restoreBicepFilePath = Path.Combine(tempDirectory, "restored.bicep");
            //File.WriteAllText(restoreBicepFilePath, bicep);

            //var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory.Object, BicepTestConstants.TemplateSpecRepositoryFactory);

            //var (output, error, result) = await Bicep(settings, "restore", restoreBicepFilePath);
            //using (new AssertionScope())
            //{
            //    output.Should().BeEmpty();

            //    if (expectedErrorRegex == null)
            //    {
            //        result.Should().Be(0);
            //        error.Should().BeEmpty();
            //    }
            //    else
            //    {
            //        result.Should().Be(1);
            //        error.Should().MatchRegex(expectedErrorRegex);
            //    }
            //}
        }

        //        [DataTestMethod]
        //        // Valid
        //        [DataRow(new string[] { BicepMediaTypes.BicepModuleLayerV1Json }, null)]
        //        // TODO: doesn't work because provider doesn't write out main.json file:
        //        //[DataRow(new string[] { BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip }, null)]
        //        [DataRow(new string[] { "unknown1", "unknown2", BicepMediaTypes.BicepModuleLayerV1Json }, null)]
        //        [DataRow(new string[] { "unknown1", BicepMediaTypes.BicepModuleLayerV1Json, "unknown2" }, null)]
        //        [DataRow(new string[] { BicepMediaTypes.BicepModuleLayerV1Json, "unknown1", "unknown2" }, null)]
        //        [DataRow(new string[] { BicepMediaTypes.BicepModuleLayerV1Json, "unknown1", "unknown1", "unknown2", "unknown2" }, null)]
        //        // TODO: doesn't work because provider doesn't write out main.json file:
        //        // [DataRow(new string[] { "unknown", BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip }, null)]
        //        //
        //        // Invalid
        //        [DataRow(new string[] { }, "Expected at least one layer")]
        //        [DataRow(new string[] { "unknown1", "unknown2" }, "Did not expect only layer media types unknown1, unknown2")]
        //        [DataRow(new string[] { BicepMediaTypes.BicepModuleLayerV1Json, BicepMediaTypes.BicepModuleLayerV1Json },
        //            $"Did not expect to find multiple layer media types of application/vnd.ms.bicep.module.layer.v1\\+json, application/vnd.ms.bicep.module.layer.v1\\+json")]
        //        [DataRow(new string[] { BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip, BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip },
        //            $"Did not expect to find multiple layer media types of application/vnd.ms.bicep.provider.layer.v1.tar\\+gzip, application/vnd.ms.bicep.provider.layer.v1.tar\\+gzip")]
        //        [DataRow(new string[] { BicepMediaTypes.BicepModuleLayerV1Json, BicepMediaTypes.BicepProviderArtifactLayerV1TarGzip },
        //            $"Did not expect to find multiple layer media types of application/vnd.ms.bicep.module.layer.v1\\+json, application/vnd.ms.bicep.provider.layer.v1.tar\\+gzip")]
        //        public async Task Restore_Artifacts_LayerMediaTypes(string[] layerMediaTypes, string expectedErrorRegex)
        //        {
        //            //asdfg
        ////            var registry = "example.com";
        ////            var registryUri = new Uri("https://" + registry);
        ////            var repository = "hello/there";
        ////            var dataSet = DataSets.Empty;
        ////            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);

        ////            var (client, clientFactory) = await PublishToMockClient(
        ////                tempDirectory,
        ////                registry,
        ////                registryUri,
        ////                repository,
        ////                dataSet,
        ////                "application/vnd.oci.image.manifest.v1+json",
        ////                "application/vnd.ms.bicep.module.artifact",
        ////                null,
        ////                layerMediaTypes);

        ////            client.Manifests.Should().HaveCount(1);
        ////            client.ManifestTags.Should().HaveCount(1);

        ////            string digest = client.Manifests.Single().Key;

        ////            var bicep = $@"
        ////module empty 'br:{registry}/{repository}@{digest}' = {{
        ////  name: 'empty'
        ////}}
        ////";

        ////            var restoreBicepFilePath = Path.Combine(tempDirectory, "restored.bicep");
        ////            File.WriteAllText(restoreBicepFilePath, bicep);

        ////            var settings = new InvocationSettings(new(TestContext, RegistryEnabled: true), clientFactory.Object, BicepTestConstants.TemplateSpecRepositoryFactory);

        ////            var (output, error, result) = await Bicep(settings, "restore", restoreBicepFilePath);
        ////            using (new AssertionScope())
        ////            {
        ////                output.Should().BeEmpty();

        ////                if (expectedErrorRegex == null)
        ////                {
        ////                    result.Should().Be(0);
        ////                    error.Should().BeEmpty();
        ////                }
        ////                else
        ////                {
        ////                    result.Should().Be(1);
        ////                    error.Should().MatchRegex(expectedErrorRegex);
        ////                }
        ////            }
        //        }

        //asdfg needed?
        //private static IEnumerable<object[]> GetAllDataSets()
        //{
        //    foreach (DataSet ds in DataSets.AllDataSets)
        //    {
        //        yield return new object[] { $"{ds.Name}, not publishing source", ds, false };
        //        yield return new object[] { $"{ds.Name}, publishing source", ds, true };
        //    }
        //}

        //private static IEnumerable<object[]> GetValidDataSetsWithExternalModules()
        //{
        //    foreach (DataSet ds in DataSets.AllDataSets.Where(ds => ds.IsValid && ds.HasExternalModules))
        //    {
        //        yield return new object[] { $"{ds.Name}, not publishing source", ds, false };
        //        yield return new object[] { $"{ds.Name}, publishing source", ds, true };
        //    }
        //}

        //public static string GetTestDisplayName(MethodInfo _, object[] objects)
        //{
        //    return (string)objects[0];
        //}
    }
}
