// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
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
            var (diagsListener, testOutputPath) = GetTestConfig();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

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

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
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
            var (diagsListener, testOutputPath) = GetTestConfig();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

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

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
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

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
            }
        }

        [TestMethod]
        public async Task BicepConfigFileCreation_ShouldRefreshCompilation()
        {
            var (diagsListener, testOutputPath) = GetTestConfig();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFile("main.bicep", bicepFileContents, testOutputPath);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
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

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
            }
        }

        [TestMethod]
        public async Task SavingBicepConfigFile_ShouldRefreshCompilation()
        {
            var (diagsListener, testOutputPath) = GetTestConfig();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFile("main.bicep", bicepFileContents, testOutputPath);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
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

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
            }
        }

        [TestMethod]
        public async Task WithBicepConfigInParentDirectory_WhenNewBicepConfigFileIsAddedToCurrentDirectory_ShouldUseNewlyAddedConfigSettings()
        {
            var (diagsListener, parentDirectoryPath) = GetTestConfig();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

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

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
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

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
            }
        }

        [TestMethod]
        public async Task WithBicepConfigInCurrentDirectory_WhenNewBicepConfigFileIsAddedToParentDirectory_ShouldUseOldConfigSettings()
        {
            var (diagsListener, parentDirectoryPath) = GetTestConfig();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

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

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
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

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
            }
        }

        [TestMethod]
        public async Task FixingErrorsInInvalidBicepConfigFileAndSaving_ShouldRefreshCompilation()
        {
            var (diagsListener, testOutputPath) = GetTestConfig();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

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
                    mainUri,
                    ($"Failed to parse the contents of the Bicep configuration file \"{bicepConfigUri.GetFileSystemPath()}\" as valid JSON",
                        DiagnosticSeverity.Error,
                        new Position(0, 0),
                        new Position(0, 0),
                        "BCP271"),
                    (@"Parameter ""storageAccountName"" is declared but never used.",
                        DiagnosticSeverity.Warning,
                        new Position(0, 6),
                        new Position(0, 24),
                        "https://aka.ms/bicep/linter/no-unused-params"));
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

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
            }
        }

        [TestMethod]
        public async Task WithMultipleConfigFiles_ShouldUseConfigSettingsFromRelevantDirectory()
        {
            var (diagsListener, parentDirectoryPath) = GetTestConfig();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

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

                await VerifyDiagnosticsAsync(diagsListener, uriOfFileInChildDirectory, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
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

                await VerifyDiagnosticsAsync(diagsListener, uriOfFileInParentDirectory, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "https://aka.ms/bicep/linter/no-unused-params"));
            }
        }

        [TestMethod]
        public async Task VerifyLinterErrorsCanBeSuppressedWithDisableNextLineDiagnosticsDirective()
        {
            var bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""error""
        }
      }
    }
  }
}";
            var bicepFileContents = @"#disable-next-line no-unused-params
param storageAccountName string = 'test'";

            await VerifyLinterDiagnosticsCanBeSuppressedWithDisableNextLineDiagnosticsDirective(bicepConfigFileContents, bicepFileContents);
        }

        [TestMethod]
        public async Task VerifyLinterWarningsCanBeSuppressedWithDisableNextLineDiagnosticsDirective()
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
            var bicepFileContents = @"#disable-next-line no-unused-params
param storageAccountName string = 'test'";

            await VerifyLinterDiagnosticsCanBeSuppressedWithDisableNextLineDiagnosticsDirective(bicepConfigFileContents, bicepFileContents);
        }

        [TestMethod]
        public async Task VerifyLinterInfoCanBeSuppressedWithDisableNextLineDiagnosticsDirective()
        {
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
            var bicepFileContents = @"#disable-next-line no-unused-params
param storageAccountName string = 'test'";

            await VerifyLinterDiagnosticsCanBeSuppressedWithDisableNextLineDiagnosticsDirective(bicepConfigFileContents, bicepFileContents);
        }

        private async Task VerifyLinterDiagnosticsCanBeSuppressedWithDisableNextLineDiagnosticsDirective(string bicepConfigContents, string bicepFileContents)
        {
            var (diagsListener, testOutputPath) = GetTestConfig();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

            var mainUri = SaveFile("main.bicep", bicepFileContents, testOutputPath);
            var bicepConfigUri = SaveFile("bicepconfig.json", bicepConfigContents, testOutputPath);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                var diagsParams = await diagsListener.WaitNext();
                diagsParams.Uri.Should().Be(mainUri);
                diagsParams.Diagnostics.Should().BeEmpty();
            }
        }

        private static async Task VerifyDiagnosticsAsync(MultipleMessageListener<PublishDiagnosticsParams> diagsListener,
            DocumentUri documentUri,
            params (string message,
            DiagnosticSeverity diagnosticSeverity,
            Position start,
            Position end,
            string code)[] expectedDiagnostics)
        {
            var diagsParams = await diagsListener.WaitNext();
            diagsParams.Uri.Should().Be(documentUri);
            diagsParams.Diagnostics.Should().SatisfyRespectively(expectedDiagnostics.Select<(string, DiagnosticSeverity, Position, Position, string), Action<Diagnostic>>(expected =>
                x =>
                {
                    var (message, diagnosticSeverity, start, end, code) = expected;
                    x.Message.Should().Contain(message);
                    x.Severity.Should().Be(diagnosticSeverity);
                    x.Code?.String.Should().Be(code);
                    x.Range.Should().Be(new Range
                    {
                        Start = start,
                        End = end
                    });
                }));
        }

        private (MultipleMessageListener<PublishDiagnosticsParams> diagsListener, string testOutputPath) GetTestConfig()
        {
            var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            return (diagsListener, testOutputPath);
        }

        private async Task<LanguageServerHelper> StartServerWithClientConnectionAsync(MultipleMessageListener<PublishDiagnosticsParams> diagsListener)
        {
            var fileSystemDict = new Dictionary<Uri, string>();
            var fileResolver = new InMemoryFileResolver(fileSystemDict);
            var serverOptions = new Server.CreationOptions(FileResolver: fileResolver);
            return await LanguageServerHelper.StartServerWithClientConnectionAsync(
                TestContext,
                options =>
                {
                    options.OnPublishDiagnostics(diags => diagsListener.AddMessage(diags));
                },
                serverOptions);
        }

        private DocumentUri SaveFile(string fileName, string fileContents, string testOutputPath)
        {
            string path = FileHelper.SaveResultFile(TestContext, fileName, fileContents, testOutputPath);
            var documentUri = DocumentUri.FromFileSystemPath(path);

            return documentUri;
        }
    }
}
