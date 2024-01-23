// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Utils;

public static class MarkdownHelper
{
    // Markdown needs two leading whitespaces before newline to insert a line break
    private const string MarkdownNewLine = "  \n";

    // actual limit for hover in VS code is 100,000 characters.
    private const int MaxCodeBlockLength = 90000;

    // we need to check for overflow due to using code blocks.
    // if we reach limit in a code block vscode will truncate it automatically, the block will not be terminated so the hover will not be properly formatted
    // therefore we need to check for the limit ourselves and truncate text inside code block, making sure it's terminated properly.
    public static string CodeBlock(string content) =>
        AppendNewline($"```bicep\n{(content.Length > MaxCodeBlockLength ? content[..MaxCodeBlockLength] : content)}\n```");

    public static string CodeBlockWithDescription(string content, string? description)
        => CodeBlock(content) + AppendNewline(description);

    public static string AppendNewline(string? input)
        => input is null ? string.Empty : $"{input}{MarkdownNewLine}";

    public static string JoinWithNewlines(IEnumerable<string> inputs)
        => string.Join(MarkdownNewLine, inputs);

    public static string? GetDocumentationLink(string? documentationUri)
        => documentationUri is null ? null : $"[View Documentation]({documentationUri})";
}
