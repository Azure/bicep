// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core.Parsing;
using Bicep.LanguageServer.Completions;

namespace Bicep.LanguageServer.Snippets
{
    public sealed class Snippet
    {
        // Below regex is used to detect following snippet syntax:
        // ${(?<index>\d+):(?<name>[^}]+)} detects placeholders i.e. tab stops with values e.g ${1:foo}
        // $(?<index>\d+) detects tab stops e.g. $1
        // $(?<index>\d+)\|((?<name>[^,]+)(.*))\|} detects placeholders with choices e.g. ${1|one,two,three|}
        // See https://microsoft.github.io/language-server-protocol/specifications/specification-current/#snippet_syntax for more information
        private static readonly Regex PlaceholderPattern = new Regex(@"\$({(?<index>\d+):(?<name>[^}]+)}|(?<index>\d+)|{(?<index>\d+)\|((?<name>[^,]+)(.*))\|})", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public Snippet(string text, CompletionPriority completionPriority = CompletionPriority.Medium, string prefix = "", string detail = "")
        {
            var matches = PlaceholderPattern.Matches(text);

            this.Text = text;
            this.Prefix = prefix;
            this.Detail = detail;
            this.CompletionPriority = completionPriority;
            this.Placeholders = matches
                .Select(CreatePlaceholder)
                .OrderBy(p=>p.Index)
                .ToImmutableArray();
        }

        public string Prefix { get; }

        public string Detail { get; }

        public string Text { get; }

        public CompletionPriority CompletionPriority { get; }

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
    }
}
