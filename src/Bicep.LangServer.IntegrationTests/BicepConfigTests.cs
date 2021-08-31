// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class BicepConfigTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task BicepConfigFileModification_ShouldRefreshCompilation()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener) = await StartServerWithClientConnectionAsync();

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

            var bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents);
            var mainUri = DocumentUri.FromFileSystemPath(Path.Combine(Path.GetDirectoryName(bicepConfigFilePath)!, "main.bicep"));
            fileSystemDict[mainUri.ToUri()] = @"param storageAccountName string = 'test'";

            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);
            fileSystemDict[bicepConfigUri.ToUri()] = bicepConfigFileContents;

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    mainUri,
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }

            // update bicepconfig.json and verify diagnostics
            {
                client.TextDocument.DidChangeTextDocument(TextDocumentParamHelper.CreateDidChangeTextDocumentParams(bicepConfigUri, @"{
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
}", 2));

                var diagsParams = await diagsListener.WaitNext();
                diagsParams.Uri.Should().Be(mainUri);
                diagsParams.Diagnostics.Should().BeEmpty();
            }
        }

        [TestMethod]
        public async Task BicepConfigFileDeletion_ShouldRefreshCompilation()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener) = await StartServerWithClientConnectionAsync();

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

            var bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents);
            var mainUri = DocumentUri.FromFileSystemPath(Path.Combine(Path.GetDirectoryName(bicepConfigFilePath)!, "main.bicep"));
            fileSystemDict[mainUri.ToUri()] = @"param storageAccountName string = 'test'";

            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);
            fileSystemDict[bicepConfigUri.ToUri()] = bicepConfigFileContents;

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    mainUri,
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }

            // Delete bicepconfig.json and verify diagnostics are based off of default bicepconfig.json
            {
                File.Delete(bicepConfigFilePath);

                client.Workspace.DidChangeWatchedFiles(new DidChangeWatchedFilesParams
                {
                    Changes = new Container<FileEvent>(new FileEvent
                    {
                        Type = FileChangeType.Deleted,
                        Uri = bicepConfigUri,
                    })
                });

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    mainUri,
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }
        }

        [TestMethod]
        public async Task BicepConfigFileCreation_ShouldRefreshCompilation()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener) = await StartServerWithClientConnectionAsync();
            var bicepFileContents = @"param storageAccountName string = 'test'";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var mainUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            fileSystemDict[mainUri.ToUri()] = bicepFileContents;

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    mainUri,
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }

            // Create bicepconfig.json and verify diagnostics
            {
                string bicepConfigFileContents = @"{
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
                FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents, Path.GetDirectoryName(bicepFilePath)!);
                var bicepConfigUri = DocumentUri.FromFileSystemPath(Path.Combine(Path.GetDirectoryName(bicepFilePath)!, "bicepconfig.json"));
                fileSystemDict[bicepConfigUri.ToUri()] = bicepConfigFileContents;

                client.Workspace.DidChangeWatchedFiles(new DidChangeWatchedFilesParams
                {
                    Changes = new Container<FileEvent>(new FileEvent
                    {
                        Type = FileChangeType.Created,
                        Uri = bicepConfigUri,
                    })
                });

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    mainUri,
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }
        }

        [TestMethod]
        public async Task WithBicepConfigInParentDirectory_WhenNewBicepConfigFileIsAddedToCurrentDirectory_ShouldUseNewlyAddedConfigSettings()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener) = await StartServerWithClientConnectionAsync();
            var bicepFileContents = @"param storageAccountName string = 'test'";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var mainUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            fileSystemDict[mainUri.ToUri()] = bicepFileContents;

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

            var bicepConfigFilePath = Path.Combine(Directory.GetParent(bicepFilePath)!.FullName, "bicepconfig.json");
            File.WriteAllText(bicepConfigFilePath, bicepConfigFileContents);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);

            fileSystemDict[bicepConfigUri.ToUri()] = bicepConfigFileContents;

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    mainUri,
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }

            // create new bicepconfig.json and verify diagnostics
            {
                bicepConfigFileContents = @"{
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
                bicepConfigFilePath = Path.Combine(Path.GetDirectoryName(bicepFilePath)!, "bicepconfig.json");
                File.WriteAllText(bicepConfigFilePath, bicepConfigFileContents);
                var newBicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);

                fileSystemDict[newBicepConfigUri.ToUri()] = bicepConfigFileContents;

                client.Workspace.DidChangeWatchedFiles(new DidChangeWatchedFilesParams
                {
                    Changes = new Container<FileEvent>(new FileEvent
                    {
                        Type = FileChangeType.Created,
                        Uri = newBicepConfigUri,
                    })
                });

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    mainUri,
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }
        }

        [TestMethod]
        public async Task WithBicepConfigInCurrentDirectory_WhenNewBicepConfigFileIsAddedToParentDirectory_ShouldUseOldConfigSettings()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener) = await StartServerWithClientConnectionAsync();
            var bicepFileContents = @"param storageAccountName string = 'test'";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var mainUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            fileSystemDict[mainUri.ToUri()] = bicepFileContents;

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

            var bicepConfigFilePath = Path.Combine(Path.GetDirectoryName(bicepFilePath)!, "bicepconfig.json");
            File.WriteAllText(bicepConfigFilePath, bicepConfigFileContents);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);
            fileSystemDict[bicepConfigUri.ToUri()] = bicepConfigFileContents;

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    mainUri,
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }

            // add bicepconfig.json to parent directory and verify diagnostics
            {
                bicepConfigFileContents = @"{
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
                var directoryContainingBicepFile = Path.GetDirectoryName(bicepFilePath)!;
                string parentDirectory = Directory.GetParent(directoryContainingBicepFile)!.FullName;
                string newBicepConfigFilePath = Path.Combine(parentDirectory, "bicepconfig.json");
                File.WriteAllText(newBicepConfigFilePath, bicepConfigFileContents);
                var newBicepConfigUri = DocumentUri.FromFileSystemPath(newBicepConfigFilePath);

                fileSystemDict[newBicepConfigUri.ToUri()] = bicepConfigFileContents;

                client.Workspace.DidChangeWatchedFiles(new DidChangeWatchedFilesParams
                {
                    Changes = new Container<FileEvent>(new FileEvent
                    {
                        Type = FileChangeType.Created,
                        Uri = newBicepConfigUri,
                    })
                });

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    mainUri,
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }
        }

        [TestMethod]
        public async Task InvalidBicepConfigFile_ShouldRefreshCompilation()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener) = await StartServerWithClientConnectionAsync();

            var bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
