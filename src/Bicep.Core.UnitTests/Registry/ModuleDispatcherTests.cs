// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.Workspaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class ModuleDispatcherTests
    {
        private static readonly MockRepository Repository = new MockRepository(MockBehavior.Strict);

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
            dispatcher.TryGetModuleReference(module, out var failureBuilder).Should().BeNull();
            failureBuilder!.Should().NotBeNull();

            using (new AssertionScope())
            {
                failureBuilder!.Should().HaveCode("BCP189");
                failureBuilder!.Should().HaveMessage("Module references are not supported in this context.");
            }

            var localModule = CreateModule("test.bicep");
            dispatcher.TryGetModuleReference(localModule, out var localModuleFailureBuilder).Should().BeNull();
            using (new AssertionScope())
            {
                localModuleFailureBuilder!.Should().HaveCode("BCP189");
                localModuleFailureBuilder!.Should().HaveMessage("Module references are not supported in this context.");
            }
        }

        [TestMethod]
        public void MockRegistries_AvailableSchemes_ShouldReturnedConfiguredSchemes()
        {
            var first = Repository.Create<IModuleRegistry>();
            first.Setup(m => m.Scheme).Returns("first");

            var second = Repository.Create<IModuleRegistry>();
            second.Setup(m => m.Scheme).Returns("second");

            var dispatcher = CreateDispatcher(first.Object, second.Object);
            dispatcher.AvailableSchemes.Should().BeEquivalentTo("first", "second");
        }

        [TestMethod]
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Not needed")]
        public async Task MockRegistries_ModuleLifecycle()
        {
            var fail = Repository.Create<IModuleRegistry>();
            fail.Setup(m => m.Scheme).Returns("fail");

            var mock = Repository.Create<IModuleRegistry>();
            mock.Setup(m => m.Scheme).Returns("mock");

            DiagnosticBuilder.ErrorBuilderDelegate? @null = null;
            var validRef = new MockModuleReference("validRef");
            mock.Setup(m => m.TryParseModuleReference("validRef", out @null))
                .Returns(validRef);

            var validRef2 = new MockModuleReference("validRef2");
            mock.Setup(m => m.TryParseModuleReference("validRef2", out @null))
                .Returns(validRef2);

            var validRef3 = new MockModuleReference("validRef3");
            mock.Setup(m => m.TryParseModuleReference("validRef3", out @null))
                .Returns(validRef3);

            DiagnosticBuilder.ErrorBuilderDelegate? badRefError = x => new ErrorDiagnostic(x.TextSpan, "BCPMock", "Bad ref error");
            mock.Setup(m => m.TryParseModuleReference("badRef", out badRefError))
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

            dispatcher.TryGetModuleReference(goodModule, out var goodValidationBuilder).Should().Be(validRef);
            goodValidationBuilder!.Should().BeNull();
            
            dispatcher.TryGetModuleReference(badModule, out var badValidationBuilder).Should().BeNull();
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

        private static IModuleDispatcher CreateDispatcher(params IModuleRegistry[] registries)
        {
            var provider = Repository.Create<IModuleRegistryProvider>();
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
        }
    }
}
