// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using Bicep.RegistryModuleTool.TestFixtures.Extensions;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using FluentAssertions;
using Json.More;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using static FluentAssertions.FluentActions;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleFileValidators
{
    [TestClass]
    public class DiffValidatorTests
    {
        private readonly MockFileSystem fileSystem;

        private readonly MainArmTemplateFile latestMainArmTemplateFile;

        private readonly DiffValidator sut;

        public DiffValidatorTests()
        {
            this.fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            this.latestMainArmTemplateFile = MainArmTemplateFile.ReadFromFileSystem(this.fileSystem);
            this.sut = new DiffValidator(this.fileSystem, MockLoggerFactory.CreateLogger(), this.latestMainArmTemplateFile);
        }

        [TestMethod]
        public void Validate_ValidMainArmTemplateFile_Succeeds()
        {
            var fileToValidate = MainArmTemplateFile.ReadFromFileSystem(this.fileSystem);

            Invoking(() => this.sut.Validate(fileToValidate)).Should().NotThrow();
        }

        [TestMethod]
        public void Validate_ModifiedMainArmTemplateFile_ThrowsException()
        {
            this.fileSystem.AddFile(this.latestMainArmTemplateFile.Path, "modified");

            var fileToValidate = MainArmTemplateFile.ReadFromFileSystem(this.fileSystem);

            Invoking(() => this.sut.Validate(fileToValidate)).Should()
                .Throw<InvalidModuleFileException>()
                .WithMessage($"The file \"{fileToValidate.Path}\" is modified or outdated. Please regenerate the file to fix it.{Environment.NewLine}");
        }

        [TestMethod]
        public void Validate_ValidArmTemplateParametersFile_Succeeds()
        {
            var fileToValidate = MainArmTemplateParametersFile.ReadFromFileSystem(this.fileSystem);

            Invoking(() => this.sut.Validate(fileToValidate)).Should().NotThrow();
        }

        [TestMethod]
        public void Validate_ValidChangesInMainArmTemplateParametersFile_Succeeds()
        {
            // Update a parameter value and add an existing parameter.
            var originalFile = MainArmTemplateParametersFile.ReadFromFileSystem(fileSystem);
            var patchedFileElement = originalFile.RootElement.Patch(
                JsonPatchOperations.Replace("/parameters/linuxAdminUsername/value", "testuser"),
                JsonPatchOperations.Add("/parameters/clusterName", new Dictionary<string, JsonElement>().AsJsonElement()),
                JsonPatchOperations.Add("/parameters/clusterName/value", "aks101cluster"));

            fileSystem.AddFile(originalFile.Path, patchedFileElement.ToFormattedString());

            var fileToValidate = MainArmTemplateParametersFile.ReadFromFileSystem(fileSystem);

            Invoking(() => this.sut.Validate(fileToValidate)).Should().NotThrow();
        }

        [TestMethod]
        public void Validate_InvalidChangesInMainArmTemplateParametersFile_ThrowsException()
        {
            // Remove a required parameter and add a non-existing parameter.
            var originalFile = MainArmTemplateParametersFile.ReadFromFileSystem(fileSystem);
            var patchedFileElement = originalFile.RootElement.Patch(
                JsonPatchOperations.Remove("/parameters/linuxAdminUsername"),
                JsonPatchOperations.Add("/parameters/nonExisting", new Dictionary<string, JsonElement>().AsJsonElement()),
                JsonPatchOperations.Add("/parameters/nonExisting/value", 0));

            fileSystem.AddFile(originalFile.Path, patchedFileElement.ToFormattedString());

            var fileToValidate = MainArmTemplateParametersFile.ReadFromFileSystem(fileSystem);

            Invoking(() => this.sut.Validate(fileToValidate)).Should()
                .Throw<InvalidModuleFileException>()
                .WithMessage($@"The file ""{fileToValidate.Path}"" is modified or outdated. Please regenerate the file to fix it.{Environment.NewLine}");
        }

        [TestMethod]
        public void Validate_ValidReadmeFile_Succeeds()
        {
            var fileToValidate = ReadmeFile.ReadFromFileSystem(this.fileSystem);

            Invoking(() => this.sut.Validate(fileToValidate)).Should().NotThrow();
        }

        [TestMethod]
        public void Validate_ModifiedReadmeFile_ThrowsException()
        {
            this.fileSystem.AddFile(this.fileSystem.Path.GetFullPath(ReadmeFile.FileName), "modified");

            var fileToValidate = ReadmeFile.ReadFromFileSystem(this.fileSystem);

            Invoking(() => this.sut.Validate(fileToValidate)).Should()
                .Throw<InvalidModuleFileException>()
                .WithMessage($@"The file ""{fileToValidate.Path}"" is modified or outdated. Please regenerate the file to fix it.{Environment.NewLine}");
        }

        [TestMethod]
        public void Validate_ValidVersionFile_Succeeds()
        {
            var fileToValidate = VersionFile.ReadFromFileSystem(this.fileSystem);

            Invoking(() => this.sut.Validate(fileToValidate)).Should().NotThrow();
        }

        [TestMethod]
        public void Validate_ModifiedVersionFile_Succeeds()
        {
            this.fileSystem.AddFile(this.fileSystem.Path.GetFullPath(VersionFile.FileName), "modified");

            var fileToValidate = VersionFile.ReadFromFileSystem(this.fileSystem);

            Invoking(() => this.sut.Validate(fileToValidate)).Should()
                .Throw<InvalidModuleFileException>()
                .WithMessage($@"The file ""{fileToValidate.Path}"" is modified or outdated. Please regenerate the file to fix it.{Environment.NewLine}");
        }
    }
}
