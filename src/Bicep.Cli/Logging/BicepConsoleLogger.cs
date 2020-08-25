// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Logging
{
    /// <summary>
    /// Writes messages to the console.
    /// </summary>
    /// <remarks>The built-in dotnet ConsoleLogger class does not write messages in the format we needed for a compiler and does not allow customization.</remarks>
    public class BicepConsoleLogger : ILogger
    {
        private readonly BicepLoggerOptions options;

        public BicepConsoleLogger(BicepLoggerOptions options)
        {
            this.options = options;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);

            if (this.options.EnableColors)
            {
                this.LogMessageWithColors(logLevel, message);
            }
            else
            {
                this.LogMessage(logLevel, message);
            }
        }

        private void LogMessageWithColors(LogLevel logLevel, string message)
        {
            var color = logLevel switch
            {
                LogLevel.Critical => this.options.ErrorColor,
                LogLevel.Error => this.options.ErrorColor,
                LogLevel.Warning => this.options.WarningColor,
                _ => Console.ForegroundColor
            };

            ConsoleColor saved = Console.ForegroundColor;
            Console.ForegroundColor = color;

            this.LogMessage(logLevel, message);

            Console.ForegroundColor = saved;
        }

        private void LogMessage(LogLevel logLevel, string message)
        {
            this.options.Writer.WriteLine(message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Error:
                case LogLevel.Critical:
                case LogLevel.Information:
                case LogLevel.Warning:
                    return true;

                default:
                    return false;
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NullDisposable();
        }

        private class NullDisposable: IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
