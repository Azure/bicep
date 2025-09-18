// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Text;
using Bicep.Cli.Arguments;
using Bicep.Cli.Services;
using Bicep.Core;
using Microsoft.Extensions.Logging;

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
            await io.Output.WriteAsync(buffer.Length == 0 ? "> " : ". ");
            var rawLine = Console.ReadLine();
            if (rawLine is null)
            {
                break; // EOF
            }

            // Only treat commands when buffer empty (except :reset)
            var trimmed = rawLine.Trim();

            if (buffer.Length == 0)
            {
                if (trimmed.Length == 0)
                {
                    continue; // ignore empty standalone line
                }

                if (trimmed.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (trimmed.Equals("clear", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Clear();
                    continue;
                }

                if (trimmed.Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    await io.Output.WriteLineAsync("Enter expressions or 'var name = <expr>'. Multi-line supported until structure closes.");
                    await io.Output.WriteLineAsync("Commands: exit, clear, :reset");
                    continue;
                }
            }

            if (trimmed.Equals(":reset", StringComparison.OrdinalIgnoreCase))
            {
                buffer.Clear();
                continue;
            }

            // Accumulate line
            if (buffer.Length > 0)
            {
                buffer.AppendLine();
            }
            buffer.Append(rawLine);

            var current = buffer.ToString();

            // If line is blank AND structurally complete -> submit
            if (trimmed.Length == 0 && IsInputStructurallyComplete(current))
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

        var result = await replEnvironment.EvaluateInput(current);
        if (result.Diagnostics.Any())
        {
            foreach (var diag in result.Diagnostics)
            {
                await io.Output.WriteLineAsync(diag.Message);
            }
        }
        else if (result.AnnotatedDiagnostics is { } annotatedDiagnostic)
        {
            await io.Output.WriteLineAsync(annotatedDiagnostic.Diagnostic);
        }
        else if (result.Value is { } value)
        {
            await io.Output.WriteLineAsync(value.ToString());
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
}
