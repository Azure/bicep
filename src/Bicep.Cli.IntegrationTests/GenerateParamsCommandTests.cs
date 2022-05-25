// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class GenerateParamsCommandTests : TestBase
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task GenerateParams_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("generate-params");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($"The input file path was not specified");
            }
        }

        [TestMethod]
        public async Task GenerateParams_NonBicepFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("generate-params", "/dev/zero");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($@"The specified input ""/dev/zero"" was not recognized as a bicep file. Bicep files must use the {LanguageConstants.LanguageFileExtension} extension.");
            }
        }

        [TestMethod]
        public async Task GenerateParams_OneParameter_Should_Succeed()
        {
            var bicep = $@"param name string = 'sampleparameter'";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""dev""
    }
  },
  ""parameters"": {
    ""name"": {
      ""value"": ""sampleparameter""
    }
  }
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_TwoParameter_Should_Succeed()
        {
            var bicep = $@"param name string = 'sampleparameter'
                           param location string = 'westus2'";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""dev""
    }
  },
  ""parameters"": {
    ""name"": {
      ""value"": ""sampleparameter""
    },
    ""location"": {
      ""value"": ""westus2""
    }
  }
}".ReplaceLineEndings());
            }
        }
    }
}
