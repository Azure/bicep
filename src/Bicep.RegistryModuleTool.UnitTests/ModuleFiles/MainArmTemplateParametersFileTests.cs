// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleFiles
{
    [TestClass]
    public class MainArmTemplateParametersFileTests
    {
        private readonly MockFileSystem fileSystem = MockFileSystemFactory.CreateFileSystemWithEmptyFolder();

        [TestMethod]
        public void ReadFromFileSystem_InvalidJson_ThrowsException()
        {
            var path = this.fileSystem.Path.GetFullPath(MainArmTemplateParametersFile.FileName);
            this.fileSystem.AddFile(path, "######");

            FluentActions.Invoking(() => MainArmTemplateParametersFile.ReadFromFileSystem(fileSystem)).Should()
                .Throw<BicepException>()
                .WithMessage($"The ARM template parameters file \"{path}\" is not a valid JSON file.*");
        }

        [TestMethod]
        public void ReadFromFileSystem_ValidJson_Succeeds()
        {
            this.fileSystem.AddFile(MainArmTemplateParametersFile.FileName, "{}");

            FluentActions.Invoking(() => MainArmTemplateParametersFile.ReadFromFileSystem(fileSystem)).Should().NotThrow();
        }
    }
}
