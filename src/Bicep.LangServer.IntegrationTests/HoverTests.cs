// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.IntegrationTests.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
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
            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, dataSet.Bicep, uri, creationOptions: new LanguageServer.Server.CreationOptions(ResourceTypeProvider: AzResourceTypeProvider.CreateWithAzTypes(), FileResolver: BicepTestConstants.FileResolver));

            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            var symbolReferences = SyntaxAggregator.Aggregate(
                compilation.SourceFileGrouping.EntryPoint.ProgramSyntax,
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

            foreach (var symbolReference in symbolReferences)
            {
                // by default, request a hover on the first character of the syntax, but for certain syntaxes, this doesn't make sense.
                // for example on an instance function call 'az.resourceGroup()', it only makes sense to request a hover on the 3rd character.
                var nodeForHover = symbolReference switch
                {
                    ITopLevelDeclarationSyntax d => d.Keyword,
                    ResourceAccessSyntax r => r.ResourceName,
                    FunctionCallSyntaxBase f => f.Name,
                    _ => symbolReference,
                };

                var hover = await client.RequestHover(new HoverParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = TextCoordinateConverter.GetPosition(lineStarts, nodeForHover.Span.Position)
                });

                // fancy method to give us some annotated source code to look at if any assertions fail :)
                using (new AssertionScope().WithVisualCursor(compilation.SourceFileGrouping.EntryPoint, nodeForHover.Span.ToZeroLengthSpan()))
                {
                    if (!symbolTable.TryGetValue(symbolReference, out var symbol))
                    {
                        if (symbolReference is InstanceFunctionCallSyntax &&
                            compilation.GetEntrypointSemanticModel().GetSymbolInfo(symbolReference) is FunctionSymbol ifcSymbol)
                        {
                            ValidateHover(hover, ifcSymbol);
                            break;
                        }

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

            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, dataSet.Bicep, uri);

            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            var nonHoverableNodes = SyntaxAggregator.Aggregate(
                compilation.SourceFileGrouping.EntryPoint.ProgramSyntax,
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
                    Position = TextCoordinateConverter.GetPosition(lineStarts, node.Span.Position)
                });

                // fancy method to give us some annotated source code to look at if any assertions fail :)
                using (new AssertionScope().WithVisualCursor(compilation.SourceFileGrouping.EntryPoint, node.Span.ToZeroLengthSpan()))
                {
                    hover.Should().BeNull();
                }
            }
        }


        [DataTestMethod]
        public async Task PropertyHovers_are_displayed_on_properties()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
resource testRes 'Test.Rp/readWriteTests@2020-01-01' = {
  n|ame: 'testRes'
  prop|erties: {
    readwri|te: 'abc'
    write|only: 'def'
    requ|ired: 'ghi'
  }
}

output string test = testRes.prop|erties.rea|donly
");

            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, file, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(ResourceTypeProvider: BuiltInTestTypes.Create()));
            var hovers = await RequestHovers(client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nname: string\n```\nname property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```\nproperties property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadwrite: string\n```\nThis is a property which supports reading AND writing!\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nwriteonly: string\n```\nThis is a property which only supports writing.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nrequired: string\n```\nThis is a property which is required.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```\nproperties property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadonly: string\n```\nThis is a property which only supports reading.\n"));
        }


        [DataTestMethod]
        public async Task PropertyHovers_are_displayed_on_properties_with_loops()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
resource testRes 'Test.Rp/readWriteTests@2020-01-01' = [for i in range(0, 10): {
  n|ame: 'testRes${i}'
  prop|erties: {
    readwri|te: 'abc'
    write|only: 'def'
    requ|ired: 'ghi'
  }
}]

output string test = testRes[3].prop|erties.rea|donly
");

            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, file, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(ResourceTypeProvider: BuiltInTestTypes.Create()));
            var hovers = await RequestHovers(client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nname: string\n```\nname property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```\nproperties property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadwrite: string\n```\nThis is a property which supports reading AND writing!\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nwriteonly: string\n```\nThis is a property which only supports writing.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nrequired: string\n```\nThis is a property which is required.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```\nproperties property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadonly: string\n```\nThis is a property which only supports reading.\n"));
        }


        [DataTestMethod]
        public async Task PropertyHovers_are_displayed_on_properties_with_conditions()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
resource testRes 'Test.Rp/readWriteTests@2020-01-01' = if (true) {
  n|ame: 'testRes'
  prop|erties: {
    readwri|te: 'abc'
    write|only: 'def'
    requ|ired: 'ghi'
  }
}

output string test = testRes.prop|erties.rea|donly
");

            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, file, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(ResourceTypeProvider: BuiltInTestTypes.Create()));
            var hovers = await RequestHovers(client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nname: string\n```\nname property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```\nproperties property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadwrite: string\n```\nThis is a property which supports reading AND writing!\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nwriteonly: string\n```\nThis is a property which only supports writing.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nrequired: string\n```\nThis is a property which is required.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```\nproperties property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadonly: string\n```\nThis is a property which only supports reading.\n"));
        }

        [DataTestMethod]
        public async Task PropertyHovers_are_displayed_on_partial_discriminator_objects()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
resource testRes 'Test.Rp/discriminatorTests@2020-01-01' = {
  ki|nd
}
");

            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, file, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(ResourceTypeProvider: BuiltInTestTypes.Create()));
            var hovers = await RequestHovers(client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nkind: 'BodyA' | 'BodyB'\n```\n"));
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
                    hover.Contents.MarkupContent.Value.Should().ContainAny(new[] { $"var {variable.Name}: {variable.Type}", $"var {variable.Name}: error" });
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

                case NamespaceSymbol @namespace:
                    hover.Contents.MarkupContent.Value.Should().Contain($"{@namespace.Name} namespace");
                    break;

                default:
                    throw new AssertFailedException($"Unexpected symbol type '{symbol.GetType().Name}'");
            }
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.NonStressDataSets.ToDynamicTestData();
        }

        private static async Task<IEnumerable<Hover?>> RequestHovers(ILanguageClient client, BicepFile bicepFile, IEnumerable<int> cursors)
        {
            var hovers = new List<Hover?>();
            foreach (var cursor in cursors)
            {
                var hover = await client.RequestHover(new HoverParams
                {
                    TextDocument = new TextDocumentIdentifier(bicepFile.FileUri),
                    Position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor),
                });

                hovers.Add(hover);
            }

            return hovers;
        }
    }
}
