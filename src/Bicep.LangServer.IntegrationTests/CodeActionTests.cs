// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.CodeAction;
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
using Bicep.LanguageServer;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
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
        private const string SecureTitle = "Add @secure";
        private const string DescriptionTitle = "Add @description";
        private const string AllowedTitle = "Add @allowed";
        private const string MinLengthTitle = "Add @minLength";
        private const string MaxLengthTitle = "Add @maxLength";
        private const string MinValueTitle = "Add @minValue";
        private const string MaxValueTitle = "Add @maxValue";
        private const string RemoveUnusedVariableTitle = "Remove unused variable";
        private const string RemoveUnusedParameterTitle = "Remove unused parameter";

        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task RequestingCodeActionWithFixableDiagnosticsShouldProduceQuickFixes(DataSet dataSet)
        {
            var (compilation, _, fileUri) = await dataSet.SetupPrerequisitesAndCreateCompilation(this.TestContext);
            var uri = DocumentUri.From(fileUri);

            // start language server
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(this.TestContext, dataSet.Bicep, uri, creationOptions: new LanguageServer.Server.CreationOptions(FileResolver: new FileResolver()));
            var client = helper.Client;

            // construct a parallel compilation
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;
            var fixables = compilation.GetEntrypointSemanticModel().GetAllDiagnostics().OfType<IFixable>();

            foreach (var fixable in fixables)
            {
                foreach (var span in GetOverlappingSpans(fixable.Span))
                {
                    var quickFixes = await client.RequestCodeAction(new CodeActionParams
                    {
                        TextDocument = new TextDocumentIdentifier(uri),
                        Range = span.ToRange(lineStarts),
                    });

                    // Assert.
                    quickFixes.Should().NotBeNull();

                    var quickFixList = quickFixes.Where(x => x.CodeAction?.Kind == CodeActionKind.QuickFix).ToList();
                    var bicepFixList = fixable.Fixes.ToList();

                    quickFixList.Should().HaveSameCount(bicepFixList);

                    for (int i = 0; i < quickFixList.Count; i++)
                    {
                        var quickFix = quickFixList[i];
                        var bicepFix = bicepFixList[i];

                        quickFix.IsCodeAction.Should().BeTrue();
                        quickFix.CodeAction!.Kind.Should().Be(CodeActionKind.QuickFix);
                        quickFix.CodeAction.Title.Should().Be(bicepFix.Description);
                        quickFix.CodeAction.Edit!.Changes.Should().ContainKey(uri);

                        var textEditList = quickFix.CodeAction.Edit.Changes![uri].ToList();
                        var replacementList = bicepFix.Replacements.ToList();

                        for (int j = 0; j < textEditList.Count; j++)
                        {
                            var textEdit = textEditList[j];
                            var replacement = replacementList[j];

                            textEdit.Range.Should().Be(replacement.ToRange(lineStarts));
                            textEdit.NewText.Should().Be(replacement.Text);
                        }
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

            var compilation = new Compilation(BicepTestConstants.Features, BicepTestConstants.NamespaceProvider, SourceFileGroupingFactory.CreateForFiles(fileSystemDict, uri, BicepTestConstants.FileResolver, BicepTestConstants.BuiltInConfiguration), BicepTestConstants.BuiltInConfiguration, BicepTestConstants.LinterAnalyzer);
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            diagnostics.Should().HaveCount(1);
            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Code.Should().Be("no-unused-params");
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

            var compilation = new Compilation(BicepTestConstants.Features, BicepTestConstants.NamespaceProvider, SourceFileGroupingFactory.CreateForFiles(files, uri, BicepTestConstants.FileResolver, BicepTestConstants.BuiltInConfiguration), BicepTestConstants.BuiltInConfiguration, BicepTestConstants.LinterAnalyzer);
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

            var compilation = new Compilation(BicepTestConstants.Features, BicepTestConstants.NamespaceProvider, SourceFileGroupingFactory.CreateForFiles(files, uri, BicepTestConstants.FileResolver, BicepTestConstants.BuiltInConfiguration), BicepTestConstants.BuiltInConfiguration, BicepTestConstants.LinterAnalyzer);
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

            using var helper = await LanguageServerHelper.StartServerWithTextAsync(
                this.TestContext,
                bicepFileContents,
                documentUri,
                creationOptions: new Server.CreationOptions(NamespaceProvider: BicepTestConstants.NamespaceProvider));
            ILanguageClient client = helper.Client;

            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;

            var codeActions = await client.RequestCodeAction(new CodeActionParams
            {
                TextDocument = new TextDocumentIdentifier(documentUri),
                Range = diagnostics.First().ToRange(lineStarts)
            });

            codeActions.Count().Should().Be(2);

            codeActions.Should().SatisfyRespectively(
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
            codeActions.Should().Contain(x => x.Title == RemoveUnusedVariableTitle);
            codeActions.First(x => x.Title == RemoveUnusedVariableTitle).Kind.Should().Be(CodeActionKind.QuickFix);

            var updatedFile = ApplyCodeAction(bicepFile, codeActions.Single(x => x.Title == RemoveUnusedVariableTitle));
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
            codeActions.Should().Contain(x => x.Title == RemoveUnusedParameterTitle);
            codeActions.First(x => x.Title == RemoveUnusedParameterTitle).Kind.Should().Be(CodeActionKind.QuickFix);

            var updatedFile = ApplyCodeAction(bicepFile, codeActions.Single(x => x.Title == RemoveUnusedParameterTitle));
            updatedFile.Should().HaveSourceText(expectedText);
        }

        [DataRow("var|")]
        [DataRow("var |")]
        [DataTestMethod]
        public async Task Unused_variable_actions_are_not_suggested_for_invalid_variables(string fileWithCursors)
        {
            var (codeActions, _) = await RunSyntaxTest(fileWithCursors);
            codeActions.Should().NotContain(x => x.Title == RemoveUnusedVariableTitle);
        }

        [DataRow("param|")]
        [DataRow("param |")]
        [DataTestMethod]
        public async Task Unused_parameter_actions_are_not_suggested_for_invalid_parameters(string fileWithCursors)
        {
            var (codeActions, _) = await RunSyntaxTest(fileWithCursors);
            codeActions.Should().NotContain(x => x.Title == RemoveUnusedParameterTitle);
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
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///main.bicep"), file);
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(TestContext, file, bicepFile.FileUri);
            var client = helper.Client;

            var codeActions = await RequestCodeActions(client, bicepFile, cursors.Single());
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
    }
}
