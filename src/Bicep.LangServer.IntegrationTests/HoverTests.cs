// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.IntegrationTests.Extensions;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SymbolKind = Bicep.Core.Semantics.SymbolKind;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class HoverTests
    {
        private static readonly SharedLanguageHelperManager DefaultServer = new();
        private static readonly SharedLanguageHelperManager ServerWithBuiltInTypes = new();
        private static readonly SharedLanguageHelperManager ServerWithTestNamespaceProvider = new();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DefaultServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext));
            ServerWithBuiltInTypes.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext, services => services.WithNamespaceProvider(BuiltInTestTypes.Create())));
            ServerWithTestNamespaceProvider.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext));
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await DefaultServer.DisposeAsync();
            await ServerWithBuiltInTypes.DisposeAsync();
            await ServerWithTestNamespaceProvider.DisposeAsync();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task HoveringOverSymbolReferencesAndDeclarationsShouldProduceHovers(DataSet dataSet)
        {
            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var uri = DocumentUri.From(fileUri);

            var helper = await ServerWithTestNamespaceProvider.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, dataSet.Bicep, uri);

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

                var hover = await helper.Client.RequestHover(new HoverParams
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

            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, dataSet.Bicep, uri);

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
                var hover = await helper.Client.RequestHover(new HoverParams
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


        [TestMethod]
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

            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri($"file:///{TestContext.TestName}-path/to/main.bicep"), file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.FileUri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nname: string\n```\nThe resource name\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```\nproperties property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadwrite: string\n```\nThis is a property which supports reading AND writing!\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nwriteonly: string\n```\nThis is a property which only supports writing.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nrequired: string\n```\nThis is a property which is required.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```\nproperties property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadonly: string\n```\nThis is a property which only supports reading.\n"));
        }


        [TestMethod]
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

            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri($"file:///{TestContext.TestName}-path/to/main.bicep"), file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.FileUri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nname: string\n```\nThe resource name\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```\nproperties property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadwrite: string\n```\nThis is a property which supports reading AND writing!\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nwriteonly: string\n```\nThis is a property which only supports writing.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nrequired: string\n```\nThis is a property which is required.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```\nproperties property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadonly: string\n```\nThis is a property which only supports reading.\n"));
        }


        [TestMethod]
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

            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri($"file:///{TestContext.TestName}-path/to/main.bicep"), file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.FileUri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nname: string\n```\nThe resource name\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```\nproperties property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadwrite: string\n```\nThis is a property which supports reading AND writing!\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nwriteonly: string\n```\nThis is a property which only supports writing.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nrequired: string\n```\nThis is a property which is required.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```\nproperties property\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadonly: string\n```\nThis is a property which only supports reading.\n"));
        }

        [TestMethod]
        public async Task Hovers_are_displayed_on_discription_decorator_objects()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
@description('''this is my module''')
module test|mod './dummy.bicep' = {
}

@description('this is my param')
var test|Param string

@sys.description('this is my var')
var test|Var string = 'hello'

@description('''this is my
multiline
resource''')
resource test|Res 'Test.Rp/discriminatorTests@2020-01-01' = {
}

@description('''this is my output''')
resource test|Output string = 'str'
");

            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri($"file:///{TestContext.TestName}-path/to/main.bicep"), file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.FileUri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is my module\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is my param\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is my var\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is my  \nmultiline  \nresource  \n[View Type Documentation](https://docs.microsoft.com/azure/templates/test.rp/discriminatortests?tabs=bicep)\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is my output  \n\n"));
        }

        [TestMethod]
        public async Task Hovers_are_displayed_on_discription_decorator_objects_across_bicep_modules()
        {
            var modFile = @"
@description('this is param1')
param param1 string

@sys.description('this is out1')
output out1 string = '${param1}-out1'

@description('''this
is
out2''')
output out2 string = '${param1}-out2'
";
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
@description('''this is mod1''')
module mod|1 './mod.bicep' = {
  name: 'myMod'
  params: {
    para|m1: 's'
  }
}

@description('''this is var1''')
var var1 = mod1.outputs.ou|t1

output moduleOutput string = '${var|1}-${mod1.outputs.o|ut2}'
");

            var moduleFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/mod.bicep"), modFile);
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);

            var files = new Dictionary<Uri, string>
            {
                [bicepFile.FileUri] = file,
                [moduleFile.FileUri] = modFile
            };

            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.FileUri, services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));
            var client = helper.Client;

            var hovers = await RequestHovers(client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is mod1\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is param1\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is out1\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is var1\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis  \nis  \nout2\n"));
        }

        [TestMethod]
        public async Task Function_hovers_include_descriptions_if_function_overload_has_been_resolved()
        {
            var hovers = await RequestHoversAtCursorLocations(@"
var rgFunc = resource|Group()
var nsRgFunc = az.resourceGroup|()

var concatFunc = conc|at('abc', 'def')
var nsConcatFunc = sys.c|oncat('abc', 'def')
");

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nfunction resourceGroup(): resourceGroup\n```\nReturns the current resource group scope.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nfunction resourceGroup(): resourceGroup\n```\nReturns the current resource group scope.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nfunction concat(... : bool | int | string): string\n```\nCombines multiple string, integer, or boolean values and returns them as a concatenated string.\n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nfunction concat(... : bool | int | string): string\n```\nCombines multiple string, integer, or boolean values and returns them as a concatenated string.\n"));
        }

        [TestMethod]
        public async Task Resource_hovers_should_include_documentation_links_for_known_resource_types()
        {
            var hovers = await RequestHoversAtCursorLocations(@"
resource fo|o 'Test.Rp/basicTests@2020-01-01' = {}

@description('This resource also has a description!')
resource b|ar 'Test.Rp/basicTests@2020-01-01' = {}

resource m|adeUp 'Test.MadeUp/nonExistentResourceType@2020-01-01' = {}
");

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().BeEquivalentToIgnoringNewlines(@"```bicep
resource foo 'Test.Rp/basicTests@2020-01-01'
```
[View Type Documentation](https://docs.microsoft.com/azure/templates/test.rp/basictests?tabs=bicep)
"),
                h => h!.Contents.MarkupContent!.Value.Should().BeEquivalentToIgnoringNewlines(@"```bicep
