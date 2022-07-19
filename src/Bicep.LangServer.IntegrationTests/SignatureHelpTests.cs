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
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Extensions;
using Bicep.LangServer.IntegrationTests.Helpers;
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
    public class SignatureHelpTests
    {
        private static readonly SharedLanguageHelperManager DefaultServer = new();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DefaultServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext));
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await DefaultServer.DisposeAsync();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task ShouldProvideSignatureHelpBetweenFunctionParentheses(DataSet dataSet)
        {
            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);

            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, dataSet.Bicep, uri);

            var symbolTable = compilation.ReconstructSymbolTable();
            var tree = compilation.SourceFileGrouping.EntryPoint;

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
                var expectDecorator = compilation.GetEntrypointSemanticModel().Binder.GetParent(functionCall) is DecoratorSyntax;

                var symbol = compilation.GetEntrypointSemanticModel().GetSymbolInfo(functionCall);

                // if the cursor is present immediate after the function argument opening paren,
                // the signature help can only show the signature of the enclosing function
                var startOffset = functionCall.OpenParen.GetEndPosition();
                await ValidateOffset(helper.Client, uri, tree, startOffset, symbol as FunctionSymbol, expectDecorator);

                // if the cursor is present immediately before the function argument closing paren,
                // the signature help can only show the signature of the enclosing function
                var endOffset = functionCall.CloseParen.Span.Position;
                await ValidateOffset(helper.Client, uri, tree, endOffset, symbol as FunctionSymbol, expectDecorator);
            }
        }

        [TestMethod]
        public async Task NonExistentUriShouldProvideNoSignatureHelp()
        {
            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, string.Empty, DocumentUri.From("/fake.bicep"));

            var signatureHelp = await RequestSignatureHelp(helper.Client, new Position(0, 0), DocumentUri.From("/fake2.bicep"));
            signatureHelp.Should().BeNull();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task NonFunctionCallSyntaxShouldProvideNoSignatureHelp(DataSet dataSet)
        {
            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);

            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, dataSet.Bicep, uri);

            var bicepFile = compilation.SourceFileGrouping.EntryPoint;

            var nonFunctions = SyntaxAggregator.Aggregate(
                bicepFile.ProgramSyntax,
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
                using (new AssertionScope().WithVisualCursor(bicepFile, nonFunction.Span.ToZeroLengthSpan()))
                {
                    var position = PositionHelper.GetPosition(bicepFile.LineStarts, nonFunction.Span.Position);
                    var signatureHelp = await RequestSignatureHelp(helper.Client, position, uri);
                    signatureHelp.Should().BeNull();
                }
            }
        }

        private static async Task ValidateOffset(ILanguageClient client, DocumentUri uri, BicepFile bicepFile, int offset, FunctionSymbol? symbol, bool expectDecorator)
        {
            var position = PositionHelper.GetPosition(bicepFile.LineStarts, offset);
            var initial = await RequestSignatureHelp(client, position, uri);

            // fancy method to give us some annotated source code to look at if any assertions fail :)
            using (new AssertionScope().WithVisualCursor(bicepFile, new TextSpan(offset, 0)))
            {
                if (symbol is not null)
                {
                    // real function should have valid signature help
                    AssertValidSignatureHelp(initial, symbol, expectDecorator);

                    if (initial!.Signatures.Count() >= 2)
                    {
                        // update index to 1 to mock user changing active signature
                        const int ExpectedActiveSignatureIndex = 1;
                        var modified = initial with
                        {
                            ActiveSignature = ExpectedActiveSignatureIndex
                        };

                        var shouldRemember = await RequestSignatureHelp(client, position, uri, new SignatureHelpContext
                        {
                            ActiveSignatureHelp = modified,
                            IsRetrigger = true,
                            TriggerKind = SignatureHelpTriggerKind.ContentChange
                        });

                        // we passed the same signature help as content with a different active index
                        // should get the same index back
                        AssertValidSignatureHelp(shouldRemember, symbol, expectDecorator);
                        shouldRemember!.ActiveSignature.Should().Be(ExpectedActiveSignatureIndex);
                    }
                }
                else
                {
                    // not a real function - no signature help expected
                    initial.Should().BeNull();
                }
            }
        }

        private static void AssertValidSignatureHelp(SignatureHelp? signatureHelp, FunctionSymbol symbol, bool expectDecorator)
        {
            signatureHelp.Should().NotBeNull();

            signatureHelp!.Signatures.Should().NotBeNull();
            foreach (var signature in signatureHelp.Signatures)
            {
                var isWildcardListFunction = signature.Label.StartsWith("list*");
                var isWellKnownListFunction = signature.Label.StartsWith("list") && !isWildcardListFunction;
                if (isWildcardListFunction)
                {
                    symbol.Overloads.Should().Contain(x => x is FunctionWildcardOverload && x.Name == "list*");
                }
                else
                {
                    signature.Label.Should().StartWith($"{symbol.Name}(");
                }

                if (expectDecorator)
                {
                    // decorators should have no return type
                    signature.Label.Should().EndWith(")");
                }
                else
                {
                    // normal function calls should include a return type
                    signature.Label.Should().Contain("): ");
                }

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

                // List functions provided by the bicep-types-az library do not contain documentation: https://github.com/Azure/bicep/issues/7611
                if (!isWellKnownListFunction)
                {
                    signature.Documentation.MarkupContent.Value.Should().NotBeEmpty();
                }
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
