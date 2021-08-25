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
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.LangServer.IntegrationTests.Extensions;
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
    public class RenameSymbolTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task RenamingIdentifierAccessOrDeclarationShouldRenameDeclarationAndAllReferences(DataSet dataSet)
        {
            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, dataSet.Bicep, uri);
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            var symbolToSyntaxLookup = symbolTable
                .Where(pair => pair.Value.Kind != SymbolKind.Error)
                .ToLookup(pair => pair.Value, pair => pair.Key);

            var validVariableAccessPairs = symbolTable
                .Where(pair => (pair.Key is VariableAccessSyntax || pair.Key is ResourceAccessSyntax || pair.Key is ITopLevelNamedDeclarationSyntax)
                               && pair.Value.Kind != SymbolKind.Error
                               && pair.Value.Kind != SymbolKind.Function
                               && pair.Value.Kind != SymbolKind.Namespace
                               // symbols whose identifiers have parse errors will have a name like <error> or <missing>
                               && pair.Value.Name.Contains("<") == false);
            
            const string expectedNewText = "NewIdentifier";
            foreach (var (syntax, symbol) in validVariableAccessPairs)
            {
                var edit = await client.RequestRename(new RenameParams
                {
                    NewName = expectedNewText,
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = IntegrationTestHelper.GetPosition(lineStarts, syntax)
                });

                edit.Should().NotBeNull();
                edit!.DocumentChanges.Should().BeNullOrEmpty();
                edit.Changes.Should().NotBeNull();
                edit.Changes.Should().HaveCount(1);
                edit.Changes.Should().ContainKey(uri);

                var textEdits = edit.Changes![uri];
                textEdits.Should().NotBeEmpty();

                var expectedEdits = symbolToSyntaxLookup[symbol]
                    .Select(node => CreateExpectedTextEdit(lineStarts, expectedNewText, node));

                textEdits.Should().BeEquivalentTo(expectedEdits);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task RenamingFunctionsShouldProduceEmptyEdit(DataSet dataSet)
        {
            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, dataSet.Bicep, uri);
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            var validFunctionCallPairs = symbolTable
                .Where(pair => pair.Value.Kind == SymbolKind.Function)
                .Select(pair=>pair.Key);

            foreach (var syntax in validFunctionCallPairs)
            {
                var edit = await client.RequestRename(new RenameParams
                {
                    NewName = "NewIdentifier",
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = IntegrationTestHelper.GetPosition(lineStarts, syntax)
                });

                edit.Should().BeNull();
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task RenamingNonSymbolsShouldProduceEmptyEdit(DataSet dataSet)
        {
            // local function
            bool IsWrongNode(SyntaxBase node) => !(node is ISymbolReference) && !(node is ITopLevelNamedDeclarationSyntax) && !(node is Token);

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
                var edit = await client.RequestRename(new RenameParams
                {
                    NewName = "NewIdentifier",
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = IntegrationTestHelper.GetPosition(lineStarts, syntax)
                });

                edit.Should().BeNull();
            }
        }

        private static TextEdit CreateExpectedTextEdit(ImmutableArray<int> lineStarts, string newText, SyntaxBase syntax) =>
            new TextEdit
            {
                NewText = newText,
                Range = PositionHelper.GetNameRange(lineStarts, syntax)
            };

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.NonStressDataSets.ToDynamicTestData();
        }
    }
}
