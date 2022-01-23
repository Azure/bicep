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
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.IntegrationTests.Commands
{
    [TestClass]
    public class ValidateCommandTests
    {
        private static readonly MockFileData MockValidMainTestArmTemplateData = @"{
  ""resources"": [
    {
      ""type"": ""Microsoft.Resources/deployments"",
      ""properties"": {
        ""template"": {
          ""metadata"": {
            ""_generator"": {
              ""templateHash"": ""7959995366596346262""
            }
          }
        }
      }
    }
  ]
}";

        private static readonly MockFileData MockInvalidMainTestArmTemplateData = @"{
  ""resources"": []
}";

        [TestMethod]
        public void Invoke_ValidFiles_ReturnsZero()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            var mockMainArmTemplateFileData = fileSystem.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(
                (args => args.Contains(MainBicepFile.FileName),  () => fileSystem.SetTempFile(mockMainArmTemplateFileData)),
                (args => args.Contains(MainBicepTestFile.FileName), () => fileSystem.SetTempFile(MockValidMainTestArmTemplateData)));

            var exitCode = Invoke(fileSystem, processProxy);

            exitCode.Should().Be(0);
        }

        [TestMethod]
        public void Invoke_InvalidFiles_ReturnsOne()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithInvalidFiles();
            var mockMainArmTemplateFileData = fileSystem.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(
                (args => args.Contains(MainBicepFile.FileName),  () => fileSystem.SetTempFile(mockMainArmTemplateFileData)),
                (args => args.Contains(MainBicepTestFile.FileName), () => fileSystem.SetTempFile(MockInvalidMainTestArmTemplateData)));

            var exitCode = Invoke(fileSystem, processProxy);

            exitCode.Should().Be(1);
        }

        [TestMethod]
        public void Invoke_InvalidFiles_WritesErrorsToConsole()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithInvalidFiles();
            var mockMainArmTemplateFileData = fileSystem.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(
                (args => args.Contains(MainBicepFile.FileName),  () => fileSystem.SetTempFile(mockMainArmTemplateFileData)),
                (args => args.Contains(MainBicepTestFile.FileName), () => fileSystem.SetTempFile(MockInvalidMainTestArmTemplateData)));

            var console = new MockConsole().ExpectErrorLines(
                $@"The file ""{fileSystem.Path.GetFullPath(MainBicepFile.FileName)}"" is invalid. Descriptions for the following parameters are missing:
  - dnsPrefix
  - servicePrincipalClientSecret

The file ""{fileSystem.Path.GetFullPath(MainBicepFile.FileName)}"" is invalid. Descriptions for the following outputs are missing:
  - controlPlaneFQDN
".ReplaceLineEndings(),
                $@"The file ""{fileSystem.Path.GetFullPath($"test/{MainBicepTestFile.FileName}")}"" is invalid. Could not find tests in the file. Please make sure to add at least one module referencing the main Bicep file.
".ReplaceLineEndings(),
                $@"The file ""{fileSystem.Path.GetFullPath(MetadataFile.FileName)}"" is invalid:
  #/description: Value is not longer than or equal to 10 characters
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
