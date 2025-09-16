// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class ModuleDispatcherTests
    {
        [TestMethod]
        public void NoRegistries_ValidateModuleReference_ShouldReturnError()
        {
            var module = CreateModule("fakeScheme:fakeModule");
            var dispatcher = CreateDispatcher();

            dispatcher.TryGetArtifactReference(BicepTestConstants.DummyBicepFile, module).IsSuccess(out var reference, out var failureBuilder).Should().BeFalse();
            reference.Should().BeNull();
            failureBuilder!.Should().NotBeNull();

            using (new AssertionScope())
            {
                failureBuilder!.Should().HaveCode("BCP189");
                failureBuilder!.Should().HaveMessage("Module references are not supported in this context.");
            }

            var localModule = CreateModule("test.bicep");
            dispatcher.TryGetArtifactReference(BicepTestConstants.DummyBicepFile, localModule).IsSuccess(out var localModuleReference, out var localModuleFailureBuilder).Should().BeFalse();
            localModuleReference.Should().BeNull();
            failureBuilder!.Should().NotBeNull();
            using (new AssertionScope())
            {
                localModuleFailureBuilder!.Should().HaveCode("BCP189");
                localModuleFailureBuilder!.Should().HaveMessage("Module references are not supported in this context.");
            }
        }

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

            DiagnosticBuilderDelegate? @null = null;

            var validFile = BicepTestConstants.CreateDummyBicepFile();
            ArtifactReference? validRef = new MockModuleReference(validFile, "validRef");
            mock.Setup(m => m.TryParseArtifactReference(validFile, ArtifactType.Module, null, "validRef")).Returns(ResultHelper.Create(validRef, @null));

            var validFile2 = BicepTestConstants.CreateDummyBicepFile();
            ArtifactReference? validRef2 = new MockModuleReference(validFile2, "validRef2");
            mock.Setup(m => m.TryParseArtifactReference(validFile2, ArtifactType.Module, null, "validRef2")).Returns(ResultHelper.Create(validRef2, @null));

            var validFile3 = BicepTestConstants.CreateDummyBicepFile();
            ArtifactReference? validRef3 = new MockModuleReference(validFile3, "validRef3");
            mock.Setup(m => m.TryParseArtifactReference(validFile3, ArtifactType.Module, null, "validRef3")).Returns(ResultHelper.Create(validRef3, @null));

            var badFile = BicepTestConstants.CreateDummyBicepFile();
            ArtifactReference? nullRef = null;
            DiagnosticBuilderDelegate? badRefError = x => new Diagnostic(x.TextSpan, DiagnosticLevel.Error, DiagnosticSource.Compiler, "BCPMock", "Bad ref error");
            mock.Setup(m => m.TryParseArtifactReference(badFile, ArtifactType.Module, null, "badRef")).Returns(ResultHelper.Create(nullRef, badRefError));

            mock.Setup(m => m.IsArtifactRestoreRequired(validRef)).Returns(true);
            mock.Setup(m => m.IsArtifactRestoreRequired(validRef2)).Returns(false);
            mock.Setup(m => m.IsArtifactRestoreRequired(validRef3)).Returns(true);

            Uri? validRefLocalUri = new("untitled://validRef");
            Uri? validRef3LocalUri = new("untitled://validRef3");

            mock.Setup(m => m.RestoreArtifacts(It.IsAny<IEnumerable<ArtifactReference>>()))
                .ReturnsAsync(new Dictionary<ArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>
                {
                    [validRef3] = x => new Diagnostic(x.TextSpan, DiagnosticLevel.Error, DiagnosticSource.Compiler, "RegFail", "Failed to restore module")
                });

            var dispatcher = CreateDispatcher(fail.Object, mock.Object);

            var goodModule = CreateModule("mock:validRef");
            var goodModule2 = CreateModule("mock:validRef2");
            var goodModule3 = CreateModule("mock:validRef3");
            var badModule = CreateModule("mock:badRef");

            dispatcher.TryGetArtifactReference(validFile, goodModule).IsSuccess(out var @ref, out var goodValidationBuilder).Should().BeTrue();
            @ref.Should().Be(validRef);
            goodValidationBuilder!.Should().BeNull();

            dispatcher.TryGetArtifactReference(badFile, badModule).IsSuccess(out @ref, out var badValidationBuilder).Should().BeFalse();
            @ref.Should().BeNull();
            badValidationBuilder!.Should().NotBeNull();
            badValidationBuilder!.Should().HaveCode("BCPMock");
            badValidationBuilder!.Should().HaveMessage("Bad ref error");

            dispatcher.GetArtifactRestoreStatus(validRef, out var goodAvailabilityBuilder).Should().Be(ArtifactRestoreStatus.Unknown);
            goodAvailabilityBuilder!.Should().HaveCode("BCP190");
            goodAvailabilityBuilder!.Should().HaveMessage("The artifact with reference \"mock:validRef\" has not been restored.");

            dispatcher.GetArtifactRestoreStatus(validRef2, out var goodAvailabilityBuilder2).Should().Be(ArtifactRestoreStatus.Succeeded);
            goodAvailabilityBuilder2!.Should().BeNull();

            dispatcher.GetArtifactRestoreStatus(validRef3, out var goodAvailabilityBuilder3).Should().Be(ArtifactRestoreStatus.Unknown);
            goodAvailabilityBuilder3!.Should().HaveCode("BCP190");
            goodAvailabilityBuilder3!.Should().HaveMessage("The artifact with reference \"mock:validRef3\" has not been restored.");

            (await dispatcher.RestoreArtifacts(new[] { validRef, validRef3 }, false)).Should().BeTrue();

            dispatcher.GetArtifactRestoreStatus(validRef3, out var goodAvailabilityBuilder3AfterRestore).Should().Be(ArtifactRestoreStatus.Failed);
            goodAvailabilityBuilder3AfterRestore!.Should().HaveCode("RegFail");
            goodAvailabilityBuilder3AfterRestore!.Should().HaveMessage("Failed to restore module");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetConfigurationData), DynamicDataSourceType.Method)]
        public async Task GetModuleRestoreStatus_ConfigurationChanges_ReturnsCachedStatusWhenChangeIsIrrelevant(RootConfiguration changedConfiguration, ArtifactRestoreStatus expectedStatus)
        {
            // Arrange.
            var configManagerMock = StrictMock.Of<IConfigurationManager>();
            configManagerMock.SetupSequence(m => m.GetConfiguration(It.IsAny<IOUri>()))
                .Returns(BicepTestConstants.CreateMockConfiguration())
                .Returns(BicepTestConstants.CreateMockConfiguration())
                .Returns(changedConfiguration);

            var badFile = BicepTestConstants.CreateDummyBicepFile(configManagerMock.Object);
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

            var dispatcher = CreateDispatcher(registryMock.Object);
            var configuration = BicepTestConstants.CreateMockConfiguration();

            await dispatcher.RestoreArtifacts(new[] { badReference }, false);

            // Act.
            var status = dispatcher.GetArtifactRestoreStatus(badReference, out _);

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

        private static IModuleDispatcher CreateDispatcher(params IArtifactRegistry[] registries) => new ModuleDispatcher(new MockArtifactRegistryProvider(registries));

        private static ModuleDeclarationSyntax CreateModule(string reference)
        {
            var file = BicepTestConstants.SourceFileFactory.CreateBicepFile(DummyFileHandle.Default, $"module foo '{reference}' = {{}}");
            return file.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>().Single();
        }

        private class MockModuleReference : ArtifactReference
        {
            public MockModuleReference(BicepSourceFile referencingFile, string reference)
                : base(referencingFile, "mock")
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
