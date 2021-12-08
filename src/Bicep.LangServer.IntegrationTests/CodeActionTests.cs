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
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class CodeActionTests
    {
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

            foreach (IFixable fixable in fixables)
            {
                foreach (var span in GetOverlappingSpans(fixable.Span))
                {
                    CommandOrCodeActionContainer? quickFixes = await client.RequestCodeAction(new CodeActionParams
                    {
                        TextDocument = new TextDocumentIdentifier(uri),
                        Range = span.ToRange(lineStarts)
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

            var compilation = new Compilation(BicepTestConstants.NamespaceProvider, SourceFileGroupingFactory.CreateForFiles(fileSystemDict, uri, BicepTestConstants.FileResolver, BicepTestConstants.BuiltInConfiguration), BicepTestConstants.BuiltInConfiguration);
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

            codeActions.Should().SatisfyRespectively(
                x =>
                {
                    x.CodeAction!.Title.Should().Be("Disable no-unused-params for this line");
                    x.CodeAction.Edit!.Changes!.First().Value.First().NewText.Should().Be("#disable-next-line no-unused-params\n");
                });
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

            var compilation = new Compilation(BicepTestConstants.NamespaceProvider, SourceFileGroupingFactory.CreateForFiles(files, uri, BicepTestConstants.FileResolver, BicepTestConstants.BuiltInConfiguration), BicepTestConstants.BuiltInConfiguration);
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

            var compilation = new Compilation(BicepTestConstants.NamespaceProvider, SourceFileGroupingFactory.CreateForFiles(files, uri, BicepTestConstants.FileResolver, BicepTestConstants.BuiltInConfiguration), BicepTestConstants.BuiltInConfiguration);
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

        private static IEnumerable<TextSpan> GetOverlappingSpans(TextSpan span)
        {
            // Same span.
            yield return span;

            // Adjacent spans before.
            int startOffset = Math.Max(0, span.Position - 10);
            yield return new TextSpan(startOffset, 10);
            yield return new TextSpan(span.Position, 0);

            // Adjacent spans after.
            yield return new TextSpan(span.GetEndPosition(), 10);
            yield return new TextSpan(span.GetEndPosition(), 0);

            // Overlapping spans.
            yield return new TextSpan(startOffset, 11);
            yield return new TextSpan(span.Position + 1, span.Length);
            yield return new TextSpan(startOffset, span.Length + 10);
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.NonStressDataSets.ToDynamicTestData();
        }
    }
}
