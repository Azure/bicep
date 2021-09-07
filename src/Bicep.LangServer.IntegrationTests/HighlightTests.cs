// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.IntegrationTests.Extensions;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SymbolKind = Bicep.Core.Semantics.SymbolKind;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class HighlightTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task HighlightsShouldShowAllReferencesOfTheSymbol(DataSet dataSet)
        {
            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, dataSet.Bicep, uri);
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            // filter out binding failures and locals with invalid identifiers
            // (locals are special because their full span is the same as the identifier span,
            // which makes it impossible to highlight locals with invalid identifiers)
            var filteredSymbolTable = symbolTable.Where(pair => pair.Value.Kind != SymbolKind.Error && (pair.Value is not LocalVariableSymbol local || local.NameSyntax.IsValid));
            // TODO: Implement for PropertySymbol
            filteredSymbolTable = filteredSymbolTable.Where(pair => pair.Value is not PropertySymbol);

            var symbolToSyntaxLookup = filteredSymbolTable.ToLookup(pair => pair.Value, pair => pair.Key);

            foreach (var (syntax, symbol) in filteredSymbolTable)
            {
                var highlights = await client.RequestDocumentHighlight(new DocumentHighlightParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = IntegrationTestHelper.GetPosition(lineStarts, syntax)
                });

                // calculate expected highlights
                var expectedHighlights = symbolToSyntaxLookup[symbol].Select(node => CreateExpectedHighlight(lineStarts, node));

                using (new AssertionScope()
                    .WithAnnotations(compilation.SourceFileGrouping.EntryPoint, "expected", expectedHighlights, _ => "here", x => x.Range)
                    .WithAnnotations(compilation.SourceFileGrouping.EntryPoint, "actual", highlights, _ => "here", x => x.Range))
                {
                    // ranges should match what we got from our own symbol table
                    highlights.Should().BeEquivalentTo(expectedHighlights);
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task RequestingHighlightsForWrongNodeShouldProduceNoHighlights(DataSet dataSet)
        {
            // local function
            static bool IsWrongNode(SyntaxBase node) =>
                !(node is PropertyAccessSyntax propertyAccessSyntax && propertyAccessSyntax.BaseExpression is ISymbolReference) &&
                node is not ISymbolReference &&
                node is not ITopLevelNamedDeclarationSyntax &&
                node is not Token;

            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, dataSet.Bicep, uri);
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            var wrongNodes = SyntaxAggregator.Aggregate(
                compilation.SourceFileGrouping.EntryPoint.ProgramSyntax,
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
                    Position = IntegrationTestHelper.GetPosition(lineStarts, syntax)
                });

                highlights.Should().BeNull();
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
                case ISymbolReference:
                case PropertyAccessSyntax:
                    return DocumentHighlightKind.Read;

                case INamedDeclarationSyntax:
                case ObjectPropertySyntax:
                    return DocumentHighlightKind.Write;

                default:
                    throw new AssertFailedException($"Unexpected syntax type '{syntax.GetType().Name}'.");
            }
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.NonStressDataSets.ToDynamicTestData();
        }
    }
}
