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
    public class VersionFileTests
    {
        [TestMethod]
        public void ReadFromFileSystem_InvalidJson_ThrowsException()
        {
            var fileSystem = new MockFileSystem();
            var path = fileSystem.Path.GetFullPath(VersionFile.FileName);

            fileSystem.AddFile(path, "######");

            FluentActions.Invoking(() => VersionFile.OpenAsync(fileSystem)).Should()
                .Throw<BicepException>()
                .WithMessage($"The version file \"{path}\" is not a valid JSON file.*");
        }

        [TestMethod]
        public void ReadFromFileSystem_RootIsNotObject_ThrowsException()
        {
            var fileSystem = new MockFileSystem();
            var path = fileSystem.Path.GetFullPath(VersionFile.FileName);

            fileSystem.AddFile(path, "[]");

            FluentActions.Invoking(() => VersionFile.OpenAsync(fileSystem)).Should()
                .Throw<BicepException>()
                .WithMessage($"The version file \"{path}\" must contain a JSON object at the root level.");
        }

        [TestMethod]
        public void ReadFromFileSystem_ValidJson_Succeeds()
        {
            var fileSystem = new MockFileSystem();
            var path = fileSystem.Path.GetFullPath(VersionFile.FileName);

            fileSystem.AddFile(path, "{}");

            FluentActions.Invoking(() => VersionFile.OpenAsync(fileSystem)).Should().NotThrow();
        }
    }
}
