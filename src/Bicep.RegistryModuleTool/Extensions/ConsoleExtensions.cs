// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Rendering;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class ConsoleExtensions
    {
        public static void WriteWarning(this IConsole console, string warning) => console.WriteMessage(console.Out, ConsoleColor.Yellow, warning);

        public static void WriteError(this IConsole console, string error) => console.WriteMessage(console.Error, ConsoleColor.Red, error);

        private static void WriteMessage(this IConsole console, IStandardStreamWriter writer, ConsoleColor color, string messsage)
        {
            var terminal = console.GetTerminal(preferVirtualTerminal: false);
            var originalForegroundColor = terminal?.ForegroundColor ??  Console.ForegroundColor;

            if (terminal is not null)
            {
                terminal.ForegroundColor = color;
            }

            writer.WriteLine(messsage);

            if (terminal is not null)
            {
                terminal.ForegroundColor = originalForegroundColor;
            }
        }
    }
}
