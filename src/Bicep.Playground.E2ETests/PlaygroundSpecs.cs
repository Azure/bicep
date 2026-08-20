// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Playwright;
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
    public async Task WhenSelectingQuickStarterTemplateWithLocalModules_ThenShouldCompileToArm()
    {
        await _page.OpenPlayground();

        await _page.SelectSampleTemplate("microsoft.desktopvirtualization/azure-virtual-desktop-with-fslogix/main.bicep");

        await _page.ExpectingArmEditorContentToContain("\"Microsoft.DesktopVirtualization/applicationGroups\"");
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
    public async Task WhenSampleDownloadFails_ThenShouldPreserveEditorContentAndShowError()
    {
        const string bicep = "param preservedContent string = 'still here'";

        await _page.OpenPlayground();
        await _page.PasteInBicepEditor(bicep);
        await Page.RouteAsync(
            "https://raw.githubusercontent.com/**",
            route => route.FulfillAsync(new RouteFulfillOptions
            {
                Status = 503,
                Body = "Sample unavailable",
            }));

        await _page.SelectSampleTemplate("canonical/anbox/main.bicep");

        await _page.ExpectingErrorToContain("could not be loaded");
        await _page.ExpectingBicepEditorContentToBeEquivalentTo(bicep);
    }

    [Fact]
    public async Task WhenClipboardWriteFails_ThenShouldShowError()
    {
        await _page.OpenPlayground();
        await Page.EvaluateAsync(
            """
            Object.defineProperty(navigator.clipboard, 'writeText', {
              configurable: true,
              value: () => Promise.reject(new Error('Clipboard permission denied')),
            });
            """);

        await _page.CopyLinkToCurrentExample();

        await _page.ExpectingErrorToContain("Clipboard permission denied");
    }

    [Fact]
    public async Task WhenDecompileFails_ThenShouldPreserveEditorContentAndShowError()
    {
        const string bicep = "param preservedContent string = 'still here'";

        await _page.OpenPlayground();
        await _page.PasteInBicepEditor(bicep);
        await Page.Locator("input[type=file]").SetInputFilesAsync(new FilePayload
        {
            Name = "invalid.json",
            MimeType = "application/json",
            Buffer = "not valid JSON"u8.ToArray(),
        });

        await _page.ExpectingErrorToContain("Unexpected character");
        await _page.ExpectingBicepEditorContentToBeEquivalentTo(bicep);
    }

    [Fact]
    public async Task WhenCompilerDownloadFails_ThenShouldShowErrorAndAllowRetry()
    {
        const string compilerScript = "**/_framework/blazor.webassembly.js";
        var port = Environment.GetEnvironmentVariable("PlaygroundPort") ?? "4173";

        await Page.RouteAsync(compilerScript, route => route.AbortAsync());
        await Page.GotoAsync($"http://localhost:{port}/");

        await Page
            .GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Bicep Playground could not start" })
            .WaitForAsync();

        await Page.UnrouteAsync(compilerScript);
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Retry" }).ClickAsync();

        await Page.Locator(".playground-editorpane").First.WaitForAsync();
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
