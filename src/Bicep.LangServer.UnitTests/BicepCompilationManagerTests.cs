// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.CompilationManager;
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
        private static readonly MockRepository Repository = new MockRepository(MockBehavior.Strict);

        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void UpsertCompilation_ShouldUpsertSuccessfully()
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = CreateMockDocument(p => receivedParams = p);

            var server = CreateMockServer(document);

            var manager = new BicepCompilationManager(server.Object, new BicepCompilationProvider(TestResourceTypeProvider.Create()));

            const int version = 42;
            var uri = DocumentUri.File(this.TestContext!.TestName);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            CompilationContext? upserted = manager.UpsertCompilation(uri, version, "hello");

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // there should have been 1 diagnostic
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(version);
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
        public void CloseAfterUpsert_ShouldClearDiagnostics()
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = CreateMockDocument(p => receivedParams = p);

            var server = CreateMockServer(document);

            var manager = new BicepCompilationManager(server.Object, new BicepCompilationProvider(TestResourceTypeProvider.Create()));

            const int version = 42;
            var uri = DocumentUri.File(this.TestContext!.TestName);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            manager.UpsertCompilation(uri, version, "hello");

            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // there should have been 1 diagnostic
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(version);
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
        public void UpsertCompilation_ShouldUpdateDiagnostics()
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = CreateMockDocument(p => receivedParams = p);

            var server = CreateMockServer(document);

            var manager = new BicepCompilationManager(server.Object, new BicepCompilationProvider(TestResourceTypeProvider.Create()));

            const int version = 42;
            var uri = DocumentUri.File(this.TestContext!.TestName);

            // first get should not return anything
            manager.GetCompilation(uri).Should().BeNull();

            // upsert the compilation
            var firstUpserted = manager.UpsertCompilation(uri, version, "hello");

            // should have pushed out diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            // there should have been 1 diagnostic
            receivedParams.Should().NotBeNull();
            receivedParams!.Uri.Should().Be(uri);
            receivedParams.Version.Should().Be(version);
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
            const int newVersion = version + 1;
            var secondUpserted = manager.UpsertCompilation(uri, newVersion, "hello\r\nthere\r\n");

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

            var manager = new BicepCompilationManager(server.Object, new BicepCompilationProvider(TestResourceTypeProvider.Create()));

            var uri = DocumentUri.File(this.TestContext!.TestName);

            manager.GetCompilation(uri).Should().BeNull();
        }

        [TestMethod]
        public void CloseNonExistentCompilation_ShouldClearDiagnostics()
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = CreateMockDocument(p => receivedParams = p);

            var server = CreateMockServer(document);

            var manager = new BicepCompilationManager(server.Object, new BicepCompilationProvider(TestResourceTypeProvider.Create()));

            var uri = DocumentUri.File(this.TestContext!.TestName);

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
        public void FatalException_ShouldProduceCorrectDiagnosticsAndClearThemWhenFileIsClosed()
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = CreateMockDocument(p => receivedParams = p);

            var server = CreateMockServer(document);

            var provider = Repository.Create<ICompilationProvider>();
            const string expectedMessage = "Internal bicep exception.";
            provider.Setup(m => m.Create(It.IsAny<DocumentUri>(), It.IsAny<string>())).Throws(new InvalidOperationException(expectedMessage));

            var manager = new BicepCompilationManager(server.Object, provider.Object);

            const int version = 74;
            var uri = DocumentUri.File(this.TestContext!.TestName);

            // upsert should fail because of the mock fatal exception
            manager.UpsertCompilation(uri, version, "fake").Should().BeNull();

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
        public void NormalUpsertAfterFatalException_ShouldReplaceDiagnostics()
        {
            PublishDiagnosticsParams? receivedParams = null;

            var document = CreateMockDocument(p => receivedParams = p);

            var server = CreateMockServer(document);

            var provider = Repository.Create<ICompilationProvider>();
            const string expectedMessage = "Internal bicep exception.";

            const int version = 74;
            var uri = DocumentUri.File(this.TestContext!.TestName);
            
            // start by failing
            bool failUpsert = true;
            provider
                .Setup(m => m.Create(It.IsAny<DocumentUri>(), It.IsAny<string>()))
                .Returns<DocumentUri, string>((documentUri, text) => failUpsert ? throw new InvalidOperationException(expectedMessage) : new BicepCompilationProvider(TestResourceTypeProvider.Create()).Create(documentUri, text));

            var manager = new BicepCompilationManager(server.Object, provider.Object);


            // upsert should fail because of the mock fatal exception
            manager.UpsertCompilation(uri, version, "fake").Should().BeNull();

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
            var upserted = manager.UpsertCompilation(uri, version, "fake\nfake\nfake\n");
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

        private static Mock<ITextDocumentLanguageServer> CreateMockDocument(Action<PublishDiagnosticsParams> callback)
        {
            var document = Repository.Create<ITextDocumentLanguageServer>();
            document
                .Setup(m => m.SendNotification(It.IsAny<MediatR.IRequest>()))
                .Callback<MediatR.IRequest>((p) => callback((PublishDiagnosticsParams)p))
                .Verifiable();

            return document;
        }

        private static Mock<ILanguageServerFacade> CreateMockServer(Mock<ITextDocumentLanguageServer> document)
        {
            var server = Repository.Create<ILanguageServerFacade>();
            server
                .Setup(m => m.TextDocument)
                .Returns(document.Object);

            return server;
        }
    }
}

