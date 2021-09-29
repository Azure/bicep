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
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
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
        public static readonly INamespaceProvider NamespaceProvider = BicepTestConstants.NamespaceProvider;

        [NotNull]
        public TestContext? TestContext { get; set; }

        public static string GetDisplayName(MethodInfo info, object[] row)
        {
            row.Should().HaveCount(3);
            row[0].Should().BeOfType<DataSet>();
            row[1].Should().BeOfType<string>();
            row[2].Should().BeAssignableTo<IList<Position>>();

            return $"{info.Name}_{((DataSet)row[0]).Name}_{row[1]}";
        }

        [TestMethod]
        public async Task EmptyFileShouldProduceDeclarationCompletions()
        {
            const string expectedSetName = "declarations";
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, string.Empty, uri);

            var actual = await GetActualCompletions(client, uri, new Position(0, 0));
            var actualLocation = FileHelper.SaveResultFile(this.TestContext, $"{this.TestContext.TestName}_{expectedSetName}", actual.ToString(Formatting.Indented));

            var expectedStr = DataSets.Completions.TryGetValue(GetFullSetName(expectedSetName));
            if (expectedStr == null)
            {
                throw new AssertFailedException($"The completion set '{expectedSetName}' does not exist.");
            }

            var expected = JToken.Parse(expectedStr);

            actual.Should().EqualWithJsonDiffOutput(this.TestContext, expected, GetGlobalCompletionSetPath(expectedSetName), actualLocation);
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
            bicepContents = StringUtils.ReplaceNewlines(bicepContents, "\n");
            var cursor = bicepContents.IndexOf("// Insert snippet here");

            // Request the expected completion from the server, and ensure it is unique + valid
            var completionText = await RequestSnippetCompletion(bicepFileName, completionData, bicepContents, cursor);

            // Replace all the placeholders with values from the placeholder file
            var replacementContents = SnippetCompletionTestHelper.GetSnippetTextAfterPlaceholderReplacements(completionText, bicepContents);

            var bicepContentsReplaced = bicepContents.Substring(0, cursor) +
                replacementContents +
                bicepContents.Substring(cursor);

            using (new AssertionScope())
            {
                var combinedFileName = Path.Combine(outputDirectory, "main.combined.bicep");
                var combinedSourceFileName = Path.Combine("src", "Bicep.LangServer.IntegrationTests", pathPrefix, Path.GetRelativePath(outputDirectory, combinedFileName));
                File.Exists(combinedFileName).Should().BeTrue($"Combined snippet file \"{combinedSourceFileName}\" should be checked in");

                var combinedFileUri = PathHelper.FilePathToFileUrl(combinedFileName);
                var sourceFileGrouping = SourceFileGroupingFactory.CreateForFiles(new Dictionary<Uri, string>
                {
                    [combinedFileUri] = bicepContentsReplaced,
                }, combinedFileUri, BicepTestConstants.FileResolver);
                var compilation = new Compilation(NamespaceProvider, sourceFileGrouping, BicepTestConstants.BuiltInConfiguration);
                var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

                var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(bicepContentsReplaced, "\n", diagnostics, diag => OutputHelper.GetDiagLoggingString(bicepContentsReplaced, outputDirectory, diag));
                File.WriteAllText(combinedFileName + ".actual", sourceTextWithDiags);

                if (diagnostics.Any())
                {
                    Execute.Assertion.FailWith(
                        "Expected {0} file to not contain errors or warnings, but found {1}. Please fix errors/warnings mentioned in below section in {0} file:\n {2}",
                        "main.combined.bicep",
                        diagnostics.Count(),
                        sourceTextWithDiags);
                }

                sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                    TestContext,
                    File.Exists(combinedFileName) ? (await File.ReadAllTextAsync(combinedFileName)) : string.Empty,
                    expectedLocation: combinedSourceFileName,
                    actualLocation: combinedFileName + ".actual");
            }
        }

        private static IEnumerable<object[]> GetSnippetCompletionData() => CompletionDataHelper.GetSnippetCompletionData();

        private async Task<string> RequestSnippetCompletion(string bicepFileName, CompletionData completionData, string placeholderFile, int cursor)
        {
            var documentUri = DocumentUri.FromFileSystemPath(bicepFileName);
            var bicepFile = SourceFileFactory.CreateBicepFile(documentUri.ToUri(), placeholderFile);

            var client = await IntegrationTestHelper.StartServerWithTextAsync(
                this.TestContext,
                placeholderFile,
                documentUri,
                null,
                creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider));

            var completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = documentUri,
                Position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor),
            });

            var matchingSnippets = completions.Where(x => x.Kind == CompletionItemKind.Snippet && x.Label == completionData.Prefix);

            matchingSnippets.Should().HaveCount(1);
            var completion = matchingSnippets.First();

            completion.TextEdit.Should().NotBeNull();
            completion.TextEdit!.TextEdit!.Range.Should().Be(new TextSpan(cursor, 0).ToRange(bicepFile.LineStarts));
            completion.TextEdit.TextEdit.NewText.Should().NotBeNullOrWhiteSpace();

            return completion.TextEdit.TextEdit.NewText;
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task CompletionRequestShouldProduceExpectedCompletions(DataSet dataSet, string setName, IList<Position> positions)
        {
            // ensure all files are present locally
            string basePath = dataSet.SaveFilesToTestDirectory(this.TestContext);

            var entryPoint = Path.Combine(basePath, "main.bicep");

            var uri = DocumentUri.FromFileSystemPath(entryPoint);

            using var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, dataSet.Bicep, uri, creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider, FileResolver: BicepTestConstants.FileResolver));

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
var interpolatedString = |'abc|${true}|de|f|${false}|gh|i'|
var multilineString = |'''|
hel|lo
'''|
";

            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task Completions_are_offered_in_string_expressions()
        {
            var fileWithCursors = @"
var interpolatedString = 'abc${|true}def${|}ghi${res|}xyz'
";

            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsNonEmpty);
        }

        [TestMethod]
        public async Task Completions_are_offered_immediately_before_and_after_comments()
        {
            var fileWithCursors = @"
var test = |// comment here
var test2 = |/* block comment */|
";

            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsNonEmpty);
        }

        [TestMethod]
        public async Task Completions_are_not_offered_immediately_after_open_paren()
        {
            var fileWithCursors = @"
param foo = {|

var bar = {|

resource testRes 'Test.Rp/readWriteTests@2020-01-01' = {|

output baz object = {|
";

            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task Completions_are_not_offered_inside_comments()
        {
            var fileWithCursors = @"
var test = /|/ comment here|
var test2 = /|* block c|omment *|/
";

            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task VerifyResourceBodyCompletionWithExistingKeywordDoesNotIncludeCustomSnippet()
        {
            string text = "resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' existing = ";

            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///main.bicep"), text);
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, text, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider));

            var completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.FileUri),
                Position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, text.Length),
            });

            completions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("{}");
                },
                c =>
                {
                    c.Label.Should().Be("required-properties");
                },
                c =>
                {
                    c.Label.Should().Be("if");
                },
                c =>
                {
                    c.Label.Should().Be("for");
                },
                c =>
                {
                    c.Label.Should().Be("for-indexed");
                },
                c =>
                {
                    c.Label.Should().Be("for-filtered");
                });
        }

        [TestMethod]
        public async Task VerifyNestedResourceBodyCompletionReturnsSnippets()
        {
            string fileWithCursors = @"resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
  location: resourceGroup().location

  resource automationCredential 'credentials@2019-06-01' = |
}";
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(TestContext, file, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider));
            var completionLists = await RequestCompletions(client, bicepFile, cursors);

            completionLists.Count().Should().Be(1);

            var snippetCompletions = completionLists.First()!.Items.Where(x => x.Kind == CompletionItemKind.Snippet);

            snippetCompletions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("{}");
                },
                c =>
                {
                    c.Label.Should().Be("snippet");
                },
                c =>
                {
                    c.Label.Should().Be("required-properties");
                },
                c =>
                {
                    c.Label.Should().Be("if");
                },
                c =>
                {
                    c.Label.Should().Be("for");
                },
                c =>
                {
                    c.Label.Should().Be("for-indexed");
                },
                c =>
                {
                    c.Label.Should().Be("for-filtered");
                });
        }

        [TestMethod]
        public async Task VerifyNestedResourceBodyCompletionReturnsCustomSnippetWithoutParentInformation()
        {
            string fileWithCursors = @"resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
  location: resourceGroup().location

  resource automationCredential 'credentials@2019-06-01' = |
}";
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(TestContext, file, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider));
            var completionLists = await RequestCompletions(client, bicepFile, cursors);

            completionLists.Count().Should().Be(1);

            var snippetCompletion = completionLists.First()!.Items.Where(x => x.Kind == CompletionItemKind.Snippet && x.Label == "snippet");

            snippetCompletion.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("snippet");
                    c.Detail.Should().Be("Automation Credential");
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.TextEdit?.TextEdit?.NewText?.Should().BeEquivalentToIgnoringNewlines(@"{
  name: ${3:'name'}
  properties: {
    userName: ${4:'userName'}
    password: ${5:'password'}
    description: ${6:'description'}
  }
}");
                });
        }

        [TestMethod]
        public async Task VerifyNestedResourceCompletionReturnsCustomSnippetWithoutParentInformation()
        {
            string fileWithCursors = @"resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
  location: resourceGroup().location

  |
}";
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(TestContext, file, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider));
            var completionLists = await RequestCompletions(client, bicepFile, cursors);

            completionLists.Count().Should().Be(1);

            var snippetCompletion = completionLists.First()!.Items.Where(x => x.Kind == CompletionItemKind.Snippet && x.Label == "res-automation-cred");

            snippetCompletion.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("res-automation-cred");
                    c.Detail.Should().Be("Automation Credential");
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.TextEdit?.TextEdit?.NewText?.Should().BeEquivalentToIgnoringNewlines(@"resource ${2:automationCredential} 'credentials@2019-06-01' = {
  name: ${3:'name'}
  properties: {
    userName: ${4:'userName'}
    password: ${5:'password'}
    description: ${6:'description'}
  }
}");
                });
        }

        [TestMethod]
        public async Task VerifyResourceBodyCompletionWithoutExistingKeywordIncludesCustomSnippet()
        {
            string text = @"resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = ";

            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///main.bicep"), text);
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, text, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider));

            var completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.FileUri),
                Position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, text.Length),
            });

            completions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("{}");
                },
                c =>
                {
                    c.Label.Should().Be("snippet");
                },
                c =>
                {
                    c.Label.Should().Be("required-properties");
                },
                c =>
                {
                    c.Label.Should().Be("if");
                },
                c =>
                {
                    c.Label.Should().Be("for");
                },
                c =>
                {
                    c.Label.Should().Be("for-indexed");
                },
                c =>
                {
                    c.Label.Should().Be("for-filtered");
                });
        }

        [TestMethod]
        public async Task VerifyResourceBodyCompletionWithDiscriminatedObjectTypeContainsRequiredPropertiesSnippet()
        {
            string text = @"resource deploymentScripts 'Microsoft.Resources/deploymentScripts@2020-10-01'=";
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///main.bicep"), text);
            using var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, text, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider));

            var completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = new TextDocumentIdentifier(bicepFile.FileUri),
                Position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, text.Length),
            });

            completions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("{}");
                },
                c =>
                {
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.Label.Should().Be("required-properties-AzureCLI");
                    c.Detail.Should().Be("Required properties");
                    c.TextEdit?.TextEdit?.NewText?.Should().BeEquivalentToIgnoringNewlines(@"{
	name: $1
	location: $2
	kind: 'AzureCLI'
	properties: {
		azCliVersion: $3
		retentionInterval: $4
	}
}$0");
                },
                c =>
                {

                    c.Label.Should().Be("required-properties-AzurePowerShell");
                    c.Detail.Should().Be("Required properties");
                    c.TextEdit?.TextEdit?.NewText?.Should().BeEquivalentToIgnoringNewlines(@"{
	name: $1
	location: $2
	kind: 'AzurePowerShell'
	properties: {
		azPowerShellVersion: $3
		retentionInterval: $4
	}
}$0");
                },
                c =>
                {
                    c.Label.Should().Be("if");
                },
                c =>
                {
                    c.Label.Should().Be("for");
                },
                c =>
                {
                    c.Label.Should().Be("for-indexed");
                },
                c =>
                {
                    c.Label.Should().Be("for-filtered");
                });
        }

        [TestMethod]
        public async Task Property_completions_include_descriptions()
        {
            var fileWithCursors = @"
resource testRes 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'testRes'
  properties: {
    |
  }
}

