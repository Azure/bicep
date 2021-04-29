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
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Completions;
using Bicep.LanguageServer.Extensions;
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
        public static readonly IResourceTypeProvider TypeProvider = AzResourceTypeProvider.CreateWithAzTypes();

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
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ValidateSnippetCompletionAfterPlaceholderReplacements(CompletionData completionData)
        {
            string pathPrefix = $"Completions/SnippetTemplates/{completionData.Prefix}";

            var outputDirectory = FileHelper.SaveEmbeddedResourcesWithPathPrefix(TestContext, typeof(CompletionTests).Assembly, pathPrefix);

            var bicepFileName = Path.Combine(outputDirectory, "main.bicep");
            var bicepSourceFileName = Path.Combine("src", "Bicep.LangServer.IntegrationTests", pathPrefix, Path.GetRelativePath(outputDirectory, bicepFileName));
            File.Exists(bicepFileName).Should().BeTrue($"Snippet placeholder file \"{bicepSourceFileName}\" should be checked in");
            var bicepContents = await File.ReadAllTextAsync(bicepFileName);

            // Request the expected completion from the server, and ensure it is unique + valid
            var completionText = await RequestSnippetCompletion(bicepFileName, completionData, bicepContents);

            // Replace all the placeholders with values from the placeholder file
            var replacementContents = SnippetCompletionTestHelper.GetSnippetTextAfterPlaceholderReplacements(completionText, bicepContents);

            using (new AssertionScope())
            {
                var combinedFileName = Path.Combine(outputDirectory, "main.combined.bicep");
                var combinedSourceFileName = Path.Combine("src", "Bicep.LangServer.IntegrationTests", pathPrefix, Path.GetRelativePath(outputDirectory, combinedFileName));
                File.Exists(combinedFileName).Should().BeTrue($"Combined snippet file \"{combinedSourceFileName}\" should be checked in");

                var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(new FileResolver(), new Workspace(), PathHelper.FilePathToFileUrl(combinedFileName));
                var compilation = new Compilation(TypeProvider, syntaxTreeGrouping);
                var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

                var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(replacementContents, Environment.NewLine, diagnostics, diag => OutputHelper.GetDiagLoggingString(replacementContents, outputDirectory, diag));
                File.WriteAllText(combinedFileName + ".actual", sourceTextWithDiags);

                sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                    TestContext,
                    File.Exists(combinedFileName) ? (await File.ReadAllTextAsync(combinedFileName)) : string.Empty,
                    expectedLocation: combinedSourceFileName,
                    actualLocation: combinedFileName + ".actual");
            }
        }

        private async Task<string> RequestSnippetCompletion(string bicepFileName, CompletionData completionData, string placeholderFile)
        {
            var documentUri = DocumentUri.FromFileSystemPath(bicepFileName);
            var syntaxTree = SyntaxTree.Create(documentUri.ToUri(), placeholderFile);

            var client = await IntegrationTestHelper.StartServerWithTextAsync(
                placeholderFile,
                documentUri,
                null,
                TypeProvider);

            var cursor = placeholderFile.IndexOf("// Insert snippet here");
            var completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = documentUri,
                Position = TextCoordinateConverter.GetPosition(syntaxTree.LineStarts, cursor),
            });

            var matchingSnippets = completions.Where(x => x.Kind == CompletionItemKind.Snippet && x.Label == completionData.Prefix);

            matchingSnippets.Should().HaveCount(1);
            var completion = matchingSnippets.First();

            completion.TextEdit.Should().NotBeNull();
            completion.TextEdit!.Range.Should().Be(new TextSpan(cursor, 0).ToRange(syntaxTree.LineStarts));
            completion.TextEdit.NewText.Should().NotBeNullOrWhiteSpace();

            return completion.TextEdit.NewText;
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
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var completeString = |'he|llo'|
var interpolatedString = |'abc${|true}|de|f${|false}|gh|i'|
var multilineString = |'''|
hel|lo
'''|
");

            var syntaxTree = SyntaxTree.Create(new Uri("file:///main.bicep"), file);
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(file, syntaxTree.FileUri, resourceTypeProvider: TypeProvider);

            foreach (var cursor in cursors)
            {
                using (new AssertionScope().WithVisualCursor(syntaxTree, new TextSpan(cursor, 0)))
                {
                    var completions = await client.RequestCompletion(new CompletionParams
                    {
                        TextDocument = new TextDocumentIdentifier(syntaxTree.FileUri),
                        Position = TextCoordinateConverter.GetPosition(syntaxTree.LineStarts, cursor),
                    });

                    completions.Should().BeEmpty();
                }
            }
        }

        [TestMethod]
        public async Task Completions_are_offered_immediately_before_and_after_comments()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var test = |// comment here
