// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.LanguageServer.Completions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LangServer.UnitTests.Completions
{
    [TestClass]
    public class CompletionItemBuilderTests
    {
        [TestMethod]
        public void AddingSnippetTextEditToInsertTextShouldThrow()
        {
            Action fail = () => CompletionItemBuilder.Create(CompletionItemKind.Class, "label")
                .WithInsertText("t")
                .WithSnippetEdit(new Range(), "s");

            fail.Should().Throw<InvalidOperationException>().WithMessage("Unable to set the text edit because the insert text is already set.");
        }

        [TestMethod]
        public void AddingPlainTextEditToInsertTextShouldThrow()
        {
            Action fail = () => CompletionItemBuilder.Create(CompletionItemKind.Class, "label")
                .WithInsertText("t")
                .WithPlainTextEdit(new Range(), "t2");

            fail.Should().Throw<InvalidOperationException>().WithMessage("Unable to set the text edit because the insert text is already set.");
        }

        [TestMethod]
        public void AddingInsertTextToTextEditShouldThrow()
        {
            Action fail = () => CompletionItemBuilder.Create(CompletionItemKind.Class, "label")
                .WithPlainTextEdit(new Range(), "t")
                .WithInsertText("t2");

            fail.Should().Throw<InvalidOperationException>().WithMessage("Unable to set the specified insert text because a text edit is already set.");
        }

        [TestMethod]
        public void AddingSnippetToTextEditShouldThrow()
        {
            Action fail = () => CompletionItemBuilder.Create(CompletionItemKind.Class, "label")
                .WithSnippetEdit(new Range(), "s")
                .WithSnippet("s2");

            fail.Should().Throw<InvalidOperationException>().WithMessage("Unable to set the specified insert text because a text edit is already set.");
        }

        [TestMethod]
        public void SnippetCompletionItemTextEditTextShouldNotContainCarriageReturnCharacter()
        {
            var snippet = "module testModule 'main.bicep' = {\r\n  name: 'myModule'\r\n  }";
            var completionItemBuilder = CompletionItemBuilder.Create(CompletionItemKind.Snippet, "label")
                .WithSnippetEdit(new Range(), snippet);
            string completionItemTextEditText = completionItemBuilder.Build().TextEdit!.TextEdit!.NewText;

            completionItemTextEditText.Should().Be("module testModule 'main.bicep' = {\n  name: 'myModule'\n  }");         
        }

        [TestMethod]
        public void PlainTextCompletionItemTextEditTextShouldNotContainCarriageReturnCharacter()
        {
            var text = "module testModule 'main.bicep' = {\r\n  name: 'myModule'\r\n  }";
            var completionItemBuilder = CompletionItemBuilder.Create(CompletionItemKind.Text, "label")
                .WithPlainTextEdit(new Range(), text);
            string completionItemTextEditText = completionItemBuilder.Build().TextEdit!.TextEdit!.NewText;

            completionItemTextEditText.Should().Be("module testModule 'main.bicep' = {\n  name: 'myModule'\n  }");
        }
    }
}
