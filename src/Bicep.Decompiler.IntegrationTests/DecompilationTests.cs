// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.UnitTests.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.IO;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.FileSystem;
using FluentAssertions.Execution;
using System.Text.RegularExpressions;
using Bicep.Decompiler.Exceptions;
using Bicep.Decompiler;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Baselines;
using System.Threading;
using System.Globalization;
using Bicep.Core.UnitTests.FileSystem;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DecompilationTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void ExampleData_should_return_a_number_of_records()
        {
            GetWorkingExampleData().Should().HaveCountGreaterOrEqualTo(10, "sanity check to ensure we're finding examples to test");
        }

        private static IEnumerable<object[]> GetWorkingExampleData()
            => EmbeddedFile.LoadAll(
                typeof(DecompilationTests).Assembly,
                "Working",
                streamName => Path.GetExtension(streamName) == ".json")
            .Select(x => new object[] { x });

        [DataTestMethod]
        [DynamicData(nameof(GetWorkingExampleData), DynamicDataSourceType.Method)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Decompiler_generates_expected_bicep_files_with_diagnostics(EmbeddedFile embeddedJson)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, embeddedJson);
            var jsonFile = baselineFolder.EntryFile;

            var jsonUri = PathHelper.FilePathToFileUrl(jsonFile.OutputFilePath);
            var decompiler = new TemplateDecompiler(BicepTestConstants.FeatureProviderFactory, BicepTestConstants.NamespaceProvider, BicepTestConstants.FileResolver, BicepTestConstants.RegistryProvider, BicepTestConstants.ApiVersionProviderFactory, BicepTestConstants.LinterAnalyzer);
            var (bicepUri, filesToSave) = decompiler.DecompileFileWithModules(jsonUri, PathHelper.ChangeToBicepExtension(jsonUri));

            var result = CompilationHelper.Compile(new(), new InMemoryFileResolver(filesToSave), filesToSave.Keys, bicepUri);
            var diagnosticsByBicepFile = result.Compilation.GetAllDiagnosticsByBicepFile();

            using (new AssertionScope())
            {
                foreach (var (bicepFile, diagnostics) in diagnosticsByBicepFile)
                {
                    var baselineFile = baselineFolder.GetFileOrEnsureCheckedIn(bicepFile.FileUri);
                    var bicepOutput = filesToSave[bicepFile.FileUri];

                    var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(bicepOutput, "\n", diagnostics, diag => OutputHelper.GetDiagLoggingString(bicepOutput, baselineFolder.OutputFolderPath, diag));
                    File.WriteAllText(baselineFile.OutputFilePath, sourceTextWithDiags);

                    baselineFile.ShouldHaveExpectedValue();
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
        [DataRow("Files/NonWorking/unknownprops.json", "[15:29]: Unrecognized top-level resource property 'madeUpProperty'")]
        [DataRow("Files/NonWorking/invalid-schema.json", "[2:98]: $schema value \"https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#\" did not match any of the known ARM template deployment schemas.")]
        [DataRow("Files/NonWorking/keyvault-secret-reference.json", "[25:38]: Failed to convert parameter \"mySecret\": KeyVault secret references are not currently supported by the decompiler.")]
        [DataRow("Files/NonWorking/symbolic-names.json", "[27:16]: Decompilation of symbolic name templates is not currently supported")]
        public void Decompiler_raises_errors_for_unsupported_features(string resourcePath, string expectedMessage)
        {
            Action onDecompile = () =>
            {
                var fileResolver = ReadResourceFile(resourcePath);
                var decompiler = new TemplateDecompiler(BicepTestConstants.FeatureProviderFactory, TestTypeHelper.CreateEmptyProvider(), fileResolver, new DefaultModuleRegistryProvider(fileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.FeatureProviderFactory, BicepTestConstants.ConfigurationManager), BicepTestConstants.ApiVersionProviderFactory, BicepTestConstants.LinterAnalyzer);
                decompiler.DecompileFileWithModules(new Uri($"file:///{resourcePath}"), new Uri("file:///unused.bicep"));
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
            }); ;

            var decompiler = new TemplateDecompiler(BicepTestConstants.FeatureProviderFactory, TestTypeHelper.CreateEmptyProvider(), fileResolver, new DefaultModuleRegistryProvider(fileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.FeatureProviderFactory, BicepTestConstants.ConfigurationManager), BicepTestConstants.ApiVersionProviderFactory, BicepTestConstants.LinterAnalyzer);
            var (entryPointUri, filesToSave) = decompiler.DecompileFileWithModules(fileUri, PathHelper.ChangeToBicepExtension(fileUri));

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
        [DataRow("equals(toLower(variables('a')),toLower(variables('b')))", "boolean", "a =~ b")]
        [DataRow("not(equals(variables('a'),variables('b')))", "boolean", "a != b")]
        [DataRow("not(equals(toLower(variables('a')),toLower(variables('b'))))", "boolean", "a !~ b")]
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

            var decompiler = new TemplateDecompiler(BicepTestConstants.FeatureProviderFactory, TestTypeHelper.CreateEmptyProvider(), fileResolver, new DefaultModuleRegistryProvider(fileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.FeatureProviderFactory, BicepTestConstants.ConfigurationManager), BicepTestConstants.ApiVersionProviderFactory, BicepTestConstants.LinterAnalyzer);
            var (entryPointUri, filesToSave) = decompiler.DecompileFileWithModules(fileUri, PathHelper.ChangeToBicepExtension(fileUri));

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

            Action sut = () =>
            {
                var decompiler = new TemplateDecompiler(BicepTestConstants.FeatureProviderFactory, TestTypeHelper.CreateEmptyProvider(), fileResolver, new DefaultModuleRegistryProvider(fileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.FeatureProviderFactory, BicepTestConstants.ConfigurationManager), BicepTestConstants.ApiVersionProviderFactory, BicepTestConstants.LinterAnalyzer);
                decompiler.DecompileFileWithModules(fileUri, PathHelper.ChangeToBicepExtension(fileUri));
            };

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

            var decompiler = new TemplateDecompiler(BicepTestConstants.FeatureProviderFactory, TestTypeHelper.CreateEmptyProvider(), fileResolver, new DefaultModuleRegistryProvider(fileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.FeatureProviderFactory, BicepTestConstants.ConfigurationManager), BicepTestConstants.ApiVersionProviderFactory, BicepTestConstants.LinterAnalyzer);
            var (entryPointUri, filesToSave) = decompiler.DecompileFileWithModules(fileUri, PathHelper.ChangeToBicepExtension(fileUri));

            filesToSave[entryPointUri].Should().Contain($"? /* TODO: User defined functions are not supported and have not been decompiled */");
        }

        [TestMethod]
        public void Decompiler_should_not_interpret_numbers_with_locale_settings()
        {
            // https://github.com/Azure/bicep/issues/7615
            const string template = @"{
    ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
    ""contentVersion"": ""1.0.0.0"",
    ""parameters"": {},
    ""variables"": {
        ""cpu"": 0.25
    },
    ""resources"": [],
    ""outputs"": {}
}";

            var fileUri = new Uri("file:///path/to/main.json");
            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [fileUri] = template,
            });

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fi-FI");

                var decompiler = new TemplateDecompiler(BicepTestConstants.FeatureProviderFactory, TestTypeHelper.CreateEmptyProvider(), fileResolver, new DefaultModuleRegistryProvider(fileResolver, BicepTestConstants.ClientFactory, BicepTestConstants.TemplateSpecRepositoryFactory, BicepTestConstants.FeatureProviderFactory, BicepTestConstants.ConfigurationManager), BicepTestConstants.ApiVersionProviderFactory, BicepTestConstants.LinterAnalyzer);
                var (entryPointUri, filesToSave) = decompiler.DecompileFileWithModules(fileUri, PathHelper.ChangeToBicepExtension(fileUri));

                filesToSave[entryPointUri].Should().Contain($"var cpu = '0.25'");
            }
            finally {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }
    }
}
