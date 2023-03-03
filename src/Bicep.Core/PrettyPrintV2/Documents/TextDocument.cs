// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public sealed class TextDocument : Document
    {
        private static readonly ImmutableDictionary<string, TextDocument> TextDocumentPool =
            LanguageConstants.ContextualKeywords
            .Concat(LanguageConstants.Keywords.Keys)
            .Concat(new[] { " ", "(", ")", "[", "]", "{", "}", "=", ":", "+", "-", "*", "/", "!" })
            .Concat(new[] { "name", "properties", "string", "bool", "int", "array", "object" })
            .ToImmutableDictionary(value => value, value => new TextDocument(value));

        private TextDocument(string text)
        {
            this.Text = text;
        }

        public string Text { get; }

        public int Width => this.Text.Length;

        public static TextDocument Create(string content) => TextDocumentPool.TryGetValue(content, out var instance)
            ? instance
            : new TextDocument(content);

        public static implicit operator string(TextDocument document) => document.Text;

        public static implicit operator TextDocument(string content) => Create(content);

        public override string ToString() => this.Text;

        public override IEnumerable<TextDocument> Flatten()
        {
            yield return this;
        }
    }
}
