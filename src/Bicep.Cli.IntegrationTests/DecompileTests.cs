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

namespace Bicep.Cli.IntegrationTests
{
    [TestClass]
    public class DecompileTests
    { 
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static (IEnumerable<string> outputLines, IEnumerable<string> errorLines, int result) ExecuteProgram(params string[] args)
        {
            var (output, error, result) = TextWriterHelper.InvokeWriterAction((outputWriter, errorWriter) =>
            {
                var program = new Program(TestResourceTypeProvider.Create(), outputWriter, errorWriter, BicepTestConstants.DevAssemblyFileVersion);

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

            var directoryName = Path.Combine(TestContext.TestRunResultsDirectory, TestContext.TestName);
            Directory.CreateDirectory(directoryName);

            var fileName = Path.Combine(directoryName, "main.json");
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
            var template = @"{
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

            var directoryName = Path.Combine(TestContext.TestRunResultsDirectory, TestContext.TestName);
            Directory.CreateDirectory(directoryName);

            var fileName = Path.Combine(directoryName, "main.json");
            File.WriteAllText(fileName, template);

            var (output, error, result) = ExecuteProgram("decompile", fileName);
            var bicepFileName = Path.ChangeExtension(fileName, "bicep");

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.Should().BeEquivalentTo(
                    "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.",
                    "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.",
                    "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.",
                    $"{bicepFileName}(4,23) : Error BCP079: This expression is referencing its own declaration, which is not allowed."
                );
                result.Should().Be(1);
            }

            var bicepFile = File.ReadAllText(bicepFileName);
            bicepFile.Should().BeEquivalentToIgnoringNewlines(@"resource resName 'My.Rp/testType@2020-01-01' = {
  name: 'resName'
  properties: {
    cyclicDependency: resName.properties
  }
}");
        }

        [TestMethod]
        public void Decompilation_of_file_with_no_errors()
        {
            var template = @"{
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

            var directoryName = Path.Combine(TestContext.TestRunResultsDirectory, TestContext.TestName);
            Directory.CreateDirectory(directoryName);

            var fileName = Path.Combine(directoryName, "main.json");
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
            bicepFile.Should().BeEquivalentToIgnoringNewlines(@"resource resName 'My.Rp/testType@2020-01-01' = {
  name: 'resName'
  location: resourceGroup().location
  properties: {
    prop1: 'val1'
  }
}");
        }
    }
}