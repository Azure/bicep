// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Cli.Helpers.WhatIf;

public static class AnsiHelper
{
    private static readonly ImmutableDictionary<string, string> namesByEscapeCode = new Dictionary<string, string>
    {
        [Color.Orange.ToString()] = nameof(Color.Orange),
        [Color.Green.ToString()] = nameof(Color.Green),
        [Color.Purple.ToString()] = nameof(Color.Purple),
        [Color.Blue.ToString()] = nameof(Color.Blue),
        [Color.Gray.ToString()] = nameof(Color.Gray),
        [Color.Reset.ToString()] = nameof(Color.Reset),
        [Color.Red.ToString()] = nameof(Color.Red),
        [Color.DarkYellow.ToString()] = nameof(Color.DarkYellow),
        [$"{Color.Esc}[1m"] = "Bold",
        [$"{Color.Esc}[1;32m"] = "Bold,Green",
        [$"{Color.Esc}[1;91m"] = "Bold,Red",
    }.ToImmutableDictionary();

    public static string ReplaceCodes(string input)
    {
        foreach (var (escapeCode, name) in namesByEscapeCode)
        {
            input = input.Replace(escapeCode, $"[{name}]");
        }

        return input;
    }

    public static string RemoveCodes(string input)
    {
        foreach (var (escapeCode, name) in namesByEscapeCode)
        {
            input = input.Replace(escapeCode, "");
        }

        return input;
    }
}