// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.IntegrationTests.Extensions;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.CompilationManager;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class HoverTests : TestBase
    {
        private static readonly SharedLanguageHelperManager DefaultServer = new();
        private static readonly SharedLanguageHelperManager ServerWithBuiltInTypes = new();
        private static readonly SharedLanguageHelperManager ServerWithTestNamespaceProvider = new();

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
                    if ((node is ISymbolReference @ref && TestSyntaxHelper.NodeShouldBeBound(@ref)) || node is ITopLevelNamedDeclarationSyntax)
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

                    switch (symbol)
                    {
                        case FunctionSymbol when symbolReference is VariableAccessSyntax or TypeVariableAccessSyntax:
                            // variable got bound to a function
                            hover.Should().BeNull();
                            break;

                        case ErrorSymbol:
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
                node is not INamedDeclarationSyntax &&
                node is not ExtensionDeclarationSyntax &&
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
",
                '|');

            var bicepFile = new LanguageClientFile($"file:///{TestContext.TestName}-path/to/main.bicep", file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.Uri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nname: string\n```  \nThe resource name  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```  \nproperties property  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadwrite: string\n```  \nThis is a property which supports reading AND writing!  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nwriteonly: string\n```  \nThis is a property which only supports writing.  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nrequired: string\n```  \nThis is a property which is required.  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```  \nproperties property  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadonly: string\n```  \nThis is a property which only supports reading.  \n"));
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
",
                '|');

            var bicepFile = new LanguageClientFile($"file:///{TestContext.TestName}-path/to/main.bicep", file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.Uri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nname: string\n```  \nThe resource name  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```  \nproperties property  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadwrite: string\n```  \nThis is a property which supports reading AND writing!  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nwriteonly: string\n```  \nThis is a property which only supports writing.  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nrequired: string\n```  \nThis is a property which is required.  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```  \nproperties property  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadonly: string\n```  \nThis is a property which only supports reading.  \n"));
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
",
                '|');

            var bicepFile = new LanguageClientFile($"file:///{TestContext.TestName}-path/to/main.bicep", file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.Uri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nname: string\n```  \nThe resource name  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```  \nproperties property  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadwrite: string\n```  \nThis is a property which supports reading AND writing!  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nwriteonly: string\n```  \nThis is a property which only supports writing.  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nrequired: string\n```  \nThis is a property which is required.  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperties: Properties\n```  \nproperties property  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nreadonly: string\n```  \nThis is a property which only supports reading.  \n"));
        }

        [TestMethod]
        public async Task Hovers_are_displayed_on_description_decorator_objects()
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
",
                '|');

            var bicepFile = new LanguageClientFile($"file:///{TestContext.TestName}-path/to/main.bicep", file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.Uri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is my module  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is my param  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is my var  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is my\nmultiline\nresource  \n[View Documentation](https://learn.microsoft.com/azure/templates/test.rp/discriminatortests?pivots=deployment-language-bicep)  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is my output  \n  \n"));
        }

        [TestMethod]
        public async Task Hovers_include_type_modifications()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
@secure()
@minLength(1)
@maxLength(128)
param constrainedSe|cureString string
",
                '|');

            var bicepFile = new LanguageClientFile($"file:///{TestContext.TestName}-path/to/main.bicep", file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.Uri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\n@minLength(1)\n@maxLength(128)\n@secure()\nparam constrainedSecureString: string\n```  \n"));
        }

        [TestMethod]
        public async Task Hovers_are_displayed_on_description_decorator_objects_across_bicep_modules()
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
",
                '|');

            var moduleFile = new LanguageClientFile("file:///path/to/mod.bicep", modFile);
            var bicepFile = new LanguageClientFile("file:///path/to/main.bicep", file);

            var files = new Dictionary<DocumentUri, string>
            {
                [bicepFile.Uri] = file,
                [moduleFile.Uri] = modFile
            };

            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.Uri, services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));
            var client = helper.Client;

            var hovers = await RequestHovers(client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is mod1  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is param1  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is out1  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is var1  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis\nis\nout2  \n"));
        }

        [TestMethod]
        public async Task Hovers_are_displayed_on_description_metadata_in_bicep_module()
        {
            var modFile = @"
metadata description = '''this
is
a description'''

param description string
output o1 string = description
";
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
@description('''this is mod1''')
module mod|1 './mod.bicep' = {
  name: 'myMod'
}

output o1 string = mod|1.name
",
                '|');

            var moduleFile = new LanguageClientFile("file:///path/to/mod.bicep", modFile);
            var bicepFile = new LanguageClientFile("file:///path/to/main.bicep", file);

            var files = new Dictionary<DocumentUri, string>
            {
                [bicepFile.Uri] = file,
                [moduleFile.Uri] = modFile
            };

            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.Uri, services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));
            var client = helper.Client;

            var hovers = await RequestHovers(client, bicepFile, cursors);

            var expectedHover = "```bicep\nmodule mod1 './mod.bicep'\n```  \nthis is mod1  \nthis\nis\na description  \n";
            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be(expectedHover),
                h => h!.Contents.MarkupContent!.Value.Should().Be(expectedHover)
            );
        }

        [DataTestMethod]
        [DataRow("json")]
        [DataRow("jsonc")]
        public async Task Hovers_are_displayed_on_description_metadata_in_json_module(string extension)
        {
            var modFile = @"
metadata description = '''this
is
a description'''

param description string
output o1 string = description
";
            var (file, cursors) = ParserHelper.GetFileWithCursors($@"
@description('''this is mod1''')
module mod|1 './mod.{extension}' = {{
  name: 'myMod'
}}

output o1 string = mod|1.name
",
                '|');

            var (template, diags, _) = CompilationHelper.Compile(modFile);
            template!.Should().NotBeNull();
            diags.Should().BeEmpty();

            var moduleTemplateFile = new LanguageClientFile($"file:///path/to/mod.{extension}", template!.ToString());
            var bicepFile = new LanguageClientFile("file:///path/to/main.bicep", file);

            var files = new Dictionary<DocumentUri, string>
            {
                [bicepFile.Uri] = file,
                [moduleTemplateFile.Uri] = template!.ToString()
            };

            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.Uri, services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));
            var client = helper.Client;

            var hovers = await RequestHovers(client, bicepFile, cursors);

            var expectedHover = $"```bicep\nmodule mod1 './mod.{extension}'\n```  \nthis is mod1  \nthis\nis\na description  \n";
            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be(expectedHover),
                h => h!.Contents.MarkupContent!.Value.Should().Be(expectedHover)
            );
        }

        [TestMethod]
        public async Task Function_hovers_include_descriptions_if_function_overload_has_been_resolved()
        {
            var hovers = await RequestHoversAtCursorLocations(@"
var rgFunc = resource|Group()
var nsRgFunc = az.resourceGroup|()

var concatFunc = conc|at('abc', 'def')
var nsConcatFunc = sys.c|oncat('abc', 'def')
",
                '|');

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nfunction resourceGroup(): resourceGroup\n```  \nReturns the current resource group scope.  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nfunction resourceGroup(): resourceGroup\n```  \nReturns the current resource group scope.  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nfunction concat(... : bool | int | string): string\n```  \nCombines multiple string, integer, or boolean values and returns them as a concatenated string.  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nfunction concat(... : bool | int | string): string\n```  \nCombines multiple string, integer, or boolean values and returns them as a concatenated string.  \n"));
        }

        [TestMethod]
        public async Task Resource_hovers_should_include_documentation_links_for_known_resource_types()
        {
            var hovers = await RequestHoversAtCursorLocations(@"
resource fo|o 'Test.Rp/basicTests@2020-01-01' = {}

@description('This resource also has a description!')
resource b|ar 'Test.Rp/basicTests@2020-01-01' = {}

resource m|adeUp 'Test.MadeUp/nonExistentResourceType@2020-01-01' = {}
",
                '|');

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().BeEquivalentToIgnoringNewlines(@"```bicep
resource foo 'Test.Rp/basicTests@2020-01-01'
```  " + @"
[View Documentation](https://learn.microsoft.com/azure/templates/test.rp/basictests?pivots=deployment-language-bicep)  " + @"
"),
                h => h!.Contents.MarkupContent!.Value.Should().BeEquivalentToIgnoringNewlines(@"```bicep
resource bar 'Test.Rp/basicTests@2020-01-01'
```  " + @"
This resource also has a description!  " + @"
[View Documentation](https://learn.microsoft.com/azure/templates/test.rp/basictests?pivots=deployment-language-bicep)  " + @"
"),
                h => h!.Contents.MarkupContent!.Value.Should().BeEquivalentToIgnoringNewlines(@"```bicep
resource madeUp 'Test.MadeUp/nonExistentResourceType@2020-01-01'
```  " + @"
  " + @"
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
",
                '|');

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkedStrings.Should().ContainInOrder(
                    "```bicep\nfunction concat(... : array): array\n```  \nCombines multiple arrays and returns the concatenated array.  \n",
                    "```bicep\nfunction concat(... : bool | int | string): string\n```  \nCombines multiple string, integer, or boolean values and returns them as a concatenated string.  \n"),
                h => h!.Contents.MarkedStrings.Should().ContainInOrder(
                    "```bicep\nfunction concat(... : array): array\n```  \nCombines multiple arrays and returns the concatenated array.  \n",
                    "```bicep\nfunction concat(... : bool | int | string): string\n```  \nCombines multiple string, integer, or boolean values and returns them as a concatenated string.  \n"));
        }

        [TestMethod]
        public async Task Hovers_are_displayed_on_description_decorator_objects_across_arm_modules()
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
",
                '|');

            var (template, diags, _) = CompilationHelper.Compile(modFile);
            template!.Should().NotBeNull();
            diags.Should().BeEmpty();

            var moduleTemplateFile = new LanguageClientFile("file:///path/to/mod.json", template!.ToString());
            var bicepFile = new LanguageClientFile("file:///path/to/main.bicep", file);

            var files = new Dictionary<DocumentUri, string>
            {
                [bicepFile.Uri] = file,
                [moduleTemplateFile.Uri] = template!.ToString()
            };

            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.Uri, services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));
            var client = helper.Client;

            var hovers = await RequestHovers(client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is mod1  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is param1  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is param2  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is out1  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis is var1  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```  \nthis\nis\nout2  \n"));
        }

        [TestMethod]
        public async Task Func_usage_hovers_display_information()
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
@description('Checks whether the input is true in a roundabout way')
func isTrue(input bool) bool => !(input == false)

var test = is|True(false)
""");

            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var hover = await file.RequestHover(cursor);
            hover!.Contents.MarkupContent!.Value.Should().Contain("""
```bicep
function isTrue(input: bool): bool
```
""");
            hover!.Contents.MarkupContent!.Value.Should().Contain("Checks whether the input is true in a roundabout way");
        }

        [TestMethod]
        public async Task Func_declaration_hovers_display_information()
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
@description('Checks whether the input is true in a roundabout way')
func isT|rue(input bool) bool => !(input == false)
""");

            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var hover = await file.RequestHover(cursor);
            hover!.Contents.MarkupContent!.Value.Should().Contain("""
```bicep
function isTrue(input: bool): bool
```
""");
            hover!.Contents.MarkupContent!.Value.Should().Contain("Checks whether the input is true in a roundabout way");
        }

        [TestMethod]
        public async Task PropertyHovers_are_displayed_on_partial_discriminator_objects()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
