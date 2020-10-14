// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
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
            public ExampleData(string bicepFilename, string jsonFilename)
            {
                BicepFilename = bicepFilename;
                JsonFilename = jsonFilename;
            }

            public string BicepFilename { get; }

            public string JsonFilename { get; }

            public static string GetDisplayName(MethodInfo info, object[] data) => ((ExampleData)data[0]).BicepFilename!;
        }

        private static IEnumerable<object[]> GetExampleData()
        {
            foreach (var bicepFilename in Directory.EnumerateFiles("examples", "*.bicep", SearchOption.AllDirectories))
            {
                var jsonFilename = Path.ChangeExtension(bicepFilename, "json");

                var example = new ExampleData(bicepFilename, jsonFilename);

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
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), PathHelper.ResolvePath(example.BicepFilename));
            var compilation = new Compilation(new AzResourceTypeProvider(), syntaxTreeGrouping);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

            var diagnosticsBySyntaxTree = syntaxTreeGrouping.SyntaxTrees.ToDictionary(x => x, x => new List<Diagnostic>());
            compilation.EmitDiagnosticsAndCheckSuccess(
                (syntaxTree, diagnostic) => {
                    if (IsPermittedMissingTypeDiagnostic(diagnostic))
                    {
                        // we allow certain 'missing type' diagnostics in examples
                        return;
                    }

                    diagnosticsBySyntaxTree[syntaxTree].Add(diagnostic);
                });

            using var stream = new MemoryStream();
            var result = emitter.Emit(stream);
            
            // group assertion failures using AssertionScope, rather than reporting the first failure
            using (new AssertionScope())
            {
                foreach (var syntaxTree in syntaxTreeGrouping.SyntaxTrees)
                {
                    diagnosticsBySyntaxTree[syntaxTree].Should().BeEmpty($"{Path.GetRelativePath(Directory.GetCurrentDirectory(), syntaxTree.FilePath)} should not have warnings or errors");
                }

                var exampleSourceFilePath = Path.Combine("docs", example.JsonFilename);
                var exampleExists = File.Exists(example.JsonFilename);
                exampleExists.Should().BeTrue($"Generated example \"{exampleSourceFilePath}\" should be checked in");

                result.Status.Should().Be(EmitStatus.Succeeded);

                if (result.Status == EmitStatus.Succeeded && exampleExists)
                {
                    stream.Position = 0;
                    var generated = new StreamReader(stream).ReadToEnd();

                    var actual = JToken.Parse(generated);
                    File.WriteAllText(example.JsonFilename + ".actual", generated);

                    actual.Should().EqualWithJsonDiffOutput(
                        JToken.Parse(File.ReadAllText(example.JsonFilename)),
                        Path.Combine("docs", example.JsonFilename),
                        Path.Combine(Directory.GetCurrentDirectory(), example.JsonFilename + ".actual"));
                }
            }
        }
    }
}