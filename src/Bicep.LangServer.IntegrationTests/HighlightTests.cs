// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Navigation;
using Bicep.Core.Parser;
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Extensions;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SymbolKind = Bicep.Core.SemanticModel.SymbolKind;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class HighlightTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task HighlightsShouldShowAllReferencesOfTheSymbol(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");

            using var client = await IntegrationTestHelper.StartServerWithText(dataSet.Bicep, uri);
            var compilation = new Compilation(TestResourceTypeProvider.CreateRegistrar(), SyntaxFactory.CreateFromText(dataSet.Bicep));
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = TextCoordinateConverter.GetLineStarts(dataSet.Bicep);

            var symbolToSyntaxLookup = symbolTable
                .Where(pair => pair.Value.Kind != SymbolKind.Error)
                .ToLookup(pair => pair.Value, pair => pair.Key);

            foreach (var (syntax, symbol) in symbolTable.Where(s => s.Value.Kind != SymbolKind.Error))
            {
                var highlights = await client.RequestDocumentHighlight(new DocumentHighlightParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = GetPosition(syntax, lineStarts)
                });

                // calculate expected highlights
                var expectedHighlights = symbolToSyntaxLookup[symbol].Select(node => CreateExpectedHighlight(lineStarts, node));

                // ranges should match what we got from our own symbol table
                highlights.Should().BeEquivalentTo(expectedHighlights);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task RequestingHighlightsForWrongNodeShouldProduceNoHighlights(DataSet dataSet)
        {
            // local function
            bool IsWrongNode(SyntaxBase node) => !(node is ISymbolReference) && !(node is IDeclarationSyntax) && !(node is Token);

            var uri = DocumentUri.From($"/{dataSet.Name}");

            using var client = await IntegrationTestHelper.StartServerWithText(dataSet.Bicep, uri);
            var compilation = new Compilation(TestResourceTypeProvider.CreateRegistrar(), SyntaxFactory.CreateFromText(dataSet.Bicep));
            var lineStarts = TextCoordinateConverter.GetLineStarts(dataSet.Bicep);

            var wrongNodes = SyntaxAggregator.Aggregate(
                compilation.ProgramSyntax,
                new List<SyntaxBase>(),
                (accumulated, node) =>
                {
                    if (IsWrongNode(node) && !(node is ProgramSyntax))
                    {
                        accumulated.Add(node);
                    }

                    return accumulated;
                },
                accumulated => accumulated,
                (accumulated, node) => IsWrongNode(node));

            foreach (var syntax in wrongNodes)
            {
                var highlights = await client.RequestDocumentHighlight(new DocumentHighlightParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = GetPosition(syntax, lineStarts)
                });

                highlights.Should().BeEmpty();
            }
        }

        private static DocumentHighlight CreateExpectedHighlight(ImmutableArray<int> lineStarts, SyntaxBase syntax) =>
            new DocumentHighlight
            {
                Range = PositionHelper.GetNameRange(lineStarts, syntax),
                Kind = GetExpectedHighlightKind(syntax)
            };

        private static DocumentHighlightKind GetExpectedHighlightKind(SyntaxBase syntax)
        {
            switch (syntax)
            {
                case ISymbolReference _:
                    return DocumentHighlightKind.Read;

                case IDeclarationSyntax _:
                    return DocumentHighlightKind.Write;

                default:
                    throw new AssertFailedException($"Unexpected syntax type '{syntax.GetType().Name}'.");
            }
        }

        private Position GetPosition(SyntaxBase syntax, ImmutableArray<int> lineStarts)
        {
            if (syntax is InstanceFunctionCallSyntax instanceFunctionCall)
            {
                return PositionHelper.GetPosition(lineStarts, instanceFunctionCall.Name.Span.Position);
            }

            return PositionHelper.GetPosition(lineStarts, syntax.Span.Position);
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.ToDynamicTestData();
        }
    }
}