resource bar 'Test.Rp/basicTests@2020-01-01'
```
This resource also has a description!  " + @"
[View Type Documentation](https://docs.microsoft.com/azure/templates/test.rp/basictests?tabs=bicep)
"),
                h => h!.Contents.MarkupContent!.Value.Should().BeEquivalentToIgnoringNewlines(@"```bicep
resource madeUp 'Test.MadeUp/nonExistentResourceType@2020-01-01'
```

"));
        }

        [TestMethod]
        public async Task Function_hovers_display_without_descriptions_if_function_overload_has_not_been_resolved()
        {
            // using the any type, we don't know which particular overload has been selected, so we cannot show an accurate description.
            // this should be taken care of with https://github.com/Azure/bicep/issues/4588
            var hovers = await RequestHoversAtCursorLocations(@"
var concatFunc = conc|at(any('hello'))
var nsConcatFunc = sys.conc|at(any('hello'))
");

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkedStrings.Should().ContainInOrder(
                    "```bicep\nfunction concat(... : array): array\n```\nCombines multiple arrays and returns the concatenated array.\n",
                    "```bicep\nfunction concat(... : bool | int | string): string\n```\nCombines multiple string, integer, or boolean values and returns them as a concatenated string.\n"),
                h => h!.Contents.MarkedStrings.Should().ContainInOrder(
                    "```bicep\nfunction concat(... : array): array\n```\nCombines multiple arrays and returns the concatenated array.\n",
                    "```bicep\nfunction concat(... : bool | int | string): string\n```\nCombines multiple string, integer, or boolean values and returns them as a concatenated string.\n"));
        }

        [TestMethod]
        public async Task Hovers_are_displayed_on_discription_decorator_objects_across_arm_modules()
        {
            var modFile = @"
@description('this is param1')
param param1 string

@metadata({
    description: 'this is param2'
})
param param2 string = '${param1}-2'

@sys.description('this is out1')
output out1 string = '${param1}-out1'

@description('''this
is
out2''')
output out2 string = '${param2}-out2'
";
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
@description('''this is mod1''')
module mo|d1 './mod.json' = {
  name: 'myMod'
  params: {
    para|m1: 's'
    p|aram2: 's'
  }
}

@description('''this is var1''')
var var1 = mod1.outputs.out|1

output moduleOutput string = '${va|r1}-${mod1.outputs.ou|t2}'
");

            var (template, diags, _) = CompilationHelper.Compile(modFile);
            template!.Should().NotBeNull();
            diags.Should().BeEmpty();

            var moduleTemplateFile = SourceFileFactory.CreateArmTemplateFile(new Uri("file:///path/to/mod.json"), template!.ToString());
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);

            var files = new Dictionary<Uri, string>
            {
                [bicepFile.FileUri] = file,
                [moduleTemplateFile.FileUri] = template!.ToString()
            };

            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.FileUri, services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));
            var client = helper.Client;

            var hovers = await RequestHovers(client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is mod1\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is param1\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is param2\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is out1\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis is var1\n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```\nthis  \nis  \nout2\n"));
        }

        [TestMethod]
        public async Task PropertyHovers_are_displayed_on_partial_discriminator_objects()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
