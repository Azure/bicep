// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Azure.Bicep.Types.Az;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Az;
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
using SymbolKind = Bicep.Core.Semantics.SymbolKind;

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
            var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri, resourceTypeProvider: new AzResourceTypeProvider(new TypeLoader()));

            // construct a parallel compilation
            var compilation = dataSet.CopyFilesAndCreateCompilation(TestContext, out _);

            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SyntaxTreeGrouping.EntryPoint.LineStarts;

            var symbolReferences = SyntaxAggregator.Aggregate(
                compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax,
                new List<SyntaxBase>(),
                (accumulated, node) =>
                {
                    if (node is ISymbolReference || node is ITopLevelNamedDeclarationSyntax)
                    {
                        accumulated.Add(node);
                    }

                    return accumulated;
                },
                accumulated => accumulated);

            foreach (SyntaxBase symbolReference in symbolReferences)
            {
                var syntaxPosition = symbolReference switch
                {
                    ITopLevelDeclarationSyntax d => d.Keyword.Span.Position,
                    ResourceAccessSyntax r => r.ResourceName.Span.Position,
                    _ => symbolReference.Span.Position,
                };

                var hover = await client.RequestHover(new HoverParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = PositionHelper.GetPosition(lineStarts, syntaxPosition)
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
            static bool IsNonHoverable(SyntaxBase node) =>
                !(node is PropertyAccessSyntax propertyAccessSyntax && propertyAccessSyntax.BaseExpression is ISymbolReference) &&
                node is not ISymbolReference &&
                node is not ITopLevelNamedDeclarationSyntax &&
                node is not Token;

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
                    // the hovers with errors don't appear in VS code and only occur in tests
                    hover.Contents.MarkupContent.Value.Should().Match(value => value.Contains($"var {variable.Name}: {variable.Type}") || value.Contains($"var {variable.Name}: error"));
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

                case LocalVariableSymbol local:
                    hover.Contents.MarkupContent.Value.Should().Contain($"{local.Name}: {local.Type}");
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
            return DataSets.NonStressDataSets.ToDynamicTestData();
        }
    }
}