";

            var bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);
            var mainUri = DocumentUri.FromFileSystemPath(Path.Combine(Path.GetDirectoryName(bicepConfigFilePath)!, "main.bicep"));
            fileSystemDict[mainUri.ToUri()] = @"param storageAccountName string = 'test'";

            fileSystemDict[bicepConfigUri.ToUri()] = bicepConfigFileContents;

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Could not load configuration file. Expected depth to be zero at the end of the JSON payload. There is an open JSON object or array that should be closed. LineNumber: 7 | BytePositionInLine: 0.",
                    mainUri,
                    DiagnosticSeverity.Error,
                    new Position(0, 0),
                    new Position(1, 0),
                    "Fatal");
            }

            // update bicepconfig.json and verify diagnostics
            {
                client.TextDocument.DidChangeTextDocument(TextDocumentParamHelper.CreateDidChangeTextDocumentParams(bicepConfigUri, @"{
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
}", 2));

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    mainUri,
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }
        }

        [TestMethod]
        public async Task WithMultipleConfigFiles_ShouldUseConfigSettingsFromRelevantDirectory()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener) = await StartServerWithClientConnectionAsync();

            string bicepConfigFileContents = @"{
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
            string bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);
            fileSystemDict[bicepConfigUri.ToUri()] = bicepConfigFileContents;

            string? directory = Path.GetDirectoryName(bicepConfigFilePath);
            var documentUriOfFileInChildDirectory = DocumentUri.FromFileSystemPath(Path.Combine(directory!, "main.bicep"));
            var uriOfFileInChildDirectory = documentUriOfFileInChildDirectory.ToUri();
            string bicepFileContents = @"param storageAccountName string = 'test'";
            fileSystemDict[uriOfFileInChildDirectory] = bicepFileContents;

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUriOfFileInChildDirectory, fileSystemDict[uriOfFileInChildDirectory], 1));

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    documentUriOfFileInChildDirectory,
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }

            // add bicepconfig.json to parent directory and verify diagnostics
            {
                DirectoryInfo? parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory());
                var documentUriOfFileInParentDirectory = DocumentUri.FromFileSystemPath(Path.Combine(parentDirectory!.FullName, "main.bicep"));
                var uriOfFileInParentDirectory = documentUriOfFileInParentDirectory.ToUri();
                fileSystemDict[uriOfFileInParentDirectory] = bicepFileContents;

                bicepConfigFileContents = @"{
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
                string newBicepConfigFilePath = Path.Combine(parentDirectory!.FullName, "bicepconfig.json");
                File.WriteAllText(newBicepConfigFilePath, bicepConfigFileContents);
                var newBicepConfigDocumentUri = DocumentUri.FromFileSystemPath(newBicepConfigFilePath);
                var newBicepConfigUri = newBicepConfigDocumentUri.ToUri();
                fileSystemDict[newBicepConfigUri] = bicepConfigFileContents;

                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUriOfFileInParentDirectory, fileSystemDict[uriOfFileInParentDirectory], 1));

                client.Workspace.DidChangeWatchedFiles(new DidChangeWatchedFilesParams
                {
                    Changes = new Container<FileEvent>(new FileEvent
                    {
                        Type = FileChangeType.Created,
                        Uri = newBicepConfigUri,
                    })
                });

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    documentUriOfFileInParentDirectory,
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }
        }

        private async Task VerifyDiagnosticsAsync(MultipleMessageListener<PublishDiagnosticsParams> diagsListener,
            string message,
            DocumentUri documentUri, 
            DiagnosticSeverity diagnosticSeverity,
            Position start,
            Position end,
            string code)
        {
            var diagsParams = await diagsListener.WaitNext();
            diagsParams.Uri.Should().Be(documentUri);
            diagsParams.Diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Message.Should().Be(message);
                    x.Severity.Should().Be(diagnosticSeverity);
                    x.Code?.String.Should().Be(code);
                    x.Range.Should().Be(new Range
                    {
                        Start = start,
                        End = end
                    });
                });
        }

        private async Task<(ILanguageClient, Dictionary<Uri, string>, MultipleMessageListener<PublishDiagnosticsParams>)> StartServerWithClientConnectionAsync()
        {
            var fileSystemDict = new Dictionary<Uri, string>();
            var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var serverOptions = new Server.CreationOptions(FileResolver: new InMemoryFileResolver(fileSystemDict));
            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                TestContext,
                options =>
                {
                    options.OnPublishDiagnostics(diags => diagsListener.AddMessage(diags));
                },
                serverOptions);

            return (client, fileSystemDict, diagsListener);
        }
    }
}
