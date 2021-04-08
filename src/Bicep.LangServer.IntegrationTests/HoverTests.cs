// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Azure.Bicep.Types.Az;
using Bicep.Core.Extensions;
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
    public class HoverTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static IAssertionScope CreateAssertionScopeWithContext(SyntaxTree syntaxTree, Hover? hover, IPositionable requestedPosition)
        {
            var assertionScope = new AssertionScope();

            // TODO: figure out how to set this only on failure, rather than always calculating it
            assertionScope.AddReportable(
                "hover context",
                PrintHelper.PrintWithAnnotations(syntaxTree, new [] { 
                    new PrintHelper.Annotation(requestedPosition.Span, "cursor position"),
                }, 1, true));

            return assertionScope;
        }

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
                var nodeForHover = symbolReference switch
                {
                    ITopLevelDeclarationSyntax d => d.Keyword,
                    ResourceAccessSyntax r => r.ResourceName,
                    _ => symbolReference,
                };

                var hover = await client.RequestHover(new HoverParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = PositionHelper.GetPosition(lineStarts, nodeForHover.Span.Position)
                });

                // fancy method to give us some annotated source code to look at if any assertions fail :)
                using (CreateAssertionScopeWithContext(compilation.SyntaxTreeGrouping.EntryPoint, hover, nodeForHover.Span.ToZeroLengthSpan()))
                {
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

                // fancy method to give us some annotated source code to look at if any assertions fail :)
                using (CreateAssertionScopeWithContext(compilation.SyntaxTreeGrouping.EntryPoint, hover, node.Span.ToZeroLengthSpan()))
                {
                    hover.Should().BeNull();
                }
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
