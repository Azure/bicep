// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Text.RegularExpressions;
using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Models;
using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core.Configuration;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.FileSystem;
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
            result.Stderr.Should().BeEmpty();

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
  "diagnostics": [],
  "outputs": {}
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
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public async Task Snapshot_with_custom_output_should_generate_and_validate_snapshot_file(bool useOutputDir, bool useRelativePath)
    {
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var bicepparamPath = Path.Combine(outputPath, "parameters", "main.bicepparam");
        var bicepPath = Path.ChangeExtension(bicepparamPath, ".bicep");
        var outputDir = Path.Combine(outputPath, "snapshots");
        var snapshotPath = Path.Combine(outputDir, useOutputDir ? "main.snapshot.json" : "custom.json");
        var outputOption = useOutputDir ? "--outdir" : "--outfile";
        var outputArgument = useOutputDir ? outputDir : snapshotPath;
        if (useRelativePath)
        {
            outputArgument = Path.GetRelativePath(outputPath, outputArgument);
        }

        var subscriptionId = Guid.Empty.ToString();

        var template = """
            targetScope = 'subscription'

            resource rg 'Microsoft.Resources/resourceGroups@2024-11-01' = {
              name: 'myRg'
              location: 'eastus'
            }
            """;
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [bicepparamPath] = new("using './main.bicep'"),
            [bicepPath] = new(template),
        });
        fileSystem.Directory.SetCurrentDirectory(outputPath);

        var missingResult = await Bicep(
            services => services.WithFileSystem(fileSystem),
            "snapshot", bicepparamPath, "--mode", "validate", outputOption, outputArgument, "--subscription-id", subscriptionId);
        missingResult.Should().Fail();
        missingResult.Stderr.Should().Contain($"Error opening file {snapshotPath}:");
        fileSystem.File.Exists(snapshotPath).Should().BeFalse();
        fileSystem.Directory.Exists(outputDir).Should().BeFalse();

        var overwriteResult = await Bicep(
            services => services.WithFileSystem(fileSystem),
            "snapshot", bicepparamPath, outputOption, outputArgument, "--subscription-id", subscriptionId);
        overwriteResult.Should().Succeed();
        overwriteResult.Stderr.Should().BeEmpty();
        fileSystem.File.Exists(Path.ChangeExtension(bicepparamPath, ".snapshot.json")).Should().BeFalse();

        var snapshotContents = fileSystem.File.ReadAllText(snapshotPath);
        snapshotContents.FromJson<JToken>().Should().DeepEqual(JObject.Parse("""
            {
              "predictedResources": [
                {
                  "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/myRg",
                  "type": "Microsoft.Resources/resourceGroups",
                  "name": "myRg",
                  "apiVersion": "2024-11-01",
                  "location": "eastus"
                }
              ],
              "diagnostics": [],
              "outputs": {}
            }
            """));

        var validateResult = await Bicep(
            services => services.WithFileSystem(fileSystem),
            "snapshot", bicepparamPath, "--mode", "validate", outputOption, outputArgument, "--subscription-id", subscriptionId);
        validateResult.Should().Succeed();
        validateResult.Stderr.Should().BeEmpty();
        fileSystem.File.ReadAllText(snapshotPath).Should().Be(snapshotContents);

        fileSystem.File.WriteAllText(bicepPath, template.Replace("'eastus'", "'westus'"));
        var changedResult = await Bicep(
            services => services.WithFileSystem(fileSystem),
            "snapshot", bicepparamPath, "--mode", "validate", outputOption, outputArgument, "--subscription-id", subscriptionId);
        changedResult.Should().Fail();
        changedResult.Stderr.Should().Contain("Snapshot validation failed.");
        fileSystem.File.ReadAllText(snapshotPath).Should().Be(snapshotContents);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task Snapshot_with_custom_output_should_overwrite_only_selected_snapshot(bool useOutputDir)
    {
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var bicepparamPath = Path.Combine(outputPath, "parameters", "production.bicepparam");
        var outputDir = Path.Combine(outputPath, "snapshot files", "nested");
        var snapshotPath = Path.Combine(outputDir, useOutputDir ? "production.snapshot.json" : "custom snapshot");
        var adjacentSnapshotPath = Path.ChangeExtension(bicepparamPath, ".snapshot.json");
        var siblingSnapshotPath = Path.Combine(outputDir, "unrelated.snapshot.json");
        const string unrelatedContents = "Leave this snapshot unchanged.";
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [bicepparamPath] = new("using './main.bicep'"),
            [Path.Combine(outputPath, "parameters", "main.bicep")] = new("output message string = 'hello'"),
            [snapshotPath] = new("Replace this snapshot."),
            [adjacentSnapshotPath] = new(unrelatedContents),
            [siblingSnapshotPath] = new(unrelatedContents),
        });
        fileSystem.Directory.SetCurrentDirectory(outputPath);

        var outputOption = useOutputDir ? "--outdir" : "--outfile";
        var outputArgument = Path.GetRelativePath(outputPath, useOutputDir ? outputDir : snapshotPath);
        var overwriteResult = await Bicep(
            services => services.WithFileSystem(fileSystem),
            "snapshot", bicepparamPath, "--mode", "overwrite", outputOption, outputArgument);
        overwriteResult.Should().Succeed();
        overwriteResult.Stderr.Should().BeEmpty();

        var snapshotContents = fileSystem.File.ReadAllText(snapshotPath);
        snapshotContents.FromJson<JToken>().Should().DeepEqual(JObject.Parse("""
            {
              "predictedResources": [],
              "diagnostics": [],
              "outputs": {
                "message": "hello"
              }
            }
            """));

        var validateResult = await Bicep(
            services => services.WithFileSystem(fileSystem),
            "snapshot", bicepparamPath, "--mode", "validate", outputOption, outputArgument);
        validateResult.Should().Succeed();
        validateResult.Stderr.Should().BeEmpty();
        fileSystem.File.ReadAllText(snapshotPath).Should().Be(snapshotContents);
        fileSystem.File.ReadAllText(adjacentSnapshotPath).Should().Be(unrelatedContents);
        fileSystem.File.ReadAllText(siblingSnapshotPath).Should().Be(unrelatedContents);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task Snapshot_validate_with_custom_output_should_not_fall_back_to_adjacent_snapshot(bool useOutputDir)
    {
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var bicepparamPath = Path.Combine(outputPath, "parameters", "main.bicepparam");
        var adjacentSnapshotPath = Path.ChangeExtension(bicepparamPath, ".snapshot.json");
        var outputDir = Path.Combine(outputPath, "snapshots", "missing");
        var snapshotPath = Path.Combine(outputDir, useOutputDir ? "main.snapshot.json" : "custom.json");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [bicepparamPath] = new("using './main.bicep'"),
            [Path.ChangeExtension(bicepparamPath, ".bicep")] = new("output message string = 'hello'"),
        });
        fileSystem.Directory.SetCurrentDirectory(outputPath);

        var overwriteResult = await Bicep(
            services => services.WithFileSystem(fileSystem),
            "snapshot", bicepparamPath);
        overwriteResult.Should().Succeed();
        overwriteResult.Stderr.Should().BeEmpty();
        var adjacentSnapshotContents = fileSystem.File.ReadAllText(adjacentSnapshotPath);

        var outputOption = useOutputDir ? "--outdir" : "--outfile";
        var outputArgument = Path.GetRelativePath(outputPath, useOutputDir ? outputDir : snapshotPath);
        var validateResult = await Bicep(
            services => services.WithFileSystem(fileSystem),
            "snapshot", bicepparamPath, "--mode", "validate", outputOption, outputArgument);
        validateResult.Should().Fail();
        validateResult.Stderr.Should().Contain($"Error opening file {snapshotPath}:");
        validateResult.Stdout.Should().BeEmpty();
        fileSystem.File.Exists(snapshotPath).Should().BeFalse();
        fileSystem.Directory.Exists(outputDir).Should().BeFalse();
        fileSystem.File.ReadAllText(adjacentSnapshotPath).Should().Be(adjacentSnapshotContents);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task Snapshot_with_outdir_should_keep_distinct_filenames(bool includeTrailingSeparator)
    {
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var parametersDir = Path.Combine(outputPath, "parameters");
        var outputDir = Path.Combine(outputPath, "snapshot files", "nested");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [Path.Combine(parametersDir, "main.bicep")] = new("param message string\noutput result string = message"),
        });
        fileSystem.Directory.SetCurrentDirectory(outputPath);

        var outputArgument = Path.GetRelativePath(outputPath, outputDir);
        if (includeTrailingSeparator)
        {
            outputArgument += Path.DirectorySeparatorChar;
        }

        (string InputFile, string SnapshotFile, string Value)[] parameterFiles =
        [
            ("development.bicepparam", "development.snapshot.json", "dev"),
            ("production.params.bicepparam", "production.params.snapshot.json", "prod"),
            ("staging environment.bicepparam", "staging environment.snapshot.json", "staging"),
        ];

        foreach (var (inputFile, _, value) in parameterFiles)
        {
            var inputPath = Path.Combine(parametersDir, inputFile);
            fileSystem.AddFile(inputPath, $"using './main.bicep'\nparam message = '{value}'");

            var result = await Bicep(
                services => services.WithFileSystem(fileSystem),
                "snapshot", inputPath, "--outdir", outputArgument);
            result.Should().Succeed();
            result.Stderr.Should().BeEmpty();
        }

        foreach (var (inputFile, snapshotFile, value) in parameterFiles)
        {
            var snapshotContents = fileSystem.File.ReadAllText(Path.Combine(outputDir, snapshotFile)).FromJson<JToken>();
            ((string?)snapshotContents.SelectToken("outputs.result")).Should().Be(value);
            fileSystem.File.Exists(Path.ChangeExtension(Path.Combine(parametersDir, inputFile), ".snapshot.json")).Should().BeFalse();
        }

        fileSystem.Directory.GetFiles(outputDir).Should().BeEquivalentTo(
            parameterFiles.Select(file => Path.Combine(outputDir, file.SnapshotFile)));
    }

    [TestMethod]
    [DataRow(false, "overwrite")]
    [DataRow(false, "validate")]
    [DataRow(true, "overwrite")]
    [DataRow(true, "validate")]
    public async Task Snapshot_with_custom_output_should_preserve_existing_snapshot_on_compilation_error(bool useOutputDir, string mode)
    {
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var bicepparamPath = Path.Combine(outputPath, "parameters", "main.bicepparam");
        var outputDir = Path.Combine(outputPath, "snapshots");
        var snapshotPath = Path.Combine(outputDir, useOutputDir ? "main.snapshot.json" : "custom.json");
        const string snapshotContents = "Leave this snapshot unchanged.";
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            [bicepparamPath] = new("using './main.bicep'"),
            [Path.ChangeExtension(bicepparamPath, ".bicep")] = new("param value string ="),
            [snapshotPath] = new(snapshotContents),
        });
        fileSystem.Directory.SetCurrentDirectory(outputPath);

        var outputOption = useOutputDir ? "--outdir" : "--outfile";
        var outputArgument = useOutputDir ? outputDir : snapshotPath;
        var result = await Bicep(
            services => services.WithFileSystem(fileSystem),
            "snapshot", bicepparamPath, "--mode", mode, outputOption, outputArgument);
        result.Should().Fail();
        result.Stderr.Should().Contain("Error BCP");
        fileSystem.File.ReadAllText(snapshotPath).Should().Be(snapshotContents);
        fileSystem.File.Exists(Path.ChangeExtension(bicepparamPath, ".snapshot.json")).Should().BeFalse();
    }

    [TestMethod]
    [EmbeddedFilesTestData(@"Files/SnapshotCommandTests/.*/main\.bicepparam")]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    public async Task Snapshot_generates_correct_format(EmbeddedFile paramFile)
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetHttpExtensionTypesTgz(), new(), artifactTarget: "example.azurecr.io/extensions/snapshot:1.2.3");
        var clientFactory = services.Build().Construct<IContainerRegistryClientFactory>();

        var subscriptionId = new Guid().ToString();
        var resourceGroupName = "myRg";

        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, paramFile);
        var snapshotFile = baselineFolder.GetFileOrEnsureCheckedIn("main.snapshot.json");

        var result = await Bicep(
            services => services.WithContainerRegistryClientFactory(clientFactory),
            "snapshot", baselineFolder.EntryFile.OutputFilePath, "--mode", "overwrite", "--subscription-id", subscriptionId, "--resource-group", resourceGroupName);
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
              "diagnostics": [],
              "outputs": {}
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
              "diagnostics": [],
              "outputs": {}
            }
            """));
    }

    [TestMethod]
    public async Task Snapshot_command_handles_extension_resources_correctly()
    {
        // https://github.com/Azure/bicep/issues/18822
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ExtensionResourceTypeHelper.GetTestTypesTgz(), new(), artifactTarget: "example.azurecr.io/snap_test:1.2.3");
        var clientFactory = services.Build().Construct<IContainerRegistryClientFactory>();

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
            extension 'br:example.azurecr.io/snap_test:1.2.3'

            resource clientAppWorking 'fooType@v1' = {
              identifier: 'foo'
              properties: {
                required: 'bar'
              }
            }

            resource clientAppWorkingSp 'fooType@v1' = {
              identifier: 'bar'
              properties: {
                required: clientAppWorking.properties.readonly
              }
            }
            """,
            testOutputPath: outputPath);

        var outputFilePath = FileHelper.GetResultFilePath(TestContext, "main.snapshot.json", testOutputPath: outputPath);

        var result = await Bicep(
            services => services.WithContainerRegistryClientFactory(clientFactory),
            "snapshot", bicepparamPath, "--mode", "overwrite");

        result.Should().Succeed();

        var snapshotContents = File.ReadAllText(outputFilePath).FromJson<JToken>();
        snapshotContents.Should().DeepEqual(JObject.Parse("""
        {
          "predictedResources": [
            {
              "properties": {
                "identifier": "foo",
                "properties": {
                  "required": "bar"
                }
              },
              "type": "fooType@v1"
            },
            {
              "properties": {
                "identifier": "bar",
                "properties": {
                  "required": "[reference('clientAppWorking').properties.readonly]"
                }
              },
              "type": "fooType@v1"
            }
          ],
          "diagnostics": [],
          "outputs": {}
        }
        """));
    }

    [TestMethod]
    public async Task Snapshot_command_supports_managementGroup_scope()
    {
        // https://github.com/Azure/bicep/issues/19319
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var bicepparamPath = FileHelper.SaveResultFile(
            TestContext,
            "main.bicepparam",
            """
                using './main.bicep'

                param policyAssignmentName = 'myAssignment'
                """,
            testOutputPath: outputPath);

        FileHelper.SaveResultFile(
            TestContext,
            "main.bicep",
            """
                targetScope = 'managementGroup'

                param policyAssignmentName string

                resource policyAssignment 'Microsoft.Authorization/policyAssignments@2022-06-01' = {
                  name: policyAssignmentName
                  properties: {
                    policyDefinitionId: '/providers/Microsoft.Management/managementGroups/${managementGroup().name}/providers/Microsoft.Authorization/policyDefinitions/00000000-0000-0000-0000-000000000000'
                  }
                }
                """,
            testOutputPath: outputPath);

        var outputFilePath = FileHelper.GetResultFilePath(TestContext, "main.snapshot.json", testOutputPath: outputPath);

        var managementGroupId = "myManagementGroup";

        var result = await Bicep("snapshot", bicepparamPath, "--mode", "overwrite", "--management-group-id", managementGroupId, "--location", "westeurope");

        result.Should().Succeed();
        result.Stderr.Should().BeEmpty();

        var snapshotContents = File.ReadAllText(outputFilePath).FromJson<JToken>();
        snapshotContents.Should().DeepEqual(JObject.Parse("""
            {
              "predictedResources": [
                {
                  "id": "/providers/Microsoft.Management/managementGroups/myManagementGroup/providers/Microsoft.Authorization/policyAssignments/myAssignment",
                  "type": "Microsoft.Authorization/policyAssignments",
                  "name": "myAssignment",
                  "apiVersion": "2022-06-01",
                  "properties": {
                    "policyDefinitionId": "/providers/Microsoft.Management/managementGroups/myManagementGroup/providers/Microsoft.Authorization/policyDefinitions/00000000-0000-0000-0000-000000000000"
                  }
                }
              ],
              "diagnostics": [],
              "outputs": {}
            }
            """));
    }

    [TestMethod]
    public async Task Snapshot_command_supports_roleDefinitions_function()
    {
        var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
        var bicepparamPath = FileHelper.SaveResultFile(
            TestContext,
            "main.bicepparam",
            """
                using './main.bicep'

                param principalId = '00000000-0000-0000-0000-000000000000'
                """,
            testOutputPath: outputPath);

        FileHelper.SaveResultFile(
            TestContext,
            "main.bicep",
            """
                targetScope = 'subscription'

                param principalId string

                var contributorRole = az.roleDefinitions('Contributor')

                resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
                  name: guid('test')
                  properties: {
                    principalId: principalId
                    roleDefinitionId: contributorRole.id
                  }
                }
                """,
            testOutputPath: outputPath);

        var outputFilePath = FileHelper.GetResultFilePath(TestContext, "main.snapshot.json", testOutputPath: outputPath);

        var subscriptionId = new Guid().ToString();

        var result = await Bicep("snapshot", bicepparamPath, "--mode", "overwrite", "--subscription-id", subscriptionId, "--location", "eastus");

        result.Should().Succeed();
        result.Stderr.Should().BeEmpty();

        var snapshotContents = File.ReadAllText(outputFilePath).FromJson<JToken>();
        ((string?)snapshotContents.SelectToken("$.predictedResources[0].properties.roleDefinitionId"))
            .Should().Be("[roleDefinitions('Contributor').id]");
        var diagnostics = (JArray?)snapshotContents.SelectToken("$.diagnostics");
        diagnostics.Should().NotBeNull();
        diagnostics!.Count.Should().Be(0);
    }
}
