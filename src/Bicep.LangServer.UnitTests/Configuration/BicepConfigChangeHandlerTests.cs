// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
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
    [TestClass]
    public class BicepConfigChangeHandlerTests
    {
        private readonly BicepConfigChangeHandler bicepConfigChangeHandler = new(BicepTestConstants.FileResolver);

        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
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
                                                         FileChangeType.Changed,
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
                                                         FileChangeType.Changed,
                                                         saveBicepConfigFile: true,
                                                         out Mock<ITextDocumentLanguageServer> document,
                                                         out Container<Diagnostic>? diagnostics);

            // Should push diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics!.Count().Should().Be(2);
            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be("Could not load configuration file. Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed. LineNumber: 8 | BytePositionInLine: 13.");
                    x.Severity.Should().Be(DiagnosticSeverity.Warning);
                    x.Code?.String.Should().Be("linter-internal-error");
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
                    x.Code?.String.Should().Be("https://aka.ms/bicep/linter/no-unused-params");
                    x.Range.Should().Be(new Range
                    {
                        Start = new Position(0, 6),
                        End = new Position(0, 24)
                    });
                });
        }

        [TestMethod]
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
                      ""level"": ""w""
                    }
                  }
                }
              }
            }";

            RetriggerCompilationOfSourceFilesInWorkspace(bicepFileContents,
                                                         bicepConfigFileContents,
                                                         FileChangeType.Changed,
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
                                                         FileChangeType.Changed,
                                                         saveBicepConfigFile: true,
                                                         out Mock<ITextDocumentLanguageServer> document,
                                                         out Container<Diagnostic>? diagnostics);

            // Shouldn't push diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Never);

            diagnostics.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void RetriggerCompilationOfSourceFilesInWorkspace_WhenBicepConfigFileIsDeleted_ShouldRetriggerCompilation()
        {
            string bicepFileContents = "param storageAccountName string = 'testAccount'";

            string bicepConfigFileContents = @"{
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

            RetriggerCompilationOfSourceFilesInWorkspace(bicepFileContents,
                                                         bicepConfigFileContents,
                                                         FileChangeType.Deleted,
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

        [TestMethod]
        public void RetriggerCompilationOfSourceFilesInWorkspace_WhenBicepConfigFileIsCreated_ShouldRetriggerCompilation()
        {
            string bicepFileContents = "param storageAccountName string = 'testAccount'";

            string bicepConfigFileContents = @"{
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

            RetriggerCompilationOfSourceFilesInWorkspace(bicepFileContents,
                                                         bicepConfigFileContents,
                                                         FileChangeType.Deleted,
                                                         saveBicepConfigFile: true,
                                                         out Mock<ITextDocumentLanguageServer> document,
                                                         out Container<Diagnostic>? diagnostics);

            // Should push diagnostics
            document.Verify(m => m.SendNotification(It.IsAny<PublishDiagnosticsParams>()), Times.Once);

            diagnostics.Should().BeNullOrEmpty();
        }

        private void RetriggerCompilationOfSourceFilesInWorkspace(string bicepFileContents, string bicepConfigFileContents, FileChangeType fileChangeType, bool saveBicepConfigFile, out Mock<ITextDocumentLanguageServer> document, out Container<Diagnostic>? diagnostics)
        {
            PublishDiagnosticsParams? receivedParams = null;
            document = BicepCompilationManagerHelper.CreateMockDocument(p => receivedParams = p);
            ILanguageServerFacade server = BicepCompilationManagerHelper.CreateMockServer(document).Object;
            string testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            string bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, testOutputPath, Encoding.UTF8);
            var workspace = new Workspace();
            ISourceFile sourceFile = SourceFileFactory.CreateSourceFile(new Uri(bicepFilePath), bicepFileContents);
            workspace.UpsertSourceFile(sourceFile);

            var bicepCompilationManager = new BicepCompilationManager(server, BicepCompilationManagerHelper.CreateEmptyCompilationProvider(), workspace, new FileResolver(), BicepCompilationManagerHelper.CreateMockScheduler().Object);

            string bicepConfigFilePath = string.Empty;
            DocumentUri bicepConfigDocumentUri = DocumentUri.From("some_path");

            if (saveBicepConfigFile)
            {
                bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents, testOutputPath, Encoding.UTF8);
                bicepConfigDocumentUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);
            }

            FileEvent fileEvent = new FileEvent
            {
                Uri = bicepConfigDocumentUri,
                Type = fileChangeType
            };

            bicepConfigChangeHandler.RetriggerCompilationOfSourceFilesInWorkspace(bicepCompilationManager, fileEvent, workspace);

            diagnostics = receivedParams?.Diagnostics;
        }
    }
}
