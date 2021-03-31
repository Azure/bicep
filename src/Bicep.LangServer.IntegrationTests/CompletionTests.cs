// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Completions;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class CompletionTests
    {
        public static readonly AzResourceTypeProvider TypeProvider = new AzResourceTypeProvider();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task EmptyFileShouldProduceDeclarationCompletions()
        {
            const string expectedSetName = "declarations";
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(string.Empty, uri);

            var actual = await GetActualCompletions(client, uri, new Position(0, 0));
            var actualLocation = FileHelper.SaveResultFile(this.TestContext, $"{this.TestContext.TestName}_{expectedSetName}", actual.ToString(Formatting.Indented));

            var expectedStr = DataSets.Completions.TryGetValue(GetFullSetName(expectedSetName));
            if (expectedStr == null)
            {
                throw new AssertFailedException($"The completion set '{expectedSetName}' does not exist.");
            }

            var expected = JToken.Parse(expectedStr);

            actual.Should().EqualWithJsonDiffOutput(TestContext, expected, GetGlobalCompletionSetPath(expectedSetName), actualLocation);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSnippetCompletionData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(CompletionData), DynamicDataDisplayName = nameof(CompletionData.GetDisplayName))]
        public async Task ValidateSnippetCompletionAfterPlaceholderReplacements(CompletionData completionData)
        {
            string pathPrefix = "Completions/SnippetTemplates/";
            string bicepManifestResourceStreamName = pathPrefix + completionData.Prefix + "/main.bicep";
            string jsonManifestResourceStreamName = pathPrefix + completionData.Prefix + "/diagnostics.json";

            // Save all the files in the containing directory to disk
            SaveFilesToDisk(bicepManifestResourceStreamName, jsonManifestResourceStreamName, out string bicepFile, out string diagnosticsFile);

            // Verify snippet placeholder and expected diagnostics files exist
            VerifyPlaceholderAndDiagnosticsInformationFilesExist(completionData.Prefix, bicepFile, diagnosticsFile);

            // Start language server, copy snippet text, replace placeholders and return diagnostics information
            Container<Diagnostic> diagnostics = await StartServerAndGetDiagnosticsAsync(bicepManifestResourceStreamName, completionData.SnippetText);

            Stream? jsonStream = typeof(CompletionTests).Assembly.GetManifestResourceStream(jsonManifestResourceStreamName);
            StreamReader streamReader = new StreamReader(jsonStream ?? throw new ArgumentNullException("Stream is null"), Encoding.Default);
            string expected = await streamReader.ReadToEndAsync();

            var actual = JToken.FromObject(diagnostics);

            var actualLocation = FileHelper.SaveResultFile(this.TestContext, $"{completionData.Prefix}_Actual.json", actual.ToString(Formatting.Indented));
            var expectedLocation = Path.Combine("src", "Bicep.LangServer.Integrationtests", "Completions", "SnippetTemplates", completionData.Prefix, "main.json");

            actual.Should().EqualWithJsonDiffOutput(TestContext, JToken.Parse(expected), expectedLocation, actualLocation, "because ");
        }

        private void SaveFilesToDisk(string bicepManifestResourceStreamName,
                                     string jsonManifestResourceStreamName,
                                     out string bicepFile,
                                     out string diagnosticsFile)
        {
            var parentStream = GetParentStreamName(bicepManifestResourceStreamName);
            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(CompletionTests).Assembly, parentStream);

            bicepFile = Path.Combine(outputDirectory, Path.GetFileName(bicepManifestResourceStreamName));
            diagnosticsFile = Path.Combine(outputDirectory, Path.GetFileName(jsonManifestResourceStreamName));
        }

        private static string GetParentStreamName(string streamName) => Path.GetDirectoryName(streamName)!.Replace('\\', '/');

        private async Task<Container<Diagnostic>> StartServerAndGetDiagnosticsAsync(string bicepFileName, string snippetText)
        {
            Dictionary<Uri, string> fileSystemDict = new Dictionary<Uri, string>();
            MultipleMessageListener<PublishDiagnosticsParams> diagnosticsListener = new MultipleMessageListener<PublishDiagnosticsParams>();

            ILanguageClient client = await IntegrationTestHelper.StartServerWithClientConnectionAsync(
                options =>
                {
                    options.OnPublishDiagnostics(diags => diagnosticsListener.AddMessage(diags));
                },
                fileResolver: new InMemoryFileResolver(fileSystemDict));

            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepFileName);
            Stream? bicepStream = typeof(CompletionTests).Assembly.GetManifestResourceStream(bicepFileName);
            StreamReader streamReader = new StreamReader(bicepStream ?? throw new ArgumentNullException("Stream is null"), Encoding.Default);

            string bicepFileWithPlaceholderReplacements = await streamReader.ReadToEndAsync();

            string snippetTextAfterReplacements = SnippetCompletionTestHelper.GetSnippetTextAfterPlaceholderReplacements(snippetText, bicepFileWithPlaceholderReplacements);
            fileSystemDict[documentUri.ToUri()] = bicepFileWithPlaceholderReplacements.Replace("// Insert snippet here", snippetTextAfterReplacements);

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, fileSystemDict[documentUri.ToUri()], 1));

            var diagsParams = await diagnosticsListener.WaitNext();
            diagsParams.Uri.Should().Be(documentUri);

            return diagsParams.Diagnostics;
        }

        private void VerifyPlaceholderAndDiagnosticsInformationFilesExist(string prefix, string bicepFileName, string jsonFileName)
        {
            // Group assertion failures using AssertionScope, rather than reporting the first failure
            using (new AssertionScope())
            {
                bool snippetPlaceholderFileExists = File.Exists(bicepFileName);
                snippetPlaceholderFileExists.Should().BeTrue($"Snippet placeholder file for snippet with label- \"{prefix}\" should be checked in");

                bool diagnosticsFileExists = File.Exists(jsonFileName);
                diagnosticsFileExists.Should().BeTrue($"Diagnostics information file- diagnostics.json for snippet with label- \"{prefix}\" should be checked in");
            }
        }

        private static IEnumerable<object[]> GetSnippetCompletionData()
        {
            Assembly languageServerAssembly = Assembly.Load("Bicep.LangServer");
            IEnumerable<string> manifestResourceNames = languageServerAssembly.GetManifestResourceNames()
                .Where(p => p.EndsWith(".bicep", StringComparison.Ordinal));

            foreach (var manifestResourceName in manifestResourceNames)
            {
                Stream? stream = languageServerAssembly.GetManifestResourceStream(manifestResourceName);
                StreamReader streamReader = new StreamReader(stream!);

                string prefix = Path.GetFileNameWithoutExtension(manifestResourceName);
                CompletionData completionData = new CompletionData(prefix, streamReader.ReadToEnd());

                yield return new object[] { completionData };
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        public async Task CompletionRequestShouldProduceExpectedCompletions(DataSet dataSet, string setName, IList<Position> positions)
        {
            // ensure all files are present locally
            string basePath = dataSet.SaveFilesToTestDirectory(this.TestContext);

            var entryPoint = Path.Combine(basePath, "main.bicep");

            var uri = DocumentUri.FromFileSystemPath(entryPoint);

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(dataSet.Bicep, uri, resourceTypeProvider: TypeProvider, fileResolver: new FileResolver());

            var intermediate = new List<(Position position, JToken actual)>();

            foreach (var position in positions)
            {
                var actual = await GetActualCompletions(client, uri, position);

                intermediate.Add((position, actual));
            }

            ValidateCompletions(dataSet, setName, intermediate);
        }

        [TestMethod]
        public async Task String_segments_do_not_return_completions()
        {
            var fileWithCursors = @"
var completeString = |'he|llo'|
var interpolatedString = |'abc${|true}|de|f${|false}|gh|i'|
var multilineString = |'''|
hel|lo
'''|
";
            var bicepFile = fileWithCursors.Replace("|", "");
            var syntaxTree = SyntaxTree.Create(new Uri("file:///main.bicep"), bicepFile);

            var cursors = new List<int>();
            for (var i = 0; i < fileWithCursors.Length; i++)
            {
                if (fileWithCursors[i] == '|')
                {
                    cursors.Add(i - cursors.Count);
                }
            }

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(bicepFile, syntaxTree.FileUri, resourceTypeProvider: TypeProvider);

            foreach (var cursor in cursors)
            {
                using var assertionScope = new AssertionScope();
                assertionScope.AddReportable(
                    "completion context",
                    PrintHelper.PrintWithAnnotations(syntaxTree, new[] {
                        new PrintHelper.Annotation(new TextSpan(cursor, 0), "cursor position"),
                    }, 1, true));

                var completions = await client.RequestCompletion(new CompletionParams
                {
                    TextDocument = new TextDocumentIdentifier(syntaxTree.FileUri),
                    Position = TextCoordinateConverter.GetPosition(syntaxTree.LineStarts, cursor),
                });

                completions.Should().BeEmpty();
            }
        }

        private void ValidateCompletions(DataSet dataSet, string setName, List<(Position position, JToken actual)> intermediate)
        {
            // if the test author asserts on the wrong completion set in their tests
            // it will not be possible to update the expected completions in a way that makes the test pass
            // to save investigation time, we will produce an error message when this condition is detected
            var groupsByActual = intermediate.GroupBy(g => g.actual.ToString(Formatting.None)).ToList();

            if (groupsByActual.Count != 1)
            {
                var buffer = new StringBuilder();
                buffer.AppendLine($"The following groups completion trigger positions asserted the same expected completion set '{setName}' but produced different actual completions:");

                foreach (var group in groupsByActual)
                {
                    buffer.Append("- ");
                    buffer.Append(group.Select(tuple => FormatPosition(tuple.position)).ConcatString(", "));
                    buffer.AppendLine();
                }

                // we encountered multiple different completions sets
                throw new AssertFailedException(buffer.ToString());
            }

            var single = groupsByActual.Single();

            var expectedInfo = ResolveExpectedCompletions(dataSet, setName);
            if (expectedInfo.HasValue == false)
            {
                throw new AssertFailedException($"The completion set with the name '{setName}' does not exist. The following completion trigger positions assert this set: {single.Select(item => FormatPosition(item.position)).ConcatString(", ")}");
            }

            var actual = JToken.Parse(single.Key);
            var actualLocation = FileHelper.SaveResultFile(this.TestContext, $"{dataSet.Name}_{setName}_Actual.json", actual.ToString(Formatting.Indented));

            var expected = expectedInfo.Value.content;
            var expectedLocation = expectedInfo.Value.scope switch
            {
                ExpectedCompletionsScope.DataSet => Path.Combine("src", "Bicep.Core.Samples", "Files", dataSet.Name, DataSet.TestCompletionsDirectory, GetFullSetName(setName)),
                _ => GetGlobalCompletionSetPath(setName)
            };

            actual.Should().EqualWithJsonDiffOutput(TestContext, expected, expectedLocation, actualLocation, "because ");
        }

        private static string GetGlobalCompletionSetPath(string setName) => Path.Combine("src", "Bicep.Core.Samples", "Files", DataSet.TestCompletionsDirectory, GetFullSetName(setName));

        public static string GetDisplayName(MethodInfo info, object[] row)
        {
            row.Should().HaveCount(3);
            row[0].Should().BeOfType<DataSet>();
            row[1].Should().BeOfType<string>();
            row[2].Should().BeAssignableTo<IList<Position>>();

            return $"{info.Name}_{((DataSet)row[0]).Name}_{row[1]}";
        }

        private static string FormatPosition(Position position) => $"({position.Line}, {position.Character})";

        private static async Task<JToken> GetActualCompletions(ILanguageClient client, DocumentUri uri, Position position)
        {
            // local function
            string NormalizeLineEndings(string value) => value.Replace("\r", string.Empty);

            var completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = new TextDocumentIdentifier(uri),
                Position = position
            });

            // normalize line endings so assertions match on all OSs
            foreach (var completion in completions)
            {
                if (completion.InsertText != null)
                {
                    completion.InsertText = NormalizeLineEndings(completion.InsertText);
                }

                if (completion.Detail != null)
                {
                    completion.Detail = NormalizeLineEndings(completion.Detail);
                }

                if (completion.Documentation?.MarkupContent?.Value != null)
                {
                    completion.Documentation.MarkupContent.Value = NormalizeLineEndings(completion.Documentation.MarkupContent.Value);
                }

                if (completion.TextEdit != null)
                {
                    completion.TextEdit.NewText = NormalizeLineEndings(completion.TextEdit.NewText);

                    // ranges in these text edits will vary by completion trigger position
                    // if we try to assert on these, we will have an explosion of assert files
                    // let's ignore it for now until we come up with a better solution
                    completion.TextEdit.Range = new Range();
                }

                // can't do != null because the container overloads the != operator 😠
                if (!(completion.AdditionalTextEdits is null))
                {
                    foreach (var additionalEdit in completion.AdditionalTextEdits)
                    {
                        additionalEdit.Range = new Range();
                    }
                }
            }

            // OmniSharp sometimes will add a $$__handler_id__$$ property to the Data dictionary of a completion
            // (likely is needed to somehow help with routing of the ResolveCompletion requests)
            // the value is different every time you run the language server
            // to make our test asserts work, we will remove it and set the Data property null if nothing else remains in the object
            foreach (var completion in completions)
            {
                const string omnisharpHandlerIdPropertyName = "$$__handler_id__$$";

                // LSP protocol dictates that the Data property is of "any" type, so we need to check
                if (completion.Data is JObject @object && @object.Property(omnisharpHandlerIdPropertyName) != null)
                {
                    @object.Remove(omnisharpHandlerIdPropertyName);

                    if (!@object.HasValues)
                    {
                        completion.Data = null;
                    }
                }
            }

            //var serialized = JToken.FromObject(completions.OrderBy(c => c.Label, StringComparer.Ordinal));

            return JToken.FromObject(completions.OrderBy(c => c.Label, StringComparer.Ordinal), DataSetSerialization.CreateSerializer());
        }

        private static (JToken content, ExpectedCompletionsScope scope)? ResolveExpectedCompletions(DataSet dataSet, string setName)
        {
            var fullSetName = GetFullSetName(setName);
            var str = dataSet.Completions.TryGetValue(fullSetName);
            if (str != null)
            {
                return (JToken.Parse(str), ExpectedCompletionsScope.DataSet);
            }

            str = DataSets.Completions.TryGetValue(fullSetName);
            if (str != null)
            {
                return (JToken.Parse(str), ExpectedCompletionsScope.Global);
            }

            return null;
        }

        private static string GetFullSetName(string setName) => setName + ".json";

        private static IEnumerable<object[]> GetData() =>
            DataSets.NonStressDataSets
                .Select(ds => (dataset: ds, triggers: CompletionTestDirectiveParser.GetTriggers(ds.Bicep)))
                .Where(tuple => tuple.triggers.Any())
                .SelectMany(tuple => tuple.triggers.Select(trigger => (tuple.dataset, trigger.Position, trigger.SetName)))
                .GroupBy(tuple => (tuple.dataset, tuple.SetName), tuple => tuple.Position)
                .Select(group => new object[] { group.Key.dataset, group.Key.SetName, group.ToList() });

        private enum ExpectedCompletionsScope
        {
            Global,
            DataSet
        }

        public class CompletionData
        {
            public CompletionData(string prefix, string snippetText)
            {
                Prefix = prefix;
                SnippetText = snippetText;
            }

            public string Prefix { get; }

            public string SnippetText { get; }

            public static string GetDisplayName(MethodInfo methodInfo, object[] data) => ((CompletionData)data[0]).Prefix!;
        }
    }
}
