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
using Moq;
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

        private MockRepository Repository = new(MockBehavior.Strict);

        [TestMethod]
        public async Task BicepConfigFileModification_ShouldNotRefreshCompilation()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) = await StartServerWithClientConnectionAsync();

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
            var bicepFileContents = @"param storageAccountName string = 'test'";

            var mainUri = SaveFileAndUpdateFileSystemDictionary("main.bicep", bicepFileContents, testOutputPath, fileSystemDict);
            var bicepConfigUri = SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, testOutputPath, fileSystemDict);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    mainUri,
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }

            // update bicepconfig.json and verify diagnostics message is not sent
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

                await diagsListener.EnsureNoMessageSent();
            }
        }

        [TestMethod]
        public async Task BicepConfigFileDeletion_ShouldRefreshCompilation()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) = await StartServerWithClientConnectionAsync();

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

            var bicepFileContents = @"param storageAccountName string = 'test'";

            var mainUri = SaveFileAndUpdateFileSystemDictionary("main.bicep", bicepFileContents, testOutputPath, fileSystemDict);
            var bicepConfigUri = SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, testOutputPath, fileSystemDict);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

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
                File.Delete(bicepConfigUri.GetFileSystemPath());

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
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) = await StartServerWithClientConnectionAsync();

            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFileAndUpdateFileSystemDictionary("main.bicep", bicepFileContents, testOutputPath, fileSystemDict);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

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
                var bicepConfigUri = SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, testOutputPath, fileSystemDict);

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
        public async Task SavingBicepConfigFile_ShouldRefreshCompilation()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) = await StartServerWithClientConnectionAsync();

            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFileAndUpdateFileSystemDictionary("main.bicep", bicepFileContents, testOutputPath, fileSystemDict);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

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

                var bicepConfigUri = SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, testOutputPath, fileSystemDict);

                client.Workspace.DidChangeWatchedFiles(new DidChangeWatchedFilesParams
                {
                    Changes = new Container<FileEvent>(new FileEvent
                    {
                        Type = FileChangeType.Changed,
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
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string parentDirectoryPath) = await StartServerWithClientConnectionAsync();

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

            SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, parentDirectoryPath, fileSystemDict);

            var childDirectoryPath = Path.Combine(parentDirectoryPath, "child");
            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFileAndUpdateFileSystemDictionary("main.bicep", bicepFileContents, childDirectoryPath, fileSystemDict);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

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

                var newBicepConfigUri = SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, childDirectoryPath, fileSystemDict);

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
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string parentDirectoryPath) = await StartServerWithClientConnectionAsync();

            var childDirectoryPath = Path.Combine(parentDirectoryPath, "child");
            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFileAndUpdateFileSystemDictionary("main.bicep", bicepFileContents, childDirectoryPath, fileSystemDict);

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
            SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, childDirectoryPath, fileSystemDict);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

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
                var newBicepConfigUri = SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, parentDirectoryPath, fileSystemDict);

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
        public async Task FixingErrorsInInvalidBicepConfigFileAndSaving_ShouldRefreshCompilation()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) = await StartServerWithClientConnectionAsync();

            var bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
