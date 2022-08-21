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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleValidators
{
    [TestClass]
    public class DescriptionsValidatorTests
    {
        private readonly MockFileSystem fileSystem;

        private readonly MainBicepFile fileToValidate;

        public DescriptionsValidatorTests()
        {
            this.fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            this.fileToValidate = MainBicepFile.ReadFromFileSystem(this.fileSystem);
        }

        [TestMethod]
        public void Validate_ValidFile_Succeeds()
        {
            var latestArmTemplateFile = MainArmTemplateFile.ReadFromFileSystem(this.fileSystem);
            var sut = this.CreateDescriptionsValidator(latestArmTemplateFile);

            FluentActions.Invoking(() => sut.Validate(this.fileToValidate)).Should().NotThrow();
        }

        [TestMethod]
        public void Validate_MissingDescriptions_ThrowsException()
        {
            var mainArmTemplateFile = MainArmTemplateFile.ReadFromFileSystem(this.fileSystem);
            var mainArmTemplateFileElement = JsonElementFactory.CreateElement(mainArmTemplateFile.Content);

            var patchedElement = mainArmTemplateFileElement.Patch(
                JsonPatchOperations.Remove("/parameters/sshRSAPublicKey/metadata/description"),
                JsonPatchOperations.Remove("/parameters/clusterName/metadata/description"),
                JsonPatchOperations.Remove("/parameters/osDiskSizeGB/metadata/description"),
                JsonPatchOperations.Remove("/outputs/controlPlaneFQDN/metadata/description"));

            fileSystem.AddFile(mainArmTemplateFile.Path, patchedElement.ToJsonString());

            var latestArmTemplateFile = MainArmTemplateFile.ReadFromFileSystem(this.fileSystem);
            var sut = this.CreateDescriptionsValidator(latestArmTemplateFile);

            FluentActions.Invoking(() => sut.Validate(this.fileToValidate)).Should()
                .Throw<InvalidModuleException>()
                .WithMessage(
$@"The file ""{this.fileToValidate.Path}"" is invalid. Descriptions for the following parameters are missing:
  - sshRSAPublicKey
  - clusterName
  - osDiskSizeGB

The file ""{this.fileToValidate.Path}"" is invalid. Descriptions for the following outputs are missing:
  - controlPlaneFQDN
".ReplaceLineEndings());
        }

        private DescriptionsValidator CreateDescriptionsValidator(MainArmTemplateFile latestMainArmTemplateFile) =>
            new(MockLoggerFactory.CreateLogger(), latestMainArmTemplateFile);
    }
}
