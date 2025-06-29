// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Rendering;
using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;

namespace Bicep.RegistryModuleTool.Extensions
{
    public static class ConsoleExtensions
    {
        public static void WriteDiagnostic(this IConsole console, BicepSourceFile file, IDiagnostic diagnostic)
        {
            (int line, int character) = TextCoordinateConverter.GetPosition(file.LineStarts, diagnostic.Span.Position);
            var codeDescription = diagnostic.Uri == null ? string.Empty : $" [{diagnostic.Uri.AbsoluteUri}]";
            var message = $"{file.FileHandle.Uri}({line + 1},{character + 1}) : {diagnostic.Level} {diagnostic.Code}: {diagnostic.Message}{codeDescription}";

            switch (diagnostic.Level)
            {
                case DiagnosticLevel.Off:
                    break;
                case DiagnosticLevel.Info:
                    console.WriteLine(message);
                    break;
                case DiagnosticLevel.Warning:
                    console.WriteWarning(message);
                    break;
                case DiagnosticLevel.Error:
                    console.WriteError(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown diagnostic level: {diagnostic.Level}.");
            }
        }

        public static void WriteWarning(this IConsole console, string warning) => console.WriteMessage(console.Out, ConsoleColor.Yellow, warning);

        public static void WriteError(this IConsole console, string error) => console.WriteMessage(console.Error, ConsoleColor.Red, error);

        private static void WriteMessage(this IConsole console, IStandardStreamWriter writer, ConsoleColor color, string message)
        {
            var terminal = console.GetTerminal(preferVirtualTerminal: false);
            var originalForegroundColor = terminal?.ForegroundColor ?? Console.ForegroundColor;

            if (terminal is not null)
            {
                terminal.ForegroundColor = color;
            }

            writer.WriteLine(message);

            if (terminal is not null)
            {
                terminal.ForegroundColor = originalForegroundColor;
            }
        }
    }
}
