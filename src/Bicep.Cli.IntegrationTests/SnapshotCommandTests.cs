// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Models;
using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis.Sarif;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class SnapshotCommandTests : TestBase
{
    [TestMethod]
    public async Task Snapshot_with_overwrite_should_generate_a_valid_snapshot_file()
    {
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var bicepparamPath = FileHelper.SaveResultFile(TestContext, "main.bicepparam", """
using './main.bicep'

param location = 'eastus'
""", testOutputPath: outputPath);

        FileHelper.SaveResultFile(TestContext, "main.bicep", """
@description('Storage Account type')
param storageAccountType string = 'Standard_LRS'

@description('The storage account location.')
param location string = resourceGroup().location

@description('The name of the storage account')
param storageAccountName string = 'store${uniqueString(resourceGroup().id)}'

resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'StorageV2'
  properties: {}
}
""", testOutputPath: outputPath);

        var outputFilePath = FileHelper.GetResultFilePath(TestContext, "main.snapshot.json", testOutputPath: outputPath);

        var subscriptionId = new Guid().ToString();
        var resourceGroupName = "myRg";

        { // overwrite
            var result = await Bicep("snapshot", bicepparamPath, "--mode", "overwrite", "--subscription-id", subscriptionId, "--resource-group", resourceGroupName);

            result.Should().Succeed();

            // this command should output an experimental warning
            result.Stderr.Should().Match("WARNING: The 'snapshot' CLI command group is an experimental feature.*");

            var snapshotContents = File.ReadAllText(outputFilePath).FromJson<JToken>();
            snapshotContents.Should().DeepEqual(JObject.Parse("""
{
  "predictedResources": [
    {
      "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/myRg/providers/Microsoft.Storage/storageAccounts/storejs26ofeqkqeve",
      "type": "Microsoft.Storage/storageAccounts",
      "name": "storejs26ofeqkqeve",
      "apiVersion": "2022-09-01",
      "location": "eastus",
      "sku": {
        "name": "Standard_LRS"
      },
      "kind": "StorageV2",
      "properties": {}
    }
  ],
  "diagnostics": []
}
"""));
        }

        { // validate
            FileHelper.SaveResultFile(TestContext, "main.bicepparam", """
using './main.bicep'

param location = 'westus'
param storageAccountType = 'Standard_ZRS'
""", testOutputPath: outputPath);

            var result = await Bicep("snapshot", bicepparamPath, "--mode", "validate", "--subscription-id", subscriptionId, "--resource-group", resourceGroupName);

            result.Should().Fail();
            // remove the color ascii control codes to make it easier to visualize
            ReplaceColorCodes(result.Stdout).Should().EqualIgnoringNewlines("""

Scope: /subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/myRg
[Purple]
  ~ Microsoft.Storage/storageAccounts/storejs26ofeqkqeve[Reset]
    [Purple]~[Reset] location[Reset]:[Reset] [Orange]"eastus"[Reset] => [Green]"westus"[Reset]
    [Purple]~[Reset] sku.name[Reset]:[Reset] [Orange]"Standard_LRS"[Reset] => [Green]"Standard_ZRS"[Reset]
[Purple][Reset]
""");
        }
    }

    private static string ReplaceColorCodes(string input)
    {
        var namesByColor = new Dictionary<Color, string>
        {
            [Color.Orange] = nameof(Color.Orange),
            [Color.Green] = nameof(Color.Green),
            [Color.Purple] = nameof(Color.Purple),
            [Color.Blue] = nameof(Color.Blue),
            [Color.Gray] = nameof(Color.Gray),
            [Color.Reset] = nameof(Color.Reset),
            [Color.Red] = nameof(Color.Red),
            [Color.DarkYellow] = nameof(Color.DarkYellow),
        };

        foreach (var (color, name) in namesByColor)
        {
            input = input.Replace(color.ToString(), $"[{name}]");
        }

        return input;
    }
}