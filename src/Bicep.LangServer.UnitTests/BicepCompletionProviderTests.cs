// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Navigation;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.SemanticModel.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.UnitTests.Completions;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Snippets;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SymbolKind = Bicep.Core.SemanticModel.SymbolKind;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepCompletionProviderTests
    {
        [TestMethod]
        public void DeclarationSnippetsShouldBeValid()
        {
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(string.Empty));
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();

            var provider = new BicepCompletionProvider();

            var completions = provider.GetFilteredCompletions(compilation.GetEntrypointSemanticModel(), new BicepCompletionContext(BicepCompletionContextKind.DeclarationStart));

            var snippetCompletions = completions
                .Where(c => c.Kind == CompletionItemKind.Snippet)
                .OrderBy(c => c.Label)
                .ToList();

            snippetCompletions.Should().OnlyContain(c => c.Kind == CompletionItemKind.Snippet && c.InsertTextFormat == InsertTextFormat.Snippet);
            snippetCompletions.Should().OnlyContain(c => c.InsertTextFormat == InsertTextFormat.Snippet);
            snippetCompletions.Should().OnlyContain(c => LanguageConstants.DeclarationKeywords.Contains(c.Label));
            snippetCompletions.Should().OnlyContain(c => c.Documentation.HasMarkupContent && c.Documentation.MarkupContent.Kind == MarkupKind.Markdown);

            var snippetsByDetail = snippetCompletions.ToDictionary(c => c.Detail);

            var replacementsByDetail = new Dictionary<string, IList<string>>
            {
                ["Parameter declaration"] = new[] {string.Empty, "myParam", "string"},
                ["Parameter declaration with default value"] = new[] {string.Empty, "myParam", "string", "'myDefault'"},
                ["Parameter declaration with default and allowed values"] = new[] {string.Empty, "myParam", "string", "'myDefault'", "'val1'\n'val2'"},
                ["Parameter declaration with options"] = new[] {string.Empty, "myParam", "string", "default: 'myDefault'\nsecure: true"},
                ["Secure string parameter"] = new[] {string.Empty, "myParam"},
                ["Variable declaration"] = new[] {"'stringVal'", "myVariable"},
                ["Resource with defaults"] = new[] {"prop1: 'val1'", "myResource", "myProvider", "myType", "2020-01-01", "'parent'", "'West US'"},
                ["Child Resource with defaults"] = new[] {"prop1: 'val1'", "myResource", "myProvider", "myType", "myChildType", "2020-01-01", "'parent/child'"},
                ["Resource without defaults"] = new[] {"properties: {\nprop1: 'val1'\n}", "myResource", "myProvider", "myType", "2020-01-01", "'parent'"},
                ["Child Resource without defaults"] = new[] {"properties: {\nprop1: 'val1'\n}", "myResource", "myProvider", "myType", "myChildType", "2020-01-01", "'parent/child'"},
                ["Output declaration"] = new[] { "'stringVal'", "myOutput", "string" }
            };

            snippetsByDetail.Keys.Should().BeEquivalentTo(replacementsByDetail.Keys);


            foreach (var (detail, completion) in snippetsByDetail)
            {
                // validate snippet
                var snippet = new Snippet(completion.InsertText);
                
                // if we don't have placeholders, why is it a snippet?
                snippet.Placeholders.Should().NotBeEmpty();

                // documentation should have the snippet without placeholders
                completion.Documentation.MarkupContent.Value.Should().Contain(snippet.FormatDocumentation());

                // perform the sample replacement
                var replacements = replacementsByDetail[detail];
                var replaced = snippet.Format((s, placeholder) => placeholder.Index >= 0 && placeholder.Index < replacements.Count
                    ? replacements[placeholder.Index]
                    : string.Empty);

                var parser = new Parser(replaced);
                var declaration = parser.Declaration();

                declaration.Should().BeAssignableTo<IDeclarationSyntax>($"because the snippet for '{detail}' failed to parse after replacements:\n{replaced}");
            }
        }

        [TestMethod]
        public void DeclarationContextShouldReturnKeywordCompletions()
        {
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(string.Empty));
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();

            var provider = new BicepCompletionProvider();

            var completions = provider.GetFilteredCompletions(compilation.GetEntrypointSemanticModel(), new BicepCompletionContext(BicepCompletionContextKind.DeclarationStart));

            var keywordCompletions = completions
                .Where(c => c.Kind == CompletionItemKind.Keyword)
                .OrderBy(c => c.Label)
                .ToList();

            keywordCompletions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("output");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().Be("output");
                    c.CommitCharacters.Should().OnlyContain(s => string.Equals(s, " "));
                    c.Detail.Should().Be("Output keyword");
                },
                c =>
                {
                    c.Label.Should().Be("param");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().Be("param");
                    c.CommitCharacters.Should().OnlyContain(s => string.Equals(s, " "));
                    c.Detail.Should().Be("Parameter keyword");
                },
                c =>
                {
                    c.Label.Should().Be("resource");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().Be("resource");
                    c.CommitCharacters.Should().OnlyContain(s => string.Equals(s, " "));
                    c.Detail.Should().Be("Resource keyword");
                },
                c =>
                {
                    c.Label.Should().Be("var");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().Be("var");
                    c.CommitCharacters.Should().OnlyContain(s => string.Equals(s, " "));
                    c.Detail.Should().Be("Variable keyword");
                });
        }

        [TestMethod]
        public void NonDeclarationContextInEmptyFileShouldReturnFunctionCompletions()
        {
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(string.Empty));
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();

            var provider = new BicepCompletionProvider();

            var completions = provider.GetFilteredCompletions(compilation.GetEntrypointSemanticModel(), new BicepCompletionContext(BicepCompletionContextKind.None)).ToList();

            completions.Where(c => c.Kind == SymbolKind.Variable.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Output.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Resource.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Parameter.ToCompletionItemKind()).Should().BeEmpty();

            AssertExpectedFunctions(completions);
        }

        [TestMethod]
        public void NonDeclarationContextShouldIncludeDeclaredSymbols()
        {
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(@"
param p string
var v = true
resource r 'Microsoft.Foo/foos@2020-09-01' = {
  name: 'foo'
}
output o int = 42
"));
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();

            var provider = new BicepCompletionProvider();
            var completions = provider.GetFilteredCompletions(compilation.GetEntrypointSemanticModel(), new BicepCompletionContext(BicepCompletionContextKind.None)).ToList();
            
            AssertExpectedFunctions(completions);

            // outputs can't be referenced so they should not show up in completions
            completions.Where(c => c.Kind == SymbolKind.Output.ToCompletionItemKind()).Should().BeEmpty();

            const string expectedVariable = "v";
            var variableCompletion = completions.Single(c => c.Kind == SymbolKind.Variable.ToCompletionItemKind());
            variableCompletion.Label.Should().Be(expectedVariable);
            variableCompletion.Kind.Should().Be(CompletionItemKind.Variable);
            variableCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            variableCompletion.InsertText.Should().Be(expectedVariable);
            variableCompletion.CommitCharacters.Should().BeNull();
            variableCompletion.Detail.Should().Be(expectedVariable);

            const string expectedResource = "r";
            var resourceCompletion = completions.Single(c => c.Kind == SymbolKind.Resource.ToCompletionItemKind());
            resourceCompletion.Label.Should().Be(expectedResource);
            resourceCompletion.Kind.Should().Be(CompletionItemKind.Interface);
            resourceCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            resourceCompletion.InsertText.Should().Be(expectedResource);
            resourceCompletion.CommitCharacters.Should().BeNull();
            resourceCompletion.Detail.Should().Be(expectedResource);

            const string expectedParam = "p";
            var paramCompletion = completions.Single(c => c.Kind == SymbolKind.Parameter.ToCompletionItemKind());
            paramCompletion.Label.Should().Be(expectedParam);
            paramCompletion.Kind.Should().Be(CompletionItemKind.Field);
            paramCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            paramCompletion.InsertText.Should().Be(expectedParam);
            paramCompletion.CommitCharacters.Should().BeNull();
            paramCompletion.Detail.Should().Be(expectedParam);
        }

        [TestMethod]
        public void DeclaringSymbolWithFunctionNameShouldHideTheFunctionCompletion()
        {
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(@"
param concat string
var resourceGroup = true
resource base64 'Microsoft.Foo/foos@2020-09-01' = {
  name: 'foo'
}
output length int = 42
"));
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();

            var provider = new BicepCompletionProvider();
            var completions = provider.GetFilteredCompletions(compilation.GetEntrypointSemanticModel(), new BicepCompletionContext(BicepCompletionContextKind.None)).ToList();

            AssertExpectedFunctions(completions, new[] {"concat", "resourceGroup", "base64"});

            // outputs can't be referenced so they should not show up in completions
            completions.Where(c => c.Kind == SymbolKind.Output.ToCompletionItemKind()).Should().BeEmpty();

            const string expectedVariable = "resourceGroup";
            var variableCompletion = completions.Single(c => c.Kind == SymbolKind.Variable.ToCompletionItemKind());
            variableCompletion.Label.Should().Be(expectedVariable);
            variableCompletion.Kind.Should().Be(CompletionItemKind.Variable);
            variableCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            variableCompletion.InsertText.Should().Be(expectedVariable);
            variableCompletion.CommitCharacters.Should().BeNull();
            variableCompletion.Detail.Should().Be(expectedVariable);

            const string expectedResource = "base64";
            var resourceCompletion = completions.Single(c => c.Kind == SymbolKind.Resource.ToCompletionItemKind());
            resourceCompletion.Label.Should().Be(expectedResource);
            resourceCompletion.Kind.Should().Be(CompletionItemKind.Interface);
            resourceCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            resourceCompletion.InsertText.Should().Be(expectedResource);
            resourceCompletion.CommitCharacters.Should().BeNull();
            resourceCompletion.Detail.Should().Be(expectedResource);

            const string expectedParam = "concat";
            var paramCompletion = completions.Single(c => c.Kind == SymbolKind.Parameter.ToCompletionItemKind());
            paramCompletion.Label.Should().Be(expectedParam);
            paramCompletion.Kind.Should().Be(CompletionItemKind.Field);
            paramCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            paramCompletion.InsertText.Should().Be(expectedParam);
            paramCompletion.CommitCharacters.Should().BeNull();
            paramCompletion.Detail.Should().Be(expectedParam);
        }

        [TestMethod]
        public void OutputTypeContextShouldReturnDeclarationTypeCompletions()
        {
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(string.Empty));
            var provider = new BicepCompletionProvider();

            var completions = provider.GetFilteredCompletions(compilation.GetEntrypointSemanticModel(), new BicepCompletionContext(BicepCompletionContextKind.OutputType));
            var declarationTypeCompletions = completions.Where(c => c.Kind == CompletionItemKind.Class).ToList();

            AssertExpectedDeclarationTypeCompletions(declarationTypeCompletions);

            completions.Where(c => c.Kind == CompletionItemKind.Snippet).Should().BeEmpty();
        }

        [TestMethod]
        public void ParameterTypeContextShouldReturnDeclarationTypeCompletions()
        {
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText(string.Empty));
            var provider = new BicepCompletionProvider();

            var completions = provider.GetFilteredCompletions(compilation.GetEntrypointSemanticModel(), new BicepCompletionContext(BicepCompletionContextKind.ParameterType));
            var declarationTypeCompletions = completions.Where(c => c.Kind == CompletionItemKind.Class).ToList();

            AssertExpectedDeclarationTypeCompletions(declarationTypeCompletions);

            completions.Where(c => c.Kind == CompletionItemKind.Snippet).Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("secureObject");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().StartWith("object");
                    c.InsertText.Should().Contain("secure: true");
                    c.Detail.Should().Be("Secure object");
                },
                c =>
                {
                    c.Label.Should().Be("secureString");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().StartWith("string");
                    c.InsertText.Should().Contain("secure: true");
                    c.Detail.Should().Be("Secure string");
                });
        }

        private static void AssertExpectedDeclarationTypeCompletions(List<CompletionItem> completions)
        {
            completions.Should().SatisfyRespectively(
                c =>
                {
                    const string expected = "array";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "bool";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "int";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "object";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "string";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                });
        }

        private static void AssertExpectedFunctions(List<CompletionItem> completions, IEnumerable<string>? excludedFunctionNames = null)
        {
            excludedFunctionNames ??= Enumerable.Empty<string>();
            var functionCompletions = completions.Where(c => c.Kind == CompletionItemKind.Function).OrderBy(c => c.Label).ToList();

            var availableFunctionNames = new NamespaceSymbol[] {new AzNamespaceSymbol(), new SystemNamespaceSymbol()}
                .SelectMany(ns => ns.Descendants.OfType<FunctionSymbol>())
                .Select(func => func.Name)
                .Except(excludedFunctionNames)
                .OrderBy(s => s);

            functionCompletions.Select(c => c.Label).Should().BeEquivalentTo(availableFunctionNames);
            functionCompletions.Should().OnlyContain(c => string.Equals(c.Label, c.InsertText));
            functionCompletions.Should().OnlyContain(c => string.Equals(c.Label, c.Detail));
            functionCompletions.Should().OnlyContain(c => c.InsertTextFormat == InsertTextFormat.PlainText);
        }
    }
}
