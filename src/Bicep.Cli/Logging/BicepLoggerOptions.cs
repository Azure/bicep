// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;

namespace Bicep.Cli.Logging
{
    public class BicepLoggerOptions
    {
        public BicepLoggerOptions(bool enableColors, ConsoleColor errorColor, ConsoleColor warningColor, TextWriter writer)
        {
            this.EnableColors = enableColors;
            this.ErrorColor = errorColor;
            this.WarningColor = warningColor;
            this.Writer = writer;
        }

        public bool EnableColors { get; }

        public ConsoleColor ErrorColor { get; }

        public ConsoleColor WarningColor { get; }

        public TextWriter Writer { get; }
    }
}
