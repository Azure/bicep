// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Logging
{
    /// <summary>
    /// Logger provider for writing log entries in an msbuild compatible format.
    /// </summary>
    public class BicepLoggerProvider : ILoggerProvider
    {
        private readonly BicepLoggerOptions options;

        public BicepLoggerProvider(BicepLoggerOptions options)
        {
            this.options = options;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new BicepConsoleLogger(this.options);
        }
    }
}

