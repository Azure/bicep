// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LangServer.IntegrationTests;

[TestClass]
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

        var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
        fileSystemDict[mainUri.ToUriEncoded()] = @"
module myMod '../toOther/module.bicep' = {
  name: 'myMod'
  params: {
    requiredInput: 'hello!'
  }
}
";

        var moduleUri = DocumentUri.FromFileSystemPath("/path/toOther/module.bicep");
        fileSystemDict[moduleUri.ToUriEncoded()] = @"
// mis-spelling!
param requiredIpnut string
";
        using var helper = await LanguageServerHelper.StartServer(
            this.TestContext,
            options => options.OnPublishDiagnostics(diagsListener.AddMessage),
            services => services.WithFileResolver(new InMemoryFileResolver(fileSystemDict)));
        var client = helper.Client;

        // open the main document
        {
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUriEncoded()], 1));

            var diagsParams = await diagsListener.WaitNext();
            diagsParams.Uri.Should().Be(mainUri);
            diagsParams.Diagnostics.Should().Contain(x => x.Message.Contains("The specified \"object\" declaration is missing the following required properties: \"requiredIpnut\"."));
        }

        // open the module document. this should trigger diagnostics for both the module and the main doc which references it.
        {
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(moduleUri, fileSystemDict[moduleUri.ToUriEncoded()], 1));

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

        var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
        fileSystemDict[mainUri.ToUriEncoded()] = @"
module myMod '../toOther/module.bicep' = {
  name: 'myMod'
  params: {
    requiredInput: 'hello!'
  }
}
";

        var moduleUri = DocumentUri.FromFileSystemPath("/path/toOther/module.bicep");
        fileSystemDict[moduleUri.ToUriEncoded()] = @"
