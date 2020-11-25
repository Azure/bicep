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
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
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
        [NotNull]
        public TestContext? TestContext { get; set; }

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
                "Resource type \"microsoft.network/networkSecurityGroups@2020-08-01\" does not have types available.",
                "Resource type \"Microsoft.Web/sites/siteextensions@2020-06-01\" does not have types available.",
                "Resource type \"Microsoft.DesktopVirtualization/hostpools/providers/diagnosticSettings@2017-05-01-preview\" does not have types available."
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
        public void ExampleIsValid(ExampleData example)
        {
            // save all the files in the containing directory to disk so that we can test module resolution
            var parentStream = Path.GetDirectoryName(example.BicepStreamName)!.Replace('\\', '/');
            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(ExamplesTests).Assembly, example.OutputFolderName, parentStream);
            var bicepFileName = Path.Combine(outputDirectory, Path.GetFileName(example.BicepStreamName));
            var jsonFileName = Path.Combine(outputDirectory, Path.GetFileName(example.JsonStreamName));
            
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(bicepFileName));
            var compilation = new Compilation(new AzResourceTypeProvider(), syntaxTreeGrouping);
            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

            // group assertion failures using AssertionScope, rather than reporting the first failure
            using (new AssertionScope())
            {
                foreach (var (syntaxTree, diagnostics) in compilation.GetAllDiagnosticsBySyntaxTree())
                {
                    var nonPermittedDiagnostics = diagnostics.Where(x => !IsPermittedMissingTypeDiagnostic(x));

                    nonPermittedDiagnostics.Should().BeEmpty($"\"{syntaxTree.FileUri.LocalPath}\" should not have warnings or errors");
                }

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
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetExampleData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExampleData), DynamicDataDisplayName = nameof(ExampleData.GetDisplayName))]
        public void Example_uses_consistent_formatting(ExampleData example)
        {
            // save all the files in the containing directory to disk so that we can test module resolution
            var parentStream = Path.GetDirectoryName(example.BicepStreamName)!.Replace('\\', '/');
            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(ExamplesTests).Assembly, example.OutputFolderName, parentStream);

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
    }
}
