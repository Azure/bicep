// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Cli.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bicep.Core.FileSystem;
using System;

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class DecompileTests
    {
        private const string ValidTemplate =  @"{
                ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
                ""contentVersion"": ""1.0.0.0"",
                ""parameters"": {},
                ""variables"": {},
                ""resources"": [
                    {
                        ""type"": ""My.Rp/testType"",
                        ""apiVersion"": ""2020-01-01"",
                        ""name"": ""resName"",
                        ""location"": ""[resourceGroup().location]"",
                        ""properties"": {
                            ""prop1"": ""val1""
                        }
                    }
                ],
                ""outputs"": {}
            }";

        private const string InvalidTemplate = @"{
            ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
            ""contentVersion"": ""1.0.0.0"",
            ""parameters"": {},
            ""variables"": {},
            ""resources"": [
                {
                    ""type"": ""My.Rp/testType"",
                    ""apiVersion"": ""2020-01-01"",
                    ""name"": ""resName"",
                    ""properties"": {
                        ""cyclicDependency"": ""[reference(resourceId('My.Rp/testType', 'resName'))]""
                    }
                }
            ],
            ""outputs"": {}
        }";

        private const string ValidTemplateExpectedDecompilation = @"resource resName 'My.Rp/testType@2020-01-01' = {
  name: 'resName'
  location: resourceGroup().location
  properties: {
    prop1: 'val1'
  }
}";

        private const string InvalidTemplateExpectedDecompilation = @"resource resName 'My.Rp/testType@2020-01-01' = {
  name: 'resName'
  properties: {
    cyclicDependency: resName.properties
  }
}";

        [NotNull]
        public TestContext? TestContext { get; set; }

        private static (IEnumerable<string> outputLines, IEnumerable<string> errorLines, int result) ExecuteProgram(params string[] args)
        {
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((outputWriter, errorWriter) =>
            {
                var program = new Program(TestTypeHelper.CreateEmptyProvider(), outputWriter, errorWriter, BicepTestConstants.DevAssemblyFileVersion);

                return program.Run(args);
            });

            return (
                Regex.Split(output, "\r?\n").Where(x => x != ""),
                Regex.Split(error, "\r?\n").Where(x => x != ""),
                result);
        }

        [TestMethod]
        public void Decompilation_of_empty_template_succeeds()
        {
            var template = @"{
    ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
    ""contentVersion"": ""1.0.0.0"",
    ""parameters"": {},
    ""variables"": {},
    ""resources"": [],
    ""outputs"": {},
    ""metadata"": {
        ""_generator"": {
            ""name"": ""bicep"",
            ""version"": ""dev"",
            ""templateHash"": ""<templateHash>""
        }
    }
}";

            var fileName = FileHelper.GetResultFilePath(TestContext, "main.json");
            File.WriteAllText(fileName, template);

            var (output, error, result) = ExecuteProgram("decompile", fileName);
            var bicepFileName = Path.ChangeExtension(fileName, "bicep");

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.Should().BeEquivalentTo(
                    "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.",
                    "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.",
                    "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.");
                result.Should().Be(0);
            }

            var bicepFile = File.ReadAllText(bicepFileName);
            bicepFile.Should().BeEquivalentTo("");
        }

        [TestMethod]
        public void Decompilation_of_file_with_errors()
        {
            var fileName = FileHelper.GetResultFilePath(TestContext, "main.json");
            File.WriteAllText(fileName, InvalidTemplate);

            var (output, error, result) = ExecuteProgram("decompile", fileName);
            var bicepFileName = Path.ChangeExtension(fileName, "bicep");

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                string.Join(string.Empty, error).Should().Contain("Error BCP079: This expression is referencing its own declaration, which is not allowed.");
                result.Should().Be(1);
            }

            var bicepFile = File.ReadAllText(bicepFileName);
            bicepFile.Should().BeEquivalentToIgnoringNewlines(InvalidTemplateExpectedDecompilation);
        }

        [TestMethod]
        public void Decompilation_of_file_with_no_errors()
        {
            var fileName = FileHelper.GetResultFilePath(TestContext, "main.json");
            File.WriteAllText(fileName, ValidTemplate);

            var (output, error, result) = ExecuteProgram("decompile", fileName);
            var bicepFileName = Path.ChangeExtension(fileName, "bicep");

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.Should().BeEquivalentTo(
                    "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.",
                    "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.",
                    "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.");
                result.Should().Be(0);
            }

            var bicepFile = File.ReadAllText(bicepFileName);
            bicepFile.Should().BeEquivalentToIgnoringNewlines(ValidTemplateExpectedDecompilation);
        }

        [TestMethod]
        public void Decompilation_of_file_with_errors_to_stdout()
        {
            var fileName = FileHelper.GetResultFilePath(TestContext, "main.json");
            File.WriteAllText(fileName, InvalidTemplate);

            var (output, error, result) = ExecuteProgram("decompile", "--stdout", fileName);

            using (new AssertionScope())
            {
                output.Should().BeEquivalentTo(
                    "resource resName 'My.Rp/testType@2020-01-01' = {",
                    "  name: 'resName'",
                    "  properties: {",
                    "    cyclicDependency: resName.properties",
                    "  }",
                    "}");
                string.Join(string.Empty, error).Should().Contain("Error BCP079: This expression is referencing its own declaration, which is not allowed.");
                result.Should().Be(1);
            }
        }

        [TestMethod]
        public void Decompilation_of_file_with_no_errors_to_stdout()
        {
            var fileName = FileHelper.GetResultFilePath(TestContext, "main.json");
            File.WriteAllText(fileName, ValidTemplate);

            var (output, error, result) = ExecuteProgram("decompile", "--stdout", fileName);

            using (new AssertionScope())
            {
                output.Should().BeEquivalentTo(
                    "resource resName 'My.Rp/testType@2020-01-01' = {",
                    "  name: 'resName'",
                    "  location: resourceGroup().location",
                    "  properties: {",
                    "    prop1: 'val1'",
                    "  }",
                    "}");
                error.Should().BeEquivalentTo(
                    "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.",
                    "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.",
                    "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.");
                result.Should().Be(0);
            }
        }

        [TestMethod]
        public void Decompilation_of_file_with_no_errors_to_outfile()
        {
            var fileName = FileHelper.GetResultFilePath(TestContext, "main.json");
            File.WriteAllText(fileName, ValidTemplate);

            var bicepFileName = Path.ChangeExtension(fileName, "bicep");

            var (output, error, result) = ExecuteProgram("decompile", "--outfile", bicepFileName, fileName);

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.Should().BeEquivalentTo(
                    "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.",
                    "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.",
                    "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.");
                result.Should().Be(0);
            }

            var bicepFile = File.ReadAllText(bicepFileName);
            bicepFile.Should().BeEquivalentToIgnoringNewlines(ValidTemplateExpectedDecompilation);
        }

        [TestMethod]
        public void Decompilation_of_file_with_no_errors_to_outdir()
        {
            var fileName = FileHelper.GetResultFilePath(TestContext, "main.json");
            File.WriteAllText(fileName, ValidTemplate);

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");
            Directory.CreateDirectory(outputFileDir);
            var expectedOutputFile = Path.Combine(outputFileDir, "main.bicep");

            var (output, error, result) = ExecuteProgram("decompile", "--outdir", outputFileDir, fileName);

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.Should().BeEquivalentTo(
                    "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.",
                    "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.",
                    "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.");
                result.Should().Be(0);
            }

            var bicepFile = File.ReadAllText(expectedOutputFile);
            bicepFile.Should().BeEquivalentToIgnoringNewlines(ValidTemplateExpectedDecompilation);
        }

        [TestMethod]
        public void Decompilation_of_file_with_no_errors_to_nonexistent_outdir()
        {
            var fileName = FileHelper.GetResultFilePath(TestContext, "main.json");
            File.WriteAllText(fileName, ValidTemplate);

            var outputFileDir = FileHelper.GetResultFilePath(TestContext, "outputdir");

            var (output, error, result) = ExecuteProgram("decompile", "--outdir", outputFileDir, fileName);

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.Should().BeEquivalentTo(
                    "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.",
                    "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.",
                    "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.",
                    $"The specified output directory \"{outputFileDir}\" does not exist.");
                result.Should().Be(1);
            }
        }

        [DataRow("DoesNotExist.json")]
        [DataRow("WrongDir\\Fake.json")]
        [DataTestMethod]
        public void Decompilation_of_invalid_input_paths_should_produce_expected_errors(string badPath)
        {
            var (output, error, result) = ExecuteProgram("decompile", badPath);
            var expectedErrorBadPath = Path.GetFullPath(badPath);
            var expectedErrorBadUri = new Uri(expectedErrorBadPath);

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.Should().BeEquivalentTo(
                    "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.",
                    "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.",
                    "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.",
                    $"{expectedErrorBadPath}: Decompilation failed with fatal error \"Failed to read {expectedErrorBadUri}\"");
                result.Should().Be(1);
            }
        }

        [DataRow("DoesNotExist.json")]
        [DataRow("WrongDir\\Fake.json")]
        [DataTestMethod]
        public void Decompilation_of_invalid_input_paths_to_stdout_should_produce_expected_errors(string badPath)
        {
            var (output, error, result) = ExecuteProgram("decompile", "--stdout", badPath);
            var expectedErrorBadPath = Path.GetFullPath(badPath);
            var expectedErrorBadUri = new Uri(expectedErrorBadPath);
            
            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.Should().BeEquivalentTo(
                    "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.",
                    "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.",
                    "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.",
                    $"{expectedErrorBadPath}: Decompilation failed with fatal error \"Failed to read {expectedErrorBadUri}\"");
                result.Should().Be(1);
            }
        }

        [TestMethod]
        public void LockedOutputFileShouldProduceExpectedError()
        {
            var inputFile = FileHelper.SaveResultFile(this.TestContext, "Empty.json", string.Empty);
            var outputFile = PathHelper.GetDefaultDecompileOutputPath(inputFile);

            // ReSharper disable once ConvertToUsingDeclaration
            using (new FileStream(outputFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                // keep the output stream open while we attempt to write to it
                // this should force an access denied error
                var (output, error, result) = ExecuteProgram("decompile", inputFile);

                output.Should().BeEmpty();
                string.Join(string.Empty, error).Should().Contain("Empty.json");
                result.Should().Be(1);
            }
        }
    }
}
