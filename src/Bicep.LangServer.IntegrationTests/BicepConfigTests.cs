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
    // Search for bicepconfig.json in DiscoverLocalConfigurationFile(..) in ConfigHelper starts from current directory.
    // In the below tests, we'll explicitly set the current directory and disable running tests in parallel to avoid conflicts
    [TestClass]
    [DoNotParallelize]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class BicepConfigTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }
        private readonly string CurrentDirectory = Directory.GetCurrentDirectory();

        [TestMethod]
        public async Task BicepConfigFileModification_ShouldRefreshCompilation()
        {
            (ILanguageClient client, Dictionary<Uri, string> fileSystemDict, MultipleMessageListener<PublishDiagnosticsParams> diagsListener) = await StartServerWithClientConnectionAsync();
            var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
            fileSystemDict[mainUri.ToUri()] = @"param storageAccountName string = 'test'";

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
            Directory.SetCurrentDirectory(Path.GetDirectoryName(bicepConfigFilePath)!);
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
            var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
            fileSystemDict[mainUri.ToUri()] = @"param storageAccountName string = 'test'";

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
            Directory.SetCurrentDirectory(Path.GetDirectoryName(bicepConfigFilePath)!);
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
            var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
            fileSystemDict[mainUri.ToUri()] = @"param storageAccountName string = 'test'";

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

                string bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents);
                Directory.SetCurrentDirectory(Path.GetDirectoryName(bicepConfigFilePath)!);
                var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);

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
            var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
            fileSystemDict[mainUri.ToUri()] = @"param storageAccountName string = 'test'";

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
            string? directoryContainingBicepConfigFile = Path.GetDirectoryName(bicepConfigFilePath);
            DirectoryInfo directoryInfo = Directory.CreateDirectory(Path.Combine(directoryContainingBicepConfigFile!, "BicepConfig"));
            string currentDirectory = Path.Combine(directoryInfo.FullName);
            Directory.SetCurrentDirectory(currentDirectory);
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
                bicepConfigFilePath = Path.Combine(currentDirectory, "bicepconfig.json");
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
            var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
            fileSystemDict[mainUri.ToUri()] = @"param storageAccountName string = 'test'";

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
            string currentDirectory = Path.GetDirectoryName(bicepConfigFilePath)!;
            Directory.SetCurrentDirectory(currentDirectory);
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
                DirectoryInfo? parentDirectory = Directory.GetParent(currentDirectory);
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
            var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
            fileSystemDict[mainUri.ToUri()] = @"param storageAccountName string = 'test'";

            string bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
";

            string bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents);
            Directory.SetCurrentDirectory(Path.GetDirectoryName(bicepConfigFilePath)!);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);

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

        [TestCleanup]
        public void Cleanup()
        {
            Directory.SetCurrentDirectory(CurrentDirectory);
        }
    }
}
