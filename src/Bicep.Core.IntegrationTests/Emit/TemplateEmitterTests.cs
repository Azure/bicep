// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.Emit
{
    [TestClass]
    public class TemplateEmitterTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ValidBicep_TemplateEmiterShouldProduceExpectedTemplate(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var clientFactory = dataSet.CreateMockRegistryClients(TestContext);
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var compiledFilePath = FileHelper.GetResultFilePath(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainCompiled));

            // emitting the template should be successful
            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, clientFactory, templateSpecRepositoryFactory, BicepTestConstants.CreateFeaturesProvider(TestContext, registryEnabled: dataSet.HasExternalModules)));
            Workspace workspace = new();
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, workspace, PathHelper.FilePathToFileUrl(bicepFilePath));
            if (await dispatcher.RestoreModules(dispatcher.GetValidModuleReferences(sourceFileGrouping.ModulesToRestore)))
            {
                sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(dispatcher, workspace, sourceFileGrouping);
            }

            var result = this.EmitTemplate(sourceFileGrouping, BicepTestConstants.EmitterSettings, compiledFilePath);
            result.Diagnostics.Should().NotHaveErrors();
            result.Status.Should().Be(EmitStatus.Succeeded);

            var outputFile = File.ReadAllText(compiledFilePath);
            var actual = JToken.Parse(outputFile);

            actual.Should().EqualWithJsonDiffOutput(
                TestContext,
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);

            // validate that the template is parseable by the deployment engine
            TemplateHelper.TemplateShouldBeValid(outputFile);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ValidBicep_EmitTemplate_should_produce_expected_symbolicname_template(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var clientFactory = dataSet.CreateMockRegistryClients(TestContext);
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            var compiledFilePath = FileHelper.GetResultFilePath(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainCompiledWithSymbolicNames));

            // emitting the template should be successful
            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, clientFactory, templateSpecRepositoryFactory, BicepTestConstants.CreateFeaturesProvider(TestContext, registryEnabled: dataSet.HasRegistryModules)));
            Workspace workspace = new();
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, workspace, PathHelper.FilePathToFileUrl(bicepFilePath));
            if (await dispatcher.RestoreModules(dispatcher.GetValidModuleReferences(sourceFileGrouping.ModulesToRestore)))
            {
                sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(dispatcher, workspace, sourceFileGrouping);
            }

            var result = this.EmitTemplate(sourceFileGrouping, BicepTestConstants.EmitterSettingsWithSymbolicNames, compiledFilePath);
            result.Diagnostics.Should().NotHaveErrors();
            result.Status.Should().Be(EmitStatus.Succeeded);

            var outputFile = File.ReadAllText(compiledFilePath);
            var actual = JToken.Parse(outputFile);

            actual.Should().EqualWithJsonDiffOutput(
                TestContext,
                JToken.Parse(dataSet.CompiledWithSymbolicNames!),
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainCompiledWithSymbolicNames),
                actualLocation: compiledFilePath);

            // validate that the template is parseable by the deployment engine
            TemplateHelper.TemplateShouldBeValid(outputFile);
        }

        [TestMethod]
        public void TemplateEmitter_output_should_not_include_UTF8_BOM()
        {
            var sourceFileGrouping = SourceFileGroupingFactory.CreateFromText("", BicepTestConstants.FileResolver);
            var compiledFilePath = FileHelper.GetResultFilePath(this.TestContext, "main.json");

            // emitting the template should be successful
            var result = this.EmitTemplate(sourceFileGrouping, BicepTestConstants.EmitterSettings, compiledFilePath);
            result.Diagnostics.Should().BeEmpty();
            result.Status.Should().Be(EmitStatus.Succeeded);

            var bytes = File.ReadAllBytes(compiledFilePath);
            // No BOM at the start of the file
            bytes.Take(3).Should().NotBeEquivalentTo(new [] { 0xEF, 0xBB, 0xBF }, "BOM should not be present");
            bytes.First().Should().Be(0x7B, "template should always begin with a UTF-8 encoded open curly");
            bytes.Last().Should().Be(0x7D, "template should always end with a UTF-8 encoded close curly");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ValidBicepTextWriter_TemplateEmiterShouldProduceExpectedTemplate(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var clientFactory = dataSet.CreateMockRegistryClients(TestContext);
            var templateSpecRepositoryFactory = dataSet.CreateMockTemplateSpecRepositoryFactory(TestContext);
            await dataSet.PublishModulesToRegistryAsync(clientFactory, TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            MemoryStream memoryStream = new MemoryStream();

            // emitting the template should be successful
            var dispatcher = new ModuleDispatcher(new DefaultModuleRegistryProvider(BicepTestConstants.FileResolver, clientFactory, templateSpecRepositoryFactory, BicepTestConstants.CreateFeaturesProvider(TestContext, registryEnabled: dataSet.HasExternalModules)));
            var workspace = new Workspace();
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, workspace, PathHelper.FilePathToFileUrl(bicepFilePath));
            if (await dispatcher.RestoreModules(dispatcher.GetValidModuleReferences(sourceFileGrouping.ModulesToRestore)))
            {
                sourceFileGrouping = SourceFileGroupingBuilder.Rebuild(dispatcher, workspace, sourceFileGrouping);
            }

            var result = this.EmitTemplate(sourceFileGrouping, BicepTestConstants.EmitterSettings, memoryStream);
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
        public void InvalidBicep_TemplateEmiterShouldNotProduceAnyTemplate(DataSet dataSet)
        {
            var outputDirectory = dataSet.SaveFilesToTestDirectory(TestContext);
            var bicepFilePath = Path.Combine(outputDirectory, DataSet.TestFileMain);
            string filePath = FileHelper.GetResultFilePath(this.TestContext, $"{dataSet.Name}_Compiled_Original.json");

            // emitting the template should fail
            var dispatcher = new ModuleDispatcher(BicepTestConstants.RegistryProvider);
            var result = this.EmitTemplate(SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, new Workspace(), PathHelper.FilePathToFileUrl(bicepFilePath)), BicepTestConstants.EmitterSettings, filePath);
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

            var expected = string.Join(newlineSequence, new [] { "this", "  is", "    a", "      multiline", "        string", "" });
            template.Should().HaveValueAtPath("$.variables.multiline", expected);
        }

        [TestMethod]
        public void TemplateEmitter_should_not_dispose_text_writer()
        {
            var (_, _, compilation) = CompilationHelper.Compile(string.Empty);

            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), BicepTestConstants.EmitterSettings);
            emitter.Emit(stringWriter);

            // second write should succeed if stringWriter wasn't closed
            emitter.Emit(stringWriter);
        }

        private EmitResult EmitTemplate(SourceFileGrouping sourceFileGrouping, EmitterSettings emitterSettings, string filePath)
        {
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), sourceFileGrouping, BicepTestConstants.BuiltInConfiguration);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), emitterSettings);

            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            return emitter.Emit(stream);
        }

        private EmitResult EmitTemplate(SourceFileGrouping sourceFileGrouping, EmitterSettings emitterSettings, MemoryStream memoryStream)
        {
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), sourceFileGrouping, BicepTestConstants.BuiltInConfiguration);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel(), emitterSettings);

            TextWriter tw = new StreamWriter(memoryStream);
            return emitter.Emit(tw);
        }

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

