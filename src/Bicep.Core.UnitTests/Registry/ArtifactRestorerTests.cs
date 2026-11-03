// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
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
            fail.Setup(x => x.OnRestoreArtifacts(It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            var mock = StrictMock.Of<IArtifactRegistry>();
            mock.Setup(m => m.Scheme).Returns("mock");
            mock.Setup(x => x.OnRestoreArtifacts(It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            var validFile = BicepTestConstants.CreateDummyBicepFile();
            ArtifactReference validRef = new MockModuleReference(validFile, "validRef");

            var validFile2 = BicepTestConstants.CreateDummyBicepFile();
            ArtifactReference validRef2 = new MockModuleReference(validFile2, "validRef2");

            var validFile3 = BicepTestConstants.CreateDummyBicepFile();
            ArtifactReference validRef3 = new MockModuleReference(validFile3, "validRef3");

            mock.Setup(m => m.IsArtifactRestoreRequired(validRef)).Returns(true);
            mock.Setup(m => m.IsArtifactRestoreRequired(validRef2)).Returns(false);
            mock.Setup(m => m.IsArtifactRestoreRequired(validRef3)).Returns(true);

            mock.Setup(m => m.RestoreArtifacts(It.IsAny<IEnumerable<ArtifactReference>>()))
                .ReturnsAsync(new Dictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>
                {
                    [validRef3] = x => new Diagnostic(x.TextSpan, DiagnosticLevel.Error, DiagnosticSource.Compiler, "RegFail", "Failed to restore module")
                });

            var restorer = CreateRestorer(fail.Object, mock.Object);

            restorer.GetRestoreStatus(validRef, out var goodAvailabilityBuilder).Should().Be(ArtifactRestoreStatus.Unknown);
            goodAvailabilityBuilder!.Should().HaveCode("BCP190");
            goodAvailabilityBuilder!.Should().HaveMessage("The artifact with reference \"mock:validRef\" has not been restored.");

            restorer.GetRestoreStatus(validRef2, out var goodAvailabilityBuilder2).Should().Be(ArtifactRestoreStatus.Succeeded);
            goodAvailabilityBuilder2!.Should().BeNull();

            restorer.GetRestoreStatus(validRef3, out var goodAvailabilityBuilder3).Should().Be(ArtifactRestoreStatus.Unknown);
            goodAvailabilityBuilder3!.Should().HaveCode("BCP190");
            goodAvailabilityBuilder3!.Should().HaveMessage("The artifact with reference \"mock:validRef3\" has not been restored.");

            (await restorer.RestoreArtifacts(new[] { validRef, validRef3 }, false)).Should().BeTrue();

            restorer.GetRestoreStatus(validRef3, out var goodAvailabilityBuilder3AfterRestore).Should().Be(ArtifactRestoreStatus.Failed);
            goodAvailabilityBuilder3AfterRestore!.Should().HaveCode("RegFail");
            goodAvailabilityBuilder3AfterRestore!.Should().HaveMessage("Failed to restore module");
        }

        [TestMethod]
        [DynamicData(nameof(GetConfigurationData))]
        public async Task GetModuleRestoreStatus_ConfigurationChanges_ReturnsCachedStatusWhenChangeIsIrrelevant(IBicepConfiguration changedConfiguration, ArtifactRestoreStatus expectedStatus)
        {
            var badFile = BicepTestConstants.CreateDummyBicepFile(BicepTestConstants.CreateMockConfiguration());
            var badReference = new MockModuleReference(badFile, "bad");

            var registryMock = StrictMock.Of<IArtifactRegistry>();
            registryMock.SetupGet(x => x.Scheme).Returns("mock");
            registryMock.Setup(x => x.RestoreArtifacts(It.IsAny<IEnumerable<ArtifactReference>>()))
                .ReturnsAsync(new Dictionary<ArtifactReference, DiagnosticBuilderDelegate>
                {
                    [badReference] = x => new Diagnostic(x.TextSpan, DiagnosticLevel.Error, DiagnosticSource.Compiler, "RestoreFailure", "Failed to restore module.")
                });
            registryMock.Setup(x => x.IsArtifactRestoreRequired(badReference))
                .Returns(true);
            registryMock.Setup(x => x.OnRestoreArtifacts(It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            var restorer = CreateRestorer(registryMock.Object);

            await restorer.RestoreArtifacts(new[] { badReference }, false);

            // Act.
            badFile = BicepTestConstants.CreateDummyBicepFile(changedConfiguration);
            badReference = new MockModuleReference(badFile, "bad");

            var status = restorer.GetRestoreStatus(badReference, out _);

            // Assert.
            status.Should().Be(expectedStatus);
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
                ArtifactRestoreStatus.Failed
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
                ArtifactRestoreStatus.Failed
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
                ArtifactRestoreStatus.Unknown
            };

            yield return new object[]
            {
                // Relevant change.
                BicepTestConstants.CreateMockConfiguration(new()
                {
                    ["cloud.credentialPrecedence"] = new[] { "VisualStudioCode" },
                }),
                ArtifactRestoreStatus.Unknown
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
            registryMock.SetupSequence(x => x.RestoreArtifacts(It.IsAny<IEnumerable<ArtifactReference>>()))
                .ReturnsAsync(new Dictionary<ArtifactReference, DiagnosticBuilderDelegate>
                {
                    [reference] = x => new Diagnostic(x.TextSpan, DiagnosticLevel.Error, DiagnosticSource.Compiler, "RestoreFailure", "Failed to restore module.")
                })
                .ReturnsAsync(() =>
                {
                    isRestored = true;
                    return new Dictionary<ArtifactReference, DiagnosticBuilderDelegate>();
                });
            registryMock.Setup(x => x.InvalidateArtifactsCache(It.IsAny<IEnumerable<ArtifactReference>>()))
                .ReturnsAsync(new Dictionary<ArtifactReference, DiagnosticBuilderDelegate>());
            registryMock.Setup(x => x.IsArtifactRestoreRequired(reference)).Returns(() => !isRestored);
            registryMock.Setup(x => x.OnRestoreArtifacts(It.IsAny<bool>())).Returns(Task.CompletedTask);
            var restorer = CreateRestorer(registryMock.Object);

            await restorer.RestoreArtifacts([reference], forceRestore: false);
            restorer.GetRestoreStatus(reference, out _).Should().Be(ArtifactRestoreStatus.Failed);

            await restorer.RestoreArtifacts([reference], forceRestore: true);
            restorer.GetRestoreStatus(reference, out _).Should().Be(ArtifactRestoreStatus.Succeeded);
        }

        private static IArtifactRestorer CreateRestorer(params IArtifactRegistry[] registries) => new ArtifactRestorer(new MockArtifactRegistryProvider(registries));

        private class MockModuleReference : ArtifactReference
        {
            public MockModuleReference(BicepSourceFile referencingFile, string reference)
                : base(referencingFile.LoadFeatures(), referencingFile.LoadConfiguration(), "mock")
            {
                this.Reference = reference;
            }

            public string Reference { get; }

            public override string UnqualifiedReference => this.Reference;

            public override bool IsExternal => true;

            public override bool Equals(object? obj) => obj is MockModuleReference other && this.Reference.Equals(other.Reference);

            public override int GetHashCode() => this.Reference.GetHashCode();

            public override ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPointFileHandle() => new(DummyFileHandle.Default);
        }

        private class MockArtifactRegistryProvider(IEnumerable<IArtifactRegistry> registries) : ArtifactRegistryProvider(registries)
        {
        }
    }
}
