// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.IntegrationTests;

[TestClass]
public class RenameSymbolTests : TestBase
{
    private static readonly SharedLanguageHelperManager DefaultServer = new();

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        DefaultServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext));
    }

    [ClassCleanup]
    public static async Task ClassCleanup()
    {
        await DefaultServer.DisposeAsync();
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
    public async Task RenamingFunctionsShouldProduceEmptyEdit(string inputWithCursor)
    {
        var (contents, cursor) = ParserHelper.GetFileWithSingleCursor(inputWithCursor);
        var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(contents);

        var edit = await file.RequestRename(cursor, "NewIdentifier");

        edit.Should().BeNull();
    }
}
