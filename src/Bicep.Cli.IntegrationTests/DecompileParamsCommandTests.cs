// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class DecompileParamsCommandTests : TestBase
    {
        private readonly string[] DecompilationDisclaimer =
        [
            "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep Template or Bicep Parameters.",
            "You may need to fix warnings and errors in the generated bicep/bicepparam file(s), or decompilation may fail entirely if an accurate conversion is not possible.",
            "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues."
        ];


        [TestMethod]
        public async Task Decompile_ValidParamFile_ShouldSucceed()
        {
            var paramFile =
  @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""first"": {
      ""value"": ""test""
    },
    ""second"": {
      ""value"": 1
    },
    ""third"" : {
      ""value"" : [
        1,
        ""foo""
      ]
    },
    ""fourth"" : {
      ""value"" : {
        ""firstKey"" : ""bar"",
        ""secondKey"" : 1
      }
    }
  }
}";
            var expectedOutput =
                """
                using '' /*TODO: Provide a path to a bicep template*/

                param first = 'test'

                param second = 1

                param third = [
                  1
                  'foo'
                ]

                param fourth = {
                  firstKey: 'bar'
                  secondKey: 1
                }

                """;

            var (jsonPath, bicepparamPath) = Setup(TestContext, paramFile);

            var (output, error, result) = await Bicep("decompile-params", jsonPath);

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.AsLines().Should().Contain(DecompilationDisclaimer);
                result.Should().Be(0);
                File.ReadAllText(bicepparamPath).Should().BeEquivalentToIgnoringNewlines(expectedOutput);
            }
        }

        [TestMethod]
        public async Task Decompile_ValidParamFileWithBicepPath_ShouldSucceed()
        {
            var paramFile =
              """
              {
                "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                "contentVersion": "1.0.0.0",
                "parameters": {
                  "foo": {
                    "value": "bar"
                  }
                }
              }
              """;
            var expectedOutput =
                """
                using 'dir/main.bicep'

                param foo = 'bar'

                """;

            var (jsonPath, bicepparamPath) = Setup(TestContext, paramFile);
            var bicepPath = PathHelper.ResolvePath("./dir/main.bicep", Path.GetDirectoryName(jsonPath));

            var (output, error, result) = await Bicep("decompile-params", jsonPath, "--bicep-file", bicepPath);

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.AsLines().Should().Contain(DecompilationDisclaimer);
                result.Should().Be(0);
                File.ReadAllText(bicepparamPath).Should().BeEquivalentToIgnoringNewlines(expectedOutput);
            }
        }

        [TestMethod]
        public async Task Decompile_ValidParamFileWithStdOut_ShouldSucceed()
        {
            var paramFile =
                """
                {
                "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                "contentVersion": "1.0.0.0",
                "parameters": {
                  "foo": {
                    "value": "bar"
                  }
                }
                }
                """;
            var expectedOutput =
                """
                using '' /*TODO: Provide a path to a bicep template*/

                param foo = 'bar'

                """;

            var (jsonPath, bicepparamPath) = Setup(TestContext, paramFile);

            var (output, error, result) = await Bicep("decompile-params", jsonPath, "--stdout");

            using (new AssertionScope())
            {
                output.Should().Be(expectedOutput);
                error.AsLines().Should().Contain(DecompilationDisclaimer);
                result.Should().Be(0);
            }
        }

        [TestMethod]
        public async Task Decompile_InvalidParamFile_ShouldFailWithErrors()
        {
            var paramFile =
  @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""foo"": { }
  }
}";

            var (jsonPath, bicepparamPath) = Setup(TestContext, paramFile);

            var (output, error, result) = await Bicep("decompile-params", jsonPath);

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.AsLines().Should().Contain(DecompilationDisclaimer);
                error.AsLines().Should().Contain($"{jsonPath}: Decompilation failed with fatal error \"[5:10]: No value found parameter foo\"");
                result.Should().Be(1);
            }
        }

        [TestMethod]
        public async Task Decompile_ValidParamFileWithCustomOutFile_ShouldSucceed()
        {
            var paramFile =
                """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                  "contentVersion": "1.0.0.0",
                  "parameters": {
                    "location": {
                      "value": "eastus"
                    }
                  }
                }
                """;

            var expectedOutput =
                """
                using '' /*TODO: Provide a path to a bicep template*/

                param location = 'eastus'

                """;

            var (jsonPath, _) = Setup(TestContext, paramFile);
            var customOutputPath = FileHelper.GetResultFilePath(TestContext, "custom-name.bicepparam");

            var (output, error, result) = await Bicep("decompile-params", jsonPath, "--outfile", customOutputPath);

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.AsLines().Should().Contain(DecompilationDisclaimer);
                result.Should().Be(0);
                File.Exists(customOutputPath).Should().BeTrue();
                File.ReadAllText(customOutputPath).Should().BeEquivalentToIgnoringNewlines(expectedOutput);
            }
        }

        [TestMethod]
        public async Task Decompile_CustomOutFileAlreadyExists_ShouldFailWithoutForce()
        {
            var paramFile =
                """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                  "contentVersion": "1.0.0.0",
                  "parameters": {
                    "location": {
                      "value": "eastus"
                    }
                  }
                }
                """;

            var (jsonPath, _) = Setup(TestContext, paramFile);
            var customOutputPath = FileHelper.GetResultFilePath(TestContext, "existing-file.bicepparam");
            
            // Create the file that already exists
            File.WriteAllText(customOutputPath, "existing content");

            var (output, error, result) = await Bicep("decompile-params", jsonPath, "--outfile", customOutputPath);

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.Should().Contain($"The output file \"{customOutputPath}\" already exists. Use --force to overwrite the existing file.");
                result.Should().Be(1);
                File.ReadAllText(customOutputPath).Should().Be("existing content"); // Should not be overwritten
            }
        }

        [TestMethod]
        public async Task Decompile_CustomOutFileAlreadyExists_ShouldSucceedWithForce()
        {
            var paramFile =
                """
                {
                  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
                  "contentVersion": "1.0.0.0",
                  "parameters": {
                    "location": {
                      "value": "eastus"
                    }
                  }
                }
                """;

            var expectedOutput =
                """
                using '' /*TODO: Provide a path to a bicep template*/

                param location = 'eastus'

                """;

            var (jsonPath, _) = Setup(TestContext, paramFile);
            var customOutputPath = FileHelper.GetResultFilePath(TestContext, "force-overwrite.bicepparam");
            
            // Create the file that already exists
            File.WriteAllText(customOutputPath, "existing content");

            var (output, error, result) = await Bicep("decompile-params", jsonPath, "--outfile", customOutputPath, "--force");

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.AsLines().Should().Contain(DecompilationDisclaimer);
                result.Should().Be(0);
                File.ReadAllText(customOutputPath).Should().BeEquivalentToIgnoringNewlines(expectedOutput); // Should be overwritten
            }
        }

        private static (string jsonPath, string bicepparamPath) Setup(TestContext context, string template, string? inputFile = null, string? outputDir = null)
        {
            var jsonPath = FileHelper.SaveResultFile(context, inputFile ?? "param.json", template);

            string bicepparamPath;

            if (outputDir is null)
            {
                bicepparamPath = PathHelper.GetBicepparamOutputPath(jsonPath);
            }
            else
            {
                bicepparamPath = FileHelper.GetResultFilePath(context, outputDir);
            }

            return (jsonPath, bicepparamPath);
        }
    }
}
