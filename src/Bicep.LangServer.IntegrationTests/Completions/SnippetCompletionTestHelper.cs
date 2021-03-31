// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bicep.LanguageServer.Snippets;
using FluentAssertions;

namespace Bicep.LangServer.IntegrationTests.Completions
{
    // Helper class to read test input file for snippet completions.
    // Input file should have following information- snippet index with replacement text, error codes(optional),
    // comment where we expect the snippet to be inserted after replacements.
    // Sample file contents:
    // $1 = myModule
    // $2 = ./output.bicep
    // $3 = 'testModule'

    // Insert snippet here
    public static class SnippetCompletionTestHelper
    {
        private static readonly Regex PlaceholderCommentPattern = new Regex(@"\/\/\s*\$(?<index>\d+)\s*=\s*(?<text>.*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string GetSnippetTextAfterPlaceholderReplacements(string snippetText, string placeholderText)
        {
            Snippet snippet = new Snippet(snippetText);

            // If we don't have placeholders, why is it a snippet?
            snippet.Placeholders.Should().NotBeEmpty();

            MatchCollection allMatches = PlaceholderCommentPattern.Matches(placeholderText);

            Dictionary<int, string> replacementsMap = new Dictionary<int, string>();

            foreach (Match match in allMatches)
            {
                int placeHolderIndex = int.Parse(match.Groups["index"].Value);
                string placeholderReplacement = match.Groups["text"].Value;

                replacementsMap.Add(placeHolderIndex, placeholderReplacement);
            }

            return snippet.Format((s, placeholder) => placeholder.Index >= 0 && placeholder.Index <= replacementsMap.Count
                ? (replacementsMap.ContainsKey(placeholder.Index) ? replacementsMap[placeholder.Index] : string.Empty)
                : string.Empty);
        }
    }
}
