// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.CodeAction;
using Bicep.Core.CodeAction.Fixes;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class CodeActionTests
    {
        private static ServiceBuilder Services => new ServiceBuilder();

        private const string SecureTitle = "Add @secure";
        private const string DescriptionTitle = "Add @description";
        private const string AllowedTitle = "Add @allowed";
        private const string MinLengthTitle = "Add @minLength";
        private const string MaxLengthTitle = "Add @maxLength";
        private const string MinValueTitle = "Add @minValue";
        private const string MaxValueTitle = "Add @maxValue";
        private const string RemoveUnusedExistingResourceTitle = "Remove unused existing resource";
        private const string RemoveUnusedParameterTitle = "Remove unused parameter";
        private const string RemoveUnusedVariableTitle = "Remove unused variable";

        private static readonly SharedLanguageHelperManager DefaultServer = new();

        private static readonly SharedLanguageHelperManager ServerWithFileResolver = new();

        private static readonly SharedLanguageHelperManager ServerWithBuiltInTypes = new();

        private static readonly SharedLanguageHelperManager ServerWithNamespaceProvider = new();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DefaultServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext));

            ServerWithFileResolver.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext, new Server.CreationOptions(FileResolver: BicepTestConstants.FileResolver)));

            ServerWithBuiltInTypes.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext, new Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create())));

            ServerWithNamespaceProvider.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext, new Server.CreationOptions(NamespaceProvider: BicepTestConstants.NamespaceProvider)));
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await DefaultServer.DisposeAsync();
            await ServerWithFileResolver.DisposeAsync();
            await ServerWithBuiltInTypes.DisposeAsync();
            await ServerWithNamespaceProvider.DisposeAsync();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task RequestingCodeActionWithFixableDiagnosticsShouldProduceQuickFixes(DataSet dataSet)
        {
            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(this.TestContext);
            var uri = DocumentUri.From(fileUri);

            // start language server
            var helper = await ServerWithFileResolver.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, dataSet.Bicep, uri);

            var client = helper.Client;

            // construct a parallel compilation
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;
            var fixables = compilation.GetEntrypointSemanticModel().GetAllDiagnostics().OfType<IFixable>().ToList();

            foreach (var fixable in fixables)
            {
                foreach (var span in GetOverlappingSpans(fixable.Span))
                {
                    using (new AssertionScope().WithVisualCursor(compilation.SourceFileGrouping.EntryPoint, fixable.Span))
                    {
                        var range = span.ToRange(lineStarts);
                        var quickFixes = await client.RequestCodeAction(new CodeActionParams
                        {
                            TextDocument = new TextDocumentIdentifier(uri),
                            Range = range,
                        });

                        // Assert.
                        quickFixes.Should().NotBeNull();

                        var spansOverlapOrAbut = (IFixable f) =>
                        {
                            if (span.Position <= f.Span.Position)
                            {
                                return span.GetEndPosition() >= f.Span.Position;
                            }

                            return f.Span.GetEndPosition() >= span.Position;
                        };

                        var bicepFixes = fixables.Where(spansOverlapOrAbut).SelectMany(f => f.Fixes).ToHashSet();
                        var quickFixList = quickFixes.Where(x => x.CodeAction?.Kind == CodeActionKind.QuickFix).ToList();

                        var bicepFixDescriptions = bicepFixes.Select(f => f.Description);
                        var quickFixTitles = quickFixList.Select(f => f.CodeAction.Title);
                        bicepFixDescriptions.Should().BeEquivalentTo(quickFixTitles);

                        for (int i = 0; i < quickFixList.Count; i++)
                        {
                            var quickFix = quickFixList[i];

                            quickFix.IsCodeAction.Should().BeTrue();
                            quickFix.CodeAction!.Kind.Should().Be(CodeActionKind.QuickFix);
                            quickFix.CodeAction.Edit!.Changes.Should().ContainKey(uri);

                            var textEditList = quickFix.CodeAction.Edit.Changes![uri].ToList();

                            bicepFixes.RemoveWhere(fix =>
                            {
                                if (fix.Description != quickFix.CodeAction.Title)
                                {
                                    return false;
                                }

                                var replacementSet = fix.Replacements.ToHashSet();
                                foreach (var edit in textEditList)
                                {
                                    replacementSet.RemoveWhere(replacement => edit.Range == replacement.ToRange(lineStarts) && edit.NewText == replacement.Text);
                                }

                                return replacementSet.Count == 0;
                            }).Should().Be(1, "No matching fix found.");
                        }

                        bicepFixes.Count.Should().Be(0);
                    }
                }
            }
        }

        [TestMethod]
        public async Task VerifyCodeActionIsAvailableToSuppressLinterWarning()
        {
            var bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""warning""
        }
      }
    }
  }
}";
            await VerifyCodeActionIsAvailableToSuppressLinterDiagnostics(bicepConfigFileContents);
        }

        [TestMethod]
        public async Task VerifyCodeActionIsAvailableToSuppressLinterError()
        {
            var bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""error""
        }
      }
    }
  }
}";
            await VerifyCodeActionIsAvailableToSuppressLinterDiagnostics(bicepConfigFileContents);
        }

        [TestMethod]
        public async Task VerifyCodeActionIsAvailableToSuppressLinterInfo()
        {
            var bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            await VerifyCodeActionIsAvailableToSuppressLinterDiagnostics(bicepConfigFileContents);
        }

        private async Task VerifyCodeActionIsAvailableToSuppressLinterDiagnostics(string bicepConfigFileContents)
        {
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            var bicepFileContents = @"param storageAccount string = 'testStorageAccount'";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents, testOutputPath);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var uri = documentUri.ToUri();

            var fileSystemDict = new Dictionary<Uri, string>();
            fileSystemDict[uri] = bicepFileContents;

            string bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents, testOutputPath);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);
            fileSystemDict[bicepConfigUri.ToUri()] = bicepConfigFileContents;

            var compilation = Services.BuildCompilation(fileSystemDict, uri);
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            diagnostics.Should().HaveCount(1);
            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Code.Should().Be("no-unused-params");
                });

            var helper = await ServerWithBuiltInTypes.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, bicepFileContents, documentUri);

            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            var codeActions = await helper.Client.RequestCodeAction(new CodeActionParams
            {
                TextDocument = new TextDocumentIdentifier(documentUri),
                Range = diagnostics.First().ToRange(lineStarts)
            });

            var disableCodeAction = codeActions.Single(x => x.CodeAction?.Title == "Disable no-unused-params for this line");
            disableCodeAction.CodeAction!.Edit!.Changes!.First().Value.First().NewText.Should().Be("#disable-next-line no-unused-params\n");
        }

        [TestMethod]
        public async Task VerifyCodeActionIsNotAvailableToSuppressCoreCompilerError()
        {
            var bicepFileContents = @"#disable-next-line BCP029 BCP068
resource test";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var uri = documentUri.ToUri();

            var files = new Dictionary<Uri, string>
            {
                [uri] = bicepFileContents,
            };

            var compilation = Services.BuildCompilation(files, uri);
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            diagnostics.Should().HaveCount(2);
            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP068");
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP029");
                });

            using var helper = await LanguageServerHelper.StartServerWithTextAsync(
                this.TestContext,
                bicepFileContents,
                documentUri,
                creationOptions: new Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create()));
            ILanguageClient client = helper.Client;

            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            var codeActions = await client.RequestCodeAction(new CodeActionParams
            {
                TextDocument = new TextDocumentIdentifier(documentUri),
                Range = diagnostics.First().ToRange(lineStarts)
            });

            codeActions.Should().BeEmpty();
        }

        [TestMethod]
        public async Task VerifyCodeActionIsAvailableToSuppressCoreCompilerWarning()
        {
            var bicepFileContents = @"var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
#disable-next-line no-hardcoded-location
  location: 'West US'
  properties: vmProperties
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var uri = documentUri.ToUri();

            var files = new Dictionary<Uri, string>
            {
                [uri] = bicepFileContents,
            };

            var compilation = Services.BuildCompilation(files, uri);
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            diagnostics.Should().HaveCount(3);
            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Warning);
                    x.Code.Should().Be("BCP036");
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Warning);
                    x.Code.Should().Be("BCP036");
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Warning);
                    x.Code.Should().Be("BCP037");
                });

            var helper = await ServerWithNamespaceProvider.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, bicepFileContents, documentUri);

            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            var codeActions = await helper.Client.RequestCodeAction(new CodeActionParams
            {
                TextDocument = new TextDocumentIdentifier(documentUri),
                Range = diagnostics.First().ToRange(lineStarts)
            });

            var disableCodeActions = codeActions.Where(x => x.CodeAction!.Title.StartsWith("Disable "));
            disableCodeActions.Count().Should().Be(2);
            disableCodeActions.Should().SatisfyRespectively(
                x =>
                {
                    x.CodeAction!.Title.Should().Be("Disable BCP036 for this line");
                    x.CodeAction.Edit!.Changes!.First().Value.First().NewText.Should().Be("#disable-next-line BCP036\n");
                },
                x =>
                {
                    x.CodeAction!.Title.Should().Be("Disable BCP037 for this line");
                    x.CodeAction.Edit!.Changes!.First().Value.First().NewText.Should().Be("#disable-next-line BCP037\n");
                });
        }

        [DataRow("string", "@secure()", SecureTitle)]
        [DataRow("object", "@secure()", SecureTitle)]
        [DataRow("string", "@description('')", DescriptionTitle)]
        [DataRow("object", "@description('')", DescriptionTitle)]
        [DataRow("array", "@description('')", DescriptionTitle)]
        [DataRow("bool", "@description('')", DescriptionTitle)]
        [DataRow("int", "@description('')", DescriptionTitle)]
        [DataRow("string", "@allowed([])", AllowedTitle)]
        [DataRow("object", "@allowed([])", AllowedTitle)]
        [DataRow("array", "@allowed([])", AllowedTitle)]
        [DataRow("bool", "@allowed([])", AllowedTitle)]
        [DataRow("int", "@allowed([])", AllowedTitle)]
        [DataRow("string", "@minLength()", MinLengthTitle)]
        [DataRow("array", "@minLength()", MinLengthTitle)]
        [DataRow("string", "@maxLength()", MaxLengthTitle)]
        [DataRow("array", "@maxLength()", MaxLengthTitle)]
        [DataRow("int", "@minValue()", MinValueTitle)]
        [DataRow("int", "@maxValue()", MaxValueTitle)]
        [DataTestMethod]
        public async Task Parameter_decorator_actions_are_suggested(string type, string decorator, string title)
        {
            (var codeActions, var bicepFile) = await RunParameterSyntaxTest(type);
            codeActions.Should().Contain(x => x.Title == title);
            codeActions.First(x => x.Title == title).Kind.Should().Be(CodeActionKind.Refactor);

            var updatedFile = ApplyCodeAction(bicepFile, codeActions.Single(x => x.Title == title));
            updatedFile.Should().HaveSourceText($@"
{decorator}
param foo {type}
");
        }

        [DataRow("string", "@secure()", SecureTitle)]
        [DataRow("object", "@secure()", SecureTitle)]
        [DataRow("string", "@description()", DescriptionTitle)]
        [DataRow("object", "@description()", DescriptionTitle)]
        [DataRow("array", "@description()", DescriptionTitle)]
        [DataRow("bool", "@description()", DescriptionTitle)]
        [DataRow("int", "@description()", DescriptionTitle)]
        [DataRow("string", "@allowed([])", AllowedTitle)]
        [DataRow("object", "@allowed([])", AllowedTitle)]
        [DataRow("array", "@allowed([])", AllowedTitle)]
        [DataRow("bool", "@allowed([])", AllowedTitle)]
        [DataRow("int", "@allowed([])", AllowedTitle)]
        [DataRow("string", "@minLength()", MinLengthTitle)]
        [DataRow("object", "@minLength()", MinLengthTitle)]
        [DataRow("string", "@maxLength()", MaxLengthTitle)]
        [DataRow("object", "@maxLength()", MaxLengthTitle)]
        [DataRow("int", "@minValue()", MinValueTitle)]
        [DataRow("int", "@maxValue()", MaxValueTitle)]
        [DataTestMethod]
        public async Task Parameter_duplicate_decorators_are_not_suggested(string type, string decorator, string title)
        {
            (var codeActions, var bicepFile) = await RunParameterSyntaxTest(type, decorator);
            codeActions.Should().NotContain(x => x.Title == title);
        }

        [DataRow("array", SecureTitle)]
        [DataRow("bool", SecureTitle)]
        [DataRow("int", SecureTitle)]
        [DataRow("object", MinLengthTitle)]
        [DataRow("bool", MinLengthTitle)]
        [DataRow("int", MinLengthTitle)]
        [DataRow("object", MaxLengthTitle)]
        [DataRow("bool", MaxLengthTitle)]
        [DataRow("int", MaxLengthTitle)]
        [DataRow("object", MinValueTitle)]
        [DataRow("bool", MinValueTitle)]
        [DataRow("string", MinValueTitle)]
        [DataRow("array", MinValueTitle)]
        [DataRow("object", MaxValueTitle)]
        [DataRow("bool", MaxValueTitle)]
        [DataRow("string", MaxValueTitle)]
        [DataRow("array", MaxValueTitle)]
        [DataTestMethod]
        public async Task Parameter_decorators_are_not_suggested_for_unsupported_type(string type, string title)
        {
            (var codeActions, var bicepFile) = await RunParameterSyntaxTest(type);
            codeActions.Should().NotContain(x => x.Title == title);
        }

        [DataRow(
            @"resource ap|p 'Microsoft.Web/sites@2021-03-01' existing = {
                name: 'app'
            }",
            "")]
        [DataRow(
            @"resource ap|p 'Microsoft.Web/sites@2021-03-01' existing = {
                name: 'app'
            }
var foo = 'foo'",
            "var foo = 'foo'")]
        [DataRow(
            @"resource app1 'Microsoft.Web/sites@2021-03-01' = {
                name: 'app1'
            }
resource ap|p2 'Microsoft.Web/sites@2021-03-01' existing = {
                name: 'app2'
            }",
            @"resource app1 'Microsoft.Web/sites@2021-03-01' = {
                name: 'app1'
            }
