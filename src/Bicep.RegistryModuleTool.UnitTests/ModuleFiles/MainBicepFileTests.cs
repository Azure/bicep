// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleValidators;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using Bicep.RegistryModuleTool.UnitTests.ModuleValidators;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text.RegularExpressions;
using static FluentAssertions.FluentActions;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleFiles
{
    [TestClass]
    public class MainBicepFileTests
    {
        [TestMethod]
        public void BicepGenerate_IfMetadataFileExists_MoveToBicep_AndDeleteMetadataFile()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithModuleWithObsoleteMetadataFile();
            var armTemplateFile = MainArmTemplateFile.ReadFromFileSystem(fileSystem);
            var metadataFile = MetadataFile.TryReadFromFileSystem(fileSystem)!;

            // act
            var bicepFile = MainBicepFile.Generate(fileSystem, metadataFile, armTemplateFile);

            MetadataFile.TryReadFromFileSystem(fileSystem).Should().BeNull("metadata file should have been deleted");
            bicepFile.Contents.Should().Be(
@"metadata name = 'Sample module'
metadata description = 'Sample summary'
metadata owner = 'test'

// My Bicep file
");
        }

        [TestMethod]
        public void BicepGenerate_IfMetadataFileExists_ButBicepAlreadyContainsSomeMetadata_MoveTheMissingOnes_AndDeleteMetadataFile()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithModuleWithObsoleteMetadataFile();
            var metadataFile = MetadataFile.TryReadFromFileSystem(fileSystem)!;

            fileSystem.AddFile(
                fileSystem.Path.GetFullPath(MainBicepFile.FileName),
@"metadata description = 'My description in Bicep'

// My Bicep file
");
            var armTemplateFile = BicepMetadataValidatorTests.ReplaceMetadataInArmTemplateFile(fileSystem,
                "description", "My description in Bicep");

            // act
            var bicepFile = MainBicepFile.Generate(fileSystem, metadataFile, armTemplateFile);

            MetadataFile.TryReadFromFileSystem(fileSystem).Should().BeNull("metadata file should have been deleted");
            bicepFile.Contents.Should().Be(
@"metadata name = 'Sample module'
metadata owner = 'test'
metadata description = 'My description in Bicep'

// My Bicep file
");
        }

        [TestMethod]
        public void BicepGenerate_AcceptDescriptionInsteadOfSummaryInMetadataFile()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithModuleWithObsoleteMetadataFile();
            fileSystem.AddFile(
                fileSystem.Path.GetFullPath(MetadataFile.FileName),
                @"{
                  ""$schema"": ""https://aka.ms/bicep-registry-module-metadata-file-schema-v2#"",
                  ""name"": ""Sample module"",
                  ""description"": ""Sample description"",
                  ""owner"": ""test""
                }");
            var armTemplateFile = MainArmTemplateFile.ReadFromFileSystem(fileSystem);
            var metadataFile = MetadataFile.TryReadFromFileSystem(fileSystem)!;

            // act
            var bicepFile = MainBicepFile.Generate(fileSystem, metadataFile, armTemplateFile);

            MetadataFile.TryReadFromFileSystem(fileSystem).Should().BeNull("metadata file should have been deleted");
            bicepFile.Contents.Should().Be(
@"metadata name = 'Sample module'
metadata description = 'Sample description'
metadata owner = 'test'

// My Bicep file
");
        }

        [TestMethod]
        public void BicepGenerate_EscapeCharacters()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithModuleWithObsoleteMetadataFile();
            fileSystem.AddFile(
                fileSystem.Path.GetFullPath(MetadataFile.FileName),
                @"{
                  ""$schema"": ""https://aka.ms/bicep-registry-module-metadata-file-schema-v2#"",
                  ""name"": ""Not your mother's \""module\"""",
                  ""summary"": ""Sample\ndescription\r\nwith\twhitespace"",
                  ""owner"": ""test""
                }");
            var a = fileSystem.File.ReadAllText(fileSystem.Path.GetFullPath(MetadataFile.FileName));
            var armTemplateFile = MainArmTemplateFile.ReadFromFileSystem(fileSystem);
            var metadataFile = MetadataFile.TryReadFromFileSystem(fileSystem)!;

            // act
            var bicepFile = MainBicepFile.Generate(fileSystem, metadataFile, armTemplateFile);

            MetadataFile.TryReadFromFileSystem(fileSystem).Should().BeNull("metadata file should have been deleted");
            bicepFile.Contents.Should().Be(
@"metadata name = 'Not your mother\'s ""module""'
metadata description = 'Sample\ndescription\r\nwith\twhitespace'
metadata owner = 'test'

// My Bicep file
");
        }
    }
}
