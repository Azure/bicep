// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Bicep.LangServer.IntegrationTests.Completions
{
    [TestClass]
    public class ParamsCompletionTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DataRow(
@"
//Parameters file

using './main.bicep'

param |",

@"
//Bicep file

param firstParam int
param secondParam string

",
new string[] { "firstParam", "secondParam" },
new CompletionItemKind[] { CompletionItemKind.Field, CompletionItemKind.Field }
)
]
        [DataRow(
@"
//Parameters file

using './main.bicep'

param |",

@"
//Bicep file

param firstParam int = 1
param secondParam string
param thirdParam string = 'hello'

",
new string[] { "firstParam", "secondParam", "thirdParam" },
new CompletionItemKind[] { CompletionItemKind.Field, CompletionItemKind.Field, CompletionItemKind.Field }
)
]
        [DataRow(
@"
//Parameters file

using './main.bicep'

param firstParam = 5
param |",

@"
//Bicep file

param firstParam int = 1
param secondParam string
param thirdParam string = 'hello'

",
new string[] { "secondParam", "thirdParam" },
new CompletionItemKind[] { CompletionItemKind.Field, CompletionItemKind.Field }
)
]
        [DataRow(
@"
//Parameters file

using './main.bicep'

param |",

@"
//Bicep file

var firstVar = 'hello'
",
new string[] { },
new CompletionItemKind[] { }
)
]
        [DataRow(
@"
//Parameters file

using './main.bicep'

param | = 1",

@"
//Bicep file

param firstParam int
param secondParam string

",
new string[] { "firstParam", "secondParam" },
new CompletionItemKind[] { CompletionItemKind.Field, CompletionItemKind.Field }
)
]
        public async Task Request_for_parameter_identifier_completions_should_return_correct_identifiers(string paramTextWithCursor, string bicepText, string[] completionLables, CompletionItemKind[] completionItemKinds)
        {
            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [InMemoryFileResolver.GetFileUri("/path/to/main.bicep")] = bicepText
            };

            var completions = await RunCompletionScenario(paramTextWithCursor, fileTextsByUri.ToImmutableDictionary(), '|');

            var expectedValueIndex = 0;
            foreach (var completion in completions)
            {
                completion.Label.Should().Be(completionLables[expectedValueIndex]);
                completion.Kind.Should().Be(completionItemKinds[expectedValueIndex]);
                expectedValueIndex += 1;
            }
        }

        [DataRow(@"using './main.bicep'
param myBool = |", @"param myBool bool", new[] { "false", "true" }, new[] { CompletionItemKind.Keyword, CompletionItemKind.Keyword })]
        [DataRow(@"using './main.bicep'
param myBool =|", @"param myBool bool", new[] { "false", "true" }, new[] { CompletionItemKind.Keyword, CompletionItemKind.Keyword })]
        [DataRow(@"using './main.bicep'
param myArray = |", @"param myArray array", new[] { "[]" }, new[] { CompletionItemKind.Value })]
        [DataRow(@"using './main.bicep'
param myArray =|", @"param myArray array", new[] { "[]" }, new[] { CompletionItemKind.Value })]
        [DataRow(@"using './main.bicep'
param myObj = |", @"param myObj object", new[] { "{}" }, new[] { CompletionItemKind.Snippet })]
        [DataRow(@"using './main.bicep'
param myObj =|", @"param myObj object", new[] { "{}" }, new[] { CompletionItemKind.Snippet })]
        [DataRow(@"using './main.bicep'
param firstParam = |", @"@allowed([
  'one'
  'two'
])
param firstParam string", new[] { "'one'", "'two'" }, new[] { CompletionItemKind.EnumMember, CompletionItemKind.EnumMember })]
        [DataRow(@"using './main.bicep'
param firstParam = 'o|'", @"@allowed([
  'one'
  'two'
])
param firstParam string", new[] { "'one'", "'two'" }, new[] { CompletionItemKind.EnumMember, CompletionItemKind.EnumMember })]
        [DataTestMethod]
        public async Task Value_completions_should_be_based_on_type(string paramTextWithCursor, string bicepText, string[] expectedLabels, CompletionItemKind[] expectedKinds)
        {
            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [InMemoryFileResolver.GetFileUri("/path/to/main.bicep")] = bicepText
            };

            var completions = await RunCompletionScenario(paramTextWithCursor, fileTextsByUri.ToImmutableDictionary(), '|');
            completions.Select(completion => completion.Label).Should().Contain(expectedLabels);
            completions.Select(completion => completion.Kind).Should().Contain(expectedKinds);
        }

        [TestMethod]
        public async Task Request_for_using_declaration_path_completions_should_return_correct_paths_for_file_directories()
        {
            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [InMemoryFileResolver.GetFileUri("/path/to/main1.bicep")] = "param foo int",
                [InMemoryFileResolver.GetFileUri("/path/to/main2.txt")] = "param bar int",
                [InMemoryFileResolver.GetFileUri("/path/to/nested1/main3.bicep")] = "param foo int",
                [InMemoryFileResolver.GetFileUri("/path/to/module1.bicep")] = "param foo string",
                [InMemoryFileResolver.GetFileUri("/path/to/nested1/module2.bicep")] = "param bar bool",
                [InMemoryFileResolver.GetFileUri("/path/to/nested2/module3.bicep")] = "param bar string"
            };

            var completions = await RunCompletionScenario(@"
using |
", fileTextsByUri.ToImmutableDictionary(), '|');

            completions.Should().SatisfyRespectively(
                x =>
                {
                    x.Label.Should().Be("main1.bicep");
                    x.Kind.Should().Be(CompletionItemKind.File);
                },
                x =>
                {
                    x.Label.Should().Be("module1.bicep");
                    x.Kind.Should().Be(CompletionItemKind.File);
                },
                x =>
                {
                    x.Label.Should().Be("../");
                    x.Kind.Should().Be(CompletionItemKind.Folder);
                },
                x =>
                {
                    x.Label.Should().Be("nested1/");
                    x.Kind.Should().Be(CompletionItemKind.Folder);
                },
                x =>
                {
                    x.Label.Should().Be("nested2/");
                    x.Kind.Should().Be(CompletionItemKind.Folder);
                });
        }

        [TestMethod]
        public async Task Request_for_using_declaration_path_completions_should_return_correct_partial_paths()
        {
            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [InMemoryFileResolver.GetFileUri("/path/to/main1.bicep")] = "param foo int",
                [InMemoryFileResolver.GetFileUri("/path/to/main2.txt")] = "param bar int",
                [InMemoryFileResolver.GetFileUri("/path/to/nested1/main3.bicep")] = "param foo int",
                [InMemoryFileResolver.GetFileUri("/path/to/module1.bicep")] = "param foo string",
                [InMemoryFileResolver.GetFileUri("/path/to/nested1/module2.bicep")] = "param bar bool",
                [InMemoryFileResolver.GetFileUri("/path/to/nested2/module3.bicep")] = "param bar string"
            };

            var completions = await RunCompletionScenario(@"
using './nested1/|'
", fileTextsByUri.ToImmutableDictionary(), '|');

            completions.Should().SatisfyRespectively(
                x =>
                {
                    x.Label.Should().Be("main3.bicep");
                    x.Kind.Should().Be(CompletionItemKind.File);
                },
                x =>
                {
                    x.Label.Should().Be("module2.bicep");
                    x.Kind.Should().Be(CompletionItemKind.File);
                },
                x =>
                {
                    x.Label.Should().Be("../");
                    x.Kind.Should().Be(CompletionItemKind.Folder);
                });
        }

        [DataRow(@"|")]
        [DataRow(@"
|")]
        [DataRow(@"param foo = 23
|")]
        [DataTestMethod]
        public async Task Param_file_should_have_keyword_completions(string text)
        {
            var completions = await RunCompletionScenario(text, ImmutableDictionary<Uri, string>.Empty, '|');

            completions.Should().SatisfyRespectively(
                x =>
                {
                    x.Label.Should().Be("param");
                    x.Detail.Should().Be("Parameter assignment keyword");
                    x.Kind.Should().Be(CompletionItemKind.Keyword);
                },
                x =>
                {
                    x.Label.Should().Be("using");
                    x.Detail.Should().Be("Using keyword");
                    x.Kind.Should().Be(CompletionItemKind.Keyword);
                });
        }

        [DataRow(@"using 'foo.bicep'
|")]
        [DataRow(@"using 'foo.bicep'
using 'bar.bicep'
|")]
        [DataTestMethod]
        public async Task Using_completion_should_only_be_offered_once(string paramTextWithCursor)
        {
            var completions = await RunCompletionScenario(paramTextWithCursor, ImmutableDictionary<Uri, string>.Empty, '|');

            completions.Should().SatisfyRespectively(
                x =>
                {
                    x.Label.Should().Be("param");
                    x.Detail.Should().Be("Parameter assignment keyword");
                    x.Kind.Should().Be(CompletionItemKind.Keyword);
                });
        }
        
        [TestMethod]
        public async Task Parameter_type_description_should_be_shown_for_params_symbol_completions()
        {
            var paramTextWithCursor = @"
using './main.bicep'

param |";

            var bicepText = @"
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
";
            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [InMemoryFileResolver.GetFileUri("/path/to/main.bicep")] = bicepText,
            }; 
        
            var completions = await RunCompletionScenario(paramTextWithCursor , fileTextsByUri.ToImmutableDictionary(), '|');

            completions.Should().SatisfyRespectively(
                x =>
                {
                    x.Label.Should().Be("myArray");
                    x.Documentation!.MarkupContent!.Value.Should().Be("Type: array");
                    x.Kind.Should().Be(CompletionItemKind.Field);
                },
                x =>
                {
                    x.Label.Should().Be("myBool");
                    x.Documentation!.MarkupContent!.Value.Should().Be("Type: bool  \nthis is a bool value");
                    x.Kind.Should().Be(CompletionItemKind.Field);
                },
                x =>
                {
                    x.Label.Should().Be("myInt");
                    x.Documentation!.MarkupContent!.Value.Should().Be("Type: 0 | 1  \nthis is an int value");
                    x.Kind.Should().Be(CompletionItemKind.Field);
                },
                x =>
                {
                    x.Label.Should().Be("myStr");
                    x.Documentation!.MarkupContent!.Value.Should().Be("Type: 'value1' | 'value2'  \nthis is a string value");
                    x.Kind.Should().Be(CompletionItemKind.Field);
                }
                );
        }

        private async Task<IEnumerable<CompletionItem>> RunCompletionScenario(string paramTextWithCursors, ImmutableDictionary<Uri, string> fileTextsByUri, char cursorInsertionMarker)
        {
            var paramUri = InMemoryFileResolver.GetFileUri("/path/to/param.bicepparam");
            var (paramFileTextNoCursor, cursor) = ParserHelper.GetFileWithSingleCursor(paramTextWithCursors, cursorInsertionMarker);
            var paramFile = SourceFileFactory.CreateBicepFile(paramUri, paramFileTextNoCursor);

            fileTextsByUri = fileTextsByUri.Add(paramUri, paramFileTextNoCursor);

            using var helper = await LanguageServerHelper.StartServerWithText(
                TestContext,
                fileTextsByUri,
                paramUri,
                services => services
                    .WithNamespaceProvider(BuiltInTestTypes.Create()));

            var file = new FileRequestHelper(helper.Client, paramFile);
            var completions = await file.RequestCompletion(cursor);
            return completions.OrderBy(completion => completion.SortText);
        }
    }
}