")]
        [DataTestMethod]
        public async Task Unused_existing_resource_actions_are_suggested(string fileWithCursors, string expectedText)
        {
            (var codeActions, var bicepFile) = await RunSyntaxTest(fileWithCursors);
            codeActions.Should().Contain(x => x.Title.StartsWith(RemoveUnusedExistingResourceTitle));
            codeActions.First(x => x.Title.StartsWith(RemoveUnusedExistingResourceTitle)).Kind.Should().Be(CodeActionKind.QuickFix);

            var updatedFile = ApplyCodeAction(bicepFile, codeActions.Single(x => x.Title.StartsWith(RemoveUnusedExistingResourceTitle)));
            updatedFile.Should().HaveSourceText(expectedText);
        }

        [DataRow(@"var fo|o = 'foo'
var foo2 = 'foo2'", "var foo2 = 'foo2'")]
        [DataRow(@"var fo|o = 'foo'", "")]
        [DataRow(@"var ad|sf = 'asdf' /*
        adf
        */", "")]
        [DataRow(@"var as|df = {
          abc: 'def'
        }", "")]
        [DataRow(@"var ab|cd = concat('foo',/*
        */'bar')", "")]
        [DataRow(@"var multi|line = '''
        This
        is
        a
        multiline
        '''", "")]
        [DataRow(@"@description('''
        ''')
        var as|df = 'asdf'", "")]
        [DataRow(@"var fo|o = 'asdf' // asdef", "")]
        [DataRow(@"/* asdfds */var fo|o = 'asdf'", "")]
        [DataRow(@"/* asdf */var fo|o = 'asdf'
var bar = 'asdf'", "var bar = 'asdf'")]
        [DataTestMethod]
        public async Task Unused_variable_actions_are_suggested(string fileWithCursors, string expectedText)
        {
            (var codeActions, var bicepFile) = await RunSyntaxTest(fileWithCursors);
            codeActions.Should().Contain(x => x.Title.StartsWith(RemoveUnusedVariableTitle));
            codeActions.First(x => x.Title.StartsWith(RemoveUnusedVariableTitle)).Kind.Should().Be(CodeActionKind.QuickFix);

            var updatedFile = ApplyCodeAction(bicepFile, codeActions.Single(x => x.Title.StartsWith(RemoveUnusedVariableTitle)));
            updatedFile.Should().HaveSourceText(expectedText);
        }

        [DataRow(@"param fo|o string
param foo2 string", "param foo2 string")]
        [DataRow(@"param fo|o string", "")]
        [DataRow(@"#disable-next-line foo
param as|df string = '123'", "#disable-next-line foo\n")]
        [DataRow(@"@secure()
param fo|o string
param foo2 string", "param foo2 string")]
        [DataTestMethod]
        public async Task Unused_parameter_actions_are_suggested(string fileWithCursors, string expectedText)
        {
            (var codeActions, var bicepFile) = await RunSyntaxTest(fileWithCursors);
            codeActions.Should().Contain(x => x.Title.StartsWith(RemoveUnusedParameterTitle));
            codeActions.First(x => x.Title.StartsWith(RemoveUnusedParameterTitle)).Kind.Should().Be(CodeActionKind.QuickFix);

            var updatedFile = ApplyCodeAction(bicepFile, codeActions.Single(x => x.Title.StartsWith(RemoveUnusedParameterTitle)));
            updatedFile.Should().HaveSourceText(expectedText);
        }

        [DataRow("var|")]
        [DataRow("var |")]
        [DataTestMethod]
        public async Task Unused_variable_actions_are_not_suggested_for_invalid_variables(string fileWithCursors)
        {
            var (codeActions, _) = await RunSyntaxTest(fileWithCursors);
            codeActions.Should().NotContain(x => x.Title.StartsWith(RemoveUnusedVariableTitle));
        }

        [DataRow("param|")]
        [DataRow("param |")]
        [DataTestMethod]
        public async Task Unused_parameter_actions_are_not_suggested_for_invalid_parameters(string fileWithCursors)
        {
            var (codeActions, _) = await RunSyntaxTest(fileWithCursors);
            codeActions.Should().NotContain(x => x.Title.StartsWith(RemoveUnusedParameterTitle));
        }

        [DataRow(@"
var foo = { |abc: 'def', ghi: 'jkl', }
", @"
var foo = {
  abc: 'def'
  ghi: 'jkl'
}
")]
        [DataRow(@"
var foo = { |abc: 'def'
ghi: 'jkl', }
", @"
var foo = {
  abc: 'def'
  ghi: 'jkl'
}
")]
        [DataRow(@"
var foo = ['abc', |'def']
", @"
var foo = [
  'abc'
  'def'
]
")]
        [DataRow(@"
var foo = [|'abc', 'def'
'ghi']
", @"
var foo = [
  'abc'
  'def'
  'ghi'
]
")]
        [DataRow(@"
@allowed(['va|l1', 'val2'])
param foo string
", @"
@allowed([
  'val1'
  'val2'
])
param foo string
")]
        [DataRow(@"
var nested = [
  { foo: [ ['bar'], 12|3 ] }
]
", @"
var nested = [
  { foo: [
    [ 'bar' ]
    123
  ] }
]
")]
        [DataTestMethod]
        public async Task Single_line_object_and_arrays_have_multi_line_format_codefix(string fileWithCursors, string result)
        {
            var (codeActions, bicepFile) = await RunSyntaxTest(fileWithCursors);
            codeActions.Should().Contain(x => x.Title == MultilineObjectsAndArraysCodeFixProvider.ConvertToMultiLineDescription);
            codeActions.First(x => x.Title == MultilineObjectsAndArraysCodeFixProvider.ConvertToMultiLineDescription).Kind.Should().Be(CodeActionKind.Refactor);

            var updatedFile = ApplyCodeAction(bicepFile, codeActions.Single(x => x.Title == MultilineObjectsAndArraysCodeFixProvider.ConvertToMultiLineDescription));
            updatedFile.Should().HaveSourceText(result);
        }

        [DataRow(@"
var foo = {
  abc: 'def'
  ghi:| 'jkl'
}
")]
        [DataRow(@"
var foo = [
  'abc'|
  'def'
]
")]
        [DataTestMethod]
        public async Task Multi_line_object_and_arrays_should_not_show_multi_line_format_codefix(string fileWithCursors)
        {
            var (codeActions, bicepFile) = await RunSyntaxTest(fileWithCursors);
            codeActions.Should().NotContain(x => x.Title == MultilineObjectsAndArraysCodeFixProvider.ConvertToMultiLineDescription);
        }

        [DataRow(@"
var foo = {
  abc: 'def'|
  ghi: 'jkl'
}
", @"
var foo = { abc: 'def', ghi: 'jkl' }
")]
        [DataRow(@"
var foo = { |abc: 'def'
ghi: 'jkl', }
", @"
var foo = { abc: 'def', ghi: 'jkl' }
")]
        [DataRow(@"
var foo = [
  'abc'|
  'def'
]
", @"
var foo = [ 'abc', 'def' ]
")]
        [DataRow(@"
var foo = [|'abc', 'def'
'ghi']
", @"
var foo = [ 'abc', 'def', 'ghi' ]
")]
        [DataRow(@"
var nested = [
  { foo: [ ['bar']
   12|3 ]}
]
", @"
var nested = [
  { foo: [ [ 'bar' ], 123 ]}
]
")]
        [DataTestMethod]
        public async Task Multi_line_object_and_arrays_have_single_line_format_codefix(string fileWithCursors, string result)
        {
            var (codeActions, bicepFile) = await RunSyntaxTest(fileWithCursors);
            codeActions.Should().Contain(x => x.Title == MultilineObjectsAndArraysCodeFixProvider.ConvertToSingleLineDescription);
            codeActions.First(x => x.Title == MultilineObjectsAndArraysCodeFixProvider.ConvertToSingleLineDescription).Kind.Should().Be(CodeActionKind.Refactor);

            var updatedFile = ApplyCodeAction(bicepFile, codeActions.Single(x => x.Title == MultilineObjectsAndArraysCodeFixProvider.ConvertToSingleLineDescription));
            updatedFile.Should().HaveSourceText(result);
        }

        [DataRow(@"
var foo = {|abc: 'def', ghi: 'jkl',}
")]
        [DataRow(@"
var foo = [|'abc', 'def',]
")]
        [DataTestMethod]
        public async Task Single_line_object_and_arrays_should_not_show_single_line_format_codefix(string fileWithCursors)
        {
            var (codeActions, bicepFile) = await RunSyntaxTest(fileWithCursors);
            codeActions.Should().NotContain(x => x.Title == MultilineObjectsAndArraysCodeFixProvider.ConvertToSingleLineDescription);
        }

        [DataRow(@"
var foo = {
  |,,,,
}
")]
        [DataRow(@"
var foo = {|
  a:
}
")]
        [DataRow(@"
var foo = {| a: }
")]
        [DataRow(@"
var foo = [
  |,,,,
]
")]
        [DataRow(@"
var foo = [ |abc def ]
")]
        [DataTestMethod]
        public async Task Incomplete_declarations_should_not_show_codefixes(string fileWithCursors)
        {
            var (codeActions, bicepFile) = await RunSyntaxTest(fileWithCursors);
            codeActions.Should().NotContain(x => x.Title == MultilineObjectsAndArraysCodeFixProvider.ConvertToSingleLineDescription);
            codeActions.Should().NotContain(x => x.Title == MultilineObjectsAndArraysCodeFixProvider.ConvertToMultiLineDescription);
        }

        [DataTestMethod]
        [DataRow(MultilineObjectsAndArraysCodeFixProvider.ConvertToSingleLineDescription, @"var foo = {
  abc: 'def'
  ghi:| 'jkl'
  deep: [|
    {|
      nesting: {|
        here: '!!!'
      }
    }
  ]
}
")]
        [DataRow(MultilineObjectsAndArraysCodeFixProvider.ConvertToMultiLineDescription, @"var foo = {
  abc: 'def'
  ghi:| 'jkl'
  deep: [|
    {|
      nesting: {|
        here: '!!!', same: 'line'
      }, same: 'line'
    }, 'same line'
  ], same: 'line'
}
")]
        public async Task Single_and_multi_line_actions_should_indent_to_match_the_formatter(string codeActionName, string fileWithCursors)
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri($"file://{TestContext.TestName}_{Guid.NewGuid():D}/main.bicep"), file);
            bicepFile.Should().NotHaveParseErrors("this test is only designed for valid Bicep files");

            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, bicepFile);
            var fileVersion = 0;

            foreach (var cursor in cursors)
            {
                using (new AssertionScope().WithVisualCursor(bicepFile, new TextSpan(cursor, 0)))
                {
                    await helper.ChangeFileAsync(TestContext, bicepFile, ++fileVersion);
                    var codeActions = await RequestCodeActions(helper.Client, bicepFile, cursor);
                    var codeAction = GetSingleCodeAction(codeActions, codeActionName);

                    var codeActionUpdatedFile = ApplyCodeAction(bicepFile, codeAction);

                    await helper.ChangeFileAsync(TestContext, codeActionUpdatedFile, ++fileVersion);
                    var formattedFile = await FormatDocument(helper.Client, codeActionUpdatedFile);

                    codeActionUpdatedFile.Should().HaveEquivalentSourceText(formattedFile);
                }
            }
        }

        private async Task<(IEnumerable<CodeAction> codeActions, BicepFile bicepFile)> RunParameterSyntaxTest(string paramType, string? decorator = null)
        {
            string fileWithCursors = @$"
param fo|o {paramType}
";
            if (decorator is not null)
            {
                fileWithCursors = @$"
{decorator}
param fo|o {paramType}
";
            }
            return await RunSyntaxTest(fileWithCursors);
        }

        private async Task<(IEnumerable<CodeAction> codeActions, BicepFile bicepFile)> RunSyntaxTest(string fileWithCursors)
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri($"file://{TestContext.TestName}_{Guid.NewGuid():D}/main.bicep"), file);

            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(TestContext, file, bicepFile.FileUri);

            var codeActions = await RequestCodeActions(helper.Client, bicepFile, cursors.Single());
            return (codeActions, bicepFile);
        }

        private static IEnumerable<TextSpan> GetOverlappingSpans(TextSpan span)
        {
            // NOTE: These code assumes there are no errors in the code that are exactly adject to each other or that overlap

            // Same span.
            yield return span;

            // Adjacent spans before.
            int startOffset = Math.Max(0, span.Position - 1);
            yield return new TextSpan(startOffset, 1);
            yield return new TextSpan(span.Position, 0);

            // Adjacent spans after.
            yield return new TextSpan(span.GetEndPosition(), 1);
            yield return new TextSpan(span.GetEndPosition(), 0);

            // Overlapping spans.
            yield return new TextSpan(startOffset, 2);
            yield return new TextSpan(span.Position + 1, span.Length);
            yield return new TextSpan(startOffset, span.Length + 1);
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.NonStressDataSets.ToDynamicTestData();
        }

        private static async Task<IEnumerable<CodeAction>> RequestCodeActions(ILanguageClient client, BicepFile bicepFile, int cursor)
        {
            var startPosition = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor);
            var endPosition = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor);

            var result = await client.RequestCodeAction(new CodeActionParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.FileUri),
                Range = new Range(startPosition, endPosition),
            });

            return result.Select(x => x.CodeAction).WhereNotNull();
        }

        private static BicepFile ApplyCodeAction(BicepFile bicepFile, CodeAction codeAction, params string[] tabStops)
        {
            // only support a small subset of possible edits for now - can always expand this later on
            codeAction.Edit!.Changes.Should().NotBeNull();
            codeAction.Edit.Changes.Should().HaveCount(1);
            codeAction.Edit.Changes.Should().ContainKey(bicepFile.FileUri);

            var changes = codeAction.Edit.Changes![bicepFile.FileUri];
            changes.Should().HaveCount(1);

            var replacement = changes.Single();

            var start = PositionHelper.GetOffset(bicepFile.LineStarts, replacement.Range.Start);
            var end = PositionHelper.GetOffset(bicepFile.LineStarts, replacement.Range.End);
            var textToInsert = replacement.NewText;

            // the handler can contain tabs. convert to double space to simplify printing.
            textToInsert = textToInsert.Replace("\t", "  ");

            var originalFile = bicepFile.ProgramSyntax.ToTextPreserveFormatting();
            var replaced = originalFile.Substring(0, start) + textToInsert + originalFile.Substring(end);

            return SourceFileFactory.CreateBicepFile(bicepFile.FileUri, replaced);
        }

        private static CodeAction GetSingleCodeAction(IEnumerable<CodeAction> codeActions, string codeActionName)
        {
            codeActions.Should().ContainSingle(x => x.Title == codeActionName);

            return codeActions.Single(x => x.Title == codeActionName);
        }

        private static async Task<BicepFile> FormatDocument(ILanguageClient client, BicepFile bicepFile)
        {
            var textEditContainer = await client.TextDocument.RequestDocumentFormatting(new DocumentFormattingParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.FileUri),
                Options = new FormattingOptions
                {
                    TabSize = 2,
                    InsertSpaces = true,
                    InsertFinalNewline = true,
                },
            });

            return SourceFileFactory.CreateBicepFile(bicepFile.FileUri, textEditContainer!.Single().NewText);
        }
    }
}