output string test = testRes.|
output string test2 = testRes.properties.|
";

            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, completions =>
                completions.Should().SatisfyRespectively(
                    x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                        d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which is required."),
                        d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which supports reading AND writing!"),
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
                        d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which is required."))));
        }

        [TestMethod]
        public async Task Property_completions_required_first_and_selected()
        {
            var fileWithCursors = @"
resource testRes 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'testRes'
  properties: {
    |
  }
}

output string test2 = testRes.properties.|
";

            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, completions =>
                completions.Should().SatisfyRespectively(
                    x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                        d =>
                        {
                            d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which is required.");
                            d.Preselect.Should().BeTrue();
                        },
                        d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which supports reading AND writing!"),
                        d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which only supports writing.")),
                    x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                        d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which only supports reading."),
                        d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which supports reading AND writing!"),
                        d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which is required."))));
        }

        [TestMethod]
        public async Task Completions_after_resource_type_should_only_include_existing_keyword()
        {
            var fileWithCursors = @"
resource testRes2 'Test.Rp/readWriteTests@2020-01-01' | = {
}

resource testRes3 'Test.Rp/readWriteTests@2020-01-01' e| = {
}

resource testRes4 'Test.Rp/readWriteTests@2020-01-01' e|= {
}

resource testRes5 'Test.Rp/readWriteTests@2020-01-01' |= {
}
";

            static void AssertExistingKeywordCompletion(CompletionItem item)
            {
                item.Label.Should().Be("existing");
                item.Detail.Should().Be("existing");
                item.Documentation.Should().BeNull();
                item.Kind.Should().Be(CompletionItemKind.Keyword);
                item.Preselect.Should().BeFalse();
                item.TextEdit!.TextEdit!.NewText.Should().Be("existing");

                // do not add = to the list of commit chars
                // it makes it difficult to type = without the "existing" keyword :)
                item.CommitCharacters.Should().BeNull();
            }


            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, completions =>
                completions.Should().SatisfyRespectively(
                    x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(d => AssertExistingKeywordCompletion(d)),
                    x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(d => AssertExistingKeywordCompletion(d)),
                    x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(d => AssertExistingKeywordCompletion(d)),
                    x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(d => AssertExistingKeywordCompletion(d))));
        }

        [TestMethod]
        public async Task ResourceTypeFollowerWithoCompletionsOffersEqualsAndExisting()
        {

            var fileWithCursors = @"
resource base64 'Microsoft.Foo/foos@2020-09-01' |

resource base64 'Microsoft.Foo/foos@2020-09-01' | {}

resource base64 'Microsoft.Foo/foos@2020-09-01' existing | {}

";

            static void AssertEqualsOperatorCompletion(CompletionItem item)
            {
                item.Label.Should().Be("=");
                item.Documentation.Should().BeNull();
                item.Kind.Should().Be(CompletionItemKind.Operator);
                item.Preselect.Should().BeTrue();
                item.TextEdit!.TextEdit!.NewText.Should().Be("=");

                // do not add = to the list of commit chars
                // it makes it difficult to type = without the "existing" keyword :)
                item.CommitCharacters.Should().BeNull();
            }

            static void AssertExistingKeywordCompletion(CompletionItem item)
            {
                item.Label.Should().Be("existing");
                item.Detail.Should().Be("existing");
                item.Documentation.Should().BeNull();
                item.Kind.Should().Be(CompletionItemKind.Keyword);
                item.Preselect.Should().BeFalse();
                item.TextEdit!.TextEdit!.NewText.Should().Be("existing");

                // do not add = to the list of commit chars
                // it makes it difficult to type = without the "existing" keyword :)
                item.CommitCharacters.Should().BeNull();
            }

            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, completions =>
                completions.Should().SatisfyRespectively(
                    x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                        d => AssertEqualsOperatorCompletion(d),
                        d => AssertExistingKeywordCompletion(d)
                    ),
                    x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                        d => AssertEqualsOperatorCompletion(d),
                        d => AssertExistingKeywordCompletion(d)
                    ),
                    x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                        d => AssertEqualsOperatorCompletion(d)
                    )));
        }

        [TestMethod]
        public async Task PropertyNameCompletionsShouldIncludeTrailingColonIfColonIsMissing()
        {
            var fileWithCursors = @"
resource testRes 'Test.Rp/readWriteTests@2020-01-01' = {
  |
}

resource testRes2 'Test.Rp/readWriteTests@2020-01-01' = {
  n|
}
";
            static void AssertPropertyNameCompletionsWithColons(CompletionList list)
            {
                list.Where(i => i.Kind == CompletionItemKind.Property)
                    .Should()
                    .OnlyContain(x => string.Equals(x.TextEdit!.TextEdit!.NewText, x.Label + ':'));
            }

            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, completions =>
                completions.Should().SatisfyRespectively(
                    l => AssertPropertyNameCompletionsWithColons(l!),
                    l => AssertPropertyNameCompletionsWithColons(l!)));
        }

        [TestMethod]
        public async Task PropertyNameCompletionsShouldNotIncludeTrailingColonIfItIsPresent()
        {
            static void AssertPropertyNameCompletionsWithoutColons(CompletionList list)
            {
                list.Where(i => i.Kind == CompletionItemKind.Property)
                    .Should()
                    .OnlyContain(x => string.Equals(x.TextEdit!.TextEdit!.NewText, x.Label));
            }

            var fileWithCursors = @"
resource testRes2 'Test.Rp/readWriteTests@2020-01-01' = {
  n|:
}

resource testRes3 'Test.Rp/readWriteTests@2020-01-01' = {
  n| :
}
";

            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, completions =>
               completions.Should().SatisfyRespectively(
                   l => AssertPropertyNameCompletionsWithoutColons(l!),
                   l => AssertPropertyNameCompletionsWithoutColons(l!)));
        }

        [TestMethod]
        public async Task RequestCompletionsInResourceBodies_AtPositionsWhereNodeShouldNotBeInserted_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