";
            var bicepFileContents = @"param storageAccountName string = 'test'";

            var bicepConfigUri = SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, testOutputPath, fileSystemDict);
            var mainUri = SaveFileAndUpdateFileSystemDictionary("main.bicep", bicepFileContents, testOutputPath, fileSystemDict);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

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
                File.WriteAllText(bicepConfigUri.GetFileSystemPath(), @"{
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
}");
                client.Workspace.DidChangeWatchedFiles(new DidChangeWatchedFilesParams
                {
                    Changes = new Container<FileEvent>(new FileEvent
                    {
                        Type = FileChangeType.Changed,
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
        public async Task WithMultipleConfigFiles_ShouldUseConfigSettingsFromRelevantDirectory()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string parentDirectoryPath) = await StartServerWithClientConnectionAsync();

            var childDirectoryPath = Path.Combine(parentDirectoryPath, "child");

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
            SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, childDirectoryPath, fileSystemDict);

            string bicepFileContents = @"param storageAccountName string = 'test'";
            var documentUriOfFileInChildDirectory = SaveFileAndUpdateFileSystemDictionary("main.bicep", bicepFileContents, childDirectoryPath, fileSystemDict);
            var uriOfFileInChildDirectory = documentUriOfFileInChildDirectory.ToUri();

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(uriOfFileInChildDirectory, bicepFileContents, 1));

                await VerifyDiagnosticsAsync(diagsListener,
                    @"Parameter ""storageAccountName"" is declared but never used.",
                    uriOfFileInChildDirectory,
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }

            // add bicepconfig.json to parent directory and verify diagnostics
            {
                var documentUriOfFileInParentDirectory = SaveFileAndUpdateFileSystemDictionary("main.bicep", bicepFileContents, parentDirectoryPath, fileSystemDict);
                var uriOfFileInParentDirectory = documentUriOfFileInParentDirectory.ToUri();

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
                var newBicepConfigUri = SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, parentDirectoryPath, fileSystemDict);

                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(uriOfFileInParentDirectory, bicepFileContents, 1));

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
                    uriOfFileInParentDirectory,
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params");
            }
        }

        [TestMethod]
        public async Task ModifyBicepConfigFileAndVerifyCallsToFileResolver()
        {
            var mockFileResolver = Repository.Create<IFileResolver>();
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) = await StartServerWithClientConnectionAsync(mockFileResolver.Object);

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
            var bicepFileContents = @"param storageAccountName string = 'test'";

            var mainUri = SaveFileAndUpdateFileSystemDictionary("main.bicep", bicepFileContents, testOutputPath, fileSystemDict);
            var bicepConfigUri = SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, testOutputPath, fileSystemDict);

            // open main.bicep and update bicepconfig.json. Verify FileResolver.FileExists(..) was called twice
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                await diagsListener.WaitNext();

                mockFileResolver.Verify(x => x.FileExists(bicepConfigUri.ToUri()), Times.Once);

                File.WriteAllText(bicepConfigUri.GetFileSystemPath(), @"{
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
}");
                client.Workspace.DidChangeWatchedFiles(new DidChangeWatchedFilesParams
                {
                    Changes = new Container<FileEvent>(new FileEvent
                    {
                        Type = FileChangeType.Changed,
                        Uri = bicepConfigUri,
                    })
                });

                mockFileResolver.Verify(x => x.FileExists(bicepConfigUri.ToUri()), Times.Once);
            }
        }

        [TestMethod]
        public async Task ModifyBicepFileAndVerifyCallsToFileResolver()
        {
            var mockFileResolver = Repository.Create<IFileResolver>();
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) = await StartServerWithClientConnectionAsync(mockFileResolver.Object);

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
            var bicepFileContents = @"param storageAccountName string = 'test'";

            var mainUri = SaveFileAndUpdateFileSystemDictionary("main.bicep", bicepFileContents, testOutputPath, fileSystemDict);
            var bicepConfigUri = SaveFileAndUpdateFileSystemDictionary("bicepconfig.json", bicepConfigFileContents, testOutputPath, fileSystemDict);

            // open and update main.bicep. Verify FileResolver.FileExists(..) was called only once
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));
                await diagsListener.WaitNext();

                mockFileResolver.Verify(x => x.FileExists(bicepConfigUri.ToUri()), Times.Once);

                client.TextDocument.DidChangeTextDocument(TextDocumentParamHelper.CreateDidChangeTextDocumentParams(mainUri, @"param storageAccountId int = 123", 2));

                mockFileResolver.VerifyNoOtherCalls();
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
                    x.Message.Should().Contain(message);
                    x.Severity.Should().Be(diagnosticSeverity);
                    x.Code?.String.Should().Be(code);
                    x.Range.Should().Be(new Range
                    {
                        Start = start,
                        End = end
                    });
                });
        }

        private async Task<(ILanguageClient, Dictionary<Uri, string>, MultipleMessageListener<PublishDiagnosticsParams>, string testOutputPath)> StartServerWithClientConnectionAsync(IFileResolver? fileResolver = null)
        {
            var fileSystemDict = new Dictionary<Uri, string>();
            if (fileResolver is null)
            {
                fileResolver = new InMemoryFileResolver(fileSystemDict);
            }
            var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var serverOptions = new Server.CreationOptions(FileResolver: fileResolver);
            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                TestContext,
                options =>
                {
                    options.OnPublishDiagnostics(diags => diagsListener.AddMessage(diags));
                },
                serverOptions);

            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            return (client, fileSystemDict, diagsListener, testOutputPath);
        }

        private DocumentUri SaveFileAndUpdateFileSystemDictionary(string fileName, string fileContents, string testOutputPath, Dictionary<Uri, string> fileSystemDict)
        {
            string path = FileHelper.SaveResultFile(TestContext, fileName, fileContents, testOutputPath);
            var documentUri = DocumentUri.FromFileSystemPath(path);
            fileSystemDict[documentUri.ToUri()] = fileContents;

            return documentUri;
        }
    }
}
