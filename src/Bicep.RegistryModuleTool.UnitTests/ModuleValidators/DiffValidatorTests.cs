// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleValidators;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text.RegularExpressions;
using static FluentAssertions.FluentActions;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleValidators
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
                .Throw<InvalidModuleException>()
                .WithMessage($"The file \"{fileToValidate.Path}\" is modified or outdated. Please regenerate the file to fix it.{Environment.NewLine}");
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
                .Throw<InvalidModuleException>()
                .WithMessage($@"The file ""{fileToValidate.Path}"" is modified or outdated. Please regenerate the file to fix it.{Environment.NewLine}");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetEmptyExamplesSectionData), DynamicDataSourceType.Method)]
        public void Validate_EmptyExamplesSectionInReadmeFile_ThrowsException(string readmeFileContents)
        {
            this.fileSystem.AddFile(this.fileSystem.Path.GetFullPath(ReadmeFile.FileName), readmeFileContents);

            var fileToValidate = ReadmeFile.ReadFromFileSystem(this.fileSystem);

            Invoking(() => this.sut.Validate(fileToValidate)).Should()
                .Throw<InvalidModuleException>()
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
            this.fileSystem.AddFile(this.fileSystem.Path.GetFullPath(VersionFile.FileName), @"{
  ""$schema"": ""https://aka.ms/bicep-registry-module-version-file-schema#"",
  ""version"": ""1.1"",
  ""pathFilters"": [
    ""./main.json""
  ]
}");

            var fileToValidate = VersionFile.ReadFromFileSystem(this.fileSystem);

            Invoking(() => this.sut.Validate(fileToValidate)).Should()
                .Throw<InvalidModuleException>()
                .WithMessage($@"The file ""{fileToValidate.Path}"" is modified or outdated. Please regenerate the file to fix it.{Environment.NewLine}");
        }

        private static IEnumerable<object[]> GetEmptyExamplesSectionData()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            var validReadmeFile = ReadmeFile.ReadFromFileSystem(fileSystem);

            var emptyExamplesSections = new string[]
            {
                "## Examples",
                "## Examples\r\n\r\n",
                "## Examples\r\n\t   ",
                "## Examples\n  \n",
            };


            foreach (var section in emptyExamplesSections)
            {
                yield return new object[]
                {
                    Regex.Replace(validReadmeFile.Contents, "## Examples.+", section, RegexOptions.Singleline)
                };
            }
        }
    }
}
