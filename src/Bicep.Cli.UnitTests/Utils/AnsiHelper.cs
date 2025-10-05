// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Bicep.Cli.Helpers.Repl;
using Bicep.Cli.Helpers.WhatIf;
public static class AnsiHelper
{
    private static readonly ImmutableDictionary<string, string> escapeCodeByName = new Dictionary<string, string>
    {
        [nameof(Color.Orange)] = Color.Orange.ToString(),
        [nameof(Color.Green)] = Color.Green.ToString(),
        [nameof(Color.Purple)] = Color.Purple.ToString(),
        [nameof(Color.Blue)] = Color.Blue.ToString(),
        [nameof(Color.Gray)] = Color.Gray.ToString(),
        [nameof(Color.Reset)] = Color.Reset.ToString(),
        [nameof(Color.Red)] = Color.Red.ToString(),
        [nameof(Color.DarkYellow)] = Color.DarkYellow.ToString(),
        ["Bold"] = $"{Color.Esc}[1m",
        ["Bold,Green"] = $"{Color.Esc}[1;32m",
        ["Bold,Red"] = $"{Color.Esc}[1;91m",
        [nameof(PrintHelper.HideCursor)] = PrintHelper.HideCursor,
        [nameof(PrintHelper.ShowCursor)] = PrintHelper.ShowCursor,
        [nameof(PrintHelper.ClearToEndOfScreen)] = PrintHelper.ClearToEndOfScreen,
        [nameof(PrintHelper.MoveCursorToLineStart)] = PrintHelper.MoveCursorToLineStart,
    }.ToImmutableDictionary();

    private static readonly ImmutableDictionary<string, string> escapeCodeRegexByName = new Dictionary<string, string>
    {
        [nameof(PrintHelper.MoveCursorRight)] = "\u001b\\[(\\d+)C",
    }.ToImmutableDictionary();

    public static string ReplaceCodes(string input)
    {
        foreach (var (name, code) in escapeCodeByName)
        {
            input = input.Replace(code, $"[{name}]");
        }

        foreach (var (name, pattern) in escapeCodeRegexByName)
        {
            input = Regex.Replace(input, pattern, $"[{name}($1)]");
        }

        return input;
    }

    public static string RemoveCodes(string input)
    {
        foreach (var (name, escapeCode) in escapeCodeByName)
        {
            input = input.Replace(escapeCode, "");
        }

        foreach (var (name, pattern) in escapeCodeRegexByName)
        {
            input = Regex.Replace(input, pattern, "");
        }

        return input;
    }
}
