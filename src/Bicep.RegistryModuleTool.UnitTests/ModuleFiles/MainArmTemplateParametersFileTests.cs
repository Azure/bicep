// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleFiles
{
    [TestClass]
    public class MainArmTemplateParametersFileTests
    {
        [TestMethod]
        public void ReadFromFileSystem_InvalidJson_ThrowsException()
        {
            var fileSystem = new MockFileSystem();
            var path = fileSystem.Path.GetFullPath(MainArmTemplateParametersFile.FileName);

            fileSystem.AddFile(path, "######");

            FluentActions.Invoking(() => MainArmTemplateParametersFile.ReadFromFileSystem(fileSystem)).Should()
                .Throw<BicepException>()
                .WithMessage($"The ARM template parameters file \"{path}\" is not a valid JSON file.*");
        }
    }
}
