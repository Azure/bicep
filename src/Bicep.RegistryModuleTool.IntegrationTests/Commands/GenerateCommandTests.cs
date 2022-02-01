// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Commands;
using Bicep.RegistryModuleTool.TestFixtures.Assertions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.Proxies;
using Bicep.RegistryModuleTool.TestFixtures.Extensions;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using Bicep.RegistryModuleTool.TestFixtures.Mocks;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Mocks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.CommandLine;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.IntegrationTests.Commands
{
    [TestClass]
    public class GenerateCommandTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetSuccessData), DynamicDataSourceType.Method)]
        public void Invoke_OnSuccess_ProducesExpectedFiles(MockFileSystem fileSystemBeforeGeneration, MockFileSystem fileSystemAfterGeneration)
        {
            var mockMainArmTemplateFileData = fileSystemAfterGeneration.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(() => fileSystemBeforeGeneration.SetTempFile(mockMainArmTemplateFileData));

            Invoke(fileSystemBeforeGeneration, processProxy);

            fileSystemBeforeGeneration.Should().HaveSameFilesAs(fileSystemAfterGeneration);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSuccessData), DynamicDataSourceType.Method)]
        public void Invoke_RepeatOnSuccess_ProducesSameFiles(MockFileSystem fileSystemBeforeGeneration, MockFileSystem fileSystemAfterGeneration)
        {
            var mockMainArmTemplateFileData = fileSystemAfterGeneration.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(() => fileSystemBeforeGeneration.SetTempFile(mockMainArmTemplateFileData));

            for (int i = 0; i < 6; i++)
            {
                Invoke(fileSystemBeforeGeneration, processProxy);
            }

            fileSystemBeforeGeneration.Should().HaveSameFilesAs(fileSystemAfterGeneration);
        }

        [TestMethod]
        public void Invoke_BicepBuildError_ReturnsOne()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithNewlyGeneratedFiles();
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(exitCode: 1, standardError: "Build error.");

            var exitCode = Invoke(fileSystem, processProxy);

            exitCode.Should().Be(1);
        }

        [TestMethod]
        public void Invoke_BicepBuildError_WritesErrorsToConsole()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithNewlyGeneratedFiles();
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(exitCode: 1, standardError: "Build error one.\nBuild error two.");
            var console = new MockConsole().ExpectErrorLines("Build error one.", "Build error two.", $"Failed to build \"{fileSystem.Path.GetFullPath(MainBicepFile.FileName)}\".");

            Invoke(fileSystem, processProxy, console);

            console.Verify();
        }

        private static IEnumerable<object[]> GetSuccessData()
        {
            yield return new object[]
            {
                MockFileSystemFactory.CreateFileSystemWithEmptyFolder(),
                MockFileSystemFactory.CreateFileSystemWithNewlyGeneratedFiles(),
            };

            yield return new object[]
            {
                MockFileSystemFactory.CreateFileSystemWithModifiedFiles(),
                MockFileSystemFactory.CreateFileSystemWithValidFiles(),
            };
        }

        private static int Invoke(IFileSystem fileSystem, IProcessProxy processProxy, IConsole? console = null)
        {
            var command = new GenerateCommand()
            {
                Handler = new GenerateCommand.CommandHandler(
                    MockEnvironmentProxy.Default,
                    processProxy,
                    fileSystem,
                    MockLoggerFactory.CreateGenericLogger<GenerateCommand>()),
            };

            return command.Invoke("", console);
        }
    }
}
