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
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class DocumentSymbolTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static readonly SharedLanguageHelperManager DefaultServer = new();

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) =>
            DefaultServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext));

        [ClassCleanup]
        public static async Task ClassCleanup() =>
            await DefaultServer.DisposeAsync();

        [TestMethod]
        public async Task RequestDocumentSymbol_should_return_full_symbol_list()
        {
            var documentUri = DocumentUri.From("/template.bicep");

            // open the document and wait for diagnostics push
            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, @"
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
", documentUri);

            var client = helper.Client;

            // client requests symbols
            var symbols = await RequestSymbols(client, documentUri);

            symbols.Should().SatisfyRespectively(
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myParam");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Field);
                    x.DocumentSymbol.Children.Should().BeEmpty();
                    x.DocumentSymbol.Detail.Should().Be("string");
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myRes");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Object);
                    x.DocumentSymbol.Detail.Should().Be("myRp/provider@2019-01-01");

                    var child = x.DocumentSymbol.Children.Should().ContainSingle().Subject;
                    child.Name.Should().Be("myChild");
                    child.Kind.Should().Be(SymbolKind.Object);
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myMod");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Module);
                    x.DocumentSymbol.Children.Should().BeEmpty();
                    x.DocumentSymbol.Detail.Should().Be("error");
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myOutput");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Interface);
                    x.DocumentSymbol.Children.Should().BeEmpty();
                    x.DocumentSymbol.Detail.Should().Be("string");
                }
            );

            // client deletes the output and renames the resource
            await helper.ChangeFileAsync(TestContext, @"
param myParam string = 'test'
resource myRenamedRes 'myRp/provider@2019-01-01' = {
  name: 'test'
}
module myMod './module.bicep' = {
  name: 'test'
}
", documentUri, 1);

            // client requests symbols
            symbols = await RequestSymbols(client, documentUri);

            symbols.Should().SatisfyRespectively(
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myParam");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Field);
                    x.DocumentSymbol.Detail.Should().Be("string");
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myRenamedRes");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Object);
                    x.DocumentSymbol.Detail.Should().Be("myRp/provider@2019-01-01");
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("myMod");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Module);
                    x.DocumentSymbol.Detail.Should().Be("error");
                }
            );
        }

        [TestMethod]
        public async Task RequestDocumentSymbol_should_return_symbols_for_param_files()
        {
            var documentUri = DocumentUri.From("/param.bicepparam");

            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, @"
param one = 42
param two = 'hello'
", documentUri);

            var client = helper.Client;
            SymbolInformationOrDocumentSymbolContainer symbols = await RequestSymbols(client, documentUri);

            symbols.Should().SatisfyRespectively(
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("one");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Constant);
                    x.DocumentSymbol.Children.Should().BeEmpty();
                    x.DocumentSymbol.Detail.Should().Be("42");
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("two");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Constant);
                    x.DocumentSymbol.Children.Should().BeEmpty();
                    x.DocumentSymbol.Detail.Should().Be("'hello'");
                });

            await helper.ChangeFileAsync(TestContext, @"
param one = 42
param two = 'hello'
param three = []
", documentUri, 1);

            symbols = await RequestSymbols(client, documentUri);

            symbols.Should().SatisfyRespectively(
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("one");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Constant);
                    x.DocumentSymbol.Children.Should().BeEmpty();
                    x.DocumentSymbol.Detail.Should().Be("42");
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("two");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Constant);
                    x.DocumentSymbol.Children.Should().BeEmpty();
                    x.DocumentSymbol.Detail.Should().Be("'hello'");
                },
                x =>
                {
                    x.DocumentSymbol!.Name.Should().Be("three");
                    x.DocumentSymbol.Kind.Should().Be(SymbolKind.Constant);
                    x.DocumentSymbol.Children.Should().BeEmpty();
                    x.DocumentSymbol.Detail.Should().Be("[]");
                });
        }

        private static async Task<SymbolInformationOrDocumentSymbolContainer> RequestSymbols(ILanguageClient client, DocumentUri documentUri) =>
            await client.TextDocument.RequestDocumentSymbol(new DocumentSymbolParams
            {
                TextDocument = new TextDocumentIdentifier
                {
                    Uri = documentUri
                }
            });
    }
}
