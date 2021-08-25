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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests
{
    // Search for bicepconfig.json in DiscoverLocalConfigurationFile(..) in ConfigHelper starts from current directory.
    // In the below tests, we'll explicitly set the current directory and disable running tests in parallel to avoid conflicts
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class BicepConfigTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }
        private readonly string CurrentDirectory = Directory.GetCurrentDirectory();

        [TestMethod]
        [DoNotParallelize]
        public async Task BicepConfigFileModification_ShouldRetriggerCompilation()
        {
            var fileSystemDict = new Dictionary<Uri, string>();
            var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                TestContext,
                options =>
                {
                    options.OnPublishDiagnostics(diags => diagsListener.AddMessage(diags));
                },
                fileResolver: new InMemoryFileResolver(fileSystemDict));

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

                var diagsParams = await diagsListener.WaitNext();
                diagsParams.Uri.Should().Be(mainUri);
                diagsParams.Diagnostics.Should().SatisfyRespectively(
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

        [TestCleanup]
        public void Cleanup()
        {
            Directory.SetCurrentDirectory(CurrentDirectory);
        }
    }
}
