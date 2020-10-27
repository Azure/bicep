﻿// Copyright (c) Microsoft Corporation.
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
            Action fail = () => CompletionItemBuilder.Create(CompletionItemKind.Class)
                .WithInsertText("t")
                .WithSnippetEdit(new Range(), "s");

            fail.Should().Throw<InvalidOperationException>().WithMessage("Unable to set the text edit because the insert text is already set.");
        }

        [TestMethod]
        public void AddingPlainTextEditToInsertTextShouldThrow()
        {
            Action fail = () => CompletionItemBuilder.Create(CompletionItemKind.Class)
                .WithInsertText("t")
                .WithPlainTextEdit(new Range(), "t2");

            fail.Should().Throw<InvalidOperationException>().WithMessage("Unable to set the text edit because the insert text is already set.");
        }

        [TestMethod]
        public void AddingInsertTextToTextEditShouldThrow()
        {
            Action fail = () => CompletionItemBuilder.Create(CompletionItemKind.Class)
                .WithPlainTextEdit(new Range(), "t")
                .WithInsertText("t2");

            fail.Should().Throw<InvalidOperationException>().WithMessage("Unable to set the specified insert text because a text edit is already set.");
        }

        [TestMethod]
        public void AddingSnippetToTextEditShouldThrow()
        {
            Action fail = () => CompletionItemBuilder.Create(CompletionItemKind.Class)
                .WithSnippetEdit(new Range(), "s")
                .WithSnippet("s2");

            fail.Should().Throw<InvalidOperationException>().WithMessage("Unable to set the specified insert text because a text edit is already set.");
        }
    }
}
