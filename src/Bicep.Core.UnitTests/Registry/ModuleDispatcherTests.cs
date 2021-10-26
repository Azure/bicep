// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
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
            var dispatcher = CreateDispatcher();
            dispatcher.AvailableSchemes.Should().BeEmpty();
        }

        [TestMethod]
        public void NoRegistries_ValidateModuleReference_ShouldReturnError()
        {
            var module = CreateModule("fakeScheme:fakeModule");
            var dispatcher = CreateDispatcher();
            var configuration = BicepTestConstants.BuiltInConfiguration;

            dispatcher.TryGetModuleReference(module, configuration, out var failureBuilder).Should().BeNull();
            failureBuilder!.Should().NotBeNull();

            using (new AssertionScope())
            {
                failureBuilder!.Should().HaveCode("BCP189");
                failureBuilder!.Should().HaveMessage("Module references are not supported in this context.");
            }

            var localModule = CreateModule("test.bicep");
            dispatcher.TryGetModuleReference(localModule, configuration, out var localModuleFailureBuilder).Should().BeNull();
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

            var dispatcher = CreateDispatcher(first.Object, second.Object);
            dispatcher.AvailableSchemes.Should().BeEquivalentTo("first", "second");
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

            var validRef = new MockModuleReference("validRef");
            mock.Setup(m => m.TryParseModuleReference(null, "validRef", configuration, out @null))
                .Returns(validRef);

            var validRef2 = new MockModuleReference("validRef2");
            mock.Setup(m => m.TryParseModuleReference(null, "validRef2", configuration, out @null))
                .Returns(validRef2);

            var validRef3 = new MockModuleReference("validRef3");
            mock.Setup(m => m.TryParseModuleReference(null, "validRef3", configuration, out @null))
                .Returns(validRef3);

            ErrorBuilderDelegate? badRefError = x => new ErrorDiagnostic(x.TextSpan, "BCPMock", "Bad ref error");
            mock.Setup(m => m.TryParseModuleReference(null, "badRef", configuration, out badRefError))
                .Returns((ModuleReference?)null);

            mock.Setup(m => m.IsModuleRestoreRequired(validRef)).Returns(true);
            mock.Setup(m => m.IsModuleRestoreRequired(validRef2)).Returns(false);
            mock.Setup(m => m.IsModuleRestoreRequired(validRef3)).Returns(true);

            mock.Setup(m => m.TryGetLocalModuleEntryPointUri(It.IsAny<Uri>(), validRef, out @null))
                .Returns(new Uri("untitled://validRef"));
            mock.Setup(m => m.TryGetLocalModuleEntryPointUri(It.IsAny<Uri>(), validRef3, out @null))
                .Returns(new Uri("untitled://validRef3"));

            mock.Setup(m => m.RestoreModules(BicepTestConstants.BuiltInConfiguration, It.IsAny<IEnumerable<ModuleReference>>()))
                .ReturnsAsync(new Dictionary<ModuleReference, DiagnosticBuilder.ErrorBuilderDelegate>
                {
                    [validRef3] = x => new ErrorDiagnostic(x.TextSpan, "RegFail", "Failed to restore module")
                });

            var dispatcher = CreateDispatcher(fail.Object, mock.Object);

            var goodModule = CreateModule("mock:validRef");
            var goodModule2 = CreateModule("mock:validRef2");
            var goodModule3 = CreateModule("mock:validRef3");
            var badModule = CreateModule("mock:badRef");

            dispatcher.TryGetModuleReference(goodModule, configuration, out var goodValidationBuilder).Should().Be(validRef);
            goodValidationBuilder!.Should().BeNull();
            
            dispatcher.TryGetModuleReference(badModule, configuration, out var badValidationBuilder).Should().BeNull();
            badValidationBuilder!.Should().NotBeNull();
            badValidationBuilder!.Should().HaveCode("BCPMock");
            badValidationBuilder!.Should().HaveMessage("Bad ref error");

            dispatcher.GetModuleRestoreStatus(validRef, configuration, out var goodAvailabilityBuilder).Should().Be(ModuleRestoreStatus.Unknown);
            goodAvailabilityBuilder!.Should().HaveCode("BCP190");
            goodAvailabilityBuilder!.Should().HaveMessage("The module with reference \"mock:validRef\" has not been restored.");

            dispatcher.GetModuleRestoreStatus(validRef2, configuration, out var goodAvailabilityBuilder2).Should().Be(ModuleRestoreStatus.Succeeded);
            goodAvailabilityBuilder2!.Should().BeNull();

            dispatcher.GetModuleRestoreStatus(validRef3, configuration, out var goodAvailabilityBuilder3).Should().Be(ModuleRestoreStatus.Unknown);
            goodAvailabilityBuilder3!.Should().HaveCode("BCP190");
            goodAvailabilityBuilder3!.Should().HaveMessage("The module with reference \"mock:validRef3\" has not been restored.");

            dispatcher.TryGetLocalModuleEntryPointUri(new Uri("mock://mock"), validRef, configuration, out var entryPointBuilder).Should().Be(new Uri("untitled://validRef"));
            entryPointBuilder!.Should().BeNull();

            dispatcher.TryGetLocalModuleEntryPointUri(new Uri("mock://mock"), validRef3, configuration, out var entryPointBuilder3).Should().Be(new Uri("untitled://validRef3"));
            entryPointBuilder3!.Should().BeNull();

            (await dispatcher.RestoreModules(BicepTestConstants.BuiltInConfiguration, new[] { validRef, validRef3 })).Should().BeTrue();

            dispatcher.GetModuleRestoreStatus(validRef3, configuration, out var goodAvailabilityBuilder3AfterRestore).Should().Be(ModuleRestoreStatus.Failed);
            goodAvailabilityBuilder3AfterRestore!.Should().HaveCode("RegFail");
            goodAvailabilityBuilder3AfterRestore!.Should().HaveMessage("Failed to restore module");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetConfigurationData), DynamicDataSourceType.Method)]
        public async Task GetModuleRestoreStatus_ConfigurationChanges_ReturnsCachedStatusWhenChangeIsIrrelevant(RootConfiguration changedConfiguration, ModuleRestoreStatus expectedStatus)
        {
            // Arrange.
            var badReference = new MockModuleReference("bad");

            var registryMock = StrictMock.Of<IModuleRegistry>();
            registryMock.SetupGet(x => x.Scheme).Returns("mock");
            registryMock.Setup(x => x.RestoreModules(It.IsAny<RootConfiguration>(), It.IsAny<IEnumerable<ModuleReference>>()))
                .ReturnsAsync(new Dictionary<ModuleReference, ErrorBuilderDelegate>
                {
                    [badReference] = x => new ErrorDiagnostic(x.TextSpan, "RestoreFailure", "Failed to restore module.")
                });
            registryMock.Setup(x => x.IsModuleRestoreRequired(badReference))
                .Returns(true);

            var dispatcher = CreateDispatcher(registryMock.Object);
            var configuration = BicepTestConstants.CreateMockConfiguration();

            await dispatcher.RestoreModules(configuration, new[] { badReference });

            // Act.
            var status = dispatcher.GetModuleRestoreStatus(badReference, changedConfiguration, out _);

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

        private static IModuleDispatcher CreateDispatcher(params IModuleRegistry[] registries)
        {
            var provider = StrictMock.Of<IModuleRegistryProvider>();
            provider.Setup(m => m.Registries).Returns(registries.ToImmutableArray());

            return new ModuleDispatcher(provider.Object);
        }

        private static ModuleDeclarationSyntax CreateModule(string reference)
        {
            var file = SourceFileFactory.CreateBicepFile(new System.Uri("untitled://hello"), $"module foo '{reference}' = {{}}");
            return file.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>().Single();
        }

        private class MockModuleReference : ModuleReference
        {
            public MockModuleReference(string reference)
                : base("mock")
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
