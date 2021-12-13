// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.Core.Json;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Extensions;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Factories;
using Bicep.RegistryModuleTool.UnitTests.TestFixtures.Mocks;
using FluentAssertions;
using Json.More;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO.Abstractions.TestingHelpers;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleFileValidators
{
    [TestClass]
    public class DescriptionsValidatorTests
    {
        private readonly static MockFileSystem FileSystem = MockFileSystemFactory.CreateMockFileSystem();

        [TestMethod]
        public void Validate_ValidMainBicepFile_Succeeds()
        {
            var file = MainBicepFile.ReadFromFileSystem(FileSystem);
            var sut = new DescriptionsValidator(MockLogger.Create(), MainArmTemplateFile.ReadFromFileSystem(FileSystem));

            FluentActions.Invoking(() => sut.Validate(file)).Should().NotThrow();
        }

        [TestMethod]
        public void Validate_MissingDescriptions_ThrowsException()
        {
            var mainBicepFile = MainBicepFile.ReadFromFileSystem(FileSystem);

            var mainArmTemplateFile = MainArmTemplateFile.ReadFromFileSystem(FileSystem);
            var mainArmTemplateFileElement = JsonElementFactory.CreateElement(mainArmTemplateFile.Content);

            // Remove some descriptions.
            var patchedElement = mainArmTemplateFileElement.Patch(
                PatchOperations.Remove("/parameters/sshRSAPublicKey/metadata/description"),
                PatchOperations.Remove("/parameters/clusterName/metadata/description"),
                PatchOperations.Remove("/parameters/osDiskSizeGB/metadata/description"),
                PatchOperations.Remove("/outputs/controlPlaneFQDN/metadata/description"));

            var tempFileSystem = new MockFileSystem();
            tempFileSystem.AddFile(mainArmTemplateFile.Path, patchedElement.ToJsonString());
            tempFileSystem.Directory.SetCurrentDirectory(tempFileSystem.Path.GetDirectoryName(mainArmTemplateFile.Path));

            var modifiedArmTemplate = MainArmTemplateFile.ReadFromFileSystem(tempFileSystem);
            var sut = new DescriptionsValidator(MockLogger.Create(), modifiedArmTemplate);

            FluentActions.Invoking(() => sut.Validate(mainBicepFile)).Should()
                .Throw<BicepException>()
                .WithMessage(
$@"Descriptions for the following parameters are missing in ""{mainBicepFile.Path}"":
  - sshRSAPublicKey
  - clusterName
  - osDiskSizeGB

Descriptions for the following outputs are missing in ""{mainBicepFile.Path}"":
  - controlPlaneFQDN
".ReplaceLineEndings());
        }
    }
}
