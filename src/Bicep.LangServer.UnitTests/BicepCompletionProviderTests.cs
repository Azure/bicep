// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Numerics;
using Azure.Deployments.Core.Definitions.Schema;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.UnitTests.Completions;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Settings;
using Bicep.LanguageServer.Snippets;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using RichardSzalay.MockHttp;
using SymbolKind = Bicep.Core.Semantics.SymbolKind;

namespace Bicep.LangServer.UnitTests
{
    // See also Bicep.LangServer.IntegrationTests/CompletionTests.cs4

    [TestClass]
    public class BicepCompletionProviderTests
    {
        private static readonly ILanguageServerFacade server = StrictMock.Of<ILanguageServerFacade>().Object;

        private static BicepCompletionProvider CreateProvider()
        {
            var mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.When("*").Respond("application/json", "{}");

            var publicModuleMetadataProvider = StrictMock.Of<IPublicModuleMetadataProvider>();

            var helper = ServiceBuilder.Create(services => services
                .AddSingleton<ILanguageServerFacade>(server)
                .AddSingleton<IAzureContainerRegistriesProvider, AzureContainerRegistriesProvider>()
                .AddSingleton<ISnippetsProvider, SnippetsProvider>()
                .AddSingleton<ISettingsProvider, SettingsProvider>()
                .AddSingleton<IModuleReferenceCompletionProvider, ModuleReferenceCompletionProvider>()
                .AddHttpClient<IPublicModuleMetadataProvider, PublicModuleMetadataProvider>()
                    .ConfigurePrimaryHttpMessageHandler(() => mockHttpMessageHandler).Services
                .AddSingleton<ITelemetryProvider, TelemetryProvider>()
                .AddSingleton<BicepCompletionProvider>()
                .AddSingleton(publicModuleMetadataProvider.Object)
            );

            return helper.Construct<BicepCompletionProvider>();
        }

        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        [TestMethod]
        public async Task DeclarationContextShouldReturnKeywordCompletions()
        {
            var compilation = Services.BuildCompilation(string.Empty);
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();

            var completionProvider = CreateProvider();
            var completions = await completionProvider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(compilation, 0), CancellationToken.None);

            var keywordCompletions = completions
                .Where(c => c.Kind == CompletionItemKind.Keyword)
                .OrderBy(c => c.Label)
                .ToList();

