// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.VSLanguageServerClient.MiddleLayerProviders;
using FluentAssertions;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.VSLanguageServerClient.UnitTests.MiddleLayerProviders
{
    [TestClass]
    public class HandleSnippetCompletionsMiddleLayerTests
    {
        [TestMethod]
        public void GetUpdatedCompletionItems_WithEmptyListOfCompletionItems_DoesNothing()
        {
            var handleSnippetCompletionsMiddleLayer = new HandleSnippetCompletionsMiddleLayer();
            var completionItems = Array.Empty<CompletionItem>();

            handleSnippetCompletionsMiddleLayer.GetUpdatedCompletionItems(completionItems).Should().BeEmpty();
        }

        [TestMethod]
        public void GetUpdatedCompletionItems_WithOnlyPlainTextCompletionItems_DoesNothing()
        {
            var handleSnippetCompletionsMiddleLayer = new HandleSnippetCompletionsMiddleLayer();
            var completionItem1 = new CompletionItem
            {
                InsertText = "author",
                Label = "author",
                InsertTextFormat = InsertTextFormat.Plaintext
            };

            var completionItem2 = new CompletionItem
            {
                InsertText = "branding",
                Label = "branding",
                InsertTextFormat = InsertTextFormat.Plaintext
            };
            var completionItems = new CompletionItem[] { completionItem1, completionItem2 };

            var result = handleSnippetCompletionsMiddleLayer.GetUpdatedCompletionItems(completionItems);

            result.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("author");
                    c.InsertText.Should().Be("author");
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Plaintext);;
                },
                c =>
                {
                    c.Label.Should().Be("branding");
                    c.InsertText.Should().Be("branding");
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Plaintext); ;
                });
        }

        [TestMethod]
        public void GetUpdatedCompletionItems_WithNonChoiceSnippetSyntaxInCompletionItems_DoesNothing()
        {
            var handleSnippetCompletionsMiddleLayer = new HandleSnippetCompletionsMiddleLayer();

            var resourceText = "resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:Type}@${4:Version}' = {\r\n  name: $5\r\n  $0\r\n}";
            var completionItem1 = new CompletionItem
            {
                InsertText = resourceText,
                Label = "resource",
                InsertTextFormat = InsertTextFormat.Snippet
            };

            var outputText = "output ${1:Identifier} ${2:Type} = $0";
            var completionItem2 = new CompletionItem
            {
                InsertText = outputText,
                Label = "output",
                InsertTextFormat = InsertTextFormat.Snippet
            };
            var completionItems = new CompletionItem[] { completionItem1, completionItem2 };

            var result = handleSnippetCompletionsMiddleLayer.GetUpdatedCompletionItems(completionItems);

            result.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("resource");
                    c.InsertText.Should().Be(resourceText);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                },
                c =>
                {
                    c.Label.Should().Be("output");
                    c.InsertText.Should().Be(outputText);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                });
        }

        [TestMethod]
        public void GetUpdatedCompletionItems_WithChoiceSnippetSyntaxInCompletionItems_ConvertsChoiceSyntaxToPlaceholderSyntax()
        {
            var handleSnippetCompletionsMiddleLayer = new HandleSnippetCompletionsMiddleLayer();

            var resourceText = @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:Type}@${4:Version}' = {
  name: ${5|'test1','test2'|}
  Application_Type: '${6|web,other|}'
  kubernetesVersion: '${7|1.19.7,1.19.6,1.18.14,1.18.10,1.17.16,1.17.13|}'
 $0
}";
            var completionItem = new CompletionItem
            {
                InsertText = resourceText,
                Label = "resource",
                InsertTextFormat = InsertTextFormat.Snippet
            };

            var completionItems = new CompletionItem[] { completionItem };

            var result = handleSnippetCompletionsMiddleLayer.GetUpdatedCompletionItems(completionItems);
            var expectedInsertText = @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:Type}@${4:Version}' = {
  name: ${5:'test1'}
  Application_Type: '${6:web}'
  kubernetesVersion: '${7:1.19.7}'
 $0
}";
            result.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("resource");
                    c.InsertText.Should().Be(expectedInsertText);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                });
        }
    }
}
