// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.IntegrationTests.Helpers;

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

            // open document
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, @"
param myParam string = 2
resource myRes 'invalidFormat' = {

}
randomToken
", 1));

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

            // change document
            diagsReceived = new TaskCompletionSource<PublishDiagnosticsParams>();
            client.TextDocument.DidChangeTextDocument(TextDocumentParamHelper.CreateDidChangeTextDocumentParams(documentUri, @"
param myParam string = 'fixed!'
resource myRes 'invalidFormat' = {

}
randomToken
", 2));

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

            // close document
            diagsReceived = new TaskCompletionSource<PublishDiagnosticsParams>();
            client.TextDocument.DidCloseTextDocument(TextDocumentParamHelper.CreateDidCloseTextDocumentParams(documentUri, 3));

            response = await IntegrationTestHelper.WithTimeout(diagsReceived.Task);
            response.Diagnostics.Should().BeEmpty();
        }
    }
}
