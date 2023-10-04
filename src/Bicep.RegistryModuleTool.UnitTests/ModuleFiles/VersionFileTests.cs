// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleFiles
{
    [TestClass]
    public class VersionFileTests
    {
        [TestMethod]
        public async Task OpenAsync_InvalidJson_Throws()
        {
            var fileSystem = new MockFileSystem();
            var path = fileSystem.Path.GetFullPath(VersionFile.FileName);

            fileSystem.AddFile(path, "######");

            await FluentActions.Invoking(() => VersionFile.OpenAsync(fileSystem)).Should()
                .ThrowAsync<BicepException>()
                .WithMessage($"The version file \"{path}\" is not a valid JSON file.*");
        }

        [TestMethod]
        public async Task OpenAsync_RootIsNotObject_Throws()
        {
            var fileSystem = new MockFileSystem();
            var path = fileSystem.Path.GetFullPath(VersionFile.FileName);

            fileSystem.AddFile(path, "[]");

            await FluentActions.Invoking(() => VersionFile.OpenAsync(fileSystem)).Should()
                .ThrowAsync<BicepException>()
                .WithMessage($"The version file \"{path}\" must contain a JSON object at the root level.");
        }

        [TestMethod]
        public async Task OpenAsync_ValidJson_Succeeds()
        {
            var fileSystem = new MockFileSystem();
            var path = fileSystem.Path.GetFullPath(VersionFile.FileName);

            fileSystem.AddFile(path, "{}");

            await FluentActions.Invoking(() => VersionFile.OpenAsync(fileSystem)).Should().NotThrowAsync();
        }
    }
}
