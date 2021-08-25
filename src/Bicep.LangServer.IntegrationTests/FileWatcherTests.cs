// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.FileSystem;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.Analyzers.Linter.Rules;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class FileWatcherTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static void SendDidChangeWatchedFiles(ILanguageClient client, params (DocumentUri documentUri, FileChangeType changeType)[] changes)
        {
            var fileChanges = new Container<FileEvent>(changes.Select(x => new FileEvent
            {
                Type = x.changeType,
                Uri = x.documentUri,
            }));

            client.Workspace.DidChangeWatchedFiles(new DidChangeWatchedFilesParams
            {
                Changes = fileChanges,
            });
        }

        [TestMethod]
        public async Task Module_file_change_should_trigger_a_recompilation()
        {
            var fileSystemDict = new Dictionary<Uri, string>();
            var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                this.TestContext,
                options =>
                {
                    options.OnPublishDiagnostics(diags => diagsListener.AddMessage(diags));
                },
                creationOptions: new LanguageServer.Server.CreationOptions(FileResolver: new InMemoryFileResolver(fileSystemDict)));

            var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
            fileSystemDict[mainUri.ToUri()] = @"
module myMod '../toOther/module.bicep' = {
  name: 'myMod'
  params: {
    requiredInput: 'hello!'
  }
}
";

            var moduleUri = DocumentUri.FromFileSystemPath("/path/toOther/module.bicep");
            fileSystemDict[moduleUri.ToUri()] = @"
// mis-spelling!
param requiredIpnut string
";

            // open the main document
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

                var diagsParams = await diagsListener.WaitNext();
                diagsParams.Uri.Should().Be(mainUri);
                diagsParams.Diagnostics.Should().Contain(x => x.Message.Contains("The specified \"object\" declaration is missing the following required properties: \"requiredIpnut\"."));
            }

            // open the module document. this should trigger diagnostics for both the module and the main doc which references it.
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(moduleUri, fileSystemDict[moduleUri.ToUri()], 1));

                var diagsParams = await diagsListener.WaitNext();
                diagsParams.Uri.Should().Be(moduleUri);
                // note that linter diagnostics do a fix up on the "Code" so check the expected documentation Uri
                diagsParams.Diagnostics.Should().Contain(x => x.CodeDescription!.Href == new NoUnusedParametersRule().Uri);

                diagsParams = await diagsListener.WaitNext();
                diagsParams.Uri.Should().Be(mainUri);
                diagsParams.Diagnostics.Should().Contain(x => x.Message.Contains("The specified \"object\" declaration is missing the following required properties: \"requiredIpnut\"."));
            }

            // fix the mis-spelling in the module! We should again get two sets of diagnostics, but with no errors
            {
                var updatedModuleContents = @"
// mis-spelling fixed!
param requiredInput string
";
                client.TextDocument.DidChangeTextDocument(TextDocumentParamHelper.CreateDidChangeTextDocumentParams(moduleUri, updatedModuleContents, 2));

                var diagsParams = await diagsListener.WaitNext();
                diagsParams.Uri.Should().Be(moduleUri);
                // note that linter diagnostics do a fix up on the "Code" so check the expected documentation Uri
                diagsParams.Diagnostics.Should().Contain(x => x.CodeDescription!.Href == new NoUnusedParametersRule().Uri);

                diagsParams = await diagsListener.WaitNext();
                diagsParams.Uri.Should().Be(mainUri);
                diagsParams.Diagnostics.Should().BeEmpty();
            }

            // close the module file. the language server should send empty diagnostics to clear the IDE state.
            {
                client.TextDocument.DidCloseTextDocument(TextDocumentParamHelper.CreateDidCloseTextDocumentParams(moduleUri, 2));

                var diagsParams = await diagsListener.WaitNext();
                diagsParams.Uri.Should().Be(moduleUri);
                diagsParams.Diagnostics.Should().BeEmpty();
            }
        }

        [TestMethod]
        public async Task Background_file_deletion_should_trigger_a_recompilation()
        {
            var fileSystemDict = new Dictionary<Uri, string>();
            var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                this.TestContext,
                options =>
                {
                    options.OnPublishDiagnostics(diags => diagsListener.AddMessage(diags));
                },
                creationOptions: new LanguageServer.Server.CreationOptions(FileResolver: new InMemoryFileResolver(fileSystemDict)));

            var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
            fileSystemDict[mainUri.ToUri()] = @"
