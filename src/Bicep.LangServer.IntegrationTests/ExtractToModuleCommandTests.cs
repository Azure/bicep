// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests;

[TestClass]
public class ExtractToModuleCommandTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public async Task ExtractToModule_should_return_module_contents_and_replacement()
    {
        var (fileText, selection) = ParserHelper.GetFileWithSingleSelection(@"""
param namePrefix string

<<resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
  name: '${namePrefix}${uniqueString(resourceGroup().id)}'
  location: resourceGroup().location
}>>
""");

        var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var filePath = FileHelper.SaveResultFile(TestContext, "main.bicep", fileText, testOutputPath);
        var modulePath = Path.Combine(testOutputPath, "storage.bicep");
        var documentUri = DocumentUri.FromFileSystemPath(filePath);

        var lineStarts = TextCoordinateConverter.GetLineStarts(fileText);
        var range = PositionHelper.GetRange(lineStarts, selection.Position, selection.Position + selection.Length);

        using var helper = await LanguageServerHelper.StartServerWithText(TestContext, fileText, documentUri);

        var response = await helper.Client.SendRequest(new ExtractToModuleParams
        {
            TextDocument = documentUri,
            Range = range,
            ModuleFilePath = modulePath,
        }, default);

        response.Should().NotBeNull();
        response.ModuleFileContents.Should().Contain("param namePrefix string");
        response.ModuleFileContents.Should().Contain("resource stg");
        response.ReplacementText.Should().Contain("module");
        response.RenamePosition.Should().NotBeNull();
    }

    [TestMethod]
    public async Task ExtractToModule_should_fail_for_partial_selection()
    {
        var (fileText, selection) = ParserHelper.GetFileWithSingleSelection(@"""
param namePrefix string

resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
  <<name: '${namePrefix}${uniqueString(resourceGroup().id)}'>>
  location: resourceGroup().location
}
""");

        var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var filePath = FileHelper.SaveResultFile(TestContext, "main.bicep", fileText, testOutputPath);
        var modulePath = Path.Combine(testOutputPath, "storage.bicep");
        var documentUri = DocumentUri.FromFileSystemPath(filePath);

        var lineStarts = TextCoordinateConverter.GetLineStarts(fileText);
        var range = PositionHelper.GetRange(lineStarts, selection.Position, selection.Position + selection.Length);

        using var helper = await LanguageServerHelper.StartServerWithText(TestContext, fileText, documentUri);

        var response = await helper.Client.SendRequest(new ExtractToModuleParams
        {
            TextDocument = documentUri,
            Range = range,
            ModuleFilePath = modulePath,
        }, default);

        response.ModuleFileContents.Should().BeEmpty();
        response.ReplacementText.Should().BeEmpty();
    }
}
