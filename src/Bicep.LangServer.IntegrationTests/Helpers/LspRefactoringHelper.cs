// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Bicep.Core.Text;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

public static class LspRefactoringHelper
{
    public static LanguageClientFile ApplyCodeAction(LanguageClientFile bicepFile, CodeAction codeAction)
    {
        // only support a small subset of possible edits for now - can always expand this later on
        codeAction.Edit!.Changes.Should().NotBeNull();
        codeAction.Edit.Changes.Should().HaveCount(1);
        codeAction.Edit.Changes.Should().ContainKey(bicepFile.Uri);

        var bicepText = bicepFile.Text;
        var changes = codeAction.Edit.Changes![bicepFile.Uri].ToArray();

        for (int i = 0; i < changes.Length; ++i)
        {
            for (int j = i + 1; j < changes.Length; ++j)
            {
                Range.AreIntersecting(changes[i].Range, changes[j].Range).Should().BeFalse("Edits must be non-overlapping (https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/#textEdit)");
            }
        }

        // Convert to Bicep coordinates
        var lineStarts = TextCoordinateConverter.GetLineStarts(bicepText);
        var convertedChanges = changes.Select(c =>
            (NewText: c.NewText, Span: c.Range.ToTextSpan(lineStarts)))
            .ToArray();

        for (var i = 0; i < changes.Length; ++i)
        {
            var replacement = convertedChanges[i];

            var start = replacement.Span.Position;
            var end = replacement.Span.Position + replacement.Span.Length;
            var textToInsert = replacement.NewText;

            // the handler can contain tabs. convert to double space to simplify printing.
            textToInsert = textToInsert.Replace("\t", "  ");

            bicepText = bicepText.Substring(0, start) + textToInsert + bicepText.Substring(end);

            // Adjust indices for the remaining changes to account for this replacement
            int replacementOffset = textToInsert.Length - (end - start);
            for (int j = i + 1; j < changes.Length; ++j)
            {
                if (convertedChanges[j].Span.Position >= replacement.Span.Position)
                {
                    convertedChanges[j].Span = convertedChanges[j].Span.MoveBy(replacementOffset);
                }
            }
        }

        return new LanguageClientFile(bicepFile.Uri, bicepText);
    }

    public static LanguageClientFile ApplyCompletion(LanguageClientFile bicepFile, CompletionList completions, string label, params string[] tabStops)
    {
        // Should().Contain is superfluous here, but it gives a better assertion message when it fails
        completions.Should().Contain(x => x.Label == label);
        completions.Should().ContainSingle(x => x.Label == label);

        return ApplyCompletion(bicepFile, completions.Single(x => x.Label == label), tabStops);
    }

    public static LanguageClientFile ApplyCompletion(LanguageClientFile bicepFile, CompletionItem completion, params string[] tabStops)
    {
        var start = PositionHelper.GetOffset(bicepFile.LineStarts, completion.TextEdit!.TextEdit!.Range.Start);
        var end = PositionHelper.GetOffset(bicepFile.LineStarts, completion.TextEdit!.TextEdit!.Range.End);
        var textToInsert = completion.TextEdit!.TextEdit!.NewText;

        // the completion handler returns tabs. convert to double space to simplify printing.
        textToInsert = textToInsert.Replace("\t", "  ");

        // always expect this mode for now
        completion.InsertTextMode.Should().Be(InsertTextMode.AdjustIndentation);

        switch (completion.InsertTextFormat)
        {
            case InsertTextFormat.Snippet:
                // replace default tab stop with the custom cursor format we use in this test suite
                textToInsert = textToInsert.Replace("$0", "|");
                for (var i = 0; i < tabStops.Length; i++)
                {
                    textToInsert = Regex.Replace(textToInsert, $"\\${i + 1}", tabStops[i]);
                    textToInsert = Regex.Replace(textToInsert, $"\\${{{i + 1}:[^}}]+}}", tabStops[i]);
                }
                break;
            case InsertTextFormat.PlainText:
                textToInsert = textToInsert + "|";
                break;
            default:
                throw new InvalidOperationException();
        }

        var originalFile = bicepFile.Text;
        var replaced = string.Concat(originalFile.AsSpan(0, start), textToInsert, originalFile.AsSpan(end));

        return new(bicepFile.Uri, replaced);
    }
}