            keywordCompletions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("extension");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Extension keyword");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("extension");
                },
                c =>
                {
                    c.Label.Should().Be("import");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Import keyword");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("import");
                },
                c =>
                {
                    c.Label.Should().Be("metadata");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Metadata keyword");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("metadata");
                },
                c =>
                {
                    c.Label.Should().Be("module");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Module keyword");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("module");
                },
                c =>
                {
                    c.Label.Should().Be("output");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Output keyword");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("output");
                },
                c =>
                {
                    c.Label.Should().Be("param");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Parameter keyword");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("param");
                },
                c =>
                {
                    c.Label.Should().Be("resource");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Resource keyword");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("resource");
                },
                c =>
                {
                    c.Label.Should().Be("targetScope");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Target Scope keyword");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("targetScope");
                },
                c =>
                {
                    c.Label.Should().Be("type");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Type keyword");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("type");
                },
                c =>
                {
                    c.Label.Should().Be("var");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Variable keyword");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("var");
                });
        }

        [TestMethod]
        public async Task NonDeclarationContextShouldIncludeDeclaredSymbols()
        {
            var compilation = Services.BuildCompilation(@"
param p string
var v =
resource r 'Microsoft.Foo/foos@2020-09-01' = {
  name: 'foo'
}
assert a1 = p == 'foo'
output o int = 42
");
            var offset = compilation.GetEntrypointSemanticModel().Root.VariableDeclarations.Select(x => x.DeclaringVariable).Single().Value.Span.Position;

            var context = BicepCompletionContext.Create(compilation, offset);
            var completionProvider = CreateProvider();
            var completions = (await completionProvider.GetFilteredCompletions(compilation, context, CancellationToken.None)).ToList();

            AssertExpectedFunctions(completions, expectParamDefaultFunctions: false);

            // outputs and assertions can't be referenced so they should not show up in completions
            completions.Where(c => c.Kind == SymbolKind.Output.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Assert.ToCompletionItemKind()).Should().BeEmpty();

            // the variable won't appear in completions because we are not suggesting cycles
            completions.Where(c => c.Kind == SymbolKind.Variable.ToCompletionItemKind()).Should().BeEmpty();

            const string expectedResource = "r";
            var resourceCompletion = completions.Single(c => c.Kind == SymbolKind.Resource.ToCompletionItemKind());
            resourceCompletion.Label.Should().Be(expectedResource);
            resourceCompletion.Kind.Should().Be(CompletionItemKind.Interface);
            resourceCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            resourceCompletion.TextEdit!.TextEdit!.NewText.Should().Be(expectedResource);
            resourceCompletion.CommitCharacters.Should().BeEquivalentTo([":",]);
            resourceCompletion.Detail.Should().Be(expectedResource);

            const string expectedParam = "p";
            var paramCompletion = completions.Single(c => c.Kind == SymbolKind.Parameter.ToCompletionItemKind());
            paramCompletion.Label.Should().Be(expectedParam);
            paramCompletion.Kind.Should().Be(CompletionItemKind.Field);
            paramCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            paramCompletion.TextEdit!.TextEdit!.NewText.Should().Be(expectedParam);
            paramCompletion.CommitCharacters.Should().BeNull();
            paramCompletion.Detail.Should().Be(expectedParam);
        }

        [TestMethod]
        public async Task CompletionsForOneLinerParameterDefaultValueShouldIncludeFunctionsValidInDefaultValues()
        {
            var compilation = Services.BuildCompilation(@"param p string = ");

            var offset = ((ParameterDefaultValueSyntax)compilation.GetEntrypointSemanticModel().Root.ParameterDeclarations.Select(x => x.DeclaringParameter).Single().Modifier!).DefaultValue.Span.Position;

            var completionProvider = CreateProvider();
            var completions = (await completionProvider.GetFilteredCompletions(
                compilation,
                BicepCompletionContext.Create(compilation, offset),
                CancellationToken.None)).ToList();

            AssertExpectedFunctions(completions, expectParamDefaultFunctions: true);

            // outputs can't be referenced so they should not show up in completions
            completions.Where(c => c.Kind == SymbolKind.Output.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Assert.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Variable.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Resource.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Module.ToCompletionItemKind()).Should().BeEmpty();

            // should not see parameter completions because we set the enclosing declaration which will exclude the corresponding symbol
            // this avoids cycle suggestions
            completions.Where(c => c.Kind == SymbolKind.Parameter.ToCompletionItemKind()).Should().BeEmpty();
        }

        [TestMethod]
        public async Task DeclaringSymbolWithFunctionNameShouldHideTheFunctionCompletion()
        {
            var compilation = Services.BuildCompilation(@"
param concat string
var resourceGroup = true
resource base64 'Microsoft.Foo/foos@2020-09-01' = {
  name: 'foo'
}
assert a1 = resourceGroup
output length int =
");
            var offset = compilation.GetEntrypointSemanticModel().Root.OutputDeclarations.Select(x => x.DeclaringOutput).Single().Value.Span.Position;

            var context = BicepCompletionContext.Create(compilation, offset);
            var completionProvider = CreateProvider();
            var completions = (await completionProvider.GetFilteredCompletions(compilation, context, CancellationToken.None)).ToList();

            AssertExpectedFunctions(completions, expectParamDefaultFunctions: false, new[] { "sys.concat", "az.resourceGroup", "sys.base64" });

            // outputs and assertions can't be referenced so they should not show up in completions
            completions.Where(c => c.Kind == SymbolKind.Output.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Assert.ToCompletionItemKind()).Should().BeEmpty();

            const string expectedVariable = "resourceGroup";
            var variableCompletion = completions.Single(c => c.Kind == SymbolKind.Variable.ToCompletionItemKind());
            variableCompletion.Label.Should().Be(expectedVariable);
            variableCompletion.Kind.Should().Be(CompletionItemKind.Variable);
            variableCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            variableCompletion.TextEdit!.TextEdit!.NewText.Should().Be(expectedVariable);
            variableCompletion.CommitCharacters.Should().BeNull();
            variableCompletion.Detail.Should().Be(expectedVariable);

            const string expectedResource = "base64";
            var resourceCompletion = completions.Single(c => c.Kind == SymbolKind.Resource.ToCompletionItemKind());
            resourceCompletion.Label.Should().Be(expectedResource);
            resourceCompletion.Kind.Should().Be(CompletionItemKind.Interface);
            resourceCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            resourceCompletion.TextEdit!.TextEdit!.NewText.Should().Be(expectedResource);
            resourceCompletion.CommitCharacters.Should().BeEquivalentTo([":",]);
            resourceCompletion.Detail.Should().Be(expectedResource);

            const string expectedParam = "concat";
            var paramCompletion = completions.Single(c => c.Kind == SymbolKind.Parameter.ToCompletionItemKind());
            paramCompletion.Label.Should().Be(expectedParam);
            paramCompletion.Kind.Should().Be(CompletionItemKind.Field);
            paramCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            paramCompletion.TextEdit!.TextEdit!.NewText.Should().Be(expectedParam);
            paramCompletion.CommitCharacters.Should().BeNull();
            paramCompletion.Detail.Should().Be(expectedParam);
        }

        [TestMethod]
        public async Task OutputTypeContextShouldReturnDeclarationTypeCompletions()
        {
            var compilation = Services.BuildCompilation("output test ");

            var offset = compilation.GetEntrypointSemanticModel().Root.OutputDeclarations.Select(x => x.DeclaringOutput).Single().Type.Span.Position;

            var completionProvider = CreateProvider();
            var completions = await completionProvider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(compilation, offset), CancellationToken.None);
            var declarationTypeCompletions = completions.Where(c => c.Kind == CompletionItemKind.Class).ToList();

            AssertExpectedDeclarationTypeCompletions(declarationTypeCompletions);

            completions.Where(c => c.Kind == CompletionItemKind.Snippet).Should().BeEmpty();
        }

        [TestMethod]
        public async Task ParameterTypeContextShouldReturnDeclarationTypeCompletions()
        {
            var compilation = Services.BuildCompilation("param foo ");

            var offset = compilation.GetEntrypointSemanticModel().Root.ParameterDeclarations.Select(x => x.DeclaringParameter).Single().Type.Span.Position;

            var completionProvider = CreateProvider();
            var completions = await completionProvider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(compilation, offset), CancellationToken.None);
            var declarationTypeCompletions = completions.Where(c => c.Kind == CompletionItemKind.Class).ToList();

            AssertExpectedDeclarationTypeCompletions(declarationTypeCompletions);

            completions.Where(c => c.Kind == CompletionItemKind.Snippet).Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("secureObject");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.TextEdit!.TextEdit!.NewText.Should().StartWith("object");
                    c.TextEdit.TextEdit.NewText.Should().Be("object");
                    c.Detail.Should().Be("Secure object");
                    c.AdditionalTextEdits!.Count().Should().Be(1);
                    c.AdditionalTextEdits!.ElementAt(0).NewText.Should().Be("@secure()\n");
                    c.AdditionalTextEdits!.ElementAt(0).Range.Start.Line.Should().Be(0);
                    c.AdditionalTextEdits!.ElementAt(0).Range.Start.Character.Should().Be(0);
                    c.AdditionalTextEdits!.ElementAt(0).Range.End.Line.Should().Be(0);
                    c.AdditionalTextEdits!.ElementAt(0).Range.End.Character.Should().Be(0);
                },
                c =>
                {
                    c.Label.Should().Be("securestring");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.TextEdit!.TextEdit!.NewText.Should().StartWith("string");
                    c.TextEdit.TextEdit.NewText.Should().Be("string");
                    c.Detail.Should().Be("Secure string");
                    c.AdditionalTextEdits!.Count().Should().Be(1);
                    c.AdditionalTextEdits!.ElementAt(0).NewText.Should().Be("@secure()\n");
                    c.AdditionalTextEdits!.ElementAt(0).Range.Start.Line.Should().Be(0);
                    c.AdditionalTextEdits!.ElementAt(0).Range.Start.Character.Should().Be(0);
                    c.AdditionalTextEdits!.ElementAt(0).Range.End.Line.Should().Be(0);
                    c.AdditionalTextEdits!.ElementAt(0).Range.End.Character.Should().Be(0);
                });
        }

        [TestMethod]
        public async Task VerifyParameterTypeCompletionWithPrecedingComment()
        {
            var compilation = Services.BuildCompilation("/*test*/param foo ");

            var offset = compilation.GetEntrypointSemanticModel().Root.ParameterDeclarations.Select(x => x.DeclaringParameter).Single().Type.Span.Position;

            var completionProvider = CreateProvider();
            var completions = await completionProvider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(compilation, offset), CancellationToken.None);
            var declarationTypeCompletions = completions.Where(c => c.Kind == CompletionItemKind.Class).ToList();

            AssertExpectedDeclarationTypeCompletions(declarationTypeCompletions);

            completions.Where(c => c.Kind == CompletionItemKind.Snippet).Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("secureObject");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.TextEdit!.TextEdit!.NewText.Should().Be("object");
                    c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.Start.Character.Should().Be(18);
                    c.Detail.Should().Be("Secure object");
                    c.AdditionalTextEdits!.Count().Should().Be(1);
                    c.AdditionalTextEdits!.ElementAt(0).NewText.Should().Be("@secure()\n");
                    c.AdditionalTextEdits!.ElementAt(0).Range.Start.Line.Should().Be(0);
                    c.AdditionalTextEdits!.ElementAt(0).Range.Start.Character.Should().Be(8);
                    c.AdditionalTextEdits!.ElementAt(0).Range.End.Line.Should().Be(0);
                    c.AdditionalTextEdits!.ElementAt(0).Range.End.Character.Should().Be(8);
                },
                c =>
                {
                    c.Label.Should().Be("securestring");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.TextEdit!.TextEdit!.NewText.Should().Be("string");
                    c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.Start.Character.Should().Be(18);
                    c.Detail.Should().Be("Secure string");
                    c.AdditionalTextEdits!.Count().Should().Be(1);
                    c.AdditionalTextEdits!.ElementAt(0).NewText.Should().Be("@secure()\n");
                    c.AdditionalTextEdits!.ElementAt(0).Range.Start.Line.Should().Be(0);
                    c.AdditionalTextEdits!.ElementAt(0).Range.Start.Character.Should().Be(8);
                    c.AdditionalTextEdits!.ElementAt(0).Range.End.Line.Should().Be(0);
                    c.AdditionalTextEdits!.ElementAt(0).Range.End.Character.Should().Be(8);
                });
        }

        [DataTestMethod]
        [DataRow("// |")]
        [DataRow("/* |")]
        [DataRow("param foo // |")]
        [DataRow("param foo /* |")]
        [DataRow("param /*| */ foo")]
        [DataRow(@"/*
*
* |
*/")]
        public async Task CommentShouldNotGiveAnyCompletions(string codeFragment)
        {
            var compilation = Services.BuildCompilation(codeFragment);

            var offset = codeFragment.IndexOf('|');

            var completionProvider = CreateProvider();
            var completions = await completionProvider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(compilation, offset), CancellationToken.None);

            completions.Should().BeEmpty();
        }

        [TestMethod]
        public async Task CompletionsShouldContainMicrosoftGraphWhenExtensibilityEnabled()
        {
            var (contents, cursor) = ParserHelper.GetFileWithSingleCursor("extension m| as graph");

            var completionProvider = CreateProvider();
            var serviceWithGraph = new ServiceBuilder();

            var compilationWithMSGraph = serviceWithGraph.BuildCompilation(contents);
            var completionsWithMSGraph = await completionProvider.GetFilteredCompletions(compilationWithMSGraph, BicepCompletionContext.Create(compilationWithMSGraph, cursor), CancellationToken.None);

            completionsWithMSGraph.Should().NotContain(c => c.Label.Contains("microsoftGraph"));
        }

        private static void AssertExpectedDeclarationTypeCompletions(List<CompletionItem> completions)
        {
            completions.Should().SatisfyRespectively(
                c =>
                {
                    const string expected = "any";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.TextEdit!.TextEdit!.NewText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "array";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.TextEdit!.TextEdit!.NewText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "bool";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.TextEdit!.TextEdit!.NewText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "int";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.TextEdit!.TextEdit!.NewText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "object";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.TextEdit!.TextEdit!.NewText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "resourceInput";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.TextEdit!.TextEdit!.NewText.Should().Be("resourceInput<'$0'>");
                    c.Detail.Should().Be("Use the type definition of the input for a specific resource rather than a user-defined type.\n\nNB: The type definition will be checked by Bicep when the template is compiled but will not be enforced by the ARM engine during a deployment.");
                },
            c =>
            {
                const string expected = "resourceOutput";
                c.Label.Should().Be(expected);
                c.Kind.Should().Be(CompletionItemKind.Class);
                c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                c.TextEdit!.TextEdit!.NewText.Should().Be("resourceOutput<'$0'>");
                c.Detail.Should().Be("Use the type definition of the return value of a specific resource rather than a user-defined type.\n\nNB: The type definition will be checked by Bicep when the template is compiled but will not be enforced by the ARM engine during a deployment.");
            },
                c =>
                {
                    const string expected = "string";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.TextEdit!.TextEdit!.NewText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                });
        }

        private static void AssertExpectedFunctions(List<CompletionItem> completions, bool expectParamDefaultFunctions, IEnumerable<string>? fullyQualifiedFunctionNames = null)
        {
            fullyQualifiedFunctionNames ??= [];

            var fullyQualifiedFunctionParts = fullyQualifiedFunctionNames.Select(fqfn =>
            {
                var parts = fqfn.Split('.', StringSplitOptions.None);
                return (@namespace: parts[0], function: parts[1]);
            });

            var functionCompletions = completions.Where(c => c.Kind == CompletionItemKind.Function).OrderBy(c => c.Label).ToList();

            var namespaces = new[] {
                TestTypeHelper.GetBuiltInNamespaceType("az"),
                TestTypeHelper.GetBuiltInNamespaceType("sys")
            };

            var availableFunctionNames = namespaces
                .SelectMany(ns => ns.MethodResolver.GetKnownFunctions().Values)
                .Where(symbol => expectParamDefaultFunctions || !symbol.FunctionFlags.HasFlag(FunctionFlags.ParamDefaultsOnly))
                .Select(func => func.Name)
                .Except(fullyQualifiedFunctionParts.Select(p => p.function), LanguageConstants.IdentifierComparer)
                .Concat(fullyQualifiedFunctionNames)
                .OrderBy(s => s);

            functionCompletions.Select(c => c.Label).Should().BeEquivalentTo(availableFunctionNames);
            functionCompletions.Should().OnlyContain(c => c.TextEdit!.TextEdit!.NewText.StartsWith(c.Label + '(', StringComparison.Ordinal));
            functionCompletions.Should().OnlyContain(c => c.Documentation != null && !string.IsNullOrWhiteSpace(c.Documentation.ToString()));
            functionCompletions.Should().OnlyContain(c => c.InsertTextFormat == InsertTextFormat.Snippet);
        }
    }
}
