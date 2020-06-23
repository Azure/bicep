using System;

namespace Bicep.Cli.Logging
{
    public class BicepLoggerOptions
    {
        public BicepLoggerOptions(bool enableColors, ConsoleColor errorColor, ConsoleColor warningColor)
        {
            this.EnableColors = enableColors;
            this.ErrorColor = errorColor;
            this.WarningColor = warningColor;
        }

        public bool EnableColors { get; }

        public ConsoleColor ErrorColor { get; }

        public ConsoleColor WarningColor { get; }
    }
}