// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests;
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

namespace Bicep.LangServer.UnitTests.Configuration
{
    // Search for bicepconfig.json in DiscoverLocalConfigurationFile(..) in ConfigHelper starts from current directory.
    // In the below tests, we'll explicitly set the current directory and disable running tests in parallel to avoid conflicts
    [TestClass]
    public class BicepConfigChangeHandlerTests
    {
        private readonly BicepConfigChangeHandler bicepConfigChangeHandler = new(BicepTestConstants.FileResolver);
        private readonly string CurrentDirectory = Directory.GetCurrentDirectory();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        [DoNotParallelize]
        public void RetriggerCompilationOfSourceFilesInWorkspace_WithValidBicepConfigFile_ShouldRetriggerCompilation()
        {
            string bicepFileContents = "param storageAccountName string = 'testAccount'";

            string bicepConfigFileContents = @"{
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

            RetriggerCompilationOfSourceFilesInWorkspace(bicepFileContents,
                                                         bicepConfigFileContents,
                                                         saveBicepConfigFile: true,
                                                         out Mock<ITextDocumentLanguageServer> document,
                                                         out Container<Diagnostic>? diagnostics);

            // Should push diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

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
        [DoNotParallelize]
        public void RetriggerCompilationOfSourceFilesInWorkspace_WithInvalidBicepConfigFile_ShouldRetriggerCompilation()
        {
            string bicepFileContents = "param storageAccountName string = 'testAccount'";

            string bicepConfigFileContents = @"{
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""no-unused-params"": {
                      ""level"": ""warning""
            }";

            RetriggerCompilationOfSourceFilesInWorkspace(bicepFileContents,
                                                         bicepConfigFileContents,
                                                         saveBicepConfigFile: true,
                                                         out Mock<ITextDocumentLanguageServer> document,
                                                         out Container<Diagnostic>? diagnostics);

            // Should push diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics!.Count().Should().Be(1);
            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be("Could not load configuration file. Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed. LineNumber: 8 | BytePositionInLine: 13.");
                    x.Severity.Should().Be(DiagnosticSeverity.Error);
                    x.Code?.String.Should().Be("Fatal");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 0),
                        End = new Position(1, 0)
                    });
                });
        }

        [TestMethod]
        [DoNotParallelize]
        public void RetriggerCompilationOfSourceFilesInWorkspace_WithBicepConfigFileThatDoesntAdhereToSchema_ShouldRetriggerCompilation()
        {
            string bicepFileContents = "param storageAccountName string = 'testAccount'";

            string bicepConfigFileContents = @"{
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

            RetriggerCompilationOfSourceFilesInWorkspace(bicepFileContents,
                                                         bicepConfigFileContents,
                                                         saveBicepConfigFile: true,
                                                         out Mock<ITextDocumentLanguageServer> document,
                                                         out Container<Diagnostic>? diagnostics);

            // Should push diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

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
        [DoNotParallelize]
        public void RetriggerCompilationOfSourceFilesInWorkspace_WithEmptySourceFile_ShouldNotRetriggerCompilation()
        {
            string bicepConfigFileContents = @"{
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""no-unused-params"": {
                      ""level"": ""w""
                    }
                  }
                }
              }
            }";

            RetriggerCompilationOfSourceFilesInWorkspace(string.Empty,
                                                         bicepConfigFileContents,
                                                         saveBicepConfigFile: true,
                                                         out Mock<ITextDocumentLanguageServer> document,
                                                         out Container<Diagnostic>? diagnostics);

            // Shouldn't push diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);

            diagnostics.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void RetriggerCompilationOfSourceFilesInWorkspace_WithoutBicepConfigFile_ShouldUseDefaultConfigAndRetriggerCompilation()
        {
            string bicepFileContents = "param storageAccountName string = 'testAccount'";

            RetriggerCompilationOfSourceFilesInWorkspace(bicepFileContents,
                                                         string.Empty,
                                                         saveBicepConfigFile: false,
                                                         out Mock<ITextDocumentLanguageServer> document,
                                                         out Container<Diagnostic>? diagnostics);

            // Should push diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

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

        private void RetriggerCompilationOfSourceFilesInWorkspace(string bicepFileContents, string bicepConfigFileContents, bool saveBicepConfigFile, out Mock<ITextDocumentLanguageServer> document, out Container<Diagnostic>? diagnostics)
        {
            PublishDiagnosticsParams? receivedParams = null;
            document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            ILanguageServerFacade server = BicepCompilationManagerHelper.CreateMockServer(document).Object;

            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var workspace = new Workspace();
            ISourceFile sourceFile = SourceFileFactory.CreateSourceFile(new Uri(bicepFilePath), bicepFileContents);
            workspace.UpsertSourceFile(sourceFile);

            var bicepCompilationManager = new BicepCompilationManager(server, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(), workspace, new FileResolver(), BicepCompilationManagerHelper.CreateMockScheduler().Object);

            DocumentUri bicepConfigDocumentUri = DocumentUri.From("some_path");

            if (saveBicepConfigFile)
            {
                string bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents);
                Directory.SetCurrentDirectory(Path.GetDirectoryName(bicepConfigFilePath)!);
                bicepConfigDocumentUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);
            }

            bicepConfigChangeHandler.RetriggerCompilationOfSourceFilesInWorkspace(bicepCompilationManager, bicepConfigDocumentUri.ToUri(), workspace, bicepConfigFileContents);

            diagnostics = receivedParams?.Diagnostics;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.SetCurrentDirectory(CurrentDirectory);
        }
    }
}