module myMod '../toOther/module.bicep' = {
  name: 'myMod'
  params: {
    requiredInput: 'hello!'
  }
}
";

            var moduleUri = DocumentUri.FromFileSystemPath("/path/toOther/module.bicep");
            fileSystemDict[moduleUri.ToUri()] = @"
// mis-spelling!
param requiredIpnut string
";

            // open the main document
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

                var diagsParams = await diagsListener.WaitNext();
                diagsParams.Uri.Should().Be(mainUri);
                diagsParams.Diagnostics.Should().Contain(x => x.Message.Contains("The specified \"object\" declaration is missing the following required properties: \"requiredIpnut\"."));
            }

            // delete the module file with a background process
            {
                fileSystemDict.Remove(moduleUri.ToUri());
                SendDidChangeWatchedFiles(client, (moduleUri, FileChangeType.Deleted));

                var nextDiags = await diagsListener.WaitNext();
                nextDiags.Uri.Should().Be(mainUri);
                nextDiags.Diagnostics.Should().Contain(x => x.Message.Contains("An error occurred reading file. Could not find file \"/path/toOther/module.bicep\""));
            }

            // delete the main file with a background process. this should be ignored, as the close document event should clean it up
            {
                fileSystemDict.Remove(mainUri.ToUri());
                SendDidChangeWatchedFiles(client, (mainUri, FileChangeType.Deleted));

                await diagsListener.EnsureNoMessageSent();
            }

            // close the main file. the language server should send empty diagnostics to clear the IDE state.
            {
                client.TextDocument.DidCloseTextDocument(TextDocumentParamHelper.CreateDidCloseTextDocumentParams(mainUri, 1));

                var diagsParams = await diagsListener.WaitNext();
                diagsParams.Uri.Should().Be(mainUri);
                diagsParams.Diagnostics.Should().BeEmpty();
            }
        }


        [TestMethod]
        public async Task Background_folder_deletion_should_trigger_a_recompilation()
        {
            var fileSystemDict = new Dictionary<Uri, string>();
            var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
            var client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                this.TestContext,
                options =>
                {
                    options.OnPublishDiagnostics(diags => diagsListener.AddMessage(diags));
                },
                creationOptions: new LanguageServer.Server.CreationOptions(FileResolver: new InMemoryFileResolver(fileSystemDict)));

            var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
            fileSystemDict[mainUri.ToUri()] = @"
module myMod '../toOther/module.bicep' = {
  name: 'myMod'
  params: {
    requiredInput: 'hello!'
  }
}
";

            var moduleUri = DocumentUri.FromFileSystemPath("/path/toOther/module.bicep");
            fileSystemDict[moduleUri.ToUri()] = @"
// mis-spelling!
param requiredIpnut string
";

            // open the main document
            {
                client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUri()], 1));

                var diagsParams = await diagsListener.WaitNext();
                diagsParams.Uri.Should().Be(mainUri);
                diagsParams.Diagnostics.Should().Contain(x => x.Message.Contains("The specified \"object\" declaration is missing the following required properties: \"requiredIpnut\"."));
            }

            // delete the module folder with a background process
            {
                fileSystemDict.Remove(moduleUri.ToUri());
                SendDidChangeWatchedFiles(client, (DocumentUri.FromFileSystemPath("/path/toOther"), FileChangeType.Deleted));

                var nextDiags = await diagsListener.WaitNext();
                nextDiags.Uri.Should().Be(mainUri);
                nextDiags.Diagnostics.Should().Contain(x => x.Message.Contains("An error occurred reading file. Could not find file \"/path/toOther/module.bicep\""));
            }
        }
    }
}