var test2 = |/* block comment */|
");

            var syntaxTree = SyntaxTree.Create(new Uri("file:///main.bicep"), file);
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(file, syntaxTree.FileUri, resourceTypeProvider: TypeProvider);

            foreach (var cursor in cursors)
            {
                using (new AssertionScope().WithVisualCursor(syntaxTree, new TextSpan(cursor, 0)))
                {
                    var completions = await client.RequestCompletion(new CompletionParams
                    {
                        TextDocument = new TextDocumentIdentifier(syntaxTree.FileUri),
                        Position = TextCoordinateConverter.GetPosition(syntaxTree.LineStarts, cursor),
                    });

                    completions.Should().NotBeEmpty();
                }
            }
        }

        [TestMethod]
        public async Task Completions_are_not_offered_inside_comments()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
var test = /|/ comment here|
var test2 = /|* block c|omment *|/
");

            var syntaxTree = SyntaxTree.Create(new Uri("file:///main.bicep"), file);
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(file, syntaxTree.FileUri, resourceTypeProvider: TypeProvider);

            foreach (var cursor in cursors)
            {
                using (new AssertionScope().WithVisualCursor(syntaxTree, new TextSpan(cursor, 0)))
                {
                    var completions = await client.RequestCompletion(new CompletionParams
                    {
                        TextDocument = new TextDocumentIdentifier(syntaxTree.FileUri),
                        Position = TextCoordinateConverter.GetPosition(syntaxTree.LineStarts, cursor),
                    });

                    completions.Should().BeEmpty();
                }
            }
        }

        [TestMethod]
        public async Task Property_completions_include_descriptions()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
resource testRes 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'testRes'
  properties: {
    |
  }
}

output string test = testRes.|
output string test2 = testRes.properties.|
");

            var syntaxTree = SyntaxTree.Create(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(file, syntaxTree.FileUri, resourceTypeProvider: BuiltInTestTypes.Create());
            var completions = await RequestCompletions(client, syntaxTree, cursors);

            completions.Should().SatisfyRespectively(
                x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                    d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which supports reading AND writing!"),
                    d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which is required."),
                    d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which only supports writing.")),
                x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                    d => d.Documentation!.MarkupContent!.Value.Should().Contain("apiVersion property"),
                    d => d.Documentation!.MarkupContent!.Value.Should().Contain("id property"),
                    d => d.Documentation!.MarkupContent!.Value.Should().Contain("name property"),
                    d => d.Documentation!.MarkupContent!.Value.Should().Contain("properties property"),
                    d => d.Documentation!.MarkupContent!.Value.Should().Contain("type property")),
                x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                    d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which only supports reading."),
                    d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which supports reading AND writing!"),
                    d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which is required.")));
        }

        [TestMethod]
        public async Task Completions_after_resource_type_should_only_include_existing_keyword()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
resource testRes 'Test.Rp/readWriteTests@2020-01-01' |

resource testRes2 'Test.Rp/readWriteTests@2020-01-01' | = {
}

resource testRes3 'Test.Rp/readWriteTests@2020-01-01' e| = {
}

