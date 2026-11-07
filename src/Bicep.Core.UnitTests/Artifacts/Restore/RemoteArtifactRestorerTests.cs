// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Registries;
using Bicep.Core.Artifacts.Restore;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
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

namespace Bicep.Core.UnitTests.Artifacts.Restore
{
    [TestClass]
    public class RemoteArtifactRestorerTests
    {
        [TestMethod]
        public async Task MockRegistries_ModuleLifecycle()
        {
            var fail = StrictMock.Of<IRemoteArtifactRegistry>();
            fail.Setup(m => m.Scheme).Returns("fail");
            fail.Setup(m => m.CanHandle(It.IsAny<RemoteArtifactReference>())).Returns(false);

            var mock = StrictMock.Of<IRemoteArtifactRegistry>();
            mock.Setup(m => m.Scheme).Returns("mock");
            mock.Setup(m => m.CanHandle(It.IsAny<RemoteArtifactReference>())).Returns(true);

            var validFile = BicepTestConstants.CreateDummyBicepFile();
            RemoteArtifactReference validRef = new MockModuleReference(validFile, "validRef");

            var validFile2 = BicepTestConstants.CreateDummyBicepFile();
            RemoteArtifactReference validRef2 = new MockModuleReference(validFile2, "validRef2");

            var validFile3 = BicepTestConstants.CreateDummyBicepFile();
            RemoteArtifactReference validRef3 = new MockModuleReference(validFile3, "validRef3");

            mock.Setup(m => m.TryGetEntryPoint(validRef)).Returns((IFileHandle?)null);
            mock.Setup(m => m.TryGetEntryPoint(validRef2)).Returns(DummyFileHandle.Default);
            mock.Setup(m => m.TryGetEntryPoint(validRef3)).Returns((IFileHandle?)null);

            mock.Setup(m => m.RestoreAsync(It.IsAny<IEnumerable<RemoteArtifactReference>>(), It.IsAny<RestoreMode>()))
                .ReturnsAsync(new Dictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>
                {
                    [validRef3] = x => new Diagnostic(x.TextSpan, DiagnosticLevel.Error, DiagnosticSource.Compiler, "RegFail", "Failed to restore module")
                });

            var restorer = CreateRestorer(fail.Object, mock.Object);

            restorer.GetRestoreState(validRef).Should().BeOfType<RemoteArtifactRestoreState.NotRestored>();
            restorer.GetRestoreState(validRef2).Should().BeOfType<RemoteArtifactRestoreState.Restored>().Which.EntryPoint.Should().BeSameAs(DummyFileHandle.Default);
            restorer.GetRestoreState(validRef3).Should().BeOfType<RemoteArtifactRestoreState.NotRestored>();

            (await restorer.RestoreAsync(new[] { validRef, validRef3 }, RestoreMode.IfMissing)).Should().BeTrue();

            var failed = restorer.GetRestoreState(validRef3).Should().BeOfType<RemoteArtifactRestoreState.RestoreFailed>().Subject;
            failed.Error.Should().HaveCode("RegFail");
            failed.Error.Should().HaveMessage("Failed to restore module");
        }

        [TestMethod]
        [DynamicData(nameof(GetConfigurationData))]
        public async Task GetModuleRestoreStatus_ConfigurationChanges_ReturnsCachedStatusWhenChangeIsIrrelevant(IBicepConfiguration changedConfiguration, Type expectedState)
        {
            var badFile = BicepTestConstants.CreateDummyBicepFile(BicepTestConstants.CreateMockConfiguration());
            var badReference = new MockModuleReference(badFile, "bad");

            var registryMock = StrictMock.Of<IRemoteArtifactRegistry>();
            registryMock.SetupGet(x => x.Scheme).Returns("mock");
            registryMock.Setup(x => x.CanHandle(It.IsAny<RemoteArtifactReference>())).Returns(true);
            registryMock.Setup(x => x.RestoreAsync(It.IsAny<IEnumerable<RemoteArtifactReference>>(), It.IsAny<RestoreMode>()))
                .ReturnsAsync(new Dictionary<RemoteArtifactReference, DiagnosticBuilderDelegate>
                {
                    [badReference] = x => new Diagnostic(x.TextSpan, DiagnosticLevel.Error, DiagnosticSource.Compiler, "RestoreFailure", "Failed to restore module.")
                });
            registryMock.Setup(x => x.TryGetEntryPoint(It.IsAny<RemoteArtifactReference>()))
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
                typeof(RemoteArtifactRestoreState.RestoreFailed)
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
                typeof(RemoteArtifactRestoreState.RestoreFailed)
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
                typeof(RemoteArtifactRestoreState.NotRestored)
            };

            yield return new object[]
            {
                // Relevant change.
                BicepTestConstants.CreateMockConfiguration(new()
                {
                    ["cloud.credentialPrecedence"] = new[] { "VisualStudioCode" },
                }),
                typeof(RemoteArtifactRestoreState.NotRestored)
            };
        }

