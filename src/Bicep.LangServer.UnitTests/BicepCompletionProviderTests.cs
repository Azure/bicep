// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.UnitTests.Completions;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Snippets;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SymbolKind = Bicep.Core.Semantics.SymbolKind;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepCompletionProviderTests
    {
        [TestMethod]
        public void DeclarationSnippetsShouldBeValid()
        {
            var grouping = SyntaxTreeGroupingFactory.CreateFromText(string.Empty);
            var compilation = new Compilation(TestResourceTypeProvider.Create(), grouping);
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();

            var provider = new BicepCompletionProvider(new FileResolver());

            var completions = provider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(grouping.EntryPoint, 0));

            var snippetCompletions = completions
                .Where(c => c.Kind == CompletionItemKind.Snippet)
                .OrderBy(c => c.Label)
                .ToList();

            snippetCompletions.Should().OnlyContain(c => c.Kind == CompletionItemKind.Snippet && c.InsertTextFormat == InsertTextFormat.Snippet);
            snippetCompletions.Should().OnlyContain(c => c.InsertTextFormat == InsertTextFormat.Snippet);
            snippetCompletions.Should().OnlyContain(c => LanguageConstants.DeclarationKeywords.Contains(c.Label));
            snippetCompletions.Should().OnlyContain(c => c.Documentation!.HasMarkupContent && c.Documentation.MarkupContent!.Kind == MarkupKind.Markdown);

            var snippetsByDetail = snippetCompletions.Where(c => c.Detail != null).ToImmutableDictionaryExcludingNull(c => c.Detail, StringComparer.Ordinal);

            var replacementsByDetail = new Dictionary<string, IList<string>>
            {
                ["Module declaration"] = new[] {string.Empty, "myModule", "./empty.bicep"},
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
                ["Output declaration"] = new[] {"'stringVal'", "myOutput", "string"}
            };

            snippetsByDetail.Keys.Should().BeEquivalentTo(replacementsByDetail.Keys);


            foreach (var (detail, completion) in snippetsByDetail)
            {
                // validate snippet
                var snippet = new Snippet(completion.TextEdit!.NewText);
                
                // if we don't have placeholders, why is it a snippet?
                snippet.Placeholders.Should().NotBeEmpty();

                // documentation should have the snippet without placeholders
                completion.Documentation!.MarkupContent!.Value.Should().Contain(snippet.FormatDocumentation());

                // perform the sample replacement
                var replacements = replacementsByDetail[detail!];
                var replaced = snippet.Format((s, placeholder) => placeholder.Index >= 0 && placeholder.Index < replacements.Count
                    ? replacements[placeholder.Index]
                    : string.Empty);

                var parser = new Parser(replaced);
                var declaration = parser.Declaration();

                declaration.Should().BeAssignableTo<ITopLevelNamedDeclarationSyntax>($"because the snippet for '{detail}' failed to parse after replacements:\n{replaced}");
            }
        }

        [TestMethod]
        public void DeclarationContextShouldReturnKeywordCompletions()
        {
            var grouping = SyntaxTreeGroupingFactory.CreateFromText(string.Empty);
            var compilation = new Compilation(TestResourceTypeProvider.Create(), grouping);
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();

            var provider = new BicepCompletionProvider(new FileResolver());

            var completions = provider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(grouping.EntryPoint, 0));

            var keywordCompletions = completions
                .Where(c => c.Kind == CompletionItemKind.Keyword)
                .OrderBy(c => c.Label)
                .ToList();

            keywordCompletions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("module");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Module keyword");
                    c.TextEdit!.NewText.Should().Be("module");
                },
                c =>
                {
                    c.Label.Should().Be("output");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Output keyword");
                    c.TextEdit!.NewText.Should().Be("output");
                },
                c =>
                {
                    c.Label.Should().Be("param");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Parameter keyword");
                    c.TextEdit!.NewText.Should().Be("param");
                },
                c =>
                {
                    c.Label.Should().Be("resource");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Resource keyword");
                    c.TextEdit!.NewText.Should().Be("resource");
                },
                c =>
                {
                    c.Label.Should().Be("targetScope");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Target Scope keyword");
                    c.TextEdit!.NewText.Should().Be("targetScope");
                },
                c =>
                {
                    c.Label.Should().Be("var");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Variable keyword");
                    c.TextEdit!.NewText.Should().Be("var");
                });
        }

        [TestMethod]
        public void NonDeclarationContextShouldIncludeDeclaredSymbols()
        {
            var grouping = SyntaxTreeGroupingFactory.CreateFromText(@"
param p string
var v = 
resource r 'Microsoft.Foo/foos@2020-09-01' = {
  name: 'foo'
}
output o int = 42
");
            var offset = grouping.EntryPoint.ProgramSyntax.Declarations.OfType<VariableDeclarationSyntax>().Single().Value.Span.Position;
            var compilation = new Compilation(TestResourceTypeProvider.Create(), grouping);
            
            var provider = new BicepCompletionProvider(new FileResolver());
            var context = BicepCompletionContext.Create(grouping.EntryPoint, offset);
            var completions = provider.GetFilteredCompletions(compilation, context).ToList();
            
            AssertExpectedFunctions(completions, expectParamDefaultFunctions: false);

            // outputs can't be referenced so they should not show up in completions
            completions.Where(c => c.Kind == SymbolKind.Output.ToCompletionItemKind()).Should().BeEmpty();

            // the variable won't appear in completions because we are not suggesting cycles
            completions.Where(c => c.Kind == SymbolKind.Variable.ToCompletionItemKind()).Should().BeEmpty();

            const string expectedResource = "r";
            var resourceCompletion = completions.Single(c => c.Kind == SymbolKind.Resource.ToCompletionItemKind());
            resourceCompletion.Label.Should().Be(expectedResource);
            resourceCompletion.Kind.Should().Be(CompletionItemKind.Interface);
            resourceCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            resourceCompletion.TextEdit!.NewText.Should().Be(expectedResource);
            resourceCompletion.CommitCharacters.Should().BeNull();
            resourceCompletion.Detail.Should().Be(expectedResource);

            const string expectedParam = "p";
            var paramCompletion = completions.Single(c => c.Kind == SymbolKind.Parameter.ToCompletionItemKind());
            paramCompletion.Label.Should().Be(expectedParam);
            paramCompletion.Kind.Should().Be(CompletionItemKind.Field);
            paramCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            paramCompletion.TextEdit!.NewText.Should().Be(expectedParam);
            paramCompletion.CommitCharacters.Should().BeNull();
            paramCompletion.Detail.Should().Be(expectedParam);
        }

        [TestMethod]
        public void CompletionsForOneLinerParameterDefaultValueShouldIncludeFunctionsValidInDefaultValues()
        {
            var grouping = SyntaxTreeGroupingFactory.CreateFromText(@"param p string = ");
            var compilation = new Compilation(TestResourceTypeProvider.Create(), grouping);

            var offset = ((ParameterDefaultValueSyntax) grouping.EntryPoint.ProgramSyntax.Declarations.OfType<ParameterDeclarationSyntax>().Single().Modifier!).DefaultValue.Span.Position;

            var provider = new BicepCompletionProvider(new FileResolver());
            var completions = provider.GetFilteredCompletions(
                compilation,
                BicepCompletionContext.Create(grouping.EntryPoint, offset)).ToList();

            AssertExpectedFunctions(completions, expectParamDefaultFunctions: true);

            // outputs can't be referenced so they should not show up in completions
            completions.Where(c => c.Kind == SymbolKind.Output.ToCompletionItemKind()).Should().BeEmpty();

            completions.Where(c => c.Kind == SymbolKind.Variable.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Resource.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Module.ToCompletionItemKind()).Should().BeEmpty();

            // should not see parameter completions because we set the enclosing declaration which will exclude the corresponding symbol
            // this avoids cycle suggestions
            completions.Where(c => c.Kind == SymbolKind.Parameter.ToCompletionItemKind()).Should().BeEmpty();
        }

        [TestMethod]
        public void CompletionsForModifierDefaultValuesShouldIncludeFunctionsValidInDefaultValues()
        {
            var grouping = SyntaxTreeGroupingFactory.CreateFromText(@"param p string {
  defaultValue: 
}");

            var offset = ((ObjectSyntax) grouping.EntryPoint.ProgramSyntax.Declarations.OfType<ParameterDeclarationSyntax>().Single().Modifier!).Properties.Single().Value.Span.Position;

            var compilation = new Compilation(TestResourceTypeProvider.Create(), grouping);
            var context = BicepCompletionContext.Create(grouping.EntryPoint, offset);

            var provider = new BicepCompletionProvider(new FileResolver());
            var completions = provider.GetFilteredCompletions(compilation, context).ToList();

            AssertExpectedFunctions(completions, expectParamDefaultFunctions: true);

            // outputs can't be referenced so they should not show up in completions
            completions.Where(c => c.Kind == SymbolKind.Output.ToCompletionItemKind()).Should().BeEmpty();

            completions.Where(c => c.Kind == SymbolKind.Variable.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Resource.ToCompletionItemKind()).Should().BeEmpty();
            completions.Where(c => c.Kind == SymbolKind.Module.ToCompletionItemKind()).Should().BeEmpty();

            // should not see parameter completions because we set the enclosing declaration which will exclude the corresponding symbol
            // this avoids cycle suggestions
            completions.Where(c => c.Kind == SymbolKind.Parameter.ToCompletionItemKind()).Should().BeEmpty();
        }

        [TestMethod]
        public void DeclaringSymbolWithFunctionNameShouldHideTheFunctionCompletion()
        {
            var grouping = SyntaxTreeGroupingFactory.CreateFromText(@"
param concat string
var resourceGroup = true
resource base64 'Microsoft.Foo/foos@2020-09-01' = {
  name: 'foo'
}
output length int = 
");
            var offset = grouping.EntryPoint.ProgramSyntax.Declarations.OfType<OutputDeclarationSyntax>().Single().Value.Span.Position;

            var compilation = new Compilation(TestResourceTypeProvider.Create(), grouping);
            var provider = new BicepCompletionProvider(new FileResolver());
            var context = BicepCompletionContext.Create(grouping.EntryPoint, offset);
            var completions = provider.GetFilteredCompletions(compilation, context).ToList();

            AssertExpectedFunctions(completions, expectParamDefaultFunctions: false, new[] {"sys.concat", "az.resourceGroup", "sys.base64"});

            // outputs can't be referenced so they should not show up in completions
            completions.Where(c => c.Kind == SymbolKind.Output.ToCompletionItemKind()).Should().BeEmpty();

            const string expectedVariable = "resourceGroup";
            var variableCompletion = completions.Single(c => c.Kind == SymbolKind.Variable.ToCompletionItemKind());
            variableCompletion.Label.Should().Be(expectedVariable);
            variableCompletion.Kind.Should().Be(CompletionItemKind.Variable);
            variableCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            variableCompletion.TextEdit!.NewText.Should().Be(expectedVariable);
            variableCompletion.CommitCharacters.Should().BeNull();
            variableCompletion.Detail.Should().Be(expectedVariable);

            const string expectedResource = "base64";
            var resourceCompletion = completions.Single(c => c.Kind == SymbolKind.Resource.ToCompletionItemKind());
            resourceCompletion.Label.Should().Be(expectedResource);
            resourceCompletion.Kind.Should().Be(CompletionItemKind.Interface);
            resourceCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            resourceCompletion.TextEdit!.NewText.Should().Be(expectedResource);
            resourceCompletion.CommitCharacters.Should().BeNull();
            resourceCompletion.Detail.Should().Be(expectedResource);

            const string expectedParam = "concat";
            var paramCompletion = completions.Single(c => c.Kind == SymbolKind.Parameter.ToCompletionItemKind());
            paramCompletion.Label.Should().Be(expectedParam);
            paramCompletion.Kind.Should().Be(CompletionItemKind.Field);
            paramCompletion.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
            paramCompletion.TextEdit!.NewText.Should().Be(expectedParam);
            paramCompletion.CommitCharacters.Should().BeNull();
            paramCompletion.Detail.Should().Be(expectedParam);
        }

        [TestMethod]
        public void OutputTypeContextShouldReturnDeclarationTypeCompletions()
        {
            var grouping = SyntaxTreeGroupingFactory.CreateFromText("output test ");
            var compilation = new Compilation(TestResourceTypeProvider.Create(), grouping);
            var provider = new BicepCompletionProvider(new FileResolver());

            var offset = grouping.EntryPoint.ProgramSyntax.Declarations.OfType<OutputDeclarationSyntax>().Single().Type.Span.Position;

            var completions = provider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(grouping.EntryPoint, offset));
            var declarationTypeCompletions = completions.Where(c => c.Kind == CompletionItemKind.Class).ToList();

            AssertExpectedDeclarationTypeCompletions(declarationTypeCompletions);

            completions.Where(c => c.Kind == CompletionItemKind.Snippet).Should().BeEmpty();
        }

        [TestMethod]
        public void ParameterTypeContextShouldReturnDeclarationTypeCompletions()
        {
            var grouping = SyntaxTreeGroupingFactory.CreateFromText("param foo ");
            var compilation = new Compilation(TestResourceTypeProvider.Create(), grouping);
            var provider = new BicepCompletionProvider(new FileResolver());

            var offset = grouping.EntryPoint.ProgramSyntax.Declarations.OfType<ParameterDeclarationSyntax>().Single().Type.Span.Position;

            var completions = provider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(grouping.EntryPoint, offset));
            var declarationTypeCompletions = completions.Where(c => c.Kind == CompletionItemKind.Class).ToList();

            AssertExpectedDeclarationTypeCompletions(declarationTypeCompletions);

            completions.Where(c => c.Kind == CompletionItemKind.Snippet).Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("secureObject");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.TextEdit!.NewText.Should().StartWith("object");
                    c.TextEdit.NewText.Should().Contain("secure: true");
                    c.Detail.Should().Be("Secure object");
                },
                c =>
                {
                    c.Label.Should().Be("secureString");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.TextEdit!.NewText.Should().StartWith("string");
                    c.TextEdit.NewText.Should().Contain("secure: true");
                    c.Detail.Should().Be("Secure string");
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
        public void CommentShouldNotGiveAnyCompletions(string codeFragment)
        {
        var grouping = SyntaxTreeGroupingFactory.CreateFromText(codeFragment);
        var compilation = new Compilation(TestResourceTypeProvider.Create(), grouping);
        var provider = new BicepCompletionProvider(new FileResolver());

        var offset = codeFragment.IndexOf('|');

        var completions = provider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(grouping.EntryPoint, offset));

        completions.Should().BeEmpty();
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
                    c.TextEdit!.NewText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "bool";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.TextEdit!.NewText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "int";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.TextEdit!.NewText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "object";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.TextEdit!.NewText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                },
                c =>
                {
                    const string expected = "string";
                    c.Label.Should().Be(expected);
                    c.Kind.Should().Be(CompletionItemKind.Class);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.TextEdit!.NewText.Should().Be(expected);
                    c.Detail.Should().Be(expected);
                });
        }

        private static void AssertExpectedFunctions(List<CompletionItem> completions, bool expectParamDefaultFunctions, IEnumerable<string>? fullyQualifiedFunctionNames = null)
        {
            fullyQualifiedFunctionNames ??= Enumerable.Empty<string>();

            var fullyQualifiedFunctionParts = fullyQualifiedFunctionNames.Select(fqfn =>
            {
                var parts = fqfn.Split('.', StringSplitOptions.None);
                return (@namespace: parts[0], function: parts[1]);
            });

            var functionCompletions = completions.Where(c => c.Kind == CompletionItemKind.Function).OrderBy(c => c.Label).ToList();

            var availableFunctionNames = new NamespaceSymbol[] {new AzNamespaceSymbol(ResourceScope.ResourceGroup), new SystemNamespaceSymbol()}
                .SelectMany(ns => ns.Type.MethodResolver.GetKnownFunctions().Values)
                .Where(symbol => expectParamDefaultFunctions || !symbol.FunctionFlags.HasFlag(FunctionFlags.ParamDefaultsOnly))
                .Select(func => func.Name)
                .Except(fullyQualifiedFunctionParts.Select(p => p.function), LanguageConstants.IdentifierComparer)
                .Concat(fullyQualifiedFunctionNames)
                .OrderBy(s => s);

            functionCompletions.Select(c => c.Label).Should().BeEquivalentTo(availableFunctionNames);
            functionCompletions.Should().OnlyContain(c => c.TextEdit!.NewText.StartsWith(c.Label + '(', StringComparison.Ordinal));
            functionCompletions.Should().OnlyContain(c => string.Equals(c.Label + "()", c.Detail));
            functionCompletions.Should().OnlyContain(c => c.InsertTextFormat == InsertTextFormat.Snippet);
        }
    }
}