// mis-spelling!
param requiredIpnut string
";

        var fileResolver = new InMemoryFileResolver(fileSystemDict);
        using var helper = await LanguageServerHelper.StartServer(
            this.TestContext,
            options => options.OnPublishDiagnostics(diagsListener.AddMessage),
            services => services.WithFileResolver(fileResolver));
        var client = helper.Client;

        // open the main document
        {
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUriEncoded()], 1));

            var diagsParams = await diagsListener.WaitNext();
            diagsParams.Uri.Should().Be(mainUri);
            diagsParams.Diagnostics.Should().Contain(x => x.Message.Contains("The specified \"object\" declaration is missing the following required properties: \"requiredIpnut\"."));
        }

        // delete the module file with a background process
        {
            fileResolver.MockFileSystem.File.Delete(moduleUri.GetFileSystemPath());
            SendDidChangeWatchedFiles(client, (moduleUri, FileChangeType.Deleted));

            var nextDiags = await diagsListener.WaitNext();
            nextDiags.Uri.Should().Be(mainUri);
            nextDiags.Diagnostics.Should().Contain(x => x.Message.Contains("An error occurred reading file. Could not find file '/path/toOther/module.bicep'."));
        }

        // delete the main file with a background process. this should be ignored, as the close document event should clean it up
        {
            fileResolver.MockFileSystem.File.Delete(moduleUri.GetFileSystemPath());
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

        var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
        fileSystemDict[mainUri.ToUriEncoded()] = @"
module myMod '../toOther/module.bicep' = {
  name: 'myMod'
  params: {
    requiredInput: 'hello!'
  }
}
";

        var moduleUri = DocumentUri.FromFileSystemPath("/path/toOther/module.bicep");
        fileSystemDict[moduleUri.ToUriEncoded()] = @"
// mis-spelling!
param requiredIpnut string
";

        var fileResolver = new InMemoryFileResolver(fileSystemDict);
        using var helper = await LanguageServerHelper.StartServer(
            this.TestContext,
            options => options.OnPublishDiagnostics(diagsListener.AddMessage),
            services => services.WithFileResolver(fileResolver));
        var client = helper.Client;

        // open the main document
        {
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, fileSystemDict[mainUri.ToUriEncoded()], 1));

            var diagsParams = await diagsListener.WaitNext();
            diagsParams.Uri.Should().Be(mainUri);
            diagsParams.Diagnostics.Should().Contain(x => x.Message.Contains("The specified \"object\" declaration is missing the following required properties: \"requiredIpnut\"."));
        }

        // delete the module folder with a background process
        {
            fileResolver.MockFileSystem.File.Delete(moduleUri.GetFileSystemPath());
            SendDidChangeWatchedFiles(client, (DocumentUri.FromFileSystemPath("/path/toOther"), FileChangeType.Deleted));

            var nextDiags = await diagsListener.WaitNext();
            nextDiags.Uri.Should().Be(mainUri);
            nextDiags.Diagnostics.Should().Contain(x => x.Message.Contains("An error occurred reading file. Could not find file '/path/toOther/module.bicep'."));
        }
    }

    [TestMethod]
    public async Task Auxiliary_file_access_is_cached()
    {
        var bicepUri = new Uri("file:///main.bicep");
        var bicepContents = """
        #disable-next-line no-unused-params
        param foo 'abc' = loadTextContent('path/to/bar.txt')
        """;
        var txtFileUri = new Uri("file:///path/to/bar.txt");
        ResultWithDiagnostic<BinaryData> result = new(BinaryData.FromBytes(Encoding.UTF8.GetBytes("abc")));

        var fileResolverMock = StrictMock.Of<IFileResolver>();
        fileResolverMock.Setup(x => x.TryReadAsBinaryData(txtFileUri, It.IsAny<int?>()))
            .Returns<Uri, int?>((_, _) => result);

        using var helper = await MultiFileLanguageServerHelper.StartLanguageServer(
            TestContext,
            services => services.WithFileResolver(fileResolverMock.Object));

        var version = 0;
        // The txt file is loaded when the bicep file is opened
        var diags = await helper.OpenFileOnceAsync(TestContext, bicepContents, bicepUri);
        diags.Diagnostics.Should().BeEmpty();
        fileResolverMock.Verify(x => x.TryReadAsBinaryData(txtFileUri, null), Times.Once);

        // Change the bicep file, verify the txt file is not reloaded
        diags = await helper.ChangeFileAsync(TestContext, bicepContents, bicepUri, ++version);
        diags.Diagnostics.Should().BeEmpty();
        fileResolverMock.Verify(x => x.TryReadAsBinaryData(txtFileUri, null), Times.Once);

        // Change the txt file, verify diagnostics are re-published for the bicep file
        result = new(BinaryData.FromBytes(Encoding.UTF8.GetBytes("def")));
        helper.ChangeWatchedFile(txtFileUri);
        diags = await helper.WaitForDiagnostics(bicepUri);
        diags.Diagnostics.Should().OnlyContain(x => x.Message.Contains("Expected a value of type \"'abc'\" but the provided value is of type \"'def'\"."));
        fileResolverMock.Verify(x => x.TryReadAsBinaryData(txtFileUri, null), Times.Exactly(2));

        // Change the bicep file, verify the txt file is not reloaded
        diags = await helper.ChangeFileAsync(TestContext, bicepContents, bicepUri, ++version);
        diags.Diagnostics.Should().OnlyContain(x => x.Message.Contains("Expected a value of type \"'abc'\" but the provided value is of type \"'def'\"."));
        fileResolverMock.Verify(x => x.TryReadAsBinaryData(txtFileUri, null), Times.Exactly(2));

        // Simulate a failure loading the txt file, verify diagnostics are re-published for the bicep file
        result = new(x => x.FilePathIsEmpty());
        helper.ChangeWatchedFile(txtFileUri);
        diags = await helper.WaitForDiagnostics(bicepUri);
        diags.Diagnostics.Should().OnlyContain(x => x.Message.Contains("The specified path is empty."));
        fileResolverMock.Verify(x => x.TryReadAsBinaryData(txtFileUri, null), Times.Exactly(3));

        // Change the bicep file, verify the txt file is not reloaded, even when there was a failure loading the file
        diags = await helper.ChangeFileAsync(TestContext, bicepContents, bicepUri, ++version);
        diags.Diagnostics.Should().OnlyContain(x => x.Message.Contains("The specified path is empty."));
        fileResolverMock.Verify(x => x.TryReadAsBinaryData(txtFileUri, null), Times.Exactly(3));

        // Delete a parent folder, verify it triggers a recompilation
        var parentDirUri = new Uri("file:///path/to");
        result = new(x => x.ErrorOccurredReadingFile("Parent directory is gone!"));
        helper.ChangeWatchedFile(parentDirUri, FileChangeType.Deleted);
        diags = await helper.ChangeFileAsync(TestContext, bicepContents, bicepUri, ++version);
        diags.Diagnostics.Should().OnlyContain(x => x.Message.Contains("An error occurred reading file. Parent directory is gone!"));
        fileResolverMock.Verify(x => x.TryReadAsBinaryData(txtFileUri, null), Times.Exactly(4));

        // Change an unrelated txt file, verify it doesn't trigger a recompilation
        var unrelatedTxtFileUri = new Uri("file:///path/to/notbar.txt");
        helper.ChangeWatchedFile(unrelatedTxtFileUri);
        diags = await helper.ChangeFileAsync(TestContext, bicepContents, bicepUri, ++version);
        diags.Diagnostics.Should().OnlyContain(x => x.Message.Contains("An error occurred reading file. Parent directory is gone!"));
        fileResolverMock.Verify(x => x.TryReadAsBinaryData(txtFileUri, null), Times.Exactly(4));
        fileResolverMock.Verify(x => x.TryReadAsBinaryData(unrelatedTxtFileUri, null), Times.Never);
    }
}
