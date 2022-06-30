// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ExamplesTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetExampleData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExampleData), DynamicDataDisplayName = nameof(ExampleData.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void ExampleIsValid(ExampleData example)
        {
            // save all the files in the containing directory to disk so that we can test module resolution
            var parentStream = GetParentStreamName(example.BicepStreamName);
            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(ExampleData).Assembly, parentStream);
            var bicepFileName = Path.Combine(outputDirectory, Path.GetFileName(example.BicepStreamName));
            var jsonFileName = Path.Combine(outputDirectory, Path.GetFileName(example.JsonStreamName));

            var dispatcher = new ModuleDispatcher(BicepTestConstants.RegistryProvider);
            var configuration = BicepTestConstants.BuiltInConfigurationWithAnalyzersDisabled;
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, new Workspace(), PathHelper.FilePathToFileUrl(bicepFileName), configuration);
            var compilation = new Compilation(BicepTestConstants.Features, BicepTestConstants.NamespaceProvider, sourceFileGrouping, configuration, new LinterAnalyzer(configuration));
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), BicepTestConstants.EmitterSettings);

            foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
            {
                DiagnosticAssertions.DoWithDiagnosticAnnotations(
                    bicepFile,
                    diagnostics.Where(x => !IsPermittedMissingTypeDiagnostic(x)),
                    diagnostics =>
                    {
                        diagnostics.Should().BeEmpty("{0} should not have warnings or errors", bicepFile.FileUri.LocalPath);
                    });
            }

            // group assertion failures using AssertionScope, rather than reporting the first failure
            using (new AssertionScope())
            {
                var exampleExists = File.Exists(jsonFileName);
                exampleExists.Should().BeTrue($"Generated example \"{jsonFileName}\" should be checked in");

                using var stream = new MemoryStream();
                var result = emitter.Emit(stream);

                result.Status.Should().Be(EmitStatus.Succeeded);

                if (result.Status == EmitStatus.Succeeded)
                {
                    stream.Position = 0;
                    var generated = new StreamReader(stream).ReadToEnd();

                    var actual = JToken.Parse(generated);
                    File.WriteAllText(jsonFileName + ".actual", generated);

                    actual.Should().EqualWithJsonDiffOutput(
                        TestContext,
                        exampleExists ? JToken.Parse(File.ReadAllText(jsonFileName)) : new JObject(),
                        ExampleData.GetBaselineUpdatePath(example.JsonStreamName),
                        jsonFileName + ".actual");

                    // validate that the template is parseable by the deployment engine
                    TemplateHelper.TemplateShouldBeValid(generated);
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExampleData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExampleData), DynamicDataDisplayName = nameof(ExampleData.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void ExampleIsValid_using_experimental_symbolic_names(ExampleData example)
        {
            var parentStream = GetParentStreamName(example.BicepStreamName);
            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(ExampleData).Assembly, parentStream);
            var bicepFileName = Path.Combine(outputDirectory, Path.GetFileName(example.BicepStreamName));
            var jsonFileName = Path.Combine(outputDirectory, Path.GetFileName(example.SymbolicNamesJsonStreamName));

            var dispatcher = new ModuleDispatcher(BicepTestConstants.RegistryProvider);
            var configuration = BicepTestConstants.BuiltInConfigurationWithAnalyzersDisabled;
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, new Workspace(), PathHelper.FilePathToFileUrl(bicepFileName), configuration);
            var compilation = new Compilation(BicepTestConstants.Features, BicepTestConstants.NamespaceProvider, sourceFileGrouping, configuration, new LinterAnalyzer(configuration));
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), BicepTestConstants.EmitterSettingsWithSymbolicNames);

            foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
            {
                DiagnosticAssertions.DoWithDiagnosticAnnotations(
                    bicepFile,
                    diagnostics.Where(x => !IsPermittedMissingTypeDiagnostic(x)),
                    diagnostics =>
                    {
                        diagnostics.Should().BeEmpty("{0} should not have warnings or errors", bicepFile.FileUri.LocalPath);
                    });
            }

            // group assertion failures using AssertionScope, rather than reporting the first failure
            using (new AssertionScope())
            {
                var exampleExists = File.Exists(jsonFileName);
                exampleExists.Should().BeTrue($"Generated example \"{jsonFileName}\" should be checked in");

                using var stream = new MemoryStream();
                var result = emitter.Emit(stream);

                result.Status.Should().Be(EmitStatus.Succeeded);

                if (result.Status == EmitStatus.Succeeded)
                {
                    stream.Position = 0;
                    var generated = new StreamReader(stream).ReadToEnd();

                    var actual = JToken.Parse(generated);
                    File.WriteAllText(jsonFileName + ".actual", generated);

                    actual.Should().EqualWithJsonDiffOutput(
                        TestContext,
                        exampleExists ? JToken.Parse(File.ReadAllText(jsonFileName)) : new JObject(),
                        ExampleData.GetBaselineUpdatePath(example.SymbolicNamesJsonStreamName),
                        jsonFileName + ".actual");

                    // validate that the template is parseable by the deployment engine
                    TemplateHelper.TemplateShouldBeValid(generated);
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExtensibilityExampleData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExampleData), DynamicDataDisplayName = nameof(ExampleData.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void ExampleIsValid_extensibility(ExampleData example)
        {
            // save all the files in the containing directory to disk so that we can test module resolution
            var parentStream = GetParentStreamName(example.BicepStreamName);
            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(ExampleData).Assembly, parentStream);
            var bicepFileName = Path.Combine(outputDirectory, Path.GetFileName(example.BicepStreamName));
            var jsonFileName = Path.Combine(outputDirectory, Path.GetFileName(example.JsonStreamName));

            var features = BicepTestConstants.CreateFeaturesProvider(TestContext, importsEnabled: true);

            var dispatcher = new ModuleDispatcher(BicepTestConstants.RegistryProvider);
            var configuration = BicepTestConstants.BuiltInConfigurationWithAnalyzersDisabled;
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, new Workspace(), PathHelper.FilePathToFileUrl(bicepFileName), configuration);
            var compilation = new Compilation(BicepTestConstants.Features, new DefaultNamespaceProvider(BicepTestConstants.AzResourceTypeLoader, features), sourceFileGrouping, configuration, new LinterAnalyzer(configuration));
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), new EmitterSettings(features));

            foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
            {
                DiagnosticAssertions.DoWithDiagnosticAnnotations(
                    bicepFile,
                    diagnostics.Where(x => !IsPermittedMissingTypeDiagnostic(x)),
                    diagnostics =>
                    {
                        diagnostics.Should().BeEmpty("{0} should not have warnings or errors", bicepFile.FileUri.LocalPath);
                    });
            }

            // group assertion failures using AssertionScope, rather than reporting the first failure
            using (new AssertionScope())
            {
                var exampleExists = File.Exists(jsonFileName);
                exampleExists.Should().BeTrue($"Generated example \"{jsonFileName}\" should be checked in");

                using var stream = new MemoryStream();
                var result = emitter.Emit(stream);

                result.Status.Should().Be(EmitStatus.Succeeded);

                if (result.Status == EmitStatus.Succeeded)
                {
                    stream.Position = 0;
                    var generated = new StreamReader(stream).ReadToEnd();

                    var actual = JToken.Parse(generated);
                    File.WriteAllText(jsonFileName + ".actual", generated);

                    actual.Should().EqualWithJsonDiffOutput(
                        TestContext,
                        exampleExists ? JToken.Parse(File.ReadAllText(jsonFileName)) : new JObject(),
                        ExampleData.GetBaselineUpdatePath(example.JsonStreamName),
                        jsonFileName + ".actual");

                    // validate that the template is parseable by the deployment engine
                    TemplateHelper.TemplateShouldBeValid(generated);
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllExampleData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExampleData), DynamicDataDisplayName = nameof(ExampleData.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Example_uses_consistent_formatting(ExampleData example)
        {
            // save all the files in the containing directory to disk so that we can test module resolution
            var parentStream = GetParentStreamName(example.BicepStreamName);
            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(ExampleData).Assembly, parentStream);

            var bicepFileName = Path.Combine(outputDirectory, Path.GetFileName(example.BicepStreamName));
            var originalContents = File.ReadAllText(bicepFileName);
            var program = ParserHelper.Parse(originalContents);

            var printOptions = new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, true);

            var formattedContents = PrettyPrinter.PrintProgram(program, printOptions);
            formattedContents.Should().NotBeNull();

            File.WriteAllText(bicepFileName + ".formatted", formattedContents);

            originalContents.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                formattedContents!,
                ExampleData.GetBaselineUpdatePath(example.BicepStreamName),
                actualLocation: bicepFileName + ".formatted");
        }

        [TestMethod]
        public void ExampleData_should_return_a_number_of_records()
        {
            GetAllExampleData().Should().HaveCountGreaterOrEqualTo(30, "sanity check to ensure we're finding examples to test");
        }

        [TestMethod]
        public void Example_folders_must_all_have_a_main_file()
        {
            var exampleData = GetAllExampleData().Select(x => (ExampleData)x[0]).ToArray();
            var parentDirs = exampleData.Select(x => GetParentStreamName(x.BicepStreamName));

            var mainFiles = exampleData.Select(x => x.BicepStreamName).Where(x => Path.GetFileName(x) == "main.bicep");
            var otherFiles = exampleData.Select(x => x.BicepStreamName).Where(x => Path.GetFileName(x) != "main.bicep");

            var mainFileDirs = new HashSet<string>(mainFiles.Select(x => GetParentStreamName(x)));

            using (new AssertionScope())
            {
                foreach (var bicepFile in otherFiles)
                {
                    var hasMainFileParent = mainFileDirs.Any(x => bicepFile.StartsWith(x, StringComparison.OrdinalIgnoreCase));
                    hasMainFileParent.Should().BeTrue($"expected \"{bicepFile}\" or one of its parent directories to contain a \"main.bicep\" file");
                }
            }
        }

        private static string GetParentStreamName(string streamName)
            => Path.GetDirectoryName(streamName)!.Replace('\\', '/');

        private static IEnumerable<object[]> GetAllExampleData()
            => ExampleData.GetAllExampleData().Select(x => new object[] { x });

        private static IEnumerable<object[]> GetExampleData()
            => ExampleData.GetAllExampleData().Where(x => !x.IsExtensibilitySample).Select(x => new object[] { x });

        private static IEnumerable<object[]> GetExtensibilityExampleData()
            => ExampleData.GetAllExampleData().Where(x => x.IsExtensibilitySample).Select(x => new object[] { x });

        private static bool IsPermittedMissingTypeDiagnostic(IDiagnostic diagnostic)
        {
            if (diagnostic.Code != "BCP081")
            {
                return false;
            }

            var permittedMissingTypeDiagnostics = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // To exclude a particular type for BCP081 (if there are missing types), add an entry of format:
                // "Resource type \"<type>\" does not have types available.",
            };

            return permittedMissingTypeDiagnostics.Contains(diagnostic.Message);
        }
    }
}
