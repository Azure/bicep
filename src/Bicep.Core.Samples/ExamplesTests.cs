using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Bicep.Core.Emit;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Json;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Samples
{
    [TestClass]
    public class ExamplesTests
    {
        public TestContext? TestContext { get; set; }

        public class ExampleData
        {
            public ExampleData(string jsonContents, string bicepContents, string bicepFileName)
            {
                JsonContents = jsonContents;
                BicepContents = bicepContents;
                BicepFileName = bicepFileName;
            }

            public string JsonContents { get; }

            public string BicepContents { get; }

            public string BicepFileName { get; }

            public static string GetDisplayName(MethodInfo info, object[] data) => ((ExampleData)data[0]).BicepFileName!;
        }

        private static IEnumerable<object[]> GetExampleData()
        {
            const string resourcePrefix = "Bicep.Core.Samples.DocsExamples.";
            const string bicepExtension = ".bicep";
            const string jsonExtension = ".json";

            var bicepResources = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(name => name.StartsWith(resourcePrefix) && name.EndsWith(bicepExtension));
            bicepResources.Should().NotBeEmpty("examples should be included for tests");

            foreach (var bicepResource in bicepResources)
            {
                var jsonResource = bicepResource.Substring(0, bicepResource.Length - bicepExtension.Length) + jsonExtension;
                
                var example = new ExampleData(
                    jsonContents: DataSet.ReadFile(jsonResource),
                    bicepContents: DataSet.ReadFile(bicepResource),
                    bicepFileName: bicepResource.Substring(resourcePrefix.Length)
                );

                yield return new object[] { example };
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExampleData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExampleData), DynamicDataDisplayName = nameof(ExampleData.GetDisplayName))]
        public void ExampleIsValid(ExampleData example)
        {
            var compilation = new Compilation(SyntaxFactory.CreateFromText(example.BicepContents));
            var emitter = new TemplateEmitter(compilation.GetSemanticModel());

            using var stream = new MemoryStream();
            var result = emitter.Emit(stream);
            
            result.Diagnostics.Should().BeEmpty();
            result.Status.Should().Be(EmitStatus.Succeeded);

            stream.Position = 0;
            var generated = new StreamReader(stream).ReadToEnd();

            var actual = JToken.Parse(generated);
            FileHelper.SaveResultFile(this.TestContext!, $"{example.BicepFileName}_Compiled_Actual.json", actual.ToString(Formatting.Indented));

            var expected = JToken.Parse(example.JsonContents!);
            FileHelper.SaveResultFile(this.TestContext!, $"{example.BicepFileName}_Compiled_Expected.json", expected.ToString(Formatting.Indented));

            JsonAssert.AreEqual(expected, actual, this.TestContext!, $"{example.BicepFileName}_Compiled_Delta.json");
        }
    }
}
