// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Commands;
using Bicep.RegistryModuleTool.IntegrationTests.Assertions;
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
        public void Invoke_Success_ReturnsZero(MockFileSystem fileSystemBeforeGeneration, MockFileSystem fileSystemAfterGeneration)
        {
            var mockMainArmTemplateFileData = fileSystemAfterGeneration.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(callback: () => fileSystemBeforeGeneration.SetTempFile(mockMainArmTemplateFileData));

            var exitCode = Invoke(fileSystemBeforeGeneration, processProxy);

            exitCode.Should().Be(0);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSuccessData), DynamicDataSourceType.Method)]
        public void Invoke_Success_ProducesExpectedFiles(MockFileSystem fileSystemBeforeGeneration, MockFileSystem fileSystemAfterGeneration)
        {
            var mockMainArmTemplateFileData = fileSystemAfterGeneration.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(callback: () => fileSystemBeforeGeneration.SetTempFile(mockMainArmTemplateFileData));

            Invoke(fileSystemBeforeGeneration, processProxy);

            fileSystemBeforeGeneration.Should().HaveSameFilesAs(fileSystemAfterGeneration);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSuccessData), DynamicDataSourceType.Method)]
        public void Invoke_MultipleTimes_ProduceSameFiles(MockFileSystem fileSystemBeforeGeneration, MockFileSystem fileSystemAfterGeneration)
        {
            var mockMainArmTemplateFileData = fileSystemAfterGeneration.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(callback:  () => fileSystemBeforeGeneration.SetTempFile(mockMainArmTemplateFileData));

            for (int i = 0; i < 6; i++)
            {
                Invoke(fileSystemBeforeGeneration, processProxy);
            }

            fileSystemBeforeGeneration.Should().HaveSameFilesAs(fileSystemAfterGeneration);
        }

        [TestMethod]
        public void Invoke_BicepBuildError_ReturnsOne()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithNewFiles();
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(exitCode: 1, standardError: "Build error.");

            var exitCode = Invoke(fileSystem, processProxy);

            exitCode.Should().Be(1);
        }

        [TestMethod]
        public void Invoke_BicepBuildError_WritesErrorsToConsole()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithNewFiles();
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(exitCode: 1, standardError: "Build error one.\nBuild error two.");
            var console = new MockConsole().ExpectErrorLines("Build error one.", "Build error two.", $"Failed to build \"{fileSystem.Path.GetFullPath(MainBicepFile.FileName)}\".");

            Invoke(fileSystem, processProxy, console);

            console.Verify();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidJsonData), DynamicDataSourceType.Method)]
        public void Invoke_InvalidJsonFile_ReturnsOne(MockFileSystem fileSystem, string _)
        {
            var mockMainArmTemplateFileData = fileSystem.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(callback:  () => fileSystem.SetTempFile(mockMainArmTemplateFileData));

            var exitCode = Invoke(fileSystem, processProxy);

            exitCode.Should().Be(1);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidJsonData), DynamicDataSourceType.Method)]
        public void Invoke_InvalidJsonFile_WritesErrorsToConsole(MockFileSystem fileSystem, string error)
        {
            var mockMainArmTemplateFileData = fileSystem.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(callback:  () => fileSystem.SetTempFile(mockMainArmTemplateFileData));
            var console = new MockConsole().ExpectErrorLines(error);

            var exitCode = Invoke(fileSystem, processProxy, console);

            console.Verify();
        }

        private static IEnumerable<object[]> GetSuccessData()
        {
            yield return new object[]
            {
                MockFileSystemFactory.CreateFileSystemWithEmptyFolder(),
                MockFileSystemFactory.CreateFileSystemWithEmptyGeneratedFiles(),
            };

            yield return new object[]
            {
                MockFileSystemFactory.CreateFileSystemWithNewFiles(),
                MockFileSystemFactory.CreateFileSystemWithNewGeneratedFiles(),
            };

            yield return new object[]
            {
                MockFileSystemFactory.CreateFileSystemWithModifiedFiles(),
                MockFileSystemFactory.CreateFileSystemWithModifiedGeneratedFiles(),
            };

            yield return new object[]
            {
                // Generate valid files should not change the ARM template parameters file.
                MockFileSystemFactory.CreateFileSystemWithValidFiles(),
                MockFileSystemFactory.CreateFileSystemWithValidFiles(),
            };
        }

        private static IEnumerable<object[]> GetInvalidJsonData()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            fileSystem.AddFile(MetadataFile.FileName, "Invalid JSON content.");

            yield return new object[]
            {
                fileSystem,
                $"The metadata file \"{fileSystem.Path.GetFullPath(MetadataFile.FileName)}\" is not a valid JSON file. 'I' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 0."
            };

            fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            fileSystem.AddFile(MainArmTemplateParametersFile.FileName, "Invalid JSON content.");

            yield return new object[]
            {
                fileSystem,
                $"The ARM template parameters file \"{fileSystem.Path.GetFullPath(MainArmTemplateParametersFile.FileName)}\" is not a valid JSON file. 'I' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 0."
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
