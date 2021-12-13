// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.Core.UnitTests.Mock;
using Bicep.RegistryModuleTool.Proxies;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

namespace Bicep.RegistryModuleTool.UnitTests.Proxies
{
    [TestClass]
    public class BicepCliProxyTests
    {
        [TestMethod]
        public void Build_BicepCliNotFound_ThrowsException()
        {
            // Arrange.
            var sut = CreateBicepCliProxy();

            // Act & Assert.
            FluentActions.Invoking(() => sut.Build("/main.bicep", "/main.json")).Should()
                .Throw<BicepException>()
                .WithMessage("Failed to locate Bicep CLI. Please make sure to add Bicep CLI to PATH, or install it via Azure CLI.");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetEnvironmentAndFileSystemData), DynamicDataSourceType.Method)]
        public void Build_ValidBicepFile_DoesNotLogAnyWarningsAndErrors(IEnvironmentProxy environmentProxy, IFileSystem fileSystem)
        {
            // Arrange.
            var processProxyMock = StrictMock.Of<IProcessProxy>();
            processProxyMock.Setup(x => x.Start(It.IsAny<string>(), It.IsAny<string>())).Returns((0, "", ""));

            var mockLogger = MockGenericLogger<It.IsAnyType>.Create();

            var sut = CreateBicepCliProxy(environmentProxy, processProxyMock.Object, fileSystem, mockLogger);

            // Act.
            sut.Build("/main.bicep", "main.json");

            // Assert.
            mockLogger.VerifyLogWarning(It.IsAny<string>(), Times.Never());
            mockLogger.VerifyLogError(It.IsAny<string>(), Times.Never());
        }

        [DataTestMethod]
        [DynamicData(nameof(GetEnvironmentAndFileSystemData), DynamicDataSourceType.Method)]
        public void Build_BicepFileWithWarnings_LogsWarnings(IEnvironmentProxy environmentProxy, IFileSystem fileSystem)
        {
            // Arrange.
            var processProxyMock = StrictMock.Of<IProcessProxy>();
            processProxyMock.Setup(x => x.Start(It.IsAny<string>(), It.IsAny<string>())).Returns((0, "", "warning message"));

            var mockLogger = MockGenericLogger<It.IsAnyType>.Create();

            var sut = CreateBicepCliProxy(environmentProxy, processProxyMock.Object, fileSystem, mockLogger);

            // Act.
            sut.Build("/main.bicep", "main.json");

            // Assert.
            mockLogger.VerifyLogWarning("warning message");
            mockLogger.VerifyLogError(It.IsAny<string>(), Times.Never());
        }

        [DataTestMethod]
        [DynamicData(nameof(GetEnvironmentAndFileSystemData), DynamicDataSourceType.Method)]
        public void Build_BicepFileWithWarningsAndErrors_LogsWarningAndErrorAndThrowsException(IEnvironmentProxy environmentProxy, IFileSystem fileSystem)
        {
            // Arrange.

            var warnings  = new[]
            {
                @"c:\main.bicep(43,5) : Warning BCP037: The property ""extra"" is not allowed on objects of type ""ManagedClusterProperties"". Permissible properties include ""aadProfile"", ""addonProfiles"", ""apiServerAccessProfile"", ""autoScalerProfile"", ""diskEncryptionSetID"", ""enablePodSecurityPolicy"", ""enableRBAC"", ""identityProfile"", ""kubernetesVersion"", ""networkProfile"", ""nodeResourceGroup"", ""windowsProfile"". If this is an inaccuracy in the documentation, please report it to the Bicep Team. [https://aka.ms/bicep-type-issues]",
                @"c:\main.bicep(56,7) : Warning BCP037: The property ""addtional"" is not allowed on objects of type ""ContainerServiceLinuxProfile"". No other properties are allowed. If this is an inaccuracy in the documentation, please report it to the Bicep Team. [https://aka.ms/bicep-type-issues]",
            };

            var errors = new[]
            {
                @"c:\main.bicep(3,23) : Error BCP008: Expected the ""="" token, or a newline at this location.",
                @"c:\main.bicep(28,26) : Error BCP027: The parameter expects a default value of type ""int"" but provided value is of type ""bool"".",
            };

            var processProxyMock = StrictMock.Of<IProcessProxy>();
            processProxyMock.Setup(x => x.Start(It.IsAny<string>(), It.IsAny<string>())).Returns((1, "", string.Join('\n', warnings.Concat(errors))));

            var mockLogger = MockGenericLogger<It.IsAnyType>.Create();

            var sut = CreateBicepCliProxy(environmentProxy, processProxyMock.Object, fileSystem, mockLogger);

            // Act.
            FluentActions.Invoking(() => sut.Build("/main.bicep", "/main.json")).Should()
                .Throw<BicepException>()
                .WithMessage("Failed to build \"/main.bicep\".");

            // Assert.
            mockLogger.VerifyLogWarning(string.Join(Environment.NewLine, warnings));
            mockLogger.VerifyLogError(string.Join(Environment.NewLine, errors));
        }

        private static BicepCliProxy CreateBicepCliProxy(
            IEnvironmentProxy? environmentProxy = null,
            IProcessProxy? processProxy = null,
            IFileSystem? fileSystem = null,
            ILogger? logger = null)
        {
            environmentProxy ??= new MockEnvironmentProxy();
            processProxy ??= StrictMock.Of<IProcessProxy>().Object;
            fileSystem ??= new MockFileSystem();
            logger ??= MockGenericLogger<It.IsAnyType>.Create();

            return new(environmentProxy, processProxy, fileSystem, logger);
        }

        private static IEnumerable<object[]> GetEnvironmentAndFileSystemData()
        {
            yield return new object[]
            {
                new MockEnvironmentProxy(new Dictionary<string, string>
                {
                    ["PATH"] = "/bin",
                }),
                new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    ["/bin/bicep"] = "",
                    ["/bin/bicep.exe"] = "",
                    ["/main.bicep"] = ""
                }),
            };

            yield return new object[]
            {
                new MockEnvironmentProxy(new Dictionary<string, string>
                {
                    ["AZURE_CONFIG_DIR"] = "/.azure",
                }),
                new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    ["/.azure/bin/bicep"] = "",
                    ["/.azure/bin/bicep.exe"] = "",
                    ["/main.bicep"] = ""
                }),
            };

            yield return new object[]
            {
                new MockEnvironmentProxy(),
                new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    ["/home/.azure/bin/bicep"] = "",
                    ["/home/.azure/bin/bicep.exe"] = "",
                    ["/main.bicep"] = ""
                }),
            };
        }
    }
}
