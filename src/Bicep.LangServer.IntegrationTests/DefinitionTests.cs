// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Parser;
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Extensions;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class DefinitionTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task GoToDefinitionRequestOnValidSymbolReferenceShouldReturnLocationOfDeclaredSymbol(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(dataSet.Bicep));
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = TextCoordinateConverter.GetLineStarts(dataSet.Bicep);

            // filter out symbols that don't have locations
            var declaredSymbolBindings = symbolTable
                .Where(pair => pair.Value is DeclaredSymbol)
                .Select(pair => new KeyValuePair<SyntaxBase, DeclaredSymbol>(pair.Key, (DeclaredSymbol) pair.Value));

            foreach (var (syntax, symbol) in declaredSymbolBindings)
            {
                var response = await client.RequestDefinition(new DefinitionParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = PositionHelper.GetPosition(lineStarts, syntax.Span.Position)
                });

                var link = ValidateDefinitionResponse(response);

                // document should match the requested document
                link.TargetUri.Should().Be(uri);

                // target range should be the whole span of the symbol
                link.TargetRange.Should().Be(symbol.DeclaringSyntax.Span.ToRange(lineStarts));

                // selection range should be the span of the identifier of the symbol
                link.TargetSelectionRange.Should().Be(symbol.NameSyntax.Span.ToRange(lineStarts));

                // origin selection range should be the span of the syntax node that references the symbol
                link.OriginSelectionRange.Should().Be(syntax.ToRange(lineStarts));
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task GoToDefinitionRequestOnUnsupportedOrInvalidSyntaxNodeShouldReturnEmptyResponse(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(dataSet.Bicep));
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = TextCoordinateConverter.GetLineStarts(dataSet.Bicep);

            var undeclaredSymbolBindings = symbolTable.Where(pair => !(pair.Value is DeclaredSymbol));

            foreach (var (syntax, _) in undeclaredSymbolBindings)
            {
                var response = await client.RequestDefinition(new DefinitionParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = PositionHelper.GetPosition(lineStarts, syntax.Span.Position)
                });

                // go to definition on a symbol that isn't declared by the user (like error or function symbol)
                // should produce an empty response
                response.Should().BeEmpty();
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task GoToDefinitionOnUnboundSyntaxNodeShouldReturnEmptyResponse(DataSet dataSet)
        {
            // local function
            bool IsUnboundNode(IDictionary<SyntaxBase, Symbol> dictionary, SyntaxBase syntax) => dictionary.ContainsKey(syntax) == false && !(syntax is Token);

            var uri = DocumentUri.From($"/{dataSet.Name}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(dataSet.Bicep));
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = TextCoordinateConverter.GetLineStarts(dataSet.Bicep);

            var unboundNodes = SyntaxAggregator.Aggregate(
                source: compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax,
                seed: new List<SyntaxBase>(),
                function: (accumulated, syntax) =>
                {
                    if (IsUnboundNode(symbolTable, syntax) && !(syntax is ProgramSyntax))
                    {
                        // only collect unbound nodes non-program nodes
                        accumulated.Add(syntax);
                    }

                    return accumulated;
                },
                resultSelector: accumulated => accumulated,
                // visit children only if current node is not bound
                continuationFunction: (accumulated, syntax) => IsUnboundNode(symbolTable, syntax));

            foreach (var syntax in unboundNodes)
            {
                var response = await client.RequestDefinition(new DefinitionParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = PositionHelper.GetPosition(lineStarts, syntax.Span.Position)
                });

                // go to definition on a syntax node that isn't bound to a symbol should produce an empty response
                response.Should().BeEmpty();
            }
        }

        private static LocationLink ValidateDefinitionResponse(LocationOrLocationLinks response)
        {
            // go to def should produce single result in all cases
            response.Should().HaveCount(1);
            var single = response.Single();

            single.IsLocation.Should().BeFalse();
            single.IsLocationLink.Should().BeTrue();

            single.Location.Should().BeNull();
            single.LocationLink.Should().NotBeNull();

            return single.LocationLink;
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.ToDynamicTestData();
        }
    }
}
