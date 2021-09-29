// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;

namespace Bicep.Core.Samples
{
    [TestClass]
    public class ExamplesTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private class IndexFileEntry
        {
            public string? FilePath { get; set; }

            public string? Description { get; set; }
        }

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

            public static string GetDisplayName(MethodInfo info, object[] data) => ((ExampleData)data[0]).BicepStreamName!;
        }

        private static string GetParentStreamName(string streamName)
            => Path.GetDirectoryName(streamName)!.Replace('\\', '/');

        private static IEnumerable<object[]> GetExampleData()
        {
            const string pathPrefix = "docs/examples/";
            const string bicepExtension = ".bicep";

            foreach (var streamName in typeof(ExamplesTests).Assembly.GetManifestResourceNames().Where(p => p.StartsWith(pathPrefix, StringComparison.Ordinal)))
            {
                var extension = Path.GetExtension(streamName);
                if (!StringComparer.OrdinalIgnoreCase.Equals(extension, bicepExtension))
                {
                    continue;
                }

                var outputFolderName = streamName
                    .Substring(0, streamName.Length - bicepExtension.Length)
                    .Substring(pathPrefix.Length)
                    .Replace('/', '_');

                var exampleData = new ExampleData(streamName, Path.ChangeExtension(streamName, "json"), outputFolderName);

                yield return new object[] { exampleData };
            }
        }

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

        [TestMethod]
        public void ExampleData_should_return_a_number_of_records()
        {
            GetExampleData().Should().HaveCountGreaterOrEqualTo(30, "sanity check to ensure we're finding examples to test");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExampleData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExampleData), DynamicDataDisplayName = nameof(ExampleData.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void ExampleIsValid(ExampleData example)
        {
            // save all the files in the containing directory to disk so that we can test module resolution
            var parentStream = GetParentStreamName(example.BicepStreamName);
            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(ExamplesTests).Assembly, parentStream);
            var bicepFileName = Path.Combine(outputDirectory, Path.GetFileName(example.BicepStreamName));
            var jsonFileName = Path.Combine(outputDirectory, Path.GetFileName(example.JsonStreamName));

            var dispatcher = new ModuleDispatcher(BicepTestConstants.RegistryProvider);
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, new Workspace(), PathHelper.FilePathToFileUrl(bicepFileName));
            var configuration = BicepTestConstants.BuiltInConfigurationWithAnalyzersDisabled;
            var compilation = new Compilation(BicepTestConstants.NamespaceProvider, sourceFileGrouping, configuration);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), BicepTestConstants.EmitterSettings);

            foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
            {
                DiagnosticAssertions.DoWithDiagnosticAnnotations(
                    bicepFile,
                    diagnostics.Where(x => !IsPermittedMissingTypeDiagnostic(x)),
                    diagnostics => {
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
                        example.JsonStreamName,
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
            example.JsonStreamName.Should().StartWith("docs/examples/");
            var relativeJsonStreamName = "experimental/symbolicnames/" + example.JsonStreamName.Substring("docs/examples/".Length);

            // save all the files in the containing directory to disk so that we can test module resolution
            var parentStream = GetParentStreamName(example.BicepStreamName);
            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(ExamplesTests).Assembly, parentStream);
            var bicepFileName = Path.Combine(outputDirectory, Path.GetFileName(example.BicepStreamName));

            var jsonParentStream = GetParentStreamName($"Bicep.Core.Samples/{relativeJsonStreamName}");
            var jsonOutputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(ExamplesTests).Assembly, jsonParentStream);
            var jsonFileName = Path.Combine(jsonOutputDirectory, Path.GetFileName(relativeJsonStreamName));

            var dispatcher = new ModuleDispatcher(BicepTestConstants.RegistryProvider);
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, new Workspace(), PathHelper.FilePathToFileUrl(bicepFileName));
            var configuration = BicepTestConstants.BuiltInConfigurationWithAnalyzersDisabled;
            var compilation = new Compilation(BicepTestConstants.NamespaceProvider, sourceFileGrouping, configuration);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), BicepTestConstants.EmitterSettingsWithSymbolicNames);

            foreach (var (bicepFile, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
            {
                DiagnosticAssertions.DoWithDiagnosticAnnotations(
                    bicepFile,
                    diagnostics.Where(x => !IsPermittedMissingTypeDiagnostic(x)),
                    diagnostics => {
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
                        $"src/Bicep.Core.Samples/Files/{relativeJsonStreamName}",
                        jsonFileName + ".actual");

                    // validate that the template is parseable by the deployment engine
                    TemplateHelper.TemplateShouldBeValid(generated);
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExampleData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExampleData), DynamicDataDisplayName = nameof(ExampleData.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Example_uses_consistent_formatting(ExampleData example)
        {
            // save all the files in the containing directory to disk so that we can test module resolution
            var parentStream = GetParentStreamName(example.BicepStreamName);
            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(ExamplesTests).Assembly, parentStream);

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
                expectedLocation: example.BicepStreamName,
                actualLocation: bicepFileName + ".formatted");
        }

        [TestMethod]
        public void Examples_have_been_added_to_index_json()
        {
            var exampleData = GetExampleData().Select(x => (ExampleData)x[0]).ToArray();
            var indexFile = "docs/examples/index.json";
            var indexFileStream = typeof(ExamplesTests).Assembly.GetManifestResourceStream(indexFile);
            var indexContents = JsonConvert.DeserializeObject<IndexFileEntry[]>(new StreamReader(indexFileStream!).ReadToEnd());

            indexContents.Should().NotContain(x => string.IsNullOrWhiteSpace(x.FilePath));
            indexContents.Should().NotContain(x => string.IsNullOrWhiteSpace(x.Description));

            var indexFiles = indexContents!.Select(x => $"docs/examples/{x.FilePath}");
            var exampleFiles = exampleData.Select(x => x.BicepStreamName).Where(x => x.EndsWith("/main.bicep", StringComparison.Ordinal));

            exampleFiles.Should().BeSubsetOf(indexFiles, $"all \"main.bicep\" example files should be added to \"{indexFile}\"");
            indexFiles.Should().BeSubsetOf(exampleFiles, $"\"{indexFile}\" should only contain valid \"main.bicep\" example files");
        }

        [TestMethod]
        public void Example_folders_must_all_have_a_main_file()
        {
            var exampleData = GetExampleData().Select(x => (ExampleData)x[0]).ToArray();
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
    }
}
