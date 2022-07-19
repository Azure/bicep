// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Snippets;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LangServer.IntegrationTests.Completions
{
    [TestClass]
    public class ParamsCompletionTests
    {
        private static readonly MockRepository Repository = new MockRepository(MockBehavior.Strict);
        private static readonly ILanguageServerFacade Server = Repository.Create<ILanguageServerFacade>().Object;
        private static readonly SnippetsProvider snippetsProvider = new(BicepTestConstants.Features, TestTypeHelper.CreateEmptyProvider(), BicepTestConstants.FileResolver, BicepTestConstants.ConfigurationManager);

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
new string[] {"firstParam", "secondParam"},
new CompletionItemKind[] {CompletionItemKind.Field, CompletionItemKind.Field}
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
new string[] {"firstParam", "secondParam", "thirdParam"},
new CompletionItemKind[] {CompletionItemKind.Field, CompletionItemKind.Field, CompletionItemKind.Field}
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
new string[] {},
new CompletionItemKind[] {}
)
]
        public void Params_completion_provider_should_return_correct_completions(string paramText, string bicepText, string[] completionLables, CompletionItemKind[] completionItemKinds)
        {   
            var (paramTextNoCursor, cursor) = ParserHelper.GetFileWithSingleCursor(paramText);
            var paramsUri = new Uri("inmemory:///params.bicepparams");
            var bicepParamFile = SourceFileFactory.CreateBicepParamFile(paramsUri, paramTextNoCursor);


            var fileResolver = new FileResolver();
            var bicepCompilation = new Compilation(BicepTestConstants.Features, TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText(bicepText, fileResolver), BicepTestConstants.BuiltInConfiguration, BicepTestConstants.LinterAnalyzer);

            var paramsSemanticModel = new ParamsSemanticModel(bicepParamFile, (uri) => bicepCompilation);

            var provider = new BicepCompletionProvider(BicepTestConstants.FileResolver, snippetsProvider, new TelemetryProvider(Server), BicepTestConstants.Features);

            var paramsCompletionContext = ParamsCompletionContext.Create(new(paramsSemanticModel, paramsSemanticModel.BicepParamFile.ProgramSyntax, paramsSemanticModel.BicepParamFile.LineStarts), cursor);

            var completions = provider.GetFilteredParamsCompletions(paramsSemanticModel, paramsCompletionContext);

            var expectedValueIndex = 0;
            
            foreach(var completion in completions)
            {
                completion.Label.Should().Be(completionLables[expectedValueIndex]);
                completion.Kind.Should().Be(completionItemKinds[expectedValueIndex]);
                expectedValueIndex +=1;
            }
        }
    }
}
