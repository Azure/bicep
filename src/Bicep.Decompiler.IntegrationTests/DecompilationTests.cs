// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.IO;
using System.Reflection;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.FileSystem;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Az;
using FluentAssertions.Execution;
using System.Text.RegularExpressions;
using Bicep.Decompiler.Exceptions;
using Bicep.Decompiler;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DecompilationTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        public class ExampleData
        {
            public ExampleData(string bicepStreamName, string jsonStreamName, string outputFolderName)
            {
                BicepStreamName = bicepStreamName;
                JsonStreamName = jsonStreamName;
                OutputFolderName = outputFolderName;
            }

            public string BicepStreamName { get; }

            public string JsonStreamName { get; }

            public string OutputFolderName { get; }

            public static string GetDisplayName(MethodInfo info, object[] data) => ((ExampleData)data[0]).JsonStreamName!;
        }

        private static IEnumerable<object[]> GetWorkingExampleData()
        {
            const string pathPrefix = "Working/";
            const string bicepExtension = ".bicep";

            // Only return files whose path segment length is 3 as entry files to avoid decompiling nested templates twice.
            var entryStreamNames = typeof(DecompilationTests).Assembly.GetManifestResourceNames()
                .Where(p => p.StartsWith(pathPrefix, StringComparison.Ordinal) && p.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Length == 3);

            foreach (var streamName in entryStreamNames)
            {
                var extension = Path.GetExtension(streamName);
                if (StringComparer.OrdinalIgnoreCase.Equals(extension, bicepExtension))
                {
                    continue;
                }

                var outputFolderName = streamName[pathPrefix.Length..^extension.Length].Replace('/', '_');
                var exampleData = new ExampleData(Path.ChangeExtension(streamName, bicepExtension), streamName, outputFolderName);

                yield return new object[] { exampleData };
            }
        }

        [TestMethod]
        public void ExampleData_should_return_a_number_of_records()
        {
            GetWorkingExampleData().Should().HaveCountGreaterOrEqualTo(10, "sanity check to ensure we're finding examples to test");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetWorkingExampleData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExampleData), DynamicDataDisplayName = nameof(ExampleData.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Decompiler_generates_expected_bicep_files_with_diagnostics(ExampleData example)
        {
            // save all the files in the containing directory to disk so that we can test module resolution
            var parentStream = Path.GetDirectoryName(example.BicepStreamName)!.Replace('\\', '/');
            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(DecompilationTests).Assembly, parentStream);
            var jsonFileName = Path.Combine(outputDirectory, Path.GetFileName(example.JsonStreamName));
            var typeProvider = AzResourceTypeProvider.CreateWithAzTypes();

            var (bicepUri, filesToSave) = TemplateDecompiler.DecompileFileWithModules(typeProvider, new FileResolver(), PathHelper.FilePathToFileUrl(jsonFileName));

            var syntaxTrees = filesToSave.Select(kvp => SyntaxTree.Create(kvp.Key, kvp.Value));
            var workspace = new Workspace();
            workspace.UpsertSyntaxTrees(syntaxTrees);

            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), workspace, bicepUri);
            var compilation = new Compilation(typeProvider, syntaxTreeGrouping);
            var diagnosticsBySyntaxTree = compilation.GetAllDiagnosticsBySyntaxTree();

            using (new AssertionScope())
            {
                foreach (var syntaxTree in syntaxTreeGrouping.SyntaxTrees)
                {
                    var exampleExists = File.Exists(syntaxTree.FileUri.LocalPath);
                    exampleExists.Should().BeTrue($"Generated example \"{syntaxTree.FileUri.LocalPath}\" should be checked in");

                    var diagnostics = diagnosticsBySyntaxTree[syntaxTree];
                    var bicepOutput = filesToSave[syntaxTree.FileUri];

                    var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(bicepOutput, "\n", diagnostics, diag => OutputHelper.GetDiagLoggingString(bicepOutput, outputDirectory, diag));
                    File.WriteAllText(syntaxTree.FileUri.LocalPath + ".actual", sourceTextWithDiags);

                    sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                        TestContext,
                        exampleExists ? File.ReadAllText(syntaxTree.FileUri.LocalPath) : "",
                        expectedLocation: Path.Combine("src", "Bicep.Decompiler.IntegrationTests", parentStream, Path.GetRelativePath(outputDirectory, syntaxTree.FileUri.LocalPath)),
                        actualLocation: syntaxTree.FileUri.LocalPath + ".actual");
                }
            }
        }

        private static IFileResolver ReadResourceFile(string resourcePath)
        {
            var manifestStream = typeof(DecompilationTests).Assembly.GetManifestResourceStream(resourcePath)!;
            var jsonContents = new StreamReader(manifestStream).ReadToEnd();

            var fileDict = new Dictionary<Uri, string>
            {
                [new Uri($"file:///{resourcePath}")] = jsonContents,
            };

            return new InMemoryFileResolver(fileDict);
        }

        [DataTestMethod]
        [DataRow("NonWorking/unknownprops.json", "[15:29]: Unrecognized top-level resource property 'madeUpProperty'")]
        [DataRow("NonWorking/invalid-schema.json", "[2:98]: $schema value \"https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#\" did not match any of the known ARM template deployment schemas.")]
        public void Decompiler_raises_errors_for_unsupported_features(string resourcePath, string expectedMessage)
        {
            Action onDecompile = () => {
                var fileResolver = ReadResourceFile(resourcePath);
                TemplateDecompiler.DecompileFileWithModules(TestTypeHelper.CreateEmptyProvider(),fileResolver, new Uri($"file:///{resourcePath}"));
            };

            onDecompile.Should().Throw<ConversionFailedException>().WithMessage(expectedMessage);
        }

        [DataTestMethod]
        [DataRow("\r\n", "\\r\\n")]
        [DataRow("\n", "\\n")]
        public void Decompiler_handles_strings_with_newlines(string newline, string escapedNewline)
        {
            var template = @"{
    ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
    ""contentVersion"": ""1.0.0.0"",
    ""parameters"": {},
    ""variables"": {
        ""multilineString"": ""multi
        line
        string""
    },
    ""resources"": [],
    ""outputs"": {}
}";

            // replace newlines with the style passed in
            template = string.Join(newline, Regex.Split(template, "\r?\n"));

            var fileUri = new Uri("file:///path/to/main.json");
            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [fileUri] = template,
            });;

            var (entryPointUri, filesToSave) = TemplateDecompiler.DecompileFileWithModules(TestTypeHelper.CreateEmptyProvider(), fileResolver, fileUri);

            // this behavior is actually controlled by newtonsoft's deserializer, but we should assert it anyway to avoid regressions.
            filesToSave[entryPointUri].Should().Contain($"var multilineString = 'multi{escapedNewline}        line{escapedNewline}        string'");
        }

        [DataTestMethod]
        [DataRow("and(variables('a'), variables('b'))", "boolean", "a && b")]
        [DataRow("and(variables('a'), variables('b'), variables('c'))", "boolean", "a && b && c")]
        [DataRow("or(variables('a'), variables('b'))", "boolean", "a || b")]
        [DataRow("or(variables('a'), variables('b'), variables('c'))", "boolean", "a || b || c")]
        [DataRow("add(variables('a'), variables('b'))", "int", "a + b")]
        [DataRow("sub(variables('a'), variables('b'))", "int", "a - b")]
        [DataRow("mul(variables('a'), variables('b'))", "int", "a * b")]
        [DataRow("div(variables('a'), variables('b'))", "int", "a / b")]
        [DataRow("mod(variables('a'), variables('b'))", "int", "a % b")]
        [DataRow("less(variables('a'), variables('b'))", "boolean", "a < b")]
        [DataRow("lessOrEquals(variables('a'), variables('b'))", "boolean", "a <= b")]
        [DataRow("greater(variables('a'), variables('b'))", "boolean", "a > b")]
        [DataRow("greaterOrEquals(variables('a'), variables('b'))", "boolean", "a >= b")]
        [DataRow("equals(variables('a'), variables('b'))", "boolean", "a == b")]
        public void Decompiler_handles_banned_function_replacement(string expression, string type, string expectedValue)
        {
            var template = @"{
    ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
    ""contentVersion"": ""1.0.0.0"",
    ""parameters"": {},
    ""variables"": {
        ""a"": true,
        ""b"": false,
        ""c"": true
    },
    ""resources"": [],
    ""outputs"": {
        ""calculated"": {
            ""type"": """ + type + @""",
            ""value"": ""[" + expression + @"]""
        }
    }
}";

            var fileUri = new Uri("file:///path/to/main.json");
            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [fileUri] = template,
            });

            var (entryPointUri, filesToSave) = TemplateDecompiler.DecompileFileWithModules(TestTypeHelper.CreateEmptyProvider(), fileResolver, fileUri);

            filesToSave[entryPointUri].Should().Contain($"output calculated {type} = ({expectedValue})");
        }

        [TestMethod]
        public void Decompiler_should_not_decompile_bicep_extension()
        {
            const string template = @"{
    ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
    ""contentVersion"": ""1.0.0.0"",
    ""parameters"": {},
    ""variables"": {},
    ""resources"": [],
    ""outputs"": {}
}";

            var fileUri = new Uri("file:///path/to/main.bicep");
            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [fileUri] = template,
            });

            Action sut = () => TemplateDecompiler.DecompileFileWithModules(TestTypeHelper.CreateEmptyProvider(), fileResolver, fileUri);

            sut.Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot decompile the file with .bicep extension: file:///path/to/main.bicep.");
        }

        [TestMethod]
        public void Decompiler_should_partially_handle_user_defined_functions_with_placeholders()
        {
            const string template = @"{
 ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
 ""contentVersion"": ""1.0.0.0"",
 ""parameters"": {
   ""storageNamePrefix"": {
     ""type"": ""string"",
     ""maxLength"": 11
   }
 },
 ""functions"": [
  {
    ""namespace"": ""contoso"",
    ""members"": {
      ""uniqueName"": {
        ""parameters"": [
          {
            ""name"": ""namePrefix"",
            ""type"": ""string""
          }
        ],
        ""output"": {
          ""type"": ""string"",
          ""value"": ""[concat(toLower(parameters('namePrefix')), uniqueString(resourceGroup().id))]""
        }
      }
    }
  }
],
 ""resources"": [
   {
     ""type"": ""Microsoft.Storage/storageAccounts"",
     ""apiVersion"": ""2019-04-01"",
     ""name"": ""[contoso.uniqueName(parameters('storageNamePrefix'))]"",
     ""location"": ""South Central US"",
     ""sku"": {
       ""name"": ""Standard_LRS""
     },
     ""kind"": ""StorageV2"",
     ""properties"": {
       ""supportsHttpsTrafficOnly"": true
     }
   }
 ]
}";

            var fileUri = new Uri("file:///path/to/main.json");
            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [fileUri] = template,
            });

            var (entryPointUri, filesToSave) = TemplateDecompiler.DecompileFileWithModules(TestTypeHelper.CreateEmptyProvider(), fileResolver, fileUri);

            filesToSave[entryPointUri].Should().Contain($"? /* TODO: User defined functions are not supported and have not been decompiled */");
        }
    }
}
