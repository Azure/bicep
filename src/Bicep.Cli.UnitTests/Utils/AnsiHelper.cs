// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Commands.Helpers.Deploy;
using Bicep.Cli.Helpers.WhatIf;
using Humanizer;

public static class AnsiHelper
{
    public static string ReplaceCodes(string input)
    {
        var namesByEscapeCode = new Dictionary<string, string>
        {
            [Color.Orange.ToString()] = nameof(Color.Orange),
            [Color.Green.ToString()] = nameof(Color.Green),
            [Color.Purple.ToString()] = nameof(Color.Purple),
            [Color.Blue.ToString()] = nameof(Color.Blue),
            [Color.Gray.ToString()] = nameof(Color.Gray),
            [Color.Reset.ToString()] = nameof(Color.Reset),
            [Color.Red.ToString()] = nameof(Color.Red),
            [Color.DarkYellow.ToString()] = nameof(Color.DarkYellow),
            [DeploymentRenderer.EraseLine] = nameof(DeploymentRenderer.EraseLine),
            [DeploymentRenderer.Bold] = nameof(DeploymentRenderer.Bold),
            [DeploymentRenderer.HideCursor] = nameof(DeploymentRenderer.HideCursor),
            [DeploymentRenderer.ShowCursor] = nameof(DeploymentRenderer.ShowCursor),
        };

        foreach (var (escapeCode, name) in namesByEscapeCode)
        {
            input = input.Replace(escapeCode, $"[{name}]");
        }

        // go backwards to avoid messing up indices as we go
        for (var i = input.Length - 1; i >= 0; i--)
        {
            if (input[i] == DeploymentRenderer.Esc &&
                i + 1 < input.Length &&
                input[i + 1] == '[')
            {
                string? newInput = null;
                for (var j = i + 2; j < input.Length; j++)
                {
                    if (input[j] == 'F')
                    {
                        newInput = $"{input[..i]}[RewindLines({input[(i + 2)..j]})]{input[(j + 1)..]}";
                        break;
                    }
                }

                if (newInput is null)
                {
                    // Add handling for other escape codes here if we hit this error
                    throw new InvalidOperationException("Unhandled ANSI escape code");
                }

                input = newInput;
            }
        }

        return input;
    }
}