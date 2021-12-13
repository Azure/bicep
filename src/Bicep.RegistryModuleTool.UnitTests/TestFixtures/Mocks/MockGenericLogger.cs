// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Mock;
using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace Bicep.RegistryModuleTool.UnitTests.TestFixtures.Mocks
{
    public class MockGenericLogger<T> : ILogger<T>
    {
        private readonly Mock<ILogger<T>> loggerMock;

        public MockGenericLogger(Mock<ILogger<T>> loggerMock)
        {
            this.loggerMock = loggerMock;
        }

        public static MockGenericLogger<T> Create()
        {
            var loggerMock = StrictMock.Of<ILogger<T>>();

            loggerMock
                .Setup(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<T>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<T, Exception?, string>>()))
                .Verifiable();

            return new(loggerMock);
        }

        public IDisposable BeginScope<TState>(TState state) => loggerMock.Object.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel) => loggerMock.Object.IsEnabled(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            loggerMock.Object.Log(logLevel, eventId, state, exception, formatter);

        public void VerifyLogDebug(string expectedMessage) => this.VerifyLogDebug(expectedMessage, Times.Once());

        public void VerifyLogDebug(string expectedMessage, Times times)
        {
            Func<object, Type, bool> state = (v, t) => v.ToString()?.CompareTo(expectedMessage) == 0;

            this.loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Debug),
                    It.IsAny<EventId>(),
                    It.Is<T>((v, t) => state(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<T, Exception?, string>>((v, t) => true)),
                times);
        }

        public void VerifyLogWarning(string expectedMessage) => this.VerifyLogWarning(expectedMessage, Times.Once());

        public void VerifyLogWarning(string expectedMessage, Times times)
        {
            Func<object, Type, bool> state = (v, t) => v?.ToString()?.CompareTo(expectedMessage) == 0;

            this.loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<T>((v, t) => state(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<T, Exception?, string>>((v, t) => true)),
                times);
        }

        public void VerifyLogError(string expectedMessage) => this.VerifyLogError(expectedMessage, Times.Once());

        public void VerifyLogError(string expectedMessage, Times times)
        {
            Func<object, Type, bool> state = (v, t) => v?.ToString()?.CompareTo(expectedMessage) == 0;

            this.loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<T>((v, t) => state(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<T, Exception?, string>>((v, t) => true)),
                times);
        }
    }
}
