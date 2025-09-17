// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers.Repl;
using Bicep.Cli.Services;
using Bicep.Core;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Commands;

/// <summary>
/// Minimal interactive console (experimental).
/// Currently just echoes input lines verbatim (except for exit / quit commands).
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
        await io.Output.WriteLineAsync(string.Empty);

        while (true)
        {
            await io.Output.WriteAsync("> ");
            var rawLine = Console.ReadLine();
            if (rawLine is null)
            {
                break; // EOF
            }

            var trimmed = rawLine.Trim();

            if (trimmed.Length == 0)
            {
                continue; // ignore empty lines
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

            // evaluate input
            var result = await replEnvironment.EvaluateInput(rawLine);

            if (result.Value is { } value)
            {
                await io.Output.WriteLineAsync(value.ToString());
            }
            else if (result.AnnotatedDiagnostics.Any())
            {
                await io.Output.WriteLineAsync(PrintHelper.PrintWithAnnotations(rawLine, result.AnnotatedDiagnostics));
            }
        }

        return 0;
    }
}
