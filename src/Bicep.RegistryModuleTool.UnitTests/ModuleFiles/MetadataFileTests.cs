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
    public class MetadataFileTests
    {
        [TestMethod]
        public void ReadFromFileSystem_InvalidJson_ThrowsException()
        {
            var fileSystem = new MockFileSystem();
            var path = fileSystem.Path.GetFullPath(MetadataFile.FileName);

            fileSystem.AddFile(path, "######");

            FluentActions.Invoking(() => MetadataFile.ReadFromFileSystem(fileSystem)).Should()
                .Throw<BicepException>()
                .WithMessage($"The metadata file \"{path}\" is not a valid JSON file.*");
        }

        [TestMethod]
        public void ReadFromFileSystem_ValidJson_Succeeds()
        {
            var fileSystem = new MockFileSystem();
            var path = fileSystem.Path.GetFullPath(MetadataFile.FileName);

            fileSystem.AddFile(path, "{}");

            FluentActions.Invoking(() => MetadataFile.ReadFromFileSystem(fileSystem)).Should().NotThrow();
        }
    }
}
