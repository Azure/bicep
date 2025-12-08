// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{

    [TestClass]
    public class DocumentFormattingTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task RequestDocumentFormattingShouldReturnFullRangeTextEdit()
        {
            var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var documentUri = DocumentUri.From("/template.bicep");

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagsListener.AddMessage));
            var client = helper.Client;

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

        [TestMethod]
        public async Task Formatting_is_supported_for_params_files()
        {
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext);
            var helper = new ServerRequestHelper(TestContext, server);

            await helper.OpenFile("/main.bicep", """
                param foo string
                param bar object
                param baz array
                """);

            var file = await helper.OpenFile("/main.bicepparam", """

                using      'main.bicep'

                     param foo =      'test'

                param bar = {
                          abc    : { }
                    def: [1,2,3]
                }

                param baz = [
                    'abc',{def:'ghi'}
                  'test'
                ]


                """);

            var textEdit = await file.Format();
            textEdit.NewText.Should().Be("""
                using 'main.bicep'

                param foo = 'test'

                param bar = {
                  abc: {}
                  def: [1, 2, 3]
                }

                param baz = [
                  'abc'
                  { def: 'ghi' }
                  'test'
                ]

                """);
        }
    }
}
