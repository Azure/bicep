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
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.IntegrationTests.Commands
{
    [TestClass]
    public class ValidateCommandTests
    {
        // NOTE: The templateHash here has to match the templateHash in SampleFiles/Valid/main.json
        private static readonly MockFileData MockValidMainTestArmTemplateData = @"{
  ""resources"": [
    {
      ""type"": ""Microsoft.Resources/deployments"",
      ""properties"": {
        ""template"": {
          ""metadata"": {
            ""_generator"": {
              ""templateHash"": ""16759655392678672335""
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

        // TODO: This test doesn't catch if main.json does not match the compiled main.bicep
        [TestMethod]
        public void Invoke_ValidFiles_ReturnsZero()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            var mockMainArmTemplateFileData = fileSystem.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(
                (args => args.Contains(MainBicepFile.FileName), () => fileSystem.SetTempFile(mockMainArmTemplateFileData)),
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
                (args => args.Contains(MainBicepFile.FileName), () => fileSystem.SetTempFile(mockMainArmTemplateFileData)),
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
                (args => args.Contains(MainBicepFile.FileName), () => fileSystem.SetTempFile(mockMainArmTemplateFileData)),
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
                $@"The file ""{fileSystem.Path.GetFullPath(MainBicepFile.FileName)}"" is invalid. ""metadata description"" must contain at least 10 characters.
".ReplaceLineEndings(),
                $@"The file ""{fileSystem.Path.GetFullPath(ReadmeFile.FileName)}"" is modified or outdated. Please run ""brm generate"" to regenerate it.
".ReplaceLineEndings(),
                $@"The file ""{fileSystem.Path.GetFullPath(VersionFile.FileName)}"" is invalid:
  #: Required properties [""$schema"",""version"",""pathFilters""] were not present
".ReplaceLineEndings());

            Invoke(fileSystem, processProxy, console);

            console.Verify();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidModulePathData), DynamicDataSourceType.Method)]
        public void Invoke_InvalidModulePath_ReturnsOne(MockFileSystem fileSystem, string error)
        {
            var mockMainArmTemplateFileData = fileSystem.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(
                (args => args.Contains(MainBicepFile.FileName), () => fileSystem.SetTempFile(mockMainArmTemplateFileData)),
                (args => args.Contains(MainBicepTestFile.FileName), () => fileSystem.SetTempFile(MockValidMainTestArmTemplateData)));

            var exitCode = Invoke(fileSystem, processProxy);

            exitCode.Should().Be(1);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidModulePathData), DynamicDataSourceType.Method)]
        public void Invoke_InvalidModulePath_WritesErrorsToConsole(MockFileSystem fileSystem, string error)
        {
            var mockMainArmTemplateFileData = fileSystem.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(
                (args => args.Contains(MainBicepFile.FileName), () => fileSystem.SetTempFile(mockMainArmTemplateFileData)),
                (args => args.Contains(MainBicepTestFile.FileName), () => fileSystem.SetTempFile(MockValidMainTestArmTemplateData)));

            var console = new MockConsole().ExpectErrorLines(error);

            Invoke(fileSystem, processProxy, console);

            console.Verify();
        }

        [TestMethod]
        public void Invoke_MetadataFileExists_WritesErrorToConsole()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithModuleWithObsoleteMetadataFile();
            var mockMainArmTemplateFileData = fileSystem.GetFile(MainArmTemplateFile.FileName);
            var processProxy = MockProcessProxyFactory.CreateProcessProxy(
                (args => args.Contains(MainBicepFile.FileName), () => fileSystem.SetTempFile(mockMainArmTemplateFileData)),
                (args => args.Contains(MainBicepTestFile.FileName), () => fileSystem.SetTempFile(MockInvalidMainTestArmTemplateData)));

            var console = new MockConsole().ExpectErrorLines(
                $@"The file ""{fileSystem.Path.GetFullPath(MetadataFile.FileName)}"" is obsolete.  Please run ""brm generate"" to have its contents moved to ""main.bicep""." + "\n");

            Invoke(fileSystem, processProxy, console);

            console.Verify();
        }

        private static IEnumerable<object[]> GetInvalidModulePathData()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();

            var moduleDirectory = fileSystem.Path.GetFullPath("/modules/FOO/BAR");
            fileSystem.MoveDirectory(fileSystem.Directory.GetCurrentDirectory(), moduleDirectory);
            fileSystem.Directory.SetCurrentDirectory(moduleDirectory);

            yield return new object[]
            {
                fileSystem,
                $@"The module path ""FOO{fileSystem.Path.DirectorySeparatorChar}BAR"" in the path ""{moduleDirectory}"" is invalid. All characters in the module path must be in lowercase.{Environment.NewLine}"
            };

            moduleDirectory = fileSystem.Path.GetFullPath("/modules/mymodule");
            fileSystem.MoveDirectory(fileSystem.Directory.GetCurrentDirectory(), moduleDirectory);
            fileSystem.Directory.SetCurrentDirectory(moduleDirectory);

            yield return new object[]
            {
                fileSystem,
                $@"The module path ""mymodule"" in the path ""{moduleDirectory}"" is invalid. The module path must be in the format of ""<module-folder>{fileSystem.Path.DirectorySeparatorChar}<module-name>"".{Environment.NewLine}"
            };
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
