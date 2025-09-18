// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
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
            AnsiHelper.ReplaceCodes(result.Stdout).Should().EqualIgnoringNewlines("""

Scope: /subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/myRg
[Purple]
  ~ Microsoft.Storage/storageAccounts/storejs26ofeqkqeve[Reset]
    [Purple]~[Reset] location[Reset]:[Reset] [Orange]"eastus"[Reset] => [Green]"westus"[Reset]
    [Purple]~[Reset] sku.name[Reset]:[Reset] [Orange]"Standard_LRS"[Reset] => [Green]"Standard_ZRS"[Reset]
[Purple][Reset]
""");
        }
    }

    [TestMethod]
    [EmbeddedFilesTestData(@"Files/SnapshotCommandTests/.*/main\.bicepparam")]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    public async Task Snapshot_generates_correct_format(EmbeddedFile paramFile)
    {
        var subscriptionId = new Guid().ToString();
        var resourceGroupName = "myRg";

        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, paramFile);
        var snapshotFile = baselineFolder.GetFileOrEnsureCheckedIn("main.snapshot.json");

        var result = await Bicep("snapshot", baselineFolder.EntryFile.OutputFilePath, "--mode", "overwrite", "--subscription-id", subscriptionId, "--resource-group", resourceGroupName);
        result.Should().Succeed();

        snapshotFile.ShouldHaveExpectedJsonValue();
    }

    [TestMethod]
    public async Task Snapshot_command_injects_appropriate_metadata_for_scope()
    {
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var bicepparamPath = FileHelper.SaveResultFile(
            TestContext,
            "main.bicepparam",
            """
                using './main.bicep'

                param rgName = 'myRg'
                """,
            testOutputPath: outputPath);

        FileHelper.SaveResultFile(
            TestContext,
            "main.bicep",
            """
                targetScope = 'subscription'

                param rgName string

                resource rg 'Microsoft.Resources/resourceGroups@2024-11-01' = {
                  name: rgName
                  location: deployment().location
                }
                """,
            testOutputPath: outputPath);

        var outputFilePath = FileHelper.GetResultFilePath(TestContext, "main.snapshot.json", testOutputPath: outputPath);

        var subscriptionId = new Guid().ToString();

        var result = await Bicep("snapshot", bicepparamPath, "--mode", "overwrite", "--subscription-id", subscriptionId, "--location", "westeurope");

        result.Should().Succeed();

        var snapshotContents = File.ReadAllText(outputFilePath).FromJson<JToken>();
        snapshotContents.Should().DeepEqual(JObject.Parse("""
            {
              "predictedResources": [
                {
                  "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/myRg",
                  "type": "Microsoft.Resources/resourceGroups",
                  "name": "myRg",
                  "apiVersion": "2024-11-01",
                  "location": "westeurope"
                }
              ],
              "diagnostics": []
            }
            """));
    }

    [TestMethod]
    public async Task Snapshot_surfaces_validation_errors_as_message()
    {
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var bicepparamPath = FileHelper.SaveResultFile(
            TestContext,
            "main.bicepparam",
            """
                using './main.bicep'

                param condition = true
                """,
            testOutputPath: outputPath);

        FileHelper.SaveResultFile(
            TestContext,
            "main.bicep",
            """
                param condition bool

                var foo = condition ? fail('condition must be false') : 'foo'
                """,
            testOutputPath: outputPath);

        var outputFilePath = FileHelper.GetResultFilePath(TestContext, "main.snapshot.json", testOutputPath: outputPath);

        var subscriptionId = new Guid().ToString();

        var result = await Bicep("snapshot", bicepparamPath, "--mode", "overwrite");

        result.Should().Fail();
        result.Stderr.Should().Contain("""Template snapshotting could not be completed for the following reason: 'The template variable 'foo' is not valid: condition must be false.""");
        result.Stderr.Should().NotContain("Unhandled exception");
    }

    [TestMethod]
    public async Task Snapshot_speculatively_evaluates_references()
    {
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var bicepparamPath = FileHelper.SaveResultFile(
            TestContext,
            "main.bicepparam",
            """
                using './main.bicep'
                """,
            testOutputPath: outputPath);

        FileHelper.SaveResultFile(
            TestContext,
            "main.bicep",
            """
                module mod 'mod.bicep' = {}
                                
                module mod2 'mod2.bicep' = {
                  params: {
                    vnetName: mod.outputs.static
                  }
                }
                """,
            testOutputPath: outputPath);

        FileHelper.SaveResultFile(
            TestContext,
            "mod.bicep",
            """output static string = 'foo'""",
            testOutputPath: outputPath);

        FileHelper.SaveResultFile(
            TestContext,
            "mod2.bicep",
            """
                param vnetName string
                                
                resource vnet 'Microsoft.Network/virtualNetworks@2024-07-01' = {
                  name: vnetName
                }
                """,
            testOutputPath: outputPath);

        var outputFilePath = FileHelper.GetResultFilePath(TestContext, "main.snapshot.json", testOutputPath: outputPath);

        var subscriptionId = new Guid().ToString();

        var result = await Bicep("snapshot", bicepparamPath, "--mode", "overwrite", "--subscription-id", subscriptionId, "--resource-group", "myRg", "--deployment-name", "deployment");

        result.Should().Succeed();

        var snapshotContents = File.ReadAllText(outputFilePath).FromJson<JToken>();
        snapshotContents.Should().DeepEqual(JObject.Parse("""
            {
              "predictedResources": [
                {
                  "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/myRg/providers/Microsoft.Network/virtualNetworks/foo",
                  "type": "Microsoft.Network/virtualNetworks",
                  "name": "foo",
                  "apiVersion": "2024-07-01"
                }
              ],
              "diagnostics": []
            }
            """));
    }
}
