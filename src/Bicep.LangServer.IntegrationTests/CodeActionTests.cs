// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Analyzers;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Diagnostic = Bicep.Core.Diagnostics.Diagnostic;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
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
            var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, dataSet.Bicep, uri, creationOptions: new LanguageServer.Server.CreationOptions(FileResolver: new FileResolver()));

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

                    var quickFixList = quickFixes.Where(x => x.CodeAction.Kind == CodeActionKind.QuickFix).ToList();
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
        public async Task DisableLinterRule_CodeActionInvocation_WithoutBicepConfig_ShouldCreateConfigFileAndDisableRule()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";
            var expectedBicepConfigContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-hardcoded-env-urls"": {
          ""level"": ""warning"",
          ""disallowedhosts"": [
            ""gallery.azure.com"",
            ""management.core.windows.net"",
            ""management.azure.com"",
            ""database.windows.net"",
            ""core.windows.net"",
            ""login.microsoftonline.com"",
            ""graph.windows.net"",
            ""trafficmanager.net"",
            ""datalake.azure.net"",
            ""azuredatalakestore.net"",
            ""azuredatalakeanalytics.net"",
            ""vault.azure.net"",
            ""api.loganalytics.io"",
            ""asazure.windows.net"",
            ""region.asazure.windows.net"",
            ""batch.core.windows.net""
          ],
          ""excludedhosts"": [
            ""schema.management.azure.com""
          ]
        },
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}";
            await VerifyLinterRuleIsDisabledAsync(bicepFileContents,
                                                  null,
                                                  DiagnosticLevel.Warning,
                                                  @"Parameter ""storageAccountName"" is declared but never used.",
                                                  expectedBicepConfigContents);
        }

        [TestMethod]
        public async Task DisableLinterRule_CodeActionInvocation_WithBicepConfig_ShouldUpdateConfigFileAndDisableRule()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";
            var bicepConfigContents = @"{
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
            var expectedBicepConfigContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}";
            await VerifyLinterRuleIsDisabledAsync(bicepFileContents,
                                                  bicepConfigContents,
                                                  DiagnosticLevel.Info,
                                                  @"Parameter ""storageAccountName"" is declared but never used.",
                                                  expectedBicepConfigContents);

        }

        [TestMethod]
        public async Task DisableLinterRule_CodeActionInvocation_WithoutRulesNodeInBicepConfig_ShouldUpdateConfigFileAndDisableRule()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";
            var bicepConfigContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true
    }
  }
}";
            var expectedBicepConfigContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}";
            await VerifyLinterRuleIsDisabledAsync(bicepFileContents,
                                                  bicepConfigContents,
                                                  DiagnosticLevel.Warning,
                                                  @"Parameter ""storageAccountName"" is declared but never used.",
                                                  expectedBicepConfigContents);

        }

        [TestMethod]
        public async Task DisableLinterRule_CodeActionInvocation_WithoutRuleInBicepConfig_ShouldUpdateConfigFileAndDisableRule()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";
            var bicepConfigContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-vars"": {
          ""level"": ""warning""
        }
      }
    }
  }
}";
            var expectedBicepConfigContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-vars"": {
          ""level"": ""warning""
        },
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}";
            await VerifyLinterRuleIsDisabledAsync(bicepFileContents,
                                                  bicepConfigContents,
                                                  DiagnosticLevel.Warning,
                                                  @"Parameter ""storageAccountName"" is declared but never used.",
                                                  expectedBicepConfigContents);

        }

        [TestMethod]
        public async Task DisableLinterRule_CodeActionInvocation_WithoutLevelNodeInRule_ShouldUpdateConfigFileAndDisableRule()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";
            var bicepConfigContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
        }
      }
    }
  }
}";
            var expectedBicepConfigContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}";
            await VerifyLinterRuleIsDisabledAsync(bicepFileContents,
                                                  bicepConfigContents,
                                                  DiagnosticLevel.Warning,
                                                  @"Parameter ""storageAccountName"" is declared but never used.",
                                                  expectedBicepConfigContents);

        }

        [TestMethod]
        public async Task DisableLinterRule_CodeActionInvocation_WithOnlyCurlyBracesBicepConfig_ShouldCreateConfigFileAndDisableRule()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";
            var bicepConfigContents = @"{}";
            var expectedBicepConfigContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-hardcoded-env-urls"": {
          ""level"": ""warning"",
          ""disallowedhosts"": [
            ""gallery.azure.com"",
            ""management.core.windows.net"",
            ""management.azure.com"",
            ""database.windows.net"",
            ""core.windows.net"",
            ""login.microsoftonline.com"",
            ""graph.windows.net"",
            ""trafficmanager.net"",
            ""datalake.azure.net"",
            ""azuredatalakestore.net"",
            ""azuredatalakeanalytics.net"",
            ""vault.azure.net"",
            ""api.loganalytics.io"",
            ""asazure.windows.net"",
            ""region.asazure.windows.net"",
            ""batch.core.windows.net""
          ],
          ""excludedhosts"": [
            ""schema.management.azure.com""
          ]
        },
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}";
            await VerifyLinterRuleIsDisabledAsync(bicepFileContents,
                                                  bicepConfigContents,
                                                  DiagnosticLevel.Warning,
                                                  @"Parameter ""storageAccountName"" is declared but never used.",
                                                  expectedBicepConfigContents);
        }


        private async Task VerifyLinterRuleIsDisabledAsync(string bicepFileContents, string? bicepConfigFileContents, DiagnosticLevel expectedDiagnosticLevel, string expectedDiagnosticMessage, string expectedBicepConfigFileContents)
        {
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents, testOutputPath);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);

            var fileSystemDict = new Dictionary<Uri, string>();
            fileSystemDict[documentUri.ToUri()] = bicepFileContents;

            string bicepConfigFilePath;

            if (bicepConfigFileContents is not null)
            {
                bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents, testOutputPath);
                var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);
                fileSystemDict[bicepConfigUri.ToUri()] = bicepConfigFileContents;
            }
            else
            {
                bicepConfigFilePath = Path.Combine(testOutputPath, LanguageConstants.BicepConfigSettingsFileName);
            }

            var workspace = new Workspace();
            var compilation = GetCompilation(testOutputPath, bicepFilePath, workspace);

            var serverOptions = new Server.CreationOptions(FileResolver: new InMemoryFileResolver(fileSystemDict));

            // Start language server
            var client = await IntegrationTestHelper.StartServerWithTextAsync(TestContext,
                bicepFileContents,
                documentUri,
                creationOptions: serverOptions);

            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            // Verify diagnostics before code action for disabling linter rule is invoked
            diagnostics.Should().HaveCount(1);
            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Level.Should().Be(expectedDiagnosticLevel);
                    x.Message.Should().Be(expectedDiagnosticMessage);
                });
            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;
            var disableLinterRuleCodeActionSpan = diagnostics.OfType<AnalyzerDiagnostic>().First().Span;

            var codeActions = await client.RequestCodeAction(new CodeActionParams
            {
                TextDocument = new TextDocumentIdentifier(documentUri),
                Range = disableLinterRuleCodeActionSpan.ToRange(lineStarts)
            });

            var command = codeActions.First().CodeAction!.Command;

            command!.Should().NotBeNull();
            command!.Name.Should().Be(LanguageConstants.DisableLinterRuleCommandName);

            await client.Workspace.ExecuteCommand(command);

            // Verify diagnostics is cleared
            GetCompilation(testOutputPath, bicepFilePath, workspace).GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();

            // Verify bicepconfig.json file contents
            File.ReadAllText(bicepConfigFilePath).Should().BeEquivalentToIgnoringNewlines(expectedBicepConfigFileContents);
        }

        private Compilation GetCompilation(string? folderContainingBicepConfig, string bicepFilePath, Workspace workspace)
        {
            var moduleRegistryProvider = new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.Features);
            var dispatcher = new ModuleDispatcher(moduleRegistryProvider);
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, workspace, PathHelper.FilePathToFileUrl(bicepFilePath));

            return new Compilation(TestTypeHelper.CreateEmptyProvider(), sourceFileGrouping, new ConfigHelper(folderContainingBicepConfig, BicepTestConstants.FileResolver));
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
