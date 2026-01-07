// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers.Repl;
using Bicep.Cli.Services;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace Bicep.Cli.Commands;

/// <summary>
/// Minimal interactive console (experimental).
/// Supports multi-line input: enter expressions or variable declarations.
/// Input is submitted automatically when structurally complete, or by entering a blank line once complete.
/// </summary>
public class ConsoleCommand(
    ILogger logger,
    IOContext io,
    IEnvironment environment,
    ReplEnvironment replEnvironment,
    IAnsiConsole console) : ICommand
{
    private const string FirstLinePrefix = "> ";

    private Rune ReadRune(char firstChar)
        => ReadRune(firstChar, () => Console.ReadKey(intercept: true).KeyChar);

    private Rune ReadRune(char firstChar, Func<char> readNextChar)
    {
        if (char.IsHighSurrogate(firstChar))
        {
            var secondChar = readNextChar();
            return new Rune(char.ConvertToUtf32(firstChar, secondChar));
        }

        return new Rune(firstChar);
    }

    private IEnumerable<Rune> GetRunes(string text)
    {
        for (var i = 0; i < text.Length; i++)
        {
            yield return ReadRune(text[i], () => text[++i]);
        }
    }

    public async Task<int> RunAsync(ConsoleArguments args)
    {
        logger.LogWarning($"WARNING: The '{args.CommandName}' CLI command is an experimental feature. Experimental features should be used for testing purposes only, as there are no guarantees about the quality or stability of these features.");

        if (!console.Profile.Capabilities.Interactive)
        {
            logger.LogError($"The '{args.CommandName}' CLI command requires an interactive console.");
            return 1;
        }

        await io.Output.WriteLineAsync($"Bicep Console version {environment.GetVersionString()}");
        await io.Output.WriteLineAsync("Type 'help' for available commands, press ESC to quit.");
        await io.Output.WriteLineAsync("Multi-line input supported.");
        await io.Output.WriteLineAsync(string.Empty);

        var buffer = new StringBuilder();

        while (true)
        {
            if (await ReadLine(buffer) is not { } rawLine)
            {
                break;
            }

            if (buffer.Length == 0)
            {
                if (rawLine.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (rawLine.Equals("clear", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Clear();
                    continue;
                }

                if (rawLine.Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    await io.Output.WriteLineAsync("Enter expressions or 'var name = <expr>'. Multi-line supported until structure closes.");
                    await io.Output.WriteLineAsync("Commands: exit, clear");
                    continue;
                }
            }

            buffer.Append(rawLine);
            buffer.Append('\n');

            var current = buffer.ToString();

            if (ReplEnvironment.ShouldSubmitBuffer(current, rawLine))
            {
                buffer.Clear();

                // evaluate input
                var output = replEnvironment.EvaluateAndGetOutput(current);
                await io.Output.WriteAsync(output);
            }
        }

        return 0;
    }

    private async Task<int> PrintHistory(StringBuilder buffer, List<Rune> lineBuffer, int cursorOffset, bool backwards)
    {
        if (replEnvironment.TryGetHistory(backwards) is { } history)
        {
            var prevBufferLineCount = buffer.ToString().Count(x => x == '\n');
            buffer.Clear();
            lineBuffer.Clear();

            var finalNewline = history.LastIndexOf('\n');
            var lineStart = finalNewline > -1 ? finalNewline + 1 : 0;

            buffer.Append(history[..lineStart]);
            lineBuffer.AddRange(GetRunes(history[lineStart..]));
            cursorOffset = lineBuffer.Count;

            var output2 = replEnvironment.HighlightInputLine(FirstLinePrefix, buffer.ToString(), lineBuffer, cursorOffset, printPrevLines: true);
            await io.Output.WriteAsync(PrintHelper.MoveCursorUp(prevBufferLineCount));
            await io.Output.WriteAsync(output2);
            return cursorOffset;
        }

        return -1;
    }

    private string GetPrefix(StringBuilder buffer)
        => buffer.Length == 0 ? FirstLinePrefix : "";

    private async Task<string?> ReadLine(StringBuilder buffer)
    {
        await io.Output.WriteAsync(GetPrefix(buffer));

        var lineBuffer = new List<Rune>();
        var cursorOffset = 0;
        while (true)
        {
            var keyInfo = Console.ReadKey(intercept: true);
            var nextChar = keyInfo.KeyChar;

            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                if (await PrintHistory(buffer, lineBuffer, cursorOffset, backwards: true) is { } newOffset and not -1)
                {
                    cursorOffset = newOffset;
                    continue;
                }
            }
            if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                if (await PrintHistory(buffer, lineBuffer, cursorOffset, backwards: false) is { } newOffset and not -1)
                {
                    cursorOffset = newOffset;
                    continue;
                }
            }
            if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                cursorOffset = Math.Max(cursorOffset - 1, 0);
            }
            if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                cursorOffset = Math.Min(cursorOffset + 1, lineBuffer.Count);
            }
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                await io.Output.FlushAsync();
                break;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (cursorOffset > 0 && cursorOffset <= lineBuffer.Count)
                {
                    lineBuffer.RemoveAt(cursorOffset - 1);
                    cursorOffset = Math.Max(cursorOffset - 1, 0);
                }
            }
            else if (keyInfo.Key == ConsoleKey.Delete)
            {
                if (cursorOffset < lineBuffer.Count)
                {
                    lineBuffer.RemoveAt(cursorOffset);
                }
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                return null;
            }
            else if (nextChar != 0)
            {
                lineBuffer.Insert(cursorOffset, ReadRune(nextChar));
                cursorOffset += 1;
            }

            var output = replEnvironment.HighlightInputLine(GetPrefix(buffer), buffer.ToString(), lineBuffer, cursorOffset, printPrevLines: false);
            await io.Output.WriteAsync(output);
        }

        await io.Output.WriteAsync("\n");

        return string.Concat(lineBuffer);
    }
}
