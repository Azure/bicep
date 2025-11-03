// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using Bicep.TextFixtures.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.Emit
{
    [TestClass]
    public class TemplateEmitterTests
    {
        private static ServiceBuilder Services => new ServiceBuilder()
            .WithEnvironmentVariables(
                ("stringEnvVariableName", "test"),
                ("intEnvVariableName", "100"),
                ("boolEnvironmentVariable", "true")
            );

        [NotNull]
        public TestContext? TestContext { get; set; }

        private async Task<Compilation> GetCompilation(DataSet dataSet, FeatureProviderOverrides features)
        {
            // Use a unique cache root directory for each test run to avoid conflicts
            FileHelper.GetCacheRootDirectory(TestContext).EnsureExists();
            features = features with { CacheRootDirectory = FileHelper.GetCacheRootDirectory(TestContext) };

            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);

            var artifactManager = new TestExternalArtifactManager(TestCompiler.ForMockFileSystemCompilation().WithFeatureOverrides<FeatureProviderOverrides, OverriddenFeatureProviderFactory>(features));
            await dataSet.PublishAllDataSetArtifacts(artifactManager, publishSource: true);

            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var bicepFileUri = PathHelper.FilePathToFileUrl(bicepFilePath);

            var compiler = Services
                .WithFeatureOverrides(features)
                .WithTestArtifactManager(artifactManager)
                .Build()
                .GetCompiler();

            return await compiler.CreateCompilation(bicepFileUri.ToIOUri());
        }

        private async Task<Compilation> GetCompilation(BaselineData_Bicepparam baseline, FeatureProviderOverrides? features = null)
        {
            // Use a unique cache root directory for each test run to avoid conflicts
            FileHelper.GetCacheRootDirectory(TestContext).EnsureExists();
            features = (features ?? CreateDefaultFeatureProviderOverrides()) with { CacheRootDirectory = FileHelper.GetCacheRootDirectory(TestContext) };

            var artifactManager = await CreateDefaultExternalArtifactManager();

            var compiler = Services
                .WithFeatureOverrides(features)
                .WithTestArtifactManager(artifactManager)
                .Build()
                .GetCompiler();

            return await compiler.CreateCompilation(baseline.GetData(TestContext).Parameters.OutputFileUri.ToIOUri());
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ValidBicep_TemplateEmiterShouldProduceExpectedTemplate(DataSet dataSet)
        {
            var compiledFilePath = FileHelper.GetResultFilePath(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainCompiled));
            var compilation = await GetCompilation(dataSet, new(TestContext));

            var result = EmitTemplate(compilation, compiledFilePath);
            result.Diagnostics.Should().NotHaveErrors();
            result.Status.Should().Be(EmitStatus.Succeeded);
            result.Features.Should().NotBeNull();

            var outputFile = File.ReadAllText(compiledFilePath);
            var actual = JToken.Parse(outputFile);

            actual.Should().EqualWithJsonDiffOutput(
                TestContext,
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);

            // validate that the template is parseable by the deployment engine
            UnitTests.Utils.TemplateHelper.TemplateShouldBeValid(outputFile, result.Features!);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ValidBicep_EmitTemplate_should_produce_expected_symbolicname_template(DataSet dataSet)
        {
            var compiledFilePath = FileHelper.GetResultFilePath(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainCompiledWithSymbolicNames));
            var compilation = await GetCompilation(dataSet, new(TestContext, SymbolicNameCodegenEnabled: true));

            var result = EmitTemplate(compilation, compiledFilePath);
            result.Diagnostics.Should().NotHaveErrors();
            result.Status.Should().Be(EmitStatus.Succeeded);
            result.Features.Should().NotBeNull();

            var outputFile = File.ReadAllText(compiledFilePath);
            var actual = JToken.Parse(outputFile);

            actual.Should().EqualWithJsonDiffOutput(
                TestContext,
                JToken.Parse(dataSet.CompiledWithSymbolicNames!),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainCompiledWithSymbolicNames),
                actualLocation: compiledFilePath);

            // validate that the template is parseable by the deployment engine
            UnitTests.Utils.TemplateHelper.TemplateShouldBeValid(outputFile, result.Features!);
        }

        [DataTestMethod]
        [EmbeddedFilesTestData(@"Files/SourceMapping/.*/main.bicep")]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Source_map_generation_should_work(EmbeddedFile file)
        {
            var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, file);
            var bicepFile = baselineFolder.EntryFile;
            var sourceMapFile = baselineFolder.GetFileOrEnsureCheckedIn("sourcemap.json");

            var features = new FeatureProviderOverrides(TestContext, SourceMappingEnabled: true);
            var compiler = ServiceBuilder.Create(s => s.WithFeatureOverrides(features)).GetCompiler();
            var bicepUri = PathHelper.FilePathToFileUrl(bicepFile.OutputFilePath).ToIOUri();

            var compilation = await compiler.CreateCompilation(bicepUri);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());
            using var memoryStream = new MemoryStream();
            var emitResult = emitter.Emit(memoryStream);

            emitResult.Status.Should().Be(EmitStatus.Succeeded);
            emitResult.SourceMap.Should().NotBeNull();

            // Here we simply verify that the format of the baseline file looks correct.
            var sourceMapJson = JToken.FromObject(emitResult.SourceMap!);
            sourceMapFile.WriteToOutputFolder(sourceMapJson.ToString());
            sourceMapFile.ShouldHaveExpectedJsonValue();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task SourceMap_maps_json_to_bicep_lines(DataSet dataSet)
        {
            var features = new FeatureProviderOverrides(TestContext, SourceMappingEnabled: true);
            var (compilation, outputDirectory, _) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext, features);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());
            using var memoryStream = new MemoryStream();
            var emitResult = emitter.Emit(memoryStream);

            emitResult.Status.Should().Be(EmitStatus.Succeeded);
            emitResult.SourceMap.Should().NotBeNull();
            var sourceMap = emitResult.SourceMap!;

            using var streamReader = new StreamReader(new MemoryStream(memoryStream.ToArray()));
            var jsonLines = (await streamReader.ReadToEndAsync()).Split("\n");

            var sourceTextWithSourceMap = OutputHelper.AddSourceMapToSourceText(
                dataSet.Bicep, DataSet.TestFileMain, dataSet.HasCrLfNewlines() ? "\r\n" : "\n", sourceMap, jsonLines);
            var sourceTextWithSourceMapFileName = Path.Combine(outputDirectory, DataSet.TestFileMainSourceMap);
            File.WriteAllText(sourceTextWithSourceMapFileName, sourceTextWithSourceMap.ToString());

            // Here we validate visually that the in-memory source map can be used to map JSON -> Bicep lines
            sourceTextWithSourceMap.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                dataSet.SourceMap!,
                expectedPath: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainSourceMap),
                actualPath: sourceTextWithSourceMapFileName);
        }

        [TestMethod]
        public void TemplateEmitter_output_should_not_include_UTF8_BOM()
        {
            var compilationResult = CompilationHelper.Compile("");
            var compiledFilePath = FileHelper.GetResultFilePath(this.TestContext, "main.json");

            // emitting the template should be successful
            var result = this.EmitTemplate(compilationResult.Compilation, compiledFilePath);
            result.Diagnostics.Should().BeEmpty();
            result.Status.Should().Be(EmitStatus.Succeeded);

            var bytes = File.ReadAllBytes(compiledFilePath);
            // No BOM at the start of the file
            bytes.Take(3).Should().NotBeEquivalentTo(new[] { 0xEF, 0xBB, 0xBF }, "BOM should not be present");
            bytes.First().Should().Be(0x7B, "template should always begin with a UTF-8 encoded open curly");
            bytes.Last().Should().Be(0x7D, "template should always end with a UTF-8 encoded close curly");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ValidBicepTextWriter_TemplateEmiterShouldProduceExpectedTemplate(DataSet dataSet)
        {
            var compilation = await GetCompilation(dataSet, new(TestContext));

            var memoryStream = new MemoryStream();
            var result = this.EmitTemplate(compilation, memoryStream);
            result.Diagnostics.Should().NotHaveErrors();
            result.Status.Should().Be(EmitStatus.Succeeded);

            // normalizing the formatting in case there are differences in indentation
            // this way the diff between actual and expected will be clean
            var actual = JToken.ReadFrom(new JsonTextReader(new StreamReader(new MemoryStream(memoryStream.ToArray()))));
            var compiledFilePath = FileHelper.SaveResultFile(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainCompiled), actual.ToString(Formatting.Indented));

            actual.Should().EqualWithJsonDiffOutput(
                TestContext,
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task InvalidBicep_TemplateEmiterShouldNotProduceAnyTemplate(DataSet dataSet)
        {
            var compilation = await GetCompilation(dataSet, new(TestContext));
            string filePath = FileHelper.GetResultFilePath(this.TestContext, $"{dataSet.Name}_Compiled_Original.json");

            // emitting the template should fail
            var result = this.EmitTemplate(compilation, filePath);
            result.Diagnostics.Should().NotBeEmpty();
            result.Status.Should().Be(EmitStatus.Failed);
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.ValidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Valid_bicepparam_TemplateEmiter_should_produce_expected_template(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);
            data.Compiled.Should().NotBeNull();

            var compilation = await GetCompilation(baselineData);

            var result = this.EmitParam(compilation, data.Compiled!.OutputFilePath);

            result.Diagnostics.Should().NotHaveErrors();
            result.Status.Should().Be(EmitStatus.Succeeded);

            data.Compiled.ShouldHaveExpectedJsonValue();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData(Filter = BaselineData_Bicepparam.TestDataFilterType.InvalidOnly)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task Invalid_bicepparam_TemplateEmiter_should_not_produce_a_template(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var compilation = await GetCompilation(baselineData);

            var result = this.EmitParam(compilation, Path.ChangeExtension(data.Parameters.OutputFilePath, ".json"));

            result.Diagnostics.Should().NotBeEmpty();
            result.Status.Should().Be(EmitStatus.Failed);
        }

        [DataTestMethod]
        [DataRow("\n")]
        [DataRow("\r\n")]
        public void Multiline_strings_should_parse_correctly(string newlineSequence)
        {
            var inputFile = @"
var multiline = '''
this
  is
    a
      multiline
        string
'''
";

            var (template, _, _) = CompilationHelper.Compile(StringUtils.ReplaceNewlines(inputFile, newlineSequence));

            var expected = string.Join(newlineSequence, ["this", "  is", "    a", "      multiline", "        string", ""]);
            template.Should().HaveValueAtPath("$.variables.multiline", expected);
        }

        [TestMethod]
        [DataRow("\n")]
        [DataRow("\r\n")]
        public void Multiline_interpolated_strings_should_parse_correctly(string newlineSequence)
        {
            var inputFile = @"
var name = 'World'
var multiline = $'''
Hello ${name}!
'''

var multiline2 = $$'''
Hello $${name}!
'''
";

            var (template, _, _) = CompilationHelper.Compile(ServiceBuilder.CreateWithFeatures(new(MultilineStringInterpolationEnabled: true)), StringUtils.ReplaceNewlines(inputFile, newlineSequence));

            var expected1 = string.Join(newlineSequence, ["[format('Hello {0}!", "', variables('name'))]"]);
            template.Should().HaveValueAtPath("$.variables.multiline", expected1);

            var expected2 = string.Join(newlineSequence, ["[format('Hello {0}!", "', variables('name'))]"]);
            template.Should().HaveValueAtPath("$.variables.multiline2", expected2);
        }

        [TestMethod]
        public void TemplateEmitter_should_not_dispose_text_writer()
        {
            var (_, _, compilation) = CompilationHelper.Compile(string.Empty);

            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());
            emitter.Emit(stringWriter);

            // second write should succeed if stringWriter wasn't closed
            emitter.Emit(stringWriter);
        }

        private EmitResult EmitTemplate(Compilation compilation, string filePath)
        {
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            return emitter.Emit(stream);
        }

        private EmitResult EmitTemplate(Compilation compilation, MemoryStream memoryStream)
        {
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

            TextWriter tw = new StreamWriter(memoryStream);
            return emitter.Emit(tw);
        }

        private EmitResult EmitParam(Compilation compilation, string outputFilePath)
        {
            var emitter = new ParametersEmitter(compilation.GetEntrypointSemanticModel());
            using var stream = new FileStream(outputFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            return emitter.Emit(stream);
        }

        protected async Task<TestExternalArtifactManager> CreateDefaultExternalArtifactManager()
        {
            FileHelper.GetCacheRootDirectory(TestContext).EnsureExists();

            return await MockRegistry.CreateDefaultExternalArtifactManager(TestContext);
        }

        protected FeatureProviderOverrides CreateDefaultFeatureProviderOverrides() => new(TestContext);

        private static IEnumerable<object[]> GetValidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid)
            .ToDynamicTestData();

        private static IEnumerable<object[]> GetInvalidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid == false)
            .ToDynamicTestData();
    }
}
