// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.Core.PrettyPrintV2.Documents
{
    public class TextDocument : Document
    {
        private static readonly ImmutableDictionary<string, TextDocument> TextDocumentPool = Enum.GetValues<TokenType>()
            .Select(SyntaxFacts.GetText)
            .OfType<string>()
            .Concat(LanguageConstants.DeclarationKeywords)
            .ToImmutableDictionary(x => x, x => new TextDocument(x));

        protected TextDocument(string value)
        {
            this.Value = value;
        }

        public string Value { get; }

        public virtual int Width => this.Value.Length;

        public static TextDocument From(string value) =>
            TextDocumentPool.TryGetValue(value, out var instance) ? instance : new TextDocument(value);

        public static implicit operator string(TextDocument document) => document.Value;

        public static implicit operator TextDocument(string content) => From(content);

        public override string ToString() => this.Value;

        public override IEnumerable<TextDocument> Flatten()
        {
            yield return this;
        }
    }
}
