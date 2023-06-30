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
using System;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleValidators
{
    [TestClass]
    public class BicepMetadataValidatorTests
    {
        [DataTestMethod]
        [DataRow(null, "*The following metadata statements are missing:\n  - metadata owner")]
        [DataRow("a", null)]
        [DataRow("", "*\"metadata owner\" must be a GitHub username or a team under the Azure organization.")]
        [DataRow("StevieWeatherfjord", null)]
        [DataRow("Azure/StevieWeatherfjord", null)]
        [DataRow("Azurey/StevieWeatherfjord", "*\"metadata owner\" must be a GitHub username or a team under the Azure organization.")]
        [DataRow("/StevieWeatherfjord", "*\"metadata owner\" must be a GitHub username or a team under the Azure organization.")]
        [DataRow("StevieWeatherfjord/", "*\"metadata owner\" must be a GitHub username or a team under the Azure organization.")]
        [DataRow("a1-b-c-d-e", null)]
        [DataRow("Azure/a/a", "*\"metadata owner\" must be a GitHub username or a team under the Azure organization.")]
        [DataRow("Azure/זרוע", "*\"metadata owner\" must be a GitHub username or a team under the Azure organization.")]
        [DataRow("azure/a1-b-c-d-e", "*\"metadata owner\" must be a GitHub username or a team under the Azure organization.")]
        [DataRow("A1--b-c-d-e", "*\"metadata owner\" must be a GitHub username or a team under the Azure organization.")]
        [DataRow("אנואוהבBicep", "*\"metadata owner\" must be a GitHub username or a team under the Azure organization.")]
        public void MetadataValidation_Owner(string? owner, string? expectedMessagePattern)
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            var mainBicepFile = MainBicepFile.ReadFromFileSystem(fileSystem);

            // The BicepMetadataValidator doesn't actually look at the metadata compiled
            // into the main.json file from the main.bicep, so we will modify the main.json
            // into what we want to test.
            var mainArmTemplateFile = ReplaceMetadataInArmTemplateFile(fileSystem, "owner", owner);

            var sut = new BicepMetadataValidator(MockLoggerFactory.CreateLogger(), mainBicepFile);

            if (expectedMessagePattern is null)
            {
                FluentActions.Invoking(() => sut.Validate(mainArmTemplateFile)).Should().NotThrow();
            }
            else
            {
                FluentActions.Invoking(() => sut.Validate(mainArmTemplateFile)).Should()
                    .Throw<InvalidModuleException>()
                    .WithMessage(expectedMessagePattern);
            }
        }

        [DataTestMethod]
        [DataRow(null, "*main.bicep*The following metadata statements are missing:\n  - metadata name")]
        [DataRow("", "*main.bicep*\"metadata name\" must contain at least 10 characters.")]
        [DataRow("123456789", "*\"metadata name\" must contain at least 10 characters.")]
        [DataRow("1234567890", null)]
        [DataRow("12345678901", null)]
        [DataRow("12345678901234567890123456789012345678901234567890123456789", null)]
        [DataRow("123456789012345678901234567890123456789012345678901234567890", null)]
        [DataRow("1234567890123456789012345678901234567890123456789012345678901", "*main.bicep*\"metadata name\" must contain no more than 60 characters.")]
        [DataRow("אני אוהב Bicep", null)]
        public void MetadataValidation_Name(string? name, string? expectedMessagePattern)
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            var mainBicepFile = MainBicepFile.ReadFromFileSystem(fileSystem);

            // The BicepMetadataValidator doesn't actually look at the metadata compiled
            // into the main.json file from the main.bicep, so we will modify the main.json
            // into what we want to test.
            var mainArmTemplateFile = ReplaceMetadataInArmTemplateFile(fileSystem, "name", name);

            var sut = new BicepMetadataValidator(MockLoggerFactory.CreateLogger(), mainBicepFile);

            if (expectedMessagePattern is null)
            {
                FluentActions.Invoking(() => sut.Validate(mainArmTemplateFile)).Should().NotThrow();
            }
            else
            {
                FluentActions.Invoking(() => sut.Validate(mainArmTemplateFile)).Should()
                    .Throw<InvalidModuleException>()
                    .WithMessage(expectedMessagePattern);
            }
        }

        [DataTestMethod]
        [DataRow(null, "*main.bicep*The following metadata statements are missing:\n  - metadata description")]
        [DataRow("", "*main.bicep*\"metadata description\" must contain at least 10 characters.")]
        [DataRow("123456789", "*\"metadata description\" must contain at least 10 characters.")]
        [DataRow("1234567890", null)]
        [DataRow("12345678901", null)]
        [DataRow("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789", null)]
        [DataRow("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", null)]
        [DataRow("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901", "*main.bicep*\"metadata description\" must contain no more than 120 characters.")]
        [DataRow("אני אוהב Bicep", null)]
        public void MetadataValidation_Description(string? description, string? expectedMessagePattern)
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            var mainBicepFile = MainBicepFile.ReadFromFileSystem(fileSystem);

            // The BicepMetadataValidator doesn't actually look at the metadata compiled
            // into the main.json file from the main.bicep, so we will modify the main.json
            // into what we want to test.
            var mainArmTemplateFile = ReplaceMetadataInArmTemplateFile(fileSystem, "description", description);

            var sut = new BicepMetadataValidator(MockLoggerFactory.CreateLogger(), mainBicepFile);

            if (expectedMessagePattern is null)
            {
                FluentActions.Invoking(() => sut.Validate(mainArmTemplateFile)).Should().NotThrow();
            }
            else
            {
                FluentActions.Invoking(() => sut.Validate(mainArmTemplateFile)).Should()
                    .Throw<InvalidModuleException>()
                    .WithMessage(expectedMessagePattern);
            }
        }

        public static MainArmTemplateFile ReplaceMetadataInArmTemplateFile(MockFileSystem fileSystem, string metadataName, string? metadataValue)
        {
            // The BicepMetadataValidator doesn't actually look at the metadata compiled
            // into the main.json file from the main.bicep, so we will modify the main.json
            // into what we want to test.
            var mainArmTemplateFile = MainArmTemplateFile.ReadFromFileSystem(fileSystem);
            var mainArmTemplateFileElement = JsonElementFactory.CreateElement(mainArmTemplateFile.Content);
            if (metadataValue is not null)
            {
                mainArmTemplateFileElement = mainArmTemplateFileElement.Patch(JsonPatchOperations.Add($"/metadata/{metadataName}", metadataValue));
            }
            else
            {
                mainArmTemplateFileElement = mainArmTemplateFileElement.Patch(JsonPatchOperations.Remove($"/metadata/{metadataName}"));
            }

            fileSystem.AddFile(mainArmTemplateFile.Path, mainArmTemplateFileElement.ToJsonString());
            return MainArmTemplateFile.ReadFromFileSystem(fileSystem);
        }
    }
}
