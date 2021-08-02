// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Providers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepCompilationManagerTests
    {
        private const int BaseVersion = 42;

        private static readonly MockRepository Repository = new(MockBehavior.Strict);

        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DataRow(".arm")]
        [DataRow(".json")]
        [DataRow(".jsonc")]
        public void UpsertCompilation_NotInWorspaceArmTemplateFile_ShouldNotUpsert(string fileExtension)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);
            var uri = DocumentUri.File(this.TestContext.TestName + fileExtension).ToUri();
            var workspace = new Workspace();
            var manager = BicepCompilationManagerHelper.CreateCompilationManager(uri, string.Empty);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpsertCompilation(uri, null, "hello");

            // second get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            workspace.TryGetSourceFile(uri, out var file);

            // The workspace should remain empty.
            file.Should().BeNull();

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(".arm")]
        [DataRow(".json")]
        [DataRow(".jsonc")]
        public void UpsertCompilation_InWorspaceArmTemplateFile_ShouldRefreshWorkspace(string fileExtension)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);
            var uri = DocumentUri.File(this.TestContext.TestName + fileExtension).ToUri();

            var originalFile = SourceFileFactory.CreateArmTemplateFile(uri, "{}");
            var workspace = new Workspace();
            workspace.UpsertSourceFile(originalFile);

            var manager = new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(), workspace);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpsertCompilation(uri, null, "hello");

            // second get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            workspace.TryGetSourceFile(uri, out var updatedFile);

            // The workspace should be refreshed.
            updatedFile.Should().NotBeNull();
            updatedFile!.FileUri.Should().Be(originalFile.FileUri);
            updatedFile.Should().NotBeSameAs(originalFile);

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);
        }

        [DataTestMethod]
        [DataRow(LanguageConstants.LanguageId)]
        [DataRow(null)]
        public void UpsertCompilation_BicepFile_ShouldUpsertSuccessfully(string? languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);
            var uri = DocumentUri.File(this.TestContext.TestName);
            var workspace = new Workspace();

            if (languageId is null)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(uri.ToUri(), ""));
            }

            var manager = new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(), workspace);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpsertCompilation(uri, BaseVersion, "hello", languageId);
            var upserted = manager.GetCompilation(uri);

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // there should have been 1 diagnostic
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(BaseVersion);
            receivedParams.Diagnostics.Should().NotBeNullOrEmpty();
            receivedParams.Diagnostics.Count().Should().Be(1);

            // reset tracked calls
            document.Invocations.Clear();

            // get again
            var actual = manager.GetCompilation(uri);
            actual.Should().NotBeNull();
            
            // should be the same object
            actual.Should().BeSameAs(upserted);

            // get should not have pushed diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);
        }

        [TestMethod]
        [DataRow(LanguageConstants.LanguageId)]
        [DataRow(null)]
        public void CloseAfterUpsert_ShouldClearDiagnostics(string? languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);
            var uri = DocumentUri.File(this.TestContext.TestName);
            var workspace = new Workspace();

            if (languageId is null)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(uri.ToUri(), ""));
            }

            var manager = new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(), workspace);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpsertCompilation(uri, BaseVersion, "hello", languageId);

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // there should have been 1 diagnostic
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(BaseVersion);
            receivedParams.Diagnostics.Should().NotBeNullOrEmpty();
            receivedParams.Diagnostics.Count().Should().Be(1);

            // reset tracked calls
            document.Invocations.Clear();

            // get again
            var actual = manager.GetCompilation(uri);
            actual.Should().NotBeNull();

            // get should not have pushed diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);

            // 2nd get should be the same
            manager.GetCompilation(uri).Should().BeSameAs(actual);

            // get should not have pushed diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);

            // close compilation
            manager.CloseCompilation(uri);

            // close should have cleared diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // expect zero diagnostics and 0 version
            receivedParams.Should().NotBeNull();
            receivedParams.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(0);
            receivedParams.Diagnostics.Should().BeEmpty();

            // reset call counts
            document.Invocations.Clear();

            // get again
            manager.GetCompilation(uri).Should().BeNull();

            // get should not have pushed diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);
        }

        [TestMethod]
        [DataRow(LanguageConstants.LanguageId)]
        [DataRow(null)]
        public void UpsertCompilation_ShouldUpdateDiagnostics(string? languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);
            var uri = DocumentUri.File(this.TestContext.TestName);
            var workspace = new Workspace();

            if (languageId is null)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(uri.ToUri(), ""));
            }

            var manager = new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(), workspace);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpsertCompilation(uri, BaseVersion, "hello", languageId);
            var firstUpserted = manager.GetCompilation(uri);

            // should have pushed out diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // there should have been 1 diagnostic
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(BaseVersion);
            receivedParams.Diagnostics.Should().NotBeNullOrEmpty();
            receivedParams.Diagnostics.Count().Should().Be(1);

            // reset tracked calls
            document.Invocations.Clear();

            // get again
            var firstActual = manager.GetCompilation(uri);
            firstActual.Should().NotBeNull();

            // should be same as first upserted
            firstActual.Should().BeSameAs(firstUpserted);

            // upsert second one
            const int newVersion = BaseVersion + 1;
            manager.UpsertCompilation(uri, newVersion, "hello\r\nthere\r\n");
            var secondUpserted = manager.GetCompilation(uri);

            secondUpserted.Should().NotBeNull();
            secondUpserted.Should().NotBeSameAs(firstUpserted);

            // should have pushed out new diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // reset invocations
            document.Invocations.Clear();

            // there should have been 2 diagnostics
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(newVersion);
            receivedParams.Diagnostics.Should().NotBeNullOrEmpty();
            receivedParams.Diagnostics.Count().Should().Be(2);

            // get latest
            var secondActual = manager.GetCompilation(uri);
            secondActual.Should().BeSameAs(secondUpserted);
        }

        [TestMethod]
        public void GetNonExistentCompilation_ShouldNotThrow()
        {
            var server = Repository.Create<ILanguageServerFacade>();

            var manager = new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(), new Workspace());

            var uri = DocumentUri.File(this.TestContext.TestName);

            manager.GetCompilation(uri).Should().BeNull();
        }

        [TestMethod]
        public void CloseNonExistentCompilation_ShouldClearDiagnostics()
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);

            var manager = new BicepCompilationManager(server.Object, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(), new Workspace());

            var uri = DocumentUri.File(this.TestContext.TestName);

            manager.CloseCompilation(uri);

            // close should have cleared diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // expect zero diagnostics and 0 version
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(0);
            receivedParams.Diagnostics.Should().BeEmpty();

            // reset call counts
            document.Invocations.Clear();
        }

        [TestMethod]
        [DataRow(LanguageConstants.LanguageId)]
        [DataRow(null)]
        public void FatalException_ShouldProduceCorrectDiagnosticsAndClearThemWhenFileIsClosed(string? languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);

            var provider = Repository.Create<ICompilationProvider>();
            const string expectedMessage = "Internal bicep exception.";
            provider.Setup(m => m.Create(It.IsAny<IReadOnlyWorkspace>(), It.IsAny<DocumentUri>())).Throws(new InvalidOperationException(expectedMessage));

            var uri = DocumentUri.File(this.TestContext.TestName);
            var workspace = new Workspace();

            if (languageId is null)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(uri.ToUri(), ""));
            }

            var manager = new BicepCompilationManager(server.Object, provider.Object, workspace);

            // upsert should fail because of the mock fatal exception
            manager.UpsertCompilation(uri, BaseVersion, "fake", languageId);
            manager.GetCompilation(uri).Should().BeNull();

            // diagnostics should have been published once
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(BaseVersion);
            receivedParams.Diagnostics.Should().HaveCount(1);

            var fatalDiagnostic = receivedParams.Diagnostics.Single();

            fatalDiagnostic.Message.Should().Be(expectedMessage);
            fatalDiagnostic.Severity.Should().Be(DiagnosticSeverity.Error);

            // reset counts
            document.Invocations.Clear();

            // close the compilation (even if it wasn't opened successfully)
            manager.CloseCompilation(uri);

            // diagnostics should have been published once
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // 0 diagnostics expected
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(0);
            receivedParams.Diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        [DataRow(LanguageConstants.LanguageId)]
        [DataRow(null)]
        public void NormalUpsertAfterFatalException_ShouldReplaceDiagnostics(string? languageId)
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            var server = BicepCompilationManagerHelper.CreateMockServer(document);

            var provider = Repository.Create<ICompilationProvider>();
            const string expectedMessage = "Internal bicep exception.";

            const int version = 74;
            var uri = DocumentUri.File(this.TestContext.TestName);
            
            // start by failing
            bool failUpsert = true;
            provider
                .Setup(m => m.Create(It.IsAny<IReadOnlyWorkspace>(), It.IsAny<DocumentUri>()))
                .Returns<IReadOnlyWorkspace, DocumentUri>((workspace, documentUri) => failUpsert ? throw new InvalidOperationException(expectedMessage) : BicepCompilationManagerHelper.CreateEmptyCompilationProvider().Create(workspace, documentUri));

            var workspace = new Workspace();

            if (languageId is null)
            {
                workspace.UpsertSourceFile(SourceFileFactory.CreateBicepFile(uri.ToUri(), ""));
            }

            var manager = new BicepCompilationManager(server.Object, provider.Object, workspace);

            // upsert should fail because of the mock fatal exception
            manager.UpsertCompilation(uri, version, "fake", languageId);
            manager.GetCompilation(uri).Should().BeNull();

            // diagnostics should have been published once
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(version);
            receivedParams.Diagnostics.Should().HaveCount(1);

            var fatalDiagnostic = receivedParams.Diagnostics.Single();

            fatalDiagnostic.Message.Should().Be(expectedMessage);
            fatalDiagnostic.Severity.Should().Be(DiagnosticSeverity.Error);

            // reset counts
            document.Invocations.Clear();

            // allow success
            failUpsert = false;

            // upsert should succeed because we allowed it
            manager.UpsertCompilation(uri, version, "fake\nfake\nfake\n");
            var upserted = manager.GetCompilation(uri);
            upserted.Should().NotBeNull();

            // new diagnostics should have been published once
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(version);
            receivedParams.Diagnostics.Should().HaveCount(3);

            // none of the messages should be our fatal message
            receivedParams.Diagnostics
                .Select(diag => diag.Message)
                .All(message => string.Equals(message, expectedMessage) == false)
                .Should().BeTrue();
        }
      }
}

