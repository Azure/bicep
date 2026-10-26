// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;

namespace Bicep.RegistryModuleTool.Extensions
{
    /// <summary>Local replacement for the removed System.CommandLine.IConsole interface.</summary>
    public interface IConsole
    {
        TextWriter Out { get; }
        TextWriter Error { get; }
    }

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
                    console.Out.WriteLine(message);
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

        public static void WriteWarning(this IConsole console, string warning) => WriteMessage(console.Error, ConsoleColor.Yellow, warning);

        public static void WriteError(this IConsole console, string error) => WriteMessage(console.Error, ConsoleColor.Red, error);

        private static void WriteMessage(TextWriter writer, ConsoleColor color, string message)
        {
            // Only apply color when writing to a real console stream.
            if (writer == Console.Out || writer == Console.Error)
            {
                var previous = Console.ForegroundColor;
                Console.ForegroundColor = color;
                writer.WriteLine(message);
                Console.ForegroundColor = previous;
            }
            else
            {
                writer.WriteLine(message);
            }
        }
    }
}
