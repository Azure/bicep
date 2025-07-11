// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Logging;

public class TestContextLoggerProvider(TestContext testContext) : ILoggerProvider
{
    private class TestContextLogger(TestContext testContext) : ILogger, IDisposable
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            => this;

        public void Dispose()
        {
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => testContext.WriteLine($"[{logLevel}] {formatter(state, exception)}");
    }

    public ILogger CreateLogger(string categoryName)
        => new TestContextLogger(testContext);

    public void Dispose() { }
}
