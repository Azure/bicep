// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
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
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_NonBicepFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("generate-params", "/dev/zero");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($@"The specified input ""/dev/zero"" was not recognized as a Bicep file. Bicep files must use the {LanguageConstants.LanguageFileExtension} extension.");
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_OneParameterWithDefaultValue_Should_Succeed()
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
  ""contentVersion"": ""1.0.0.0""
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ExplicitIncludeParamsAll_OneParameterWithDefaultValue_Should_Succeed()
        {
            var bicep = $@"param name string = 'sampleparameter'";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--include-params", "all", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""name"": {
      ""value"": """"
    }
  }
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ExplicitIncludeParamsAll_OneParameterWithDefaultValueAndOneRequiredParameter_Should_Succeed()
        {
            var bicep = $@"param optional string = 'sampleparameter'
param required string";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--include-params", "all", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""optional"": {
      ""value"": """"
    },
    ""required"": {
      ""value"": """"
    }
  }
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_OneParameterWithoutDefaultValue_Should_Succeed()
        {
            var bicep = $@"param name string";

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
  ""parameters"": {
    ""name"": {
      ""value"": """"
    }
  }
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ExplicitIncludeParamsAll_OneParameterWithoutDefaultValue_Should_Succeed()
        {
            var bicep = $@"param name string";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--include-params", "all", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""name"": {
      ""value"": """"
    }
  }
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_TwoParameter_TwoDefaultValues_Should_Succeed()
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
  ""contentVersion"": ""1.0.0.0""
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_TwoParameter_OneDefaultValues_Should_Succeed()
        {
            var bicep = $@"param name string = 'sampleparameter'
                           param location string";

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
  ""parameters"": {
    ""location"": {
      ""value"": """"
    }
  }
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_TwoParameter_ZeroDefaultValues_Should_Succeed()
        {
            var bicep = $@"param name string
                           param location string";

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
  ""parameters"": {
    ""name"": {
      ""value"": """"
    },
    ""location"": {
      ""value"": """"
    }
  }
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_OneParameter_WithDefaultValue_ExistingParamsFileWithTheSameParameter_Should_Succeed()
        {
            var bicep = $@"param name string = 'sampleparameter'";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var existingParamsFilePath = Path.Combine(tempDirectory, "built.parameters.json");
            File.WriteAllText(existingParamsFilePath, @"{ ""parameters"": {
""name"": { ""value"": ""existingparameter"" }
} }".ReplaceLineEndings());

            var (output, error, result) = await Bicep("generate-params", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0""
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_OneParameter_WithoutDefaultValue_ExistingParamsFileWithTheSameParameter_Should_Succeed()
        {
            var bicep = $@"param name string";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var existingParamsFilePath = Path.Combine(tempDirectory, "built.parameters.json");
            File.WriteAllText(existingParamsFilePath, @"{ ""parameters"": {
""name"": { ""value"": ""existingparameter"" }
} }".ReplaceLineEndings());

            var (output, error, result) = await Bicep("generate-params", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""name"": {
      ""value"": ""existingparameter""
    }
  }
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_WithDevaultValue_InvalidExistingParamsFile_Should_Succeed()
        {
            var bicep = $@"param name string = 'sampleparameter'";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var existingParamsFilePath = Path.Combine(tempDirectory, "built.parameters.json");
            File.WriteAllText(existingParamsFilePath, "{INVALID}".ReplaceLineEndings());

            var (output, error, result) = await Bicep("generate-params", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0""
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_WithoutDevaultValue_InvalidExistingParamsFile_Should_Succeed()
        {
            var bicep = $@"param name string";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var existingParamsFilePath = Path.Combine(tempDirectory, "built.parameters.json");
            File.WriteAllText(existingParamsFilePath, "{INVALID}".ReplaceLineEndings());

            var (output, error, result) = await Bicep("generate-params", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""name"": {
      ""value"": """"
    }
  }
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_WithoutDefaultValue_ExistingParamsFile_Should_KeepContentVersion()
        {
            var bicep = $@"param name string";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var existingParamsFilePath = Path.Combine(tempDirectory, "built.parameters.json");
            File.WriteAllText(existingParamsFilePath, @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""2.0.0.0"",
  ""parameters"": {
    ""name"": {
      ""value"": """"
    }
  }
}".ReplaceLineEndings());

            var (output, error, result) = await Bicep("generate-params", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""2.0.0.0"",
  ""parameters"": {
    ""name"": {
      ""value"": """"
    }
  }
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_OneParameter_ExistingParamsFileWithExtraParameter_Should_RemoveExtraParameter()
        {
            var bicep = $@"param name string = 'sampleparameter'";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var existingParamsFilePath = Path.Combine(tempDirectory, "built.parameters.json");
            File.WriteAllText(existingParamsFilePath, @"{ ""parameters"": {
""name"": { ""value"": ""existingparameter"" },
""location"": { ""value"": ""existinglocation"" }
} }".ReplaceLineEndings());

            var (output, error, result) = await Bicep("generate-params", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0""
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_OneParameter_ExistingParamsFileWithDifferentParameters_Should_RemoveExtraParameters()
        {
            var bicep = $@"param name string = 'sampleparameter'";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var existingParamsFilePath = Path.Combine(tempDirectory, "built.parameters.json");
            File.WriteAllText(existingParamsFilePath, @"{ ""parameters"": {
""param"": { ""value"": ""existingparameter"" },
""location"": { ""value"": ""existinglocation"" }
} }".ReplaceLineEndings());

            var (output, error, result) = await Bicep("generate-params", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0""
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_UserDefinedTypeParameters_JsonFormat_Should_Succeed()
        {
            var bicep = $@"
                param foo object = {{ bar: 'hello', baz: 42 }}
                param bar {{ bar: string, baz: int }}
                param baz {{ bar: string, baz: int }} = {{ bar: 'hello', baz: 42 }}
            ";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--include-params", "all", "--output-format", "json", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""foo"": {
      ""value"": {}
    },
    ""bar"": {
      ""value"": {
        ""bar"": """",
        ""baz"": 0
      }
    },
    ""baz"": {
      ""value"": {
        ""bar"": """",
        ""baz"": 0
      }
    }
  }
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ImplicitOutputFormatJson_ImplicitIncludeParamsRequiredOnly_file_should_be_overwritten_in_full()
        {
            // https://github.com/Azure/bicep/issues/7239

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "main.bicep");
            File.WriteAllText(bicepFilePath, @"
param foo string
param bar string
");
            var (output, error, result) = await Bicep("generate-params", bicepFilePath);

            File.WriteAllText(bicepFilePath, @"
param foo string
// param bar string
");
            (output, error, result) = await Bicep("generate-params", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "main.parameters.json")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""foo"": {
      ""value"": """"
    }
  }
}".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_OutputFormatBicepParam_ZeroFiles_ShouldFail_WithExpectedErrorMessage()
        {
            var (output, error, result) = await Bicep("generate-params", "--output-format", "bicepparam");

            using (new AssertionScope())
            {
                result.Should().Be(1);
                output.Should().BeEmpty();

                error.Should().NotBeEmpty();
                error.Should().Contain($"The input file path was not specified");
            }
        }

        [TestMethod]
        public async Task GenerateParams_ExplicitOutputFormatBicepParam_ImplicitIncludeParamsRequiredOnly_OneParameterWithDefaultValue_Should_Succeed()
        {
            var bicep = $@"param name string = 'sampleparameter'";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--output-format", "bicepparam", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.bicepparam")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"using './built.bicep'

".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ExplicitOutputFormatBicepParam_ExplicitIncludeParamsAll_OneParameterWithDefaultValue_Should_Succeed()
        {
            var bicep = $@"param name string = 'sampleparameter'";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--output-format", "bicepparam", "--include-params", "all", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.bicepparam")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"using './built.bicep'

param name = 'sampleparameter'

".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ExplicitOutputFormatBicepParam_ExplicitIncludeParamsAll_OneStringParameterWithoutDefaultValue_Should_Succeed()
        {
            var bicep = $@"param name string";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--output-format", "bicepparam", "--include-params", "all", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.bicepparam")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"using './built.bicep'

param name = ''

".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ExplicitOutputFormatBicepParam_ExplicitIncludeParamsAll_OneIntegerParameterWithoutDefaultValue_Should_Succeed()
        {
            var bicep = $@"param count int";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--output-format", "bicepparam", "--include-params", "all", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.bicepparam")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"using './built.bicep'

param count = 0

".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ExplicitOutputFormatBicepParam_ExplicitIncludeParamsAll_OneBooleanParameterWithoutDefaultValue_Should_Succeed()
        {
            var bicep = $@"param enabled bool";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--output-format", "bicepparam", "--include-params", "all", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.bicepparam")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"using './built.bicep'

param enabled = false

".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ExplicitOutputFormatBicepParam_ExplicitIncludeParamsAll_OneObjectParameterWithoutDefaultValue_Should_Succeed()
        {
            var bicep = $@"param enabled object";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--output-format", "bicepparam", "--include-params", "all", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.bicepparam")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"using './built.bicep'

param enabled = {}

".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_ExplicitOutputFormatBicepParam_ExplicitIncludeParamsAll_OneArrayParameterWithoutDefaultValue_Should_Succeed()
        {
            var bicep = $@"param enabled array";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--output-format", "bicepparam", "--include-params", "all", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.bicepparam")).ReplaceLineEndings();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"using './built.bicep'

param enabled = []

".ReplaceLineEndings());
            }
        }

        [TestMethod]
        public async Task GenerateParams_UserDefinedTypeParameters_BicepparamFormat_Should_Succeed()
        {
            var bicep = $@"
                param foo object = {{ bar: 'hello', baz: 42 }}
                param bar {{ bar: string, baz: int }}
                param baz {{ bar: string, baz: int }} = {{ bar: 'hello', baz: 42 }}
            ";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--include-params", "all", "--output-format", "bicepparam", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.bicepparam")).ReplaceLineEndings().Trim();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"using './built.bicep'

param foo = {
  bar: 'hello'
  baz: 42
}
param bar = {
  bar: ''
  baz: 0
}
param baz = {
  bar: 'hello'
  baz: 42
}
".ReplaceLineEndings().Trim());
            }
        }

        [TestMethod]
        public async Task GenerateParams_UserDefinedTypeParameters_ObjectInObject_BicepparamFormat_Should_Succeed()
        {
            var bicep = $@"
                param bar {{ bar: string, baz: {{ foo: int }} }}
                param baz {{ bar: string, baz: {{ foo: {{ bar: int }} }} }}
                param bat {{ bar: string, baz: {{ foo: {{ bar: {{ baz: string }} }} }} }}
                param bab {{ bar: string, baz: {{ foo: int }} }} = {{ bar: 'hello', baz: {{ foo: 42 }} }}
            ";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--include-params", "all", "--output-format", "bicepparam", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.bicepparam")).ReplaceLineEndings().Trim();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"using './built.bicep'

param bar = {
  bar: ''
  baz: {
    foo: 0
  }
}
param baz = {
  bar: ''
  baz: {
    foo: {
      bar: 0
    }
  }
}
param bat = {
  bar: ''
  baz: {
    foo: {
      bar: {
        baz: ''
      }
    }
  }
}
param bab = {
  bar: 'hello'
  baz: {
    foo: 42
  }
}
".ReplaceLineEndings().Trim());
            }
        }

        [TestMethod]
        public async Task GenerateParams_WithDecorator_BicepparamFormat_Should_Succeed()
        {
            var bicep = $@"
                @allowed([5, 10])
                param foo int
            ";

            var tempDirectory = FileHelper.GetUniqueTestOutputPath(TestContext);
            Directory.CreateDirectory(tempDirectory);

            var bicepFilePath = Path.Combine(tempDirectory, "built.bicep");
            File.WriteAllText(bicepFilePath, bicep);

            var (output, error, result) = await Bicep("generate-params", "--include-params", "all", "--output-format", "bicepparam", bicepFilePath);

            var content = File.ReadAllText(Path.Combine(tempDirectory, "built.bicepparam")).ReplaceLineEndings().Trim();

            using (new AssertionScope())
            {
                result.Should().Be(0);

                content.Should().Be(@"using './built.bicep'

param foo = 5
".ReplaceLineEndings().Trim());
            }
        }
    }
}
