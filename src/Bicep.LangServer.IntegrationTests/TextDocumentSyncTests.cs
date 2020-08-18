using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Bicep.LangServer.IntegrationTests.Assertions;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class TextDocumentSyncTests
    {
        [TestMethod]
        public async Task DidOpenTextDocument_should_trigger_PublishDiagnostics()
        {
            var documentUri = DocumentUri.From("/template.bicep");
            var diagsReceived = new TaskCompletionSource<PublishDiagnosticsParams>();

            var client = await IntegrationTestHelper.StartServerWithClientConnection(options => 
            {
                options.OnPublishDiagnostics(diags => {
                    diagsReceived.SetResult(diags);
                });
            });

            client.TextDocument.DidOpenTextDocument(new DidOpenTextDocumentParams
            {
                TextDocument = new TextDocumentItem
                {
                    LanguageId = "bicep",
                    Version = 1,
                    Uri = documentUri,
                    Text = @"
param myParam string = 2
resource myRes 'invalidFormat' = {

}
randomToken
",
                },
            });

            var response = await IntegrationTestHelper.WithTimeout(diagsReceived.Task);
            response.Diagnostics.Should().SatisfyRespectively(
                d => {
                    d.Range.Should().HaveRange((1, 23), (1, 24));
                    d.Should().HaveCodeAndSeverity("BCP027", DiagnosticSeverity.Error);
                },
                d => {
                    d.Range.Should().HaveRange((2, 15), (2, 30));
                    d.Should().HaveCodeAndSeverity("BCP029", DiagnosticSeverity.Error);
                },
                d => {
                    d.Range.Should().HaveRange((5, 0), (5, 11));
                    d.Should().HaveCodeAndSeverity("BCP007", DiagnosticSeverity.Error);
                }
            );

            diagsReceived = new TaskCompletionSource<PublishDiagnosticsParams>();
            client.TextDocument.DidChangeTextDocument(new DidChangeTextDocumentParams
            {
                TextDocument = new VersionedTextDocumentIdentifier
                {
                    Version = 2,
                    Uri = documentUri,
                },
                ContentChanges = new Container<TextDocumentContentChangeEvent>(
                    new TextDocumentContentChangeEvent
                    {
                        Text = @"
param myParam string = 'fixed!'
resource myRes 'invalidFormat' = {

}
randomToken
",
                    }
                ),
            });

            response = await IntegrationTestHelper.WithTimeout(diagsReceived.Task);
            response.Diagnostics.Should().SatisfyRespectively(
                d => {
                    d.Range.Should().HaveRange((2, 15), (2, 30));
                    d.Should().HaveCodeAndSeverity("BCP029", DiagnosticSeverity.Error);
                },
                d => {
                    d.Range.Should().HaveRange((5, 0), (5, 11));
                    d.Should().HaveCodeAndSeverity("BCP007", DiagnosticSeverity.Error);
                }
            );
        }
    }
}