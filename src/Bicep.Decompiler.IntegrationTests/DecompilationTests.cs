// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.IO;
using System.Reflection;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.FileSystem;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Az;
using FluentAssertions.Execution;
using Bicep.Core.UnitTests.FileSystem;
using System.Text.RegularExpressions;
using Bicep.Decompiler.Exceptions;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DecompilationTests
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

            public static string GetDisplayName(MethodInfo info, object[] data) => ((ExampleData)data[0]).JsonStreamName!;
        }

        private static IEnumerable<object[]> GetWorkingExampleData()
        {
            const string pathPrefix = "Working/";
            const string jsonExtension = ".json";

            foreach (var streamName in typeof(DecompilationTests).Assembly.GetManifestResourceNames().Where(p => p.StartsWith(pathPrefix, StringComparison.Ordinal)))
            {
                var extension = Path.GetExtension(streamName);
                if (!StringComparer.OrdinalIgnoreCase.Equals(extension, jsonExtension))
                {
                    continue;
                }

                var outputFolderName = streamName
                    .Substring(0, streamName.Length - jsonExtension.Length)
                    .Substring(pathPrefix.Length)
                    .Replace('/', '_');

                var exampleData = new ExampleData(streamName, Path.ChangeExtension(streamName, "json"), outputFolderName);

                yield return new object[] { exampleData };
            }
        }

        [TestMethod]
        public void ExampleData_should_return_a_number_of_records()
        {
            GetWorkingExampleData().Should().HaveCountGreaterOrEqualTo(9, "sanity check to ensure we're finding examples to test");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetWorkingExampleData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(ExampleData), DynamicDataDisplayName = nameof(ExampleData.GetDisplayName))]
        public void Decompiler_generates_expected_bicep_files_with_diagnostics(ExampleData example)
        {
            // save all the files in the containing directory to disk so that we can test module resolution
            var parentStream = Path.GetDirectoryName(example.BicepStreamName)!.Replace('\\', '/');
            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(DecompilationTests).Assembly, example.OutputFolderName, parentStream);
            var bicepFileName = Path.Combine(outputDirectory, Path.GetFileName(example.BicepStreamName));
            var jsonFileName = Path.Combine(outputDirectory, Path.GetFileName(example.JsonStreamName));

            var (bicepUri, filesToSave) = Decompiler.Decompiler.DecompileFileWithModules(new FileResolver(), PathHelper.FilePathToFileUrl(jsonFileName));

            var syntaxTrees = filesToSave.Select(kvp => SyntaxTree.Create(kvp.Key, kvp.Value));
            var workspace = new Workspace();
            workspace.UpsertSyntaxTrees(syntaxTrees);

            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), workspace, bicepUri);
            var compilation = new Compilation(new AzResourceTypeProvider(), syntaxTreeGrouping);
            var diagnosticsBySyntaxTree = compilation.GetAllDiagnosticsBySyntaxTree();

            using (new AssertionScope())
            {
                foreach (var syntaxTree in syntaxTreeGrouping.SyntaxTrees)
                {
                    var exampleExists = File.Exists(syntaxTree.FileUri.LocalPath);
                    exampleExists.Should().BeTrue($"Generated example \"{syntaxTree.FileUri.LocalPath}\" should be checked in");

                    var diagnostics = diagnosticsBySyntaxTree[syntaxTree];
                    var bicepOutput = filesToSave[syntaxTree.FileUri];
                    
                    var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(bicepOutput, Environment.NewLine, diagnostics, diag => OutputHelper.GetDiagLoggingString(bicepOutput, outputDirectory, diag));
                    File.WriteAllText(syntaxTree.FileUri.LocalPath + ".actual", sourceTextWithDiags);

                    sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                        TestContext, 
                        exampleExists ? File.ReadAllText(syntaxTree.FileUri.LocalPath) : "",
                        expectedLocation: Path.Combine("src", "Bicep.Decompiler.IntegrationTests", parentStream, Path.GetRelativePath(outputDirectory, syntaxTree.FileUri.LocalPath)),
                        actualLocation: syntaxTree.FileUri.LocalPath + ".actual");
                }
            }
        }

        private static IFileResolver ReadResourceFile(string resourcePath)
        {
            var manifestStream = typeof(DecompilationTests).Assembly.GetManifestResourceStream(resourcePath)!;
            var jsonContents = new StreamReader(manifestStream).ReadToEnd();

            var fileDict = new Dictionary<Uri, string>
            {
                [new Uri($"file:///{resourcePath}")] = jsonContents,
            };

            return new InMemoryFileResolver(fileDict);
        }

        [TestMethod]
        public void Decompiler_raises_errors_for_unsupported_features()
        {
            var resourcePath = "";
            Action onDecompile = () => {
                var fileResolver = ReadResourceFile(resourcePath);
                Decompiler.Decompiler.DecompileFileWithModules(fileResolver, new Uri($"file:///{resourcePath}"));
            };

            using (new AssertionScope())
            {
                resourcePath = "NonWorking/conditional.json";
                onDecompile.Should().Throw<ConversionFailedException>().WithMessage("[75:17]: The 'condition' property is not supported");

                resourcePath = "NonWorking/copyloop.json";
                onDecompile.Should().Throw<ConversionFailedException>().WithMessage("[11:9]: The 'copy' property is not supported");

                resourcePath = "NonWorking/unknownprops.json";
                onDecompile.Should().Throw<ConversionFailedException>().WithMessage("[15:29]: Unrecognized top-level resource property 'madeUpProperty'");
            }
        }

        [DataTestMethod]
        [DataRow("\r\n", "\\r\\n")]
        [DataRow("\n", "\\n")]
        public void Decompiler_handles_strings_with_newlines(string newline, string escapedNewline)
        {
            var template = @"{
    ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
    ""contentVersion"": ""1.0.0.0"",
    ""parameters"": {},
    ""variables"": {
        ""multilineString"": ""multi
        line
        string""
    },
    ""resources"": [],
    ""outputs"": {}
}";

            // replace newlines with the style passed in
            template = string.Join(newline, Regex.Split(template, "\r?\n"));

            var fileUri = new Uri("file:///path/to/main.json");
            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [fileUri] = template,
            });;

            var (entryPointUri, filesToSave) = Decompiler.Decompiler.DecompileFileWithModules(fileResolver, fileUri);

            // this behavior is actaully controlled by newtonsoft's deserializer, but we should assert it anyway to avoid regressions.
            filesToSave[entryPointUri].Should().Contain($"var multilineString = 'multi{escapedNewline}        line{escapedNewline}        string'");
        }
    }
}