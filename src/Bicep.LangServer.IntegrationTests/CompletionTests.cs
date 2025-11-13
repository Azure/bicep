// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Text;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.Parsing;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Samples;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Mock.Registry;
using Bicep.Core.UnitTests.Mock.Registry.Catalog;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.FileSystem;
using Bicep.LangServer.IntegrationTests.Assertions;
using Bicep.LangServer.IntegrationTests.Completions;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using CompilationHelper = Bicep.Core.UnitTests.Utils.CompilationHelper;
using LocalFileSystem = System.IO.Abstractions.FileSystem;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.IntegrationTests.Completions
{
    [TestClass]
    public class CompletionTests
    {
        private static ServiceBuilder Services => new();

        private static readonly SharedLanguageHelperManager ServerWithNamespaceProvider = new();

        private static readonly SharedLanguageHelperManager ServerWithNamespaceAndTestResolver = new();

        private static readonly SharedLanguageHelperManager DefaultServer = new();

        private static readonly SharedLanguageHelperManager ServerWithBuiltInTypes = new();

        private static readonly SharedLanguageHelperManager ServerWithResourceTypedParamsEnabled = new();

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
            ServerWithNamespaceProvider.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext));

            var settingsProvider = StrictMock.Of<ISettingsProvider>();
            ServerWithNamespaceAndTestResolver.Initialize(
                async () => await MultiFileLanguageServerHelper.StartLanguageServer(
                    testContext,
                    services => services.AddSingleton<ISettingsProvider>(settingsProvider.Object)));

            DefaultServer.Initialize(async () => await MultiFileLanguageServerHelper.StartLanguageServer(testContext));

            ServerWithBuiltInTypes.Initialize(
                async () => await MultiFileLanguageServerHelper.StartLanguageServer(
                    testContext,
                    services => services.WithNamespaceProvider(BuiltInTestTypes.Create())));

            ServerWithResourceTypedParamsEnabled.Initialize(
                async () => await MultiFileLanguageServerHelper.StartLanguageServer(
                    testContext,
                    services => services.WithFeatureOverrides(new(testContext, ResourceTypedParamsAndOutputsEnabled: true))
                        .WithNamespaceProvider(BuiltInTestTypes.Create())));
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await ServerWithNamespaceProvider.DisposeAsync();
            await ServerWithNamespaceAndTestResolver.DisposeAsync();
            await DefaultServer.DisposeAsync();
            await ServerWithBuiltInTypes.DisposeAsync();
            await ServerWithResourceTypedParamsEnabled.DisposeAsync();
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
            (bicepContents, var cursor) = ParserHelper.GetFileWithSingleCursor(bicepContents, "// Insert snippet here");

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
                    expectedPath: combinedSourceFileName,
                    actualPath: combinedFileName + ".actual");
            }
        }

        private static IEnumerable<object[]> GetSnippetCompletionData() => CompletionDataHelper.GetSnippetCompletionData();

        private async Task<string> RequestSnippetCompletion(string bicepFileName, CompletionData completionData, string placeholderText, int cursor)
        {
            var documentUri = DocumentUri.FromFileSystemPath(bicepFileName);
            var bicepFile = new LanguageClientFile(documentUri, placeholderText);

            var helper = await ServerWithNamespaceProvider.GetAsync();
            await helper.OpenFileOnceAsync(this.TestContext, placeholderText, documentUri);

            var completions = await helper.Client.RequestCompletion(new CompletionParams
            {
                TextDocument = documentUri,
                Position = bicepFile.GetPosition(cursor),
            });

            var matchingSnippets = completions.Where(x => x.Kind == CompletionItemKind.Snippet && x.Label == completionData.Prefix);

            matchingSnippets.Should().HaveCount(1);
            var completion = matchingSnippets.First();

            completion.TextEdit.Should().NotBeNull();
            completion.TextEdit!.TextEdit!.Range.ToTextSpan(bicepFile.LineStarts).Should().Be(new TextSpan(cursor, 0));
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

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty, '|');
        }

        [TestMethod]
        public async Task Completions_are_offered_in_string_expressions()
        {
            var fileWithCursors = @"
var interpolatedString = 'abc${|true}def${|}ghi${res|}xyz'
";

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsNonEmpty, '|');
        }

        [TestMethod]
        public async Task Completions_are_offered_immediately_before_and_after_comments()
        {
            var fileWithCursors = @"
var test = |// comment here
var test2 = |/* block comment */|
";

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsNonEmpty, '|');
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

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty, '|');
        }

        [TestMethod]
        public async Task Documentation_should_be_shown_for_bicep_symbol_completions()
        {
            var bicepTextWithCursor = @"
@allowed(
    [
        0
        1
    ]
)
@description('this is an int value')
param myInt int

@allowed(
    [
        'value1'
        'value2'
    ]
)
@description('this is a string value')
param myStr string

@description('this is a bool value')
param myBool bool

param myArray array

@description('this is a variable')
var myVar = 'foobar'

@description('this is a storage account resource')
resource storageAct 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'foorbar'
  location: 'westus2'
  sku: {
    name: 'standard_LRS'
  }
  kind: 'StorageV2'
}

@description('this is a bicep module')
module myMod './myMod.bicep' = {
  name: 'myModule'
}

resource service 'Microsoft.Storage/storageAccounts/fileServices@2021-02-01' = {
  name: 'default'
  parent:  |
}
";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(bicepTextWithCursor, '|');
            DocumentUri mainUri = DocumentUri.From("file:///main.bicep");
            var files = new Dictionary<DocumentUri, string>
            {
                [DocumentUri.From("file:///myMod.bicep")] = "",
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri
            );

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestAndResolveCompletions(cursor);

            var symbolCompletions = completions.Items.Where(x => x.Kind == CompletionItemKind.Field ||
                                                                 x.Kind == CompletionItemKind.Variable ||
                                                                 x.Kind == CompletionItemKind.Interface ||
                                                                 x.Kind == CompletionItemKind.Module);

            symbolCompletions.Should().SatisfyRespectively(
              x =>
              {
                  x.Label.Should().Be("myInt");
                  x.Kind.Should().Be(CompletionItemKind.Field);
                  x.Documentation!.MarkupContent!.Value.Should().Be("Type: `0 | 1`  \nthis is an int value  \n");
              },
              x =>
              {
                  x.Label.Should().Be("myStr");
                  x.Kind.Should().Be(CompletionItemKind.Field);
                  x.Documentation!.MarkupContent!.Value.Should().Be("Type: `'value1' | 'value2'`  \nthis is a string value  \n");
              },
              x =>
              {
                  x.Label.Should().Be("myBool");
                  x.Kind.Should().Be(CompletionItemKind.Field);
                  x.Documentation!.MarkupContent!.Value.Should().Be("Type: `bool`  \nthis is a bool value  \n");
              },
              x =>
              {
                  x.Label.Should().Be("myArray");
                  x.Kind.Should().Be(CompletionItemKind.Field);
                  x.Documentation!.MarkupContent!.Value.Should().Be("Type: `array`  \n");
              },
              x =>
              {
                  x.Label.Should().Be("myVar");
                  x.Kind.Should().Be(CompletionItemKind.Variable);
                  x.Documentation!.MarkupContent!.Value.Should().Be("this is a variable  \n");
              },
              x =>
              {
                  x.Label.Should().Be("storageAct");
                  x.Kind.Should().Be(CompletionItemKind.Interface);
                  x.Documentation!.MarkupContent!.Value.Should().Be("this is a storage account resource  \n");
              },
              x =>
              {
                  x.Label.Should().Be("myMod");
                  x.Kind.Should().Be(CompletionItemKind.Module);
                  x.Documentation!.MarkupContent!.Value.Should().Be("this is a bicep module  \n");
              }
            );
        }

        [TestMethod]
        public async Task Completions_are_not_offered_inside_comments()
        {
            var fileWithCursors = @"
var test = /|/ comment here|
var test2 = /|* block c|omment *|/
";

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty, '|');
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

                    var snippetCompletions = completionLists.First().Items.Where(x => x.Kind == CompletionItemKind.Snippet);

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
                },
                '|');
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

                    var snippetCompletion = completionLists.First().Items.Where(x => x.Kind == CompletionItemKind.Snippet && x.Label == "snippet");

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
                },
                '|');
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

                    var snippetCompletion = completionLists.First().Items.Where(x => x.Kind == CompletionItemKind.Snippet && x.Label == "res-automation-cred");

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
                },
                '|');
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
        public async Task VerifyNullablePropertiesAreNotLabeledRequired()
        {
            var fileWithCursors = @"
module mod 'mod.bicep' = {
  name: 'mod'
  params: {
    foo: {
      |
    }
  }
}
";

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = @"param foo {
  requiredProperty: string
  optionalProperty: string?
}",
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestCompletions(cursors);
            completions.Count().Should().Be(1);
            completions.Single().OrderBy(c => c.SortText).Should().SatisfyRespectively(
              x => x.Detail.Should().Be("requiredProperty (Required)"),
              x => x.Detail.Should().Be("optionalProperty"));
        }

        [TestMethod]
        public async Task VerifyNullablePropertiesAreNotIncludedInRequiredPropertiesCompletion()
        {
            var fileWithCursors = @"
module mod 'mod.bicep' = {
  name: 'mod'
  params: |
}
";

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = @"param foo {
  requiredProperty: string
  optionalProperty: string?
}",
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestCompletions(cursors);
            completions.Count().Should().Be(1);

            var withRequiredProps = file.ApplyCompletion(completions.Single(), "required-properties").Text;
            withRequiredProps.Should().Contain("requiredProperty");
            withRequiredProps.Should().NotContain("optionalProperty");
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
                            d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which is required."))),
                '|');
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
                            d => d.Documentation!.MarkupContent!.Value.Should().Contain("This is a property which is required."))),
                '|');
        }

        [TestMethod]
        public async Task Nonnull_assertion_operator_unwraps_union_with_null()
        {
            var fileWithCursors = @"
param foos (null | { bar: { baz: { quux: 'quux' } } })[]

var bar = foos[0]!.ǂ
var baz = foos[0]!.bar.ǂ
var quux = foos[0]!.bar.baz.ǂ
";

            await RunCompletionScenarioTest(
                this.TestContext,
                DefaultServer,
                fileWithCursors,
                completions =>
                    completions.Should().SatisfyRespectively(
                        d => d.Single().Label.Should().Be("bar"),
                        d => d.Single().Label.Should().Be("baz"),
                        d => d.Single().Label.Should().Be("quux")),
                'ǂ');
        }

        [TestMethod]
        public async Task Property_completions_acknowledge_nullability()
        {
            var fileWithCursors = @"
param foos (null | { bar: { baz: { quux: 'quux' } } })[]

var bar = foos[?0].ǂ
var baz = foos[?0].bar.ǂ
var quux = foos[?0].bar.baz.ǂ
";

            await RunCompletionScenarioTest(
                this.TestContext,
                DefaultServer,
                fileWithCursors,
                completions =>
                    completions.Should().SatisfyRespectively(
                        d => d.Single().Label.Should().Be("bar"),
                        d => d.Single().Label.Should().Be("baz"),
                        d => d.Single().Label.Should().Be("quux")),
                'ǂ');
        }

        [TestMethod]
        public async Task Completions_are_offered_within_values_with_a_nullable_declared_type()
        {
            var module = @"
type myObject = {
  requiredProperty: string
  optionalProperty: string?
}

param nullableArray myObject[]?

param arrayOfNullables (myObject?)[]

param nullableObject myObject?

param withNullableObjectProperty {
  nullableProperty: myObject?
}
";
            var fileWithCursors = @"
module mod 'mod.bicep' = {
  name: 'mod'
  params: {
    nullableArray: [
      |
    ]
    arrayOfNullables: [
      |
    ]
    nullableObject: |
    withNullableObjectProperty: {
      nullableProperty: |
    }
  }
}
";

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = module,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestCompletions(cursors);

            // despite being in a location with a nullable type or nested within a nullable type, each cursor should be recognized as accepting a typed object and therefore offer the "required-properties" snippet as a completion
            completions.Should().AllSatisfy(y => y.Any(x => x.Label == "required-properties").Should().BeTrue());
        }

        [TestMethod]
        public async Task Completions_are_offered_within_values_with_an_interpolated_property_name()
        {
            var module = @"
param properties {
  *: {
    nestedProperty: string
  }
}
";
            var fileWithCursors = @"
param unknownValue string
var knownValue = 'fizz'

module mod 'mod.bicep' = {
  name: 'mod'
  params: {
    properties: {
      foo: {
        |
      }
      '${knownValue}': {
        |
      }
      '${unknownValue}': {
        |
      }
    }
  }
}
";

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = module,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestCompletions(cursors);

            completions.Should().AllSatisfy(y => y.Any(x => x.Label == "nestedProperty").Should().BeTrue());
        }

        [TestMethod]
        public async Task Completions_are_not_offered_within_values_with_an_ambiguous_interpolated_property_name()
        {
            var module = @"
param properties {
  foo: {
    bar: string
  }
  *: {
    nestedProperty: string
  }
}
";
            var fileWithCursors = @"
param unknownValue string
var knownValue = 'fizz'

module mod 'mod.bicep' = {
  name: 'mod'
  params: {
    properties: {
      foo: {
        |
      }
      '${knownValue}': {
        |
      }
      '${unknownValue}': {
        |
      }
    }
  }
}
";

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors);
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = module,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestCompletions(cursors);

            completions.Should().SatisfyRespectively(
              y => y.Any(x => x.Label == "bar").Should().BeTrue(),
              y => y.Any(x => x.Label == "nestedProperty").Should().BeTrue(),
              y => y.Any().Should().BeFalse());
        }

        [TestMethod]
        public async Task Item_completions_are_offered_for_array_typed_parameter_default_values()
        {
            var fileWithCursors = """
                type fizz = {
                  buzz: string
                  pop: string
                }

                param fizzes fizz[] = [
                  |
                ]
                """;

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);

            var updatedFile = file.ApplyCompletion(completions, "required-properties");
            updatedFile.Should().HaveSourceText("""
                type fizz = {
                  buzz: string
                  pop: string
                }

                param fizzes fizz[] = [
                  {
                  buzz: $1
                  pop: $2
                }|
                ]
                """);
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
                        x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(d => AssertExistingKeywordCompletion(d))),
                '|');
        }

        [TestMethod]
        public async Task Partial_identifier_resource_type_completions_work()
        {
            {
                var fileWithCursors = @"
resource myRes Te|st
";

                var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
                var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

                var completions = await file.RequestAndResolveCompletions(cursor);

                var updatedFile = file.ApplyCompletion(completions, "'Test.Rp/basicTests'");
                updatedFile.Should().HaveSourceText(@"
resource myRes 'Test.Rp/basicTests@|'
");
            }
        }

        [TestMethod]
        public async Task Partial_string_resource_type_completions_work()
        {
            {
                var fileWithCursors = @"
resource myRes 'Test.Rp/ba|si
";

                var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
                var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

                var completions = await file.RequestAndResolveCompletions(cursor);

                var updatedFile = file.ApplyCompletion(completions, "'Test.Rp/basicTests'");
                updatedFile.Should().HaveSourceText(@"
resource myRes 'Test.Rp/basicTests@|'
");
            }

            {
                var fileWithCursors = @"
resource myRes 'Test.Rp/basicTests@|
";

                var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
                var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

                var completions = await file.RequestAndResolveCompletions(cursor);

                var updatedFile = file.ApplyCompletion(completions, "2020-01-01");
                updatedFile.Should().HaveSourceText(@"
resource myRes 'Test.Rp/basicTests@2020-01-01'|
");
            }
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
                            )),
                '|');
        }

        [TestMethod]
        public async Task OutputTypeFollowerWithoCompletionsOffersEquals()
        {
            var fileWithCursors = @"
output test string |
";

            static void AssertEqualsOperatorCompletion(CompletionItem item)
            {
                item.Label.Should().Be("=");
                item.Documentation.Should().BeNull();
                item.Kind.Should().Be(CompletionItemKind.Operator);
                item.Preselect.Should().BeTrue();
                item.TextEdit!.TextEdit!.NewText.Should().Be("=");
                item.CommitCharacters.Should().BeNull();
            }

            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithBuiltInTypes,
                fileWithCursors,
                completions =>
                    completions.Should().SatisfyRespectively(
                        x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                            d => AssertEqualsOperatorCompletion(d)
                        )),
                '|');
        }

        [TestMethod]
        public async Task TypeDrivenCompletionsAreOfferedInParameterAndOutputValues()
        {
            var fileWithCursors = """
                type bigObject = {
                  foo: {
                    bar: {
                      baz: bool
                    }
                  }
                  fizz: {
                    buzz: {
                      pop: 'snap' | 'crackle'
                    }
                  }
                }

                param p bigObject = {
                  ǂ
                }

                param p2 bigObject = {
                  foo: {
                    ǂ
                  }
                }

                param p3 bigObject = {
                  foo: {
                    bar: {
                      ǂ
                    }
                  }
                }

                param p4 bigObject = {
                  foo: {
                    bar: {
                      baz: ǂ
                    }
                  }
                }

                output o bigObject = {
                  fizz: {
                    buzz: {
                      pop: ǂ
                    }
                  }
                }
                """;

            await RunCompletionScenarioTest(
                this.TestContext,
                DefaultServer,
                fileWithCursors,
                completions =>
                    completions.Should().SatisfyRespectively(
                        x => x!.OrderBy(d => d.SortText).Should().SatisfyRespectively(
                            d => d.Label.Should().Be("fizz"),
                            d => d.Label.Should().Be("foo")
                        ),
                        x => x!.Should().SatisfyRespectively(
                            d => d.Label.Should().Be("bar")
                        ),
                        x => x!.Should().SatisfyRespectively(
                            d => d.Label.Should().Be("baz")
                        ),
                        x => x!.Should().Contain(d => d.Label == "false"),
                        x => x!.Should().Contain(d => d.Label == "'crackle'")),
                'ǂ');
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
                    l => AssertPropertyNameCompletionsWithColons(l!)),
                '|');
        }

        [TestMethod]
        public Task Spread_operator_supports_outer_object_property_completions() => RunCompletionTest("""
type myType = {
  foo: string
  bar: string
}

output foo myType = {
  ...{|}
  bar: 'bar'
}
""",
          "foo", """
type myType = {
  foo: string
  bar: string
}

output foo myType = {
  ...{foo:|}
  bar: 'bar'
}
""");

        [TestMethod] // https://github.com/Azure/bicep/issues/14056
        public Task Spread_object_property_completions_work_with_ternary() => RunCompletionTest("""
var nsgDeploy = true

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2023-11-01' = {
  name: 'vnet/subnet2'
  properties: {
    addressPrefix: ''
    ...nsgDeploy ? {
      |
    }
  }
}
""",
          "ipAllocations", """
var nsgDeploy = true

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2023-11-01' = {
  name: 'vnet/subnet2'
  properties: {
    addressPrefix: ''
    ...nsgDeploy ? {
      ipAllocations:|
    }
  }
}
""");

        [TestMethod]
        public Task Spread_array_completions_work_with_parentheses() => RunCompletionTest("""
param foo { foo: 'asdf' }[] = [
  ...[
   (|)
  ]
]
""",
          "required-properties", """
param foo { foo: 'asdf' }[] = [
  ...[
   ({
  foo: $1
}|)
  ]
]
""");

        [TestMethod] // https://github.com/Azure/bicep/issues/14066
        public Task Object_completions_work_inside_ternary() => RunCompletionTest("""
var foo1 = {}
var foo2 = []

resource str 'Microsoft.Storage/storageAccounts@2023-04-01' = {
  name: 'str0909'
  properties: {
    networkAcls: empty(foo1) ? null : {
      defaultAction: 'Allow'
      resourceAccessRules: empty(foo2) ? null : [
        {
          resourceId: ''
          |
        }
      ]
    }
  }
}
""",
          "tenantId", """
var foo1 = {}
var foo2 = []

resource str 'Microsoft.Storage/storageAccounts@2023-04-01' = {
  name: 'str0909'
  properties: {
    networkAcls: empty(foo1) ? null : {
      defaultAction: 'Allow'
      resourceAccessRules: empty(foo2) ? null : [
        {
          resourceId: ''
          tenantId:|
        }
      ]
    }
  }
}
""");

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
                       l => AssertPropertyNameCompletionsWithoutColons(l!)),
                '|');
        }

        [TestMethod]
        public async Task RequestCompletionsInResourceBodies_AtPositionsWhereNodeShouldNotBeInserted_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