resource myRes 'Test.Rp/readWriteTests@2020-01-01' = {|
 | name: 'myRes' |
  tags | : {
    a: 'A'   |
  }
|}
";
            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task RequestCompletionsInResourceBodies_AtPositionsWhereNestedResourceCanBeInserted_ReturnsNestedResourceCompletions()
        {
            var fileWithCursors = @"
resource myRes 'Test.Rp/readWriteTests@2020-01-01' = {
|
   |
  re|
   |res
}
";

            static void AssertAllCompletionsContainResourceLabel(IEnumerable<CompletionList?> completionLists)
            {
                foreach (var completionList in completionLists)
                {
                    completionList.Should().Contain(x => x.Label == "resource");
                }
            }

            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsContainResourceLabel);
        }

        [TestMethod]
        public async Task RequestCompletionsInProgram_AtPositionsWhereNodeShouldNotBeInserted_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
|  var obj = {} |

|  resource myRes 'Test.Rp/readWriteTests@2020-01-01' = {
  name: 'myRes'
}
";
            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task RequestCompletionsInObjects_AtPositionsWhereNodeShouldNotBeInserted_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
var obj1 = {|}
var obj2 = {| }
var obj3 = |{ |}
var obj4 = { | }|
var obj5 = {|
  | prop: | true |
|}
var obj6 = { |
  prop  | : false
 |  }
