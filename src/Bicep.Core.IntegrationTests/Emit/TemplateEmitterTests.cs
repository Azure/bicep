// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bicep.Core.Emit;
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.Emit
{
    [TestClass]
    public class TemplateEmitterTests
    {
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void ValidBicep_TemplateEmiterShouldProduceExpectedTemplate(DataSet dataSet)
        {
            var compiledFilePath = FileHelper.GetResultFilePath(this.TestContext!, Path.Combine(dataSet.Name, DataSet.TestFileMainCompiled));

            // emitting the template should be successful
            var result = this.EmitTemplate(dataSet.Bicep, compiledFilePath);
            result.Status.Should().Be(EmitStatus.Succeeded);
            result.Diagnostics.Should().BeEmpty();

            var actual = JToken.Parse(File.ReadAllText(compiledFilePath));

            actual.Should().EqualWithJsonDiffOutput(
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: Path.Combine("src", "Bicep.Core.Samples", dataSet.Name, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [TestMethod]
        public void TemplateEmitter_output_should_not_include_UTF8_BOM()
        {
            var compiledFilePath = FileHelper.GetResultFilePath(this.TestContext!, "main.json");

            // emitting the template should be successful
            var result = this.EmitTemplate("", compiledFilePath);
            result.Status.Should().Be(EmitStatus.Succeeded);
            result.Diagnostics.Should().BeEmpty();

            var bytes = File.ReadAllBytes(compiledFilePath);
            // No BOM at the start of the file
            bytes.Take(3).Should().NotBeEquivalentTo(new [] { 0xEF, 0xBB, 0xBF }, "BOM should not be present");
            bytes.First().Should().Be(0x7B, "template should always begin with a UTF-8 encoded open curly");
            bytes.Last().Should().Be(0x7D, "template should always end with a UTF-8 encoded close curly");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void ValidBicepTextWriter_TemplateEmiterShouldProduceExpectedTemplate(DataSet dataSet)
        {
            MemoryStream memoryStream = new MemoryStream();

            // emitting the template should be successful
            var result = this.EmitTemplate(dataSet.Bicep, memoryStream);
            result.Diagnostics.Should().BeEmpty();
            result.Status.Should().Be(EmitStatus.Succeeded);

            // normalizing the formatting in case there are differences in indentation
            // this way the diff between actual and expected will be clean
            var actual = JToken.ReadFrom(new JsonTextReader(new StreamReader(new MemoryStream(memoryStream.ToArray()))));
            var compiledFilePath = FileHelper.SaveResultFile(this.TestContext!, Path.Combine(dataSet.Name, DataSet.TestFileMainCompiled), actual.ToString(Formatting.Indented));

            actual.Should().EqualWithJsonDiffOutput(
                JToken.Parse(dataSet.Compiled!),
                expectedLocation: Path.Combine("src", "Bicep.Core.Samples", dataSet.Name, DataSet.TestFileMainCompiled),
                actualLocation: compiledFilePath);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void InvalidBicep_TemplateEmiterShouldNotProduceAnyTemplate(DataSet dataSet)
        {
            string filePath = FileHelper.GetResultFilePath(this.TestContext!, $"{dataSet.Name}_Compiled_Original.json");

            // emitting the template should fail
            var result = this.EmitTemplate(dataSet.Bicep, filePath);
            result.Status.Should().Be(EmitStatus.Failed);
            result.Diagnostics.Should().NotBeEmpty();
        }

        private EmitResult EmitTemplate(string text, string filePath)
        {
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(text));
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            return emitter.Emit(stream);
        }

        private EmitResult EmitTemplate(string text, MemoryStream memoryStream)
        {
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(text));
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

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

