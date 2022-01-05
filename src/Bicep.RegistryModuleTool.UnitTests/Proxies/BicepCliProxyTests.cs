// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.Proxies;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using Bicep.RegistryModuleTool.TestFixtures.Mocks;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

namespace Bicep.RegistryModuleTool.UnitTests.Proxies
{
    [TestClass]
    public class BicepCliProxyTests
    {
        private static readonly string[] SampleWarnings = new[]
        {
            @"c:\main.bicep(43,5) : Warning BCP037: The property ""extra"" is not allowed on objects of type ""ManagedClusterProperties"". Permissible properties include ""aadProfile"", ""addonProfiles"", ""apiServerAccessProfile"", ""autoScalerProfile"", ""diskEncryptionSetID"", ""enablePodSecurityPolicy"", ""enableRBAC"", ""identityProfile"", ""kubernetesVersion"", ""networkProfile"", ""nodeResourceGroup"", ""windowsProfile"". If this is an inaccuracy in the documentation, please report it to the Bicep Team. [https://aka.ms/bicep-type-issues]",
            @"c:\main.bicep(56,7) : Warning BCP037: The property ""addtional"" is not allowed on objects of type ""ContainerServiceLinuxProfile"". No other properties are allowed. If this is an inaccuracy in the documentation, please report it to the Bicep Team. [https://aka.ms/bicep-type-issues]",
        };

        private static readonly string[] SampleErrors = new[]
        {
            @"c:\main.bicep(3,23) : Error BCP008: Expected the ""="" token, or a newline at this location.",
            @"c:\main.bicep(28,26) : Error BCP027: The parameter expects a default value of type ""int"" but provided value is of type ""bool"".",
        };

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
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(0, "", "");
            var mockConsole = new MockConsole();

            var sut = CreateBicepCliProxy(environmentProxy, processProxy, fileSystem, console: mockConsole);

            // Act.
            sut.Build("/main.bicep", "main.json");

            // Assert.
            mockConsole.Verify();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetEnvironmentAndFileSystemData), DynamicDataSourceType.Method)]
        public void Build_BicepFileWithWarnings_LogsWarnings(IEnvironmentProxy environmentProxy, IFileSystem fileSystem)
        {
            // Arrange.
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(0, "", string.Join('\n', SampleWarnings));
            var mockConsole = new MockConsole().ExpectOutLines(SampleWarnings.Append(""));

            var sut = CreateBicepCliProxy(environmentProxy, processProxy, fileSystem, console: mockConsole);

            // Act.
            sut.Build("/main.bicep", "main.json");

            // Assert.
            mockConsole.Verify();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetEnvironmentAndFileSystemData), DynamicDataSourceType.Method)]
        public void Build_BicepFileWithWarningsAndErrors_LogsWarningAndErrorAndThrowsException(IEnvironmentProxy environmentProxy, IFileSystem fileSystem)
        {
            // Arrange.
            var mockProcessProxy = MockProcessProxyFactory.CreateProcessProxy(1, "", string.Join('\n', SampleWarnings.Concat(SampleErrors)));
            var mockConsole = new MockConsole()
                .ExpectOutLines(SampleWarnings)
                .ExpectErrorLines(SampleErrors);

            var sut = CreateBicepCliProxy(environmentProxy, mockProcessProxy, fileSystem, console: mockConsole);

            // Act & Assert.
            FluentActions.Invoking(() => sut.Build("/main.bicep", "/main.json")).Should()
                .Throw<BicepException>()
                .WithMessage("Failed to build \"/main.bicep\".");

            mockConsole.Verify();
        }

        private static BicepCliProxy CreateBicepCliProxy(
            IEnvironmentProxy? environmentProxy = null,
            IProcessProxy? processProxy = null,
            IFileSystem? fileSystem = null,
            ILogger? logger = null,
            IConsole? console = null)
        {
            environmentProxy ??= new MockEnvironmentProxy();
            processProxy ??= StrictMock.Of<IProcessProxy>().Object;
            fileSystem ??= new MockFileSystem();
            logger ??= MockLoggerFactory.CreateLogger();
            console ??= new MockConsole();

            return new(environmentProxy, processProxy, fileSystem, logger, console);
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
