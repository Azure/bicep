// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Bicep.VSLanguageServerClient.Logging
{
    internal class LogHubLoggerProvider : ILoggerProvider
    {
        // Internal for testing
        internal const string OmniSharpFrameworkCategoryPrefix = "OmniSharp.Extensions.LanguageServer.Server";

        private readonly LogHubLogWriter _logWriter;
        private readonly ILogger _noopLogger;

        public LogHubLoggerProvider(LogHubLogWriter logWriter)
        {
            if (logWriter is null)
            {
                throw new ArgumentNullException(nameof(logWriter));
            }

            _logWriter = logWriter;
            _noopLogger = new NoopLogger();
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (categoryName is null)
            {
                throw new ArgumentNullException(nameof(categoryName));
            }

            if (categoryName.StartsWith(OmniSharpFrameworkCategoryPrefix, StringComparison.Ordinal))
            {
                // Loggers created for O# framework pieces should be ignored. They emit too much noise.
                return _noopLogger;
            }

            return new LogHubLogger(categoryName, _logWriter);
        }

        public TraceSource GetTraceSource() => _logWriter.GetTraceSource();

        public void Dispose()
        {
        }

        private class NoopLogger : ILogger
        {
            public IDisposable BeginScope<TState>(TState state) => Scope.Instance;

            public bool IsEnabled(LogLevel logLevel) => false;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
            }

            private class Scope : IDisposable
            {
                public static readonly Scope Instance = new();

                public void Dispose()
                {
                }
            }
        }
    }
}
