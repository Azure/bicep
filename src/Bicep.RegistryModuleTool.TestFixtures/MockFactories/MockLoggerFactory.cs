// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.TestFixtures.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bicep.RegistryModuleTool.TestFixtures.MockFactories
{
    public static class MockLoggerFactory
    {
        public static ILogger<T> CreateGenericLogger<T>()
        {
            var loggerMock = StrictMock.Of<ILogger<T>>();

            loggerMock
                .Setup(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
                .Verifiable();

            return loggerMock.Object;
        }

        public static ILogger CreateLogger() => CreateGenericLogger<It.IsAnyType>();
    }
}
