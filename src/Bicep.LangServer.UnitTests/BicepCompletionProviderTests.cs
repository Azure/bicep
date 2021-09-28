// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.UnitTests.Completions;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Snippets;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using SymbolKind = Bicep.Core.Semantics.SymbolKind;

namespace Bicep.LangServer.UnitTests
{
    [TestClass]
    public class BicepCompletionProviderTests
    {
        private static readonly MockRepository Repository = new MockRepository(MockBehavior.Strict);
        private static readonly ILanguageServerFacade Server = Repository.Create<ILanguageServerFacade>().Object;
        private static readonly SnippetsProvider snippetsProvider = new(TestTypeHelper.CreateEmptyProvider(), BicepTestConstants.FileResolver, BicepTestConstants.ConfigurationManager);

        [TestMethod]
        public void DeclarationContextShouldReturnKeywordCompletions()
        {
            var grouping = SourceFileGroupingFactory.CreateFromText(string.Empty, BicepTestConstants.FileResolver);
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), grouping, BicepTestConstants.BuiltInConfiguration);
            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();

            var provider = new BicepCompletionProvider(BicepTestConstants.FileResolver, snippetsProvider, new TelemetryProvider(Server), BicepTestConstants.Features);

            var completions = provider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(BicepTestConstants.Features, compilation, 0));

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
                    c.Label.Should().Be("var");
                    c.Kind.Should().Be(CompletionItemKind.Keyword);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.PlainText);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Variable keyword");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("var");
                });
        }

        [TestMethod]
        public void NonDeclarationContextShouldIncludeDeclaredSymbols()
        {
            var grouping = SourceFileGroupingFactory.CreateFromText(@"
param p string
var v = 
resource r 'Microsoft.Foo/foos@2020-09-01' = {
  name: 'foo'
}
output o int = 42
", BicepTestConstants.FileResolver);
            var offset = grouping.EntryPoint.ProgramSyntax.Declarations.OfType<VariableDeclarationSyntax>().Single().Value.Span.Position;
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), grouping, BicepTestConstants.BuiltInConfiguration);
            
            var provider = new BicepCompletionProvider(BicepTestConstants.FileResolver, snippetsProvider, new TelemetryProvider(Server), BicepTestConstants.Features);
            var context = BicepCompletionContext.Create(BicepTestConstants.Features, compilation, offset);
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
            resourceCompletion.TextEdit!.TextEdit!.NewText.Should().Be(expectedResource);
            resourceCompletion.CommitCharacters.Should().BeEquivalentTo(new[]{ ":", });
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
        public void CompletionsForOneLinerParameterDefaultValueShouldIncludeFunctionsValidInDefaultValues()
        {
            var grouping = SourceFileGroupingFactory.CreateFromText(@"param p string = ", BicepTestConstants.FileResolver);
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), grouping, BicepTestConstants.BuiltInConfiguration);

            var offset = ((ParameterDefaultValueSyntax) grouping.EntryPoint.ProgramSyntax.Declarations.OfType<ParameterDeclarationSyntax>().Single().Modifier!).DefaultValue.Span.Position;

            var provider = new BicepCompletionProvider(BicepTestConstants.FileResolver, snippetsProvider, new TelemetryProvider(Server), BicepTestConstants.Features);
            var completions = provider.GetFilteredCompletions(
                compilation,
                BicepCompletionContext.Create(BicepTestConstants.Features, compilation, offset)).ToList();

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
            var grouping = SourceFileGroupingFactory.CreateFromText(@"
param concat string
var resourceGroup = true
resource base64 'Microsoft.Foo/foos@2020-09-01' = {
  name: 'foo'
}
output length int = 
", BicepTestConstants.FileResolver);
            var offset = grouping.EntryPoint.ProgramSyntax.Declarations.OfType<OutputDeclarationSyntax>().Single().Value.Span.Position;

            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), grouping, BicepTestConstants.BuiltInConfiguration);
            var provider = new BicepCompletionProvider(BicepTestConstants.FileResolver, snippetsProvider, new TelemetryProvider(Server), BicepTestConstants.Features);
            var context = BicepCompletionContext.Create(BicepTestConstants.Features, compilation, offset);
            var completions = provider.GetFilteredCompletions(compilation, context).ToList();

            AssertExpectedFunctions(completions, expectParamDefaultFunctions: false, new[] {"sys.concat", "az.resourceGroup", "sys.base64"});

            // outputs can't be referenced so they should not show up in completions
            completions.Where(c => c.Kind == SymbolKind.Output.ToCompletionItemKind()).Should().BeEmpty();

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
            resourceCompletion.CommitCharacters.Should().BeEquivalentTo(new []{ ":", });
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
        public void OutputTypeContextShouldReturnDeclarationTypeCompletions()
        {
            var grouping = SourceFileGroupingFactory.CreateFromText("output test ", BicepTestConstants.FileResolver);
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), grouping, BicepTestConstants.BuiltInConfiguration);
            var provider = new BicepCompletionProvider(BicepTestConstants.FileResolver, snippetsProvider, new TelemetryProvider(Server), BicepTestConstants.Features);

            var offset = grouping.EntryPoint.ProgramSyntax.Declarations.OfType<OutputDeclarationSyntax>().Single().Type.Span.Position;

            var completions = provider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(BicepTestConstants.Features, compilation, offset));
            var declarationTypeCompletions = completions.Where(c => c.Kind == CompletionItemKind.Class).ToList();

            AssertExpectedDeclarationTypeCompletions(declarationTypeCompletions);

            completions.Where(c => c.Kind == CompletionItemKind.Snippet).Should().BeEmpty();
        }

        [TestMethod]
        public void ParameterTypeContextShouldReturnDeclarationTypeCompletions()
        {
            var grouping = SourceFileGroupingFactory.CreateFromText("param foo ", BicepTestConstants.FileResolver);
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), grouping, BicepTestConstants.BuiltInConfiguration);
            var provider = new BicepCompletionProvider(BicepTestConstants.FileResolver, snippetsProvider, new TelemetryProvider(Server), BicepTestConstants.Features);

            var offset = grouping.EntryPoint.ProgramSyntax.Declarations.OfType<ParameterDeclarationSyntax>().Single().Type.Span.Position;

            var completions = provider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(BicepTestConstants.Features, compilation, offset));
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
                    c.Label.Should().Be("secureString");
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
        public void VerifyParameterTypeCompletionWithPrecedingComment()
        {
            var grouping = SourceFileGroupingFactory.CreateFromText("/*test*/param foo ", BicepTestConstants.FileResolver);
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), grouping, BicepTestConstants.BuiltInConfiguration);
            var provider = new BicepCompletionProvider(BicepTestConstants.FileResolver, snippetsProvider, new TelemetryProvider(Server), BicepTestConstants.Features);

            var offset = grouping.EntryPoint.ProgramSyntax.Declarations.OfType<ParameterDeclarationSyntax>().Single().Type.Span.Position;

            var completions = provider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(BicepTestConstants.Features, compilation, offset));
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
                    c.Label.Should().Be("secureString");
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
        public void CommentShouldNotGiveAnyCompletions(string codeFragment)
        {
        var grouping = SourceFileGroupingFactory.CreateFromText(codeFragment, BicepTestConstants.FileResolver);
        var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), grouping, BicepTestConstants.BuiltInConfiguration);
        var provider = new BicepCompletionProvider(BicepTestConstants.FileResolver, snippetsProvider, new TelemetryProvider(Server), BicepTestConstants.Features);

        var offset = codeFragment.IndexOf('|');

        var completions = provider.GetFilteredCompletions(compilation, BicepCompletionContext.Create(BicepTestConstants.Features, compilation, offset));

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
            fullyQualifiedFunctionNames ??= Enumerable.Empty<string>();

            var fullyQualifiedFunctionParts = fullyQualifiedFunctionNames.Select(fqfn =>
            {
                var parts = fqfn.Split('.', StringSplitOptions.None);
                return (@namespace: parts[0], function: parts[1]);
            });

            var functionCompletions = completions.Where(c => c.Kind == CompletionItemKind.Function).OrderBy(c => c.Label).ToList();

            var namespaceProvider = BicepTestConstants.NamespaceProvider;
            var namespaces = new [] {
                namespaceProvider.TryGetNamespace("az", "az", ResourceScope.ResourceGroup)!,
                namespaceProvider.TryGetNamespace("sys", "sys", ResourceScope.ResourceGroup)!,
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
            functionCompletions.Should().OnlyContain(c => string.Equals(c.Label + "()", c.Detail));
            functionCompletions.Should().OnlyContain(c => c.InsertTextFormat == InsertTextFormat.Snippet);
        }
    }
}
