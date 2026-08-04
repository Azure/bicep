// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Testing.Baselines;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
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

        public static async Task RunExampleTest(TestContext testContext, TestEmbeddedFile embeddedBicep, FeatureProviderOverrides? features = null, string jsonFileExtension = ".json")
        {
            features ??= new(testContext);
            FileHelper.GetCacheRootDirectory(testContext).EnsureExists();
            var baselineFiles = testContext.MaterializeBaseline(embeddedBicep);
            var bicepFile = baselineFiles.EntryFile;
            var jsonFile = baselineFiles.GetFile(Path.ChangeExtension(embeddedBicep.FileName, jsonFileExtension));

            var compiler = Services.WithFeatureOverrides(features).Build().GetCompiler();
            var compilation = await compiler.CreateCompilation(IOUri.FromFilePath(bicepFile.OutputFilePath));
            var model = compilation.GetEntrypointSemanticModel();

            var emitter = new TemplateEmitter(model);

            foreach (var (file, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
            {
                DiagnosticAssertions.DoWithDiagnosticAnnotations(
                    file,
                    diagnostics.Where(d => !IsPermittedMissingTypeDiagnostic(d)),
                    diagnostics =>
                    {
                        diagnostics.Should().BeEmpty("{0} should not have warnings or errors", file.FileHandle.Uri);
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
                    stringWriter.ToString().Should().MatchJsonBaseline(jsonFile);

                    // validate that the template is parseable by the deployment engine
                    UnitTests.Utils.TemplateHelper.TemplateShouldBeValid(stringWriter.ToString(), model.Features);
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetAllExampleData), DynamicDataSourceType.Method)]
        [TestCategory(TestCategories.Baseline)]
        public Task ExampleIsValid(TestEmbeddedFile embeddedBicep)
            => RunExampleTest(TestContext, embeddedBicep, new(TestContext), ".json");

        [DataTestMethod]
        [DynamicData(nameof(GetAllExampleData), DynamicDataSourceType.Method)]
        [TestCategory(TestCategories.Baseline)]
        public Task ExampleIsValid_using_experimental_symbolic_names(TestEmbeddedFile embeddedBicep)
            => RunExampleTest(TestContext, embeddedBicep, new(TestContext, SymbolicNameCodegenEnabled: true), ".symbolicnames.json");

        [DataTestMethod]
        [DynamicData(nameof(GetAllExampleData), DynamicDataSourceType.Method)]
        [TestCategory(TestCategories.Baseline)]
        public void Example_uses_consistent_formatting(TestEmbeddedFile embeddedBicep)
        {
            var baselineFiles = TestContext.MaterializeBaseline(embeddedBicep);
            var bicepFile = baselineFiles.EntryFile;

            var program = ParserHelper.Parse(embeddedBicep.Contents, out var lexingErrorLookup, out var parsingErrorLookup);
            var context = PrettyPrinterV2Context.Create(PrettyPrinterV2Options.Default, lexingErrorLookup, parsingErrorLookup);
            var formattedContents = PrettyPrinterV2.Print(program, context);
            formattedContents.Should().NotBeNull();

            formattedContents.Should().MatchTextBaseline(bicepFile);
        }

        [TestMethod]
        public void ExampleData_should_return_a_number_of_records()
        {
            GetAllExampleData().Should().HaveCountGreaterOrEqualTo(30, "sanity check to ensure we're finding examples to test");
        }

        private static IEnumerable<object[]> GetAllExampleData()
            => ExampleData.GetAllExampleData().Select(x => new object[] { x.BicepFile });

        private static bool IsPermittedMissingTypeDiagnostic(IDiagnostic diagnostic)
        {
            if (diagnostic.Code != "BCP081")
            {
                return false;
            }

            var permittedMissingTypeDiagnostics = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // To exclude a particular type for BCP081 (if there are missing types), add an entry of format:
                // "Resource type \"<type>\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed.",
            };

            return permittedMissingTypeDiagnostics.Contains(diagnostic.Message);
        }

        public record ExampleData(
            TestEmbeddedFile BicepFile)
        {
            public static IEnumerable<ExampleData> GetAllExampleData()
            {
                var embeddedFiles = TestEmbeddedFile.LoadAll(
                    typeof(Bicep.Core.Samples.AssemblyInitializer).Assembly,
                    "user_submitted",
                    streamName => Path.GetExtension(streamName) == ".bicep");

                foreach (var bicepFile in embeddedFiles)
                {
                    yield return new ExampleData(bicepFile);
                }
            }
        }
    }
}
