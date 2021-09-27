// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.IntegrationTests.Extensions;
using Bicep.LanguageServer.Utils;
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
    public class ReferencesTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task FindReferencesWithDeclarationsShouldProduceCorrectResults(DataSet dataSet)
        {
            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, dataSet.Bicep, uri);
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            // filter out bind failures and locals with invalid identifiers
            // (locals are special because their span is equal to their identifier span)
            var filteredSymbolTable = symbolTable.Where(pair => pair.Value.Kind != SymbolKind.Error && (pair.Value is not LocalVariableSymbol local || local.NameSyntax.IsValid));
            // TODO: Implement for PropertySymbol
            filteredSymbolTable = filteredSymbolTable.Where(pair => pair.Value is not PropertySymbol);
            var symbolToSyntaxLookup = filteredSymbolTable.ToLookup(pair => pair.Value, pair => pair.Key);

            foreach (var (syntax, symbol) in filteredSymbolTable)
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

                using (new AssertionScope()
                    .WithAnnotations(compilation.SourceFileGrouping.EntryPoint, "expected", expectedRanges, _ => "here", x => x)
                    .WithAnnotations(compilation.SourceFileGrouping.EntryPoint, "actual", locations, _ => "here", x => x.Range))
                {
                    // ranges should match what we got from our own symbol table
                    locations.Select(l => l.Range).Should().BeEquivalentTo(expectedRanges);
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task FindReferencesWithoutDeclarationsShouldProduceCorrectResults(DataSet dataSet)
        {
            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, dataSet.Bicep, uri);
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            // filter out bind failures and locals with invalid identifiers
            // (locals are special because their span is equal to their identifier span)
            var filteredSymbolTable = symbolTable.Where(pair => pair.Value.Kind != SymbolKind.Error && (pair.Value is not LocalVariableSymbol local || local.NameSyntax.IsValid));
            // TODO: Implement for PropertySymbol
            filteredSymbolTable = filteredSymbolTable.Where(pair => pair.Value is not PropertySymbol);
            var symbolToSyntaxLookup = filteredSymbolTable.ToLookup(pair => pair.Value, pair => pair.Key);

            foreach (var (syntax, symbol) in filteredSymbolTable)
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
                    .Where(node => !(node is INamedDeclarationSyntax))
                    .Select(node => PositionHelper.GetNameRange(lineStarts, node));

                using (new AssertionScope()
                    .WithAnnotations(compilation.SourceFileGrouping.EntryPoint, "expected", expectedRanges, _ => "here", x => x)
                    .WithAnnotations(compilation.SourceFileGrouping.EntryPoint, "actual", locations, _ => "here", x => x.Range))
                {
                    // ranges should match what we got from our own symbol table
                    locations.Select(l => l.Range).Should().BeEquivalentTo(expectedRanges);
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task FindReferencesOnNonSymbolsShouldProduceEmptyResult(DataSet dataSet)
        {
            // local function
            static bool IsWrongNode(SyntaxBase node) =>
                !(node is PropertyAccessSyntax propertyAccessSyntax && propertyAccessSyntax.BaseExpression is ISymbolReference) &&
                node is not ISymbolReference &&
                node is not ITopLevelNamedDeclarationSyntax &&
                node is not Token;

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

        [TestMethod]
        public async Task FindReferences_displays_references_on_namespaced_and_non_namespaced_methods()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var rg1 = resourc|eGroup().location

var rg2 = az.r|esourceGroup()

var rg3 = resourceGr|oup().id

var dep1 = az.depl|oyment()

var dep2 = az.deploy|ment()
");

            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, file, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create()));
            var references = await RequestReferences(client, bicepFile, cursors);
            
            references.Should().SatisfyRespectively(
                r => r.Should().HaveCount(3),
                r => r.Should().HaveCount(3),
                r => r.Should().HaveCount(3),
                r => r.Should().HaveCount(2),
                r => r.Should().HaveCount(2));
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.NonStressDataSets.ToDynamicTestData();
        }

        private static async Task<IEnumerable<LocationContainer?>> RequestReferences(ILanguageClient client, BicepFile bicepFile, IEnumerable<int> cursors)
        {
            var references = new List<LocationContainer?>();
            foreach (var cursor in cursors)
            {
                var referenceList = await client.RequestReferences(new ReferenceParams
                {
                    TextDocument = new TextDocumentIdentifier(bicepFile.FileUri),
                    Context = new ReferenceContext
                    {
                        IncludeDeclaration = false
                    },
                    Position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor),
                });

                references.Add(referenceList);
            }

            return references;
        }
    }
}
