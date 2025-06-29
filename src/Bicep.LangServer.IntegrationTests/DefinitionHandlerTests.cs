// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.IO.InMemory;
using Bicep.LangServer.IntegrationTests.Extensions;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using Bicep.TextFixtures.IO;
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
    public class DefinitionHandlerTests
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
                .Where(pair => pair.Value is DeclaredSymbol && (pair.Value is not LocalVariableSymbol local || local.NameSource.IsValid))
                .Select(pair => new KeyValuePair<SyntaxBase, DeclaredSymbol>(pair.Key, (DeclaredSymbol)pair.Value));

            foreach (var (syntax, symbol) in declaredSymbolBindings)
            {
                var response = await helper.Client.RequestDefinition(new DefinitionParams
                {
                    TextDocument = new TextDocumentIdentifier(uri),
                    Position = IntegrationTestHelper.GetPosition(lineStarts, syntax)
                });

                var link = ValidateDefinitionResponse(response!);

                if (symbol is not ImportedSymbol and not WildcardImportSymbol)
                {
                    // document should match the requested document
                    link.TargetUri.Should().Be(uri);

                    // target range should be the whole span of the symbol
                    link.TargetRange.Should().Be(symbol.DeclaringSyntax.Span.ToRange(lineStarts));

                    // selection range should be the span of the identifier of the symbol
                    link.TargetSelectionRange.Should().Be(symbol.NameSource.Span.ToRange(lineStarts));
                }

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
            await RunDefinitionScenarioTest(TestContext, text, '|', results => results.Should().SatisfyRespectively(
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
            await RunDefinitionScenarioTestWithFiles(TestContext, text, '|',
                results => results.Should().SatisfyRespectively(
                    x => x.Should().NotBeEmpty().And.Subject.ElementAt(0).LocationLink!.TargetUri.Path.Should().EndWith("file.json")),
            files);
        }

        [TestMethod]
        public async Task Goto_definition_works_with_using_statement()
        {
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext);
            var helper = new ServerRequestHelper(TestContext, server);

            var (contents, cursor) = ParserHelper.GetFileWithSingleCursor(@"
using 'main.bi|cep'
");
            var (bicepContents, bicepCursors) = ParserHelper.GetFileWithCursors(@"||
var foo = 'foo'
");

            await helper.OpenFile("/main.bicep", bicepContents);
            var file = await helper.OpenFile("/main.bicepparam", contents);

            var response = await file.GotoDefinition(cursor);

            var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(bicepContents), bicepCursors[0], bicepCursors[1]);
            response.TargetUri.Path.Should().Be("/main.bicep");
            response.TargetRange.Should().Be(expectedRange);
        }

        [TestMethod]
        public async Task Goto_definition_works_with_param_assignment_statements()
        {
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext);
            var helper = new ServerRequestHelper(TestContext, server);

            var (contents, cursor) = ParserHelper.GetFileWithSingleCursor(@"
using 'main.bicep'

param f|oo = 'foo'
");
            var (bicepContents, bicepCursors) = ParserHelper.GetFileWithCursors(@"
@description('foo param')
param |foo| string
");
            await helper.OpenFile("/main.bicep", bicepContents);
            var file = await helper.OpenFile("/main.bicepparam", contents);

            var response = await file.GotoDefinition(cursor);

            var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(bicepContents), bicepCursors[0], bicepCursors[1]);
            response.TargetUri.Path.Should().Be("/main.bicep");
            response.TargetRange.Should().Be(expectedRange);
        }

        [TestMethod]
        public async Task Goto_definition_works_with_import_statement()
        {
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext);
            var helper = new ServerRequestHelper(TestContext, server);

            var (contents, cursor) = ParserHelper.GetFileWithSingleCursor("""
                import * as mod from 'mod.bi|cep'
                """);
            var (moduleContents, moduleCursors) = ParserHelper.GetFileWithCursors("""
                ||@export()
                type foo = string
                """);

            await helper.OpenFile("/mod.bicep", moduleContents);
            var file = await helper.OpenFile("/main.bicep", contents);

            var response = await file.GotoDefinition(cursor);

            var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(moduleContents), moduleCursors[0], moduleCursors[1]);
            response.TargetUri.Path.Should().Be("/mod.bicep");
            response.TargetRange.Should().Be(expectedRange);
        }

        [TestMethod]
        public async Task Goto_definition_works_with_wildcard_import_statements()
        {
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext);
            var helper = new ServerRequestHelper(TestContext, server);

            var (contents, cursors) = ParserHelper.GetFileWithCursors("""
                import *| |a|s| |m|o|d from 'mod.bicep'
                """);
            var (moduleContents, moduleCursors) = ParserHelper.GetFileWithCursors("""
                ||@export()
                type foo = string
                """);

            await helper.OpenFile("/mod.bicep", moduleContents);
            var file = await helper.OpenFile("/main.bicep", contents);

            foreach (var cursor in cursors)
            {
                var response = await file.GotoDefinition(cursor);

                var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(moduleContents), moduleCursors[0], moduleCursors[1]);
                response.TargetUri.Path.Should().Be("/mod.bicep");
                response.TargetRange.Should().Be(expectedRange);
            }
        }

        [TestMethod]
        public async Task Goto_definition_works_with_wildcard_import_references()
        {
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext);
            var helper = new ServerRequestHelper(TestContext, server);

            var (contents, cursors) = ParserHelper.GetFileWithCursors("""
                import * as mod from 'mod.bicep'

                param foo m|o|d.foo
                """);
            var (_, targetRangeCursors) = ParserHelper.GetFileWithCursors("""
                import |* as mod| from 'mod.bicep'

                param foo mod.foo
                """);

            var file = await helper.OpenFile("/main.bicep", contents);

            foreach (var cursor in cursors)
            {
                var response = await file.GotoDefinition(cursor);

                var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(contents), targetRangeCursors[0], targetRangeCursors[1]);
                response.TargetUri.Path.Should().Be("/main.bicep");
                response.TargetRange.Should().Be(expectedRange);
            }
        }

        [TestMethod]
        public async Task Goto_definition_works_with_wildcard_import_property_references()
        {
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext);
            var helper = new ServerRequestHelper(TestContext, server);

            var (contents, cursors) = ParserHelper.GetFileWithCursors("""
                import * as mod from 'mod.bicep'

                param foo mod.f|o|o
                """);
            var (moduleContents, moduleCursors) = ParserHelper.GetFileWithCursors("""
                @export()
                type bar = int

                @export()
                type |foo| = string
                """);

            await helper.OpenFile("/mod.bicep", moduleContents);
            var file = await helper.OpenFile("/main.bicep", contents);

            foreach (var cursor in cursors)
            {
                var response = await file.GotoDefinition(cursor);

                var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(moduleContents), moduleCursors[0], moduleCursors[1]);
                response.TargetUri.Path.Should().Be("/mod.bicep");
                response.TargetRange.Should().Be(expectedRange);
            }
        }

        [TestMethod]
        public async Task Goto_definition_works_with_cherrypick_import_statements_and_references()
        {
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext);
            var helper = new ServerRequestHelper(TestContext, server);

            var (contents, cursors) = ParserHelper.GetFileWithCursors("""
                import {f|o|o| |a|s| |f|i|z|z} from 'mod.bicep'

                param foo f|i|z|z
                """);
            var (moduleContents, moduleCursors) = ParserHelper.GetFileWithCursors("""
                @export()
                type bar = int

                @export()
                type |foo| = string
                """);

            await helper.OpenFile("/mod.bicep", moduleContents);
            var file = await helper.OpenFile("/main.bicep", contents);

            foreach (var cursor in cursors)
            {
                var response = await file.GotoDefinition(cursor);

                var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(moduleContents), moduleCursors[0], moduleCursors[1]);
                response.TargetUri.Path.Should().Be("/mod.bicep");
                response.TargetRange.Should().Be(expectedRange);
            }
        }

        [TestMethod]
        public async Task Goto_definition_works_with_wildcard_arm_import_property_references()
        {
            var (contents, cursors) = ParserHelper.GetFileWithCursors("""
                import * as mod from 'mod.json'

                param foo mod.f|o|o
                """);
            var (moduleContents, moduleCursors) = ParserHelper.GetFileWithCursors($$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "languageVersion": "2.0",
                    "definitions": {
                        "foo": {||
                            "metadata": {
                                "{{LanguageConstants.MetadataExportedPropertyName}}": true
                            },
                            "type": "array",
                            "items": {
                                "$ref": "#/definitions/bar"
                            }
                        },
                        "bar": {
                            "type": "string"
                        }
                    },
                    "resources": {}
                }
                """);

            var fileSet = InMemoryTestFileSet.Create(("mod.json", moduleContents));
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext, services => services
                .WithFileExplorer(fileSet.FileExplorer));
            var helper = new ServerRequestHelper(TestContext, server);

            var file = await helper.OpenFile("/path/to/main.bicep", contents);

            foreach (var cursor in cursors)
            {
                var response = await file.GotoDefinition(cursor);

                var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(moduleContents), moduleCursors[0], moduleCursors[1]);
                response.TargetUri.Path.Should().Be("/path/to/mod.json");
                response.TargetRange.Should().Be(expectedRange);
            }
        }

        [TestMethod]
        public async Task Goto_definition_works_with_cherrypick_arm_type_import_statements_and_references()
        {
            var (contents, cursors) = ParserHelper.GetFileWithCursors("""
                import {f|o|o| |a|s| |f|i|z|z} from 'mod.json'

                param foo f|i|z|z
                """);
            var (moduleContents, moduleCursors) = ParserHelper.GetFileWithCursors($$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "languageVersion": "2.0",
                    "definitions": {
                        "foo": {||
                            "metadata": {
                                "{{LanguageConstants.MetadataExportedPropertyName}}": true
                            },
                            "type": "array",
                            "items": {
                                "$ref": "#/definitions/bar"
                            }
                        },
                        "bar": {
                            "type": "string"
                        }
                    },
                    "resources": {}
                }
                """);

            var fileSet = InMemoryTestFileSet.Create(("mod.json", moduleContents));
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext, services => services.WithFileExplorer(fileSet.FileExplorer));
            var helper = new ServerRequestHelper(TestContext, server);

            var file = await helper.OpenFile("/path/to/main.bicep", contents);

            foreach (var cursor in cursors)
            {
                var response = await file.GotoDefinition(cursor);

                var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(moduleContents), moduleCursors[0], moduleCursors[1]);
                response.TargetUri.Path.Should().Be("/path/to/mod.json");
                response.TargetRange.Should().Be(expectedRange);
            }
        }

        [TestMethod]
        public async Task Goto_definition_works_with_cherrypick_arm_variable_import_statements_and_references()
        {
            var (contents, cursors) = ParserHelper.GetFileWithCursors("""
                import {f|o|o| |a|s| |f|i|z|z} from 'mod.json'

                var foo = f|i|z|z
                """);
            var (moduleContents, moduleCursors) = ParserHelper.GetFileWithCursors($$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "metadata": {
                        "{{LanguageConstants.TemplateMetadataExportedVariablesName}}": [
                            {
                                "name": "foo"
                            }
                        ]
                    },
                    "variables": {
                        "foo": "foo"||
                    },
                    "resources": {}
                }
                """);

            var fileSet = InMemoryTestFileSet.Create(("mod.json", moduleContents));
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext, services => services.WithFileExplorer(fileSet.FileExplorer));
            var helper = new ServerRequestHelper(TestContext, server);

            var file = await helper.OpenFile("/path/to/main.bicep", contents);

            foreach (var cursor in cursors)
            {
                var response = await file.GotoDefinition(cursor);

                var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(moduleContents), moduleCursors[0], moduleCursors[1]);
                response.TargetUri.Path.Should().Be("/path/to/mod.json");
                response.TargetRange.Should().Be(expectedRange);
            }
        }

        [TestMethod]
        public async Task Goto_definition_works_with_cherrypick_arm_copy_variable_import_statements_and_references()
        {
            var (contents, cursors) = ParserHelper.GetFileWithCursors("""
                import {f|o|o| |a|s| |f|i|z|z} from 'mod.json'

                var foo = f|i|z|z
                """);
            var (moduleContents, moduleCursors) = ParserHelper.GetFileWithCursors($$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "metadata": {
                        "{{LanguageConstants.TemplateMetadataExportedVariablesName}}": [
                            {
                                "name": "foo"
                            }
                        ]
                    },
                    "variables": {
                        "copy": [
                            {||
                                "name": "foo",
                                "count": 1,
                                "input": "foo"
                            }
                        ]
                    },
                    "resources": {}
                }
                """);

            var fileSet = InMemoryTestFileSet.Create(("mod.json", moduleContents));
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext, services => services.WithFileExplorer(fileSet.FileExplorer));
            var helper = new ServerRequestHelper(TestContext, server);

            var file = await helper.OpenFile("/path/to/main.bicep", contents);

            foreach (var cursor in cursors)
            {
                var response = await file.GotoDefinition(cursor);

                var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(moduleContents), moduleCursors[0], moduleCursors[1]);
                response.TargetUri.Path.Should().Be("/path/to/mod.json");
                response.TargetRange.Should().Be(expectedRange);
            }
        }

        [TestMethod]
        public async Task Goto_definition_works_with_wildcard_property_references_for_arm_copy_variables()
        {
            var (contents, cursors) = ParserHelper.GetFileWithCursors("""
                import * as mod from 'mod.json'

                var foo = mod.f|o|o
                """);
            var (moduleContents, moduleCursors) = ParserHelper.GetFileWithCursors($$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "metadata": {
                        "{{LanguageConstants.TemplateMetadataExportedVariablesName}}": [
                            {
                                "name": "foo"
                            }
                        ]
                    },
                    "variables": {
                        "copy": [
                            {||
                                "name": "foo",
                                "count": 1,
                                "input": "foo"
                            }
                        ]
                    },
                    "resources": {}
                }
                """);

            var fileSet = InMemoryTestFileSet.Create(("mod.json", moduleContents));
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext, services => services.WithFileExplorer(fileSet.FileExplorer));
            var helper = new ServerRequestHelper(TestContext, server);

            var file = await helper.OpenFile("/path/to/main.bicep", contents);

            foreach (var cursor in cursors)
            {
                var response = await file.GotoDefinition(cursor);

                var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(moduleContents), moduleCursors[0], moduleCursors[1]);
                response.TargetUri.Path.Should().Be("/path/to/mod.json");
                response.TargetRange.Should().Be(expectedRange);
            }
        }

        [TestMethod]
        public async Task Goto_definition_works_with_cherrypick_arm_function_import_statements_and_references()
        {
            var (contents, cursors) = ParserHelper.GetFileWithCursors("""
                import {f|o|o| |a|s| |f|i|z|z} from 'mod.json'

                var foo = f|i|z|z()
                """);
            var (moduleContents, moduleCursors) = ParserHelper.GetFileWithCursors($$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "functions": [
                        {
                            "namespace": "{{EmitConstants.UserDefinedFunctionsNamespace}}",
                            "members": {
                                "foo": {||
                                    "parameters": [],
                                    "output": {
                                        "type": "string",
                                        "value": "foo"
                                    },
                                    "metadata": {
                                        "{{LanguageConstants.MetadataExportedPropertyName}}": true
                                    }
                                }
                            }
                        }
                    ],
                    "resources": {}
                }
                """);

            var fileSet = InMemoryTestFileSet.Create(("mod.json", moduleContents));
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext, services => services.WithFileExplorer(fileSet.FileExplorer));
            var helper = new ServerRequestHelper(TestContext, server);

            var file = await helper.OpenFile("/path/to/main.bicep", contents);

            foreach (var cursor in cursors)
            {
                var response = await file.GotoDefinition(cursor);

                var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(moduleContents), moduleCursors[0], moduleCursors[1]);
                response.TargetUri.Path.Should().Be("/path/to/mod.json");
                response.TargetRange.Should().Be(expectedRange);
            }
        }

        [TestMethod]
        public async Task Goto_definition_works_with_arm_wildcard_instance_function_invocation()
        {
            var (contents, cursors) = ParserHelper.GetFileWithCursors("""
                import * as mod from 'mod.json'

                var foo = mod.f|o|o()
                """);
            var (moduleContents, moduleCursors) = ParserHelper.GetFileWithCursors($$"""
                {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "functions": [
                        {
                            "namespace": "{{EmitConstants.UserDefinedFunctionsNamespace}}",
                            "members": {
                                "foo": {||
                                    "parameters": [],
                                    "output": {
                                        "type": "string",
                                        "value": "foo"
                                    },
                                    "metadata": {
                                        "{{LanguageConstants.MetadataExportedPropertyName}}": true
                                    }
                                }
                            }
                        }
                    ],
                    "resources": {}
                }
                """);

            var fileSet = InMemoryTestFileSet.Create(("mod.json", moduleContents));
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext, services => services.WithFileExplorer(fileSet.FileExplorer));
            var helper = new ServerRequestHelper(TestContext, server);

            var file = await helper.OpenFile("/path/to/main.bicep", contents);

            foreach (var cursor in cursors)
            {
                var response = await file.GotoDefinition(cursor);

                var expectedRange = PositionHelper.GetRange(TextCoordinateConverter.GetLineStarts(moduleContents), moduleCursors[0], moduleCursors[1]);
                response.TargetUri.Path.Should().Be("/path/to/mod.json");
                response.TargetRange.Should().Be(expectedRange);
            }
        }

        private static async Task RunDefinitionScenarioTest(TestContext testContext, string fileWithCursors, char cursor, Action<List<LocationOrLocationLinks>> assertAction)
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors, cursor);
            var bicepFile = new LanguageClientFile($"{testContext.TestName}/path/to/main.bicep", file);

            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(testContext, file, bicepFile.Uri);

            var results = await RequestDefinitions(helper.Client, bicepFile, cursors);

            assertAction(results);
        }

        private static async Task RunDefinitionScenarioTestWithFiles(
            TestContext testContext,
            string fileWithCursors,
            char cursor,
            Action<List<LocationOrLocationLinks>> assertAction,
            IEnumerable<(string fileName, string fileBody)> additionalFiles)
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors, cursor);
            var bicepFile = new LanguageClientFile($"{testContext.TestName}/path/to/main.bicep", file);

            var server = new SharedLanguageHelperManager();
            var fileResolver = new InMemoryFileResolver(additionalFiles.ToDictionary(x => new Uri($"file:///{testContext.TestName}/path/to/{x.fileName}"), x => x.fileBody));
            var fileExplorer = new FileSystemFileExplorer(fileResolver.MockFileSystem);
            server.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext, services =>
                services.WithFileResolver(fileResolver).WithFileExplorer(fileExplorer)));

            var helper = await server.GetAsync();
            await helper.OpenFileOnceAsync(testContext, file, bicepFile.Uri);
            var results = await RequestDefinitions(helper.Client, bicepFile, cursors);

            assertAction(results);

            await server.DisposeAsync();
        }

        private static async Task<List<LocationOrLocationLinks>> RequestDefinitions(ILanguageClient client, LanguageClientFile bicepFile, IEnumerable<int> cursors)
        {
            var results = new List<LocationOrLocationLinks>();
            foreach (var cursor in cursors)
            {
                var result = await client.RequestDefinition(new DefinitionParams()
                {
                    TextDocument = bicepFile.Uri,
                    Position = bicepFile.GetPosition(cursor),
                });

                results.Add(result!);
            }

            return results;
        }

        private bool ValidUnboundNode(List<SyntaxBase> accumulated, int index)
        {
            // Module path
            if (index > 1 &&
                accumulated[index] is StringSyntax &&
                accumulated[index - 1] is IdentifierSyntax &&
                accumulated[index - 2] is ModuleDeclarationSyntax)
            {
                return true;
            }

            // import path
            if (index > 1 &&
                accumulated[index] is StringSyntax &&
                accumulated[index - 1] is CompileTimeImportFromClauseSyntax
            )
            {
                return true;
            }

            return false;
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
