// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Commands;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.Proxies;
using Bicep.RegistryModuleTool.TestFixtures.Extensions;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using Bicep.RegistryModuleTool.TestFixtures.Mocks;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Mocks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.CommandLine;
using System.IO.Abstractions;

namespace Bicep.RegistryModuleTool.IntegrationTests.Commands
{
    [TestClass]
    public class ValidateCommandTests
    {
        [TestMethod]
        public void Invoke_ValidFiles_ReturnsZero()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            var mockMainArmTemplateFileData = fileSystem.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(callback: () => fileSystem.SetTempFile(mockMainArmTemplateFileData));

            var exitCode = Invoke(fileSystem, processProxy);

            exitCode.Should().Be(0);
        }

        [TestMethod]
        public void Invoke_InvalidFiles_ReturnsOne()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithInvalidFiles();
            var mockMainArmTemplateFileData = fileSystem.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(callback: () => fileSystem.SetTempFile(mockMainArmTemplateFileData));

            var exitCode = Invoke(fileSystem, processProxy);

            exitCode.Should().Be(1);
        }

        [TestMethod]
        public void Invoke_InvalidFiles_WritesErrorsToConsole()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithInvalidFiles();
            var mockMainArmTemplateFileData = fileSystem.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(callback: () => fileSystem.SetTempFile(mockMainArmTemplateFileData));

            var console = new MockConsole().ExpectErrorLines(
                $@"The file ""{fileSystem.Path.GetFullPath(MainArmTemplateParametersFile.FileName)}"" is invalid:
  #/parameters/dnsPrefix: Expected 1 matching subschema but found 0
  #/parameters/dnsPrefix: Required properties [value] were not present
  #/parameters/dnsPrefix: Required properties [reference] were not present
  #/parameters/linuxAdminUsername: Expected 1 matching subschema but found 0
  #/parameters/linuxAdminUsername: Required properties [value] were not present
  #/parameters/linuxAdminUsername: Required properties [reference] were not present
".ReplaceLineEndings(),
                $@"The file ""{fileSystem.Path.GetFullPath(MetadataFile.FileName)}"" is invalid:
  #/description: Value is not longer than or equal to 10 characters
  #/summary: Value is not longer than or equal to 10 characters
".ReplaceLineEndings(),
                $@"The file ""{fileSystem.Path.GetFullPath(ReadmeFile.FileName)}"" is modified or outdated. Please regenerate the file to fix it.
".ReplaceLineEndings(),
                $@"The file ""{fileSystem.Path.GetFullPath(VersionFile.FileName)}"" is modified or outdated. Please regenerate the file to fix it.
".ReplaceLineEndings());

            Invoke(fileSystem, processProxy, console);

            console.Verify();
        }

        private static int Invoke(IFileSystem fileSystem, IProcessProxy processProxy, IConsole? console = null)
        {
            var command = new ValidateCommand()
            {
                Handler = new ValidateCommand.CommandHandler(
                    MockEnvironmentProxy.Default,
                    processProxy,
                    fileSystem,
                    MockLoggerFactory.CreateGenericLogger<ValidateCommand>()),
            };

            return command.Invoke("", console);
        }
    }
}
