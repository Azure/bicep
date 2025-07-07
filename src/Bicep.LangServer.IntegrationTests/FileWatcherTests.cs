// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Extensions;
using Bicep.TextFixtures.IO;
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

    private static void SendDidChangeWatchedFiles(ILanguageClient client, params (IOUri fileUri, FileChangeType changeType)[] changes)
    {
        var fileChanges = new Container<FileEvent>(changes.Select(x => new FileEvent
        {
            Type = x.changeType,
            Uri = x.fileUri.ToDocumentUri(),
        }));

        client.Workspace.DidChangeWatchedFiles(new DidChangeWatchedFilesParams
        {
            Changes = fileChanges,
        });
    }

    [TestMethod]
    public async Task Module_file_change_should_trigger_a_recompilation()
    {
        var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
        var mainContent = """
            module myMod '../toOther/module.bicep' = {
              name: 'myMod'
              params: {
                requiredInput: 'hello!'
              }
            }
            """;
        var moduleContent = """
            // mis-spelling!
            param requiredIpnut string
            """;
        var fileSet = InMemoryTestFileSet.Create(
            ("main.bicep", mainContent),
            ("../toOther/module.bicep", moduleContent));
        var mainUri = fileSet.GetUri("main.bicep").ToDocumentUri();
        var moduleUri = fileSet.GetUri("../toOther/module.bicep").ToDocumentUri();

        using var helper = await LanguageServerHelper.StartServer(
            this.TestContext,
            options => options.OnPublishDiagnostics(diagsListener.AddMessage),
            services => services.WithFileExplorer(fileSet.FileExplorer));
        var client = helper.Client;

        // open the main document
        {
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri, mainContent, 1));

            var diagsParams = await diagsListener.WaitNext();
            diagsParams.Uri.Should().Be(mainUri);
            diagsParams.Diagnostics.Should().Contain(x => x.Message.Contains("The specified \"object\" declaration is missing the following required properties: \"requiredIpnut\"."));
        }

        // open the module document. this should trigger diagnostics for both the module and the main doc which references it.
        {
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(moduleUri, moduleContent, 1));

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
        var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();
        var mainContent = """
            module myMod '../toOther/module.bicep' = {
              name: 'myMod'
              params: {
                requiredInput: 'hello!'
              }
            }
            """;

        var fileSet = new InMemoryTestFileSet().AddFiles(
            ("main.bicep", mainContent),
            ("../toOther/module.bicep", """
                // mis-spelling!
                param requiredIpnut string
                """));

        var mainUri = fileSet.GetUri("main.bicep");
        var moduleUri = fileSet.GetUri("../toOther/module.bicep");

        //var fileResolver = new InMemoryFileResolver(fileSystemDict);
        using var helper = await LanguageServerHelper.StartServer(
            this.TestContext,
            options => options.OnPublishDiagnostics(diagsListener.AddMessage),
            services => services.WithFileExplorer(fileSet.FileExplorer));
        var client = helper.Client;

        // open the main document
        {
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri.ToString(), mainContent, 1));

            var diagsParams = await diagsListener.WaitNext();
            diagsParams.Uri.Should().Be(DocumentUri.From(mainUri.ToString()));
            diagsParams.Diagnostics.Should().Contain(x => x.Message.Contains("The specified \"object\" declaration is missing the following required properties: \"requiredIpnut\"."));
        }

        // delete the module file with a background process
        {
            fileSet.RemoveFile(moduleUri);
            SendDidChangeWatchedFiles(client, (moduleUri, FileChangeType.Deleted));

            var nextDiags = await diagsListener.WaitNext();

            // DocumentUri lowercases the drive letter, causing a mismatch with the IOUri used by the language server, which is annoying.
            // We need to convert the DocumentUri to a string and then back to a DocumentUri to get the expected format.
            nextDiags.Diagnostics.Should().HaveCount(1);
            nextDiags.Diagnostics.Single().Message.Should().Be($"An error occurred reading file. File '{moduleUri}' does not exist.");
        }

        // delete the main file with a background process. this should be ignored, as the close document event should clean it up
        {
            fileSet.RemoveFile(mainUri);
            SendDidChangeWatchedFiles(client, (mainUri, FileChangeType.Deleted));

            await diagsListener.EnsureNoMessageSent();
        }

        // close the main file. the language server should send empty diagnostics to clear the IDE state.
        {
            client.TextDocument.DidCloseTextDocument(TextDocumentParamHelper.CreateDidCloseTextDocumentParams(mainUri.ToDocumentUri(), 1));

            var diagsParams = await diagsListener.WaitNext();
            diagsParams.Uri.Should().Be(mainUri.ToDocumentUri());
            diagsParams.Diagnostics.Should().BeEmpty();
        }
    }


    [TestMethod]
    public async Task Background_folder_deletion_should_trigger_a_recompilation()
    {
        var fileSystemDict = new Dictionary<Uri, string>();
        var diagsListener = new MultipleMessageListener<PublishDiagnosticsParams>();

        var mainContent = """
            module myMod '../toOther/module.bicep' = {
              name: 'myMod'
              params: {
                requiredInput: 'hello!'
              }
            }
            """;

        var fileSet = new InMemoryTestFileSet().AddFiles(
            ("main.bicep", mainContent),
            ("../toOther/module.bicep", """
                // mis-spelling!
                param requiredIpnut string
                """));

        var mainUri = fileSet.GetUri("main.bicep");
        var moduleUri = fileSet.GetUri("../toOther/module.bicep");

        using var helper = await LanguageServerHelper.StartServer(
            this.TestContext,
            options => options.OnPublishDiagnostics(diagsListener.AddMessage),
            services => services.WithFileExplorer(fileSet.FileExplorer));
        var client = helper.Client;

        // open the main document
        {
            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(mainUri.ToDocumentUri(), mainContent, 1));

            var diagsParams = await diagsListener.WaitNext();
            diagsParams.Uri.Should().Be(mainUri.ToDocumentUri());
            diagsParams.Diagnostics.Should().Contain(x => x.Message.Contains("The specified \"object\" declaration is missing the following required properties: \"requiredIpnut\"."));
        }

        // delete the module folder with a background process
        {
            fileSet.RemoveFile(moduleUri);
            SendDidChangeWatchedFiles(client, (moduleUri, FileChangeType.Deleted));

            var nextDiags = await diagsListener.WaitNext();
            nextDiags.Uri.Should().Be(mainUri.ToDocumentUri());
            nextDiags.Diagnostics.Should().Contain(x => x.Message.Contains($"An error occurred reading file. File '{moduleUri}' does not exist."));
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
        ResultWithDiagnosticBuilder<BinaryData> result = new(BinaryData.FromBytes(Encoding.UTF8.GetBytes("abc")));

        var textFileData = BinaryData.FromBytes(Encoding.UTF8.GetBytes("abc"));
        var textFileHandleMock = StrictMock.Of<IFileHandle>();
        textFileHandleMock.Setup(x => x.Uri).Returns(new IOUri("file", "", "/path/to/bar.txt"));
        textFileHandleMock.Setup(x => x.OpenRead()).Returns(textFileData.ToStream()).Verifiable();

        var nonExistingDirectoryMock = StrictMock.Of<IDirectoryHandle>();
        nonExistingDirectoryMock.Setup(x => x.Exists()).Returns(false);

        var directoryHandleMock = StrictMock.Of<IDirectoryHandle>();
        directoryHandleMock.Setup(x => x.Uri).Returns(new IOUri("file", "", "/"));
        directoryHandleMock.Setup(x => x.GetDirectory(It.IsAny<string>())).Returns(nonExistingDirectoryMock.Object);
        directoryHandleMock.Setup(x => x.GetFile(It.IsAny<string>())).Returns(textFileHandleMock.Object).Verifiable();

        var bicepFileHandleMock = StrictMock.Of<IFileHandle>();
        bicepFileHandleMock.Setup(x => x.Uri).Returns(new IOUri("file", "", "/main.bicep"));
        bicepFileHandleMock.Setup(x => x.GetParent()).Returns(directoryHandleMock.Object);

        var fileExplorerMock = StrictMock.Of<IFileExplorer>();
        fileExplorerMock.Setup(x => x.GetFile(It.IsAny<IOUri>()))
            .Returns(bicepFileHandleMock.Object);

        using var helper = await MultiFileLanguageServerHelper.StartLanguageServer(
            TestContext,
            services => services
                .WithFileExplorer(fileExplorerMock.Object)
                .WithConfigurationManager(BicepTestConstants.BuiltInOnlyConfigurationManager));

        var version = 0;
        // The txt file is loaded when the bicep file is opened
        var diags = await helper.OpenFileOnceAsync(TestContext, bicepContents, bicepUri);
        diags.Diagnostics.Should().BeEmpty();
        textFileHandleMock.Verify(x => x.OpenRead(), Times.Once);

        // Change the bicep file, verify the txt file is not reloaded
        diags = await helper.ChangeFileAsync(TestContext, bicepContents, bicepUri, ++version);
        diags.Diagnostics.Should().BeEmpty();
        textFileHandleMock.Verify(x => x.OpenRead(), Times.Once);

        // Change the txt file, verify diagnostics are re-published for the bicep file
        textFileData = BinaryData.FromBytes(Encoding.UTF8.GetBytes("def"));
        textFileHandleMock.Setup(x => x.OpenRead()).Returns(textFileData.ToStream()).Verifiable();

        helper.ChangeWatchedFile(txtFileUri);
        diags = await helper.WaitForDiagnostics(bicepUri);
        diags.Diagnostics.Should().OnlyContain(x => x.Message.Contains("Expected a value of type \"'abc'\" but the provided value is of type \"'def'\"."));
        textFileHandleMock.Verify(x => x.OpenRead(), Times.Exactly(2));

        // Change the bicep file, verify the txt file is not reloaded
        diags = await helper.ChangeFileAsync(TestContext, bicepContents, bicepUri, ++version);
        diags.Diagnostics.Should().OnlyContain(x => x.Message.Contains("Expected a value of type \"'abc'\" but the provided value is of type \"'def'\"."));
        textFileHandleMock.Verify(x => x.OpenRead(), Times.Exactly(2));

        // Simulate a failure loading the txt file, verify diagnostics are re-published for the bicep file
        textFileHandleMock.Setup(x => x.OpenRead()).Throws(new IOException("Oops")).Verifiable();

        helper.ChangeWatchedFile(txtFileUri);
        diags = await helper.WaitForDiagnostics(bicepUri);
        diags.Diagnostics.Should().OnlyContain(x => x.Message.Contains("Oops"));
        textFileHandleMock.Verify(x => x.OpenRead(), Times.Exactly(3));

        // Change the bicep file, verify the txt file is not reloaded, even when there was a failure loading the file
        diags = await helper.ChangeFileAsync(TestContext, bicepContents, bicepUri, ++version);
        diags.Diagnostics.Should().OnlyContain(x => x.Message.Contains("Oops"));
        textFileHandleMock.Verify(x => x.OpenRead(), Times.Exactly(3));

        // Delete a parent folder, verify it triggers a recompilation
        var parentDirUri = new Uri("file:///path/to");
        textFileHandleMock.Setup(x => x.OpenRead()).Throws(new IOException("Parent directory is gone!")).Verifiable();

        helper.ChangeWatchedFile(parentDirUri, FileChangeType.Deleted);

        diags = await helper.ChangeFileAsync(TestContext, bicepContents, bicepUri, ++version);
        diags.Diagnostics.Should().OnlyContain(x => x.Message.Contains("Parent directory is gone!"));
        textFileHandleMock.Verify(x => x.OpenRead(), Times.Exactly(4));

        // Change an unrelated txt file, verify it doesn't trigger a recompilation
        var unrelatedTxtFileUri = new Uri("file:///path/to/notbar.txt");
        helper.ChangeWatchedFile(unrelatedTxtFileUri);
        diags = await helper.ChangeFileAsync(TestContext, bicepContents, bicepUri, ++version);
        diags.Diagnostics.Should().OnlyContain(x => x.Message.Contains("An error occurred reading file. Parent directory is gone!"));
        textFileHandleMock.Verify(x => x.OpenRead(), Times.Exactly(4));
        directoryHandleMock.Verify(x => x.GetFile("path/to/notbar.txt"), Times.Never);
    }
}
