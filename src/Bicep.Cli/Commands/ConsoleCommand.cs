// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.Text;
using Bicep.Cli.Arguments;
using Bicep.Cli.Constants;
using Bicep.Cli.Helpers.Repl;
using Bicep.Cli.Services;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using static System.ConsoleKey;

namespace Bicep.Cli.Commands;

/// <summary>
/// Minimal interactive console
/// Supports multi-line input: enter expressions or variable declarations.
/// Input is submitted automatically when structurally complete, or by entering a blank line once complete.
/// </summary>
public class ConsoleCommand(
    IOContext io,
    IEnvironment environment,
    ReplEnvironment replEnvironment) : ICommand
{
    private const string FirstLinePrefix = "> ";

    private Rune ReadRune(char firstChar)
        => ReadRune(firstChar, () => Console.ReadKey(intercept: true).KeyChar);

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(Constants.Command.Console, "Opens an interactive Bicep console.");

        command.SetAction((result, ct) => context.RunCommandAsync(
            () => context.GetCommand<ConsoleCommand>().RunAsync(new ConsoleArguments())));

        return command;
    }

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
        if (io.Input.IsRedirected)
        {
            // Read all input from stdin if redirected (via pipe or file redirection)
            var input = await io.Input.Reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            // Handle input line by line (to support multi-line strings)
            var outputBuilder = new StringBuilder();
            var inputBuffer = new StringBuilder();
            var pendingTypeContinuationRedirected = false;

            using var reader = new StringReader(input);
            while (await reader.ReadLineAsync() is { } line)
            {
                if (pendingTypeContinuationRedirected && !IsTypeContinuationLine(line))
                {
                    outputBuilder.Append(EvaluateAndClearBuffer(inputBuffer));
                    pendingTypeContinuationRedirected = false;

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                }

                inputBuffer.Append(line);
                inputBuffer.Append('\n');

                var current = inputBuffer.ToString();
                var bufferState = ReplEnvironment.GetBufferState(current, line);
                if (bufferState.ShouldSubmit)
                {
                    if (bufferState.IsTypeDeclaration)
                    {
                        pendingTypeContinuationRedirected = true;
                    }
                    else
                    {
                        outputBuilder.Append(EvaluateAndClearBuffer(inputBuffer));
                        pendingTypeContinuationRedirected = false;
                    }
                }
            }

            if (inputBuffer.Length > 0)
            {
                outputBuilder.Append(EvaluateAndClearBuffer(inputBuffer));
            }

            var output = outputBuilder.ToString();
            if (io.Output.IsRedirected)
            {
                output = AnsiHelper.RemoveCodes(output);
            }
            await io.Output.Writer.WriteAsync(output);
            return 0;
        }

        await io.Output.Writer.WriteLineAsync($"Bicep Console version {environment.GetVersionString()}");
        await io.Output.Writer.WriteLineAsync("Type 'help' for available commands, press ESC or CTRL+C to quit.");
        await io.Output.Writer.WriteLineAsync("Multi-line input supported.");
        await io.Output.Writer.WriteLineAsync(string.Empty);

        Console.TreatControlCAsInput = true;

        var buffer = new StringBuilder();
        var pendingTypeContinuation = false;

        while (true)
        {
            if (await ReadLine(buffer) is not { } inputLine)
            {
                break;
            }

            var rawLine = inputLine.Text;
            if (pendingTypeContinuation && !IsTypeContinuationLine(rawLine))
            {
                await io.Output.Writer.WriteAsync(EvaluateAndClearBuffer(buffer));
                pendingTypeContinuation = false;

                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    continue;
                }
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
                    await io.Output.Writer.WriteLineAsync("Enter expressions or 'var name = <expr>'. Multi-line supported until structure closes.");
                    await io.Output.Writer.WriteLineAsync("Commands: exit, clear");
                    continue;
                }
            }

            buffer.Append(rawLine);
            buffer.Append('\n');

            var current = buffer.ToString();

            var bufferState = ReplEnvironment.GetBufferState(current, rawLine);
            if (bufferState.ShouldSubmit)
            {
                if (bufferState.IsTypeDeclaration && ShouldWaitForTypeContinuation(inputLine))
                {
                    pendingTypeContinuation = true;
                }
                else
                {
                    pendingTypeContinuation = false;
                    await io.Output.Writer.WriteAsync(EvaluateAndClearBuffer(buffer));
                }
            }
        }

        return 0;
    }

    private string EvaluateAndClearBuffer(StringBuilder buffer)
    {
        var current = buffer.ToString();
        buffer.Clear();

        return replEnvironment.EvaluateAndGetOutput(current);
    }

    private static bool IsTypeContinuationLine(string line)
        => line.TrimStart().StartsWith('|');

    private static bool ShouldWaitForTypeContinuation(InputLine inputLine)
        => inputLine.HasBufferedInputAfterEnter ||
            (IsTypeContinuationLine(inputLine.Text) && !inputLine.StartedWithBufferedInput);

    private readonly record struct InputLine(string Text, bool StartedWithBufferedInput, bool HasBufferedInputAfterEnter);

    private async Task<bool> PrintHistory(StringBuilder buffer, LineEditor editor, bool backwards)
    {
        if (replEnvironment.TryGetHistory(backwards) is not { } history)
        {
            return false;
        }

        var prevBufferLineCount = buffer.ToString().Count(x => x == '\n');
        buffer.Clear();

        var finalNewline = history.LastIndexOf('\n');
        var lineStart = finalNewline > -1 ? finalNewline + 1 : 0;

        buffer.Append(history[..lineStart]);
        editor.Reset(GetRunes(history[lineStart..]));

        var output = replEnvironment.HighlightInputLine(FirstLinePrefix, buffer.ToString(), editor.Buffer, editor.Cursor, printPrevLines: true);
        await io.Output.Writer.WriteAsync(PrintHelper.MoveCursorUp(prevBufferLineCount));
        await io.Output.Writer.WriteAsync(output);
        return true;
    }

    private string GetPrefix(StringBuilder buffer)
        => buffer.Length == 0 ? FirstLinePrefix : "";

    private async Task<InputLine?> ReadLine(StringBuilder buffer)
    {
        await io.Output.Writer.WriteAsync(GetPrefix(buffer));

        var editor = new LineEditor();
        var startedWithBufferedInput = Console.KeyAvailable;

        while (true)
        {
            var keyInfo = Console.ReadKey(intercept: true);

            switch ((keyInfo.Modifiers, keyInfo.Key))
            {
                case (_, UpArrow) or (_, DownArrow):
                    // History navigation re-renders the whole line itself, so skip the redraw below when it handled the key.
                    if (await PrintHistory(buffer, editor, backwards: keyInfo.Key == UpArrow))
                    {
                        continue;
                    }
                    break;

                // referenced from how NodeJS REPL handles jumping between words for MacOS:
                // https://github.com/nodejs/node/blob/0e2126d8b1c0eb93b105ac53c0939908392cbb42/lib/internal/readline/interface.js#L1435-L1445
                // Option key in MacOS maps to Alt
                case (ConsoleModifiers.Control, LeftArrow) or (ConsoleModifiers.Alt, B):
                    editor.MoveToWordBoundary(-1);
                    break;

                case (_, LeftArrow):
                    editor.MoveLeft();
                    break;

                // referenced from how NodeJS REPL handles jumping between words for MacOS:
                // https://github.com/nodejs/node/blob/0e2126d8b1c0eb93b105ac53c0939908392cbb42/lib/internal/readline/interface.js#L1435-L1445
                // Option key in MacOS maps to Alt
                case (ConsoleModifiers.Control, RightArrow) or (ConsoleModifiers.Alt, F): 
                    editor.MoveToWordBoundary(+1);
                    break;
                    
                case (_, RightArrow):
                    editor.MoveRight();
                    break;

                case (_, Home):
                    editor.MoveToStart();
                    break;

                case (_, End):
                    editor.MoveToEnd();
                    break;

                case (_, Enter):
                    await io.Output.Writer.FlushAsync();
                    await io.Output.Writer.WriteAsync("\n");
                    return new(string.Concat(editor.Buffer), startedWithBufferedInput, Console.KeyAvailable);

                case (_, Backspace):
                    editor.Backspace();
                    break;

                case (_, Delete):
                    editor.Delete();
                    break;

                case (ConsoleModifiers.Control, Z):
                    editor.Undo();
                    break;

                case (ConsoleModifiers.Control, Y):
                    editor.Redo();
                    break;

                case (_, Escape):
                    return null;

                case (ConsoleModifiers.Control, C):
                    return null;

                default:
                    if (keyInfo.KeyChar != 0)
                    {
                        editor.Insert(ReadRune(keyInfo.KeyChar));
                    }
                    break;
            }

            editor.Track();

            await io.Output.Writer.WriteAsync(
                replEnvironment.HighlightInputLine(GetPrefix(buffer), buffer.ToString(), editor.Buffer, editor.Cursor, printPrevLines: false));
        }
    }
}
