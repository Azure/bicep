﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Completions
{
    public static class CompletionItemBuilder
    {
        public static CompletionItem Create(CompletionItemKind kind) => new CompletionItem {Kind = kind};

        public static CompletionItem WithLabel(this CompletionItem item, string label)
        {
            item.Label = label;
            return item;
        }

        public static CompletionItem WithInsertText(this CompletionItem item, string insertText, InsertTextMode insertTextMode = InsertTextMode.AsIs)
        {
            AssertNoTextEdit(item);

            item.InsertText = insertText;
            item.InsertTextFormat = InsertTextFormat.PlainText;
            item.InsertTextMode = insertTextMode;

            return item;
        }

        public static CompletionItem WithSnippet(this CompletionItem item, string snippet, InsertTextMode insertTextMode = InsertTextMode.AsIs)
        {
            AssertNoTextEdit(item);

            item.InsertText = snippet;
            item.InsertTextFormat = InsertTextFormat.Snippet;
            item.InsertTextMode = insertTextMode;

            return item;
        }

        public static CompletionItem WithPlainTextEdit(this CompletionItem item, Range range, string text, InsertTextMode insertTextMode = InsertTextMode.AsIs)
        {
            AssertNoInsertText(item);
            SetTextEditInternal(item, range, InsertTextFormat.PlainText, text, insertTextMode);
            return item;
        }

        public static CompletionItem WithSnippetEdit(this CompletionItem item, Range range, string snippet, InsertTextMode insertTextMode = InsertTextMode.AsIs)
        {
            AssertNoInsertText(item);
            SetTextEditInternal(item, range, InsertTextFormat.Snippet, snippet, insertTextMode);
            return item;
        }

        public static CompletionItem WithAdditionalEdits(this CompletionItem item, TextEditContainer editContainer)
        {
            item.AdditionalTextEdits = editContainer;
            return item;
        }

        public static CompletionItem WithDetail(this CompletionItem item, string detail)
        {
            item.Detail = detail;
            return item;
        }

        public static CompletionItem WithDocumentation(this CompletionItem item, string markdown)
        {
            item.Documentation = new StringOrMarkupContent(new MarkupContent
            {
                Kind = MarkupKind.Markdown,
                Value = markdown
            });
            return item;
        }

        public static CompletionItem WithSortText(this CompletionItem item, string sortText)
        {
            item.SortText = sortText;
            return item;
        }

        public static CompletionItem Preselect(this CompletionItem item) => item.Preselect(preselect: true);

        public static CompletionItem Preselect(this CompletionItem item, bool preselect)
        {
            item.Preselect = preselect;
            return item;
        }

        public static CompletionItem WithCommitCharacters(this CompletionItem item, Container<string> commitCharacters)
        {
            item.CommitCharacters = commitCharacters;
            return item;
        }

        private static void SetTextEditInternal(CompletionItem item, Range range, InsertTextFormat format, string text, InsertTextMode insertTextMode)
        {
            item.InsertTextFormat = format;
            item.TextEdit = new TextEdit
            {
                Range = range,
                NewText = text
            };
            item.InsertTextMode = insertTextMode;
        }

        private static void AssertNoTextEdit(CompletionItem item)
        {
            if (item.TextEdit != null)
            {
                throw new InvalidOperationException("Unable to set the specified insert text because a text edit is already set.");
            }
        }

        private static void AssertNoInsertText(CompletionItem item)
        {
            if (item.InsertText != null)
            {
                throw new InvalidOperationException("Unable to set the text edit because the insert text is already set.");
            }
        }
    }
}
