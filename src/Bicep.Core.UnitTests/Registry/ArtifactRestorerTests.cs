// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class ArtifactRestorerTests
    {
        [TestMethod]
        public async Task MockRegistries_ModuleLifecycle()
        {
            var fail = StrictMock.Of<IArtifactRegistry>();
            fail.Setup(m => m.Scheme).Returns("fail");
            fail.Setup(m => m.CanHandle(It.IsAny<ExternalArtifactReference>())).Returns(false);

            var mock = StrictMock.Of<IArtifactRegistry>();
            mock.Setup(m => m.Scheme).Returns("mock");
            mock.Setup(m => m.CanHandle(It.IsAny<ExternalArtifactReference>())).Returns(true);

            var validFile = BicepTestConstants.CreateDummyBicepFile();
            ExternalArtifactReference validRef = new MockModuleReference(validFile, "validRef");

            var validFile2 = BicepTestConstants.CreateDummyBicepFile();
            ExternalArtifactReference validRef2 = new MockModuleReference(validFile2, "validRef2");

            var validFile3 = BicepTestConstants.CreateDummyBicepFile();
            ExternalArtifactReference validRef3 = new MockModuleReference(validFile3, "validRef3");

            mock.Setup(m => m.TryGetEntryPoint(validRef)).Returns((IFileHandle?)null);
            mock.Setup(m => m.TryGetEntryPoint(validRef2)).Returns(DummyFileHandle.Default);
            mock.Setup(m => m.TryGetEntryPoint(validRef3)).Returns((IFileHandle?)null);

            mock.Setup(m => m.RestoreAsync(It.IsAny<IEnumerable<ExternalArtifactReference>>(), It.IsAny<RestoreMode>()))
                .ReturnsAsync(new Dictionary<ExternalArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>
                {
                    [validRef3] = x => new Diagnostic(x.TextSpan, DiagnosticLevel.Error, DiagnosticSource.Compiler, "RegFail", "Failed to restore module")
                });

            var restorer = CreateRestorer(fail.Object, mock.Object);

            restorer.GetRestoreState(validRef).Should().BeOfType<ArtifactRestoreState.NotRestored>();
            restorer.GetRestoreState(validRef2).Should().BeOfType<ArtifactRestoreState.Restored>().Which.EntryPoint.Should().BeSameAs(DummyFileHandle.Default);
            restorer.GetRestoreState(validRef3).Should().BeOfType<ArtifactRestoreState.NotRestored>();

            (await restorer.RestoreAsync(new[] { validRef, validRef3 }, RestoreMode.IfMissing)).Should().BeTrue();

            var failed = restorer.GetRestoreState(validRef3).Should().BeOfType<ArtifactRestoreState.RestoreFailed>().Subject;
            failed.Error.Should().HaveCode("RegFail");
            failed.Error.Should().HaveMessage("Failed to restore module");
        }

        [TestMethod]
        [DynamicData(nameof(GetConfigurationData))]
        public async Task GetModuleRestoreStatus_ConfigurationChanges_ReturnsCachedStatusWhenChangeIsIrrelevant(IBicepConfiguration changedConfiguration, Type expectedState)
        {
            var badFile = BicepTestConstants.CreateDummyBicepFile(BicepTestConstants.CreateMockConfiguration());
            var badReference = new MockModuleReference(badFile, "bad");

            var registryMock = StrictMock.Of<IArtifactRegistry>();
            registryMock.SetupGet(x => x.Scheme).Returns("mock");
            registryMock.Setup(x => x.CanHandle(It.IsAny<ExternalArtifactReference>())).Returns(true);
            registryMock.Setup(x => x.RestoreAsync(It.IsAny<IEnumerable<ExternalArtifactReference>>(), It.IsAny<RestoreMode>()))
                .ReturnsAsync(new Dictionary<ExternalArtifactReference, DiagnosticBuilderDelegate>
                {
                    [badReference] = x => new Diagnostic(x.TextSpan, DiagnosticLevel.Error, DiagnosticSource.Compiler, "RestoreFailure", "Failed to restore module.")
                });
            registryMock.Setup(x => x.TryGetEntryPoint(It.IsAny<ExternalArtifactReference>()))
                .Returns((IFileHandle?)null);

            var restorer = CreateRestorer(registryMock.Object);

            await restorer.RestoreAsync(new[] { badReference }, RestoreMode.IfMissing);

            // Act.
            badFile = BicepTestConstants.CreateDummyBicepFile(changedConfiguration);
            badReference = new MockModuleReference(badFile, "bad");

            var state = restorer.GetRestoreState(badReference);

            // Assert.
            state.Should().BeOfType(expectedState);
        }

        private static IEnumerable<object[]> GetConfigurationData()
        {
            yield return new object[]
            {
                // Irrelevant change.
                BicepTestConstants.CreateMockConfiguration(new()
                {
                    ["cloud.profiles.AzureCloud.resourceManagerEndpoint"] = "HTTPS://EXAMPLE.INVALID",
                    ["cloud.profiles.AzureCloud.activeDirectoryAuthority"] = "https://example.invalid/",
                }),
                typeof(ArtifactRestoreState.RestoreFailed)
            };

            yield return new object[]
            {
                // Irrelevant change.
                BicepTestConstants.CreateMockConfiguration(new()
                {
                    ["cloud.currentProfile"] = "MyCloud",
                    ["cloud.profiles.MyCloud.resourceManagerEndpoint"] = "HTTPS://EXAMPLE.INVALID",
                    ["cloud.profiles.MyCloud.activeDirectoryAuthority"] = "https://example.invalid/",
                }),
                typeof(ArtifactRestoreState.RestoreFailed)
            };

            yield return new object[]
            {
                // Relevant change.
                BicepTestConstants.CreateMockConfiguration(new()
                {
                    ["cloud.currentProfile"] = "MyCloud",
                    ["cloud.profiles.MyCloud.resourceManagerEndpoint"] = "https://example.invalid",
                    ["cloud.profiles.MyCloud.activeDirectoryAuthority"] = "https://foo.bar.com",
                }),
                typeof(ArtifactRestoreState.NotRestored)
            };

            yield return new object[]
            {
                // Relevant change.
                BicepTestConstants.CreateMockConfiguration(new()
                {
                    ["cloud.credentialPrecedence"] = new[] { "VisualStudioCode" },
                }),
                typeof(ArtifactRestoreState.NotRestored)
            };
        }

        [TestMethod]
        public async Task RestoreArtifacts_Forced_RetriesAModuleThatFailedRecently()
        {
            var file = BicepTestConstants.CreateDummyBicepFile(BicepTestConstants.CreateMockConfiguration());
            var reference = new MockModuleReference(file, "flaky");
            var isRestored = false;

            var registryMock = StrictMock.Of<IArtifactRegistry>();
            registryMock.SetupGet(x => x.Scheme).Returns("mock");
            registryMock.Setup(x => x.CanHandle(It.IsAny<ExternalArtifactReference>())).Returns(true);
            registryMock.SetupSequence(x => x.RestoreAsync(It.IsAny<IEnumerable<ExternalArtifactReference>>(), It.IsAny<RestoreMode>()))
                .ReturnsAsync(new Dictionary<ExternalArtifactReference, DiagnosticBuilderDelegate>
                {
                    [reference] = x => new Diagnostic(x.TextSpan, DiagnosticLevel.Error, DiagnosticSource.Compiler, "RestoreFailure", "Failed to restore module.")
                })
                .ReturnsAsync(() =>
                {
                    isRestored = true;
                    return new Dictionary<ExternalArtifactReference, DiagnosticBuilderDelegate>();
                });
            registryMock.Setup(x => x.TryGetEntryPoint(reference)).Returns(() => isRestored ? DummyFileHandle.Default : null);
            var restorer = CreateRestorer(registryMock.Object);

            await restorer.RestoreAsync([reference], RestoreMode.IfMissing);
            restorer.GetRestoreState(reference).Should().BeOfType<ArtifactRestoreState.RestoreFailed>();

            await restorer.RestoreAsync([reference], RestoreMode.Force);
            restorer.GetRestoreState(reference).Should().BeOfType<ArtifactRestoreState.Restored>();
        }

        [TestMethod]
        public async Task RestoreArtifacts_SameArtifactWithDifferentClouds_RestoresOncePerCloud()
        {
            var defaultCloudReference = new MockModuleReference(BicepTestConstants.CreateDummyBicepFile(BicepTestConstants.CreateMockConfiguration()), "shared");
            var otherCloudReference = new MockModuleReference(BicepTestConstants.CreateDummyBicepFile(BicepTestConstants.CreateMockConfiguration(new()
            {
                ["cloud.profiles.AzureCloud.resourceManagerEndpoint"] = "https://other.invalid",
            })), "shared");
            var restored = new List<ExternalArtifactReference>();

            var registryMock = StrictMock.Of<IArtifactRegistry>();
            registryMock.SetupGet(x => x.Scheme).Returns("mock");
            registryMock.Setup(x => x.CanHandle(It.IsAny<ExternalArtifactReference>())).Returns(true);
            registryMock.Setup(x => x.TryGetEntryPoint(It.IsAny<ExternalArtifactReference>())).Returns((IFileHandle?)null);
            registryMock.Setup(x => x.RestoreAsync(It.IsAny<IEnumerable<ExternalArtifactReference>>(), It.IsAny<RestoreMode>()))
                .ReturnsAsync((IEnumerable<ExternalArtifactReference> references, RestoreMode _) =>
                {
                    restored.AddRange(references);
                    return new Dictionary<ExternalArtifactReference, DiagnosticBuilderDelegate>
                    {
                        [otherCloudReference] = x => new Diagnostic(x.TextSpan, DiagnosticLevel.Error, DiagnosticSource.Compiler, "RestoreFailure", "Failed to restore module."),
                    };
                });
            var restorer = CreateRestorer(registryMock.Object);

            // The same reference written twice is restored once.
            await restorer.RestoreAsync([defaultCloudReference, otherCloudReference, otherCloudReference], RestoreMode.IfMissing);

            restored.Should().Equal(defaultCloudReference, otherCloudReference);
            restored[1].Context.Cloud.ResourceManagerEndpointUri.Should().Be(new Uri("https://other.invalid"));

            // A failure in one cloud does not affect the other.
            restorer.GetRestoreState(otherCloudReference).Should().BeOfType<ArtifactRestoreState.RestoreFailed>();
            restorer.GetRestoreState(defaultCloudReference).Should().BeOfType<ArtifactRestoreState.NotRestored>();
        }

        [TestMethod]
        [DataRow(RestoreMode.IfMissing, false)]
        [DataRow(RestoreMode.Force, true)]
        public async Task RestoreAsync_PassesTheModeToTheRegistryAndDownloadsThePublicModuleMetadata(RestoreMode mode, bool forceMetadataUpdate)
        {
            var reference = new MockModuleReference(BicepTestConstants.CreateDummyBicepFile(BicepTestConstants.CreateMockConfiguration()), "module");

            var registryMock = StrictMock.Of<IArtifactRegistry>();
            registryMock.Setup(x => x.CanHandle(It.IsAny<ExternalArtifactReference>())).Returns(true);
            registryMock.Setup(x => x.TryGetEntryPoint(reference)).Returns((IFileHandle?)null);
            registryMock.Setup(x => x.RestoreAsync(It.IsAny<IEnumerable<ExternalArtifactReference>>(), mode))
                .ReturnsAsync(new Dictionary<ExternalArtifactReference, DiagnosticBuilderDelegate>());
            var metadataProvider = CreatePublicModuleMetadataProvider();
            var restorer = new ArtifactRestorer([registryMock.Object], metadataProvider.Object);

            (await restorer.RestoreAsync([reference], mode)).Should().BeTrue();

            registryMock.Verify(x => x.RestoreAsync(It.Is<IEnumerable<ExternalArtifactReference>>(refs => refs.Single() == reference), mode), Times.Once);
            metadataProvider.Verify(x => x.TryAwaitCache(forceMetadataUpdate), Times.Once);
        }

        private static IArtifactRestorer CreateRestorer(params IArtifactRegistry[] registries) => new ArtifactRestorer(registries, CreatePublicModuleMetadataProvider().Object);

        private static Mock<IPublicModuleMetadataProvider> CreatePublicModuleMetadataProvider()
        {
            var provider = StrictMock.Of<IPublicModuleMetadataProvider>();
            provider.Setup(x => x.TryAwaitCache(It.IsAny<bool>())).Returns(Task.CompletedTask);
            return provider;
        }

        private class MockModuleReference : ExternalArtifactReference
        {
            public MockModuleReference(BicepSourceFile referencingFile, string reference)
                : base(TemplateSpecArtifactContext.Create(referencingFile.LoadConfiguration(), referencingFile.LoadFeatures()), "mock", ArtifactType.Module)
            {
                this.Reference = reference;
            }

            public string Reference { get; }

            public override string UnqualifiedReference => this.Reference;

            public override bool Equals(object? obj) => obj is MockModuleReference other && this.Reference.Equals(other.Reference) && this.Context.Equals(other.Context);

            public override int GetHashCode() => HashCode.Combine(this.Reference, this.Context);
        }

    }
}
