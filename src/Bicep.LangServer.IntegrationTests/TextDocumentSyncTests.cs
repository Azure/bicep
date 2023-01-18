// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.Core.Analyzers.Linter.Rules;
using System;
using System.Collections.Generic;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class TextDocumentSyncTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task DidOpenTextDocument_should_trigger_PublishDiagnostics()
        {
            var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var documentUri = DocumentUri.From("/template.bicep");

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagsListener.AddMessage));
            var client = helper.Client;

            // open document
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, @"
param myParam string = 2
resource myRes 'invalidFormat' = {

}
randomToken
", 1));

            var response = await diagsListener.WaitNext();
            response.Diagnostics.Should().SatisfyRespectively(
                d =>
                {
                    d.Range.Should().HaveRange((1, 6), (1, 13));
                    // note documentation pretty printing moves Uri to code for output
                    d.Should().HaveCodeAndSeverity(new NoUnusedParametersRule().Uri!.AbsoluteUri, DiagnosticSeverity.Warning);
                },
                d =>
                {
                    d.Range.Should().HaveRange((1, 23), (1, 24));
                    d.Should().HaveCodeAndSeverity("BCP033", DiagnosticSeverity.Error);
                },
                d =>
                {
                    d.Range.Should().HaveRange((2, 15), (2, 30));
                    d.Should().HaveCodeAndSeverity("BCP029", DiagnosticSeverity.Error);
                },
                d =>
                {
                    d.Range.Should().HaveRange((5, 0), (5, 11));
                    d.Should().HaveCodeAndSeverity("BCP007", DiagnosticSeverity.Error);
                }
            );

            // change document
            client.TextDocument.DidChangeTextDocument(TextDocumentParamHelper.CreateDidChangeTextDocumentParams(documentUri, @"
param myParam string = 'fixed!'
resource myRes 'invalidFormat' = {

}
randomToken
", 2));

            response = await diagsListener.WaitNext();
            response.Diagnostics.Should().SatisfyRespectively(
                d =>
                {
                    d.Range.Should().HaveRange((1, 6), (1, 13));
                    // documentation provided with linter sets code to uri for pretty link print outs
                    d.Should().HaveCodeAndSeverity(new NoUnusedParametersRule().Uri!.AbsoluteUri, DiagnosticSeverity.Warning);
                },
                d =>
                {
                    d.Range.Should().HaveRange((2, 15), (2, 30));
                    d.Should().HaveCodeAndSeverity("BCP029", DiagnosticSeverity.Error);
                },
                d =>
                {
                    d.Range.Should().HaveRange((5, 0), (5, 11));
                    d.Should().HaveCodeAndSeverity("BCP007", DiagnosticSeverity.Error);
                }
            );

            // close document
            client.TextDocument.DidCloseTextDocument(TextDocumentParamHelper.CreateDidCloseTextDocumentParams(documentUri, 3));

            response = await diagsListener.WaitNext();
            response.Diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Module_resolution_does_not_work_for_file_paths_containing_escaped_spaces()
        {
            // Here's a repro for https://github.com/Azure/bicep/issues/9466. It has not yet been fixed, but this test makes it simple to debug.
            var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var entryPointUri = new Uri("file:///src/demo%2520repo/main.bicep");
            var fileSystemDict = new Dictionary<Uri, string>
            {
                [entryPointUri] = @"
module asf './storage-account.bicep' = {
  name: 'asf'
}
",
                [new Uri("file:///src/demo%2520repo/storage-account.bicep")] = @"
",
            };

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagsListener.AddMessage),
                services => services.WithFileResolver(new InMemoryFileResolver(fileSystemDict)));
            var client = helper.Client;

            // open document
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(entryPointUri, fileSystemDict[entryPointUri], 1));

            var diagnostics = await diagsListener.WaitNext();
            diagnostics.Diagnostics.Should().Contain(x => x.Code == "BCP091" && x.Message == "An error occurred reading file. Could not find file '/src/demo repo/storage-account.bicep'.");
        }
    }
}
