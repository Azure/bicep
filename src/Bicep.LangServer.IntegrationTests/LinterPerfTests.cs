// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class LinterPerfTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task CollectPerfData_LargeTemplate()
        {
            var (diagsListener, testOutputPath) = GetTestConfig();
            var bicepFile = FileHelper.SaveResultFile(this.TestContext, "main.bicep", DataSets.LargeTemplate_Stress_LF.Bicep, testOutputPath);
            var bicepFileContents = File.ReadAllText(bicepFile);
            var mainUri = SaveFile("main.bicep", bicepFileContents, testOutputPath);
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

            // open the main document with analyzer turned on
            {
                Trace.WriteLine("---- Analyzer turned on ----");

                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                var diagnostics = await GetDiagnosticsAsync(diagsListener, mainUri);
            }

            // open the main document with analyzer turned off
            {
                string bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
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

                Trace.WriteLine("---- Analyzer turned off ----");

                var diagnostics = await GetDiagnosticsAsync(diagsListener, mainUri);
            }
        }

        [TestMethod]
        public async Task CollectPerfData_SmallTemplate()
        {
            var (diagsListener, testOutputPath) = GetTestConfig();
            var bicepFile = FileHelper.SaveResultFile(this.TestContext, "main.bicep", DataSets.IntermediaryVariables_LF.Bicep, testOutputPath);
            var bicepFileContents = File.ReadAllText(bicepFile);
            var mainUri = SaveFile("main.bicep", bicepFileContents, testOutputPath);
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

            // open the main document with analyzer turned on
            {
                Trace.WriteLine("---- Analyzer turned on ----");

                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                var diagnostics = await GetDiagnosticsAsync(diagsListener, mainUri);
            }

            // open the main document with analyzer turned off
            {
                string bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
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

                Trace.WriteLine("---- Analyzer turned off ----");

                var diagnostics = await GetDiagnosticsAsync(diagsListener, mainUri);
            }
        }

        [TestMethod]
        public async Task CollectPerfData_SingleParameter()
        {
            var (diagsListener, testOutputPath) = GetTestConfig();
            var bicepFile = FileHelper.SaveResultFile(this.TestContext, "main.bicep", DataSets.InvalidTargetScopes_LF.Bicep, testOutputPath);
            var bicepFileContents = File.ReadAllText(bicepFile);
            var mainUri = SaveFile("main.bicep", bicepFileContents, testOutputPath);
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

            // open the main document with analyzer turned on
            {
                Trace.WriteLine("---- Analyzer turned on ----");

                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                var diagnostics = await GetDiagnosticsAsync(diagsListener, mainUri);
            }

            // open the main document with analyzer turned off
            {
                string bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
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

                Trace.WriteLine("---- Analyzer turned off ----");

                var diagnostics = await GetDiagnosticsAsync(diagsListener, mainUri);
            }
        }

        [TestMethod]
        public async Task CollectPerfData_SingleVar()
        {
            var (diagsListener, testOutputPath) = GetTestConfig();
            var bicepFile = FileHelper.SaveResultFile(this.TestContext, "main.bicep", DataSets.InvalidParameters_LF.Bicep, testOutputPath);
            var bicepFileContents = File.ReadAllText(bicepFile);
            var mainUri = SaveFile("main.bicep", bicepFileContents, testOutputPath);
            using var helper = await StartServerWithClientConnectionAsync(diagsListener);
            var client = helper.Client;

            // open the main document with analyzer turned on
            {
                Trace.WriteLine("---- Analyzer turned on ----");

                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, bicepFileContents, 1));

                var diagnostics = await GetDiagnosticsAsync(diagsListener, mainUri);
            }

            // open the main document with analyzer turned off
            {
                string bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
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

                Trace.WriteLine("---- Analyzer turned off ----");

                var diagnostics = await GetDiagnosticsAsync(diagsListener, mainUri);
            }
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

        private static async Task<IEnumerable<Diagnostic>> GetDiagnosticsAsync(MultipleMessageListener<PublishDiagnosticsParams> diagsListener, DocumentUri documentUri)
        {
            var diagsParams = await diagsListener.WaitNext();
            diagsParams.Uri.Should().Be(documentUri);
            return diagsParams.Diagnostics;
        }
    }
}