resource testRes4 'Test.Rp/readWriteTests@2020-01-01' e|= {
}

resource testRes5 'Test.Rp/readWriteTests@2020-01-01' |= {
}
");

            static void AssertExistingKeywordCompletion(CompletionItem item)
            {
                item.Label.Should().Be("existing");
                item.Detail.Should().Be("existing");
                item.Documentation.Should().BeNull();
                item.Kind.Should().Be(CompletionItemKind.Keyword);
                item.Preselect.Should().BeFalse();
                item.TextEdit!.NewText.Should().Be("existing");

                // do not add = to the list of commit chars
                // it makes it difficult to type = without the "existing" keyword :)
                item.CommitCharacters.Should().BeNull();
            }

            var syntaxTree = SyntaxTree.Create(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(file, syntaxTree.FileUri, resourceTypeProvider: BuiltInTestTypes.Create());
            var completions = await RequestCompletions(client, syntaxTree, cursors);

            completions.Should().SatisfyRespectively(
                x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(d => AssertExistingKeywordCompletion(d)),
                x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(d => AssertExistingKeywordCompletion(d)),
                x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(d => AssertExistingKeywordCompletion(d)),
                x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(d => AssertExistingKeywordCompletion(d)),
                x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(d => AssertExistingKeywordCompletion(d)));
        }

        [TestMethod]
        public async Task PropertyNameCompletionsShouldIncludeTrailingColonIfColonIsMissing()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
resource testRes 'Test.Rp/readWriteTests@2020-01-01' = {
  |
}

resource testRes2 'Test.Rp/readWriteTests@2020-01-01' = {
  n|
}
");
            static void AssertPropertyNameCompletionsWithColons(CompletionList list)
            {
                list.Where(i => i.Kind == CompletionItemKind.Property)
                    .Should()
                    .OnlyContain(x => string.Equals(x.TextEdit!.NewText, x.Label + ':'));
            }

            var syntaxTree = SyntaxTree.Create(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(file, syntaxTree.FileUri, resourceTypeProvider: BuiltInTestTypes.Create());
            var completions = await RequestCompletions(client, syntaxTree, cursors);
            completions.Should().SatisfyRespectively(
                l => AssertPropertyNameCompletionsWithColons(l!),
                l => AssertPropertyNameCompletionsWithColons(l!));
        }

        [TestMethod]
        public async Task PropertyNameCompletionsShouldNotIncludeTrailingColonIfItIsPresent()
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
resource testRes2 'Test.Rp/readWriteTests@2020-01-01' = {
  n|:
}

resource testRes3 'Test.Rp/readWriteTests@2020-01-01' = {
  n| :
}
");
            static void AssertPropertyNameCompletionsWithColons(CompletionList list)
            {
                list.Where(i => i.Kind == CompletionItemKind.Property)
                    .Should()
                    .OnlyContain(x => string.Equals(x.TextEdit!.NewText, x.Label));
            }

            var syntaxTree = SyntaxTree.Create(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(file, syntaxTree.FileUri, resourceTypeProvider: BuiltInTestTypes.Create());
            var completions = await RequestCompletions(client, syntaxTree, cursors);
            completions.Should().SatisfyRespectively(
                l => AssertPropertyNameCompletionsWithColons(l!),
                l => AssertPropertyNameCompletionsWithColons(l!));
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

        private static async Task<IEnumerable<CompletionList?>> RequestCompletions(ILanguageClient client, SyntaxTree syntaxTree, IEnumerable<int> cursors)
        {
            var completions = new List<CompletionList?>();
            foreach (var cursor in cursors)
            {
                var completionList = await client.RequestCompletion(new CompletionParams
                {
                    TextDocument = new TextDocumentIdentifier(syntaxTree.FileUri),
                    Position = TextCoordinateConverter.GetPosition(syntaxTree.LineStarts, cursor),
                });

                completions.Add(completionList);
            }

            return completions;
        }
    }
}
