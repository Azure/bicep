// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Extensions;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class SignatureHelpTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task ShouldProvideSignatureHelpBetweenFunctionParentheses(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);
            var compilation = dataSet.CopyFilesAndCreateCompilation(TestContext, out _);
            var symbolTable = compilation.ReconstructSymbolTable();
            var tree = compilation.SyntaxTreeGrouping.EntryPoint;

            var functionCalls = SyntaxAggregator.Aggregate(
                tree.ProgramSyntax,
                new List<FunctionCallSyntaxBase>(),
                (accumulated, current) =>
                {
                    if (current is FunctionCallSyntaxBase functionCallBase)
                    {
                        accumulated.Add(functionCallBase);
                    }

                    return accumulated;
                },
                accumulated => accumulated);

            
            foreach (FunctionCallSyntaxBase functionCall in functionCalls)
            {
                var symbol = compilation.GetEntrypointSemanticModel().GetSymbolInfo(functionCall);
                
                // if the cursor is present immediate after the function argument opening paren,
                // the signature help can only show the signature of the enclosing function
                var startOffset = functionCall.OpenParen.GetEndPosition();
                await ValidateOffset(compilation, client, uri, tree, startOffset, symbol as FunctionSymbol);
                
                // if the cursor is present immediately before the function argument closing paren,
                // the signature help can only show the signature of the enclosing function
                var endOffset = functionCall.CloseParen.Span.Position;
                await ValidateOffset(compilation, client, uri, tree, endOffset, symbol as FunctionSymbol);
            }
        }

        [TestMethod]
        public async Task NonExistentUriShouldProvideNoSignatureHelp()
        {
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(string.Empty, DocumentUri.From("/fake.bicep"));

            var signatureHelp = await RequestSignatureHelp(client, new Position(0, 0), DocumentUri.From("/fake2.bicep"));
            signatureHelp.Should().BeNull();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task NonFunctionCallSyntaxShouldProvideNoSignatureHelp(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri);
            var compilation = dataSet.CopyFilesAndCreateCompilation(TestContext, out _);
            var tree = compilation.SyntaxTreeGrouping.EntryPoint;

            var nonFunctions = SyntaxAggregator.Aggregate(
                tree.ProgramSyntax,
                new List<SyntaxBase>(),
                (accumulated, current) =>
                {
                    if (current is not FunctionCallSyntaxBase)
                    {
                        accumulated.Add(current);
                    }

                    return accumulated;
                },
                accumulated => accumulated,
                // requesting signature help on non-function nodes that are placed inside function call nodes will produce signature help
                // since we don't want that, stop the visitor from visiting inner nodes when a function call is encountered
                (accumulated, current) => current is not FunctionCallSyntaxBase);

            foreach (var nonFunction in nonFunctions)
            {
                using (new AssertionScope().WithVisualCursor(tree, nonFunction.Span.ToZeroLengthSpan()))
                {
                    var position = PositionHelper.GetPosition(tree.LineStarts, nonFunction.Span.Position);
                    var signatureHelp = await RequestSignatureHelp(client, position, uri);
                    signatureHelp.Should().BeNull();
                }
            }
        }

        private static async Task ValidateOffset(Compilation compilation, ILanguageClient client, DocumentUri uri, SyntaxTree tree, int offset, FunctionSymbol? symbol)
        {
            var position = PositionHelper.GetPosition(tree.LineStarts, offset);
            var initial = await RequestSignatureHelp(client, position, uri);

            // fancy method to give us some annotated source code to look at if any assertions fail :)
            using (new AssertionScope().WithVisualCursor(tree, new TextSpan(offset, 0)))
            {
                if (symbol is not null)
                {
                    // real function should have valid signature help
                    AssertValidSignatureHelp(initial, symbol);

                    if (initial!.Signatures.Count() >= 2)
                    {
                        // update index to 1 to mock user changing active signature
                        initial.ActiveSignature = 1;

                        var shouldRemember = await RequestSignatureHelp(client, position, uri, new SignatureHelpContext
                        {
                            ActiveSignatureHelp = initial,
                            IsRetrigger = true,
                            TriggerKind = SignatureHelpTriggerKind.ContentChange
                        });

                        // we passed the same signature help as content with a different active index
                        // should get the same index back
                        AssertValidSignatureHelp(shouldRemember, symbol);
                        shouldRemember!.ActiveSignature.Should().Be(1);
                    }
                }
                else
                {
                    // not a real function - no signature help expected
                    initial.Should().BeNull();
                }
            }
        }

        private static void AssertValidSignatureHelp(SignatureHelp? signatureHelp, Symbol symbol)
        {
            signatureHelp.Should().NotBeNull();

            signatureHelp!.Signatures.Should().NotBeNull();
            foreach (var signature in signatureHelp.Signatures)
            {
                signature.Label.Should().StartWith(symbol.Name.StartsWith("list") ? "list*(" : $"{symbol.Name}(");

                signature.Label.Should().EndWith(")");

                signature.Parameters.Should().NotBeNull();

                if (signature.Parameters!.Count() >= 2)
                {
                    signature.Label.Should().Contain(", ");
                }

                // we use the top level active parameter index
                signature.ActiveParameter.Should().BeNull();

                signature.Documentation.Should().NotBeNull();
                signature.Documentation!.MarkupContent.Should().NotBeNull();
                signature.Documentation.MarkupContent!.Kind.Should().Be(MarkupKind.Markdown);
                signature.Documentation.MarkupContent.Value.Should().NotBeEmpty();
            }
        }

        private static async Task<SignatureHelp?> RequestSignatureHelp(ILanguageClient client, Position position, DocumentUri uri, SignatureHelpContext? context = null) =>
            await client.RequestSignatureHelp(new SignatureHelpParams
            {
                Position = position,
                TextDocument = new TextDocumentIdentifier(uri),
                Context = context ?? new SignatureHelpContext
                {
                    TriggerKind = SignatureHelpTriggerKind.Invoked,
                    IsRetrigger = false
                }
            });

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.NonStressDataSets.ToDynamicTestData();
        }
    }
}
