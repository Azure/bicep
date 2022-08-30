// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.ModuleValidators;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO.Abstractions.TestingHelpers;
using static FluentAssertions.FluentActions;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleValidators
{
    [TestClass]
    public class ModulePathValidatorTests
    {
        [TestMethod]
        public void ValidateModulePath_InvalidPath_Throws()
        {
            const string currentDirectory = "/foo/bar";
            var fileSystem = new MockFileSystem();

            fileSystem.AddDirectory(currentDirectory);
            fileSystem.Directory.SetCurrentDirectory(fileSystem.Path.GetFullPath(currentDirectory));

            Invoking(() => ModulePathValidator.ValidateModulePath(fileSystem))
                .Should()
                .Throw<InvalidModuleException>()
                .WithMessage($@"Could not find the ""modules"" folder in the path ""{fileSystem.Directory.GetCurrentDirectory()}"".");
        }

        [TestMethod]
        public void ValidateModulePath_ValidPath_DoesNotThrow()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithEmptyFolder();

            Invoking(() => ModulePathValidator.ValidateModulePath(fileSystem))
                .Should()
                .NotThrow();
        }
    }
}
