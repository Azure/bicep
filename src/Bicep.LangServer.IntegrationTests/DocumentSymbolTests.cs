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
    public class DocumentSymbolTests
    {
        [TestMethod]
        public async Task RequestDocumentSymbol_should_return_full_symbol_list()
        {
            var documentUri = DocumentUri.From("/template.bicep");
            var diagsReceived = new TaskCompletionSource<PublishDiagnosticsParams>();

            var client = await IntegrationTestHelper.StartServerWithClientConnection(options => 
            {
                options.OnPublishDiagnostics(diags => {
                    diagsReceived.SetResult(diags);
                });
            });

            // client opens the document
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, @"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name = 'test'
  }
  output myOutput string = 'myOutput'
", 0));

            // client requests symbols
            var symbols = await client.TextDocument.RequestDocumentSymbol(new DocumentSymbolParams
            {
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = documentUri,
                },
            });

            symbols.Should().SatisfyRespectively(
                x => {
                    x.DocumentSymbol.Name.Should().Be("myParam");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Field);
                    x.DocumentSymbol.Range.Should().HaveRange((1, 0), (1, 29));
                },
                x => {
                    x.DocumentSymbol.Name.Should().Be("myRes");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Object);
                    x.DocumentSymbol.Range.Should().HaveRange((2, 0), (4, 3));
                },
                x => {
                    x.DocumentSymbol.Name.Should().Be("myOutput");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Interface);
                    x.DocumentSymbol.Range.Should().HaveRange((5, 2), (5, 37));
                }
            );

            // client deletes the output and renames the resource
            client.TextDocument.DidChangeTextDocument(TextDocumentParamHelper.CreateDidChangeTextDocumentParams(documentUri, @"
param myParam string = 'test'
resource myRenamedRes 'myRp/provider@2019-01-01' = {
  name = 'test'
  }
", 1));

            // client requests symbols
            symbols = await client.TextDocument.RequestDocumentSymbol(new DocumentSymbolParams
            {
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = documentUri,
                },
            });

            symbols.Should().SatisfyRespectively(
                x => {
                    x.DocumentSymbol.Name.Should().Be("myParam");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Field);
                    x.DocumentSymbol.Range.Should().HaveRange((1, 0), (1, 29));
                },
                x => {
                    x.DocumentSymbol.Name.Should().Be("myRenamedRes");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Object);
                    x.DocumentSymbol.Range.Should().HaveRange((2, 0), (4, 3));
                }
            );
        }
    }
}
