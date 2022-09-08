// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.Workspaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class ModuleDispatcherTests
    {
        [TestMethod]
        public void NoRegistries_AvailableSchemes_ShouldReturnEmpty()
        {
            var dispatcher = CreateDispatcher(BicepTestConstants.ConfigurationManager);
            dispatcher.AvailableSchemes(RandomFileUri()).Should().BeEmpty();
        }

        [TestMethod]
        public void NoRegistries_ValidateModuleReference_ShouldReturnError()
        {
            var module = CreateModule("fakeScheme:fakeModule");
            var dispatcher = CreateDispatcher(BicepTestConstants.ConfigurationManager);
            var configuration = BicepTestConstants.BuiltInConfiguration;

            dispatcher.TryGetModuleReference(module, RandomFileUri(), out var reference, out var failureBuilder).Should().BeFalse();
            reference.Should().BeNull();
            failureBuilder!.Should().NotBeNull();

            using (new AssertionScope())
            {
                failureBuilder!.Should().HaveCode("BCP189");
                failureBuilder!.Should().HaveMessage("Module references are not supported in this context.");
            }

            var localModule = CreateModule("test.bicep");
            dispatcher.TryGetModuleReference(localModule, RandomFileUri(), out var localModuleReference, out var localModuleFailureBuilder).Should().BeFalse();
            localModuleReference.Should().BeNull();
            failureBuilder!.Should().NotBeNull();
            using (new AssertionScope())
            {
                localModuleFailureBuilder!.Should().HaveCode("BCP189");
                localModuleFailureBuilder!.Should().HaveMessage("Module references are not supported in this context.");
            }
        }

        [TestMethod]
        public void MockRegistries_AvailableSchemes_ShouldReturnedConfiguredSchemes()
        {
            var first = StrictMock.Of<IModuleRegistry>();
            first.Setup(m => m.Scheme).Returns("first");

            var second = StrictMock.Of<IModuleRegistry>();
            second.Setup(m => m.Scheme).Returns("second");

            var dispatcher = CreateDispatcher(BicepTestConstants.ConfigurationManager, first.Object, second.Object);
            dispatcher.AvailableSchemes(RandomFileUri()).Should().BeEquivalentTo("first", "second");
        }

        [TestMethod]
        public async Task MockRegistries_ModuleLifecycle()
        {
            var fail = StrictMock.Of<IModuleRegistry>();
            fail.Setup(m => m.Scheme).Returns("fail");

            var mock = StrictMock.Of<IModuleRegistry>();
            mock.Setup(m => m.Scheme).Returns("mock");

            ErrorBuilderDelegate? @null = null;
            var configuration = BicepTestConstants.BuiltInConfiguration;

            var validRefUri = RandomFileUri();
            ModuleReference? validRef = new MockModuleReference("validRef", validRefUri);
            mock.Setup(m => m.TryParseModuleReference(null, "validRef", out validRef, out @null))
                .Returns(true);

            var validRefUri2 = RandomFileUri();
            ModuleReference? validRef2 = new MockModuleReference("validRef2", validRefUri2);
            mock.Setup(m => m.TryParseModuleReference(null, "validRef2", out validRef2, out @null))
                .Returns(true);

            var validRefUri3 = RandomFileUri();
            ModuleReference? validRef3 = new MockModuleReference("validRef3", validRefUri3);
            mock.Setup(m => m.TryParseModuleReference(null, "validRef3", out validRef3, out @null))
                .Returns(true);

            var badRefUri = RandomFileUri();
            ModuleReference? nullRef = null;
            ErrorBuilderDelegate? badRefError = x => new ErrorDiagnostic(x.TextSpan, "BCPMock", "Bad ref error");
            mock.Setup(m => m.TryParseModuleReference(null, "badRef", out nullRef, out badRefError))
                .Returns(false);

            mock.Setup(m => m.IsModuleRestoreRequired(validRef)).Returns(true);
            mock.Setup(m => m.IsModuleRestoreRequired(validRef2)).Returns(false);
            mock.Setup(m => m.IsModuleRestoreRequired(validRef3)).Returns(true);

            Uri? validRefLocalUri = new Uri("untitled://validRef");
            mock.Setup(m => m.TryGetLocalModuleEntryPointUri(validRef, out validRefLocalUri, out @null))
                .Returns(true);
            Uri? validRef3LocalUri = new Uri("untitled://validRef3");
            mock.Setup(m => m.TryGetLocalModuleEntryPointUri(validRef3, out validRef3LocalUri, out @null))
                .Returns(true);

            mock.Setup(m => m.RestoreModules(It.IsAny<IEnumerable<ModuleReference>>()))
                .ReturnsAsync(new Dictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>
                {
                    [validRef3] = x => new ErrorDiagnostic(x.TextSpan, "RegFail", "Failed to restore module")
                });

            var dispatcher = CreateDispatcher(BicepTestConstants.ConfigurationManager, fail.Object, mock.Object);

            var goodModule = CreateModule("mock:validRef");
            var goodModule2 = CreateModule("mock:validRef2");
            var goodModule3 = CreateModule("mock:validRef3");
            var badModule = CreateModule("mock:badRef");

            dispatcher.TryGetModuleReference(goodModule, validRefUri, out var @ref, out var goodValidationBuilder).Should().BeTrue();
            @ref.Should().Be(validRef);
            goodValidationBuilder!.Should().BeNull();

            dispatcher.TryGetModuleReference(badModule, badRefUri, out @ref, out var badValidationBuilder).Should().BeFalse();
            @ref.Should().BeNull();
            badValidationBuilder!.Should().NotBeNull();
            badValidationBuilder!.Should().HaveCode("BCPMock");
            badValidationBuilder!.Should().HaveMessage("Bad ref error");

            dispatcher.GetModuleRestoreStatus(validRef, out var goodAvailabilityBuilder).Should().Be(ModuleRestoreStatus.Unknown);
            goodAvailabilityBuilder!.Should().HaveCode("BCP190");
            goodAvailabilityBuilder!.Should().HaveMessage("The module with reference \"mock:validRef\" has not been restored.");

            dispatcher.GetModuleRestoreStatus(validRef2, out var goodAvailabilityBuilder2).Should().Be(ModuleRestoreStatus.Succeeded);
            goodAvailabilityBuilder2!.Should().BeNull();

            dispatcher.GetModuleRestoreStatus(validRef3, out var goodAvailabilityBuilder3).Should().Be(ModuleRestoreStatus.Unknown);
            goodAvailabilityBuilder3!.Should().HaveCode("BCP190");
            goodAvailabilityBuilder3!.Should().HaveMessage("The module with reference \"mock:validRef3\" has not been restored.");

            dispatcher.TryGetLocalModuleEntryPointUri(validRef, out var @uri, out var entryPointBuilder).Should().BeTrue();
            @uri.Should().Be(new Uri("untitled://validRef"));
            entryPointBuilder!.Should().BeNull();

            dispatcher.TryGetLocalModuleEntryPointUri(validRef3, out @uri, out var entryPointBuilder3).Should().BeTrue();
            @uri.Should().Be(new Uri("untitled://validRef3"));
            entryPointBuilder3!.Should().BeNull();

            (await dispatcher.RestoreModules(new[] { validRef, validRef3 })).Should().BeTrue();

            dispatcher.GetModuleRestoreStatus(validRef3, out var goodAvailabilityBuilder3AfterRestore).Should().Be(ModuleRestoreStatus.Failed);
            goodAvailabilityBuilder3AfterRestore!.Should().HaveCode("RegFail");
            goodAvailabilityBuilder3AfterRestore!.Should().HaveMessage("Failed to restore module");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetConfigurationData), DynamicDataSourceType.Method)]
        public async Task GetModuleRestoreStatus_ConfigurationChanges_ReturnsCachedStatusWhenChangeIsIrrelevant(RootConfiguration changedConfiguration, ModuleRestoreStatus expectedStatus)
        {
            // Arrange.
            var badReferenceUri = RandomFileUri();
            var badReference = new MockModuleReference("bad", badReferenceUri);

            var registryMock = StrictMock.Of<IModuleRegistry>();
            registryMock.SetupGet(x => x.Scheme).Returns("mock");
            registryMock.Setup(x => x.RestoreModules(It.IsAny<IEnumerable<ModuleReference>>()))
                .ReturnsAsync(new Dictionary<ModuleReference, ErrorBuilderDelegate>
                {
                    [badReference] = x => new ErrorDiagnostic(x.TextSpan, "RestoreFailure", "Failed to restore module.")
                });
            registryMock.Setup(x => x.IsModuleRestoreRequired(badReference))
                .Returns(true);

            var configManagerMock = StrictMock.Of<IConfigurationManager>();
            configManagerMock.SetupSequence(m => m.GetConfiguration(badReferenceUri))
                .Returns(BicepTestConstants.CreateMockConfiguration())
                .Returns(BicepTestConstants.CreateMockConfiguration())
                .Returns(changedConfiguration);

            var dispatcher = CreateDispatcher(configManagerMock.Object, registryMock.Object);
            var configuration = BicepTestConstants.CreateMockConfiguration();

            await dispatcher.RestoreModules(new[] { badReference });

            // Act.
            var status = dispatcher.GetModuleRestoreStatus(badReference, out _);

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
                ModuleRestoreStatus.Failed
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
                ModuleRestoreStatus.Failed
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
                ModuleRestoreStatus.Unknown
            };

            yield return new object[]
            {
                // Relevant change.
                BicepTestConstants.CreateMockConfiguration(new()
                {
                    ["cloud.credentialPrecedence"] = new[] { "VisualStudioCode" },
                }),
                ModuleRestoreStatus.Unknown
            };
        }

        private static IModuleDispatcher CreateDispatcher(IConfigurationManager configurationManager, params IModuleRegistry[] registries)
        {
            var provider = StrictMock.Of<IModuleRegistryProvider>();
            provider.Setup(m => m.Registries(It.IsAny<Uri>())).Returns(registries.ToImmutableArray());

            return new ModuleDispatcher(provider.Object, configurationManager);
        }

        private static ModuleDeclarationSyntax CreateModule(string reference)
        {
            var file = SourceFileFactory.CreateBicepFile(new System.Uri("untitled://hello"), $"module foo '{reference}' = {{}}");
            return file.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>().Single();
        }

        private static Uri RandomFileUri() => PathHelper.FilePathToFileUrl(Path.GetTempFileName());

        private class MockModuleReference : ModuleReference
        {
            public MockModuleReference(string reference, Uri parentModuleUri)
                : base("mock", parentModuleUri)
            {
                this.Reference = reference;
            }

            public string Reference { get; }

            public override string UnqualifiedReference => this.Reference;

            public override bool IsExternal => true;

            public override bool Equals(object? obj) => obj is MockModuleReference other && this.Reference.Equals(other.Reference);

            public override int GetHashCode() => this.Reference.GetHashCode();
        }
    }
}
