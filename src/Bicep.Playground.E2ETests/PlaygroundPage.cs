// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Playwright;

namespace Bicep.Playground.E2ETests;

public class PlaygroundPage(IPage page)
{
    private const string EditorsSelector = ".playground-editorpane";
    private ILocator BicepEditorPane => page.Locator(EditorsSelector).First;

    private ILocator ArmEditorPane => page.Locator(EditorsSelector).Last;

    public async Task OpenPlayground()
    {
        await page.Context.GrantPermissionsAsync(["clipboard-read", "clipboard-write"]);
        var port = Environment.GetEnvironmentVariable("PlaygroundPort") ?? "4173";
        await page.GotoAsync($"http://localhost:{port}/");
    }

    public async Task CopyLinkToCurrentExample()
    {
        await page.GetByRole(
                AriaRole.Button,
                new PageGetByRoleOptions { Name = "Copy Link" })
            .ClickAsync();
    }

    public async Task NavigateToCopiedLink()
    {
        await page.FocusAsync("body"); // Ensure the page is focused
        var url = await page.EvaluateAsync<string>("async () => await window.navigator.clipboard.readText()");
        await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    public async Task PasteInBicepEditor(string bicep)
    {
        await page.EvaluateAsync("async (text) => await navigator.clipboard.writeText(text);", bicep);
        await BicepEditorPane.ClickAsync();
        await page.Keyboard.PressAsync("Control+A");
        await page.Keyboard.PressAsync("Control+V");
    }

    public async Task DeleteBicepContent()
    {
        await BicepEditorPane.ClickAsync();
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

        await page.WaitForSelectorAsync(EditorsSelector,
            new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });
    }

    public async Task ExpectingBicepEditorContentToContain(string expectedContent)
    {
        var content = await GetEditorContent(BicepEditorPane);
        content.Should().Contain(ReplaceLineEndings(expectedContent));
    }

    public async Task ExpectingBicepEditorContentToBeEquivalentTo(string expectedContent)
    {
        var content = await GetEditorContent(BicepEditorPane);
        content.Should().BeEquivalentTo(ReplaceLineEndings(expectedContent));
    }

    public async Task ExpectingArmEditorContentToBeEquivalentTo(string expectedContent)
    {
        var content = await GetEditorContent(ArmEditorPane);
        content = IgnoreGeneratorField(content);
        content.Should().BeEquivalentTo(ReplaceLineEndings(IgnoreGeneratorField(expectedContent)));
    }

    private string IgnoreGeneratorField(string content)
    {
        var pattern = "\"_generator\"\\s*:\\s*\\{[^}]*\\}";
        return Regex.Replace(content, pattern, string.Empty, RegexOptions.Singleline);
    }

    private async Task<string> GetEditorContent(ILocator editorPane)
    {
        // Needs to get content this way because monaco editor keeps content in different divs.
        await editorPane.ClickAsync();
        await page.Keyboard.PressAsync("Control+A");
        await page.Keyboard.PressAsync("Control+C");
        var content = await page.EvaluateAsync<string>("async () => await window.navigator.clipboard.readText()");
        return ReplaceLineEndings(content);
    }

    private string ReplaceLineEndings(string content)
    {
        return content.Replace("\r\n", "\n");
    }
}
