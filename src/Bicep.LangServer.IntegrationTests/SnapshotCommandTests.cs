// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class SnapshotCommandTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private async Task<(string output, string snapshotPath)> RunSnapshot(string bicepparamContents, params (string name, string contents)[] otherFiles)
        {
            var diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();

            using var helper = await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options.OnPublishDiagnostics(diagnosticsListener.AddMessage),
                services => services.WithFeatureOverrides(new(TestContext)));
            var client = helper.Client;

            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            foreach (var (name, contents) in otherFiles)
            {
                FileHelper.SaveResultFile(TestContext, name, contents, outputPath);
            }
            var bicepparamPath = FileHelper.SaveResultFile(TestContext, "main.bicepparam", bicepparamContents, outputPath);

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParamsFromFile(bicepparamPath, 1));
            await diagnosticsListener.WaitNext();

            var output = await client.Workspace.ExecuteCommandWithResponse<string>(new Command
            {
                Name = "snapshot",
                Arguments = new JArray { DocumentUri.FromFileSystemPath(bicepparamPath).ToString() },
            });

            return (output, Path.Combine(outputPath, "main.snapshot.json"));
        }

        [TestMethod]
        public async Task Snapshot_command_should_generate_snapshot_file()
        {
            var (output, snapshotPath) = await RunSnapshot(
                """
                using './main.bicep'

                param location = 'eastus'
                """,
                ("main.bicep", """
                    param location string

                    resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                      name: 'mystorage'
                      location: location
                      sku: {
                        name: 'Standard_LRS'
                      }
                      kind: 'StorageV2'
                    }
                    """));

            output.Should().StartWith("Snapshot generation succeeded. Created file ");
            File.ReadAllText(snapshotPath).FromJson<JToken>().Should().DeepEqual(JObject.Parse("""
                {
                  "predictedResources": [
                    {
                      "id": "[resourceId(subscription().subscriptionId, resourceGroup().name, 'Microsoft.Storage/storageAccounts', 'mystorage')]",
                      "type": "Microsoft.Storage/storageAccounts",
                      "name": "mystorage",
                      "apiVersion": "2022-09-01",
                      "location": "eastus",
                      "sku": {
                        "name": "Standard_LRS"
                      },
                      "kind": "StorageV2"
                    }
                  ],
                  "diagnostics": [],
                  "outputs": {}
                }
                """));
        }

        [TestMethod]
        public async Task Snapshot_command_should_report_compilation_errors_and_not_write_file()
        {
            var (output, snapshotPath) = await RunSnapshot(
                """
                using './main.bicep'

                param location = 123
                """,
                ("main.bicep", "param location string"));

            output.Should().StartWith("Generating snapshot file failed. Please fix below errors:");
            output.Should().Contain("BCP");
            File.Exists(snapshotPath).Should().BeFalse();
        }

        [TestMethod]
        public async Task Snapshot_command_should_report_unsupported_for_using_none()
        {
            var (output, snapshotPath) = await RunSnapshot(
                """
                using none

                param location = 'eastus'
                """);

            output.Should().Be("Generating snapshot file failed. Snapshots are only supported for parameters files that reference a local Bicep or ARM template file.");
            File.Exists(snapshotPath).Should().BeFalse();
        }

        [TestMethod]
        public async Task Snapshot_command_should_surface_template_validation_errors()
        {
            var (output, snapshotPath) = await RunSnapshot(
                """
                using './main.bicep'

                param condition = true
                """,
                ("main.bicep", """
                    param condition bool

                    var foo = condition ? fail('condition must be false') : 'foo'
                    """));

            output.Should().StartWith("Snapshot generation failed: ");
            output.Should().Contain("condition must be false");
            File.Exists(snapshotPath).Should().BeFalse();
        }
    }
}
