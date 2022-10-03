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
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Bicep.Core.Text;
using Bicep.Core.Navigation;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.Core.FileSystem;
using Bicep.LanguageServer;
using Bicep.Core.UnitTests.FileSystem;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class DefinitionTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static readonly SharedLanguageHelperManager DefaultServer = new();

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
        public async Task GoToDefinitionRequestOnValidSymbolReferenceShouldReturnLocationOfDeclaredSymbol(DataSet dataSet)
        {
            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);
            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, dataSet.Bicep, uri);

            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            // filter out symbols that don't have locations as well as locals with invalid identifiers
            // (locals are special because their full span is the same as the identifier span,
            // which makes it impossible to go to definition on a local with invalid identifiers)
            var declaredSymbolBindings = symbolTable
                .Where(pair => pair.Value is DeclaredSymbol && (pair.Value is not LocalVariableSymbol local || local.NameSyntax.IsValid))
                .Select(pair => new KeyValuePair<SyntaxBase, DeclaredSymbol>(pair.Key, (DeclaredSymbol)pair.Value));

            foreach (var (syntax, symbol) in declaredSymbolBindings)
            {
                var response = await helper.Client.RequestDefinition(new DefinitionParams
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

                if (syntax is ParameterDeclarationSyntax parameterSyntax)
                {
                    // we only underline the key of the param declaration syntax
                    link.OriginSelectionRange.Should().Be(parameterSyntax.Name.ToRange(lineStarts));
                }
                else if (syntax is ITopLevelNamedDeclarationSyntax namedSyntax)
                {
                    // Instead of underlining everything, we only underline the resource name
                    link.OriginSelectionRange.Should().Be(namedSyntax.Name.ToRange(lineStarts));
                }
                else
                {
                    // origin selection range should be the span of the syntax node that references the symbol
                    link.OriginSelectionRange.Should().Be(syntax.ToRange(lineStarts));
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task GoToDefinitionRequestOnUnsupportedOrInvalidSyntaxNodeShouldReturnEmptyResponse(DataSet dataSet)
        {
            var uri = DocumentUri.From($"/{dataSet.Name}");

            //using var helper = await LanguageServerHelper.StartServerWithTextAsync(this.TestContext, dataSet.Bicep, uri);
            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, dataSet.Bicep, uri);

            var (compilation, _, _) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            var undeclaredSymbolBindings = symbolTable.Where(pair => pair.Value is not DeclaredSymbol and not PropertySymbol);

            foreach (var (syntax, _) in undeclaredSymbolBindings)
            {
                var response = await helper.Client.RequestDefinition(new DefinitionParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = IntegrationTestHelper.GetPosition(lineStarts, syntax)
                });

                using (new AssertionScope().WithVisualCursor(compilation.SourceFileGrouping.EntryPoint, syntax.Span))
                {
                    // go to definition on a symbol that isn't declared by the user (like error or function symbol)
                    // should produce an empty response
                    response.Should().BeEmpty();
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task GoToDefinitionOnUnboundSyntaxNodeShouldReturnEmptyResponse(DataSet dataSet)
        {
            // local function
            bool IsUnboundNode(IDictionary<SyntaxBase, Symbol> dictionary, SyntaxBase syntax) => dictionary.ContainsKey(syntax) == false && !(syntax is Token);

            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);

            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, dataSet.Bicep, uri);

            var symbolTable = compilation.ReconstructSymbolTable();
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            var unboundNodes = SyntaxAggregator.Aggregate(
                source: compilation.SourceFileGrouping.EntryPoint.ProgramSyntax,
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

            for (int i = 0; i < unboundNodes.Count(); i++)
            {
                var syntax = unboundNodes[i];
                if (ValidUnboundNode(unboundNodes, i))
                {
                    continue;
                }
                var response = await helper.Client.RequestDefinition(new DefinitionParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = IntegrationTestHelper.GetPosition(lineStarts, syntax)
                });

                // go to definition on a syntax node that isn't bound to a symbol should produce an empty response
                response.Should().BeEmpty();
            }
        }

        [TestMethod]
        public async Task GoToDefinitionOnInvalidModuleShouldNotThrow()
        {
            var text = @"
module appPlanDeploy 'fake:fa|ke' = {
  name: 'planDeploy'
  scope: rg
  params: {
    namePrefix: 'hello'
  }
}

module appPlanDeploy2 'wrong|.bicep' = {
  name: 'planDeploy'
  scope: rg
  params: {
    namePrefix: 'hello'
  }
}
";
            await RunDefinitionScenarioTest(TestContext, text, results => results.Should().SatisfyRespectively(
                x => x.Should().BeEmpty(),
                x => x.Should().BeEmpty()));
        }

        [DataTestMethod]
        [DataRow("loadTextContent")]
        [DataRow("loadFileAsBase64")]
        [DataRow("loadJsonContent")]
        public async Task GoToDefinition_onLoadFunctionArgument_shouldNotThrow(string functionCall)
        {
            var text = $"var file = {functionCall}('file.j|son')";
            var files = new[] { ("file.json", "{\"test\":1235}") };
            await RunDefinitionScenarioTestWithFiles(TestContext, text,
                results => results.Should().SatisfyRespectively(
                    x => x.Should().NotBeEmpty().And.Subject.ElementAt(0).LocationLink!.TargetUri.Path.Should().EndWith("file.json")),
            files);
        }

        private static async Task RunDefinitionScenarioTest(TestContext testContext, string fileWithCursors, Action<List<LocationOrLocationLinks>> assertAction)
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new($"file:///{testContext.TestName}/path/to/main.bicep"), file);

            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(testContext, file, bicepFile.FileUri);

            var results = await RequestDefinitions(helper.Client, bicepFile, cursors);

            assertAction(results);
        }

        private static async Task RunDefinitionScenarioTestWithFiles(
            TestContext testContext,
            string fileWithCursors,
            Action<List<LocationOrLocationLinks>> assertAction,
            IEnumerable<(string fileName, string fileBody)> additionalFiles)
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new($"file:///{testContext.TestName}/path/to/main.bicep"), file);

            var langServerOptions = new Server.CreationOptions
            {
                FileResolver = new InMemoryFileResolver(additionalFiles.ToDictionary(x => new Uri($"file:///{testContext.TestName}/path/to/{x.fileName}"), x => x.fileBody))
            };
            var server = new SharedLanguageHelperManager();
            server.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext, langServerOptions));

            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(testContext, file, bicepFile.FileUri);
            var results = await RequestDefinitions(helper.Client, bicepFile, cursors);

            assertAction(results);

            await server.DisposeAsync();
        }

        private static async Task<List<LocationOrLocationLinks>> RequestDefinitions(ILanguageClient client, BicepFile bicepFile, IEnumerable<int> cursors)
        {
            var results = new List<LocationOrLocationLinks>();
            foreach (var cursor in cursors)
            {
                var result = await client.RequestDefinition(new DefinitionParams()
                {
                    TextDocument = new TextDocumentIdentifier(bicepFile.FileUri),
                    Position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor)
                });

                results.Add(result);
            }

            return results;
        }

        private bool ValidUnboundNode(List<SyntaxBase> accumulated, int index)
        {
            // Module path
            return index > 1
            && accumulated[index] is StringSyntax
            && accumulated[index - 1] is IdentifierSyntax
            && accumulated[index - 2] is ModuleDeclarationSyntax;
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
            return DataSets.NonStressDataSets.ToDynamicTestData();
        }
    }
}
