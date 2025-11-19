// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace Playground.E2ETests;

public class PlaygroundSpecs : PageTest
{
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

        await _page.ExpectingBicepEditor()
            .ToContainTextAsync("anbox", new LocatorAssertionsToContainTextOptions { IgnoreCase = true });
            
        await _page.ExpectingArmEditor()
            .ToContainTextAsync("anbox", new LocatorAssertionsToContainTextOptions { IgnoreCase = true });
    }

    [Fact]
    public async Task WhenCopyLink_ThenContentShouldBeSameAfterOpenLink()
    {
        await _page.OpenPlayground();

        await _page.WriteBicep("""
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
                               """);

        var bicepContentBefore = await _page.GetBicepEditorContent();

        await _page.CopyLink();

        await _page.OpenLink();

        var bicepContentAfter = await _page.GetBicepEditorContent();

        bicepContentBefore.Should().BeEquivalentTo(bicepContentAfter);
    }
}
