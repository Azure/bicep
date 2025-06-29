// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Bicep.Core;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Utils;
using Bicep.Decompiler;
using Bicep.Decompiler.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Decompiler.IntegrationTests
{
    [TestClass]
    public class DecompilationTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static BicepDecompiler CreateDecompiler()
            => ServiceBuilder.Create(s => s.WithEmptyAzResources()).GetDecompiler();

        [DataTestMethod]
        [EmbeddedFilesTestData(@"Files/Working/.*\.json")]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Decompiler_generates_expected_bicep_files_with_diagnostics(EmbeddedFile embeddedJson)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, embeddedJson);
            var jsonFile = baselineFolder.EntryFile;

            var jsonUri = PathHelper.FilePathToFileUrl(jsonFile.OutputFilePath);
            var decompiler = ServiceBuilder.Create().GetDecompiler();
            var (bicepUri, filesToSave) = await decompiler.Decompile(PathHelper.ChangeToBicepExtension(jsonUri), jsonFile.EmbeddedFile.Contents);

            var compilation = new ServiceBuilder().BuildCompilation(filesToSave, PathHelper.ChangeToBicepExtension(jsonUri));
            var diagnosticsByBicepFile = compilation.GetAllDiagnosticsByBicepFile();

            using (new AssertionScope())
            {
                foreach (var (bicepFile, diagnostics) in diagnosticsByBicepFile)
                {
                    var baselineFile = baselineFolder.GetFileOrEnsureCheckedIn(bicepFile.Uri);
                    var bicepOutput = filesToSave[bicepFile.Uri];

                    var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(bicepOutput, "\n", diagnostics, diag => OutputHelper.GetDiagLoggingString(bicepOutput, baselineFolder.OutputFolderPath, diag));

                    baselineFile.WriteToOutputFolder(sourceTextWithDiags);
                    baselineFile.ShouldHaveExpectedValue();
                }
            }
        }

        [DataTestMethod]
        [EmbeddedFilesTestData(@"Files/Parameters/.*\.json")]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Decompiler_generates_expected_bicepparam_files_with_diagnostics(EmbeddedFile embeddedJson)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, embeddedJson);
            var jsonFile = baselineFolder.EntryFile;

            var jsonUri = PathHelper.FilePathToFileUrl(jsonFile.OutputFilePath);
            var decompiler = ServiceBuilder.Create().GetDecompiler();

            var (entryPointUri, filesToSave) = decompiler.DecompileParameters(jsonFile.EmbeddedFile.Contents, PathHelper.ChangeExtension(jsonUri, LanguageConstants.ParamsFileExtension), null);

            var baselineFile = baselineFolder.GetFileOrEnsureCheckedIn(entryPointUri);
            baselineFile.WriteToOutputFolder(filesToSave[entryPointUri]);
            baselineFile.ShouldHaveExpectedValue();
        }

        private static string ReadResourceFile(string resourcePath)
        {
            var manifestStream = typeof(DecompilationTests).Assembly.GetManifestResourceStream(resourcePath)!;
            return new StreamReader(manifestStream).ReadToEnd();
        }

        [DataTestMethod]
        [DataRow("Files/NonWorking/unknownprops.json", "[15:29]: Unrecognized top-level resource property 'madeUpProperty'")]
        [DataRow("Files/NonWorking/invalid-schema.json", "[2:98]: $schema value \"https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#\" did not match any of the known ARM template deployment schemas.")]
        [DataRow("Files/NonWorking/keyvault-secret-reference.json", "[25:38]: Failed to convert parameter \"mySecret\": KeyVault secret references are not currently supported by the decompiler.")]
        [DataRow("Files/NonWorking/symbolic-names.json", "[19:16]: Decompilation of symbolic name templates is not currently supported")]
        [DataRow("Files/NonWorking/parameter-value-as-expression.json", "[39:218]: Expected to find an object, but found an expression. This is not currently supported by the decompiler.")]
        [DataRow("Files/NonWorking/parameter-value-as-string.json", "[14:43]: Expected to find an object, but found: This is incorrect")]
        public async Task Decompiler_raises_errors_for_unsupported_features(string resourcePath, string expectedMessage)
        {
            Func<Task> onDecompile = async () =>
            {
                var jsonContent = ReadResourceFile(resourcePath);
                var decompiler = CreateDecompiler();
                await decompiler.Decompile(new Uri("file:///unused.bicep"), jsonContent);
            };

            await onDecompile.Should().ThrowAsync<ConversionFailedException>().WithMessage(expectedMessage);
        }

        [DataTestMethod]
        [DataRow("\r\n", "\\r\\n")]
        [DataRow("\n", "\\n")]
        public async Task Decompiler_handles_strings_with_newlines(string newline, string escapedNewline)
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

            var decompiler = CreateDecompiler();
            var (entryPointUri, filesToSave) = await decompiler.Decompile(PathHelper.ChangeToBicepExtension(fileUri), template);

            // this behavior is actually controlled by newtonsoft's deserializer, but we should assert it anyway to avoid regressions.
            filesToSave[entryPointUri].Should().Contain($"var multilineString = 'multi{escapedNewline}        line{escapedNewline}        string'");
        }

        [DataTestMethod]
        [DataRow("and(variables('a'), variables('b'))", "boolean", "(a && b)")]
        [DataRow("and(variables('a'), variables('b'), variables('c'))", "boolean", "(a && b && c)")]
        [DataRow("or(variables('a'), variables('b'))", "boolean", "(a || b)")]
        [DataRow("or(variables('a'), variables('b'), variables('c'))", "boolean", "(a || b || c)")]
        [DataRow("add(variables('a'), variables('b'))", "int", "(a + b)")]
        [DataRow("sub(variables('a'), variables('b'))", "int", "(a - b)")]
        [DataRow("mul(variables('a'), variables('b'))", "int", "(a * b)")]
        [DataRow("div(variables('a'), variables('b'))", "int", "(a / b)")]
        [DataRow("mod(variables('a'), variables('b'))", "int", "(a % b)")]
        [DataRow("less(variables('a'), variables('b'))", "boolean", "(a < b)")]
        [DataRow("lessOrEquals(variables('a'), variables('b'))", "boolean", "(a <= b)")]
        [DataRow("greater(variables('a'), variables('b'))", "boolean", "(a > b)")]
        [DataRow("greaterOrEquals(variables('a'), variables('b'))", "boolean", "(a >= b)")]
        [DataRow("equals(variables('a'), variables('b'))", "boolean", "(a == b)")]
        [DataRow("equals(toLower(variables('a')),toLower(variables('b')))", "boolean", "(a =~ b)")]
        [DataRow("not(equals(variables('a'),variables('b')))", "boolean", "(a != b)")]
        [DataRow("not(equals(toLower(variables('a')),toLower(variables('b'))))", "boolean", "(a !~ b)")]
        [DataRow("createArray(1, 2, 3)", "array", "[\n  1\n  2\n  3\n]")]
        [DataRow("createObject('key', 'value')", "object", "{\n  key: 'value'\n}")]
        [DataRow("tryGet(parameters('z'), 'y')", "int", "z.?y")]
        [DataRow("tryGet(parameters('z'), 'y', 'x', 'w')", "int", "z.?y.x.w")]
        [DataRow("tryGet(tryGet(parameters('z'), 'y', 'x'), 'w', 'v')", "int", "z.?y.x.?w.v")]
        [DataRow("tryGet(parameters('z'), 'y', 'x', 'w').v", "int", "(z.?y.x.w).v")]
        [DataRow("tryIndexFromEnd(parameters('z').array, 3, 'foo')", "int", "z.array[?^3].foo")]
        [DataRow("tryIndexFromEnd(parameters('z').array, 3, createObject('value', 2, 'fromEnd', true()))", "int", "z.array[?^3][^2]")]
        [DataRow("tryIndexFromEnd(parameters('z').array, 3, createObject('value', 2))", "int", "z.array[?^3][2]")]
        [DataRow("tryIndexFromEnd(parameters('z').array, 3, createObject('value', 2)).foo", "int", "(z.array[?^3][2]).foo")]
        [DataRow("indexFromEnd(parameters('z').array, 3)", "int", "z.array[^3]")]
        public async Task Decompiler_handles_banned_function_replacement(string expression, string type, string expectedValue)
        {
            var template = @"{
    ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
    ""contentVersion"": ""1.0.0.0"",
    ""parameters"": {
        ""z"": {""type"":""object""}
    },
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

            var decompiler = CreateDecompiler();
            var (entryPointUri, filesToSave) = await decompiler.Decompile(PathHelper.ChangeToBicepExtension(fileUri), template);

            filesToSave[entryPointUri].Should().Contain($"output calculated {type} = {expectedValue}");
        }

        [TestMethod]
        public async Task Decompiler_should_not_interpret_numbers_with_locale_settings()
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

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fi-FI");

                var decompiler = CreateDecompiler();
                var (entryPointUri, filesToSave) = await decompiler.Decompile(PathHelper.ChangeToBicepExtension(fileUri), template);

                filesToSave[entryPointUri].Should().Contain($"var cpu = json('0.25')");
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }
    }
}
