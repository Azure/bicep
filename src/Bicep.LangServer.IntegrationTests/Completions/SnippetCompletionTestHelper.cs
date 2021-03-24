// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.LanguageServer.Snippets;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests.Completions
{
    // Helper class to read test input file for snippet completions.
    // Input file should have following information- snippet index with replacement text, error codes(optional),
    // comment where we expect the snippet to be inserted after replacements.
    // Sample file contents:
    // $1 = myModule
    // $2 = ./output.bicep
    // $3 = 'testModule'

    // Expected error codes for: Module declaration = BCP091

    // Insert snippet here
    public static class SnippetCompletionTestHelper
    {
        private static readonly Regex PlaceholderCommentPattern = new Regex(@"\/\/\s*\$(?<index>\d+)\s*=\s*(?<text>.*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex ErrorCodesCommentPattern = new Regex(@"\/\/\s*Expected error codes for:\s*(?<completionDetail>.*)\s*=\s*(?<errorCodes>.*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string GetSnippetTextAfterPlaceholderReplacements(CompletionItem completionItem, string placeholderText)
        {
            Snippet snippet = new Snippet(completionItem.TextEdit!.NewText);

            // If we don't have placeholders, why is it a snippet?
            snippet.Placeholders.Should().NotBeEmpty();

            // Documentation should have the snippet without placeholders
            completionItem.Documentation!.MarkupContent!.Value.Should().Contain(snippet.FormatDocumentation());

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

        public static List<string> GetErrorCodes(string completionDetail, string placeholderText)
        {
            MatchCollection allMatches = ErrorCodesCommentPattern.Matches(placeholderText);
            List<string> errorCodes = new List<string>();

            if (!allMatches.Any())
            {
                return errorCodes;
            }

            foreach (Match match in allMatches)
            {
                string completionDetailFromTestInputFile = match.Groups["completionDetail"].Value.Trim();

                // Some snippets have same label e.g. resource snippets. To help match error codes, we document the
                // completionItem detail in test input file. We'll return error codes only if the detail text match
                if (completionDetailFromTestInputFile == completionDetail)
                {
                    string errors = match.Groups["errorCodes"].Value;

                    return errors.Split(",").ToList();
                }
            }

            return errorCodes;
        }
    }
}
