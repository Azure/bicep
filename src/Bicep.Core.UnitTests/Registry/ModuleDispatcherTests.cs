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
using Microsoft.Extensions.Configuration;
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
            mock.Setup(m => m.TryParseModuleReference("validRef", configuration, out @null))
                .Returns(validRef);

            var validRef2 = new MockModuleReference("validRef2");
            mock.Setup(m => m.TryParseModuleReference("validRef2", configuration, out @null))
                .Returns(validRef2);

            var validRef3 = new MockModuleReference("validRef3");
            mock.Setup(m => m.TryParseModuleReference("validRef3", configuration, out @null))
                .Returns(validRef3);

            ErrorBuilderDelegate? badRefError = x => new ErrorDiagnostic(x.TextSpan, "BCPMock", "Bad ref error");
            mock.Setup(m => m.TryParseModuleReference("badRef", configuration, out badRefError))
                .Returns((ModuleReference?)null);

            mock.Setup(m => m.IsModuleRestoreRequired(validRef)).Returns(true);
            mock.Setup(m => m.IsModuleRestoreRequired(validRef2)).Returns(false);
            mock.Setup(m => m.IsModuleRestoreRequired(validRef3)).Returns(true);

            mock.Setup(m => m.TryGetLocalModuleEntryPointUri(It.IsAny<Uri>(), validRef, out @null))
                .Returns(new Uri("untitled://validRef"));
            mock.Setup(m => m.TryGetLocalModuleEntryPointUri(It.IsAny<Uri>(), validRef3, out @null))
                .Returns(new Uri("untitled://validRef3"));

            mock.Setup(m => m.RestoreModules(It.IsAny<IEnumerable<ModuleReference>>()))
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

            dispatcher.GetModuleRestoreStatus(validRef, out var goodAvailabilityBuilder).Should().Be(ModuleRestoreStatus.Unknown);
            goodAvailabilityBuilder!.Should().HaveCode("BCP190");
            goodAvailabilityBuilder!.Should().HaveMessage("The module with reference \"mock:validRef\" has not been restored.");

            dispatcher.GetModuleRestoreStatus(validRef2, out var goodAvailabilityBuilder2).Should().Be(ModuleRestoreStatus.Succeeded);
            goodAvailabilityBuilder2!.Should().BeNull();

            dispatcher.GetModuleRestoreStatus(validRef3, out var goodAvailabilityBuilder3).Should().Be(ModuleRestoreStatus.Unknown);
            goodAvailabilityBuilder3!.Should().HaveCode("BCP190");
            goodAvailabilityBuilder3!.Should().HaveMessage("The module with reference \"mock:validRef3\" has not been restored.");

            dispatcher.TryGetLocalModuleEntryPointUri(new Uri("mock://mock"), validRef, out var entryPointBuilder).Should().Be(new Uri("untitled://validRef"));
            entryPointBuilder!.Should().BeNull();

            dispatcher.TryGetLocalModuleEntryPointUri(new Uri("mock://mock"), validRef3, out var entryPointBuilder3).Should().Be(new Uri("untitled://validRef3"));
            entryPointBuilder3!.Should().BeNull();

            (await dispatcher.RestoreModules(new[] { validRef, validRef3 })).Should().BeTrue();

            dispatcher.GetModuleRestoreStatus(validRef3, out var goodAvailabilityBuilder3AfterRestore).Should().Be(ModuleRestoreStatus.Failed);
            goodAvailabilityBuilder3AfterRestore!.Should().HaveCode("RegFail");
            goodAvailabilityBuilder3AfterRestore!.Should().HaveMessage("Failed to restore module");
        }

        [DataTestMethod]
        [DataRow("ts/:mySpec:v1", "")]
        [DataRow("ts/foo/:mySpec:v1", "foo/")]
        [DataRow("ts/****:mySpec:v1", "****")]
        [DataRow("br/:myModule:v2", "")]
        [DataRow("br/bar   :mySpec:v1", "bar   ")]
        [DataRow("br//:mySpec:v1", "/")]
        public void TryGetModuleReference_InvalidAliasName_ReturnsNullAndSetsErrorDiagnostic(string referenceValue, string aliasName)
        {
            var templateSpecRegistryMock = StrictMock.Of<IModuleRegistry>();
            templateSpecRegistryMock.Setup(x => x.Scheme).Returns("ts");

            var ociArtifactRegistryMock = StrictMock.Of<IModuleRegistry>();
            ociArtifactRegistryMock.Setup(x => x.Scheme).Returns("br");

            var dispatcher = CreateDispatcher(templateSpecRegistryMock.Object, ociArtifactRegistryMock.Object);
            var configuration = CreateMockConfiguration(new Dictionary<string, string>(), "EmptyConfiguration");

            var reference = dispatcher.TryGetModuleReference(referenceValue, configuration, out var errorBuilder);

            reference.Should().BeNull();
            ((object?)errorBuilder).Should().NotBeNull();
            errorBuilder!.Should().HaveCode("BCP211");
            errorBuilder!.Should().HaveMessage($"The module alias name \"{aliasName}\" is invalid. Valid characters are alphanumeric, \"_\", or \"-\".");
        }

        [DataTestMethod]
        [DataRow("ts/prodRG:mySpec:v1", "BCP212", "The Template Spec module alias name \"prodRG\" does not exist in the Bicep configuration \"EmptyConfiguration\".")]
        [DataRow("br/myModulePath:myModule:v2", "BCP213", "The OCI artifact module alias name \"myModulePath\" does not exist in the Bicep configuration \"EmptyConfiguration\".")]
        public void TryGetModuleReference_AliasNameNotInConfiguration_ReturnsNullAndSetsErrorDiagnostic(string referenceValue, string expectedCode, string expectedMessage)
        {
            var templateSpecRegistryMock = StrictMock.Of<IModuleRegistry>();
            templateSpecRegistryMock.Setup(x => x.Scheme).Returns("ts");

            var ociArtifactRegistryMock = StrictMock.Of<IModuleRegistry>();
            ociArtifactRegistryMock.Setup(x => x.Scheme).Returns("br");

            var dispatcher = CreateDispatcher(templateSpecRegistryMock.Object, ociArtifactRegistryMock.Object);
            var configuration = CreateMockConfiguration(new Dictionary<string, string>(), "EmptyConfiguration");

            var reference = dispatcher.TryGetModuleReference(referenceValue, configuration, out var errorBuilder);

            reference.Should().BeNull();
            ((object?)errorBuilder).Should().NotBeNull();
            errorBuilder!.Should().HaveCode(expectedCode);
            errorBuilder!.Should().HaveMessage(expectedMessage);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetReferenceAndConfigurationData), DynamicDataSourceType.Method)]
        public void TryGetModuleReference_AliasNameInConfiguration_ReplacesReferenceValue(string referenceValue, string replacedValue, RootConfiguration configuration)
        {
            var registryMock = StrictMock.Of<IModuleRegistry>();
            registryMock.Setup(x => x.Scheme).Returns(referenceValue.Split('/')[0]);

            registryMock.Setup(x => x.TryParseModuleReference(It.IsAny<string>(), It.IsAny<RootConfiguration>(), out It.Ref<ErrorBuilderDelegate?>.IsAny))
                .Returns((ModuleReference?)null)
                .Verifiable();

            var dispatcher = CreateDispatcher(registryMock.Object);

            dispatcher.TryGetModuleReference(referenceValue, configuration, out var errorBuilder);

            registryMock.Verify(x => x.TryParseModuleReference(replacedValue, configuration, out errorBuilder), Times.Once);
        }

        public static IEnumerable<object[]> GetReferenceAndConfigurationData()
        {
            yield return new object[]
            {
                "ts/prodRG:mySpec:v1",
                "1E7593D0-FCD1-4570-B132-51E4FD254967/production-resource-group/mySpec:v1",
                CreateMockConfiguration(new Dictionary<string, string>
                {
                    ["moduleAliases:ts:prodRG:subscription"] = "1E7593D0-FCD1-4570-B132-51E4FD254967",
                    ["moduleAliases:ts:prodRG:resourceGroup"] = "production-resource-group",
                }),
            };

            yield return new object[]
            {
                "br/myRegistry:myModulePath/myModule:1.0.0",
                "127.0.0.1:5000/myModulePath/myModule:1.0.0",
                CreateMockConfiguration(new Dictionary<string, string>
                {
                    ["moduleAliases:br:myRegistry:registry"] = "127.0.0.1:5000",
                }),
            };

            yield return new object[]
            {
                "br/infra:storageAccount:v2",
                "example.com/root/storage/storageAccount:v2",
                CreateMockConfiguration(new Dictionary<string, string>
                {
                    ["moduleAliases:br:infra:registry"] = "example.com",
                    ["moduleAliases:br:infra:modulePath"] = "root/storage",
                }),
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

        private static RootConfiguration CreateMockConfiguration(IReadOnlyDictionary<string, string> configurationData, string configurationResourceName = "bicepconfig.json")
        {
            var rawConfiguration = new ConfigurationBuilder().AddInMemoryCollection(configurationData).Build();

            return RootConfiguration.Bind(rawConfiguration, configurationResourceName);
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
        }
    }
}