        [TestMethod]
        public async Task RestoreArtifacts_Forced_RetriesAModuleThatFailedRecently()
        {
            var file = BicepTestConstants.CreateDummyBicepFile(BicepTestConstants.CreateMockConfiguration());
            var reference = new MockModuleReference(file, "flaky");
            var isRestored = false;

            var registryMock = StrictMock.Of<IRemoteArtifactRegistry>();
            registryMock.SetupGet(x => x.Scheme).Returns("mock");
            registryMock.Setup(x => x.CanHandle(It.IsAny<RemoteArtifactReference>())).Returns(true);
            registryMock.SetupSequence(x => x.RestoreAsync(It.IsAny<IEnumerable<RemoteArtifactReference>>(), It.IsAny<RestoreMode>()))
                .ReturnsAsync(new Dictionary<RemoteArtifactReference, DiagnosticBuilderDelegate>
                {
                    [reference] = x => new Diagnostic(x.TextSpan, DiagnosticLevel.Error, DiagnosticSource.Compiler, "RestoreFailure", "Failed to restore module.")
                })
                .ReturnsAsync(() =>
                {
                    isRestored = true;
                    return new Dictionary<RemoteArtifactReference, DiagnosticBuilderDelegate>();
                });
            registryMock.Setup(x => x.TryGetEntryPoint(reference)).Returns(() => isRestored ? DummyFileHandle.Default : null);
            var restorer = CreateRestorer(registryMock.Object);

            await restorer.RestoreAsync([reference], RestoreMode.IfMissing);
            restorer.GetRestoreState(reference).Should().BeOfType<RemoteArtifactRestoreState.RestoreFailed>();

            await restorer.RestoreAsync([reference], RestoreMode.Force);
            restorer.GetRestoreState(reference).Should().BeOfType<RemoteArtifactRestoreState.Restored>();
        }

        [TestMethod]
        public async Task RestoreArtifacts_SameArtifactWithDifferentClouds_RestoresOncePerCloud()
        {
            var defaultCloudReference = new MockModuleReference(BicepTestConstants.CreateDummyBicepFile(BicepTestConstants.CreateMockConfiguration()), "shared");
            var otherCloudReference = new MockModuleReference(BicepTestConstants.CreateDummyBicepFile(BicepTestConstants.CreateMockConfiguration(new()
            {
                ["cloud.profiles.AzureCloud.resourceManagerEndpoint"] = "https://other.invalid",
            })), "shared");
            var restored = new List<RemoteArtifactReference>();

            var registryMock = StrictMock.Of<IRemoteArtifactRegistry>();
            registryMock.SetupGet(x => x.Scheme).Returns("mock");
            registryMock.Setup(x => x.CanHandle(It.IsAny<RemoteArtifactReference>())).Returns(true);
            registryMock.Setup(x => x.TryGetEntryPoint(It.IsAny<RemoteArtifactReference>())).Returns((IFileHandle?)null);
            registryMock.Setup(x => x.RestoreAsync(It.IsAny<IEnumerable<RemoteArtifactReference>>(), It.IsAny<RestoreMode>()))
                .ReturnsAsync((IEnumerable<RemoteArtifactReference> references, RestoreMode _) =>
                {
                    restored.AddRange(references);
                    return new Dictionary<RemoteArtifactReference, DiagnosticBuilderDelegate>
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
            restorer.GetRestoreState(otherCloudReference).Should().BeOfType<RemoteArtifactRestoreState.RestoreFailed>();
            restorer.GetRestoreState(defaultCloudReference).Should().BeOfType<RemoteArtifactRestoreState.NotRestored>();
        }

        [TestMethod]
        [DataRow(RestoreMode.IfMissing, false)]
        [DataRow(RestoreMode.Force, true)]
        public async Task RestoreAsync_PassesTheModeToTheRegistryAndDownloadsThePublicModuleMetadata(RestoreMode mode, bool forceMetadataUpdate)
        {
            var reference = new MockModuleReference(BicepTestConstants.CreateDummyBicepFile(BicepTestConstants.CreateMockConfiguration()), "module");

            var registryMock = StrictMock.Of<IRemoteArtifactRegistry>();
            registryMock.Setup(x => x.CanHandle(It.IsAny<RemoteArtifactReference>())).Returns(true);
            registryMock.Setup(x => x.TryGetEntryPoint(reference)).Returns((IFileHandle?)null);
            registryMock.Setup(x => x.RestoreAsync(It.IsAny<IEnumerable<RemoteArtifactReference>>(), mode))
                .ReturnsAsync(new Dictionary<RemoteArtifactReference, DiagnosticBuilderDelegate>());
            var metadataProvider = CreatePublicModuleMetadataProvider();
            var restorer = new RemoteArtifactRestorer([registryMock.Object], metadataProvider.Object);

            (await restorer.RestoreAsync([reference], mode)).Should().BeTrue();

            registryMock.Verify(x => x.RestoreAsync(It.Is<IEnumerable<RemoteArtifactReference>>(refs => refs.Single() == reference), mode), Times.Once);
            metadataProvider.Verify(x => x.TryAwaitCache(forceMetadataUpdate), Times.Once);
        }

        private static IRemoteArtifactRestorer CreateRestorer(params IRemoteArtifactRegistry[] registries) => new RemoteArtifactRestorer(registries, CreatePublicModuleMetadataProvider().Object);

        private static Mock<IPublicModuleMetadataProvider> CreatePublicModuleMetadataProvider()
        {
            var provider = StrictMock.Of<IPublicModuleMetadataProvider>();
            provider.Setup(x => x.TryAwaitCache(It.IsAny<bool>())).Returns(Task.CompletedTask);
            return provider;
        }

        private class MockModuleReference : RemoteArtifactReference
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
