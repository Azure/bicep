// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Configuration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LangServer.UnitTests.Configuration
{
    [TestClass]
    public class BicepConfigChangeHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private string CurrentDirectory = Directory.GetCurrentDirectory();

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
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics!.Count().Should().Be(1);

            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be(@"Parameter ""storageAccountName"" is declared but never used.");
                    x.Severity.Should().Be(DiagnosticSeverity.Information);
                    x.Code?.String.Should().Be("https://aka.ms/bicep/linter/no-unused-params");
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
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics!.Count().Should().Be(1);
            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().MatchRegex(@"Failed to parse the contents of the Bicep configuration file "".+bicepconfig.json"" as valid JSON: ""Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed. LineNumber: 8 | BytePositionInLine: 13."".");
                    x.Severity.Should().Be(DiagnosticSeverity.Error);
                    x.Code?.String.Should().Be("Invalid Bicep Configuration");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 0),
                        End = new Position(1, 0)
                    });
                });
        }

        [TestMethod]
        public void RefreshCompilationOfSourceFilesInWorkspace_WithBicepConfigFileThatDoesntAdhereToSchema_ShouldRefreshCompilation()
        {
            var bicepFileContents = "param storageAccountName string = 'testAccount'";

            var bicepConfigFileContents = @"{
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""no-unused-params"": {
                      ""level"": ""abcdef""
                    }
                  }
                }
              }
            }";

            RefreshCompilationOfSourceFilesInWorkspace(bicepFileContents,
                                                       bicepConfigFileContents,
                                                       saveBicepConfigFile: true,
                                                       out Mock<ITextDocumentLanguageServer> document,
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics!.Count().Should().Be(1);

            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be(@"Parameter ""storageAccountName"" is declared but never used.");
                    x.Severity.Should().Be(DiagnosticSeverity.Warning);
                    x.Code?.String.Should().Be("https://aka.ms/bicep/linter/no-unused-params");
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
                                                       out Container<Diagnostic>? diagnostics);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics!.Count().Should().Be(1);

            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be(@"Parameter ""storageAccountName"" is declared but never used.");
                    x.Severity.Should().Be(DiagnosticSeverity.Warning);
                    x.Code?.String.Should().Be("https://aka.ms/bicep/linter/no-unused-params");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 6),
                        End = new Position(0, 24)
                    });
                });
        }

        private void RefreshCompilationOfSourceFilesInWorkspace(string bicepFileContents, string bicepConfigFileContents, bool saveBicepConfigFile, out Mock<ITextDocumentLanguageServer> document, out Container<Diagnostic>? diagnostics)
        {
            PublishDiagnosticsParams? receivedParams = null;
            document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            ILanguageServerFacade server = BicepCompilationManagerHelper.CreateMockServer(document).Object;

            string testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath);
            var workspace = new Workspace();

            var bicepCompilationManager = new BicepCompilationManager(server, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(), workspace, new FileResolver(), BicepCompilationManagerHelper.CreateMockScheduler().Object, new ConfigurationManager(new IOFileSystem()));
            bicepCompilationManager.UpsertCompilation(DocumentUri.From(bicepFilePath), null, bicepFileContents, LanguageConstants.LanguageId);

            var bicepConfigDocumentUri = DocumentUri.FromFileSystemPath(bicepFilePath);

            if (saveBicepConfigFile)
            {
                string bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents, testOutputPath);
                bicepConfigDocumentUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);
            }

            BicepConfigChangeHandler.RefreshCompilationOfSourceFilesInWorkspace(bicepCompilationManager, workspace);

            diagnostics = receivedParams?.Diagnostics;
        }
    }
}
