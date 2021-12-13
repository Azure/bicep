// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Factories;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Extensions;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Mocks;
using FluentAssertions;
using Json.More;
using Json.Patch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleFileValidators
{
    [TestClass]
    public class JsonSchemaValidatorTests
    {
        private readonly static MockFileSystem FileSystem = MockFileSystemFactory.CreateMockFileSystem();

        private readonly JsonSchemaValidator sut = new(MockLogger.Create());

        [TestMethod]
        public void Validate_ValidMetadataFile_Succeeds()
        {
            var file = MetadataFile.ReadFromFileSystem(FileSystem);

            FluentActions.Invoking(() => this.sut.Validate(file)).Should().NotThrow();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidMetadataFileData), DynamicDataSourceType.Method)]
        public void Validate_InvalidMetadataFile_ThrowsException(MetadataFile invalidFile, string expectedErrorMessage)
        {
            FluentActions.Invoking(() => this.sut.Validate(invalidFile)).Should()
                .Throw<BicepException>()
                .WithMessage(expectedErrorMessage.ReplaceLineEndings());
        }

        [TestMethod]
        public void Validate_ValidMainArmTemplateParametersFile_Succeeds()
        {
            var file = MainArmTemplateParametersFile.ReadFromFileSystem(FileSystem);

            FluentActions.Invoking(() => this.sut.Validate(file)).Should().NotThrow();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidMainArmTemplateParametersFileData), DynamicDataSourceType.Method)]
        public void Validate_InvalidMainArmTemplateParametersFile_ThrowsException(MainArmTemplateParametersFile invalidFile, string expectedErrorMessage)
        {
            FluentActions.Invoking(() => this.sut.Validate(invalidFile)).Should()
                .Throw<BicepException>()
                .WithMessage(expectedErrorMessage.ReplaceLineEndings());
        }

        private static IEnumerable<object[]> GetInvalidMetadataFileData()
        {
            var file = MetadataFile.ReadFromFileSystem(FileSystem);

            yield return new object[]
            {
                PatchMetadataFile(file, PatchOperations.Add("/extra", true)),
                @$"The file ""{file.Path}"" is invalid:
  #/extra: The property is not allowed
",
            };

            yield return new object[]
            {
                PatchMetadataFile(
                    file,
                    PatchOperations.Remove("/itemDisplayName"),
                    PatchOperations.Replace("/description", ""),
                    PatchOperations.Replace("/dateUpdated", "42")),
                @$"The file ""{file.Path}"" is invalid:
  #/description: Value is not longer than or equal to 10 characters
  #/dateUpdated: Value does not match the pattern of ""^(20[1-9][0-9])-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[01])$""
  #: Required properties [itemDisplayName] were not present
",
            };
        }

        private static IEnumerable<object[]> GetInvalidMainArmTemplateParametersFileData()
        {
            var file = MainArmTemplateParametersFile.ReadFromFileSystem(FileSystem);

            yield return new object[]
            {
                PatchMainArmTemplateParametersFile(file, PatchOperations.Add("/extra", true)),
                @$"The file ""{file.Path}"" is invalid:
  #/extra: The property is not allowed
",
            };

            yield return new object[]
            {
                PatchMainArmTemplateParametersFile(
                    file,
                    PatchOperations.Remove("/$schema"),
                    PatchOperations.Replace("/contentVersion", "v1")),
                @$"The file ""{file.Path}"" is invalid:
  #/contentVersion: Value does not match the pattern of ""(^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$)""
  #: Required properties [$schema] were not present
",
            };
        }

        private static MetadataFile PatchMetadataFile(MetadataFile file, params PatchOperation[] operations)
        {
            var patchedElement = file.RootElement.Patch(operations);
            var tempFileSystem = new MockFileSystem();
            tempFileSystem.AddFile(file.Path, patchedElement.ToJsonString());
            tempFileSystem.Directory.SetCurrentDirectory(tempFileSystem.Path.GetDirectoryName(file.Path));

            return MetadataFile.ReadFromFileSystem(tempFileSystem);
        }

        private static MainArmTemplateParametersFile PatchMainArmTemplateParametersFile(MainArmTemplateParametersFile file, params PatchOperation[] operations)
        {
            var patchedElement = file.RootElement.Patch(operations);
            var tempFileSystem = new MockFileSystem();
            tempFileSystem.AddFile(file.Path, patchedElement.ToJsonString());
            tempFileSystem.Directory.SetCurrentDirectory(tempFileSystem.Path.GetDirectoryName(file.Path));

            return MainArmTemplateParametersFile.ReadFromFileSystem(tempFileSystem);
        }
    }
}