resource myRes 'Test.Rp/readWriteTests@2020-01-01' = {
 | name: 'myRes' |
  tags | : {
    a: 'A'   |
  }
}
";
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty, '|');
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

            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsContainResourceLabel, '|');
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
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty, '|');
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
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty, '|');
        }

        [TestMethod]
        public async Task RequestCompletionsInArrays_AtPositionsWhereNodeShouldNotBeInserted_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
var arr1 = []|
var arr2 = |[]
var arr3 = [|
  | null |
|]
var arr4 = [ |
  12345
  |  true
| ]
";
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty, '|');
        }

        [TestMethod]
        public async Task RequestCompletionsInExpressions_AtPositionsWhereNodeShouldNotBeInserted_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
var unary = |! | true
var binary = -1 | |+| | 2
var ternary = true | |?| | 'yes' | |:| | 'no'
";
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty, '|');
        }

        [TestMethod]
        public async Task RequestCompletionsInTopLevelDeclarations_AtPositionsWhereNodeShouldNotBeInserted_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
|param foo string
v|ar expr = 1 + 2
";
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty, '|');
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
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty, '|');
        }


        [TestMethod]
        public async Task RequestCompletions_MatchingNodeIsBooleanOrIntegerOrNullLiteral_ReturnsEmptyCompletions()
        {
            var fileWithCursors = @"
var booleanExp = !|tr|ue| && |fal|se|
var integerExp = |12|345| + |543|21|
var nullLit = |n|ull|
";
            await RunCompletionScenarioTest(this.TestContext, ServerWithBuiltInTypes, fileWithCursors, AssertAllCompletionsEmpty, '|');
        }

        [TestMethod]
        public async Task RequestingCompletionsForLoopBodyShouldReturnNonLoopCompletions()
        {
            var fileWithCursors = @"
resource foo 'Microsoft.AgFoodPlatform/farmBeats@2020-05-12-preview' = [for item in list: |]

module bar 'doesNotExist.bicep' = [for item in list:|]

module bar2 'test.bicep' = [for item in list: |  ]
";

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///test.bicep"] = @"param foo string",
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.Uri, services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));

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
            DocumentUri mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");
            var (mainFileText, cursor) = ParserHelper.GetFileWithSingleCursor(@"
module mod1 './module1.txt' = {}
module mod2 './template3.jsonc' = {}
module mod2 './|' = {}
",
                '|');
            var mainFile = new LanguageClientFile(mainUri, mainFileText);
            var schema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";

            var fileTextsByUri = new Dictionary<DocumentUri, string>
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

            using var helper = await LanguageServerHelper.StartServerWithText(
                TestContext,
                fileTextsByUri,
                mainUri,
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));

            var file = new FileRequestHelper(helper.Client, mainFile);

            var completions = await file.RequestAndResolveCompletions(cursor);
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

                    var snippetCompletions = completionLists.First().Items.Where(x => x.Kind == CompletionItemKind.Snippet);

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
                },
                '|');
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

                    var snippetCompletions = completionLists.First().Items.Where(x => x.Kind == CompletionItemKind.Snippet);

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
                },
                '|');
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

                    var snippetCompletions = completionLists.First().Items.Where(x => x.Kind == CompletionItemKind.Snippet);

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
                },
                '|');
        }

        [TestMethod]
        public async Task Extension_completions_work()
        {

            var fileWithCursors = @"
            |
            extension ns1 |
            extension ns2 a|
            extension ns3 as|
            extension |
            extension a|
            ";
            await RunCompletionScenarioTest(this.TestContext, DefaultServer, fileWithCursors,
                completions => completions.Should().SatisfyRespectively(
                    c => c!.Select(x => x.Label).Should().Contain("extension"),
                    c => c!.Select(x => x.Label).Should().Equal("with", "as"),
                    c => c!.Select(x => x.Label).Should().Equal("with", "as"),
                    c => c!.Select(x => x.Label).Should().BeEmpty(),
                    c => c!.Select(x => x.Label).Should().Equal($"az", "kubernetes", "sys"),
                    c => c!.Select(x => x.Label).Should().Equal($"az", "kubernetes", "sys")
                ),
                '|');
        }

        [TestMethod]
        public async Task Extension_configuration_completions_work()
        {
            {
                var fileWithCursors = @"
extension kubernetes with | as k8s
";

                var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
                var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

                var completions = await file.RequestAndResolveCompletions(cursor);
                completions.Should().Contain(x => x.Label == "{}");
                completions.Should().Contain(x => x.Label == "required-properties");

                var updatedFile = file.ApplyCompletion(completions, "required-properties");
                updatedFile.Should().HaveSourceText(@"
extension kubernetes with {
  kubeConfig: $1
  namespace: $2
}| as k8s
");
            }

            {
                var fileWithCursors = @"
extension kubernetes with {
  |
}
";

                var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
                var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

                var completions = await file.RequestAndResolveCompletions(cursor);
                completions.Should().Contain(x => x.Label == "namespace");
                completions.Should().Contain(x => x.Label == "kubeConfig");

                var updatedFile = file.ApplyCompletion(completions, "kubeConfig");
                updatedFile.Should().HaveSourceText(@"
extension kubernetes with {
  kubeConfig:|
}
");
            }
        }

        [TestMethod]
        public async Task TypeCompletionsIncludeAmbientTypes()
        {
            var fileWithCursors = @"
type a = ǂ
";
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, 'ǂ');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(x => x.Label == "string");
            completions.Should().Contain(x => x.Label == "int");
            completions.Should().Contain(x => x.Label == "bool");
            completions.Should().Contain(x => x.Label == "object");
            completions.Should().Contain(x => x.Label == "array");
        }

        [TestMethod]
        public async Task TypeCompletionsIncludeUserDefinedTypes()
        {
            var fileWithCursors = @"
type a = ǂ
type b = string
";
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, 'ǂ');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(x => x.Label == "b");
        }

        [TestMethod]
        public async Task UnionTypeMembersShouldReceiveTypeCompletions()
        {
            var fileWithCursors = @"
type a = 'fizz'|'buzz'|ǂ
type b = 'pop'
type c = ['foo', 'bar']
type d = {
  key: 'a'|'union'|'of'|'values'
}
";
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, 'ǂ');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(x => x.Label == "b");
            completions.Should().Contain(x => x.Label == "c");
            completions.Should().Contain(x => x.Label == "d");
        }

        [TestMethod]
        public async Task UnionTypeMembersWithSeparationShouldReceiveTypeCompletions()
        {
            var fileWithCursors = @"
type a = 'fizz' | 'buzz' | ǂ
type b = 'pop'
";
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, 'ǂ');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(x => x.Label == "b");
        }

        [TestMethod]
        public async Task UnionTypeMemberCompletionsShouldNotIncludeNonLiteralTypes()
        {
            var fileWithCursors = @"
type a = 'fizz'|'buzz'|ǂ
type b = 'pop'
type c = string[]
type d = [1, 2, int]
type e = {
  key: 'value'
  anotherKey: string
}
";
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, 'ǂ');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(x => x.Label == "b");
            completions.Should().NotContain(x => x.Label == "c");
            completions.Should().NotContain(x => x.Label == "d");
            completions.Should().NotContain(x => x.Label == "e");
            completions.Should().NotContain(x => x.Label == "string");
            completions.Should().NotContain(x => x.Label == "int");
            completions.Should().NotContain(x => x.Label == "bool");
            completions.Should().NotContain(x => x.Label == "object");
            completions.Should().NotContain(x => x.Label == "array");
        }

        [TestMethod]
        public async Task TypeCompletionsShouldNotIncludeCyclicReferences()
        {
            var fileWithCursors = @"
type a = ǂ
";
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, 'ǂ');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().NotContain(x => x.Label == "a");
        }

        [TestMethod]
        public async Task UnionTypeMemberCompletionsShouldNotIncludeCyclicReferences()
        {
            var fileWithCursors = @"
type a = 'fizz'|'buzz'|ǂ
";
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, 'ǂ');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().NotContain(x => x.Label == "a");
        }

        [TestMethod]
        public async Task UnaryOperationTypeCompletionsShouldNotIncludeCyclicReferences()
        {
            var fileWithCursors = @"
type a = -ǂ
";
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, 'ǂ');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().NotContain(x => x.Label == "a");
        }

        [TestMethod]
        public async Task ParameterTypeCompletionsShouldIncludeUserDefinedTypes()
        {
            var fileWithCursors = @"
type myString = string
param stringParam |
";
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(x => x.Label == "myString");
        }

        [TestMethod]
        public async Task OutputTypeCompletionsShouldIncludeUserDefinedTypes()
        {
            var fileWithCursors = @"
type myString = string
output stringOutput |
";
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(x => x.Label == "myString");
        }

        [TestMethod]
        public async Task TypePropertyDecoratorsShouldAlignWithPropertyType()
        {
            var fileWithCursors = """
                type obj = {
                  @|
                  intProp: int
                }
                """;
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);

            completions.Should().Contain(x => x.Label == "description");
            completions.Should().Contain(x => x.Label == "metadata");
            completions.Should().Contain(x => x.Label == "minValue");
            completions.Should().Contain(x => x.Label == "maxValue");

            completions.Should().NotContain(x => x.Label == "sealed");
            completions.Should().NotContain(x => x.Label == "secure");
            completions.Should().NotContain(x => x.Label == "discriminator");
        }

        [TestMethod]
        public async Task ModuleCompletionsShouldNotBeUrlEscaped()
        {
            var fileWithCursors = @"
module a '|' = {
    name: 'modA'
}
";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            DocumentUri mainUri = InMemoryFileResolver.GetFileUri("/dir/main.bicep");
            var files = new Dictionary<DocumentUri, string>
            {
                [InMemoryFileResolver.GetFileUri("/dir/folder with space/mod with space.bicep")] = @"param foo string",
                [InMemoryFileResolver.GetFileUri("/dir/percentage%file.bicep")] = @"param foo string",
                [InMemoryFileResolver.GetFileUri("/dir/already%20escaped.bicep")] = @"param foo string",
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(this.TestContext, files, bicepFile.Uri, services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestAndResolveCompletions(cursor);

            completions.OrderBy(x => x.SortText).Should().SatisfyRespectively(
                x => x.Label.Should().Be("already%20escaped.bicep"),
                x => x.Label.Should().Be("percentage%file.bicep"),
                x => x.Label.Should().Be("../"),
                x => x.Label.Should().Be("folder with space/"),
                x => x.Label.Should().Be("br/public:"),
                x => x.Label.Should().Be("ts/"),
                x => x.Label.Should().Be("br:"),
                x => x.Label.Should().Be("ts:")
            );
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

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = moduleContent,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri,
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));

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
                            x => x.Label.Should().Be("2020-01-01"))),
                '|');
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
                            x => x.Label == "'Test.Rp/basicTests'")),
                '|');
        }

        [TestMethod]
        public async Task Resource_type_completions_for_MicrosoftApp_and_subtypes_use_common_keywords_in_filter()
        {
            var fileWithCursors = """
                resource r 'appservice|'
                """;

            await RunCompletionScenarioTest(TestContext, ServerWithNamespaceProvider, fileWithCursors, completionLists =>
            {
                var completionList = completionLists.Should().HaveCount(1).And.Subject.First();

                var microsoftApp = completionList.Where(completion => completion.Label.StartsWith("'microsoft.app/", StringComparison.InvariantCultureIgnoreCase)).ToArray();
                foreach (var completion in microsoftApp)
                {
                    completion.FilterText.Should().NotBeNull();
                    var filters = completion.FilterText!.Trim('\'').Split(' ');

                    filters.Where(x => x.StartsWith("microsoft.app", StringComparison.OrdinalIgnoreCase)).Should().HaveCount(1);
                    filters.Should().Contain("containerapp");
                }

                foreach (var completion in completionList.Except(microsoftApp))
                {
                    if (completion.FilterText is string filterText)
                    {
                        var filters = filterText.Trim('\'').Split(' ');

                        filters.Where(x => x.StartsWith("microsoft.app", StringComparison.OrdinalIgnoreCase)).Should().HaveCount(0);
                        filters.Should().NotContain("containerapp");
                    }
                }
            });
        }

        [TestMethod]
        public async Task Resource_type_completions_for_MicrosoftWebServerFarms_use_common_keywords_in_filter()
        {
            var fileWithCursors = """
                resource r 'appservice|'
                """;

            await RunCompletionScenarioTest(TestContext, ServerWithNamespaceProvider, fileWithCursors, completionLists =>
            {
                var completionList = completionLists.Should().HaveCount(1).And.Subject.First();

                // Everything under Microsoft.Web/serverFarms should have keywords "appservice", "webapp", "function" in filter text
                var serverFarms = completionList.Where(completion => completion.Label.StartsWith("'microsoft.web/serverfarms", StringComparison.InvariantCultureIgnoreCase)).ToArray();
                foreach (var completion in serverFarms)
                {
                    completion.FilterText.Should().NotBeNull();
                    var filters = completion.FilterText!.Trim('\'').Split(' ');

                    filters.Where(x => x.StartsWith("microsoft.web/serverfarms", StringComparison.OrdinalIgnoreCase)).Should().HaveCount(1);
                    filters.Should().Contain("appserviceplan");
                    filters.Should().Contain("asp");
                    filters.Should().Contain("hostingplan");
                }

                // Everything else under Microsoft.Web other than serverFarms should not have these keywords
                var webButNotServerFarms = completionList.Where(completion => completion.Label.StartsWith("'microsoft.web", StringComparison.InvariantCultureIgnoreCase)
                    && !completion.Label.Contains("serverfarms", StringComparison.InvariantCultureIgnoreCase)).ToArray();
                foreach (var completion in webButNotServerFarms)
                {
                    if (completion.FilterText is string filterText)
                    {
                        var filters = filterText.Trim('\'').Split(' ');

                        filters.Where(x => x.StartsWith("microsoft.web/serverfarms", StringComparison.OrdinalIgnoreCase)).Should().HaveCount(0);
                        filters.Should().NotContain("appserviceplan");
                        filters.Should().NotContain("asp");
                        filters.Should().NotContain("hostingplan");
                    }
                }

                // Everything not under Microsoft.Web should not have these keywords
                foreach (var completion in completionList.Except(serverFarms).Except(webButNotServerFarms))
                {
                    if (completion.FilterText is string filterText)
                    {
                        var filters = filterText.Trim('\'').Split(' ');

                        filters.Where(x => x.StartsWith("microsoft.web/serverfarms", StringComparison.OrdinalIgnoreCase)).Should().HaveCount(0);
                        filters.Should().NotContain("appserviceplan");
                        filters.Should().NotContain("asp");
                        filters.Should().NotContain("hostingplan");
                    }
                }
            });
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

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
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
        public async Task Known_list_functions_are_offered_on_resource_typed_params()
        {
            var fileWithCursors = """
                param ir resource 'Test.Rp/listFuncTests@2020-01-01'

                output authkeys string = ir.|
                """;

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithResourceTypedParamsEnabled).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(x => x.Label == "listNoInput");
            completions.Should().Contain(x => x.Label == "listWithInput");

            var updatedFile = file.ApplyCompletion(completions, "listWithInput");
            updatedFile.Should().HaveSourceText("""
                param ir resource 'Test.Rp/listFuncTests@2020-01-01'

                output authkeys string = ir.listWithInput(|)
                """);
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

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
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

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
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

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
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
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
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
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
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
                    ),
                '|'
);
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterPoundSign_ShouldReturnCompletionItem()
        {
            var fileWithCursors = @"#|
param storageAccount string = 'testAccount";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);

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

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors, '|');
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
",
                '|');

            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
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
",
                '|');

            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            var updatedFile = file.ApplyCompletion(completions, "(arg1, arg2) => ...", "foo", "bar");
            updatedFile.Should().HaveSourceText(@"
var foo = sort([123], (foo, bar) => |)
");
        }

        [TestMethod]
        public async Task Func_definition_lambda_completions_suggest_outer_variables()
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
var outerVar = 'asdf'