resource testRes 'Test.Rp/discriminatorTests@2020-01-01' = {
  ki|nd
}
");
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri($"file:///{TestContext.TestName}-path/to/main.bicep"), file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.FileUri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nkind: 'BodyA' | 'BodyB'\n```\n"));
        }

        private static void ValidateHover(Hover? hover, Symbol symbol)
        {
            hover.Should().NotBeNull();
            hover!.Range!.Should().NotBeNull();
            hover.Contents.Should().NotBeNull();

            List<string> tooltips = new();
            if (hover.Contents.HasMarkedStrings)
            {
                tooltips.AddRange(hover.Contents.MarkedStrings!.Select(ms => ms.Value));
            }
            else
            {
                hover.Contents.MarkupContent!.Kind.Should().Be(MarkupKind.Markdown);
                tooltips.Add(hover.Contents.MarkupContent!.Value);
            }

            foreach (var tooltip in tooltips)
            {
                tooltip.Should().StartWith("```bicep\n");
                Regex.Matches(tooltip, "```").Count.Should().Be(2);

                switch (symbol)
                {

                    case MetadataSymbol metadata:
                        tooltip.Should().Contain($"metadata {metadata.Name}: {metadata.Type}");
                        break;

                    case ParameterSymbol parameter:
                        tooltip.Should().Contain($"param {parameter.Name}: {parameter.Type}");
                        break;

                    case VariableSymbol variable:
                        // the hovers with errors don't appear in VS code and only occur in tests
                        tooltip.Should().ContainAny(new[] { $"var {variable.Name}: {variable.Type}", $"var {variable.Name}: error" });
                        break;

                    case ResourceSymbol resource:
                        tooltip.Should().Contain($"resource {resource.Name}");
                        tooltip.Should().Contain(resource.Type.Name);
                        break;

                    case ModuleSymbol module:
                        tooltip.Should().Contain($"module {module.Name}");
                        break;

                    case OutputSymbol output:
                        tooltip.Should().Contain($"output {output.Name}: {output.Type}");
                        break;

                    case FunctionSymbol function:
                        if (function.Overloads.All(fo => fo is FunctionWildcardOverload))
                        {
                            tooltip.Should().Contain($"function ");
                            tooltip.Should().Contain($"*(");
                        }
                        else
                        {
                            tooltip.Should().Contain($"function {function.Name}(");
                        }
                        break;

                    case LocalVariableSymbol local:
                        tooltip.Should().Contain($"{local.Name}: {local.Type}");
                        break;

                    case ImportedNamespaceSymbol import:
                        tooltip.Should().Contain($"{import.Name} namespace");
                        break;

                    case BuiltInNamespaceSymbol @namespace:
                        tooltip.Should().Contain($"{@namespace.Name} namespace");
                        break;

                    default:
                        throw new AssertFailedException($"Unexpected symbol type '{symbol.GetType().Name}'");
                }
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

        public async Task<IEnumerable<Hover?>> RequestHoversAtCursorLocations(string fileWithCursors)
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);

            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri($"file:///{TestContext.TestName}-path/to/main.bicep"), file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.FileUri);

            return await RequestHovers(helper.Client, bicepFile, cursors);
        }
    }
}
