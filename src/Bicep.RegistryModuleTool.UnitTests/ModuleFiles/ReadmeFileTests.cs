// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.ModuleFiles;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using Bicep.RegistryModuleTool.TestFixtures.MockFactories;
using FluentAssertions;
using Json.More;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Exceptions;

namespace Bicep.RegistryModuleTool.UnitTests.ModuleFiles
{
    [TestClass]
    public class ReadmeFileTests
    {
        [TestMethod]
        public void ReadmeFileGenerate_ShouldReplaceDescriptionWithDetails()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            fileSystem.AddFile(fileSystem.Path.GetFullPath(ReadmeFile.FileName), ReadmeFile.OpenAsync(fileSystem).Content.Replace("## Details", "## Description"));
            ReadmeFile.OpenAsync(fileSystem).Content.Should().Contain("## Description");
            ReadmeFile.OpenAsync(fileSystem).Content.Should().NotContain("## Details");

            var generatedFile = ReadmeFile.Generate(fileSystem, MainArmTemplateFile.OpenAsync(fileSystem));

            generatedFile.Contents.Should().Contain("## Details");
            generatedFile.Contents.Should().NotContain("## Description");
            generatedFile.Contents.Should().Contain("The quick brown fox jumps over the lazy dog");
        }

        [TestMethod]
        public void ReadmeFileGenerate_ShouldNotHaveBothDetailsAndDescriptionSections()
        {
            var fileSystem = MockFileSystemFactory.CreateFileSystemWithValidFiles();
            fileSystem.AddFile(fileSystem.Path.GetFullPath(ReadmeFile.FileName), ReadmeFile.OpenAsync(fileSystem).Content + "\n## Description\n\nMy description");
            ReadmeFile.OpenAsync(fileSystem).Content.Should().Contain("## Description");
            ReadmeFile.OpenAsync(fileSystem).Content.Should().Contain("## Details");

            FluentActions.Invoking(() => ReadmeFile.Generate(fileSystem, MainArmTemplateFile.OpenAsync(fileSystem))).Should()
                .Throw<BicepException>()
                .WithMessage("The readme file *README.md must not contain both a Description and a Details section.");
        }
    }
}
