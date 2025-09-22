// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.FileSystem;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using LocalFileSystem = System.IO.Abstractions.FileSystem;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.UnitTests.Configuration
{
    [TestClass]
    public class BicepConfigChangeHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static readonly LinterRulesProvider LinterRulesProvider = new();

        [TestMethod]
        public void RefreshCompilationOfSourceFilesInWorkspace_WithValidBicepConfigFile_ShouldRefreshCompilation()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";

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

            RefreshCompilationOfSourceFilesInWorkspace(bicepFileContents,
                                                       bicepConfigFileContents,
                                                       saveBicepConfigFile: true,
                                                       out Mock<ITextDocumentLanguageServer> document,
                                                       out _,
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics!.Count().Should().Be(1);

            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be(@"Parameter ""storageAccountName"" is declared but never used.");
                    x.Severity.Should().Be(DiagnosticSeverity.Information);
                    x.Code?.String.Should().Be("no-unused-params");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 6),
                        End = new Position(0, 24)
                    });
                });
        }

        [TestMethod]
        public void RefreshCompilationOfSourceFilesInWorkspace_WithInvalidBicepConfigFile_ShouldRefreshCompilation()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";

            var bicepConfigFileContents = @"{
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""no-unused-params"": {
                      ""level"": ""warning""
            }";

            RefreshCompilationOfSourceFilesInWorkspace(bicepFileContents,
                                                       bicepConfigFileContents,
                                                       saveBicepConfigFile: true,
                                                       out Mock<ITextDocumentLanguageServer> document,
                                                       out var configFilePath,
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be(@$"Failed to parse the contents of the Bicep configuration file ""{configFilePath}"" as valid JSON: Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed. LineNumber: 8 | BytePositionInLine: 13.");
                    x.Severity.Should().Be(DiagnosticSeverity.Error);
                    x.Code?.String.Should().Be("BCP271");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 0),
                        End = new Position(0, 0)
                    });
                },
                x =>
                {
                    x.Message.Should().Be(@"Parameter ""storageAccountName"" is declared but never used.");
                    x.Severity.Should().Be(DiagnosticSeverity.Warning);
                    x.Code?.String.Should().Be("no-unused-params");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 6),
                        End = new Position(0, 24)
                    });
                });
        }

        [TestMethod]
        public void RefreshCompilationOfSourceFilesInWorkspace_WithBicepConfigFileThatDoesntAdhereToSchema_ShouldRefreshCompilation()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";

            var bicepConfigFileContents = """
                {
                  "analyzers": {
                    "core": {
                      "verbose": false,
                      "enabled": true,
                      "rules": {
                        "no-unused-params": {
                          "level": "abcdef"
                        }
                      }
                    }
                  }
                }
                """;

            RefreshCompilationOfSourceFilesInWorkspace(bicepFileContents,
                                                       bicepConfigFileContents,
                                                       saveBicepConfigFile: true,
                                                       out Mock<ITextDocumentLanguageServer> document,
                                                       out _,
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics!.Count().Should().Be(1);

            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be(@"Parameter ""storageAccountName"" is declared but never used.");
                    x.Severity.Should().Be(DiagnosticSeverity.Warning);
                    x.Code?.String.Should().Be("no-unused-params");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 6),
                        End = new Position(0, 24)
                    });
                });
        }

        [TestMethod]
        public void RefreshCompilationOfSourceFilesInWorkspace_WithEmptySourceFile_ShouldNotRefreshCompilation()
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

            RefreshCompilationOfSourceFilesInWorkspace(string.Empty,
                                                       bicepConfigFileContents,
                                                       saveBicepConfigFile: true,
                                                       out _,
                                                       out _,
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void RefreshCompilationOfSourceFilesInWorkspace_WithoutBicepConfigFile_ShouldUseDefaultConfigAndRefreshCompilation()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";

            RefreshCompilationOfSourceFilesInWorkspace(bicepFileContents,
                                                       string.Empty,
                                                       saveBicepConfigFile: false,
                                                       out Mock<ITextDocumentLanguageServer> document,
                                                       out _,
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics!.Count().Should().Be(1);

            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be(@"Parameter ""storageAccountName"" is declared but never used.");
                    x.Severity.Should().Be(DiagnosticSeverity.Warning);
                    x.Code?.String.Should().Be("no-unused-params");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 6),
                        End = new Position(0, 24)
                    });
                });
        }

        private void RefreshCompilationOfSourceFilesInWorkspace(string bicepFileContents, string bicepConfigFileContents, bool saveBicepConfigFile, out Mock<ITextDocumentLanguageServer> document, out string? bicepConfigFilePath, out Container<Diagnostic>? diagnostics)
        {
            PublishDiagnosticsParams? receivedParams = null;
            document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            ILanguageServerFacade server = BicepCompilationManagerHelper.CreateMockServer(document).Object;

            var mockFileSystem = new MockFileSystem();

            var bicepFilePath = "/input.bicep";
            mockFileSystem.AddFile(bicepFilePath, bicepFileContents);

            if (saveBicepConfigFile)
            {
                bicepConfigFilePath = mockFileSystem.Path.GetFullPath("/bicepconfig.json");
                mockFileSystem.AddFile(bicepConfigFilePath, bicepConfigFileContents);
            }
            else
            {
                bicepConfigFilePath = null;
            }

            var workspace = new ActiveSourceFileSet();
            var fileExplorer = new FileSystemFileExplorer(mockFileSystem);
            var configurationManager = new ConfigurationManager(fileExplorer);
            var sourceFileFactory = new SourceFileFactory(configurationManager, BicepTestConstants.FeatureProviderFactory, BicepTestConstants.AuxiliaryFileCache, BicepTestConstants.FileExplorer);
            var bicepCompilationManager = new BicepCompilationManager(
                server,
                BicepCompilationManagerHelper.CreateEmptyCompilationProvider(configurationManager),
                workspace,
                BicepCompilationManagerHelper.CreateMockScheduler().Object,
                BicepTestConstants.CreateMockTelemetryProvider().Object,
                new LinterRulesProvider(),
                sourceFileFactory,
                BicepTestConstants.AuxiliaryFileCache);
            bicepCompilationManager.OpenCompilation(DocumentUri.From(InMemoryFileResolver.GetFileUri(bicepFilePath)), null, bicepFileContents, LanguageConstants.LanguageId);

            var bicepConfigChangeHandler = new BicepConfigChangeHandler(bicepCompilationManager,
                                                                        configurationManager,
                                                                        LinterRulesProvider,
                                                                        BicepTestConstants.CreateMockTelemetryProvider().Object,
                                                                        workspace);

            bicepConfigChangeHandler.RefreshCompilationOfSourceFilesInWorkspace();

            diagnostics = receivedParams?.Diagnostics;
        }
    }
}
