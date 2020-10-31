// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bicep.Core.Navigation;
using Bicep.Core.Parser;
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.IntegrationTests.Extensions;
using Bicep.LangServer.IntegrationTests.Helpers;
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
    public class HoverTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task HoveringOverSymbolReferencesAndDeclarationsShouldProduceHovers(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");
            var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);

            // construct a parallel compilation
            var compilation = dataSet.CopyFilesAndCreateCompilation(TestContext, out _);
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SyntaxTreeGrouping.EntryPoint.LineStarts;

            var symbolReferences = SyntaxAggregator.Aggregate(
                compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax,
                new List<SyntaxBase>(),
                (accumulated, node) =>
                {
                    if (node is ISymbolReference || node is IDeclarationSyntax)
                    {
                        accumulated.Add(node);
                    }

                    return accumulated;
                },
                accumulated => accumulated);

            foreach (SyntaxBase symbolReference in symbolReferences)
            {
                var hover = await client.RequestHover(new HoverParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = PositionHelper.GetPosition(lineStarts, symbolReference.Span.Position)
                });

                if (symbolTable.TryGetValue(symbolReference, out var symbol) == false)
                {
                    // symbol ref not bound to a symbol
                    hover.Should().BeNull();
                    continue;
                }

                switch (symbol!.Kind)
                {
                    case SymbolKind.Function when symbolReference is VariableAccessSyntax:
                        // variable got bound to a function
                        hover.Should().BeNull();
                        break;

                    // when a namespace value is found and there was an error with the function call or
                    // is a valid function call or namespace access, all these cases will have a hover range
                    // with some text
                    case SymbolKind.Error when symbolReference is InstanceFunctionCallSyntax:
                    case SymbolKind.Function when symbolReference is InstanceFunctionCallSyntax:
                    case SymbolKind.Namespace:
                        ValidateInstanceFunctionCallHover(hover);
                        break;

                    case SymbolKind.Error:
                        // error symbol
                        hover.Should().BeNull();
                        break;

                    default:
                        ValidateHover(hover, symbol);
                        break;
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task HoveringOverNonHoverableElementsShouldProduceEmptyHovers(DataSet dataSet)
        {
            // local function
            bool IsNonHoverable(SyntaxBase node) => !(node is ISymbolReference) && !(node is IDeclarationSyntax) && !(node is Token);

            var uri = DocumentUri.From($"/{dataSet.Name}");
            var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);

            // construct a parallel compilation
            var compilation = dataSet.CopyFilesAndCreateCompilation(TestContext, out _);
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SyntaxTreeGrouping.EntryPoint.LineStarts;

            var nonHoverableNodes = SyntaxAggregator.Aggregate(
                compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax,
                new List<SyntaxBase>(),
                (accumulated, node) =>
                {
                    if (IsNonHoverable(node) && !(node is ProgramSyntax))
                    {
                        accumulated.Add(node);
                    }

                    return accumulated;
                },
                accumulated => accumulated,
                // don't visit hoverable nodes or their children
                (accumulated, node) => IsNonHoverable(node));

            foreach (SyntaxBase node in nonHoverableNodes)
            {
                var hover = await client.RequestHover(new HoverParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = PositionHelper.GetPosition(lineStarts, node.Span.Position)
                });

                hover.Should().BeNull();
            }
        }

        private static void ValidateHover(Hover? hover, Symbol symbol)
        {
            hover.Should().NotBeNull();
            hover!.Range!.Should().NotBeNull();
            hover.Contents.Should().NotBeNull();

            hover.Contents.HasMarkedStrings.Should().BeFalse();
            hover.Contents.HasMarkupContent.Should().BeTrue();
            hover.Contents.MarkedStrings.Should().BeNull();
            hover.Contents.MarkupContent.Should().NotBeNull();

            hover.Contents.MarkupContent!.Kind.Should().Be(MarkupKind.Markdown);
            hover.Contents.MarkupContent.Value.Should().StartWith("```bicep\n");
            hover.Contents.MarkupContent.Value.Should().EndWith("```");

            switch (symbol)
            {
                case ParameterSymbol parameter:
                    hover.Contents.MarkupContent.Value.Should().Contain($"param {parameter.Name}: {parameter.Type}");
                    break;

                case VariableSymbol variable:
                    hover.Contents.MarkupContent.Value.Should().Contain($"var {variable.Name}: {variable.Type}");
                    break;

                case ResourceSymbol resource:
                    hover.Contents.MarkupContent.Value.Should().Contain($"resource {resource.Name}");
                    hover.Contents.MarkupContent.Value.Should().Contain(resource.Type.Name);
                    break;

                case ModuleSymbol module:
                    hover.Contents.MarkupContent.Value.Should().Contain($"module {module.Name}");
                    break;

                case OutputSymbol output:
                    hover.Contents.MarkupContent.Value.Should().Contain($"output {output.Name}: {output.Type}");
                    break;

                case FunctionSymbol function:
                    hover.Contents.MarkupContent.Value.Should().Contain($"function {function.Name}(");
                    break;

                default:
                    throw new AssertFailedException($"Unexpected symbol type '{symbol.GetType().Name}'");
            }
        }

        private static void ValidateInstanceFunctionCallHover(Hover? hover)
        {
            hover.Should().NotBeNull();
            hover!.Range!.Should().NotBeNull();
            hover.Contents.Should().NotBeNull();

            hover.Contents.HasMarkedStrings.Should().BeFalse();
            hover.Contents.HasMarkupContent.Should().BeTrue();
            hover.Contents.MarkedStrings.Should().BeNull();
            hover.Contents.MarkupContent.Should().NotBeNull();

            hover.Contents.MarkupContent!.Kind.Should().Be(MarkupKind.Markdown);
            hover.Contents.MarkupContent.Value.Should().StartWith("```bicep\n");
            hover.Contents.MarkupContent.Value.Should().EndWith("```");
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.ToDynamicTestData();
        }
    }
}
