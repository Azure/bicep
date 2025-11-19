// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Playwright;

namespace Playground.E2ETests;

public class PlaygroundPage(IPage page)
{
    private const string EditorsSelector = ".playground-editorpane";
    private ILocator BicepEditor => page.Locator(EditorsSelector).First;

    private ILocator ArmEditor => page.Locator(EditorsSelector).Last;

    public async Task OpenPlayground()
    {
        await Init();
        await page.GotoAsync("http://localhost:5173/");
    }

    private async Task Init()
    {
        await page.Context.GrantPermissionsAsync(["clipboard-read", "clipboard-write"]);
    }

    public async Task CopyLink()
    {
        await page.GetByRole(
                AriaRole.Button,
                new PageGetByRoleOptions { Name = "Copy Link" })
            .ClickAsync();
    }

    public async Task OpenLink()
    {
        await page.FocusAsync("body"); // Ensure the page is focused
        var url = await page.EvaluateAsync<string>("async () => await window.navigator.clipboard.readText()");
        await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    public async Task WriteBicep(string bicep)
    {
        await BicepEditor.ClickAsync();
        await page.Keyboard.InsertTextAsync(bicep);
    }

    public async Task DeleteBicepContent()
    {
        await BicepEditor.ClickAsync();
        await page.Keyboard.PressAsync("Control+A"); // select existing content
        await page.Keyboard.PressAsync("Delete");
    }

    public async Task SelectSampleTemplate(string templateName)
    {
        await page.GetByRole(
                AriaRole.Button,
                new PageGetByRoleOptions { Name = "Sample Template" })
            .ClickAsync();

        await page.GetByRole(
                AriaRole.Button,
                new PageGetByRoleOptions { Name = templateName })
            .ClickAsync();

        await page.WaitForSelectorAsync(EditorsSelector, new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
    }

    public async Task<IReadOnlyList<string>> GetBicepEditorContent()
    {
        return await BicepEditor.AllTextContentsAsync();
    }

    public async Task<IReadOnlyList<string>> GetArmEditorContent()
    {
        return await ArmEditor.AllTextContentsAsync();
    }

    public ILocatorAssertions ExpectingBicepEditor()
    {
        return Assertions.Expect(BicepEditor);
    }

    public ILocatorAssertions ExpectingArmEditor()
    {
        return Assertions.Expect(ArmEditor);
    }
}
