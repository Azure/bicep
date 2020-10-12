// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class ReferencesTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task FindReferencesWithDeclarationsShouldProduceCorrectResults(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(dataSet.Bicep));
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = TextCoordinateConverter.GetLineStarts(dataSet.Bicep);

            var symbolToSyntaxLookup = symbolTable
                .Where(pair => pair.Value.Kind != SymbolKind.Error)
                .ToLookup(pair => pair.Value, pair => pair.Key);

            foreach (var (syntax, symbol) in symbolTable.Where(s => s.Value.Kind != SymbolKind.Error))
            {
                var locations = await client.RequestReferences(new ReferenceParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Context = new ReferenceContext
                    {
                        IncludeDeclaration = true
                    },
                    Position = IntegrationTestHelper.GetPosition(lineStarts, syntax)
                });

                // all URIs should be the same in the results
                locations.Select(r => r.Uri).Should().AllBeEquivalentTo(uri);

                // calculate expected ranges
                var expectedRanges = symbolToSyntaxLookup[symbol]
                    .Select(node => PositionHelper.GetNameRange(lineStarts, node));

                // ranges should match what we got from our own symbol table
                locations.Select(l => l.Range).Should().BeEquivalentTo(expectedRanges);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task FindReferencesWithoutDeclarationsShouldProduceCorrectResults(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(dataSet.Bicep));
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = TextCoordinateConverter.GetLineStarts(dataSet.Bicep);

            var symbolToSyntaxLookup = symbolTable
                .Where(pair => pair.Value.Kind != SymbolKind.Error)
                .ToLookup(pair => pair.Value, pair => pair.Key);

            foreach (var (syntax, symbol) in symbolTable.Where(s => s.Value.Kind != SymbolKind.Error))
            {
                var locations = await client.RequestReferences(new ReferenceParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Context = new ReferenceContext
                    {
                        IncludeDeclaration = false
                    },
                    Position = IntegrationTestHelper.GetPosition(lineStarts, syntax)
                });

                // all URIs should be the same in the results
                locations.Select(r => r.Uri).Should().AllBeEquivalentTo(uri);

                // exclude declarations when calculating expected ranges
                var expectedRanges = symbolToSyntaxLookup[symbol]
                    .Where(node => !(node is IDeclarationSyntax))
                    .Select(node => PositionHelper.GetNameRange(lineStarts, node));

                // ranges should match what we got from our own symbol table
                locations.Select(l => l.Range).Should().BeEquivalentTo(expectedRanges);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task FindReferencesOnNonSymbolsShouldProduceEmptyResult(DataSet dataSet)
        {
            // local function
            bool IsWrongNode(SyntaxBase node) => !(node is ISymbolReference) && !(node is IDeclarationSyntax) && !(node is Token);

            var uri = DocumentUri.From($"/{dataSet.Name}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(dataSet.Bicep));
            var lineStarts = TextCoordinateConverter.GetLineStarts(dataSet.Bicep);

            var wrongNodes = SyntaxAggregator.Aggregate(
                compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax,
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
                var locations = await client.RequestReferences(new ReferenceParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Context = new ReferenceContext
                    {
                        IncludeDeclaration = false
                    },
                    Position = IntegrationTestHelper.GetPosition(lineStarts, syntax)
                });

                locations.Should().BeEmpty();
            }
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.ToDynamicTestData();
        }
    }

    
}
