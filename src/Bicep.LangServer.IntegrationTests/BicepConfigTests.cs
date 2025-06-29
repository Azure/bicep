// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
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
        private readonly MockFileSystem fileSystem = new();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task BicepConfigFileModification_ShouldNotRefreshCompilation()
        {
            var diagsListener = GetDiagnosticListener();
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

            var mainUri = SaveFile("main.bicep", bicepFileContents);
            var bicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "no-unused-params"));
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
            var diagsListener = GetDiagnosticListener();
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

            var mainUri = SaveFile("main.bicep", bicepFileContents);
            var bicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "no-unused-params"));
            }

            // Delete bicepconfig.json and verify diagnostics are based off of default bicepconfig.json
            {
                this.fileSystem.RemoveFile("bicepconfig.json");

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
                    "no-unused-params"));
            }
        }

        [TestMethod]
        public async Task BicepConfigFileCreation_ShouldRefreshCompilation()
        {
            var diagsListener = GetDiagnosticListener();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFile("main.bicep", bicepFileContents);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "no-unused-params"));
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
                var bicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents);

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
                    "no-unused-params"));
            }
        }

        [TestMethod]
        public async Task SavingBicepConfigFile_ShouldRefreshCompilation()
        {
            var diagsListener = GetDiagnosticListener();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFile("main.bicep", bicepFileContents);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Warning,
                    new Position(0, 6),
                    new Position(0, 24),
                    "no-unused-params"));
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

                var bicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents);

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
                    "no-unused-params"));
            }
        }

        [TestMethod]
        public async Task WithBicepConfigInParentDirectory_WhenNewBicepConfigFileIsAddedToCurrentDirectory_ShouldUseNewlyAddedConfigSettings()
        {
            var diagsListener = GetDiagnosticListener();
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

            SaveFile("bicepconfig.json", bicepConfigFileContents);

            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFile("parent/main.bicep", bicepFileContents);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "no-unused-params"));
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

                var newBicepConfigUri = SaveFile("parent/bicepconfig.json", bicepConfigFileContents);

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
                    "no-unused-params"));
            }
        }

        [TestMethod]
        public async Task WithBicepConfigInCurrentDirectory_WhenNewBicepConfigFileIsAddedToParentDirectory_ShouldUseOldConfigSettings()
        {
            var diagsListener = GetDiagnosticListener();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

            var bicepFileContents = @"param storageAccountName string = 'test'";
            var mainUri = SaveFile("parent/main.bicep", bicepFileContents);

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
            SaveFile("parent/bicepconfig.json", bicepConfigFileContents);

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                await VerifyDiagnosticsAsync(diagsListener, mainUri, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "no-unused-params"));
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
                var newBicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents);

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
                    "no-unused-params"));
            }
        }

        [TestMethod]
        public async Task FixingErrorsInInvalidBicepConfigFileAndSaving_ShouldRefreshCompilation()
        {
            var diagsListener = GetDiagnosticListener();
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

            var bicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents);
            var mainUri = SaveFile("main.bicep", bicepFileContents);

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
                        "no-unused-params"));
            }

            // update bicepconfig.json and verify diagnostics
            {
                this.fileSystem.File.WriteAllText("bicepconfig.json", @"{
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
                    "no-unused-params"));
            }
        }

        [TestMethod]
        public async Task WithMultipleConfigFiles_ShouldUseConfigSettingsFromRelevantDirectory()
        {
            var diagsListener = GetDiagnosticListener();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

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
            SaveFile("parent/bicepconfig.json", bicepConfigFileContents);

            string bicepFileContents = @"param storageAccountName string = 'test'";
            var documentUriOfFileInChildDirectory = SaveFile("parent/main.bicep", bicepFileContents);
            var uriOfFileInChildDirectory = documentUriOfFileInChildDirectory.ToUriEncoded();

            // open the main document and verify diagnostics
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(uriOfFileInChildDirectory, bicepFileContents, 1));

                await VerifyDiagnosticsAsync(diagsListener, uriOfFileInChildDirectory, (@"Parameter ""storageAccountName"" is declared but never used.",
                    DiagnosticSeverity.Information,
                    new Position(0, 6),
                    new Position(0, 24),
                    "no-unused-params"));
            }

            // add bicepconfig.json to parent directory and verify diagnostics
            {
                var documentUriOfFileInParentDirectory = SaveFile("main.bicep", bicepFileContents);
                var uriOfFileInParentDirectory = documentUriOfFileInParentDirectory.ToUriEncoded();

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
                var newBicepConfigUri = SaveFile("bicepconfig.json", bicepConfigFileContents);

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
                    "no-unused-params"));
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
            var diagsListener = GetDiagnosticListener();
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

            var mainUri = SaveFile("main.bicep", bicepFileContents);
            var bicepConfigUri = SaveFile("bicepconfig.json", bicepConfigContents);

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

        private static MultipleMessageListener<PublishDiagnosticsParams> GetDiagnosticListener() => new();

        private async Task<LanguageServerHelper> StartServerWithClientConnectionAsync(MultipleMessageListener<PublishDiagnosticsParams> diagsListener)
        {
            var fileSystemDict = new Dictionary<Uri, string>();
            return await LanguageServerHelper.StartServer(
                TestContext,
                options => options.OnPublishDiagnostics(diagsListener.AddMessage),
                services => services.WithFileSystem(this.fileSystem));
        }

        private DocumentUri SaveFile(string fileName, string fileContents)
        {
            this.fileSystem.AddFile(fileName, fileContents);

            return DocumentUri.FromFileSystemPath(fileSystem.Path.GetFullPath(fileName));
        }
    }
}
