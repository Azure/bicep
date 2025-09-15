// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Arguments;
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

    public ConsoleCommand(IOContext io)
    {
        this.io = io;
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
            if (trimmed.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            await io.Output.WriteLineAsync(rawLine);
        }

        return 0;
    }
}
