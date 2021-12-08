// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.Core.UnitTests.Mock;
using Bicep.RegistryModuleTool.Proxies;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.UnitTests.Proxies
{
    [TestClass]
    public class BicepCliProxyTests
    {
        [TestMethod]
        public void Build_BicepCliNotFound_ThrowsBicepException()
        {
            var mockFileSystem = new MockFileSystem();

            var environmentProxyMock = StrictMock.Of<IEnvironmentProxy>();
            environmentProxyMock.Setup(x => x.GetEnvironmentVariable("PATH")).Returns("/bin");
            environmentProxyMock.Setup(x => x.GetEnvironmentVariable("AZURE_CONFIG_DIR")).Returns<string?>(null);
            environmentProxyMock.Setup(x => x.GetHomeDirectory()).Returns("/user/foo");

            var processProxyMock = StrictMock.Of<IProcessProxy>();

            var loggerMock = StrictMock.Of<ILogger>();
            loggerMock.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

            var sut = new BicepCliProxy(environmentProxyMock.Object, processProxyMock.Object, mockFileSystem, loggerMock.Object);

            FluentActions.Invoking(() => sut.Build("/main.bicep")).Should()
                .Throw<BicepException>()
                .WithMessage("Failed to locate Bicep CLI. Please make sure to add Bicep CLI to PATH, or install it via Azure CLI.");
        }
    }
}
