// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Playwright.Xunit;
using Xunit;

namespace Bicep.Playground.E2ETests;

public class PlaygroundSpecs : PageTest
{
    private const string StorageBicep = """
                                        param storageName string
                                        param location string

                                        resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
                                            name: storageName
                                            location: location
                                            kind: 'StorageV2'
                                            sku: {
                                                name: 'Standard_LRS'
                                            }
                                            properties: {
                                                accessTier: 'Hot'
                                                supportsHttpsTrafficOnly: true
                                                minimumTlsVersion: 'TLS1_2'
                                                allowBlobPublicAccess: true
                                            }
                                        }
                                        """;

    private PlaygroundPage _page = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _page = new PlaygroundPage(Page);
    }

    [Fact]
    public async Task WhenSelectingQuickStarterTemplate_ThenShouldOpenSampleAndDecompileToArm()
    {
        await _page.OpenPlayground();

        await _page.SelectSampleTemplate("canonical/anbox/main.bicep");

        await _page.ExpectingBicepEditorContentToContain("""
                                                         @description('Add a dedicated disk for the LXD storage pool')
                                                         param addDedicatedDataDiskForLXD bool = true
                                                         """);
    }

    [Fact]
    public async Task WhenCopyLink_ThenContentShouldBeSameAfterOpenLink()
    {
        await _page.OpenPlayground();

        await _page.PasteInBicepEditor(StorageBicep);

        await _page.CopyLinkToCurrentExample();

        await _page.NavigateToCopiedLink();

        await _page.ExpectingBicepEditorContentToBeEquivalentTo(StorageBicep);
    }

    [Fact]
    public async Task WhenInsertingBicep_ThenShouldCompileToArmJson()
    {
        await _page.OpenPlayground();

        await _page.PasteInBicepEditor(StorageBicep);

        await _page.ExpectingArmEditorContentToBeEquivalentTo("""
                                                              {
                                                                "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                                                                "contentVersion": "1.0.0.0",
                                                                "metadata": {
                                                                  "_generator": {
                                                                    "name": "bicep",
                                                                    "version": "0.39.78.63741",
                                                                    "templateHash": "9724347989709413195"
                                                                  }
                                                                },
                                                                "parameters": {
                                                                  "storageName": {
                                                                    "type": "string"
                                                                  },
                                                                  "location": {
                                                                    "type": "string"
                                                                  }
                                                                },
                                                                "resources": [
                                                                  {
                                                                    "type": "Microsoft.Storage/storageAccounts",
                                                                    "apiVersion": "2021-02-01",
                                                                    "name": "[parameters('storageName')]",
                                                                    "location": "[parameters('location')]",
                                                                    "kind": "StorageV2",
                                                                    "sku": {
                                                                      "name": "Standard_LRS"
                                                                    },
                                                                    "properties": {
                                                                      "accessTier": "Hot",
                                                                      "supportsHttpsTrafficOnly": true,
                                                                      "minimumTlsVersion": "TLS1_2",
                                                                      "allowBlobPublicAccess": true
                                                                    }
                                                                  }
                                                                ]
                                                              }
                                                              """);
    }
}
