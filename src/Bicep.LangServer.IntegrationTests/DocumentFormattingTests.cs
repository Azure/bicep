// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{

    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class DocumentFormattingTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task RequestDocumentFormattingShouldReturnFullRangeTextEdit()
        {
            var documentUri = DocumentUri.From("/template.bicep");
            var diagnosticsReceived = new TaskCompletionSource<PublishDiagnosticsParams>();

            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(this.TestContext, options => 
            {
                options.OnPublishDiagnostics(diagnostics => {
                    diagnosticsReceived.SetResult(diagnostics);
                });
            });

            // client opens the document
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, @"
param myParam object

// line comment
var myVar1 = 1 + mod(1, 2)
var myVar2 = myParam.foo[1]

resource myResource 'myRP/provider@2020-11-01' = {
/* block
  comment
  */
  name: 'test'
}

module myModule './module.bicep' = {
  name: 'test' 
}

output myOutput string = 'value'", 0));

            // client requests symbols
            var textEditContainer = await client.TextDocument.RequestDocumentFormatting(new DocumentFormattingParams
            {
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = documentUri
                },
                Options = new FormattingOptions
                {
                    TabSize = 4,
                    InsertSpaces = true,
                    InsertFinalNewline = true,
                }
            });

            textEditContainer.Should().NotBeNull();
            textEditContainer.Should().HaveCount(1);
        }
    }
}
