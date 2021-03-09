// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
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
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task GoToDefinitionRequestOnValidSymbolReferenceShouldReturnLocationOfDeclaredSymbol(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);
            var compilation = dataSet.CopyFilesAndCreateCompilation(TestContext, out _);
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SyntaxTreeGrouping.EntryPoint.LineStarts;

            // filter out symbols that don't have locations as well as locals with invalid identifiers
            // (locals are special because their full span is the same as the identifier span,
            // which makes it impossible to go to definition on a local with invalid identifiers)
            var declaredSymbolBindings = symbolTable
                .Where(pair => pair.Value is DeclaredSymbol && (pair.Value is not LocalVariableSymbol local || local.NameSyntax.IsValid))
                .Select(pair => new KeyValuePair<SyntaxBase, DeclaredSymbol>(pair.Key, (DeclaredSymbol) pair.Value));

            foreach (var (syntax, symbol) in declaredSymbolBindings)
            {
                var response = await client.RequestDefinition(new DefinitionParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = IntegrationTestHelper.GetPosition(lineStarts, syntax)
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
            var compilation = dataSet.CopyFilesAndCreateCompilation(TestContext, out _);
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SyntaxTreeGrouping.EntryPoint.LineStarts;

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
            var compilation = dataSet.CopyFilesAndCreateCompilation(TestContext, out _);
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SyntaxTreeGrouping.EntryPoint.LineStarts;

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
                var offset = syntax switch
                {
                    // base expression could be a variable access which is bound and will throw off the test
                    PropertyAccessSyntax propertyAccess => propertyAccess.PropertyName.Span.Position,
                    ArrayAccessSyntax arrayAccess => arrayAccess.OpenSquare.Span.Position,

                    _ => syntax.Span.Position
                };

                var response = await client.RequestDefinition(new DefinitionParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = PositionHelper.GetPosition(lineStarts, offset)
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

            return single.LocationLink!;
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.ToDynamicTestData();
        }
    }
}
