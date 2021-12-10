// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Extensions;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Factories;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Mocks;
using FluentAssertions;
using Json.More;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;

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
            this.fileSystem = MockFileSystemFactory.CreateMockFileSystem();
            this.latestMainArmTemplateFile = MainArmTemplateFile.ReadFromFileSystem(this.fileSystem);
            this.sut = new DiffValidator(fileSystem, MockLogger.Create(), this.latestMainArmTemplateFile);
        }

        [TestMethod]
        public void Validate_ValidMainArmTemplateFile_Succeeds()
        {
            var fileToValidate = MainArmTemplateFile.ReadFromFileSystem(this.fileSystem);

            FluentActions.Invoking(() => this.sut.Validate(fileToValidate)).Should().NotThrow();
        }

        [TestMethod]
        public void Validate_ModifiedMainArmTemplateFile_ThrowsException()
        {
            this.fileSystem.AddFile(this.latestMainArmTemplateFile.Path, "modified");

            var fileToValidate = MainArmTemplateFile.ReadFromFileSystem(this.fileSystem);

            FluentActions.Invoking(() => this.sut.Validate(fileToValidate)).Should()
                .Throw<BicepException>()
                .WithMessage($@"The file ""{fileToValidate.Path}"" is modified or outdated. Please regenerate the file to fix it.");
        }

        [TestMethod]
        public void Validate_ValidArmTemplateParametersFile_Succeeds()
        {
            var fileToValidate = MainArmTemplateParametersFile.ReadFromFileSystem(this.fileSystem);

            FluentActions.Invoking(() => this.sut.Validate(fileToValidate)).Should().NotThrow();
        }

        [TestMethod]
        public void Validate_ValidChangesInMainArmTemplateParametersFile_Succeeds()
        {
            // Update a parameter value and add an existing parameter.
            var originalFile = MainArmTemplateParametersFile.ReadFromFileSystem(fileSystem);
            var patchedFileElement = originalFile.RootElement.Patch(
                PatchOperations.Replace("/parameters/linuxAdminUsername/value", "testuser"),
                PatchOperations.Add("/parameters/clusterName", new Dictionary<string, JsonElement>().AsJsonElement()),
                PatchOperations.Add("/parameters/clusterName/value", "aks101cluster"));

            fileSystem.AddFile(originalFile.Path, patchedFileElement.ToFormattedJsonString());

            var fileToValiate = MainArmTemplateParametersFile.ReadFromFileSystem(fileSystem);

            FluentActions.Invoking(() => this.sut.Validate(fileToValiate)).Should().NotThrow();
        }

        [TestMethod]
        public void Validate_InvalidChangesInMainArmTemplateParametersFile_ThrowsException()
        {
            // Remove a required parameter and add a non-existing parameter.
            var originalFile = MainArmTemplateParametersFile.ReadFromFileSystem(fileSystem);
            var patchedFileElement = originalFile.RootElement.Patch(
                PatchOperations.Remove("/parameters/linuxAdminUsername"),
                PatchOperations.Add("/parameters/nonExisting", new Dictionary<string, JsonElement>().AsJsonElement()),
                PatchOperations.Add("/parameters/nonExisting/value", 0));

            fileSystem.AddFile(originalFile.Path, patchedFileElement.ToJsonString());

            var fileToValiate = MainArmTemplateParametersFile.ReadFromFileSystem(fileSystem);

            FluentActions.Invoking(() => this.sut.Validate(fileToValiate)).Should()
                .Throw<BicepException>()
                .WithMessage($@"The file ""{fileToValiate.Path}"" is modified or outdated. Please regenerate the file to fix it.");
        }

        [TestMethod]
        public void Validate_ValidReadmeFile_Succeeds()
        {
            var fileToValidate = ReadmeFile.ReadFromFileSystem(this.fileSystem);

            FluentActions.Invoking(() => this.sut.Validate(fileToValidate)).Should().NotThrow();
        }

        [TestMethod]
        public void Validate_ModifiedReadmeFile_Succeeds()
        {
            this.fileSystem.AddFile(this.fileSystem.Path.GetFullPath(ReadmeFile.FileName), "modified");

            var fileToValidate = ReadmeFile.ReadFromFileSystem(this.fileSystem);

            FluentActions.Invoking(() => this.sut.Validate(fileToValidate)).Should()
                .Throw<BicepException>()
                .WithMessage($@"The file ""{fileToValidate.Path}"" is modified or outdated. Please regenerate the file to fix it.");
        }

        [TestMethod]
        public void Validate_ValidVersionFile_Succeeds()
        {
            var fileToValidate = VersionFile.ReadFromFileSystem(this.fileSystem);

            FluentActions.Invoking(() => this.sut.Validate(fileToValidate)).Should().NotThrow();
        }

        [TestMethod]
        public void Validate_ModifiedVersionFile_Succeeds()
        {
            this.fileSystem.AddFile(this.fileSystem.Path.GetFullPath(VersionFile.FileName), "modified");

            var fileToValidate = VersionFile.ReadFromFileSystem(this.fileSystem);

            FluentActions.Invoking(() => this.sut.Validate(fileToValidate)).Should()
                .Throw<BicepException>()
                .WithMessage($@"The file ""{fileToValidate.Path}"" is modified or outdated. Please regenerate the file to fix it.");
        }
    }
}
