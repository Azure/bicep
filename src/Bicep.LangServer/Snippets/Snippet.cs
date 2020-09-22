// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core.Parser;

namespace Bicep.LanguageServer.Snippets
{
    public sealed class Snippet
    {
        private static readonly Regex PlaceholderPattern = new Regex(@"\$({(?<index>\d+):(?<name>\w+)}|(?<index>\d+))", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public Snippet(string text)
        {
            var matches = PlaceholderPattern.Matches(text);

            this.Text = text;
            this.Placeholders = matches
                .Select(CreatePlaceholder)
                .OrderBy(p=>p.Index)
                .ToImmutableArray();

            this.Validate();
        }

        public string Text { get; }

        // placeholders ordered by index
        public ImmutableArray<SnippetPlaceholder> Placeholders { get; }

        public string Format(Func<Snippet, SnippetPlaceholder, string?> placeholderCallback)
        {
            // we will be performing multiple string replacements
            // better to do it in-place
            var buffer = new StringBuilder(this.Text);

            // to avoid recomputing spans, we will perform the replacements in reverse order by span position
            foreach (var placeholder in this.Placeholders.OrderByDescending(p => p.Span.Position))
            {
                // remove original placeholder
                buffer.Remove(placeholder.Span.Position, placeholder.Span.Length);

                // get the replacement string from the callback
                string? replacement = placeholderCallback(this, placeholder);

                // insert the replacement (if any)
                if (string.IsNullOrEmpty(replacement) == false)
                {
                    buffer.Insert(placeholder.Span.Position, replacement);
                }
            }

            return buffer.ToString();
        }

        public string FormatDocumentation() => Format((snippet, placeholder) => placeholder.Name);

        private static SnippetPlaceholder CreatePlaceholder(Match match)
        {
            var name = match.Groups["name"].Value;
            if (string.IsNullOrEmpty(name))
            {
                name = null;
            }

            return new SnippetPlaceholder(
                index: int.Parse(match.Groups["index"].Value),
                name: name,
                span: new TextSpan(match.Index, match.Length));
        }

        private void Validate()
        {
            // empty snippet is pointless but still valid
            if (this.Placeholders.IsEmpty)
            {
                return;
            }

            var firstPlaceholderIndex = this.Placeholders.First().Index;
            if (firstPlaceholderIndex != 0 && firstPlaceholderIndex != 1)
            {
                throw new ArgumentException($"The first snippet placeholder must have index 0 or 1, but the provided index is {firstPlaceholderIndex}");
            }

            // loop skips first placeholder
            for (int i = 1; i < this.Placeholders.Length; i++)
            {
                var current = this.Placeholders[i];
                var expectedIndex = firstPlaceholderIndex + i;

                if (current.Index != expectedIndex)
                {
                    throw new ArgumentException($"The placeholder indices must be contiguous increasing integers, but the placeholder with index {expectedIndex} is missing.");
                }
            }
        }
    }
}
