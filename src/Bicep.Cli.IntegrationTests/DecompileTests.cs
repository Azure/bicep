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
            bicepFile.Should().BeEquivalentToIgnoringNewlines(@"resource resName 'My.Rp/testType@2020-01-01' = {
  name: 'resName'
  location: resourceGroup().location
  properties: {
    prop1: 'val1'
  }
}");
        }

         [TestMethod]
        public void Decompilation_of_multiple_files_with_no_errors()
        {
            var template1 = @"{
    ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
    ""contentVersion"": ""1.0.0.0"",
    ""parameters"": {},
    ""variables"": {},
    ""resources"": [
        {
            ""type"": ""My.Rp/testType1"",
            ""apiVersion"": ""2020-01-01"",
            ""name"": ""resName1"",
            ""location"": ""[resourceGroup().location]"",
            ""properties"": {
                ""prop1"": ""val1""
            }
        }
    ],
    ""outputs"": {}
}";

            var template2 = @"{
    ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
    ""contentVersion"": ""1.0.0.0"",
    ""parameters"": {},
    ""variables"": {},
    ""resources"": [
        {
            ""type"": ""My.Rp/testType2"",
            ""apiVersion"": ""2021-01-01"",
            ""name"": ""resName2"",
            ""location"": ""[resourceGroup().location]"",
            ""properties"": {
                ""prop2"": ""val2""
            }
        }
    ],
    ""outputs"": {}
}";

            var fileName1 = FileHelper.GetResultFilePath(TestContext, "template1.json");
            var fileName2 = FileHelper.GetResultFilePath(TestContext, "template2.json");
            File.WriteAllText(fileName1, template1);
            File.WriteAllText(fileName2, template2);

            var (output, error, result) = ExecuteProgram("decompile", fileName1, fileName2);
            var bicepFileName1 = Path.ChangeExtension(fileName1, "bicep");
            var bicepFileName2 = Path.ChangeExtension(fileName2, "bicep");

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.Should().BeEquivalentTo(
                    "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.",
                    "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.",
                    "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.");
                result.Should().Be(0);
            }

            var bicepFile1 = File.ReadAllText(bicepFileName1);
            bicepFile1.Should().BeEquivalentToIgnoringNewlines(@"resource resName1 'My.Rp/testType1@2020-01-01' = {
  name: 'resName1'
  location: resourceGroup().location
  properties: {
    prop1: 'val1'
  }
}");
            var bicepFile2 = File.ReadAllText(bicepFileName2);
            bicepFile2.Should().BeEquivalentToIgnoringNewlines(@"resource resName2 'My.Rp/testType2@2021-01-01' = {
  name: 'resName2'
  location: resourceGroup().location
  properties: {
    prop2: 'val2'
  }
}");
        }

    [TestMethod]
        public void Decompilation_of_multiple_files_with_error()
        {
            var template1 = @"{
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
                ""prop"": ""val""
            }
        }
    ],
    ""outputs"": {}
}";

            var template2 = @"{
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

            var fileName1 = FileHelper.GetResultFilePath(TestContext, "template1.json");
            var fileName2 = FileHelper.GetResultFilePath(TestContext, "template2.json");
            File.WriteAllText(fileName1, template1);
            File.WriteAllText(fileName2, template2);

            var (output, error, result) = ExecuteProgram("decompile", fileName1, fileName2);
            var bicepFileName1 = Path.ChangeExtension(fileName1, "bicep");
            var bicepFileName2 = Path.ChangeExtension(fileName2, "bicep");

            using (new AssertionScope())
            {
                output.Should().BeEmpty();
                error.Should().BeEquivalentTo(
                    "WARNING: Decompilation is a best-effort process, as there is no guaranteed mapping from ARM JSON to Bicep.",
                    "You may need to fix warnings and errors in the generated bicep file(s), or decompilation may fail entirely if an accurate conversion is not possible.",
                    "If you would like to report any issues or inaccurate conversions, please see https://github.com/Azure/bicep/issues.",
                    $"{bicepFileName2}(4,23) : Error BCP079: This expression is referencing its own declaration, which is not allowed."
                );
                result.Should().Be(1);
            }

            var bicepFile1 = File.ReadAllText(bicepFileName1);
            bicepFile1.Should().BeEquivalentToIgnoringNewlines(@"resource resName 'My.Rp/testType@2020-01-01' = {
  name: 'resName'
  location: resourceGroup().location
  properties: {
    prop: 'val'
  }
}");
            var bicepFile2 = File.ReadAllText(bicepFileName2);
            bicepFile2.Should().BeEquivalentToIgnoringNewlines(@"resource resName 'My.Rp/testType@2020-01-01' = {
  name: 'resName'
  properties: {
    cyclicDependency: resName.properties
  }
}");
        }
    }
}