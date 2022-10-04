// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

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
        public async Task Request_for_parameter_identifier_completions_should_return_correct_identifiers(string paramText, string bicepText, string[] completionLables, CompletionItemKind[] completionItemKinds)
        {
            var (paramFileTextNoCursor, cursor) = ParserHelper.GetFileWithSingleCursor(paramText);

            var paramUri = InMemoryFileResolver.GetFileUri("/path/to/param.bicepparam");
            var paramFile = SourceFileFactory.CreateBicepFile(paramUri, paramFileTextNoCursor);

            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [paramUri] = paramFileTextNoCursor,
                [InMemoryFileResolver.GetFileUri("/path/to/main.bicep")] = bicepText
            };

            var fileResolver = new InMemoryFileResolver(fileTextsByUri);
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(
                TestContext,
                paramFileTextNoCursor,
                paramUri,
                creationOptions: new LanguageServer.Server.CreationOptions(
                    NamespaceProvider: BuiltInTestTypes.Create(),
                    FileResolver: fileResolver,
                    FeatureProviderFactory: IFeatureProviderFactory.WithStaticFeatureProvider(BicepTestConstants.CreateFeatureProvider(TestContext, paramsFilesEnabled: true))));

            var file = new FileRequestHelper(helper.Client, paramFile);

            var completions = await file.RequestCompletion(cursor);

            var expectedValueIndex = 0;
            foreach (var completion in completions)
            {
                completion.Label.Should().Be(completionLables[expectedValueIndex]);
                completion.Kind.Should().Be(completionItemKinds[expectedValueIndex]);
                expectedValueIndex += 1;
            }
        }

        [DataTestMethod]
        [DataRow(
@"
//Parameters file

using './main.bicep'

param firstParam = |",

@"
//Bicep file

@allowed([
  'one'
  'two'
])
param firstParam string

",
new string[] { "'one'", "'two'" },
new CompletionItemKind[] { CompletionItemKind.EnumMember, CompletionItemKind.EnumMember }
)
]

        [DataRow(
@"
//Parameters file

using './main.bicep'

param firstParam = |",

@"
//Bicep file

param firstParam string

",
new string[] { },
new CompletionItemKind[] { }
)
]
        [DataRow(
@"
//Parameters file

using './main.bicep'

param firstParam = 'o|'",

@"
//Bicep file

@allowed([
  'one'
  'two'
])
param firstParam string

",
new string[] { "'one'", "'two'" },
new CompletionItemKind[] { CompletionItemKind.EnumMember, CompletionItemKind.EnumMember }
)
]
        public async Task Request_for_parameter_allowed_value_completions_should_return_correct_value(string paramText, string bicepText, string[] completionLables, CompletionItemKind[] completionItemKinds)
        {
            var (paramFileTextNoCursor, cursor) = ParserHelper.GetFileWithSingleCursor(paramText);

            var paramUri = InMemoryFileResolver.GetFileUri("/path/to/param.bicepparam");
            var paramFile = SourceFileFactory.CreateBicepFile(paramUri, paramFileTextNoCursor);

            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [paramUri] = paramFileTextNoCursor,
                [InMemoryFileResolver.GetFileUri("/path/to/main.bicep")] = bicepText
            };

            var fileResolver = new InMemoryFileResolver(fileTextsByUri);
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(
                TestContext,
                paramFileTextNoCursor,
                paramUri,
                creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), FileResolver: fileResolver,
                FeatureProviderFactory: IFeatureProviderFactory.WithStaticFeatureProvider(BicepTestConstants.CreateFeatureProvider(TestContext, paramsFilesEnabled: true))));

            var file = new FileRequestHelper(helper.Client, paramFile);

            var completions = await file.RequestCompletion(cursor);

            var expectedValueIndex = 0;
            foreach (var completion in completions)
            {
                completion.Label.Should().Be(completionLables[expectedValueIndex]);
                completion.Kind.Should().Be(completionItemKinds[expectedValueIndex]);
                expectedValueIndex += 1;
            }
        }

        [TestMethod]
        public async Task Request_for_using_declaration_path_completions_should_return_correct_paths_for_file_directories()
        {
            var paramUri = InMemoryFileResolver.GetFileUri("/path/to/param.bicepparam");
            var (paramFileTextNoCursor, cursor) = ParserHelper.GetFileWithSingleCursor(@"
using |
");
            var paramFile = SourceFileFactory.CreateBicepFile(paramUri, paramFileTextNoCursor);

            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [paramUri] = paramFileTextNoCursor,
                [InMemoryFileResolver.GetFileUri("/path/to/main1.bicep")] = "param foo int",
                [InMemoryFileResolver.GetFileUri("/path/to/main2.txt")] = "param bar int",
                [InMemoryFileResolver.GetFileUri("/path/to/nested1/main3.bicep")] = "param foo int",
                [InMemoryFileResolver.GetFileUri("/path/to/module1.bicep")] = "param foo string",
                [InMemoryFileResolver.GetFileUri("/path/to/nested1/module2.bicep")] = "param bar bool",
                [InMemoryFileResolver.GetFileUri("/path/to/nested2/module3.bicep")] = "param bar string"
            };

            var fileResolver = new InMemoryFileResolver(fileTextsByUri);
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(
                TestContext,
                paramFileTextNoCursor,
                paramUri,
                creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), FileResolver: fileResolver,
                    FeatureProviderFactory: IFeatureProviderFactory.WithStaticFeatureProvider(BicepTestConstants.CreateFeatureProvider(TestContext, paramsFilesEnabled: true))));

            var file = new FileRequestHelper(helper.Client, paramFile);

            var completions = await file.RequestCompletion(cursor);
            completions.Should().SatisfyRespectively(
                x => x.Label.Should().Be("main1.bicep"),
                x => x.Label.Should().Be("module1.bicep"),
                x => x.Label.Should().Be("nested1/"),
                x => x.Label.Should().Be("nested2/"),
                x => x.Label.Should().Be("../"));
            completions.Should().SatisfyRespectively(
                x => x.Kind.Should().Be(CompletionItemKind.File),
                x => x.Kind.Should().Be(CompletionItemKind.File),
                x => x.Kind.Should().Be(CompletionItemKind.Folder),
                x => x.Kind.Should().Be(CompletionItemKind.Folder),
                x => x.Kind.Should().Be(CompletionItemKind.Folder));
        }

        [TestMethod]
        public async Task Request_for_using_declaration_path_completions_should_return_correct_partial_paths()
        {
            var paramUri = InMemoryFileResolver.GetFileUri("/path/to/param.bicepparam");

            var (paramFileTextNoCursor, cursor) = ParserHelper.GetFileWithSingleCursor(@"
using './nested1/|'
");
            var paramFile = SourceFileFactory.CreateBicepFile(paramUri, paramFileTextNoCursor);

            var fileTextsByUri = new Dictionary<Uri, string>
            {
                [paramUri] = paramFileTextNoCursor,
                [InMemoryFileResolver.GetFileUri("/path/to/main1.bicep")] = "param foo int",
                [InMemoryFileResolver.GetFileUri("/path/to/main2.txt")] = "param bar int",
                [InMemoryFileResolver.GetFileUri("/path/to/nested1/main3.bicep")] = "param foo int",
                [InMemoryFileResolver.GetFileUri("/path/to/module1.bicep")] = "param foo string",
                [InMemoryFileResolver.GetFileUri("/path/to/nested1/module2.bicep")] = "param bar bool",
                [InMemoryFileResolver.GetFileUri("/path/to/nested2/module3.bicep")] = "param bar string"
            };

            var fileResolver = new InMemoryFileResolver(fileTextsByUri);
            using var helper = await LanguageServerHelper.StartServerWithTextAsync(
                TestContext,
                paramFileTextNoCursor,
                paramUri,
                creationOptions: new LanguageServer.Server.CreationOptions(NamespaceProvider: BuiltInTestTypes.Create(), FileResolver: fileResolver,
                    FeatureProviderFactory: IFeatureProviderFactory.WithStaticFeatureProvider(BicepTestConstants.CreateFeatureProvider(TestContext, paramsFilesEnabled: true))));

            var file = new FileRequestHelper(helper.Client, paramFile);

            var completions = await file.RequestCompletion(cursor);
            completions.Should().SatisfyRespectively(
                x => x.Label.Should().Be("main3.bicep"),
                x => x.Label.Should().Be("module2.bicep"),
                x => x.Label.Should().Be("../"));
            completions.Should().SatisfyRespectively(
                x => x.Kind.Should().Be(CompletionItemKind.File),
                x => x.Kind.Should().Be(CompletionItemKind.File),
                x => x.Kind.Should().Be(CompletionItemKind.Folder));
        }
    }
}
