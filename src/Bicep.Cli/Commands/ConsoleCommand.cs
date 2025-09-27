// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Text;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers.Repl;
using Bicep.Cli.Services;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Syntax;
using Microsoft.Diagnostics.Tracing.StackSources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Commands;

/// <summary>
/// Minimal interactive console (experimental).
/// Supports multi-line input: enter expressions or variable declarations.
/// Input is submitted automatically when structurally complete, or by entering a blank line once complete.
/// Type :reset to discard the current multi-line buffer.
/// </summary>
public class ConsoleCommand : ICommand
{
    private readonly IOContext io;
    private readonly ReplEnvironment replEnvironment;

    public ConsoleCommand(IOContext io, ReplEnvironment replEnvironment)
    {
        this.io = io;
        this.replEnvironment = replEnvironment;
    }

    public async Task<int> RunAsync(ConsoleArguments _)
    {
        await io.Output.WriteLineAsync("Bicep Console v1.0.0");
        await io.Output.WriteLineAsync("Type 'help' for available commands, 'exit' to quit.");
        await io.Output.WriteLineAsync("Multi-line input supported. Use :reset to clear current buffer.");
        await io.Output.WriteLineAsync(string.Empty);

        var buffer = new StringBuilder();

        while (true)
        {
            if (buffer.Length == 0)
            {
                await io.Output.WriteAsync("> ");
            }

            var lineBuffer = new StringBuilder();
            var cursorOffset = 0;
            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);
                var nextChar = keyInfo.KeyChar;

                var prevCursorOffset = cursorOffset;

                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    cursorOffset = Math.Max(cursorOffset - 1, 0);
                }
                if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    cursorOffset = Math.Min(cursorOffset + 1, lineBuffer.Length);
                }
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    await io.Output.FlushAsync();
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (cursorOffset > 0 && cursorOffset <= lineBuffer.Length)
                    {
                        lineBuffer.Remove(cursorOffset - 1, 1);
                        cursorOffset = Math.Max(cursorOffset - 1, 0);
                    }
                }
                else if (nextChar != 0)
                {
                    lineBuffer.Insert(cursorOffset, nextChar);
                    cursorOffset += 1;
                }

                // Reprint line with highlighting
                var highlighted = await replEnvironment.HighlightInputLine(buffer.ToString(), lineBuffer.ToString());

                // Hide cursor
                await io.Output.WriteAsync("\u001b[?25l");

                if (prevCursorOffset > 0)
                {
                    // Move cursor to start of line
                    await io.Output.WriteAsync("\u001b[" + prevCursorOffset + "D");
                }

                // Clear from cursor to end of line
                await io.Output.WriteAsync("\u001b[0J");

                // Write the line
                await io.Output.WriteAsync(highlighted);

                if (lineBuffer.Length - cursorOffset > 0)
                {
                    // Move cursor back to correct position
                    await io.Output.WriteAsync("\u001b[" + (lineBuffer.Length - cursorOffset) + "D");
                }

                // Show cursor
                await io.Output.WriteAsync("\u001b[?25h");
            }

            await io.Output.WriteAsync("\n");

            var rawLine = lineBuffer.ToString();

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

            buffer.AppendLine(rawLine);

            var current = buffer.ToString();

            // If line is blank -> submit
            if (rawLine.Length == 0)
            {
                await SubmitBuffer(buffer, current);
                continue;
            }

            // Auto-submit when structure complete immediately after this line.
            if (IsInputStructurallyComplete(current))
            {
                await SubmitBuffer(buffer, current);
            }
        }

        return 0;
    }

    private async Task SubmitBuffer(StringBuilder buffer, string current)
    {
        if (current.Trim().Length == 0)
        {
            buffer.Clear();
            return;
        }

        // evaluate input
        var result = await replEnvironment.EvaluateInput(current);

        if (result.Value is { } value)
        {
            var context = PrettyPrinterV2Context.Create(PrettyPrinterV2Options.Default, EmptyDiagnosticLookup.Instance, EmptyDiagnosticLookup.Instance);
            var lineText = PrettyPrinterV2.Print(ParseJToken(value), context);

            var highlighted = await replEnvironment.HighlightInputLine("", lineText);

            await io.Output.WriteLineAsync(highlighted);
        }
        else if (result.AnnotatedDiagnostics.Any())
        {
            await io.Output.WriteLineAsync(PrintHelper.PrintWithAnnotations(current, result.AnnotatedDiagnostics));
        }
        buffer.Clear();
    }

    /// <summary>
    /// Heuristic structural completeness check: ensures bracket/brace/paren balance
    /// and not inside (multi-line) string or interpolation expression.
    /// Not a full parse; parse errors still reported by real parser.
    /// </summary>
    private static bool IsInputStructurallyComplete(string text)
    {
        int openCurly = 0, openSquare = 0, openParen = 0;
        bool inString = false; // single or multi-line (same delimiter logic)
        bool inMultiline = false;
        int interpolationDepth = 0; // inside ${ ... } within string
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            // Handle single-line comments // ... (ignore content until newline)
            if (!inString && c == '/' && i + 1 < text.Length && text[i + 1] == '/')
            {
                // skip until newline or end
                i += 2;
                while (i < text.Length && text[i] != '\n')
                {
                    i++;
                }
                continue;
            }

            if (!inString)
            {
                // Start (multi-line) string detection: ''' or single '
                if (c == '\'')
                {
                    if (i + 2 < text.Length && text[i + 1] == '\'' && text[i + 2] == '\'')
                    {
                        inString = true; inMultiline = true; i += 2; // skip the two extra quotes
                        continue;
                    }
                    inString = true; inMultiline = false; continue;
                }

                switch (c)
                {
                    case '{':
                        openCurly++;
                        break;
                    case '}':
                        if (openCurly > 0)
                        {
                            openCurly--;
                        }
                        break;
                    case '[':
                        openSquare++;
                        break;
                    case ']':
                        if (openSquare > 0)
                        {
                            openSquare--;
                        }
                        break;
                    case '(':
                        openParen++;
                        break;
                    case ')':
                        if (openParen > 0)
                        {
                            openParen--;
                        }
                        break;
                }
            }
            else // inside string (maybe interpolation)
            {
                if (!inMultiline && c == '\n')
                {
                    // normal single quoted string cannot span lines; treat newline as termination safeguard
                    // (actual parser will raise a diagnostic); we end early to avoid blocking user.
                    inString = false; interpolationDepth = 0; continue;
                }

                if (c == '$' && i + 1 < text.Length && text[i + 1] == '{')
                {
                    interpolationDepth++; i++; continue;
                }
                if (c == '{' && interpolationDepth > 0)
                {
                    interpolationDepth++; continue;
                }
                if (c == '}' && interpolationDepth > 0)
                {
                    interpolationDepth--; continue;
                }

                // End of string
                if (c == '\'')
                {
                    if (inMultiline)
                    {
                        // need '''
                        if (i + 2 < text.Length && text[i + 1] == '\'' && text[i + 2] == '\'')
                        {
                            inString = false; inMultiline = false; interpolationDepth = 0; i += 2; continue;
                        }
                    }
                    else if (interpolationDepth == 0)
                    {
                        inString = false; interpolationDepth = 0; continue;
                    }
                }
            }
        }

        return openCurly == 0 && openSquare == 0 && openParen == 0 && !inString && interpolationDepth == 0;
    }

    private static SyntaxBase ParseJToken(JToken value)
        => value switch {
            JObject jObject => ParseJObject(jObject),
            JArray jArray => ParseJArray(jArray),
            JValue jValue => ParseJValue(jValue),
            _ => throw new NotImplementedException($"Unrecognized token type {value.Type}"),
        };

    private static SyntaxBase ParseJValue(JValue value)
        => value.Type switch {
            JTokenType.Integer => SyntaxFactory.CreatePositiveOrNegativeInteger(value.Value<long>()),
            JTokenType.String => SyntaxFactory.CreateStringLiteral(value.ToString()),
            JTokenType.Boolean => SyntaxFactory.CreateBooleanLiteral(value.Value<bool>()),
            JTokenType.Null => SyntaxFactory.CreateNullLiteral(),
            _ => throw new NotImplementedException($"Unrecognized token type {value.Type}"),
        };

    private static SyntaxBase ParseJArray(JArray jArray)
        => SyntaxFactory.CreateArray(
            jArray.Select(ParseJToken));

    private static SyntaxBase ParseJObject(JObject jObject)
        => SyntaxFactory.CreateObject(
            jObject.Properties()
                .Select(x => SyntaxFactory.CreateObjectProperty(x.Name, ParseJToken(x.Value))));
}
