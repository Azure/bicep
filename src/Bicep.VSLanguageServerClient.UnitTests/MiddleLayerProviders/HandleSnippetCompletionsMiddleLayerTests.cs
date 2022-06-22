// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.VSLanguageServerClient.MiddleLayerProviders;
using FluentAssertions;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.VSLanguageServerClient.UnitTests.MiddleLayerProviders
{
    [TestClass]
    public class HandleSnippetCompletionsMiddleLayerTests
    {
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        public void GetUpdatedCompletionItem_InvalidText_DoesNothing(string text)
        {
            var handleSnippetCompletionsMiddleLayer = new HandleSnippetCompletionsMiddleLayer();
            var completionItem = new CompletionItem
            {
                Label = "author",
                InsertTextFormat = InsertTextFormat.Snippet,
                TextEdit = new TextEdit()
                {
                    NewText = text,
                    Range = new Range()
                    {
                        Start = new Position(0, 1),
                        End = new Position(0, 4)
                    }
                }
            };

            handleSnippetCompletionsMiddleLayer.GetUpdatedCompletionItem(completionItem).Should().Be(completionItem);
        }

        [TestMethod]
        public void GetUpdatedCompletionItem_WithPlainTextCompletionItem_DoesNothing()
        {
            var handleSnippetCompletionsMiddleLayer = new HandleSnippetCompletionsMiddleLayer();
            var completionItem = new CompletionItem
            {
                InsertText = "author",
                Label = "author",
                InsertTextFormat = InsertTextFormat.Plaintext
            };

            handleSnippetCompletionsMiddleLayer.GetUpdatedCompletionItem(completionItem).Should().Be(completionItem);
        }

        [TestMethod]
        public void GetUpdatedCompletionItem_WithNonChoiceSnippetSyntaxInCompletionItem_DoesNothing()
        {
            var handleSnippetCompletionsMiddleLayer = new HandleSnippetCompletionsMiddleLayer();

            var resourceText = "resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:Type}@${4:Version}' = {\r\n  name: $5\r\n  $0\r\n}";
            var completionItem = new CompletionItem
            {
                Label = "resource",
                InsertTextFormat = InsertTextFormat.Snippet,
                TextEdit = new TextEdit()
                {
                    NewText = resourceText,
                    Range = new Range()
                    {
                        Start = new Position(0, 1),
                        End = new Position(0, 4)
                    }
                }
            };

            handleSnippetCompletionsMiddleLayer.GetUpdatedCompletionItem(completionItem).Should().Be(completionItem);
        }

        [TestMethod]
        public void GetUpdatedCompletionItem_WithChoiceSnippetSyntaxInCompletionItem_ConvertsChoiceSyntaxToPlaceholderSyntax()
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
                TextEdit = new TextEdit()
                {
                    NewText = resourceText,
                    Range = new Range()
                    {
                        Start = new Position(0, 1),
                        End = new Position(0, 4)
                    }
                }
            };

            var result = handleSnippetCompletionsMiddleLayer.GetUpdatedCompletionItem(completionItem);
            var textEdit = result.TextEdit;
            textEdit.Should().NotBeNull();

            var expectedInsertText = @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:Type}@${4:Version}' = {
  name: ${5:'test1'}
  Application_Type: '${6:web}'
  kubernetesVersion: '${7:1.19.7}'
 $0
}";
            textEdit!.NewText.Should().Be(expectedInsertText);
        }
    }
}