";
            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task RequestCompletionsInArrays_AtPositionsWhereNodeShouldNotBeInserted_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
var arr1 = [|]
var arr2 = [| ]
var arr3 = [ |]|
var arr4 = |[ | ]
var arr5 = [|
  | null |
|]
var arr6 = [ |
  12345
  |  true
| ]
";
            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task RequestCompletionsInExpressions_AtPositionsWhereNodeShouldNotBeInserted_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
var unary = |! | true
var binary = -1 | |+| | 2
var ternary = true | |?| | 'yes' | |:| | 'no'
";
            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task RequestCompletionsInTopLevelDeclarations_AtPositionsWhereNodeShouldNotBeInserted_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
|param foo string
v|ar expr = 1 + 2
";
            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task RequestCompletionsInObjectValues_InStringSegments_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
var v2 = 'V2'

resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'name'
  location: resourceGroup().location
  kind: 'S|torage${v2}'
  sku: {
    name: 'Premium_LRS'
    tier: 'Premium'
  }
}
";
            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsEmpty);
        }


        [TestMethod]
        public async Task RequestCompletions_MatchingNodeIsBooleanOrIntegerOrNullLiteral_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
var booleanExp = !|tr|ue| && |fal|se|
var integerExp = |12|345| + |543|21|
var nullLit = |n|ull|
";
            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task RequestingCompletionsForLoopBodyShouldReturnNonLoopCompletions()
        {
            var fileWithCursors = @"
resource foo 'Microsoft.AgFoodPlatform/farmBeats@2020-05-12-preview' = [for item in list: |]

module bar 'doesNotExist.bicep' = [for item in list:|]

module bar2 'test.bicep' = [for item in list: |  ]
";

            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            Uri mainUri = new Uri("file:///main.bicep");
            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [new Uri("file:///test.bicep")] = @"param foo string",
                [mainUri] = file
            });

            var bicepFile = SourceFileFactory.CreateBicepFile(mainUri, file);
            var creationOptions = new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), FileResolver: fileResolver);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(this.TestContext, file, bicepFile.FileUri, creationOptions: creationOptions);
            var completions = await RequestCompletions(client, bicepFile, cursors);

            completions.Should().SatisfyRespectively(
                y => y.Should().SatisfyRespectively(
                    x => x.Label.Should().Be("{}"),
                    x => x.Label.Should().Be("required-properties"),
                    x => x.Label.Should().Be("if")),
                y => y.Should().SatisfyRespectively(
                    x => x.Label.Should().Be("{}"),
                    // no required-properties because the module doesn't exist
                    x => x.Label.Should().Be("if")),
                y => y.Should().SatisfyRespectively(
                    x => x.Label.Should().Be("{}"),
                    // valid module with a parameter, so we should have required-properties
                    x => x.Label.Should().Be("required-properties"),
                    x => x.Label.Should().Be("if")));
        }

        [TestMethod]
        public async Task RequestModulePathCompletions_ArmTemplateFilesInDir_ReturnsCompletionsIncludingArmTemplatePaths()
        {
            var mainUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
            var armTemplateUri1 = DocumentUri.FromFileSystemPath("/path/to/template1.arm");
            var armTemplateUri2 = DocumentUri.FromFileSystemPath("/path/to/template2.json");
            var armTemplateUri3 = DocumentUri.FromFileSystemPath("/path/to/template3.jsonc");
            var armTemplateUri4 = DocumentUri.FromFileSystemPath("/path/to/template4.json");
            var armTemplateUri5 = DocumentUri.FromFileSystemPath("/path/to/template5.json");
            var jsonUri1 = DocumentUri.FromFileSystemPath("/path/to/json1.json");
            var jsonUri2 = DocumentUri.FromFileSystemPath("/path/to/json2.json");
            var bicepModuleUri1 = DocumentUri.FromFileSystemPath("/path/to/module1.txt");
            var bicepModuleUri2 = DocumentUri.FromFileSystemPath("/path/to/module2.bicep");
            var bicepModuleUri3 = DocumentUri.FromFileSystemPath("/path/to/module3.bicep");

            var (mainFileText, cursors) = ParserHelper.GetFileWithCursors(@"
module mod1 './module1.txt' = {}
module mod2 './template3.jsonc' = {}
module mod2 './|' = {}
");
            var mainFile = SourceFileFactory.CreateBicepFile(mainUri.ToUri(), mainFileText);
            var schema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";

            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [mainUri.ToUri()] = mainFileText,
                [armTemplateUri1.ToUri()] = "",
                [armTemplateUri2.ToUri()] = @$"{{ ""schema"": ""{schema}"" }}",
                [armTemplateUri3.ToUri()] = @"{}",
                [armTemplateUri4.ToUri()] = new string('x', 2000 - schema.Length) + schema,
                [armTemplateUri5.ToUri()] = new string('x', 2002 - schema.Length) + schema,
                [jsonUri1.ToUri()] = "{}",
                [jsonUri2.ToUri()] = @"[{ ""name"": ""value"" }]",
                [bicepModuleUri1.ToUri()] = "param foo string",
                [bicepModuleUri2.ToUri()] = "param bar bool",
                [bicepModuleUri3.ToUri()] = "",
            };

            var fileResolver = new InMemoryFileResolver(fileTextsByUri);

            var client = await IntegrationTestHelper.StartServerWithTextAsync(
                TestContext,
                mainFileText,
                mainUri,
                creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), FileResolver: fileResolver));

            var completionLists = await RequestCompletions(client, mainFile, cursors);
            completionLists.Should().HaveCount(1);

            var completionItems = completionLists.Single()!.Items;
            completionItems.Should().SatisfyRespectively(
                x => x.Label.Should().Be("module2.bicep"),
                x => x.Label.Should().Be("module3.bicep"),
                x => x.Label.Should().Be("template1.arm"),
                x => x.Label.Should().Be("template2.json"),
                x => x.Label.Should().Be("template3.jsonc"),
                x => x.Label.Should().Be("template4.json"));
        }

        [TestMethod]
        public async Task VerifyObjectBodyCompletionReturnsEmptyAndRequiredPropertiesSnippets()
        {
            string fileWithCursors = @"resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: 'name'
  properties: |
}";
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(TestContext, file, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider));
            var completionLists = await RequestCompletions(client, bicepFile, cursors);

            completionLists.Count().Should().Be(1);

            var snippetCompletions = completionLists.First()!.Items.Where(x => x.Kind == CompletionItemKind.Snippet);

            snippetCompletions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("{}");
                },
                c =>
                {
                    c.Label.Should().Be("required-properties");
                });
        }

        [TestMethod]
        public async Task VerifyCompletionRequestWithinResourceDeclarationReturnsSnippets()
        {
            string fileWithCursors = @"resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
  location: resourceGroup().location
  |
}";
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(TestContext, file, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider));
            var completionLists = await RequestCompletions(client, bicepFile, cursors);

            completionLists.Count().Should().Be(1);

            var snippetCompletions = completionLists.First()!.Items.Where(x => x.Kind == CompletionItemKind.Snippet);

            snippetCompletions.Should().SatisfyRespectively(
                x =>
                {
                    x.Label.Should().Be("resource-with-defaults");
                },
                x =>
                {
                    x.Label.Should().Be("resource-without-defaults");
                },
                x =>
                {
                    x.Label.Should().Be("res-automation-cert");
                },
                x =>
                {
                    x.Label.Should().Be("res-automation-cred");
                },
                x =>
                {
                    x.Label.Should().Be("res-automation-job-schedule");
                },
                x =>
                {
                    x.Label.Should().Be("res-automation-runbook");
                },
                x =>
                {
                    x.Label.Should().Be("res-automation-schedule");
                },
                x =>
                {
                    x.Label.Should().Be("res-automation-variable");
                });
        }

        [TestMethod]
        public async Task VerifyObjectBodyCompletionInsideExistingArrayOfObjectsReturnsEmptyAndRequiredPropertiesSnippets()
        {
            string fileWithCursors = @"resource applicationGatewayFirewall 'Microsoft.Network/ApplicationGatewayWebApplicationFirewallPolicies@2020-11-01' = {
  name: 'name'
  properties: {
    managedRules: {
      managedRuleSets: [
        |
      ]
    }
  }
}";
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///path/to/main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(TestContext, file, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider));
            var completionLists = await RequestCompletions(client, bicepFile, cursors);

            completionLists.Count().Should().Be(1);

            var snippetCompletions = completionLists.First()!.Items.Where(x => x.Kind == CompletionItemKind.Snippet);

            snippetCompletions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("{}");
                },
                c =>
                {
                    c.Label.Should().Be("required-properties");
                });
        }

        [TestMethod]
        public async Task Import_completions_work_if_feature_enabled()
        {
            var fileWithCursors = @"
|
import ns1 |
import ns2 f|
import ns3 f|r
import ns4 from|
import ns5 from |
import ns6 from a|
import ns7 from s|y
import ns8 from sys|
";
            var features = BicepTestConstants.CreateFeaturesProvider(TestContext, importsEnabled: true);
            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, completions => completions.Should().SatisfyRespectively(
                c => c!.Select(x => x.Label).Should().Contain("import"),
                c => c!.Select(x => x.Label).Should().Equal("from"),
                c => c!.Select(x => x.Label).Should().Equal("from"),
                c => c!.Select(x => x.Label).Should().Equal("from"),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().Equal("az", "sys"),
                c => c!.Select(x => x.Label).Should().Equal("az", "sys"),
                c => c!.Select(x => x.Label).Should().Equal("az", "sys"),
                c => c!.Select(x => x.Label).Should().Equal("az", "sys")
            ), features);

            features = BicepTestConstants.CreateFeaturesProvider(TestContext, importsEnabled: false);
            await RunCompletionScenarioTest(this.TestContext, fileWithCursors, completions => completions.Should().SatisfyRespectively(
                c => c!.Select(x => x.Label).Should().NotContain("import"),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().BeEmpty()
            ), features);
        }

        private static void AssertAllCompletionsNonEmpty(IEnumerable<CompletionList?> completionLists)
        {
            foreach (var completionList in completionLists)
            {
                completionList.Should().NotBeEmpty();
            }
        }

        private static void AssertAllCompletionsEmpty(IEnumerable<CompletionList?> completionLists)
        {
            foreach (var completionList in completionLists)
            {
                completionList.Should().BeEmpty();
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

            actual.Should().EqualWithJsonDiffOutput(this.TestContext, expected, expectedLocation, actualLocation, "because ");
        }

        private static string GetGlobalCompletionSetPath(string setName) => Path.Combine("src", "Bicep.Core.Samples", "Files", DataSet.TestCompletionsDirectory, GetFullSetName(setName));

        private static async Task RunCompletionScenarioTest(TestContext testContext, string fileWithCursors, Action<IEnumerable<CompletionList?>> assertAction, IFeatureProvider? featureProvider = null)
        {
            var (file, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var bicepFile = SourceFileFactory.CreateBicepFile(new Uri("file:///main.bicep"), file);
            var client = await IntegrationTestHelper.StartServerWithTextAsync(testContext, file, bicepFile.FileUri, creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), Features: featureProvider));
            var completions = await RequestCompletions(client, bicepFile, cursors);

            assertAction(completions);
        }

        private static string FormatPosition(Position position) => $"({position.Line}, {position.Character})";

        private static async Task<JToken> GetActualCompletions(ILanguageClient client, DocumentUri uri, Position position)
        {
            // local function
            string? NormalizeLineEndings(string? value) => value is null
                ? null
                : value.Replace("\r", string.Empty);

            StringOrMarkupContent? NormalizeDocumentation(StringOrMarkupContent? value) =>
                value?.MarkupContent?.Value is null
                    ? null
                    : new(value.MarkupContent with
                    {
                        Value = NormalizeLineEndings(value.MarkupContent.Value)!
                    });

            TextEditOrInsertReplaceEdit? NormalizeTextEdit(TextEditOrInsertReplaceEdit? value) =>
                value is null
                    ? null
                    : new TextEditOrInsertReplaceEdit(value.TextEdit! with
                    {
                        NewText = NormalizeLineEndings(value.TextEdit.NewText)!,

                        // ranges in these text edits will vary by completion trigger position
                        // if we try to assert on these, we will have an explosion of assert files
                        // let's ignore it for now until we come up with a better solution
                        Range = new Range()
                    });

            TextEditContainer? NormalizeAdditionalTextEdits(TextEditContainer? value) =>
                value is null
                    ? null
                    : new TextEditContainer(value.Select(edit => edit with
                    {
                        Range = new Range()
                    }));

            // OmniSharp sometimes will add a $$__handler_id__$$ property to the Data dictionary of a completion
            // (likely is needed to somehow help with routing of the ResolveCompletion requests)
            // the value is different every time you run the language server
            // to make our test asserts work, we will remove it and set the Data property null if nothing else remains in the object
            JToken? NormalizeData(JToken? value)
            {
                // LSP protocol dictates that the Data property is of "any" type, so we need to check
                if (value is JObject @object)
                {
                    const string omnisharpHandlerIdPropertyName = "$$__handler_id__$$";

                    if (@object.Property(omnisharpHandlerIdPropertyName) != null)
                    {
                        @object.Remove(omnisharpHandlerIdPropertyName);

                        if (!@object.HasValues)
                        {
                            return null;
                        }

                        return @object;
                    }
                }

                return value;
            }

            var completions = await client.RequestCompletion(new CompletionParams
            {
                TextDocument = new TextDocumentIdentifier(uri),
                Position = position
            });

            // normalize the completions to remove OS-specific mismatches (line endings) as well as any randomness
            var normalized = completions.Select(completion => completion with
            {
                InsertText = NormalizeLineEndings(completion.InsertText),
                Detail = NormalizeLineEndings(completion.Detail),
                Documentation = NormalizeDocumentation(completion.Documentation),
                TextEdit = NormalizeTextEdit(completion.TextEdit),
                AdditionalTextEdits = NormalizeAdditionalTextEdits(completion.AdditionalTextEdits),
                Data = NormalizeData(completion.Data)
            });

            return JToken.FromObject(normalized.OrderBy(c => c.Label, StringComparer.Ordinal), DataSetSerialization.CreateSerializer());
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

        private static async Task<IEnumerable<CompletionList?>> RequestCompletions(ILanguageClient client, BicepFile bicepFile, IEnumerable<int> cursors)
        {
            var completions = new List<CompletionList?>();
            foreach (var cursor in cursors)
            {
                var completionList = await client.RequestCompletion(new CompletionParams
                {
                    TextDocument = new TextDocumentIdentifier(bicepFile.FileUri),
                    Position = TextCoordinateConverter.GetPosition(bicepFile.LineStarts, cursor)
                });

                completions.Add(completionList);
            }

            return completions;
        }
    }
}
