// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.JsonRpc.Server;
using LspRange = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests;

[TestClass]
public class RenameSymbolTests : TestBase
{
    private static readonly SharedLanguageHelperManager DefaultServer = new();
    private static readonly SharedLanguageHelperManager OrchestrationServer = new();

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        DefaultServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext));
        OrchestrationServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(
            testContext,
            services => services.WithFeatureOverrides(new(testContext, OrchestrationEnabled: true))));
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        await DefaultServer.DisposeAsync();
        await OrchestrationServer.DisposeAsync();
    }

    [TestMethod]
    public async Task Renaming_grouped_stack_updates_sibling_and_qualified_references()
    {
        var (contents, cursor) = ParserHelper.GetFileWithSingleCursor("""
            targetScope = 'orchestrator'
            group apps = {
              stack backend './stack.bicepparam' = {}
              stack frontend './stack.bicepparam' = { requires: [backend] }
            }
            stack global './stack.bicepparam' = { requires: [apps::ba|ckend] }
            """);
        var file = await new ServerRequestHelper(TestContext, OrchestrationServer).OpenFile(contents);

        var edit = await file.RequestRename(cursor, "service");
        var updated = file.ApplyWorkspaceEdit(edit).Text;

        updated.Should().EqualIgnoringTrailingWhitespace("""
            targetScope = 'orchestrator'
            group apps = {
              stack service './stack.bicepparam' = {}
              stack frontend './stack.bicepparam' = { requires: [service] }
            }
            stack global './stack.bicepparam' = { requires: [apps::service] }
            """);
    }

    [TestMethod]
    [DataRow("""
var te|st = 'asdf'

var asdf = test
var blah = '${test}'
""", """
var NewIdentifier = 'asdf'

var asdf = NewIdentifier
var blah = '${NewIdentifier}'
""")]
    public async Task RenamingIdentifierAccessOrDeclarationShouldRenameDeclarationAndAllReferences(string inputWithCursor, string expectedOutput)
    {
        var (contents, cursor) = ParserHelper.GetFileWithSingleCursor(inputWithCursor);
        var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(contents);

        var edit = await file.RequestRename(cursor, "NewIdentifier");

        var appliedEdit = file.ApplyWorkspaceEdit(edit).Text;

        appliedEdit.Should().EqualIgnoringTrailingWhitespace(expectedOutput);
    }

    [TestMethod]
    [DataRow("""
var test = 'asdf'
|
var asdf = test
var blah = '${test}'
""")]
    public async Task Rename_WithInvalidLocation_ThrowsRequestFailed(string inputWithCursor)
    {
        var (contents, cursor) = ParserHelper.GetFileWithSingleCursor(inputWithCursor);
        var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(contents);

        Func<Task> request = async () => await file.RequestRename(cursor, "NewIdentifier");

        var exception = await request.Should().ThrowAsync<JsonRpcException>()
            .WithMessage("The selected location does not contain a renameable Bicep symbol.");
        exception.Which.Error.Should().BeEmpty();
    }

    [TestMethod]
    [DataRow("""
var te|st = 'asdf'

var asdf = test
var blah = '${test}'
""")]
    public async Task PrepareRename_WithRenameableSymbol_ReturnsIdentifierRange(string inputWithCursor)
    {
        var (contents, cursor) = ParserHelper.GetFileWithSingleCursor(inputWithCursor);
        var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(contents);

        var result = await file.RequestPrepareRename(cursor);

        result.Should().NotBeNull();
        result!.IsPlaceholderRange.Should().BeTrue();
        result.PlaceholderRange.Should().NotBeNull();
        result.PlaceholderRange!.Range.Should().Be(new LspRange(0, 4, 0, 8));
        result.PlaceholderRange!.Placeholder.Should().Be("test");
    }

    [TestMethod]
    [DataRow("""
var test = 'asdf'
|
var asdf = test
var blah = '${test}'
""")]
    public async Task PrepareRename_WithInvalidLocation_ThrowsRequestFailed(string inputWithCursor)
    {
        var (contents, cursor) = ParserHelper.GetFileWithSingleCursor(inputWithCursor);
        var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(contents);

        Func<Task> request = async () => await file.RequestPrepareRename(cursor);

        var exception = await request.Should().ThrowAsync<JsonRpcException>()
            .WithMessage("The selected location does not contain a renameable Bicep symbol.");
        exception.Which.Error.Should().BeEmpty();
    }
}