func foo(innerVar string) string => '${|}'
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(x => x.Label == "innerVar");
            completions.Should().Contain(x => x.Label == "outerVar");
            var updatedFile = file.ApplyCompletion(completions, "outerVar");
            updatedFile.Should().HaveSourceText("""
var outerVar = 'asdf'

func foo(innerVar string) string => '${outerVar|}'
""");
        }

        [TestMethod]
        public async Task Func_definition_lambda_completions_suggest_imported_variables()
        {
            var exportContent = """              
@export()
var whatsup = 'Whatsup?'
""";
            var mainContent = """
import { whatsup } from './exports.bicep'
func greet(name string) string => '${|}'
""";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(mainContent, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///exports.bicep"] = exportContent,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);

            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(c => c.Label == "whatsup");

            var updatedFile = file.ApplyCompletion(completions, "whatsup");
            updatedFile.Should().HaveSourceText("""
import { whatsup } from './exports.bicep'
func greet(name string) string => '${whatsup|}'
""");
        }

        [TestMethod]
        public async Task Func_definition_lambda_completions_can_suggest_other_funcs()
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
var outerVar = 'asdf'
func bar() string = 'asdf'

func foo(innerVar string) string => '${|}'
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(x => x.Label == "bar");
            var updatedFile = file.ApplyCompletion(completions, "bar");
            updatedFile.Should().HaveSourceText("""
var outerVar = 'asdf'
func bar() string = 'asdf'

func foo(innerVar string) string => '${bar()|}'
""");
        }

        [TestMethod]
        public async Task Func_keyword_completion_provides_snippet()
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(@"
f|
");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            var updatedFile = file.ApplyCompletion(completions, "func", "foo", "string");
            updatedFile.Should().HaveSourceText(@"
func foo() string => |
");
        }

        [DataTestMethod]
        [DataRow("func foo() | => 'blah'", "func foo() string| => 'blah'")]
        [DataRow("func foo() a| => 'blah'", "func foo() string| => 'blah'")]
        [DataRow("func foo() |a => 'blah'", "func foo() string| => 'blah'")]
        [DataRow("func foo() |", "func foo() string|")]
        [DataRow("func foo() a|", "func foo() string|")]
        [DataRow("func foo() |a", "func foo() string|")]
        public async Task Func_lambda_output_type_completions_only_suggest_types(string before, string after)
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor($"""
{before}
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            var updatedFile = file.ApplyCompletion(completions, "string");
            updatedFile.Should().HaveSourceText($"""
{after}
""");
        }

        [DataTestMethod]
        [DataRow("func foo(bar |) string => 'blah'", "func foo(bar string|) string => 'blah'")]
        [DataRow("func foo(bar a|) string => 'blah'", "func foo(bar string|) string => 'blah'")]
        [DataRow("func foo(bar |a) string => 'blah'", "func foo(bar string|) string => 'blah'")]
        public async Task Func_lambda_argument_type_completions_only_suggest_types(string before, string after)
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor($"""
{before}
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            var updatedFile = file.ApplyCompletion(completions, "string");
            updatedFile.Should().HaveSourceText($"""
{after}
""");
        }

        [DataTestMethod]
        [DataRow("func foo(|) string => 'blah'")]
        [DataRow("func foo( | ) string => 'blah'")]
        [DataRow("func foo(a|) string => 'blah'")]
        [DataRow("func foo(|a) string => 'blah'")]
        public async Task Func_lambda_argument_name_offers_no_completions(string before)
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor($"""
{before}
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Func_usage_completions_are_presented()
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
@description('Checks whether the input is true in a roundabout way')
func isTrue(input bool) bool => !(input == false)

var test = is|
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);

            var updatedFile = file.ApplyCompletion(completions, "isTrue");
            updatedFile.Should().HaveSourceText($"""
@description('Checks whether the input is true in a roundabout way')
func isTrue(input bool) bool => !(input == false)

var test = isTrue(|)
""");

            var completion = completions.Single(x => x.Label == "isTrue").Documentation!.MarkupContent!.Value
                .Should().Contain("Checks whether the input is true in a roundabout way");
        }

        [TestMethod]
        public async Task Func_usage_param_property_completions_are_offered()
        {
            var serverHelper = new ServerRequestHelper(TestContext, ServerWithNamespaceProvider);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
type PathExtension = {
  path: string
}

func getPath(input PathExtension) string => input.path

var test = getPath({|})
""");
            var file = await serverHelper.OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(c => c.Label == "path");

            var newFile = file.ApplyCompletion(completions, "path");
            newFile.Should().HaveSourceText("""
type PathExtension = {
  path: string
}

func getPath(input PathExtension) string => input.path

var test = getPath({path:|})
""");
        }

        [TestMethod]
        public async Task Imported_func_usage_param_property_completions_are_offered()
        {
            var modContent = """
type PathExtension = {
path: string
}

@export()
func getPath(input PathExtension) string => input.path
""";


            var mainContent = """
import * as mod from 'mod.bicep'
import { getPath } from 'mod.bicep'
var foo = getPath({|})
var bar = mod.getPath({|})
""";

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = modContent,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);

            var completions = await file.RequestAndResolveCompletions(cursors[0]);
            completions.Should().Contain(c => c.Label == "path");
            completions = await file.RequestAndResolveCompletions(cursors[1]);
            completions.Should().Contain(c => c.Label == "path");
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
                    completions => completions.Should().BeEmpty()),
                '|');
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
                    completions => completions.Should().BeEmpty()),
                '|');
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
                    completions => completions.Should().BeEmpty()),
                '|');
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
                    completions => completions.Should().BeEmpty()),
                '|');
        }

        [TestMethod]
        public async Task VerifyCompletionRequestAfterPoundSign_WithWhiteSpaceBeforePoundSign_ShouldReturnCompletionItem()
        {
            var fileWithCursors = @"    #|
param storageAccount1 string = 'testAccount'
    #|
param storageAccount2 string = 'testAccount'";

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursors[0]);
            completions.Should().Contain(x => x.Label == LanguageConstants.DisableNextLineDiagnosticsKeyword);

            var updatedFile = file.ApplyCompletion(completions, LanguageConstants.DisableNextLineDiagnosticsKeyword);
            updatedFile.Should().HaveSourceText(@"    #disable-next-line|
param storageAccount1 string = 'testAccount'
    #
param storageAccount2 string = 'testAccount'");

            completions = await file.RequestAndResolveCompletions(cursors[1]);
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

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
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

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithNamespaceProvider).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursors[0]);
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
#disable-next-line "/* <- preserve trailing space */+ @"
  properties: vmProperties
}");

            completions = await file.RequestAndResolveCompletions(cursors[1]);
            completions.Should().Contain(x => x.Label == "BCP036");
            completions.Should().Contain(x => x.Label == "BCP037");

            updatedFile = file.ApplyCompletion(completions, "BCP036");
            updatedFile.Should().HaveSourceText(@"#disable-next-line "/* <- preserve trailing space */+ @"
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
            updatedFile.Should().HaveSourceText(@"#disable-next-line "/* <- preserve trailing space */+ @"
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

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithNamespaceProvider).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
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

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithNamespaceProvider).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
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
                    completions => completions.Should().BeEmpty()),
                '|');
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
                    completions => completions.Should().BeEmpty()),
                '|');
        }

        [TestMethod]
        public async Task VerifyDisableNextLineDiagnosticsDirectiveCompletionIsNotAvailableToSuppressCoreCompilerErrors()
        {
            string fileWithCursors = @"#disable-next-line |
resource test";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithBuiltInTypes).OpenFile(text);

            CompilationHelper.Compile(text).ExcludingLinterDiagnostics().Diagnostics.Should().SatisfyRespectively(
                x => x.Code.Should().Be("BCP226"),
                x => x.Code.Should().Be("BCP068"),
                x => x.Code.Should().Be("BCP029"));

            var completions = await file.RequestAndResolveCompletions(cursor);
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
                    completions => completions!.Where(x => x.Label == "resourceId").First().Documentation!.MarkupContent!.Value.Should().EqualIgnoringNewlines(
                      @"```bicep
resourceId(resourceType: string, ... : string): string
resourceId(subscriptionId: string, resourceType: string, ... : string): string
resourceId(resourceGroupName: string, resourceType: string, ... : string): string
resourceId(subscriptionId: string, resourceGroupName: string, resourceType: string, ... : string): string

```  " + @"
Returns the unique identifier of a resource. You use this function when the resource name is ambiguous or not provisioned within the same template. The format of the returned identifier varies based on whether the deployment happens at the scope of a resource group, subscription, management group, or tenant.  " + @"
")));
        }

        [TestMethod]
        public async Task VerifyCompletionRequestResourceDependsOn_ResourceSymbolsVeryHighPriority()
        {
            var moduleContent = @"
@description('input that you want multiplied by 3')
param input int = 2

@description('input multiplied by 3')
output inputTimesThree int = input * 3
";

            string mainContent = @"
resource aResource 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'bar'
}
param paramObj object = {
  name: 'test'
}
var notAResource = 'I\'m a string!'
module aModule 'mod.bicep' = {
  name: 'someModule'
}
resource storageArr 'Microsoft.Storage/storageAccounts@2022-09-01' = [for i in range(0, 2): {
  name: 'storage${i}'
  kind: 'StorageV2'
}]
resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'foo'
  dependsOn: [
    |
  ]
}";

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = moduleContent,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri,
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create())
            );

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestAndResolveCompletions(cursors[0]);

            completions.Should().Contain(c => c.Label == "aResource" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_aResource");
            completions.Should().Contain(c => c.Label == "aModule" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_aModule");
            completions.Should().Contain(c => c.Label == "storageArr" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_storageArr");
            completions.Should().Contain(c => c.Label == "notAResource" && c.SortText == $"{(int)CompletionPriority.Medium}_notAResource");
            completions.Should().Contain(c => c.Label == "paramObj" && c.SortText == $"{(int)CompletionPriority.Medium}_paramObj");
            completions.Should().NotContain(c => c.Label == "foo");
            completions.Should().NotContain(c => c.Label == "[]");
            completions.Should().NotContain(c => c.Preselect);
        }

        [TestMethod]
        public async Task VerifyCompletionRequestResourceDependsOn_ResourceSymbolsVeryHighPriority_NestedResources()
        {
            string mainContent = @"
resource aResource 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'bar'
}
param paramArr array = []
var notAResource = 'I\'m a string!'
resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'foo'
  dependsOn: [
    |
  ]

  resource fooChild1 'fileServices' = {
    name: 'fooChild1'
    dependsOn: [
      |
    ]

    resource fooChild1Child1 'shares' = {
      name: 'fooChild1Child1'
      dependsOn: [|]
    }
  }

  resource fooChild2 'fileServices' = {
    name: 'fooChild2'
    dependsOn: [
      |
    ]

    resource fooChild2Child1 'shares' = {
      name: 'fooChild2Child1'
      dependsOn: [|]
    }
  }
}
";

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithNamespaceProvider).OpenFile(text);
            var allCompletions = await file.RequestCompletions(cursors);

            allCompletions.Should().HaveCount(5);

            foreach (var completions in allCompletions)
            {
                completions.Should().Contain(c => c.Label == "aResource" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_aResource");
            }

            var fooDependsOnCompletions = allCompletions[0];
            fooDependsOnCompletions.Should().NotContain(c => c.Label.StartsWith("foo"));

            var fooChild1DependsOnCompletions = allCompletions[1];
            fooChild1DependsOnCompletions.Should().Contain(c => c.Label == "fooChild2" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_fooChild2");
            fooChild1DependsOnCompletions.Should().Contain(c => c.Label == "foo" && c.SortText == $"{(int)CompletionPriority.Medium}_foo");
            fooChild1DependsOnCompletions.Should().NotContain(c => c.Label == "fooChild1");
            fooChild1DependsOnCompletions.Should().NotContain(c => c.Label == "fooChild1Child1");
            fooChild1DependsOnCompletions.Should().NotContain(c => c.Label == "fooChild2Child1");

            var fooChild1Child1DependsOnCompletions = allCompletions[2];
            fooChild1Child1DependsOnCompletions.Should().Contain(c => c.Label == "fooChild2" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_fooChild2");
            fooChild1Child1DependsOnCompletions.Should().Contain(c => c.Label == "fooChild1" && c.SortText == $"{(int)CompletionPriority.Medium}_fooChild1");
            fooChild1Child1DependsOnCompletions.Should().Contain(c => c.Label == "foo" && c.SortText == $"{(int)CompletionPriority.Medium}_foo");
            fooChild1Child1DependsOnCompletions.Should().NotContain(c => c.Label == "fooChild1Child1");

            var fooChild2DependsOnCompletions = allCompletions[3];
            fooChild2DependsOnCompletions.Should().Contain(c => c.Label == "fooChild1" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_fooChild1");
            fooChild2DependsOnCompletions.Should().Contain(c => c.Label == "foo" && c.SortText == $"{(int)CompletionPriority.Medium}_foo");
            fooChild2DependsOnCompletions.Should().NotContain(c => c.Label == "fooChild2");
            fooChild2DependsOnCompletions.Should().NotContain(c => c.Label == "fooChild1Child1");
            fooChild2DependsOnCompletions.Should().NotContain(c => c.Label == "fooChild2Child1");

            var fooChild2Child1DependsOnCompletions = allCompletions[4];
            fooChild2Child1DependsOnCompletions.Should().Contain(c => c.Label == "fooChild1" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_fooChild1");
            fooChild2Child1DependsOnCompletions.Should().Contain(c => c.Label == "fooChild2" && c.SortText == $"{(int)CompletionPriority.Medium}_fooChild2");
            fooChild2Child1DependsOnCompletions.Should().Contain(c => c.Label == "foo" && c.SortText == $"{(int)CompletionPriority.Medium}_foo");
            fooChild2Child1DependsOnCompletions.Should().NotContain(c => c.Label == "fooChild2Child1");
        }

        [TestMethod]
        public async Task VerifyCompletionRequestResourceDependsOn_ResourceSymbolsVeryHighPriority_TernaryAndParentheses()
        {
            string fileWithCursors = @"
resource aResource 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'bar'
}
var notAResource = 'I\'m a string!'
resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'foo'
  dependsOn: [
    (|)
    ((|))
    notAResource == 'test' ? |
    notAResource == 'test' ? aResource : |
    notAResource == 'test' ? (true ? aResource : |) : aResource
    notAResource == 'test' ? (|
  ]
}";

            var (text, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithNamespaceProvider).OpenFile(text);

            var allCompletions = await file.RequestCompletions(cursors);
            allCompletions.Should().HaveCount(6);
            foreach (var completions in allCompletions)
            {
                completions.Should().Contain(c => c.Label == "aResource" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_aResource");
                completions.Should().NotContain(c => c.Label == "foo");
            }
        }

        [DataTestMethod]
        [DataRow("[(|)]")]
        [DataRow("[(|]")]
        [DataRow("[((|))]")]
        [DataRow("[(( | ))]")]
        [DataRow("[a, (|)]")]
        [DataRow("[tmp ? |]")]
        [DataRow("[tmp ? a : |]")]
        [DataRow("[a, tmp ? |]")]
        [DataRow("[a, tmp ? | ]")]
        [DataRow("[a, tmp ? a : |]")]
        [DataRow("[(a), tmp ? a : b, |]")]
        [DataRow("[tmp ? (tmp ? |)]")]
        [DataRow("[, (|)]")]
        [DataRow("[, ( | )]")]
        [DataRow("[, (tmp ? (tmp ? |))]")]
        [DataRow("[, (tmp ? (tmp ? |]")]
        public async Task VerifyCompletionRequestResourceDependsOn_ResourceSymbolsVeryHighPriority_TernaryAndParentheses_SingleLineArray(string arrayText)
        {
            string fileWithCursors = @$"
resource aResource 'Microsoft.Storage/storageAccounts@2022-09-01' = {{
  name: 'bar'
}}
var tmp = true
var notAResource = 'I\'m a string!'
resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {{
  name: 'foo'
  dependsOn: {arrayText}
}}
";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithNamespaceProvider).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(c => c.Label == "aResource" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_aResource");
            completions.Should().NotContain(c => c.Label == "foo");
        }

        [TestMethod]
        public async Task VerifyCompletionRequestModuleDependsOn_ResourceSymbolsVeryHighPriority()
        {
            var moduleContent = @"
@description('input that you want multiplied by 3')
param input int = 2

@description('input multiplied by 3')
output inputTimesThree int = input * 3
";

            var mainContent = @"
resource aResource 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'bar'
}
module bModule 'mod2.bicep' = {
  name: 'someModule'
}
var notAResource = 'I\'m a string!'
module aModule 'mod.bicep' = {
  name: 'someModule'
  dependsOn: [
    |
  ]
}
";

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = moduleContent,
                ["file:///mod2.bicep"] = moduleContent,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri,
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create())
            );

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestAndResolveCompletions(cursors[0]);

            completions.Should().Contain(c => c.Label == "aResource" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_aResource");
            completions.Should().Contain(c => c.Label == "bModule" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_bModule");
            completions.Should().Contain(c => c.Label == "notAResource" && c.SortText == $"{(int)CompletionPriority.Medium}_notAResource");
            completions.Should().NotContain(c => c.Label == "aModule");
            completions.Should().NotContain(c => c.Label == "[]");
            completions.Should().NotContain(c => c.Preselect);
        }

        [TestMethod]
        public async Task VerifyCompletionRequestModuleDependsOn_ResourceSymbolsVeryHighPriority_TernaryAndParentheses()
        {
            var moduleContent = @"
@description('input that you want multiplied by 3')
param input int = 2

@description('input multiplied by 3')
output inputTimesThree int = input * 3
";

            var mainContentWithCursors = @"
resource aResource 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'bar'
}
module aModule 'mod.bicep' = {
  name: 'someModule'
}
var notAResource = 'I\'m a string!'
module foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'foo'
  dependsOn: [
    (|)
    ((|))
    notAResource == 'test' ? |
    notAResource == 'test' ? aResource : |
    notAResource == 'test' ? (true ? aResource : |) : aResource
    notAResource == 'test' ? (|
  ]
}";

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContentWithCursors, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = moduleContent,
                ["file:///mod2.bicep"] = moduleContent,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri,
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create())
            );
            var file = new FileRequestHelper(helper.Client, bicepFile);
            var allCompletions = await file.RequestCompletions(cursors);

            allCompletions.Should().HaveCount(6);
            foreach (var completions in allCompletions)
            {
                completions.Should().Contain(c => c.Label == "aResource" && c.SortText == $"{(int)CompletionPriority.VeryHigh}_aResource");
                completions.Should().NotContain(c => c.Label == "foo");
                completions.Should().NotContain(c => c.Label == "[]");
                completions.Should().NotContain(c => c.Preselect);
            }
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
                ExpectedCompletionsScope.DataSet => DataSet.GetBaselineUpdatePath(dataSet, Path.Combine(DataSet.TestCompletionsDirectory, GetFullSetName(setName))),
                _ => GetGlobalCompletionSetPath(setName)
            };

            actual.Should().EqualWithJsonDiffOutput(this.TestContext, expected, expectedLocation, actualLocation, "because ");
        }

        private static string GetGlobalCompletionSetPath(string setName) => DataSet.GetBaselineUpdatePath(DataSet.TestCompletionsDirectory, GetFullSetName(setName));

        private static async Task<CompletionList> RunSingleCompletionScenarioTest(TestContext testContext, SharedLanguageHelperManager server, string text, int offset)
        {
            var file = await new ServerRequestHelper(testContext, server).OpenFile(text);

            return await file.RequestAndResolveCompletions(offset);
        }

        private static async Task RunCompletionScenarioTest(TestContext testContext, SharedLanguageHelperManager server, string fileWithCursors, Action<IEnumerable<CompletionList>> assertAction, char cursor = '|')
        {
            var (fileText, cursors) = ParserHelper.GetFileWithCursors(fileWithCursors, cursor);
            var file = await new ServerRequestHelper(testContext, server).OpenFile(fileText);

            var completions = await file.RequestCompletions(cursors);

            assertAction(completions);
        }

        private async Task RunCompletionTest(string fileWithCursor, string completionLabel, string expectedOutput)
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursor);
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            var updatedFile = file.ApplyCompletion(completions, completionLabel);
            updatedFile.Should().HaveSourceText(expectedOutput);
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
        [DataRow("loadYamlContent", false, true)]
        [DataRow("loadDirectoryFileInfo", false, false, true)]
        public async Task LoadFunctionsPathArgument_returnsFilesInCompletions(string functionName, bool jsonOnTop = false, bool yamlOnTop = false, bool directoryOnTop = false)
        {
            var mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");

            var (mainFileText, cursor) = ParserHelper.GetFileWithSingleCursor(@"
var file = " + functionName + @"('|')
",
                '|');
            var mainFile = new LanguageClientFile(mainUri, mainFileText);
            var schema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";

            var fileTextsByUri = new Dictionary<DocumentUri, string>
            {
                [mainUri] = mainFileText,
                [InMemoryFileResolver.GetFileUri("/path/to/template1.arm")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/template2.json")] = @$"{{ ""schema"": ""{schema}"" }}",
                [InMemoryFileResolver.GetFileUri("/path/to/template3.jsonc")] = @"{}",
                [InMemoryFileResolver.GetFileUri("/path/to/template4.json")] = new string('x', 2000 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/template5.json")] = new string('x', 2002 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/template6.yaml")] = @$"{{ ""schema"": ""{schema}"" }}",
                [InMemoryFileResolver.GetFileUri("/path/to/template7.yml")] = @"",
                [InMemoryFileResolver.GetFileUri("/path/to/template8.yaml")] = new string('x', 2000 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/template9.yaml")] = new string('x', 2002 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/json1.json")] = "{}",
                [InMemoryFileResolver.GetFileUri("/path/to/json2.json")] = @"[{ ""name"": ""value"" }]",
                [InMemoryFileResolver.GetFileUri("/path/to/yaml1.yaml")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/yaml2.yaml")] = @"[{ ""name"": ""value"" }]",
                [InMemoryFileResolver.GetFileUri("/path/to/module1.txt")] = "param foo string",
                [InMemoryFileResolver.GetFileUri("/path/to/module2.bicep")] = "param bar bool",
                [InMemoryFileResolver.GetFileUri("/path/to/module3.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/moduleFolder1/module.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/moduleFolder2/module.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/moduleFolder3/module.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/moduleFolder4/module.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/moduleFolder5/module.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/moduleFolder6/module.bicep")] = "",
            };

            using var helper = await LanguageServerHelper.StartServerWithText(
                TestContext,
                fileTextsByUri,
                mainUri,
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));

            var file = new FileRequestHelper(helper.Client, mainFile);

            var completions = await file.RequestAndResolveCompletions(cursor);

            var completionItems = completions.Where(x => !directoryOnTop ? x.Kind == CompletionItemKind.File : x.Kind == CompletionItemKind.Folder).OrderBy(x => x.SortText);
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
                    x => x.Label.Should().Be("template1.arm"),
                    x => x.Label.Should().Be("template6.yaml"),
                    x => x.Label.Should().Be("template7.yml"),
                    x => x.Label.Should().Be("template8.yaml"),
                    x => x.Label.Should().Be("template9.yaml"),
                    x => x.Label.Should().Be("yaml1.yaml"),
                    x => x.Label.Should().Be("yaml2.yaml")
                );
            }
            else if (yamlOnTop)
            {
                completionItems.Should().SatisfyRespectively(
                    x => x.Label.Should().Be("template6.yaml"),
                    x => x.Label.Should().Be("template7.yml"),
                    x => x.Label.Should().Be("template8.yaml"),
                    x => x.Label.Should().Be("template9.yaml"),
                    x => x.Label.Should().Be("yaml1.yaml"),
                    x => x.Label.Should().Be("yaml2.yaml"),
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
            else if (directoryOnTop)
            {
                completionItems.Should().SatisfyRespectively(
                    x => x.Label.Should().Be("../"),
                    x => x.Label.Should().Be("moduleFolder1/"),
                        x => x.Label.Should().Be("moduleFolder2/"),
                        x => x.Label.Should().Be("moduleFolder3/"),
                        x => x.Label.Should().Be("moduleFolder4/"),
                        x => x.Label.Should().Be("moduleFolder5/"),
                        x => x.Label.Should().Be("moduleFolder6/"),
                        x => x.Label.Should().Be("az"),
                        x => x.Label.Should().Be("sys")
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
                    x => x.Label.Should().Be("template5.json"),
                     x => x.Label.Should().Be("template6.yaml"),
                    x => x.Label.Should().Be("template7.yml"),
                    x => x.Label.Should().Be("template8.yaml"),
                    x => x.Label.Should().Be("template9.yaml"),
                    x => x.Label.Should().Be("yaml1.yaml"),
                    x => x.Label.Should().Be("yaml2.yaml")
                    );
            }
        }

        [DataTestMethod]
        [DataRow("loadTextContent")]
        [DataRow("loadFileAsBase64")]
        [DataRow("loadJsonContent", true)]
        [DataRow("loadYamlContent", false, true)]
        [DataRow("loadDirectoryFileInfo", false, false, true)]
        public async Task LoadFunctionsPathArgument_returnsSymbolsAndFilePathsInCompletions(string functionName, bool jsonOnTop = false, bool yamlOnTop = false, bool directoryOnTop = false)
        {
            var mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");

            var (mainFileText, cursor) = ParserHelper.GetFileWithSingleCursor(@"
var template = 'template1.json'
var file = " + functionName + @"(templ|)
",
                '|');
            var mainFile = new LanguageClientFile(mainUri, mainFileText);
            var schema = "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#";

            var fileTextsByUri = new Dictionary<DocumentUri, string>
            {
                [mainUri] = mainFileText,
                [InMemoryFileResolver.GetFileUri("/path/to/template1.arm")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/template2.json")] = @$"{{ ""schema"": ""{schema}"" }}",
                [InMemoryFileResolver.GetFileUri("/path/to/template3.jsonc")] = @"{}",
                [InMemoryFileResolver.GetFileUri("/path/to/template4.json")] = new string('x', 2000 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/template5.json")] = new string('x', 2002 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/template6.yaml")] = @$"{{ ""schema"": ""{schema}"" }}",
                [InMemoryFileResolver.GetFileUri("/path/to/template7.yml")] = @"",
                [InMemoryFileResolver.GetFileUri("/path/to/template8.yaml")] = new string('x', 2000 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/template9.yaml")] = new string('x', 2002 - schema.Length) + schema,
                [InMemoryFileResolver.GetFileUri("/path/to/json1.json")] = "{}",
                [InMemoryFileResolver.GetFileUri("/path/to/json2.json")] = @"[{ ""name"": ""value"" }]",
                [InMemoryFileResolver.GetFileUri("/path/to/yaml1.yaml")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/yaml2.yaml")] = @"[{ ""name"": ""value"" }]",
                [InMemoryFileResolver.GetFileUri("/path/to/module1.txt")] = "param foo string",
                [InMemoryFileResolver.GetFileUri("/path/to/module2.bicep")] = "param bar bool",
                [InMemoryFileResolver.GetFileUri("/path/to/module3.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/moduleFolder1/module.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/moduleFolder2/module.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/moduleFolder3/module.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/moduleFolder4/module.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/moduleFolder5/module.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/moduleFolder6/module.bicep")] = "",
            };

            using var helper = await LanguageServerHelper.StartServerWithText(
                TestContext,
                fileTextsByUri,
                mainUri,
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));
            var file = new FileRequestHelper(helper.Client, mainFile);

            var completions = await file.RequestAndResolveCompletions(cursor);

            var completionItems = !directoryOnTop ? completions.OrderBy(x => x.SortText).Where(x => x.Label.StartsWith("templ"))
                    : completions.OrderBy(x => x.SortText).Where(x => x.Label.StartsWith("moduleFol"));
            if (jsonOnTop)
            {
                completionItems.Should().SatisfyRespectively(
                    x => x.Label.Should().Be("template2.json"),
                    x => x.Label.Should().Be("template3.jsonc"),
                    x => x.Label.Should().Be("template4.json"),
                    x => x.Label.Should().Be("template5.json"),
                    x => x.Label.Should().Be("template"),
                    x => x.Label.Should().Be("template1.arm"),
                    x => x.Label.Should().Be("template6.yaml"),
                    x => x.Label.Should().Be("template7.yml"),
                    x => x.Label.Should().Be("template8.yaml"),
                    x => x.Label.Should().Be("template9.yaml")
                );
            }
            else if (yamlOnTop)
            {
                completionItems.Should().SatisfyRespectively(
                    x => x.Label.Should().Be("template6.yaml"),
                    x => x.Label.Should().Be("template7.yml"),
                    x => x.Label.Should().Be("template8.yaml"),
                    x => x.Label.Should().Be("template9.yaml"),
                    x => x.Label.Should().Be("template"),
                    x => x.Label.Should().Be("template1.arm"),
                    x => x.Label.Should().Be("template2.json"),
                    x => x.Label.Should().Be("template3.jsonc"),
                    x => x.Label.Should().Be("template4.json"),
                    x => x.Label.Should().Be("template5.json")

                );
            }
            else if (directoryOnTop)
            {
                completionItems.Should().SatisfyRespectively(
                    x => x.Label.Should().Be("moduleFolder1/"),
                    x => x.Label.Should().Be("moduleFolder2/"),
                    x => x.Label.Should().Be("moduleFolder3/"),
                    x => x.Label.Should().Be("moduleFolder4/"),
                    x => x.Label.Should().Be("moduleFolder5/"),
                    x => x.Label.Should().Be("moduleFolder6/")
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
                    x => x.Label.Should().Be("template6.yaml"),
                    x => x.Label.Should().Be("template7.yml"),
                    x => x.Label.Should().Be("template8.yaml"),
                    x => x.Label.Should().Be("template9.yaml"),
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
        [DataRow("import {} from |", "other.bicep", "import {} from 'other.bicep'|")]
        [DataRow("import {} from 'oth|'", "other.bicep", "import {} from 'other.bicep'|")]
        [DataRow("import {} from oth|", "other.bicep", "import {} from 'other.bicep'|")]
        public async Task Module_path_completions_are_offered(string fileWithCursors, string expectedLabel, string expectedResult)
        {
            var fileUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");
            var fileResolver = new InMemoryFileResolver(new Dictionary<Uri, string>
            {
                [InMemoryFileResolver.GetFileUri("/path/to/other.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to2/main.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path2/to/main.bicep")] = "",
            });

            using var helper = await MultiFileLanguageServerHelper.StartLanguageServer(TestContext, services => services.WithFileExplorer(new FileSystemFileExplorer(fileResolver.MockFileSystem)));

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, helper).OpenFile(fileUri, text);

            var completions = await file.RequestAndResolveCompletions(cursor);

            completions.Should().Contain(x => x.Label == expectedLabel, $"\"{fileWithCursors}\" should have completion");
            var updatedFile = file.ApplyCompletion(completions, expectedLabel);
            updatedFile.Should().HaveSourceText(expectedResult);
        }

        [DataTestMethod]
        [DataRow("module test 'br/|'", "groups.bicep", CompletionItemKind.File, "../", CompletionItemKind.Folder, "public", CompletionItemKind.Snippet)]
        [DataRow("module test 'br/|", "br/", CompletionItemKind.Folder, "../", CompletionItemKind.Folder, "public", CompletionItemKind.Snippet)]
        public async Task ModuleRegistryReferenceCompletions_GetCompletionsAfterBrSchema(
            string inputWithCursors,
            string expectedLabel1,
            CompletionItemKind completionItemKind1,
            string expectedLabel2,
            CompletionItemKind completionItemKind2,
            string expectedLabel3,
            CompletionItemKind completionItemKind3)
        {
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(inputWithCursors, '|');

            var mainBicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", text, testOutputPath);
            var mainUri = DocumentUri.FromFileSystemPath(mainBicepFilePath);

            FileHelper.SaveResultFile(TestContext, "groups.bicep", string.Empty, Path.Combine(testOutputPath, "br"));

            var settingsProvider = StrictMock.Of<ISettingsProvider>();
            settingsProvider.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(false);

            using var helper = await MultiFileLanguageServerHelper.StartLanguageServer(
                TestContext,
                services => services.AddSingleton<ISettingsProvider>(settingsProvider.Object));

            var file = await new ServerRequestHelper(TestContext, helper).OpenFile(mainUri.ToUriEncoded(), text);
            var completions = await file.RequestAndResolveCompletions(cursor);

            completions.Count().Should().Be(3);
            completions.Should().Contain(x => x.Label == expectedLabel1 && x.Kind == completionItemKind1);
            completions.Should().Contain(x => x.Label == expectedLabel2 && x.Kind == completionItemKind2);
            completions.Should().Contain(x => x.Label == expectedLabel3 && x.Kind == completionItemKind3);
        }

        [TestMethod]
        public async Task ModuleRegistryReferenceCompletions_GetCompletionsForFolderInsideBr()
        {
            var testOutputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("module test 'br/foo/|'", '|');

            var mainBicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", text, testOutputPath);
            var mainUri = DocumentUri.FromFileSystemPath(mainBicepFilePath);

            FileHelper.SaveResultFile(TestContext, "bar.bicep", string.Empty, Path.Combine(testOutputPath, Path.Combine(testOutputPath, "br", "foo")));

            var settingsProvider = StrictMock.Of<ISettingsProvider>();
            settingsProvider.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(false);

            using var helper = await MultiFileLanguageServerHelper.StartLanguageServer(
                TestContext,
                services => services
                .AddSingleton<ISettingsProvider>(settingsProvider.Object));

            var file = await new ServerRequestHelper(TestContext, helper).OpenFile(mainUri.ToUriEncoded(), text);
            var completions = await file.RequestAndResolveCompletions(cursor);

            completions.Count().Should().Be(2);
            completions.Should().Contain(x => x.Label == "bar.bicep" && x.Kind == CompletionItemKind.File);
            completions.Should().Contain(x => x.Label == "../" && x.Kind == CompletionItemKind.Folder);
        }

        [DataTestMethod]
        [DataRow("module test 'br/public:app/dapr-containerapp:|'", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br/public:app/dapr-containerapp:|", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/app/dapr-containerapp:|'", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/app/dapr-containerapp:|", BicepSourceFileKind.BicepFile)]
        [DataRow("using 'br/public:app/dapr-containerapp:|'", BicepSourceFileKind.ParamsFile)]
        [DataRow("using 'br/public:app/dapr-containerapp:|", BicepSourceFileKind.ParamsFile)]
        [DataRow("using 'br:mcr.microsoft.com/bicep/app/dapr-containerapp:|'", BicepSourceFileKind.ParamsFile)]
        [DataRow("using 'br:mcr.microsoft.com/bicep/app/dapr-containerapp:|", BicepSourceFileKind.ParamsFile)]
        public async Task Public_module_version_completions(string inputWithCursors, BicepSourceFileKind kind)
        {
            var extension = kind == BicepSourceFileKind.ParamsFile ? "bicepparam" : "bicep";
            var (fileText, cursor) = ParserHelper.GetFileWithSingleCursor(inputWithCursors, '|');
            var fileUri = new Uri($"file:///{Guid.NewGuid():D}/{TestContext.TestName}/main.{extension}");

            var settingsProvider = StrictMock.Of<ISettingsProvider>();
            settingsProvider.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(false);

            var publicModuleMetadataProvider = RegistryCatalogMocks.MockPublicMetadataProvider(
                [("bicep/app/dapr-containerapp", "d1", "contoso.com/help1", [
                    new("1.0.1", null, null),
                    new("1.0.2", "d1", "contoso.com/help1")
                ])]);

            using var helper = await MultiFileLanguageServerHelper.StartLanguageServer(
                TestContext,
                services => services
                .AddSingleton(publicModuleMetadataProvider.Object)
                .AddSingleton(settingsProvider.Object));

            var file = await new ServerRequestHelper(TestContext, helper).OpenFile(fileUri, fileText);
            var completions = await file.RequestAndResolveCompletions(cursor);

            completions.Count().Should().Be(2);
            completions.Should().SatisfyRespectively(
                first =>
                {
                    first.Label.Should().Be("1.0.2");
                    first.SortText.Should().Be("0000");
                    first.Kind.Should().Be(CompletionItemKind.Snippet);
                    first.Detail.Should().Be("d1");
                    first.Documentation!.MarkupContent!.Value.Should().Be("[View Documentation](contoso.com/help1)");
                },
                second =>
                {
                    second.Label.Should().Be("1.0.1");
                    second.SortText.Should().Be("0001");
                    second.Kind.Should().Be(CompletionItemKind.Snippet);
                    second.Detail.Should().BeNull();
                    second.Documentation.Should().BeNull();
                }
            );
        }

        [DataTestMethod]
        [DataRow("module test 'br/contoso:app/private-app:|'", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br/contoso:app/private-app:|", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br:private.contoso.com/app/private-app:|'", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br:private.contoso.com/app/private-app:|", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br/contoso:app/private-app:|'", BicepSourceFileKind.ParamsFile)]
        [DataRow("module test 'br/contoso:app/private-app:|", BicepSourceFileKind.ParamsFile)]
        [DataRow("module test 'br:private.contoso.com/app/private-app:|'", BicepSourceFileKind.ParamsFile)]
        [DataRow("module test 'br:private.contoso.com/app/private-app:|", BicepSourceFileKind.ParamsFile)]
        public async Task Private_module_version_completions(string inputWithCursors, BicepSourceFileKind kind)
        {
            var extension = kind == BicepSourceFileKind.ParamsFile ? "bicepparam" : "bicep";
            var (fileText, cursor) = ParserHelper.GetFileWithSingleCursor(inputWithCursors, '|');

            var settingsProvider = StrictMock.Of<ISettingsProvider>();
            settingsProvider.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(false);

            var privateModuleMetadataProvider = RegistryCatalogMocks.MockPrivateMetadataProvider(
                "private.contoso.com",
                [("app/private-app", "d1", "contoso.com/help1", [
                    new("v100", "d100", "contoso.com/help/d100.html"),
                    new("v101", "d101", "contoso.com/help/d101.html")])
                ]);
            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(
                null,
                privateModuleMetadataProvider);

            var configurationManager = StrictMock.Of<IConfigurationManager>();
            var moduleAliasesConfiguration = BicepTestConstants.BuiltInConfiguration.With(
                moduleAliases: RegistryCatalogMocks.ModuleAliases(
                    """
                    {
                        "br": {
                            "contoso": {
                                "registry": "private.contoso.com"
                            }
                        }
                    }
                    """));
            var fileUri = DocumentUri.From($"file:///{Guid.NewGuid():D}/{TestContext.TestName}/main.{extension}");
            configurationManager.Setup(x => x.GetConfiguration(fileUri.ToIOUri())).Returns(moduleAliasesConfiguration);

            using var helper = await MultiFileLanguageServerHelper.StartLanguageServer(
                TestContext,
                services => services
                .AddSingleton(settingsProvider.Object)
                .AddSingleton(configurationManager.Object)
                .AddSingleton(catalog));

            var file = await new ServerRequestHelper(TestContext, helper).OpenFile(fileUri, fileText);
            var completions = await file.RequestAndResolveCompletions(cursor);

            completions.Count().Should().Be(2);
            completions.Should().SatisfyRespectively(
                first =>
                {
                    first.Label.Should().Be("v101");
                    first.SortText.Should().Be("0000");
                    first.Kind.Should().Be(CompletionItemKind.Snippet);
                    first.Detail.Should().Be("d101");
                    first.Documentation!.MarkupContent!.Value.Should().Be("[View Documentation](contoso.com/help/d101.html)");
                },
                second =>
                {
                    second.Label.Should().Be("v100");
                    second.SortText.Should().Be("0001");
                    second.Kind.Should().Be(CompletionItemKind.Snippet);
                    second.Detail.Should().Be("d100");
                    second.Documentation!.MarkupContent!.Value.Should().Be("[View Documentation](contoso.com/help/d100.html)");
                }
            );
        }

        [TestMethod]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/abc/foo|'", "bicep/abc/foo/bar", "'br:mcr.microsoft.com/bicep/abc/foo/bar:$0'", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/abc/foo|", "bicep/abc/foo/bar", "'br:mcr.microsoft.com/bicep/abc/foo/bar:$0'", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br/public:abc/foo|'", "abc/foo/bar", "'br/public:abc/foo/bar:$0'", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br/public:abc/foo|", "abc/foo/bar", "'br/public:abc/foo/bar:$0'", BicepSourceFileKind.BicepFile)]
        [DataRow("using 'br:mcr.microsoft.com/bicep/abc/foo|'", "bicep/abc/foo/bar", "'br:mcr.microsoft.com/bicep/abc/foo/bar:$0'", BicepSourceFileKind.ParamsFile)]
        [DataRow("using 'br:mcr.microsoft.com/bicep/abc/foo|", "bicep/abc/foo/bar", "'br:mcr.microsoft.com/bicep/abc/foo/bar:$0'", BicepSourceFileKind.ParamsFile)]
        [DataRow("using 'br/public:abc/foo|'", "abc/foo/bar", "'br/public:abc/foo/bar:$0'", BicepSourceFileKind.ParamsFile)]
        [DataRow("using 'br/public:abc/foo|", "abc/foo/bar", "'br/public:abc/foo/bar:$0'", BicepSourceFileKind.ParamsFile)]
        public async Task Public_registry_module_completions_support_prefix_matching(string text, string expectedLabelForFoo, string expectedInsertTextForFoo, BicepSourceFileKind kind)
        {
            var extension = kind == BicepSourceFileKind.ParamsFile ? "bicepparam" : "bicep";
            var (fileText, cursor) = ParserHelper.GetFileWithSingleCursor(text, '|');
            var fileUri = new Uri($"file:///{Guid.NewGuid():D}/{TestContext.TestName}/main.{extension}");

            var settingsProvider = StrictMock.Of<ISettingsProvider>();
            settingsProvider.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(false);

            var publicModuleMetadataProvider = RegistryCatalogMocks.MockPublicMetadataProvider([
                   ("bicep/abc/foo/bar", "d1", "contoso.com/help1", []),
                ("bicep/abc/food/bar", "d2", "contoso.com/help2", []),
                ("bicep/abc/bar/bar", "d3", "contoso.com/help3", []),
            ]);

            using var helper = await MultiFileLanguageServerHelper.StartLanguageServer(
                TestContext,
                services => services
                    .AddSingleton<IPublicModuleMetadataProvider>(publicModuleMetadataProvider.Object)
                    .AddSingleton(settingsProvider.Object));

            var file = await new ServerRequestHelper(TestContext, helper).OpenFile(fileUri, fileText);
            var completions = await file.RequestAndResolveCompletions(cursor);

            completions.Count().Should().Be(2);
            completions.Select(x => (Label: x.Label, InsertText: x.TextEdit!.TextEdit!.NewText)).Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be(expectedLabelForFoo);
                    c.InsertText.Should().Be(expectedInsertTextForFoo);
                },
                c =>
                {
                    c.Label.Should().Be(expectedLabelForFoo.Replace("foo/", "food/"));
                    c.InsertText.Should().Be(expectedInsertTextForFoo.Replace("foo/", "food/"));
                }
            );
        }

        [TestMethod]
        [DataRow("module test 'br:registry.contoso.io/bicep/whatever/abc/foo|'", "bicep/whatever/abc/foo/bar", "'br:registry.contoso.io/bicep/whatever/abc/foo/bar:$0'", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br:registry.contoso.io/bicep/whatever/abc/foo|", "bicep/whatever/abc/foo/bar", "'br:registry.contoso.io/bicep/whatever/abc/foo/bar:$0'", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br/myRegistry:abc/foo|'", "abc/foo/bar", "'br/myRegistry:abc/foo/bar:$0'", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br/myRegistry_noPath:bicep/whatever/abc/foo|", "bicep/whatever/abc/foo/bar", "'br/myRegistry_noPath:bicep/whatever/abc/foo/bar:$0'", BicepSourceFileKind.BicepFile)]
        [DataRow("module test 'br:registry.contoso.io/bicep/whatever/abc/foo|'", "bicep/whatever/abc/foo/bar", "'br:registry.contoso.io/bicep/whatever/abc/foo/bar:$0'", BicepSourceFileKind.ParamsFile)]
        [DataRow("module test 'br/myRegistry_noPath:bicep/whatever/abc/foo|", "bicep/whatever/abc/foo/bar", "'br/myRegistry_noPath:bicep/whatever/abc/foo/bar:$0'", BicepSourceFileKind.ParamsFile)]
        public async Task Private_registry_completions_support_prefix_matching(string text, string expectedLabelForFoo, string expectedInsertTextForFoo, BicepSourceFileKind kind)
        {
            var extension = kind == BicepSourceFileKind.ParamsFile ? "bicepparam" : "bicep";
            var (fileText, cursor) = ParserHelper.GetFileWithSingleCursor(text, '|');
            var baseFolder = $"{Guid.NewGuid():D}";

            var configurationManager = StrictMock.Of<IConfigurationManager>();
            var moduleAliasesConfiguration = BicepTestConstants.BuiltInConfiguration.With(
                moduleAliases: ModuleAliasesConfiguration.Bind(JsonElementFactory.CreateElement(
                """
                    {
                        "br": {
                            "myRegistry": {
                                "registry": "registry.contoso.io",
                                "modulePath": "bicep/whatever"
                            },
                            "myRegistry_noPath": {
                                "registry": "registry.contoso.io"
                            }
                        }
                    }
                    """),
                null));
            var fileUri = DocumentUri.From($"file:///{baseFolder}/{TestContext.TestName}/main.{extension}");
            configurationManager.Setup(x => x.GetConfiguration(fileUri.ToIOUri())).Returns(moduleAliasesConfiguration);

            var settingsProvider = StrictMock.Of<ISettingsProvider>();
            settingsProvider.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(false);

            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(
                null,
                RegistryCatalogMocks.MockPrivateMetadataProvider(
                    "registry.contoso.io",
                    [
                        ("bicep/whatever/abc/foo/bar", "d1", "contoso.com/help1", []),
                        ("bicep/whatever/abc/food/bar", "d2", "contoso.com/help2", []),
                        ("bicep/whatever/abc/bar/bar", "d3", "contoso.com/help3", []),

                    ])
                );

            using var helper = await MultiFileLanguageServerHelper.StartLanguageServer(
                TestContext,
                services => services
                    .AddSingleton(settingsProvider.Object)
                    .AddSingleton(catalog)
                    .AddSingleton(configurationManager.Object)
            );

            var file = await new ServerRequestHelper(TestContext, helper).OpenFile(fileUri, fileText);
            var completions = await file.RequestAndResolveCompletions(cursor);

            completions.Count().Should().Be(2);
            completions.Select(x => (Label: x.Label, InsertText: x.TextEdit!.TextEdit!.NewText)).Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be(expectedLabelForFoo);
                    c.InsertText.Should().Be(expectedInsertTextForFoo);
                },
                c =>
                {
                    c.Label.Should().Be(expectedLabelForFoo.Replace("foo/", "food/"));
                    c.InsertText.Should().Be(expectedInsertTextForFoo.Replace("foo/", "food/"));
                }
            );
        }

        [TestMethod]
        [DataRow("module test 'br/ms:bicep/app/|'", "bicep/app/dapr-containerapp", "'br/ms:bicep/app/dapr-containerapp:$0'")]
        [DataRow("module test 'br/ms_empty:bicep/app/|'", "bicep/app/dapr-containerapp", "'br/ms_empty:bicep/app/dapr-containerapp:$0'")]
        [DataRow("module test 'br/ms_bicep:app/|'", "app/dapr-containerapp", "'br/ms_bicep:app/dapr-containerapp:$0'")]
        public async Task Public_registry_via_alias_supports_completions(string text, string expectedLabel, string expectedInsertText)
        {
            var (fileText, cursor) = ParserHelper.GetFileWithSingleCursor(text, '|');
            var baseFolder = $"{Guid.NewGuid():D}";

            var configurationManager = StrictMock.Of<IConfigurationManager>();
            var moduleAliasesConfiguration = BicepTestConstants.BuiltInConfiguration.With(
                moduleAliases: ModuleAliasesConfiguration.Bind(JsonElementFactory.CreateElement(
                """
                    {
                        "br": {
                          "ms": {
                            "registry": "mcr.microsoft.com",
                            "modulePath": ""
                          },
                          "ms_empty": {
                            "registry": "mcr.microsoft.com"
                          },
                          "ms_bicep": {
                            "registry": "mcr.microsoft.com",
                            "modulePath": "bicep"
                          }
                        }
                      }
                    """),
                null));
            var fileUri = DocumentUri.From($"file:///{baseFolder}/{TestContext.TestName}/main.bicep");
            configurationManager.Setup(x => x.GetConfiguration(fileUri.ToIOUri())).Returns(moduleAliasesConfiguration);

            var settingsProvider = StrictMock.Of<ISettingsProvider>();
            settingsProvider.Setup(x => x.GetSetting(LangServerConstants.GetAllAzureContainerRegistriesForCompletionsSetting)).Returns(false);

            var catalog = RegistryCatalogMocks.CreateCatalogWithMocks(
              RegistryCatalogMocks.MockPublicMetadataProvider(
                [("bicep/app/dapr-containerapp", "d1", "contoso.com/help1", [
                    new("1.0.1", null, null),
                    new("1.0.2", "d1", "contoso.com/help1")
                ])]
              ));

            using var helper = await MultiFileLanguageServerHelper.StartLanguageServer(
                TestContext,
                services => services
                    .AddSingleton(settingsProvider.Object)
                    .AddSingleton(catalog)
                    .AddSingleton(configurationManager.Object)
            );

            var file = await new ServerRequestHelper(TestContext, helper).OpenFile(fileUri, fileText);
            var completions = await file.RequestAndResolveCompletions(cursor);

            completions.Count().Should().Be(1);
            completions.Select(x => (Label: x.Label, InsertText: x.TextEdit!.TextEdit!.NewText)).Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be(expectedLabel);
                    c.InsertText.Should().Be(expectedInsertText);
                }
            );
        }

        [DataTestMethod]
        [DataRow("var arr1 = [|]")]
        [DataRow("param arr array = [|]")]
        [DataRow("var arr2 = [a, |]")]
        [DataRow("var arr3 = [a,|]")]
        [DataRow("var arr4 = [a|]")]
        [DataRow("var arr5 = [|, b]")]
        [DataRow("var arr6 = [a, |, b]")]
        public async Task GenericArray_SingleLine_HasCompletions(string text)
        {
            var (fileText, cursor) = ParserHelper.GetFileWithSingleCursor(text, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithNamespaceProvider).OpenFile(fileText);

            var completions = await file.RequestAndResolveCompletions(cursor);
            // test completions that are unlikely to change over time
            completions.Should().Contain(c => c.Label == "sys");
            completions.Should().Contain(c => c.Label == "if-else");
        }

        [TestMethod]
        public async Task GenericArray_Multiline_HasCompletions()
        {
            const string text = @"
var arr1 = [
  |
]
param arr2 array = [
  |
]
var arr3 = [
  a|
]
var arr4 = [
  a
  |
]
param arr5 array = [
  a
  |
  b
]
var arr6 = [

  |

]";

            var (fileText, cursors) = ParserHelper.GetFileWithCursors(text, '|');
            var file = await new ServerRequestHelper(TestContext, ServerWithNamespaceProvider).OpenFile(fileText);

            var allCompletions = await file.RequestCompletions(cursors);

            allCompletions.Should().HaveCount(6);
            foreach (var completions in allCompletions)
            {
                // test completions that are unlikely to change over time
                completions.Should().Contain(c => c.Label == "sys");
                completions.Should().Contain(c => c.Label == "if-else");
            }
        }

        [TestMethod]
        public async Task Spread_operator_supports_expression_completions()
        {
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
param bar object

var foo = {
  prop: 'bar'
  ...|
}
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);

            var updatedFile = file.ApplyCompletion(completions, "bar");
            updatedFile.Should().HaveSourceText("""
param bar object

var foo = {
  prop: 'bar'
  ...bar|
}
""");
        }

        [TestMethod]
        public async Task Required_properties_completion_is_not_offered_for_invalid_recursive_types()
        {
            // https://github.com/Azure/bicep/issues/14867
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
type invalidRecursiveObjectType = {
  level1: {
    level2: {
      level3: {
        level4: {
          level5: invalidRecursiveObjectType
        }
      }
    }
  }
}

param p invalidRecursiveObjectType = |
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().NotContain(c => c.Label == "required-properties");
        }

        [TestMethod]
        public async Task Required_properties_completion_works_for_valid_recursive_types()
        {
            // https://github.com/Azure/bicep/issues/14867
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
type validRecursiveObjectType = {
  level1: {
    level2: {
      level3: {
        level4: {
          level5: validRecursiveObjectType?
        }
      }
    }
  }
}

param p validRecursiveObjectType = |
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            var updatedFile = file.ApplyCompletion(completions, "required-properties");

            updatedFile.Should().HaveSourceText("""
type validRecursiveObjectType = {
  level1: {
    level2: {
      level3: {
        level4: {
          level5: validRecursiveObjectType?
        }
      }
    }
  }
}

param p validRecursiveObjectType = {
  level1: {
    level2: {
      level3: {
        level4: {}
      }
    }
  }
}|
""");
        }

        [TestMethod]
        public async Task Compile_time_imports_offer_target_path_completions()
        {
            var mainContent = """
              import * as foo from |
              """;

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");
            var files = new Dictionary<DocumentUri, string>
            {
                [InMemoryFileResolver.GetFileUri("/path/to/mod.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/mod2.bicep")] = "",
                [InMemoryFileResolver.GetFileUri("/path/to/mod2.json")] = @"{ ""schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"" }",
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestAndResolveCompletions(cursors[0]);

            completions.Should().Contain(c => c.Label == "mod.bicep" && c.Kind == CompletionItemKind.File);
            completions.Should().Contain(c => c.Label == "mod2.bicep" && c.Kind == CompletionItemKind.File);
            completions.Should().Contain(c => c.Label == "mod2.json" && c.Kind == CompletionItemKind.File);
            completions.Should().Contain(c => c.Label == "../" && c.Kind == CompletionItemKind.Folder);
            completions.Should().Contain(c => c.Label == "br/public:" && c.Kind == CompletionItemKind.Reference);
            completions.Should().Contain(c => c.Label == "br:" && c.Kind == CompletionItemKind.Reference);
            completions.Should().Contain(c => c.Label == "ts:" && c.Kind == CompletionItemKind.Reference);
        }

        [TestMethod]
        public async Task Compile_time_imports_offer_import_expression_completions()
        {
            var fileWithCursors = """
              import |
              """;

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(x => x.Label == "{}");
            completions.Should().Contain(x => x.Label == "* as");
        }

        [TestMethod]
        public async Task Compile_time_imports_offer_as_keyword_completions()
        {
            var fileWithCursors = """
              import * |
              """;

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, '|');
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Should().Contain(x => x.Label == "as");
        }

        [TestMethod]
        public async Task Compile_time_imports_offer_imported_symbol_list_item_completions()
        {
            var modContent = """
              @export()
              type foo = string

              @export()
              type bar = int
              """;

            var mod2Content = """
              @export()
              type fizz = string

              @export()
              type buzz = int
              """;

            var mainContent = """
              import {|} from 'mod.bicep'
              import {|} from 'mod2.bicep'
              """;

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = modContent,
                ["file:///mod2.bicep"] = mod2Content,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);

            var completions = await file.RequestAndResolveCompletions(cursors[0]);
            completions.Should().Contain(c => c.Label == "foo");
            completions.Should().Contain(c => c.Label == "bar");
            completions.Should().NotContain(c => c.Label == "fizz");
            completions.Should().NotContain(c => c.Label == "buzz");

            completions = await file.RequestAndResolveCompletions(cursors[1]);
            completions.Should().Contain(c => c.Label == "fizz");
            completions.Should().Contain(c => c.Label == "buzz");
            completions.Should().NotContain(c => c.Label == "foo");
            completions.Should().NotContain(c => c.Label == "bar");
        }

        [TestMethod]
        public async Task Compile_time_imports_do_not_offer_types_as_imported_symbol_list_item_completions_in_bicepparam_files()
        {
            var modContent = """
              @export()
              type foo = string

              @export()
              var bar = 'bar'
              """;

            var paramsContent = """
              import {|} from 'mod.bicep'
              """;

            var (text, cursors) = ParserHelper.GetFileWithCursors(paramsContent, '|');
            DocumentUri mainUri = "file:///params.bicepparam";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = modContent,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);

            var completions = await file.RequestAndResolveCompletions(cursors[0]);
            completions.Should().NotContain(c => c.Label == "foo");
            completions.Should().Contain(c => c.Label == "bar");
        }

        [TestMethod]
        public async Task Import_completions_work_between_braces()
        {
            // https://github.com/Azure/bicep/issues/16934
            var serverHelper = new ServerRequestHelper(TestContext, DefaultServer);
            var folder = $"{Guid.NewGuid():D}";

            await serverHelper.OpenFile($"/{folder}/mod.bicep", """
@export()
var bar = 'bar'
""");

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
import { | } from 'mod.bicep'
""");
            var mainFile = await serverHelper.OpenFile($"/{folder}/main.bicep", text);

            var newFile = await mainFile.RequestAndApplyCompletion(cursor, "bar");
            newFile.Should().HaveSourceText("""
import { bar| } from 'mod.bicep'
""");
        }

        [TestMethod]
        public async Task Import_completions_work_after_commas()
        {
            // https://github.com/Azure/bicep/issues/16934
            var serverHelper = new ServerRequestHelper(TestContext, DefaultServer);
            var folder = $"{Guid.NewGuid():D}";

            await serverHelper.OpenFile($"/{folder}/mod.bicep", """
@export()
var foo = 'bar'

@export()
var bar = 'bar'
""");

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
import { foo, | } from 'mod.bicep'
""");
            var mainFile = await serverHelper.OpenFile($"/{folder}/main.bicep", text);

            var newFile = await mainFile.RequestAndApplyCompletion(cursor, "bar");
            newFile.Should().HaveSourceText("""
import { foo, bar| } from 'mod.bicep'
""");
        }

        [TestMethod]
        public async Task Imported_symbol_list_item_completions_quote_and_escape_names_when_name_is_not_a_valid_identifier()
        {
            var jsonModContent = $$"""
              {
                "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                "contentVersion": "1.0.0.0",
                "metadata": {
                  "{{LanguageConstants.TemplateMetadataExportedVariablesName}}": [
                    {
                      "name": "foo.bar"
                    },
                    {
                      "name": "'"
                    }
                  ]
                },
                "variables": {
                  "foo.bar": "baz",
                  "'": "apostrophe"
                },
                "resources": []
              }
              """;

            var mainContent = "import {|} from 'mod.json'";

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.json"] = jsonModContent,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);

            HashSet<string> expectedContent = new()
            {
                "import {'foo.bar' as } from 'mod.json'",
                @"import {'\'' as } from 'mod.json'",
            };

            foreach (var completion in await file.RequestAndResolveCompletions(cursors[0]))
            {
                var start = PositionHelper.GetOffset(bicepFile.LineStarts, completion.TextEdit!.TextEdit!.Range.Start);
                var end = PositionHelper.GetOffset(bicepFile.LineStarts, completion.TextEdit!.TextEdit!.Range.End);
                var textToInsert = completion.TextEdit!.TextEdit!.NewText;
                var updated = text[..start] + textToInsert + text[end..];
                expectedContent.Remove(updated).Should().BeTrue();
            }

            expectedContent.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Compile_time_imports_offer_imported_wildcard_property_completions()
        {
            var modContent = """
              @export()
              type foo = string

              @export()
              type bar = int
              """;

            var mod2Content = """
              @export()
              type fizz = string

              @export()
              type buzz = int
              """;

            var mainContent = """
              import * as mod from 'mod.bicep'
              import * as mod2 from 'mod2.bicep'

              type a = mod.|
              type b = mod2.|
              """;

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = modContent,
                ["file:///mod2.bicep"] = mod2Content,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);

            var completions = await file.RequestAndResolveCompletions(cursors[0]);
            completions.Should().Contain(c => c.Label == "foo");
            completions.Should().Contain(c => c.Label == "bar");
            completions.Should().NotContain(c => c.Label == "fizz");
            completions.Should().NotContain(c => c.Label == "buzz");

            completions = await file.RequestAndResolveCompletions(cursors[1]);
            completions.Should().Contain(c => c.Label == "fizz");
            completions.Should().Contain(c => c.Label == "buzz");
            completions.Should().NotContain(c => c.Label == "foo");
            completions.Should().NotContain(c => c.Label == "bar");
        }

        [TestMethod]
        public async Task Imported_wildcard_property_completions_use_array_access_when_name_is_not_a_valid_identifier()
        {
            var jsonModContent = $$"""
              {
                "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                "contentVersion": "1.0.0.0",
                "languageVersion": "2.0",
                "definitions": {
                  "foo.bar": {
                    "type": "string",
                    "metadata": {
                      "{{LanguageConstants.MetadataExportedPropertyName}}": true
                    }
                  },
                  "'": {
                    "type": "string",
                    "metadata": {
                      "{{LanguageConstants.MetadataExportedPropertyName}}": true
                    }
                  },
                  "fizz": {
                    "type": "string",
                    "metadata": {
                      "{{LanguageConstants.MetadataExportedPropertyName}}": true
                    }
                  }
                },
                "resources": {}
              }
              """;

            var mainContent = """
              import * as mod from 'mod.json'

              type a = |
              """;

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.json"] = jsonModContent,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);

            var completions = await file.RequestAndResolveCompletions(cursors[0]);
            completions.Should().Contain(c => c.Label == "mod['foo.bar']");
            completions.Should().Contain(c => c.Label == @"mod['\'']");
            completions.Should().Contain(c => c.Label == "mod.fizz");
        }

        [TestMethod]
        public async Task Imported_type_completions_are_offered_within_type_syntax()
        {
            var modContent = """
              @export()
              type foo = string

              @export()
              type bar = int
              """;

            var mod2Content = """
              @export()
              type fizz = string

              @export()
              type buzz = int
              """;

            var mainContent = """
              import {foo, bar} from 'mod.bicep'
              import * as mod2 from 'mod2.bicep'

              param a |
              output b |
              type c = |
              type d = {
                property: |
              }
              """;

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = modContent,
                ["file:///mod2.bicep"] = mod2Content,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);

            foreach (var cursor in cursors)
            {
                var completions = await file.RequestAndResolveCompletions(cursor);
                completions.Should().Contain(c => c.Label == "foo");
                completions.Should().Contain(c => c.Label == "bar");
                completions.Should().Contain(c => c.Label == "mod2.fizz");
                completions.Should().Contain(c => c.Label == "mod2.buzz");
            }
        }

        [TestMethod]
        public async Task Compile_time_imports_offer_imported_symbol_property_completions()
        {
            var modContent = """
              @export()
              var foo = {
                bar: 'bar'
                baz: 'baz'
              }
              """;

            var mod2Content = """
              @export()
              var fizz = {
                buzz: 'buzz'
                pop: 'pop'
              }
              """;

            var mainContent = """
              import {foo} from 'mod.bicep'
              import {fizz} from 'mod2.bicep'

              output obj object = {
                bar: foo.|
                pop: fizz.|
              }
              """;

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = "file:///main.bicep";
            var files = new Dictionary<DocumentUri, string>
            {
                ["file:///mod.bicep"] = modContent,
                ["file:///mod2.bicep"] = mod2Content,
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri);

            var file = new FileRequestHelper(helper.Client, bicepFile);

            var completions = await file.RequestAndResolveCompletions(cursors[0]);
            completions.Should().Contain(c => c.Label == "bar");
            completions.Should().Contain(c => c.Label == "baz");
            completions.Should().NotContain(c => c.Label == "buzz");
            completions.Should().NotContain(c => c.Label == "pop");

            completions = await file.RequestAndResolveCompletions(cursors[1]);
            completions.Should().Contain(c => c.Label == "buzz");
            completions.Should().Contain(c => c.Label == "pop");
            completions.Should().NotContain(c => c.Label == "bar");
            completions.Should().NotContain(c => c.Label == "baz");
        }

        [TestMethod]
        public async Task Description_markdown_is_correctly_formatted()
        {
            // https://github.com/Azure/bicep/issues/12412
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
type foo = {
  @description('''Source port ranges.
    Can be a single valid port number, a range in the form of \<start\>-\<end\>, or a * for any ports.
    When a wildcard is used, that needs to be the only value.''')
  sourcePortRanges: string[]
}

param foo1 foo = {
  so|
}
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);
            completions.Single(x => x.Label == "sourcePortRanges").Documentation!.MarkupContent!.Value
                .Should().BeEquivalentToIgnoringNewlines(
                    @"Type: `string[]`  " + @"
Source port ranges.
Can be a single valid port number, a range in the form of \<start\>-\<end\>, or a * for any ports.
When a wildcard is used, that needs to be the only value.  " + @"
");
        }

        [TestMethod]
        public async Task Resource_utility_type_offered_as_completion_if_enabled()
        {
            var mainContent = """
                type acct = |
                """;

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                text,
                mainUri);

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestAndResolveCompletions(cursors[0]);

            var updated = file.ApplyCompletion(completions, "resourceInput");
            updated.Should().HaveSourceText("""
                type acct = resourceInput<'|'>
                """);
        }

        [TestMethod]
        public async Task Legacy_resource_utility_type_offered_not_as_completion_if_enabled()
        {
            var mainContent = """
                type acct = |
                """;

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                text,
                mainUri);

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestAndResolveCompletions(cursors[0]);

            completions.Should().NotContain(completion => completion.Label == LanguageConstants.TypeNameResource);
        }

        [TestMethod]
        public async Task Resource_types_offered_as_completion_for_single_argument_to_resource_utility_type()
        {
            var mainContent = """
                type acct = resourceInput<stor|>
                type fullyQualified = sys.resourceInput<stor|>
                """;

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                text,
                mainUri);

            var file = new FileRequestHelper(helper.Client, bicepFile);

            var completions = await file.RequestAndResolveCompletions(cursors[0]);
            var updated = file.ApplyCompletion(completions, "'Microsoft.Storage/storageAccounts'");
            updated.Should().HaveSourceText("""
                type acct = resourceInput<'Microsoft.Storage/storageAccounts@|'>
                type fullyQualified = sys.resourceInput<stor>
                """);

            completions = await file.RequestAndResolveCompletions(cursors[1]);
            updated = file.ApplyCompletion(completions, "'Microsoft.Storage/storageAccounts'");
            updated.Should().HaveSourceText("""
                type acct = resourceInput<stor>
                type fullyQualified = sys.resourceInput<'Microsoft.Storage/storageAccounts@|'>
                """);
        }

        [TestMethod]
        public async Task Resource_api_versions_offered_as_completion_for_single_argument_to_resource_utility_type_with_resource_type_name_already_filled_in()
        {
            var mainContent = """
                type acct = resourceInput<'Microsoft.Storage/storageAccounts@|'>
                type fullyQualified = sys.resourceInput<'Microsoft.Storage/storageAccounts@|'>
                """;

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                text,
                mainUri);

            var file = new FileRequestHelper(helper.Client, bicepFile);

            var completions = await file.RequestAndResolveCompletions(cursors[0]);
            var updated = file.ApplyCompletion(completions, "2022-09-01");
            updated.Should().HaveSourceText("""
                type acct = resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'|>
                type fullyQualified = sys.resourceInput<'Microsoft.Storage/storageAccounts@'>
                """);

            completions = await file.RequestAndResolveCompletions(cursors[1]);
            updated = file.ApplyCompletion(completions, "2022-09-01");
            updated.Should().HaveSourceText("""
                type acct = resourceInput<'Microsoft.Storage/storageAccounts@'>
                type fullyQualified = sys.resourceInput<'Microsoft.Storage/storageAccounts@2022-09-01'|>
                """);
        }

        [TestMethod]
        public async Task Type_properties_are_offered_as_completions_within_type_clause()
        {
            var fileWithCursors = """
              type foo = {
                bar: {
                  baz: string
                  quux: {
                    *: {
                      pop: int
                    }
                  }
                }
              }

              type completeMe = foo.|
              type completeMeToo = foo.bar.|
              type additionalPropertiesCompletion = foo.bar.quux.|
              """;

            await RunCompletionScenarioTest(TestContext, ServerWithNamespaceProvider, fileWithCursors, completionLists =>
            {
                completionLists.Count().Should().Be(3);

                var completionList = completionLists.First();
                completionList.Should().SatisfyRespectively(i => i.Label.Should().Be("bar"));

                completionList = completionLists.Skip(1).First();
                completionList.Should().SatisfyRespectively(i => i.Label.Should().Be("baz"),
                  i => i.Label.Should().Be("quux"));

                completionList = completionLists.Skip(2).First();
                completionList.Should().SatisfyRespectively(i => i.Label.Should().Be("*"));
            });
        }

        [TestMethod]
        public async Task Strings_in_required_property_completions_are_correctly_escaped()
        {
            var fileWithCursors = """
@discriminator('odata.type')
type alertType = alertWebtestType | alertResourceType | alertMultiResourceType
type alertResourceType = {
  'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
  allof: array
}
type alertMultiResourceType = {
  'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
  allof: array
}
type alertWebtestType = {
  'odata.type': 'Microsoft.Azure.Monitor.WebtestLocationAvailabilityCriteria'
  componentId: string
  failedLocationCount: int
  webTestId: string
}

param myAlert alertType = |>
""";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, "|>");
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);

            var updatedFile = file.ApplyCompletion(completions, "required-properties-Microsoft.Azure.Monitor.WebtestLocationAvailabilityCriteria");
            updatedFile.Should().HaveSourceText("""
@discriminator('odata.type')
type alertType = alertWebtestType | alertResourceType | alertMultiResourceType
type alertResourceType = {
  'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
  allof: array
}
type alertMultiResourceType = {
  'odata.type': 'Microsoft.Azure.Monitor.MultipleResourceMultipleMetricCriteria'
  allof: array
}
type alertWebtestType = {
  'odata.type': 'Microsoft.Azure.Monitor.WebtestLocationAvailabilityCriteria'
  componentId: string
  failedLocationCount: int
  webTestId: string
}

param myAlert alertType = {
  componentId: $1
  failedLocationCount: $2
  'odata.type': 'Microsoft.Azure.Monitor.WebtestLocationAvailabilityCriteria'
  webTestId: $3
}|
""");
        }

        [TestMethod]
        public async Task Nested_tab_stops_are_correctly_ordered_in_required_properties_snippet()
        {
            var fileWithCursors = """
type nestedType = {
  foo: string
  bar: {
    bar: string
  }
  baz: string
}

param test nestedType = |>
""";

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursors, "|>");
            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);

            var completions = await file.RequestAndResolveCompletions(cursor);

            var updatedFile = file.ApplyCompletion(completions, "required-properties");
            updatedFile.Should().HaveSourceText("""
type nestedType = {
  foo: string
  bar: {
    bar: string
  }
  baz: string
}

param test nestedType = {
  bar: {
    bar: $1
  }
  baz: $2
  foo: $3
}|
""");
        }

        [TestMethod]
        public async Task Unions_of_object_types_support_completions()
        {
            // https://github.com/azure/bicep/issues/14839
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
var items = [
  { bar: 'abc' }
  { bar: 'def' }
]

output foo string[] = [for item in items: item.|]
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);

            var updatedFile = file.ApplyCompletion(completions, "bar");
            updatedFile.Should().HaveSourceText("""
var items = [
  { bar: 'abc' }
  { bar: 'def' }
]

output foo string[] = [for item in items: item.bar|]
""");
        }

        [TestMethod]
        public async Task Unions_of_object_types_support_completions_with_additional_properties()
        {
            // https://github.com/azure/bicep/issues/14839
            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
param firstItem object

var items = [
  firstItem
  { bar: 'def' }
]

output foo string[] = [for item in items: item.|]
""");

            var file = await new ServerRequestHelper(TestContext, DefaultServer).OpenFile(text);
            var completions = await file.RequestAndResolveCompletions(cursor);

            // bar is still offered as a completion, even though there may be other properties supported
            var updatedFile = file.ApplyCompletion(completions, "bar");
            updatedFile.Should().HaveSourceText("""
param firstItem object

var items = [
  firstItem
  { bar: 'def' }
]

output foo string[] = [for item in items: item.bar|]
""");
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/15569
        public async Task String_literal_union_with_object_value_should_not_cause_stack_overflow()
        {
            var serverHelper = new ServerRequestHelper(TestContext, DefaultServer);

            // single parameter of string literal union type
            var moduleText = "param foo 'foo' | 'bar'";
            var moduleFile = await serverHelper.OpenFile("/mod.bicep", moduleText);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
targetScope = 'resourceGroup'

module mod 'mod.bicep' = {
  name: ''
  params: {
    foo: {
      |
    }
  }
}
""");
            var mainFile = await serverHelper.OpenFile("/main.bicep", text);

            var completions = await mainFile.RequestAndResolveCompletions(cursor);
            completions.Should().BeEmpty();
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/14429
        public async Task Lambda_output_type_completions_are_offered()
        {
            var serverHelper = new ServerRequestHelper(TestContext, DefaultServer);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
type fooType = {
  bar: 'bar'
}

func fooFunc() fooType => {
  |
}
""");
            var mainFile = await serverHelper.OpenFile(text);

            var newFile = await mainFile.RequestAndApplyCompletion(cursor, "bar");

            newFile.Should().HaveSourceText("""
type fooType = {
  bar: 'bar'
}

func fooFunc() fooType => {
  bar:|
}
""");
        }

        [TestMethod]
        public async Task Typed_variable_post_name_completions_are_offered()
        {
            var serverHelper = new ServerRequestHelper(TestContext, ServerWithNamespaceProvider);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
type fooType = {
  bar: 'bar'
}

var foo |
""");
            var mainFile = await serverHelper.OpenFile(text);

            var newFile = await mainFile.RequestAndApplyCompletion(cursor, "fooType");
            newFile.Should().HaveSourceText("""
type fooType = {
  bar: 'bar'
}

var foo fooType|
""");
        }

        [TestMethod]
        public async Task Typed_variable_value_completions_are_offered()
        {
            var serverHelper = new ServerRequestHelper(TestContext, ServerWithNamespaceProvider);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
type fooType = {
  bar: 'bar'
}

var foo fooType = |
""");
            var mainFile = await serverHelper.OpenFile(text);

            var newFile = await mainFile.RequestAndApplyCompletion(cursor, "required-properties", ["bar"]);
            newFile.Should().HaveSourceText("""
type fooType = {
  bar: 'bar'
}

var foo fooType = {
  bar: bar
}|
""");
        }

        [TestMethod]
        public async Task Typed_variable_object_property_completions_are_offered()
        {
            var serverHelper = new ServerRequestHelper(TestContext, ServerWithNamespaceProvider);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
type fooType = {
  bar: 'bar'
}

var foo fooType = {
  |
}
""");
            var mainFile = await serverHelper.OpenFile(text);

            var newFile = await mainFile.RequestAndApplyCompletion(cursor, "bar");
            newFile.Should().HaveSourceText("""
type fooType = {
  bar: 'bar'
}

var foo fooType = {
  bar:|
}
""");
        }

        [TestMethod]
        public async Task Typed_variable_type_completions_are_offered()
        {
            var serverHelper = new ServerRequestHelper(TestContext, ServerWithNamespaceProvider);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
var foo | = [
  'bar'
]
""");
            var mainFile = await serverHelper.OpenFile(text);

            var newFile = await mainFile.RequestAndApplyCompletion(cursor, "array");
            newFile.Should().HaveSourceText("""
var foo array| = [
  'bar'
]
""");
        }

        [TestMethod]
        public async Task Typed_variable_type_completions_are_offered_2()
        {
            var serverHelper = new ServerRequestHelper(TestContext, ServerWithNamespaceProvider);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
var foo a| = [
  'bar'
]
""");
            var mainFile = await serverHelper.OpenFile(text);

            var newFile = await mainFile.RequestAndApplyCompletion(cursor, "array");
            newFile.Should().HaveSourceText("""
var foo array| = [
  'bar'
]
""");
        }

        [TestMethod] // https://github.com/Azure/bicep/issues/16556
        public Task Array_object_type_completions_are_offered() => RunCompletionTest("""
type Person = {
  name: string
  age: int
}

output people Person[] = [{
  |
}]
""",
          "name", """
type Person = {
  name: string
  age: int
}

output people Person[] = [{
  name:|
}]
""");

        [TestMethod]
        public async Task Resource_types_offered_as_completion_for_single_argument_to_resource_utility_type_with_unclosed_chevrons()
        {
            var mainContent = """
                type acct = resourceInput<stor|
                """;

            var (text, cursors) = ParserHelper.GetFileWithCursors(mainContent, '|');
            DocumentUri mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                text,
                mainUri);

            var file = new FileRequestHelper(helper.Client, bicepFile);

            var completions = await file.RequestAndResolveCompletions(cursors[0]);
            var updated = file.ApplyCompletion(completions, "'Microsoft.Storage/storageAccounts'");
            updated.Should().HaveSourceText("""
                type acct = resourceInput<'Microsoft.Storage/storageAccounts@|'
                """);
        }

        [TestMethod]
        public async Task LoadFunctionsPathArgument_returnsFilesInCompletions_withUnclosedParentheses()
        {
            var mainUri = InMemoryFileResolver.GetFileUri("/path/to/main.bicep");

            var (mainFileText, cursor) = ParserHelper.GetFileWithSingleCursor("var file = loadJsonContent('|'", '|');
            var mainFile = new LanguageClientFile(mainUri, mainFileText);

            var fileTextsByUri = new Dictionary<DocumentUri, string>
            {
                [mainUri] = mainFileText,
                [InMemoryFileResolver.GetFileUri("/path/to/json1.json")] = "{}",
            };

            using var helper = await LanguageServerHelper.StartServerWithText(
                TestContext,
                fileTextsByUri,
                mainUri,
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));

            var file = new FileRequestHelper(helper.Client, mainFile);

            var completions = await file.RequestAndResolveCompletions(cursor);

            var completionItems = completions.Where(x => x.Kind == CompletionItemKind.File).OrderBy(x => x.SortText);
            completionItems.Should().SatisfyRespectively(x => x.Label.Should().Be("json1.json"));
        }

        [TestMethod]
        public async Task Readonly_required_properties_are_not_offered_as_completions()
        {
            var customTypes = new[] {
                TestTypeHelper.CreateCustomResourceTypeWithTopLevelProperties("My.Rp/myType", "2020-01-01", TypeSymbolValidationFlags.Default, [
                    new NamedTypeProperty("required", LanguageConstants.String, TypePropertyFlags.Required),
                    new NamedTypeProperty("readOnlyRequired", LanguageConstants.String, TypePropertyFlags.ReadOnly | TypePropertyFlags.Required),
                ]),
            };

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
            resource myRes 'My.Rp/myType@2020-01-01' = {
              name: 'foo'
              |
            }
            
            output readOnlyRequired string = myRes.readOnlyRequired
            """);

            var bicepFile = new LanguageClientFile(InMemoryFileResolver.GetFileUri("/path/to/main.bicep"), text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                TestContext,
                text,
                bicepFile.Uri,
                services => services.WithAzResources(customTypes));

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestAndResolveCompletions(cursor);

            completions.Should().Contain(x => x.Label == "required");
            completions.Should().NotContain(x => x.Label == "readOnlyRequired");
        }

        [TestMethod]
        public async Task Write_only_properties_are_not_offered_as_completions_this_namespace()
        {
            var customTypes = new[] {
                TestTypeHelper.CreateCustomResourceTypeWithTopLevelProperties("My.Rp/myType", "2020-01-01", TypeSymbolValidationFlags.Default, null, [
                    new NamedTypeProperty("required", LanguageConstants.String, TypePropertyFlags.Required),
                    new NamedTypeProperty("readOnlyRequired", LanguageConstants.String, TypePropertyFlags.ReadOnly | TypePropertyFlags.Required),
                    new NamedTypeProperty("writeOnly", LanguageConstants.String, TypePropertyFlags.WriteOnly),
                ]),
            };

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
            resource myRes 'My.Rp/myType@2020-01-01' = {
              name: 'foo'
              properties: {
                required: this.existingResource().?properties.|
              }
            }
            """);

            var bicepFile = new LanguageClientFile(InMemoryFileResolver.GetFileUri("/path/to/main.bicep"), text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                TestContext,
                text,
                bicepFile.Uri,
                services => services.WithFeatureOverrides(new(ThisNamespaceEnabled: true)).WithAzResources(customTypes));

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestAndResolveCompletions(cursor);

            completions.Should().Contain(x => x.Label == "required");
            completions.Should().Contain(x => x.Label == "readOnlyRequired");
            completions.Should().NotContain(x => x.Label == "writeOnly");
        }

        [TestMethod]
        public async Task Identity_property_completions_are_offered_for_resource()
        {
            // Resource identity property completion
            var resourceFileWithCursor = """
resource myRes 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: 'myRes'
  identity: |
}
""";
            await RunCompletionScenarioTest(
                this.TestContext,
                ServerWithNamespaceProvider,
                resourceFileWithCursor,
                completionLists =>
                {
                    completionLists.Count().Should().Be(1);
                    var identitySnippets = completionLists.First().Items
                        .Where(x => x.Kind == CompletionItemKind.Snippet)
                        .Select(x => x.Label)
                        .ToList();

                    identitySnippets.Should().Contain("user-assigned-identity");
                    identitySnippets.Should().Contain("system-assigned-identity");
                    identitySnippets.Should().Contain("user-and-system-assigned-identity");
                    identitySnippets.Should().Contain("none-identity");
                    identitySnippets.Should().Contain("user-assigned-identity-array");
                },
                '|');
        }

        [TestMethod]
        public async Task Identity_property_completions_are_offered_for_module()
        {
            // Module identity property completion (when feature is enabled)
            var moduleFileWithCursor = """
module myMod './mod.bicep' = {
  name: 'myMod'
  identity: |
}
""";
            var (text, cursors) = ParserHelper.GetFileWithCursors(moduleFileWithCursor, '|');
            DocumentUri mainUri = DocumentUri.From("file:///main.bicep");
            var files = new Dictionary<DocumentUri, string>
            {
                [DocumentUri.From("file:///mod.bicep")] = """
param foo string = 'bar'
""",
                [mainUri] = text
            };

            var bicepFile = new LanguageClientFile(mainUri, text);
            using var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                files,
                bicepFile.Uri,
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create())
            );

            var file = new FileRequestHelper(helper.Client, bicepFile);
            var completions = await file.RequestCompletions(cursors);

            completions.Count().Should().Be(1);
            var identitySnippets = completions.First().Items
                .Where(x => x.Kind == CompletionItemKind.Snippet)
                .Select(x => x.Label)
                .ToList();

            identitySnippets.Should().Contain("user-assigned-identity");
            identitySnippets.Should().Contain("none-identity");
            identitySnippets.Should().Contain("user-assigned-identity-array");
        }

        [TestMethod]
        public async Task Using_with_keyword_completions_require_experimental_feature()
        {
            var helper = new ServerRequestHelper(TestContext, DefaultServer);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
            using 'main.bicep' |
            """);

            var bicepFile = await helper.OpenFile("/path/to/main.bicep", "");
            var bicepParamFile = await helper.OpenFile("/path/to/main.bicepparam", text);

            var completions = await bicepParamFile.RequestAndResolveCompletions(cursor);
            completions.Should().NotContain(x => x.Label == "with");
        }

        [TestMethod]
        public async Task Using_with_keyword_completions_work()
        {
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(
                TestContext,
                s => s.WithFeatureOverrides(new(DeployCommandsEnabled: true)));
            var helper = new ServerRequestHelper(TestContext, server);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
            using 'main.bicep' |
            """);

            var bicepFile = await helper.OpenFile("/path/to/main.bicep", "");
            var bicepParamFile = await helper.OpenFile("/path/to/main.bicepparam", text);

            var completions = await bicepParamFile.RequestAndResolveCompletions(cursor);

            var updatedFile = bicepParamFile.ApplyCompletion(completions, "with");

            updatedFile.Should().HaveSourceText("""
            using 'main.bicep' with|
            """);
        }

        [TestMethod]
        public async Task Using_with_completions_work()
        {
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(
                TestContext,
                s => s.WithFeatureOverrides(new(DeployCommandsEnabled: true)));
            var helper = new ServerRequestHelper(TestContext, server);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
            using 'main.bicep' with |
            """);

            var bicepFile = await helper.OpenFile("/path/to/main.bicep", "");
            var bicepParamFile = await helper.OpenFile("/path/to/main.bicepparam", text);

            var completions = await bicepParamFile.RequestAndResolveCompletions(cursor);

            var updatedFile = bicepParamFile.ApplyCompletion(
                completions,
                "required-properties-stack",
                [
                    "'/subscriptions/foo/resourceGroups/bar'",
                    "'delete'",
                    "'denyDelete'",
                ]);

            updatedFile.Should().HaveSourceText("""
            using 'main.bicep' with {
              scope: '/subscriptions/foo/resourceGroups/bar'
              actionOnUnmanage: {
                resources: 'delete'
              }
              denySettings: {
                mode: 'denyDelete'
              }
              mode: 'stack'
            }|
            """);
        }

        [TestMethod]
        public async Task Using_with_property_completions_work()
        {
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(
                TestContext,
                s => s.WithFeatureOverrides(new(DeployCommandsEnabled: true)));
            var helper = new ServerRequestHelper(TestContext, server);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
            using 'main.bicep' with {
              mode: 'deployment'
              |
            }
            """);

            var bicepFile = await helper.OpenFile("/path/to/main.bicep", "");
            var bicepParamFile = await helper.OpenFile("/path/to/main.bicepparam", text);

            var completions = await bicepParamFile.RequestAndResolveCompletions(cursor);

            var updatedFile = bicepParamFile.ApplyCompletion(completions, "scope");

            updatedFile.Should().HaveSourceText("""
            using 'main.bicep' with {
              mode: 'deployment'
              scope:|
            }
            """);
        }

        [TestMethod]
        public async Task Using_with_discriminator_completions_work()
        {
            using var server = await MultiFileLanguageServerHelper.StartLanguageServer(
                TestContext,
                s => s.WithFeatureOverrides(new(DeployCommandsEnabled: true)));
            var helper = new ServerRequestHelper(TestContext, server);

            var (text, cursor) = ParserHelper.GetFileWithSingleCursor("""
            using 'main.bicep' with {
              mode: |
            }
            """);

            var bicepFile = await helper.OpenFile("/path/to/main.bicep", "");
            var bicepParamFile = await helper.OpenFile("/path/to/main.bicepparam", text);

            var completions = await bicepParamFile.RequestAndResolveCompletions(cursor);

            var updatedFile = bicepParamFile.ApplyCompletion(completions, "'stack'");

            updatedFile.Should().HaveSourceText("""
            using 'main.bicep' with {
              mode: 'stack'|
            }
            """);
        }
    }
}
