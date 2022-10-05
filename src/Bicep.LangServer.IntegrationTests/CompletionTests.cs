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
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Completions;
using Bicep.LangServer.IntegrationTests.Helpers;
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
    public class CompletionTests
    {
        private static ServiceBuilder Services => new ServiceBuilder();

        public static readonly INamespaceProvider NamespaceProvider = BicepTestConstants.NamespaceProvider;

        private static readonly SharedLanguageHelperManager ServerWithNamespaceProvider = new();

        private static readonly SharedLanguageHelperManager ServerWithNamespaceAndTestResolver = new();

        private static readonly SharedLanguageHelperManager DefaultServer = new();

        private static readonly SharedLanguageHelperManager ServerWithImportsEnabled = new();

        private static readonly SharedLanguageHelperManager ServerWithBuiltInTypes = new();

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

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            ServerWithNamespaceProvider.Initialize(
                async () => await MultiFileLanguageServerHelper.StartLanguageServer(
                    testContext,
                    creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider)));

            ServerWithNamespaceAndTestResolver.Initialize(
                async () => await MultiFileLanguageServerHelper.StartLanguageServer(
                    testContext,
                    creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: NamespaceProvider, FileResolver: BicepTestConstants.FileResolver)));

            DefaultServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext));

            ServerWithImportsEnabled.Initialize(
                async () => await MultiFileLanguageServerHelper.StartLanguageServer(
                    testContext,
                    new LanguageServer.Server.CreationOptions(FeatureProviderFactory: IFeatureProviderFactory.WithStaticFeatureProvider(BicepTestConstants.CreateFeatureProvider(testContext, importsEnabled: true)))));

            ServerWithBuiltInTypes.Initialize(
                async () => await MultiFileLanguageServerHelper.StartLanguageServer(
                    testContext,
                    new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create())));
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await ServerWithNamespaceProvider.DisposeAsync();
            await ServerWithNamespaceAndTestResolver.DisposeAsync();
            await DefaultServer.DisposeAsync();
            await ServerWithImportsEnabled.DisposeAsync();
            await ServerWithBuiltInTypes.DisposeAsync();
        }

        [TestMethod]
        public async Task EmptyFileShouldProduceDeclarationCompletions()
        {
            const string expectedSetName = "declarations";
            var uri = DocumentUri.From($"/{this.TestContext.TestName}");

            var helper = await DefaultServer.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, string.Empty, uri);

            var actual = await GetActualCompletions(helper.Client, uri, new Position(0, 0));
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
            string pathPrefix = $"Files/SnippetTemplates/{completionData.Prefix}";

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
                var compilation = Services.BuildCompilation(new Dictionary<Uri, string>
                {
                    [combinedFileUri] = bicepContentsReplaced,
                }, combinedFileUri);
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

            var helper = await ServerWithNamespaceProvider.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, placeholderFile, documentUri);

            var completions = await helper.Client.RequestCompletion(new CompletionParams
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

            var helper = await ServerWithNamespaceAndTestResolver.GetAsync();

            await helper.OpenFileOnceAsync(this.TestContext, dataSet.Bicep, uri);

            var intermediate = new List<(Position position, JToken actual)>();

            foreach (var position in positions)
            {
                var actual = await GetActualCompletions(helper.Client, uri, position);

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

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task Completions_are_offered_in_string_expressions()
        {
            var fileWithCursors = @"
var interpolatedString = 'abc${|true}def${|}ghi${res|}xyz'
";

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsNonEmpty);
        }

        [TestMethod]
        public async Task Completions_are_offered_immediately_before_and_after_comments()
        {
            var fileWithCursors = @"
var test = |// comment here
var test2 = |/* block comment */|
";

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsNonEmpty);
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

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task Completions_are_not_offered_inside_comments()
        {
            var fileWithCursors = @"
var test = /|/ comment here|
var test2 = /|* block c|omment *|/
";

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task VerifyResourceBodyCompletionWithExistingKeywordDoesNotIncludeCustomSnippet()
        {
            string text = "resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' existing = ";
            var completions = await RunSingleCompletionScenarioTest(this.TestContext, ServerWithNamespaceProvider, text, text.Length);

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
            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                fileWithCursors,
                (completionLists) =>
                {
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
            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                fileWithCursors,
                completionLists =>
                {
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
            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                fileWithCursors,
                completionLists =>
                {
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
                });
        }

        [TestMethod]
        public async Task VerifyResourceBodyCompletionWithoutExistingKeywordIncludesCustomSnippet()
        {
            string text = @"resource aksCluster 'Microsoft.ContainerService/managedClusters@2021-03-01' = ";
            var completions = await RunSingleCompletionScenarioTest(this.TestContext, ServerWithNamespaceProvider, text, text.Length);

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
            var completions = await RunSingleCompletionScenarioTest(this.TestContext, ServerWithNamespaceProvider, text, text.Length);

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

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithBuiltInTypes,
                fileWithCursors,
                completions =>
                    completions.Should().SatisfyRespectively(
                        x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                            d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which is required."),
                            d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which supports reading AND writing!"),
                            d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which only supports writing.")),
                        x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                            d => d.Documentation!.MarkupContent!.Value.Should().Contain("The resource api version"),
                            d => d.Documentation!.MarkupContent!.Value.Should().Contain("The resource id"),
                            d => d.Documentation!.MarkupContent!.Value.Should().Contain("The resource name"),
                            d => d.Documentation!.MarkupContent!.Value.Should().Contain("properties property"),
                            d => d.Documentation!.MarkupContent!.Value.Should().Contain("The resource type")),
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

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithBuiltInTypes,
                fileWithCursors,
                completions =>
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


            await RunCompletionScenarioTest(this.TestContext,
                ServerWithBuiltInTypes,
                fileWithCursors,
                completions =>
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

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithBuiltInTypes,
                    fileWithCursors,
                completions =>
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

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithBuiltInTypes,
                fileWithCursors,
                completions =>
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

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithBuiltInTypes,
                fileWithCursors,
                completions =>
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
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty);
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

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsContainResourceLabel);
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
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty);
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
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty);
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
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task RequestCompletionsInExpressions_AtPositionsWhereNodeShouldNotBeInserted_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
var unary = |! | true
var binary = -1 | |+| | 2
var ternary = true | |?| | 'yes' | |:| | 'no'
";
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task RequestCompletionsInTopLevelDeclarations_AtPositionsWhereNodeShouldNotBeInserted_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
|param foo string
v|ar expr = 1 + 2
";
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty);
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
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty);
        }


        [TestMethod]
        public async Task RequestCompletions_MatchingNodeIsBooleanOrIntegerOrNullLiteral_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
