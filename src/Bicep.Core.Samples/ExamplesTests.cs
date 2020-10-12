// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
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

        private static bool IsPermittedMissingTypeDiagnostic(Diagnostic diagnostic)
        {
            if (diagnostic.Code != "BCP081")
            {
                return false;
            }

            var permittedMissingTypeDiagnostics = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Resource type \"Microsoft.AppConfiguration/configurationStores@2020-07-01-preview\" does not have types available.",
                "Resource type \"Microsoft.AppConfiguration/configurationStores/keyValues@2020-07-01-preview\" does not have types available.",
                "Resource type \"Microsoft.Web/sites/config@2018-11-01\" does not have types available.",
                "Resource type \"Microsoft.KeyVault/vaults/keys@2019-09-01\" does not have types available.",
                "Resource type \"Microsoft.KeyVault/vaults/secrets@2018-02-14\" does not have types available.",
                "Resource type \"microsoft.web/serverFarms@2018-11-01\" does not have types available.",
                "Resource type \"Microsoft.OperationalInsights/workspaces/providers/diagnosticSettings@2017-05-01-preview\" does not have types available.",
                "Resource type \"Microsoft.Sql/servers@2020-02-02-preview\" does not have types available.",
                "Resource type \"Microsoft.Sql/servers/databases@2020-02-02-preview\" does not have types available.",
                "Resource type \"Microsoft.Sql/servers/databases/transparentDataEncryption@2017-03-01-preview\" does not have types available.",
                "Resource type \"Microsoft.Web/sites/config@2020-06-01\" does not have types available.",
                "Resource type \"Microsoft.KeyVault/vaults/secrets@2019-09-01\" does not have types available.",
                "Resource type \"Microsoft.KeyVault/vaults@2019-06-01\" does not have types available.",
                "Resource type \"microsoft.network/networkSecurityGroups@2020-08-01\" does not have types available."
            };

            return permittedMissingTypeDiagnostics.Contains(diagnostic.Message);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExampleData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExampleData), DynamicDataDisplayName = nameof(ExampleData.GetDisplayName))]
        public void ExampleIsValid(ExampleData example)
        {
            var compilation = new Compilation(new AzResourceTypeProvider(), SyntaxFactory.CreateFromText(example.BicepContents));
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

            using var stream = new MemoryStream();
            var result = emitter.Emit(stream);

            // allow 'type not available' warnings for examples
            var diagnostics = result.Diagnostics.Where(x => !(IsPermittedMissingTypeDiagnostic(x)));
            
            diagnostics.Should().BeEmpty();
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

