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

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class DocumentSymbolTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task RequestDocumentSymbol_should_return_full_symbol_list()
        {
            var documentUri = DocumentUri.From("/template.bicep");
            var diagsReceived = new TaskCompletionSource<PublishDiagnosticsParams>();

            using var helper = await LanguageServerHelper.StartServerWithClientConnectionAsync(this.TestContext, options =>
            {
                options.OnPublishDiagnostics(diags =>
                {
                    diagsReceived.SetResult(diags);
                });
            });
            var client = helper.Client;

            // client opens the document
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, @"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'test'

  resource myChild 'child' = {
    name: 'test'
  }
}
module myMod './module.bicep' = {
  name: 'test'
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
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myParam");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Field);
                    x.DocumentSymbol.Children.Should().BeEmpty();
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myRes");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Object);

                    var child = x.DocumentSymbol.Children.Should().ContainSingle().Subject;
                    child.Name.Should().Be("myChild");
                    child.Kind.Should().Be(SymbolKind.Object);
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myMod");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Module);
                    x.DocumentSymbol.Children.Should().BeEmpty();
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myOutput");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Interface);
                    x.DocumentSymbol.Children.Should().BeEmpty();
                }
            );

            // client deletes the output and renames the resource
            client.TextDocument.DidChangeTextDocument(TextDocumentParamHelper.CreateDidChangeTextDocumentParams(documentUri, @"
param myParam string = 'test'
resource myRenamedRes 'myRp/provider@2019-01-01' = {
  name: 'test'
}
module myMod './module.bicep' = {
  name: 'test'
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
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myParam");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Field);
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myRenamedRes");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Object);
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myMod");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Module);
                }
            );
        }
    }
}
