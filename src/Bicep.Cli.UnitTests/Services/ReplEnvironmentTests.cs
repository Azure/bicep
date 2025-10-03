// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Parsing;
using Bicep.Core.UnitTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;

namespace Bicep.Cli.UnitTests.Services;

[TestClass]
public class ReplEnvironmentTests
{
    private static ReplEnvironment CreateReplEnvironment()
        => ServiceBuilder.Create(x => x.AddSingleton<ReplEnvironment>())
            .Construct<ReplEnvironment>();

    [TestMethod]
    public void HighlightInputLine_succeeds()
    {
        var replEnvironment = CreateReplEnvironment();

        var lines = StringUtils.SplitOnNewLine("""
            var test = /*
            foo
            bar
            */
            v
            """).ToArray();

        var output = new List<string>();
        for (var i = 0; i < lines.Length; i++)
        {
            var prevLines = string.Concat(lines.Take(i).Select(x => x + "\n"));
            var runes = lines[i].Select(x => new Rune(x)).ToArray();

            var prefix = i == 0 ? "> " : "";
            output.Add(replEnvironment.HighlightInputLine(prefix, prevLines, runes, runes.Length, printPrevLines: false));
        }

        output.Select(AnsiHelper.ReplaceCodes).Should().BeEquivalentTo([
            "[HideCursor][MoveCursorToLineStart][ClearToEndOfScreen]> [Orange]var[Reset] [Purple]test[Reset] = [Green]/*[Reset][MoveCursorToLineStart][MoveCursorRight(2)][MoveCursorRight(13)][ShowCursor]",
            "[HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Green]foo[Reset][MoveCursorToLineStart][MoveCursorRight(3)][ShowCursor]",
            "[HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Green]bar[Reset][MoveCursorToLineStart][MoveCursorRight(3)][ShowCursor]",
            "[HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Green]*/[Reset][MoveCursorToLineStart][MoveCursorRight(2)][ShowCursor]",
            "[HideCursor][MoveCursorToLineStart][ClearToEndOfScreen][Purple]v[Reset][MoveCursorToLineStart][MoveCursorRight(1)][ShowCursor]",
        ]);
    }
}