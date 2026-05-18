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
        var response = await ExtractToModuleAsync(@"
param namePrefix string

<<resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
  name: '${namePrefix}${uniqueString(resourceGroup().id)}'
  location: resourceGroup().location
}>>
");

        response.Should().NotBeNull();
        response.ModuleFileContents.Should().Contain("param namePrefix string");
        response.ModuleFileContents.Should().Contain("resource stg");
        response.ReplacementText.Should().Contain("module");
        response.RenamePosition.Should().NotBeNull();
    }

    [TestMethod]
    public async Task ExtractToModule_should_fail_for_partial_selection()
    {
        var response = await ExtractToModuleAsync(@"
param namePrefix string

resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
  <<name: '${namePrefix}${uniqueString(resourceGroup().id)}'>>
  location: resourceGroup().location
}
");

        AssertEmptyResponse(response);
    }

    [TestMethod]
    public async Task ExtractToModule_should_fail_for_empty_selection()
    {
        var response = await ExtractToModuleAsync(@"
param namePrefix string
|
resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
    name: '${namePrefix}${uniqueString(resourceGroup().id)}'
    location: resourceGroup().location
}
");

        AssertEmptyResponse(response);
    }

    [TestMethod]
    public async Task ExtractToModule_should_fail_for_variable_selection()
    {
        var response = await ExtractToModuleAsync(@"
<<var accountName = 'stg${uniqueString(resourceGroup().id)}'>>

resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
    name: accountName
    location: resourceGroup().location
}
");

        AssertEmptyResponse(response);
    }

    [TestMethod]
    public async Task ExtractToModule_should_fail_when_selection_contains_non_resource_declaration()
    {
        var response = await ExtractToModuleAsync(@"
<<resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
    name: 'stg${uniqueString(resourceGroup().id)}'
    location: resourceGroup().location
}

output storageId string = stg.id>>
");

        AssertEmptyResponse(response);
    }

    [TestMethod]
    public async Task ExtractToModule_should_fail_when_selected_resource_is_referenced_outside_selection()
    {
        var response = await ExtractToModuleAsync(@"
<<resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
    name: 'stg${uniqueString(resourceGroup().id)}'
    location: resourceGroup().location
}>>

output storageId string = stg.id
");

        AssertEmptyResponse(response);
    }

    [TestMethod]
    public async Task ExtractToModule_should_extract_multiple_resources_with_internal_references()
    {
        var response = await ExtractToModuleAsync(@"
<<resource plan 'Microsoft.Web/serverfarms@2022-03-01' = {
    name: 'plan${uniqueString(resourceGroup().id)}'
    location: resourceGroup().location
    sku: {
        name: 'F1'
        tier: 'Free'
    }
}

resource site 'Microsoft.Web/sites@2022-03-01' = {
    name: 'site${uniqueString(resourceGroup().id)}'
    location: resourceGroup().location
    properties: {
        serverFarmId: plan.id
    }
}>>
");

        response.ModuleFileContents.Should().Contain("resource plan");
        response.ModuleFileContents.Should().Contain("resource site");
        response.ModuleFileContents.Should().Contain("serverFarmId: plan.id");
        response.ReplacementText.Should().Be("module storage './storage.bicep' = {\n  name: 'storage'\n}\n");
        response.RenamePosition.Should().NotBeNull();
    }

    [TestMethod]
    public async Task ExtractToModule_should_create_params_for_external_param_and_variable_dependencies()
    {
        var response = await ExtractToModuleAsync(@"
param prefix string
param location string
var accountName = '${prefix}${uniqueString(resourceGroup().id)}'

<<resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
    name: accountName
    location: location
}>>
");

        response.ModuleFileContents.Should().Contain("param location string\nparam accountName string\n\nresource stg");
        response.ModuleFileContents.Should().NotContain("param prefix string");
        response.ReplacementText.Should().Be("module storage './storage.bicep' = {\n  name: 'storage'\n  params: {\n    location: location\n    accountName: accountName\n  }\n}\n");
    }

    [TestMethod]
    public async Task ExtractToModule_should_emit_each_external_dependency_once()
    {
        var response = await ExtractToModuleAsync(@"
param tags object

<<resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
    name: 'stg${uniqueString(resourceGroup().id)}'
    location: resourceGroup().location
    tags: tags
    properties: {
        supportsHttpsTrafficOnly: contains(keys(tags), 'secure')
    }
}>>
");

        CountOccurrences(response.ModuleFileContents, "param tags object").Should().Be(1);
        CountOccurrences(response.ReplacementText, "tags: tags").Should().Be(1);
    }

    [TestMethod]
    public async Task ExtractToModule_should_avoid_existing_declaration_name_for_module_symbol()
    {
        var response = await ExtractToModuleAsync(@"
var storage = 'alreadyUsed'

<<resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
    name: 'stg${uniqueString(resourceGroup().id)}'
    location: resourceGroup().location
}>>
");

        response.ReplacementText.Should().Be("module storage1 './storage.bicep' = {\n  name: 'storage1'\n}\n");
        response.RenamePosition.Should().NotBeNull();
    }

    [TestMethod]
    public async Task ExtractToModule_should_sanitize_module_symbol_name_and_preserve_nested_relative_path()
    {
        var response = await ExtractToModuleAsync(@"
<<resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
    name: 'stg${uniqueString(resourceGroup().id)}'
    location: resourceGroup().location
}>>
", outputPath => Path.Combine(outputPath, "modules", "web-app.bicep"));

        response.ReplacementText.Should().Be("module web_app './modules/web-app.bicep' = {\n  name: 'web_app'\n}\n");
        response.RenamePosition.Should().NotBeNull();
    }

    [TestMethod]
    public async Task ExtractToModule_should_fall_back_to_default_symbol_name_for_invalid_module_file_name()
    {
        var response = await ExtractToModuleAsync(@"
<<resource stg 'Microsoft.Storage/storageAccounts@2023-04-01' = {
    name: 'stg${uniqueString(resourceGroup().id)}'
    location: resourceGroup().location
}>>
", outputPath => Path.Combine(outputPath, "123-storage.bicep"));

        response.ReplacementText.Should().Be("module extractedModule './123-storage.bicep' = {\n  name: 'extractedModule'\n}\n");
        response.RenamePosition.Should().NotBeNull();
    }

    private async Task<ExtractToModuleResponse> ExtractToModuleAsync(string fileWithSelection, Func<string, string>? getModulePath = null)
    {
        var (fileText, selection) = ParserHelper.GetFileWithSingleSelection(fileWithSelection);

        var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var filePath = FileHelper.SaveResultFile(TestContext, "main.bicep", fileText, testOutputPath);
        var modulePath = getModulePath?.Invoke(testOutputPath) ?? Path.Combine(testOutputPath, "storage.bicep");
        var documentUri = DocumentUri.FromFileSystemPath(filePath);

        var lineStarts = TextCoordinateConverter.GetLineStarts(fileText);
        var range = PositionHelper.GetRange(lineStarts, selection.Position, selection.Position + selection.Length);

        using var helper = await LanguageServerHelper.StartServerWithText(TestContext, fileText, documentUri);

        return await helper.Client.SendRequest(new ExtractToModuleParams
        {
            TextDocument = documentUri,
            Range = range,
            ModuleFilePath = modulePath,
        }, default);
    }

    private static void AssertEmptyResponse(ExtractToModuleResponse response)
    {
        response.Should().NotBeNull();
        response.ModuleFileContents.Should().BeEmpty();
        response.ReplacementText.Should().BeEmpty();
        response.RenamePosition.Should().BeNull();
    }

    private static int CountOccurrences(string text, string value)
    {
        var count = 0;
        var index = 0;

        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }
}
