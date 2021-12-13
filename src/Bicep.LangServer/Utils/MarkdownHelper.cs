// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Utils
{
    public static class MarkdownHelper
    {
        private const int MaxHoverMarkdownCodeBlockLength = 90000;
        //actual limit for hover in VS code is 100,000 characters.
        
        //we need to check for overflow due to using code blocks.
        //if we reach limit in a code block vscode will truncate it automatically, the block will not be terminated so the hover will not be properly formatted
        //therefore we need to check for the limit ourselves and truncate text inside code block, making sure it's terminated properly.
        private static string CodeBlock(string content) =>
        $"```bicep\n{(content.Length > MaxHoverMarkdownCodeBlockLength ? content.Substring(0, MaxHoverMarkdownCodeBlockLength) : content)}\n```\n";
        
        private static string DescriptionFormatting(string description) =>
        $"{description.Replace("\n", "  \n")}\n";

        // Markdown needs two leading whitespaces before newline to insert a line break
        public static string CodeBlockWithDescription(string content, string? description) => CodeBlock(content) + (description is not null ? DescriptionFormatting(description) : string.Empty);
    }
}