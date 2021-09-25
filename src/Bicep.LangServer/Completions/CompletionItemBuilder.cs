// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Completions
{
    public class CompletionItemBuilder
    {
        private readonly CompletionItemKind kind;
        private readonly string label;

        private TextEditContainer? additionalTextEdits;
        private Container<string>? commitCharacters;
        private string? detail;
        private StringOrMarkupContent? documentation;
        private string? filterText;
        private string? insertText;
        private InsertTextFormat insertTextFormat;
        private TextEditOrInsertReplaceEdit? textEdit;
        private InsertTextMode insertTextMode;
        
        private string? sortText;
        private bool preselect;
        private Command? command;

        private CompletionItemBuilder(CompletionItemKind kind, string label)
        {
            this.kind = kind;
            this.label = label;
        }

        public static CompletionItemBuilder Create(CompletionItemKind kind, string label) => new CompletionItemBuilder(kind, label);

        public CompletionItem Build()
        {
            return new()
            {
                Label = this.label,
                Kind = kind,

                AdditionalTextEdits = this.additionalTextEdits,
                CommitCharacters = this.commitCharacters,
                Detail = this.detail,
                Documentation = this.documentation,
                FilterText = this.filterText,
                InsertText = this.insertText,
                InsertTextFormat = this.insertTextFormat,
                TextEdit = this.textEdit,
                InsertTextMode = this.insertTextMode,                
                SortText = this.sortText,
                Preselect = this.preselect,
                Command = this.command
            };
        }

        public CompletionItemBuilder WithAdditionalEdits(TextEditContainer editContainer)
        {
            this.additionalTextEdits = editContainer;
            return this;
        }

        public CompletionItemBuilder WithCommitCharacters(Container<string> commitCharacters)
        {
            this.commitCharacters = commitCharacters;
            return this;
        }

        public CompletionItemBuilder WithDetail(string detail)
        {
            this.detail = detail;
            return this;
        }

        public CompletionItemBuilder WithDocumentation(string markdown)
        {
            this.documentation = new StringOrMarkupContent(new MarkupContent
            {
                Kind = MarkupKind.Markdown,
                Value = markdown
            });
            return this;
        }
        
        public CompletionItemBuilder WithFilterText(string filterText)
        {
            this.filterText = filterText;
            return this;
        }

        public CompletionItemBuilder WithInsertText(string insertText)
        {
            this.AssertNoTextEdit();

            this.insertText = insertText;
            this.insertTextFormat = InsertTextFormat.PlainText;
            this.insertTextMode = InsertTextMode.AdjustIndentation;

            return this;
        }
        
        public CompletionItemBuilder WithPlainTextEdit(Range range, string text)
        {
            this.AssertNoInsertText();
            this.SetTextEditInternal(range, InsertTextFormat.PlainText, text);
            return this;
        }

        public CompletionItemBuilder WithSnippet(string snippet)
        {
            this.AssertNoTextEdit();

            this.insertText = snippet;
            this.insertTextFormat = InsertTextFormat.Snippet;
            this.insertTextMode = InsertTextMode.AdjustIndentation;

            return this;
        }

        public CompletionItemBuilder WithSnippetEdit(Range range, string snippet)
        {
            this.AssertNoInsertText();
            this.SetTextEditInternal(range, InsertTextFormat.Snippet, snippet);
            return this;
        }

        public CompletionItemBuilder WithSortText(string sortText)
        {
            this.sortText = sortText;
            return this;
        }

        public CompletionItemBuilder Preselect() => this.Preselect(preselect: true);

        public CompletionItemBuilder Preselect(bool preselect)
        {
            this.preselect = preselect;
            return this;
        }

        public CompletionItemBuilder WithCommand(Command command)
        {
            this.command = command;
            return this;
        }

        private void SetTextEditInternal(Range range, InsertTextFormat format, string text)
        {
            this.insertTextFormat = format;
            this.textEdit = new TextEdit
            {
                Range = range,
                NewText = text.Replace("\r", string.Empty)
            };
            this.insertTextMode = InsertTextMode.AdjustIndentation;
        }

        private void AssertNoTextEdit()
        {
            if (this.textEdit != null)
            {
                throw new InvalidOperationException("Unable to set the specified insert text because a text edit is already set.");
            }
        }

        private void AssertNoInsertText()
        {
            if (this.insertText != null)
            {
                throw new InvalidOperationException("Unable to set the text edit because the insert text is already set.");
            }
        }
    }
}
