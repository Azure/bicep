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
    public class BicepConfigTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task BicepConfigFileModification_ShouldNotRefreshCompilation()
        {
            (ILanguageClient client, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) = await StartServerWithClientConnectionAsync();

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

            var mainUri = SaveFile("main.bicep", bicepFileContents, testOutputPath);
            var bicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents, testOutputPath);

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
            (ILanguageClient client, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) = await StartServerWithClientConnectionAsync();

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

            var mainUri = SaveFile("main.bicep", bicepFileContents, testOutputPath);
            var bicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents, testOutputPath);

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
            (ILanguageClient client, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) = await StartServerWithClientConnectionAsync();

            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFile("main.bicep", bicepFileContents, testOutputPath);

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
                var bicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents, testOutputPath);

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
            (ILanguageClient client, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) = await StartServerWithClientConnectionAsync();

            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFile("main.bicep", bicepFileContents, testOutputPath);

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

                var bicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents, testOutputPath);

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
            (ILanguageClient client, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string parentDirectoryPath) = await StartServerWithClientConnectionAsync();

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

            SaveFile("bicepconfig.json", bicepConfigFileContents, parentDirectoryPath);

            var childDirectoryPath = Path.Combine(parentDirectoryPath, "child");
            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFile("main.bicep", bicepFileContents, childDirectoryPath);

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

                var newBicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents, childDirectoryPath);

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
            (ILanguageClient client,  MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string parentDirectoryPath) = await StartServerWithClientConnectionAsync();

            var childDirectoryPath = Path.Combine(parentDirectoryPath, "child");
            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFile("main.bicep", bicepFileContents, childDirectoryPath);

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
            SaveFile("bicepconfig.json", bicepConfigFileContents, childDirectoryPath);

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
                var newBicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents, parentDirectoryPath);

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
            (ILanguageClient client, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) = await StartServerWithClientConnectionAsync();

            var bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
";
            var bicepFileContents = @"param storageAccountName string = 'test'";

            var bicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents, testOutputPath);
            var mainUri = SaveFile("main.bicep", bicepFileContents, testOutputPath);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                await VerifyDiagnosticsAsync(diagsListener,
                    $"Failed to parse the contents of the Bicep configuration file \"{bicepConfigUri.GetFileSystemPath()}\" as valid JSON",
                    mainUri,
                    DiagnosticSeverity.Error,
                    new Position(0, 0),
                    new Position(1, 0),
                    "Invalid Bicep Configuration");
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
            (ILanguageClient client, MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string parentDirectoryPath) = await StartServerWithClientConnectionAsync();

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
            SaveFile("bicepconfig.json", bicepConfigFileContents, childDirectoryPath);

            string bicepFileContents = @"param storageAccountName string = 'test'";
            var documentUriOfFileInChildDirectory = SaveFile("main.bicep", bicepFileContents, childDirectoryPath);
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
                var documentUriOfFileInParentDirectory = SaveFile("main.bicep", bicepFileContents, parentDirectoryPath);
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
                var newBicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents, parentDirectoryPath);

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

        private static async Task VerifyDiagnosticsAsync(MultipleMessageListener<PublishDiagnosticsParams> diagsListener,
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

        private async Task<(ILanguageClient, MultipleMessageListener<PublishDiagnosticsParams>, string testOutputPath)> StartServerWithClientConnectionAsync()
        {
            var fileSystemDict = new Dictionary<Uri, string>();
            var fileResolver = new InMemoryFileResolver(fileSystemDict);
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

            return (client, diagsListener, testOutputPath);
        }

        private DocumentUri SaveFile(string fileName, string fileContents, string testOutputPath)
        {
            string path = FileHelper.SaveResultFile(TestContext, fileName, fileContents, testOutputPath);
            var documentUri = DocumentUri.FromFileSystemPath(path);

            return documentUri;
        }
    }
}
