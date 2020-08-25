// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bicep.Core.Emit;
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Json;
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
            string filePath = FileHelper.GetResultFilePath(this.TestContext!, $"{dataSet.Name}_Compiled_Original.json");

            // emitting the template should be successful
            var result = this.EmitTemplate(dataSet.Bicep, filePath);
            result.Status.Should().Be(EmitStatus.Succeeded);
            result.Diagnostics.Should().BeEmpty();

            // normalizing the formatting in case there are differences in indentation
            // this way the diff between actual and expected will be clean
            var actual = JToken.Parse(File.ReadAllText(filePath));
            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Compiled_Actual.json", actual.ToString(Formatting.Indented));

            var expected = JToken.Parse(dataSet.Compiled!);
            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Compiled_Expected.json", expected.ToString(Formatting.Indented));

            JsonAssert.AreEqual(expected, actual, this.TestContext!, $"{dataSet.Name}_Compiled_Delta.json");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void ValidBicepTextWriter_TemplateEmiterShouldProduceExpectedTemplate(DataSet dataSet)
        {
            MemoryStream memoryStream = new MemoryStream();

            // emitting the template should be successful
            var result = this.EmitTemplate(dataSet.Bicep, memoryStream);
            result.Status.Should().Be(EmitStatus.Succeeded);
            result.Diagnostics.Should().BeEmpty();

            // normalizing the formatting in case there are differences in indentation
            // this way the diff between actual and expected will be clean
            var actual = JToken.ReadFrom(new JsonTextReader(new StreamReader(new MemoryStream(memoryStream.ToArray()))));
            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Compiled_Actual.json", actual.ToString(Formatting.Indented));

            var expected = JToken.Parse(dataSet.Compiled!);
            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Compiled_Expected.json", expected.ToString(Formatting.Indented));

            JsonAssert.AreEqual(expected, actual, this.TestContext!, $"{dataSet.Name}_Compiled_Delta.json");
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
            var compilation = new Compilation(SyntaxFactory.CreateFromText(text));
            var emitter = new TemplateEmitter(compilation.GetSemanticModel());

            return emitter.Emit(filePath);
        }

        private EmitResult EmitTemplate(string text, MemoryStream memoryStream)
        {
            var compilation = new Compilation(SyntaxFactory.CreateFromText(text));
            var emitter = new TemplateEmitter(compilation.GetSemanticModel());

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

