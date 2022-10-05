// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ExamplesTests
    {
        private static ServiceBuilder Services => new ServiceBuilder().WithDisabledAnalyzersConfiguration();

        [NotNull]
        public TestContext? TestContext { get; set; }

        private void RunExampleTest(EmbeddedFile embeddedBicep, IFeatureProvider features, string jsonFileExtension)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, embeddedBicep);
            var bicepFile = baselineFolder.EntryFile;
            var jsonFile = baselineFolder.GetFileOrEnsureCheckedIn(Path.ChangeExtension(embeddedBicep.FileName, jsonFileExtension));

            var configManager = IConfigurationManager.WithStaticConfiguration(BicepTestConstants.BuiltInConfigurationWithAllAnalyzersDisabled);
            var dispatcher = new ModuleDispatcher(BicepTestConstants.RegistryProvider, configManager);
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, new Workspace(), PathHelper.FilePathToFileUrl(bicepFile.OutputFilePath));
            var compilation = Services.WithFeatureProviderFactory(IFeatureProviderFactory.WithStaticFeatureProvider(features)).Build().BuildCompilation(sourceFileGrouping);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

            foreach (var (file, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
            {
                DiagnosticAssertions.DoWithDiagnosticAnnotations(
                    file,
                    diagnostics.Where(x => !IsPermittedMissingTypeDiagnostic(x)),
                    diagnostics =>
                    {
                        diagnostics.Should().BeEmpty("{0} should not have warnings or errors", file.FileUri.LocalPath);
                    });
            }

            // group assertion failures using AssertionScope, rather than reporting the first failure
            using (new AssertionScope())
            {
                var stringWriter = new StringWriter();
                var result = emitter.Emit(stringWriter);

                result.Status.Should().Be(EmitStatus.Succeeded);

                if (result.Status == EmitStatus.Succeeded)
                {
                    File.WriteAllText(jsonFile.OutputFilePath, stringWriter.ToString());
                    jsonFile.ShouldHaveExpectedJsonValue();

                    // validate that the template is parseable by the deployment engine
                    TemplateHelper.TemplateShouldBeValid(stringWriter.ToString());
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExampleData), DynamicDataSourceType.Method)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void ExampleIsValid(EmbeddedFile embeddedBicep)
            => RunExampleTest(embeddedBicep, BicepTestConstants.Features, ".json");

        [DataTestMethod]
        [DynamicData(nameof(GetExampleData), DynamicDataSourceType.Method)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void ExampleIsValid_using_experimental_symbolic_names(EmbeddedFile embeddedBicep)
            => RunExampleTest(embeddedBicep, BicepTestConstants.Features with { SymbolicNameCodegenEnabled = true }, ".symbolicnames.json");

        [DataTestMethod]
        [DynamicData(nameof(GetExtensibilityExampleData), DynamicDataSourceType.Method)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void ExampleIsValid_extensibility(EmbeddedFile embeddedBicep)
            => RunExampleTest(embeddedBicep, BicepTestConstants.Features with { ImportsEnabled = true }, ".json");

        [DataTestMethod]
        [DynamicData(nameof(GetAllExampleData), DynamicDataSourceType.Method)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Example_uses_consistent_formatting(EmbeddedFile embeddedBicep)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, embeddedBicep);
            var bicepFile = baselineFolder.EntryFile;

            var program = ParserHelper.Parse(embeddedBicep.Contents);
            var printOptions = new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, true);

            var formattedContents = PrettyPrinter.PrintProgram(program, printOptions);
            formattedContents.Should().NotBeNull();

            File.WriteAllText(bicepFile.OutputFilePath, formattedContents);

            bicepFile.ShouldHaveExpectedValue();
        }

        [TestMethod]
        public void ExampleData_should_return_a_number_of_records()
        {
            GetAllExampleData().Should().HaveCountGreaterOrEqualTo(30, "sanity check to ensure we're finding examples to test");
        }

        private static string GetParentStreamName(string streamName)
            => Path.GetDirectoryName(streamName)!.Replace('\\', '/');

        private static IEnumerable<object[]> GetAllExampleData()
            => ExampleData.GetAllExampleData().Select(x => new object[] { x.BicepFile });

        private static IEnumerable<object[]> GetExampleData()
            => ExampleData.GetAllExampleData().Where(x => !x.IsExtensibilitySample).Select(x => new object[] { x.BicepFile });

        private static IEnumerable<object[]> GetExtensibilityExampleData()
            => ExampleData.GetAllExampleData().Where(x => x.IsExtensibilitySample).Select(x => new object[] { x.BicepFile });

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

        public record ExampleData(
            EmbeddedFile BicepFile,
            bool IsExtensibilitySample)
        {
            public static IEnumerable<ExampleData> GetAllExampleData()
            {
                var embeddedFiles = EmbeddedFile.LoadAll(
                    typeof(Bicep.Core.Samples.AssemblyInitializer).Assembly,
                    "user_submitted",
                    streamName => Path.GetExtension(streamName) == ".bicep");

                foreach (var bicepFile in embeddedFiles)
                {
                    var isExtensibilitySample = bicepFile.StreamPath.StartsWith($"Files/user_submitted/extensibility/", StringComparison.Ordinal);

                    yield return new ExampleData(
                        BicepFile: bicepFile,
                        IsExtensibilitySample: isExtensibilitySample);
                }
            }
        }
    }
}
