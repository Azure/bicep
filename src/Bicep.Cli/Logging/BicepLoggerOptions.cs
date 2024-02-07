// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Logging
{
    public class BicepLoggerOptions(bool enableColors, ConsoleColor errorColor, ConsoleColor warningColor, TextWriter writer)
    {
        public bool EnableColors { get; } = enableColors;

        public ConsoleColor ErrorColor { get; } = errorColor;

        public ConsoleColor WarningColor { get; } = warningColor;

        public TextWriter Writer { get; } = writer;
    }
}
