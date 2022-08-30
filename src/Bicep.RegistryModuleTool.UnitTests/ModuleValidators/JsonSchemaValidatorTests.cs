// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleValidators;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using FluentAssertions;
using Json.More;
using Json.Patch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleValidators
{
    [TestClass]
    public class JsonSchemaValidatorTests
    {
        private static readonly MockFileSystem FileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();

        private readonly JsonSchemaValidator sut = new(MockLoggerFactory.CreateLogger());

        [TestMethod]
        public void Validate_ValidMetadataFile_Succeeds()
        {
            var fileToValidate = MetadataFile.ReadFromFileSystem(FileSystem);

            FluentActions.Invoking(() => this.sut.Validate(fileToValidate)).Should().NotThrow();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidMetadataFileData), DynamicDataSourceType.Method)]
        public void Validate_InvalidMetadataFile_ThrowsException(MetadataFile invalidFile, string expectedErrorMessage)
        {
            FluentActions.Invoking(() => this.sut.Validate(invalidFile)).Should()
                .Throw<InvalidModuleException>()
                .WithMessage(expectedErrorMessage.ReplaceLineEndings());
        }

        private static IEnumerable<object[]> GetInvalidMetadataFileData()
        {
            var file = MetadataFile.ReadFromFileSystem(FileSystem);

            yield return new object[]
            {
                PatchMetadataFile(file, JsonPatchOperations.Add("/extra", true)),
                @$"The file ""{file.Path}"" is invalid:
  #/extra: The property is not allowed
",
            };

            yield return new object[]
            {
                PatchMetadataFile(
                    file,
                    JsonPatchOperations.Remove("/name"),
                    JsonPatchOperations.Replace("/summary", ""),
                    JsonPatchOperations.Replace("/owner", "")),
                @$"The file ""{file.Path}"" is invalid:
  #/summary: Value is not longer than or equal to 10 characters
  #/owner: Value does not match the pattern of ""^(?:Azure\/)?[a-zA-Z\d](?:[a-zA-Z\d]|-(?=[a-zA-Z\d])){{0,38}}$""
  #: Required properties [""name""] were not present
",
            };
        }

        private static MetadataFile PatchMetadataFile(MetadataFile file, params PatchOperation[] operations)
        {
            var patchedElement = file.RootElement.Patch(operations);
            var tempFileSystem = MockFileSystemFactory.CreateFileSystemWithEmptyFolder();
            tempFileSystem.AddFile(file.Path, patchedElement.ToJsonString());

            return MetadataFile.ReadFromFileSystem(tempFileSystem);
        }
    }
}