var booleanExp = !|tr|ue| && |fal|se|
var integerExp = |12|345| + |543|21|
var nullLit = |n|ull|
";
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty);
        }

        [TestMethod]
        public async Task RequestingCompletionsForLoopBodyShouldReturnNonLoopCompletions()
        {
            var fileWithCursors = @"
resource foo 'Microsoft.AgFoodPlatform/farmBeats@2020-05-12-preview' = [for item in list: |]

module bar 'doesNotExist.bicep' = [for item in list:|]

module bar2 'test.bicep' = [for item in list: |  ]
";

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            Uri mainUri = new Uri("file:///main.bicep");
            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [new Uri("file:///test.bicep")] = @"param foo string",
                [mainUri] = text
            });

            var bicepFile = SourceFileFactory.CreateBicepFile(mainUri, text);
            var creationOptions = new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), FileResolver: fileResolver);
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(this.TestContext, text, bicepFile.FileUri, creationOptions: creationOptions);

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestCompletions(cursors);

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
            var mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");
            var (mainFileText, cursor) = ParserHelper.GetFileWithSingleCursor(@"
module mod1 './module1.txt' = {}
module mod2 './template3.jsonc' = {}
module mod2 './|' = {}
");
            var mainFile = SourceFileFactory.CreateBicepFile(mainUri, mainFileText);
            var schema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";

            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [mainUri] = mainFileText,
                [InMemoryFileResolver.GetFileUri("/path/to/template1.arm")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/template2.json")] = @$"{{ ""schema"": ""{schema}"" }}",
                [InMemoryFileResolver.GetFileUri("/path/to/template3.jsonc")] = @"{}",
                [InMemoryFileResolver.GetFileUri("/path/to/template4.json")] = new string('x', 2000 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/template5.json")] = new string('x', 2002 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/json1.json")] = "{}",
                [InMemoryFileResolver.GetFileUri("/path/to/json2.json")] = @"[{ ""name"": ""value"" }]",
                [InMemoryFileResolver.GetFileUri("/path/to/module1.txt")] = "param foo string",
                [InMemoryFileResolver.GetFileUri("/path/to/module2.bicep")] = "param bar bool",
                [InMemoryFileResolver.GetFileUri("/path/to/module3.bicep")] = "",
            };

            var fileResolver = new InMemoryFileResolver(fileTextsByUri);
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(
                TestContext,
                mainFileText,
                mainUri,
                creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), FileResolver: fileResolver));

            var file = new FileRequestHelper(helper.Client, mainFile);

            var completions = await file.RequestCompletion(cursor);
            completions.Should().SatisfyRespectively(
                x => x.Label.Should().Be("module2.bicep"),
                x => x.Label.Should().Be("module3.bicep"),
                x => x.Label.Should().Be("template1.arm"),
                x => x.Label.Should().Be("template2.json"),
                x => x.Label.Should().Be("template3.jsonc"),
                x => x.Label.Should().Be("template4.json"),
                x => x.Label.Should().Be("../"));

            file.ApplyCompletion(completions, "module2.bicep").Should().HaveSourceText(@"
module mod1 './module1.txt' = {}
module mod2 './template3.jsonc' = {}
module mod2 './module2.bicep'| = {}
");
        }

        [TestMethod]
        public async Task VerifyObjectBodyCompletionReturnsSnippets()
        {
            string fileWithCursors = @"resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' = {
  name: 'name'
  properties: |
}";
            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                fileWithCursors,
                completionLists =>
                {
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
                        },
                        c =>
                        {
                            c.Label.Should().Be("if-else");
                        });
                });
        }

        [TestMethod]
        public async Task VerifyCompletionRequestWithinResourceDeclarationReturnsSnippets()
        {
            string fileWithCursors = @"#disable-next-line use-recent-api-versions
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
  location: resourceGroup().location
  |
}";
            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                fileWithCursors,
                completionLists =>
                {
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
                            x.Label.Should().Be("res-automation-module");
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
            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                fileWithCursors,
                completionLists =>
                {
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
                        },
                        c =>
                        {
                            c.Label.Should().Be("if-else");
                        });
                });
        }

        [TestMethod]
        public async Task Import_completions_work_if_feature_enabled()
        {
            var fileWithCursors = @"
|
import ns1 |
import ns2 a|
import ns3 as|
import |
import a|
";
            await RunCompletionScenarioTest(this.TestContext, ServerWithImportsEnabled, fileWithCursors, completions => completions.Should().SatisfyRespectively(
                c => c!.Select(x => x.Label).Should().Contain("import"),
                c => c!.Select(x => x.Label).Should().Equal("as"),
                c => c!.Select(x => x.Label).Should().Equal("as"),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().Equal("az", "kubernetes", "sys"),
                c => c!.Select(x => x.Label).Should().Equal("az", "kubernetes", "sys")
            ));

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, completions => completions.Should().SatisfyRespectively(
                c => c!.Select(x => x.Label).Should().NotContain("import"),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().BeEmpty(),
                c => c!.Select(x => x.Label).Should().BeEmpty()
            ));
        }

        [TestMethod]
        public async Task Import_configuration_completions_work()
        {
            {
                var fileWithCursors = @"
import kubernetes as k8s |
";

                var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
                var file = await new ServerRequestHelper(TestContext, ServerWithImportsEnabled).OpenFile(text);

                var completions = await file.RequestCompletion(cursor);
                completions.Should().Contain(x => x.Label == "{}");
                completions.Should().Contain(x => x.Label == "required-properties");

                var updatedFile = file.ApplyCompletion(completions, "required-properties");
                updatedFile.Should().HaveSourceText(@"
import kubernetes as k8s {
  kubeConfig: $1
  namespace: $2
}|
");
            }

            {
                var fileWithCursors = @"
import kubernetes as k8s {
  |
}
";

                var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
                var file = await new ServerRequestHelper(TestContext, ServerWithImportsEnabled).OpenFile(text);

                var completions = await file.RequestCompletion(cursor);
                completions.Should().Contain(x => x.Label == "namespace");
                completions.Should().Contain(x => x.Label == "kubeConfig");

                var updatedFile = file.ApplyCompletion(completions, "kubeConfig");
                updatedFile.Should().HaveSourceText(@"
import kubernetes as k8s {
  kubeConfig:|
}
");
            }

            {
                // az provider does not support configuration - expect no completions
                var fileWithCursors = @"
import az as az |
";

                var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
                var file = await new ServerRequestHelper(TestContext, ServerWithImportsEnabled).OpenFile(text);

                var completions = await file.RequestCompletion(cursor);
                completions.Should().BeEmpty();
            }
        }

        [TestMethod]
        public async Task ModuleCompletionsShouldNotBeUrlEscaped()
        {
            var fileWithCursors = @"
module a '|' = {
    name: 'modA'
}
";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            Uri mainUri = InMemoryFileResolver.GetFileUri("/dir/main.bicep");
            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [InMemoryFileResolver.GetFileUri("/dir/folder with space/mod with space.bicep")] = @"param foo string",
                [InMemoryFileResolver.GetFileUri("/dir/percentage%file.bicep")] = @"param foo string",
                [InMemoryFileResolver.GetFileUri("/dir/already%20escaped.bicep")] = @"param foo string",
                [mainUri] = text
            });

            var bicepFile = SourceFileFactory.CreateBicepFile(mainUri, text);
            var creationOptions = new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), FileResolver: fileResolver);
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(this.TestContext, text, bicepFile.FileUri, creationOptions: creationOptions);

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestCompletion(cursor);

            completions.Should().SatisfyRespectively(
                x => x.Label.Should().Be("percentage%file.bicep"),
                x => x.Label.Should().Be("already%20escaped.bicep"),
                x => x.Label.Should().Be("folder with space/"),
                x => x.Label.Should().Be("../"));
        }

        [TestMethod]
        public async Task ModuleCompletionsShouldContainDescriptions()
        {
            var moduleContent = @"
@description('input that you want multiplied by 3')
param input int = 2

@description('input multiplied by 3')
output inputTimesThree int = input * 3
";


            var mainContent = @"
module m 'mod.bicep' = {
    name: 'myMod'
    params: {
        i|
    }
}

var modOut = m.outputs.inputTi|
";

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent);
            Uri mainUri = new Uri("file:///main.bicep");
            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [new Uri("file:///mod.bicep")] = moduleContent,
                [mainUri] = text
            });

            var bicepFile = SourceFileFactory.CreateBicepFile(mainUri, text);
            var creationOptions = new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), FileResolver: fileResolver);
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(
                this.TestContext,
                text,
                bicepFile.FileUri,
                null,
                creationOptions);

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestCompletions(cursors);

            completions.Should().SatisfyRespectively(
                y => y.Should().SatisfyRespectively(
                    x => x.Documentation?.MarkupContent?.Value.Should().Contain("input that you want multiplied by 3")),
                y => y.Should().SatisfyRespectively(
                    x => x.Documentation?.MarkupContent?.Value.Should().Contain("input multiplied by 3"))
            );
        }

        [TestMethod]
        public async Task Resource_type_completions_return_filtered_api_versions()
        {
            var fileWithCursors = @"
resource abc 'Test.Rp/basicTests@|'
";

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithBuiltInTypes,
                fileWithCursors,
                completions =>
                    completions.Should().SatisfyRespectively(
                        c => c.Should().SatisfyRespectively(
                            x => x.Label.Should().Be("2020-01-01"))));
        }

        [TestMethod]
        public async Task Resource_type_completions_return_filtered_types()
        {
            var fileWithCursors = @"
resource abc 'Test.Rp/basic|'
";

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithBuiltInTypes,
                fileWithCursors,
                completions =>
                    completions.Should().SatisfyRespectively(
                        c => c.Should().Contain(
                            x => x.Label == "'Test.Rp/basicTests'")));
        }

        [TestMethod]
        public async Task Known_list_functions_are_offered()
        {
            var fileWithCursors = @"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.|
";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestCompletion(cursor);
            completions.Should().Contain(x => x.Label == "listNoInput");
            completions.Should().Contain(x => x.Label == "listWithInput");

            var updatedFile = file.ApplyCompletion(completions, "listWithInput");
            updatedFile.Should().HaveSourceText(@"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput(|)
");
        }

        [TestMethod]
        public async Task List_functions_accepting_inputs_suggest_api_version_param()
        {
            var fileWithCursors = @"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput(|)
";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestCompletion(cursor);
            completions.Should().Contain(x => x.Label == "'2020-01-01'");

            var updatedFile = file.ApplyCompletion(completions, "'2020-01-01'");
            updatedFile.Should().HaveSourceText(@"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01'|)
");
        }

        [TestMethod]
        public async Task List_functions_accepting_inputs_suggest_required_properties()
        {
            var fileWithCursors = @"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', |)
";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestCompletion(cursor);
            completions.Should().Contain(x => x.Label == "required-properties");

            var updatedFile = file.ApplyCompletion(completions, "required-properties", "foo");
            updatedFile.Should().HaveSourceText(@"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: foo
}|)
");
        }

        [TestMethod]
        public async Task List_functions_accepting_inputs_permit_object_key_completions()
        {
            var fileWithCursors = @"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: 'hello'
  |
})
";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestCompletion(cursor);
            completions.Should().Contain(x => x.Label == "optionalVal");

            var updatedFile = file.ApplyCompletion(completions, "optionalVal");
            updatedFile.Should().HaveSourceText(@"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: 'hello'
  optionalVal:|
})
");
        }

        [DataTestMethod]
        [DataRow(@"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: 'hello'
  optionalLiteralVal: |
})
", @"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: 'hello'
  optionalLiteralVal: 'either'|
})
")]
        [DataRow(@"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: 'hello'
  optionalLiteralVal: (|)
})
", @"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: 'hello'
  optionalLiteralVal: ('either'|)
})
")]
        [DataRow(@"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: 'hello'
  optionalLiteralVal: true ? | : 'or'
})
", @"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: 'hello'
  optionalLiteralVal: true ? 'either'| : 'or'
})
")]
        [DataRow(@"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: 'hello'
  optionalLiteralVal: true ? 'or' : |
})
", @"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: 'hello'
  optionalLiteralVal: true ? 'or' : 'either'|
})
")]
        [DataRow(@"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: 'hello'
  optionalLiteralVal: true ? 'or' : (true ? |)
})
", @"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput('2020-01-01', {
  withInputInputVal: 'hello'
  optionalLiteralVal: true ? 'or' : (true ? 'either'|)
})
")]
        public async Task List_functions_accepting_inputs_permit_object_value_completions(string fileWithCursors, string updatedFileWithCursors)
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestCompletion(cursor);
            completions.Should().Contain(x => x.Label == "'either'");
            completions.Should().Contain(x => x.Label == "'or'");

            var updatedFile = file.ApplyCompletion(completions, "'either'");
            updatedFile.Should().HaveSourceText(updatedFileWithCursors);
        }

        [TestMethod]
        public async Task List_functions_return_property_completions()
        {
            var fileWithCursors = @"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput().|
";
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestCompletion(cursor);
            completions.Should().Contain(x => x.Label == "withInputOutputVal");

            var updatedFile = file.ApplyCompletion(completions, "withInputOutputVal");
            updatedFile.Should().HaveSourceText(@"
resource abc 'Test.Rp/listFuncTests@2020-01-01' existing = {
  name: 'abc'
}

var outTest = abc.listWithInput().withInputOutputVal|
");
        }

        [TestMethod]
        public async Task List_functions_support_completions_for_az_types()
        {
            var fileWithCursors = @"
resource stg 'Microsoft.Storage/storageAccounts@2021-06-01' existing = {
  name: 'stg'
}

var testA = stg.|
var testB = stg.listKeys().|
var testC = stg.listKeys().keys[0].|
var testD = stg.listAccountSas(|)
var testE = stg.listAccountSas('2021-06-01', |)
var testF = stg.listAccountSas('2021-06-01', {}).|
";

            await RunCompletionScenarioTest(
                this.TestContext,
                DefaultServer,
                fileWithCursors,
                completions =>
                    completions.Should().SatisfyRespectively(
                        c => c!.Select(x => x.Label).Should().Contain("listKeys", "listAccountSas", "listServiceSas"),
                        c => c!.Select(x => x.Label).Should().Contain("keys"),
                        c => c!.Select(x => x.Label).Should().Contain("creationTime", "value", "permissions", "keyName"),
                        c => c!.Select(x => x.Label).Should().Contain("'2021-06-01'"),
                        c => c!.Select(x => x.Label).Should().Contain("required-properties"),
                        c => c!.Select(x => x.Label).Should().Contain("accountSasToken")
                    )
);
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterPoundSign_ShouldReturnCompletionItem()
        {
            var fileWithCursors = @"#|
param storageAccount string = 'testAccount";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestCompletion(cursor);

            var updatedFile = file.ApplyCompletion(completions, LanguageConstants.DisableNextLineDiagnosticsKeyword);
            updatedFile.Should().HaveSourceText(@"#disable-next-line|
param storageAccount string = 'testAccount");
        }

        [TestMethod]
        public async Task Lambda_function_completions_suggest_local_variables()
        {
            var fileWithCursors = @"
var foo = map([123], i => |)
var bar = map([123], i => {
  key: |
})
var sorted = sort(['abc'], (x, y) => |)
var sorted2 = sort(['abc'], (x, y) => x < |)
";

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestCompletions(cursors);

            completions.Should().SatisfyRespectively(
                c => c.Should().Contain(x => x.Label == "i"),
                c => c.Should().Contain(x => x.Label == "i"),
                c => c.Should().Contain(x => x.Label == "x")
                    .And.Contain(x => x.Label == "y"),
                c => c.Should().Contain(x => x.Label == "x")
                    .And.Contain(x => x.Label == "y"));
        }

        [TestMethod]
        public async Task List_comprehension_functions_return_lambda_snippets_single_arg()
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(@"
var foo = map([123], |)
");

            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestCompletion(cursor);
            var updatedFile = file.ApplyCompletion(completions, "arg => ...", "foo");
            updatedFile.Should().HaveSourceText(@"
var foo = map([123], foo => |)
");
        }

        [TestMethod]
        public async Task List_comprehension_functions_return_lambda_snippets_multiple_args()
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(@"
var foo = sort([123], |)
");

            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestCompletion(cursor);
            var updatedFile = file.ApplyCompletion(completions, "(arg1, arg2) => ...", "foo", "bar");
            updatedFile.Should().HaveSourceText(@"
var foo = sort([123], (foo, bar) => |)
");
        }

        [TestMethod]
        public async Task VerifyCompletionrequestAfterPoundSignWithinComment_ShouldDoNothing()
        {
            var fileWithCursors = @"// This is a comment #|";

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                fileWithCursors,
                completionLists => completionLists.Should().SatisfyRespectively(
                    completions => completions.Should().BeEmpty()));
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterPoundSign_WithTextBeforePoundSign_ShouldDoNothing()
        {
            var fileWithCursors = @"var terminatedWithDirective = 'foo' #|";

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                fileWithCursors,
                completionLists => completionLists.Should().SatisfyRespectively(
                    completions => completions.Should().BeEmpty()));
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterPoundSign_WithCommentBeforePoundSign_ShouldDoNothing()
        {
            var fileWithCursors = @"/* test */#|";

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                fileWithCursors,
                completionLists => completionLists.Should().SatisfyRespectively(
                    completions => completions.Should().BeEmpty()));
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterPoundSign_WithWhiteSpaceAndCommentBeforePoundSign_ShouldDoNothing()
        {
            var fileWithCursors = @"   /* test */  #|";

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                fileWithCursors,
                completionLists => completionLists.Should().SatisfyRespectively(
                    completions => completions.Should().BeEmpty()));
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterPoundSign_WithWhiteSpaceBeforePoundSign_ShouldReturnCompletionItem()
        {
            var fileWithCursors = @"    #|
param storageAccount1 string = 'testAccount'
    #|
param storageAccount2 string = 'testAccount'";

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestCompletion(cursors[0]);
            completions.Should().Contain(x => x.Label == LanguageConstants.DisableNextLineDiagnosticsKeyword);

            var updatedFile = file.ApplyCompletion(completions, LanguageConstants.DisableNextLineDiagnosticsKeyword);
            updatedFile.Should().HaveSourceText(@"    #disable-next-line|
param storageAccount1 string = 'testAccount'
    #
param storageAccount2 string = 'testAccount'");

            completions = await file.RequestCompletion(cursors[1]);
            completions.Should().Contain(x => x.Label == LanguageConstants.DisableNextLineDiagnosticsKeyword);

            updatedFile = file.ApplyCompletion(completions, LanguageConstants.DisableNextLineDiagnosticsKeyword);
            updatedFile.Should().HaveSourceText(@"    #
param storageAccount1 string = 'testAccount'
    #disable-next-line|
param storageAccount2 string = 'testAccount'");
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterPoundSign_WithinResource_ShouldReturnCompletionItem()
        {
            string fileWithCursors = @"var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'West US'
#|
  properties: vmProperties
}";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestCompletion(cursor);
            completions.Should().Contain(x => x.Label == LanguageConstants.DisableNextLineDiagnosticsKeyword);

            var updatedFile = file.ApplyCompletion(completions, LanguageConstants.DisableNextLineDiagnosticsKeyword);
            updatedFile.Should().HaveSourceText(@"var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'West US'
#disable-next-line|
  properties: vmProperties
}");
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterDisableNextLineKeyword_ShouldReturnDiagnosticsCodes()
        {
            string fileWithCursors = @"#disable-next-line |
param storageAccount string = 'testAccount'
var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'West US'
#disable-next-line |
  properties: vmProperties
}";

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithNamespaceProvider).OpenFile(text);

            var completions = await file.RequestCompletion(cursors[0]);
            completions.Should().Contain(x => x.Label == "no-unused-params");

            var updatedFile = file.ApplyCompletion(completions, "no-unused-params");
            updatedFile.Should().HaveSourceText(@"#disable-next-line no-unused-params|
param storageAccount string = 'testAccount'
var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'West US'
#disable-next-line "/* <- preserve trailing space */+@"
  properties: vmProperties
}");

            completions = await file.RequestCompletion(cursors[1]);
            completions.Should().Contain(x => x.Label == "BCP036");
            completions.Should().Contain(x => x.Label == "BCP037");

            updatedFile = file.ApplyCompletion(completions, "BCP036");
            updatedFile.Should().HaveSourceText(@"#disable-next-line "/* <- preserve trailing space */+@"
param storageAccount string = 'testAccount'
var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'West US'
#disable-next-line BCP036|
  properties: vmProperties
}");

            updatedFile = file.ApplyCompletion(completions, "BCP037");
            updatedFile.Should().HaveSourceText(@"#disable-next-line "/* <- preserve trailing space */+@"
param storageAccount string = 'testAccount'
var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'West US'
#disable-next-line BCP037|
  properties: vmProperties
}");
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterDiagnosticCodeInDisableNextLineDirective_ShouldReturnCompletionItemWithRemainingDiagnosticCode()
        {
            string fileWithCursors = @"var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'West US'
#disable-next-line BCP036 |
  properties: vmProperties
}";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithNamespaceProvider).OpenFile(text);

            var completions = await file.RequestCompletion(cursor);
            completions.Should().Contain(x => x.Label == "BCP037");

            var updatedFile = file.ApplyCompletion(completions, "BCP037");
            updatedFile.Should().HaveSourceText(@"var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'West US'
#disable-next-line BCP036 BCP037|
  properties: vmProperties
}");
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterDisableNextLineDirective_WithDiagnosticsSpanningMultipleLines_ShouldReturnCompletionItems()
        {
            string fileWithCursors = @"#disable-next-line |
var foo = concat('abc'/*
  */, 'asd')";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithNamespaceProvider).OpenFile(text);

            var completions = await file.RequestCompletion(cursor);
            completions.Should().Contain(x => x.Label == "no-unused-vars");
            completions.Should().Contain(x => x.Label == "prefer-interpolation");

            var updatedFile = file.ApplyCompletion(completions, "no-unused-vars");
            updatedFile.Should().HaveSourceText(@"#disable-next-line no-unused-vars|
var foo = concat('abc'/*
  */, 'asd')");

            updatedFile = file.ApplyCompletion(completions, "prefer-interpolation");
            updatedFile.Should().HaveSourceText(@"#disable-next-line prefer-interpolation|
var foo = concat('abc'/*
  */, 'asd')");
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterDiagnosticCodeInDisableNextLineDirectiveWithoutWhiteSpace_ShouldDoNothing()
        {
            string fileWithCursors = @"var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'West US'
#disable-next-line BCP036|
  properties: vmProperties
}";

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                fileWithCursors,
                completionLists => completionLists.Should().SatisfyRespectively(
                    completions => completions.Should().BeEmpty()));
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterDiagnosticCodeInDisableNextLineDirective_WithNoDiagnostics_ShouldDoNothing()
        {
            string fileWithCursors = @"var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
  location: 'West US'
#disable-next-line BCP036 BCP037 |
  properties: vmProperties
}";

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                fileWithCursors,
                completionLists => completionLists.Should().SatisfyRespectively(
                    completions => completions.Should().BeEmpty()));
        }

        [TestMethod]
        public async Task VerifyDisableNextLineDiagnosticsDirectiveCompletionIsNotAvailableToSuppressCoreCompilerErrors()
        {
            string fileWithCursors = @"#disable-next-line |
resource test";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            CompilationHelper.Compile(text).ExcludingLinterDiagnostics().Diagnostics.Should().SatisfyRespectively(
                x => x.Code.Should().Be("BCP226"),
                x => x.Code.Should().Be("BCP068"),
                x => x.Code.Should().Be("BCP029"));

            var completions = await file.RequestCompletion(cursor);
            completions.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Descriptions_for_function_completions()
        {
            var fileWithCursors = @"
var foo = resourceI|
";

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithBuiltInTypes,
                fileWithCursors,
                completionLists => completionLists.Should().SatisfyRespectively(
                    completions => completions!.Where(x => x.Label == "resourceId").First().Documentation!.MarkupContent!.Value.Should().EqualIgnoringNewlines(@"```bicep
resourceId(resourceType: string, ... : string): string
resourceId(subscriptionId: string, resourceType: string, ... : string): string
resourceId(resourceGroupName: string, resourceType: string, ... : string): string
resourceId(subscriptionId: string, resourceGroupName: string, resourceType: string, ... : string): string

```
Returns the unique identifier of a resource. You use this function when the resource name is ambiguous or not provisioned within the same template. The format of the returned identifier varies based on whether the deployment happens at the scope of a resource group, subscription, management group, or tenant.
")));
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

        private static async Task<CompletionList> RunSingleCompletionScenarioTest(TestContext testContext, SharedLanguageHelperManager server, string text, int offset)
        {
            var file = await new ServerRequestHelper(testContext, server).OpenFile(text);

            return await file.RequestCompletion(offset);
        }

        private static async Task RunCompletionScenarioTest(TestContext testContext, SharedLanguageHelperManager server, string fileWithCursors, Action<IEnumerable<CompletionList>> assertAction)
        {
            var (fileText, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            var file = await new ServerRequestHelper(testContext, server).OpenFile(fileText);

            var completions = await file.RequestCompletions(cursors);

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


        [DataTestMethod]
        [DataRow("loadTextContent")]
        [DataRow("loadFileAsBase64")]
        [DataRow("loadJsonContent", true)]
        public async Task LoadFunctionsPathArgument_returnsFilesInCompletions(string functionName, bool jsonOnTop = false)
        {
            var mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");

            var (mainFileText, cursor) = ParserHelper.GetFileWithSingleCursor(@"
var file = " + functionName + @"('|')
");
            var mainFile = SourceFileFactory.CreateBicepFile(mainUri, mainFileText);
            var schema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";

            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [mainUri] = mainFileText,
                [InMemoryFileResolver.GetFileUri("/path/to/template1.arm")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/template2.json")] = @$"{{ ""schema"": ""{schema}"" }}",
                [InMemoryFileResolver.GetFileUri("/path/to/template3.jsonc")] = @"{}",
                [InMemoryFileResolver.GetFileUri("/path/to/template4.json")] = new string('x', 2000 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/template5.json")] = new string('x', 2002 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/json1.json")] = "{}",
                [InMemoryFileResolver.GetFileUri("/path/to/json2.json")] = @"[{ ""name"": ""value"" }]",
                [InMemoryFileResolver.GetFileUri("/path/to/module1.txt")] = "param foo string",
                [InMemoryFileResolver.GetFileUri("/path/to/module2.bicep")] = "param bar bool",
                [InMemoryFileResolver.GetFileUri("/path/to/module3.bicep")] = "",
            };

            var fileResolver = new InMemoryFileResolver(fileTextsByUri);

            using var helper = await LanguageServerHelper.StartServerWithTextAsync(
                TestContext,
                mainFileText,
                mainUri,
                creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), FileResolver: fileResolver));

            var file = new FileRequestHelper(helper.Client, mainFile);

            var completions = await file.RequestCompletion(cursor);

            var completionItems = completions.Where(x => x.Kind == CompletionItemKind.File).OrderBy(x => x.SortText);
            if (jsonOnTop)
            {
                completionItems.Should().SatisfyRespectively(
                    x => x.Label.Should().Be("json1.json"),
                    x => x.Label.Should().Be("json2.json"),
                    x => x.Label.Should().Be("template2.json"),
                    x => x.Label.Should().Be("template3.jsonc"),
                    x => x.Label.Should().Be("template4.json"),
                    x => x.Label.Should().Be("template5.json"),
                    x => x.Label.Should().Be("module1.txt"),
                    x => x.Label.Should().Be("module2.bicep"),
                    x => x.Label.Should().Be("module3.bicep"),
                    x => x.Label.Should().Be("template1.arm")
                );
            }
            else
            {
                completionItems.Should().SatisfyRespectively(
                    x => x.Label.Should().Be("json1.json"),
                    x => x.Label.Should().Be("json2.json"),
                    x => x.Label.Should().Be("module1.txt"),
                    x => x.Label.Should().Be("module2.bicep"),
                    x => x.Label.Should().Be("module3.bicep"),
                    x => x.Label.Should().Be("template1.arm"),
                    x => x.Label.Should().Be("template2.json"),
                    x => x.Label.Should().Be("template3.jsonc"),
                    x => x.Label.Should().Be("template4.json"),
                    x => x.Label.Should().Be("template5.json")
                    );
            }
        }

        [DataTestMethod]
        [DataRow("loadTextContent")]
        [DataRow("loadFileAsBase64")]
        [DataRow("loadJsonContent", true)]
        public async Task LoadFunctionsPathArgument_returnsSymbolsAndFilePathsInCompletions(string functionName, bool jsonOnTop = false)
        {
            var mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");

            var (mainFileText, cursor) = ParserHelper.GetFileWithSingleCursor(@"
var template = 'template1.json'
var file = " + functionName + @"(templ|)
");
            var mainFile = SourceFileFactory.CreateBicepFile(mainUri, mainFileText);
            var schema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";

            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [mainUri] = mainFileText,
                [InMemoryFileResolver.GetFileUri("/path/to/template1.arm")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/template2.json")] = @$"{{ ""schema"": ""{schema}"" }}",
                [InMemoryFileResolver.GetFileUri("/path/to/template3.jsonc")] = @"{}",
                [InMemoryFileResolver.GetFileUri("/path/to/template4.json")] = new string('x', 2000 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/template5.json")] = new string('x', 2002 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/json1.json")] = "{}",
                [InMemoryFileResolver.GetFileUri("/path/to/json2.json")] = @"[{ ""name"": ""value"" }]",
                [InMemoryFileResolver.GetFileUri("/path/to/module1.txt")] = "param foo string",
                [InMemoryFileResolver.GetFileUri("/path/to/module2.bicep")] = "param bar bool",
                [InMemoryFileResolver.GetFileUri("/path/to/module3.bicep")] = "",
            };

            var fileResolver = new InMemoryFileResolver(fileTextsByUri);

            using var helper = await LanguageServerHelper.StartServerWithTextAsync(
                TestContext,
                mainFileText,
                mainUri,
                creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), FileResolver: fileResolver));
            var file = new FileRequestHelper(helper.Client, mainFile);

            var completions = await file.RequestCompletion(cursor);

            var completionItems = completions.OrderBy(x => x.SortText).Where(x => x.Label.StartsWith("templ"));
            if (jsonOnTop)
            {
                completionItems.Should().SatisfyRespectively(
                    x => x.Label.Should().Be("template2.json"),
                    x => x.Label.Should().Be("template3.jsonc"),
                    x => x.Label.Should().Be("template4.json"),
                    x => x.Label.Should().Be("template5.json"),
                    x => x.Label.Should().Be("template"),
                    x => x.Label.Should().Be("template1.arm")
                );
            }
            else
            {
                completionItems.Should().SatisfyRespectively(
                    x => x.Label.Should().Be("template1.arm"),
                    x => x.Label.Should().Be("template2.json"),
                    x => x.Label.Should().Be("template3.jsonc"),
                    x => x.Label.Should().Be("template4.json"),
                    x => x.Label.Should().Be("template5.json"),
                    x => x.Label.Should().Be("template")
                );
            }
        }

        [DataTestMethod]
        [DataRow("module foo |", "../", "module foo '../|'")]
        [DataRow("module foo |", "other.bicep", "module foo 'other.bicep'|")]
        [DataRow("module foo .|", "../", "module foo '../|'")]
        [DataRow("module foo '.'|", "../", "module foo '../|'")]
        [DataRow("module foo ./|", "../", "module foo '../|'")]
        [DataRow("module foo ./|", "other.bicep", "module foo 'other.bicep'|")]
        [DataRow("module foo './|'", "other.bicep", "module foo './other.bicep'|")]
        [DataRow("module foo ../|", "../", "module foo '../|'")]
        [DataRow("module foo '../'|", "../", "module foo '../../|'")]
        [DataRow("module foo '../../|'", "path2/", "module foo '../../path2/|'")]
        [DataRow("module foo '../../../|'", "path2/", "module foo '../../path2/|'")]
        [DataRow("module foo ot|h", "other.bicep", "module foo 'other.bicep'|")]
        [DataRow("module foo |oth", "other.bicep", "module foo 'other.bicep'|")]
        [DataRow("module foo oth|", "other.bicep", "module foo 'other.bicep'|")]
        [DataRow("module foo 'ot|h'", "other.bicep", "module foo 'other.bicep'|")]
        [DataRow("module foo '../to2/|'", "main.bicep", "module foo '../to2/main.bicep'|")]
        public async Task Module_path_completions_are_offered(string fileWithCursors, string expectedLabel, string expectedResult)
        {
            var fileUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");
            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string> {
                [InMemoryFileResolver.GetFileUri("/path/to/other.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to2/main.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path2/to/main.bicep")] = "",
            });

            using var helper = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext, new LanguageServer.Server.CreationOptions {
                FileResolver = fileResolver,
            });

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors);
            var file = await new ServerRequestHelper(TestContext, helper).OpenFile(fileUri, text);

            var completions = await file.RequestCompletion(cursor);

            completions.Should().Contain(x => x.Label == expectedLabel, $"\"{fileWithCursors}\" should have completion");
            var updatedFile = file.ApplyCompletion(completions, expectedLabel);
            updatedFile.Should().HaveSourceText(expectedResult);
        }
    }
}