resource testRes 'Test.Rp/discriminatorTests@2020-01-01' = {
  ki|nd
}
",
                '|');
            var bicepFile = new LanguageClientFile($"file:///{TestContext.TestName}-path/to/main.bicep", file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.Uri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nkind: 'BodyA' | 'BodyB'\n```  \n"));
        }

        [DataTestMethod]
        //
        // DocumentationUri only, no description
        //
        [DataRow(
            "http://test.com",
            "br:test.azurecr.io/bicep/modules/storage:1.0.1",
            "test.azurecr.io",
            "bicep/modules/storage",
            null,
            "1.0.1",
            null, // description

             // documentationUri passed in, use it
             "```bicep\nmodule test 'br:test.azurecr.io/bicep/modules/storage:1.0.1'\n```  \n[View Documentation](http://test.com)  \n")]
        [DataRow(
            "http://test.com",
            "br:mcr.microsoft.com/bicep/modules/storage:1.0.1",
            "mcr.microsoft.com",
            "bicep/modules/storage",
            null,
            "1.0.1",
            null, // description

            // documentationUri was passed in (overrides MCR location)
            "```bicep\nmodule test 'br:mcr.microsoft.com/bicep/modules/storage:1.0.1'\n```  \n[View Documentation](http://test.com)  \n")]
        [DataRow(
            null,
            "br:mcr.microsoft.com/bicep/app/dapr-containerapps-environment:1.0.1",
            "mcr.microsoft.com",
            "bicep/app/dapr-containerapps-environment",
            null,
            "1.0.1",
            null, // description

             // documentationUri calculated from MCR location
             "```bicep\nmodule test 'br:mcr.microsoft.com/bicep/app/dapr-containerapps-environment:1.0.1'\n```  \n[View Documentation](https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapps-environment/1.0.1/modules/app/dapr-containerapps-environment/README.md)  \n")]
        [DataRow(
            null,
            "br:mcr.microsoft.com/bicep/app/dapr-containerapps-environment:1.0.2",
            "mcr.microsoft.com",
            "bicep/app/dapr-containerapps-environment",
            null,
            "1.0.2",
            null, // description

             // documentationUri calculated from MCR location
             "```bicep\nmodule test 'br:mcr.microsoft.com/bicep/app/dapr-containerapps-environment:1.0.2'\n```  \n[View Documentation](https://github.com/Azure/bicep-registry-modules/tree/app/dapr-containerapps-environment/1.0.2/modules/app/dapr-containerapps-environment/README.md)  \n")]
        //
        // Description only, no documentationUri
        //
        [DataRow(
            null,
            "br:test.azurecr.io/bicep/modules/storage@sha256:0000000000000000000000000000000000000000000000000000000000000000",
            "test.azurecr.io",
            "bicep/modules/storage",
            "sha256:0000000000000000000000000000000000000000000000000000000000000000",
            null,
            "my description", // description

             "```bicep\nmodule test 'br:test.azurecr.io/bicep/modules/storage@sha256:0000000000000000000000000000000000000000000000000000000000000000'\n```  \nmy description  \n")]
        [DataRow(
            null,
            "br:test.azurecr.io/bicep/modules/storage@sha256:0000000000000000000000000000000000000000000000000000000000000000",
            "test.azurecr.io",
            "bicep/modules/storage",
            "sha256:0000000000000000000000000000000000000000000000000000000000000000",
            null,
            "my \\\"description\\\"", // description

             "```bicep\nmodule test 'br:test.azurecr.io/bicep/modules/storage@sha256:0000000000000000000000000000000000000000000000000000000000000000'\n```  \nmy \"description\"  \n")]
        [DataRow(
            null,
            "br:test.azurecr.io/bicep/modules/storage@sha256:0000000000000000000000000000000000000000000000000000000000000000",
            "test.azurecr.io",
            "bicep/modules/storage",
            "sha256:0000000000000000000000000000000000000000000000000000000000000000",
            null,
            "my [description", // description

             "```bicep\nmodule test 'br:test.azurecr.io/bicep/modules/storage@sha256:0000000000000000000000000000000000000000000000000000000000000000'\n```  \nmy [description  \n")]
        //
        // Neither documentationUri nor description
        [DataRow(
            null,
            "br:test.azurecr.io/bicep/modules/storage@sha256:0000000000000000000000000000000000000000000000000000000000000000",
            "test.azurecr.io",
            "bicep/modules/storage",
            "sha256:0000000000000000000000000000000000000000000000000000000000000000",
            null,
            null, // description
            "```bicep\nmodule test 'br:test.azurecr.io/bicep/modules/storage@sha256:0000000000000000000000000000000000000000000000000000000000000000'\n```  \n  \n")]
        //
        // Both documentationUri and description
        //
        [DataRow(
            "http://test.com",
            "br:test.azurecr.io/bicep/modules/storage@sha256:0000000000000000000000000000000000000000000000000000000000000000",
            "test.azurecr.io",
            "bicep/modules/storage",
            "sha256:0000000000000000000000000000000000000000000000000000000000000000",
            null,
            "my description", // description
             "```bicep\nmodule test 'br:test.azurecr.io/bicep/modules/storage@sha256:0000000000000000000000000000000000000000000000000000000000000000'\n```  \nmy description  \n[View Documentation](http://test.com)  \n")]
        public async Task Verify_Hover_ContainerRegistry(string? documentationUri, string repositoryAndTag, string registry, string repository, string? digest, string? tag, string? description, string expectedHoverContent)
        {
            var fileWithCursors = $$"""
                module |test '{{repositoryAndTag}}' = {
                  name: 'abc'
                }
                """;
            var (bicepFileContents, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors, '|');
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile("input.bicep", bicepFileContents);

            SaveManifestFileToModuleRegistryCache(
                mockFileSystem,
                registry,
                repository,
                GetManifestFileContents(documentationUri, description),
                digest,
                tag);

            var helper = await GetLanguageClientAsync(mockFileSystem);

            var bicepFile = new LanguageClientFile(mockFileSystem.Path.GetFullPath("input.bicep"), bicepFileContents);

            await helper.OpenFileOnceAsync(TestContext, bicepFile.Text, bicepFile.Uri);
            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Single()!.Contents.MarkupContent!.Value.Should().Be(expectedHoverContent);
        }

        private static void SaveManifestFileToModuleRegistryCache(
            MockFileSystem mockFileSystem,
            string registry,
            string repository,
            string manifestFileContents,
            string? digest,
            string? tag)
        {
            var fileExplorer = new FileSystemFileExplorer(mockFileSystem);
            var featureProvider = new FeatureProvider(BicepTestConstants.BuiltInConfiguration, fileExplorer);

            var cachePath = digest is not null
                ? $"br/{registry}/{repository.Replace("/", "$")}/{digest.Replace(":", "#")}"
                : $"br/{registry}/{repository.Replace("/", "$")}/{tag}$";

            featureProvider.CacheRootDirectory.GetDirectory(cachePath)?.GetFile("manifest").Write(manifestFileContents);
        }

        [TestMethod]
        public async Task ParamHovers_are_displayed_on_param_symbols_params_file()
        {
            var bicepText = @"
@allowed(
  [ 'value1'
    'value2'
  ]
)
@description('this is a string value')
param foo string

@allowed(
    [
        0
        1
    ]
)
@description('this is an int value')
param bar int

@description('this is a bool value')
param foobar bool
";

            var bicepparamTextWithCursor = @"
using 'main.bicep'

param f|oo = 'value1'

param ba|r = 1

param foo|bar = true
";

            var (bicepparamText, cursors) = ParserHelper.GetFileWithCursors(bicepparamTextWithCursor, '|');

            var paramsFile = new LanguageClientFile("file:///path/to/params.bicepparam", bicepparamText);
            var bicepFile = new LanguageClientFile("file:///path/to/main.bicep", bicepText);

            var files = new Dictionary<DocumentUri, string>
            {
                [paramsFile.Uri] = bicepparamText,
                [bicepFile.Uri] = bicepText
            };

            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, paramsFile.Uri);
            var client = helper.Client;

            var hovers = await RequestHovers(client, paramsFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```bicep\nparam foo: 'value1' | 'value2'\n```  \nthis is a string value  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```bicep\nparam bar: 0 | 1\n```  \nthis is an int value  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```bicep\nparam foobar: bool\n```  \nthis is a bool value  \n")
            );
        }

        [TestMethod]
        public async Task Hovers_are_displayed_on_imported_types()
        {
            var moduleText = """
                @export()
                @description('The foo type')
                type foo = string
                """;

            var mainTextWithCursor = """
                import {foo} from 'mod.bicep'
                import * as mod from 'mod.bicep'

                type fooAlias = f|oo
                type fooAliasOffWildcard = m|od.f|oo
                """;

            var (mainText, cursors) = ParserHelper.GetFileWithCursors(mainTextWithCursor, '|');

            var mainFile = new LanguageClientFile("file:///path/to/main.bicep", mainText);
            var moduleFile = new LanguageClientFile("file:///path/to/mod.bicep", moduleText);

            var files = new Dictionary<DocumentUri, string>
            {
                [mainFile.Uri] = mainText,
                [moduleFile.Uri] = moduleText
            };

            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, mainFile.Uri);
            var client = helper.Client;

            var hovers = await RequestHovers(client, mainFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```bicep\ntype foo: Type<string>\n```  \nThe foo type  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```bicep\nmod namespace\n```  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().EndWith("```bicep\nfoo: Type<string>\n```  \nThe foo type  \n"));
        }

        [TestMethod]
        public async Task Description_markdown_is_correctly_formatted()
        {
            // https://github.com/Azure/bicep/issues/12412
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
                type foo = {
                  @description('''Source port ranges.
                    Can be a single valid port number, a range in the form of \<start\>-\<end\>, or a * for any ports.
                    When a wildcard is used, that needs to be the only value.''')
                  sourcePortRanges: string[]
                }

                param foo1 foo = {
                  sourceP|ortRanges:
                }
                """);

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var hover = await file.RequestHover(cursor);
            hover!.Contents!.MarkupContent!.Value
                .Should().BeEquivalentToIgnoringNewlines(
                    @"```bicep
sourcePortRanges: string[]
```  " + @"
Source port ranges.
Can be a single valid port number, a range in the form of \<start\>-\<end\>, or a * for any ports.
When a wildcard is used, that needs to be the only value.  " + @"
");
        }

        [TestMethod]
        public async Task Description_markdown_is_shown_when_hovering_over_type_property_declaration()
        {
            // https://github.com/Azure/bicep/issues/13398
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
                type foo = {
                  @description('''Source port ranges.
                    Can be a single valid port number, a range in the form of \<start\>-\<end\>, or a * for any ports.
                    When a wildcard is used, that needs to be the only value.''')
                  sourcePortR|anges: string[]
                }
                """);

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var hover = await file.RequestHover(cursor);
            hover!.Contents!.MarkupContent!.Value
                .Should().BeEquivalentToIgnoringNewlines(
                    @"```bicep
sourcePortRanges: string[]
```  " + @"
Source port ranges.
Can be a single valid port number, a range in the form of \<start\>-\<end\>, or a * for any ports.
When a wildcard is used, that needs to be the only value.  " + @"
");
        }

        [TestMethod]
        public async Task Description_markdown_indenting_is_normalized()
        {
            // https://github.com/Azure/bicep/issues/13982
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
type obj = {

  @description('This is the name')
  name: string

  @description('''lorem ipsum

  lorem ipsum
  ''')
  complex: string

  @description('This is the propertis')
  properties: {
    @description('A simple property')
    property1: string

    @description('''This is a more complex property

    This property requries more then one line of text to explain
    There might also be a link to something [link](www.google.com)
    ''')
    proper|ty2: string
  }
}
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var hover = await file.RequestHover(cursor);
            hover!.Contents!.MarkupContent!.Value
                .Should().EqualIgnoringTrailingWhitespace("""
```bicep
property2: string
```
This is a more complex property

This property requries more then one line of text to explain
There might also be a link to something [link](www.google.com)


""");
        }

        [TestMethod]
        public async Task Hovers_are_displayed_on_type_property_access()
        {
            var (text, cursors) = ParserHelper.GetFileWithCursors("""
                type t = {
                    @description('A named property')
                    property: string
                    *: int
                }
                param foo t
                param bar t.pro|perty
                param baz t.|*

                param fizz t = {
                    pro|perty: 'property'
                    another|Property: 10
                }

                output a string = foo.pro|perty
                output b ing = foo.another|Property
                """);

            var bicepFile = new LanguageClientFile($"file:///{TestContext.TestName}-path/to/main.bicep", text);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, text, bicepFile.Uri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperty: string\n```  \nA named property  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\n*: int\n```  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperty: string\n```  \nA named property  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\n*: int\n```  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\nproperty: string\n```  \nA named property  \n"),
                h => h!.Contents.MarkupContent!.Value.Should().Be("```bicep\n*: int\n```  \n"));
        }

        [TestMethod]
        public async Task Compiled_json_contains_trimmed_descriptions_in_hovers()
        {
            // https://github.com/Azure/bicep/issues/14864
            var jsonContents = CompilationHelper.Compile("""
type fooType = {
  @description('''Description

    ```sql
    AzureMetrics
    | where ResourceProvider == 'MICROSOFT.ANALYSISSERVICES'
    ```
    ''')
  foo2: string?
}

param foo fooType = {}
""");

            var (bicepparamText, cursor) = ParserHelper.GetFileWithSingleCursor("""
using 'main.json'
param foo = {
  fo|o2:
}
""");

            var jsonUri = "file:///path/to/main.json";
            var paramsUri = "file:///path/to/params.bicepparam";

            var files = new Dictionary<DocumentUri, string>
            {
                [jsonUri] = jsonContents.Template.ToJson(),
                [paramsUri] = bicepparamText,
            };

            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, paramsUri, services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));
            var client = helper.Client;

            var hover = await RequestHover(client, new LanguageClientFile(paramsUri, bicepparamText), cursor);
            hover!.Contents!.MarkupContent!.Value
                .Should().BeEquivalentToIgnoringTrailingWhitespace("""
```bicep
foo2: null | string
```
Description

```sql
AzureMetrics
| where ResourceProvider == 'MICROSOFT.ANALYSISSERVICES'
```


""");
        }

        [TestMethod]
        public async Task Hovers_are_displayed_when_hovering_over_quoted_object_property_names()
        {
            // https://github.com/Azure/bicep/issues/18411
            var (text, cursors) = ParserHelper.GetFileWithCursors("""
                type foo = {
                  @description('A string property')
                  stringProp: string
                  @description('An int property')
                  intProp: int
                }
                param myParam foo = {
                  'str|ingProp': 'hello'
                  'intP|rop': 42
                }
                """, '|');

            var bicepFile = new LanguageClientFile($"file:///{TestContext.TestName}-path/to/main.bicep", text);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, text, bicepFile.Uri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().AllSatisfy(h => h.Should().NotBeNull());
            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Contain("stringProp: string").And.Contain("A string property"),
                h => h!.Contents.MarkupContent!.Value.Should().Contain("intProp: int").And.Contain("An int property")
            );
        }

        [TestMethod]
        public async Task Hovers_are_displayed_when_hovering_over_quoted_resource_property_names()
        {
            // https://github.com/Azure/bicep/issues/18411
            var (text, cursors) = ParserHelper.GetFileWithCursors("""
                resource storage 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                  'na|me': 'mystorageaccount'
                  'loca|tion': 'eastus'
                  'ki|nd': 'StorageV2'
                  sku: {
                    'na|me': 'Standard_LRS'
                  }
                }
                """, '|');

            var bicepFile = new LanguageClientFile($"file:///{TestContext.TestName}-path/to/main.bicep", text);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, text, bicepFile.Uri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().AllSatisfy(h => h.Should().NotBeNull());
        }

        [TestMethod]
        public async Task Hovers_are_displayed_when_hovering_over_quoted_type_property_names()
        {
            // https://github.com/Azure/bicep/issues/18411
            var (text, cursors) = ParserHelper.GetFileWithCursors("""
                type foo = {
                  @description('A string property')
                  'strin|gProp': string
                  @description('An int property')
                  'intP|rop': int
                }
                """, '|');

            var bicepFile = new LanguageClientFile($"file:///{TestContext.TestName}-path/to/main.bicep", text);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, text, bicepFile.Uri);

            var hovers = await RequestHovers(helper.Client, bicepFile, cursors);

            hovers.Should().AllSatisfy(h => h.Should().NotBeNull());
            hovers.Should().SatisfyRespectively(
                h => h!.Contents.MarkupContent!.Value.Should().Contain("stringProp: string").And.Contain("A string property"),
                h => h!.Contents.MarkupContent!.Value.Should().Contain("intProp: int").And.Contain("An int property")
            );
        }

        private string GetManifestFileContents(string? documentationUri, string? description)
        {
            string annotations =
                string.Join(
                    ",",
                    new string?[] {
                        documentationUri is null ? null : $"\"org.opencontainers.image.documentation\": \"{documentationUri}\"",
                        description is null ? description  : $"\"org.opencontainers.image.description\": \"{description}\"",
                    }.WhereNotNull());

            return @"{
  ""schemaVersion"": 2,
  ""artifactType"": ""application/vnd.ms.bicep.module.artifact"",
  ""config"": {
    ""mediaType"": ""application/vnd.ms.bicep.module.config.v1+json"",
    ""digest"": ""sha256:e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"",
    ""size"": 0,
    ""annotations"": {}
  },
  ""layers"": [
    {
      ""mediaType"": ""application/vnd.ms.bicep.module.layer.v1+json"",
      ""digest"": ""sha256:9846dcfde47a4b2943be478754d1169ece3adc6447c9596d9ba48e2579c24173"",
      ""size"": 735131,
      ""annotations"": {}
    }
  ],
  ""annotations"": {" + annotations + @"}
  }";
        }

        private async Task<MultiFileLanguageServerHelper> GetLanguageClientAsync(MockFileSystem mockFileSystem)
        {
            SharedLanguageHelperManager sharedLanguageHelperManager = new();
            sharedLanguageHelperManager.Initialize(
                () => MultiFileLanguageServerHelper.StartLanguageServer(
                    TestContext,
                    services => services.WithFileSystem(mockFileSystem)));

            var multiFileLanguageServerHelper = await sharedLanguageHelperManager.GetAsync();
            return multiFileLanguageServerHelper;
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

                    case TypeAliasSymbol declaredType:
                        tooltip.Should().Contain($"type {declaredType.Name}: {declaredType.Type}");
                        break;

                    case ImportedTypeSymbol importedType:
                        tooltip.Should().Contain($"type {importedType.Name}: {importedType.Type}");
                        break;

                    case AmbientTypeSymbol ambientType:
                        tooltip.Should().Contain($"type {ambientType.Name}: {ambientType.Type}");
                        break;

                    case VariableSymbol variable:
                        // the hovers with errors don't appear in VS code and only occur in tests
                        tooltip.Should().ContainAny([$"var {variable.Name}: {variable.Type}", $"var {variable.Name}: error"]);
                        break;

                    case ImportedVariableSymbol importedVariable:
                        tooltip.Should().Contain($"var {importedVariable.Name}: {importedVariable.Type}");
                        break;

                    case TestSymbol variable:
                        // the hovers with errors don't appear in VS code and only occur in tests
                        tooltip.Should().ContainAny([$"test {variable.Name}", $"var {variable.Name}"]);
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

                    case DeclaredFunctionSymbol declaredFunction:
                        tooltip.Should().Contain($"function {declaredFunction.Name}(");
                        break;

                    case ImportedFunctionSymbol importedFunction:
                        tooltip.Should().Contain($"function {importedFunction.Name}(");
                        break;

                    case LocalVariableSymbol local:
                        tooltip.Should().Contain($"{local.Name}: {local.Type}");
                        break;

                    case ExtensionNamespaceSymbol extension:
                        tooltip.Should().Contain($"extension {extension.Name}");
                        break;

                    case BuiltInNamespaceSymbol @namespace:
                        tooltip.Should().Contain($"{@namespace.Name} namespace");
                        break;

                    case WildcardImportSymbol wildcardImport:
                        tooltip.Should().Contain($"{wildcardImport.Name} namespace");
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

        private static async Task<Hover?> RequestHover(ILanguageClient client, LanguageClientFile bicepFile, int cursor)
        {
            var hovers = await RequestHovers(client, bicepFile, [cursor]);

            return hovers.Single();
        }

        private static async Task<IEnumerable<Hover?>> RequestHovers(ILanguageClient client, LanguageClientFile bicepFile, IEnumerable<int> cursors)
        {
            var hovers = new List<Hover?>();
            foreach (var cursor in cursors)
            {
                var hover = await client.RequestHover(new HoverParams
                {
                    TextDocument = bicepFile.Uri,
                    Position = bicepFile.GetPosition(cursor),
                });

                hovers.Add(hover);
            }

            return hovers;
        }

        public async Task<IEnumerable<Hover?>> RequestHoversAtCursorLocations(string fileWithCursors, char cursor)
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors, cursor);

            var bicepFile = new LanguageClientFile($"file:///{TestContext.TestName}-path/to/main.bicep", file);

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.Uri);

            return await RequestHovers(helper.Client, bicepFile, cursors);
        }
    }
}
