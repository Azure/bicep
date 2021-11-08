// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Analyzers;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class CodeActionTests
    {
        private static readonly INamespaceProvider NamespaceProvider = BicepTestConstants.NamespaceProvider;

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
        public async Task DisableLinterRuleCodeActionInvocation_WithoutBicepConfig_ShouldCreateConfigFileAndDisableRule()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";
            var expectedBicepConfigContents = @"{
  ""cloud"": {
    ""currentProfile"": ""AzureCloud"",
    ""profiles"": {
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.com""
      },
      ""AzureChinaCloud"": {
        ""resourceManagerEndpoint"": ""https://management.chinacloudapi.cn"",
        ""activeDirectoryAuthority"": ""https://login.chinacloudapi.cn""
      },
      ""AzureUSGovernment"": {
        ""resourceManagerEndpoint"": ""https://management.usgovcloudapi.net"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.us""
      }
    },
    ""credentialPrecedence"": [
      ""AzureCLI"",
      ""AzurePowerShell""
    ]
  },
  ""moduleAliases"": {
    ""ts"": {},
    ""br"": {}
  },
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
            await VerifyLinterRuleIsDisabledAsync(bicepFileContents: bicepFileContents,
                                                  bicepConfigFileContents: null,
                                                  diagnosticLevel: DiagnosticLevel.Warning,
                                                  diagnosticMessage: @"Parameter ""storageAccountName"" is declared but never used.",
                                                  expectedBicepConfigFileContents: expectedBicepConfigContents);
        }

        [TestMethod]
        public async Task DisableLinterRuleCodeActionInvocation_WithBicepConfig_ShouldUpdateConfigFileAndDisableRule()
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
            await VerifyLinterRuleIsDisabledAsync(bicepFileContents: bicepFileContents,
                                                  bicepConfigFileContents: bicepConfigContents,
                                                  diagnosticLevel: DiagnosticLevel.Info,
                                                  diagnosticMessage: @"Parameter ""storageAccountName"" is declared but never used.",
                                                  expectedBicepConfigFileContents: expectedBicepConfigContents);

        }

        [TestMethod]
        public async Task DisableLinterRuleCodeActionInvocation_WithoutRulesNodeInBicepConfig_ShouldUpdateConfigFileAndDisableRule()
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
            await VerifyLinterRuleIsDisabledAsync(bicepFileContents: bicepFileContents,
                                                  bicepConfigFileContents: bicepConfigContents,
                                                  diagnosticLevel: DiagnosticLevel.Warning,
                                                  diagnosticMessage: @"Parameter ""storageAccountName"" is declared but never used.",
                                                  expectedBicepConfigFileContents: expectedBicepConfigContents);

        }

        [TestMethod]
        public async Task DisableLinterRuleCodeActionInvocation_WithoutRuleInBicepConfig_ShouldUpdateConfigFileAndDisableRule()
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
            await VerifyLinterRuleIsDisabledAsync(bicepFileContents: bicepFileContents,
                                                  bicepConfigFileContents: bicepConfigContents,
                                                  diagnosticLevel: DiagnosticLevel.Warning,
                                                  diagnosticMessage: @"Parameter ""storageAccountName"" is declared but never used.",
                                                  expectedBicepConfigFileContents: expectedBicepConfigContents);

        }

        [TestMethod]
        public async Task DisableLinterRuleCodeActionInvocation_WithoutLevelPropertyInRule_ShouldUpdateConfigFileAndDisableRule()
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
            await VerifyLinterRuleIsDisabledAsync(bicepFileContents: bicepFileContents,
                                                  bicepConfigFileContents: bicepConfigContents,
                                                  diagnosticLevel: DiagnosticLevel.Warning,
                                                  diagnosticMessage: @"Parameter ""storageAccountName"" is declared but never used.",
                                                  expectedBicepConfigFileContents: expectedBicepConfigContents);

        }

        [TestMethod]
        public async Task DisableLinterRuleCodeActionInvocation_WithOnlyCurlyBracesInBicepConfig_ShouldUpdateConfigFileAndDisableRule()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";
            var bicepConfigContents = @"{}";
            var expectedBicepConfigContents = @"{
  ""cloud"": {
    ""currentProfile"": ""AzureCloud"",
    ""profiles"": {
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.com""
      },
      ""AzureChinaCloud"": {
        ""resourceManagerEndpoint"": ""https://management.chinacloudapi.cn"",
        ""activeDirectoryAuthority"": ""https://login.chinacloudapi.cn""
      },
      ""AzureUSGovernment"": {
        ""resourceManagerEndpoint"": ""https://management.usgovcloudapi.net"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.us""
      }
    },
    ""credentialPrecedence"": [
      ""AzureCLI"",
      ""AzurePowerShell""
    ]
  },
  ""moduleAliases"": {
    ""ts"": {},
    ""br"": {}
  },
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
            await VerifyLinterRuleIsDisabledAsync(bicepFileContents: bicepFileContents,
                                                  bicepConfigFileContents: bicepConfigContents,
                                                  diagnosticLevel: DiagnosticLevel.Warning,
                                                  diagnosticMessage: @"Parameter ""storageAccountName"" is declared but never used.",
                                                  expectedBicepConfigFileContents: expectedBicepConfigContents);
        }


        [TestMethod]
        public async Task VerifyDisableNextLineDiagnosticsCodeActionInvocation()
        {
            var bicepFileContents = @"param storageAccount string = 'testStorageAccount'";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var uri = documentUri.ToUri();

            var files = new Dictionary<Uri, string>
            {
                [uri] = bicepFileContents,
            };

            var compilation = new Compilation(BicepTestConstants.NamespaceProvider, SourceFileGroupingFactory.CreateForFiles(files, uri, BicepTestConstants.FileResolver, BicepTestConstants.BuiltInConfiguration), BicepTestConstants.BuiltInConfiguration);
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
                    x.CodeAction!.Title.Should().Be("Disable linter rule");
                },
                x =>
                {
                    x.CodeAction!.Title.Should().Be("Disable no-unused-params");
                });
        }

        private async Task VerifyLinterRuleIsDisabledAsync(string bicepFileContents, string? bicepConfigFileContents, DiagnosticLevel diagnosticLevel, string diagnosticMessage, string expectedBicepConfigFileContents)
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
                bicepConfigFilePath = Path.Combine(testOutputPath, LanguageConstants.BicepConfigurationFileName);
            }

            var workspace = new Workspace();
            var compilation = GetCompilation(bicepFilePath, workspace);

            var serverOptions = new Server.CreationOptions(FileResolver: new InMemoryFileResolver(fileSystemDict));

            // Start language server
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(TestContext,
                bicepFileContents,
                documentUri,
                creationOptions: serverOptions);
            var client = helper.Client;

            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            // Verify diagnostics before code action for disabling linter rule is invoked
            diagnostics.Should().HaveCount(1);
            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Level.Should().Be(diagnosticLevel);
                    x.Message.Should().Be(diagnosticMessage);
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
            GetCompilation(bicepFilePath, workspace).GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();

            // Verify bicepconfig.json file contents
            File.ReadAllText(bicepConfigFilePath).Should().BeEquivalentToIgnoringNewlines(expectedBicepConfigFileContents);
        }

        private Compilation GetCompilation(string bicepFilePath, Workspace workspace)
        {
            var moduleRegistryProvider = new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.Features);
            var dispatcher = new ModuleDispatcher(moduleRegistryProvider);
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, workspace, PathHelper.FilePathToFileUrl(bicepFilePath), BicepTestConstants.BuiltInConfiguration);
            var configuration = BicepTestConstants.ConfigurationManager.GetConfiguration(new Uri(bicepFilePath));

            return new Compilation(TestTypeHelper.CreateEmptyProvider(), sourceFileGrouping, configuration);
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
