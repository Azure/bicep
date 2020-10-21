// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using System.Text;
using Bicep.Core;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Snippets;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SymbolKind = Bicep.Core.SemanticModel.SymbolKind;

namespace Bicep.LanguageServer.Completions
{
    public static class CompletionItemFactory
    {
        private const string MarkdownNewLine = "  \n";

        private static readonly Container<string> PropertyCommitChars = new Container<string>(":");
        
        public static CompletionItem CreatePropertyNameCompletion(TypeProperty property, bool preselect = false, CompletionPriority priority = CompletionPriority.Medium) =>
            new CompletionItem
            {
                Kind = CompletionItemKind.Property,
                Label = property.Name,
                InsertTextFormat = InsertTextFormat.PlainText,
                // property names containg spaces need to be escaped
                InsertText = IsPropertyNameEscapingRequired(property) ? StringUtils.EscapeBicepString(property.Name) : property.Name,
                CommitCharacters = PropertyCommitChars,
                Detail = FormatPropertyDetail(property),
                Documentation = new StringOrMarkupContent(new MarkupContent
                {
                    Kind = MarkupKind.Markdown,
                    Value = FormatPropertyDocumentation(property)
                }),
                Preselect = preselect,
                SortText = GetSortText(property.Name, priority)
            };

        public static CompletionItem CreatePlaintextCompletion(CompletionItemKind kind, string insertText, string detail, bool preselect = false, CompletionPriority priority = CompletionPriority.Medium, Container<string>? commitCharacters = null) =>
            new CompletionItem
            {
                Kind = kind,
                Label = insertText,
                InsertTextFormat = InsertTextFormat.PlainText,
                InsertText = insertText,
                Detail = detail,
                Preselect = preselect,
                SortText = GetSortText(insertText, priority),
                CommitCharacters = commitCharacters
            };

        /// <summary>
        /// Creates a completion that inserts a snippet. The user may not necessarily know that a snippet is being inserted.
        /// </summary>
        public static CompletionItem CreateSnippetCompletion(CompletionItemKind kind, string label, string snippet, string detail, bool preselect = false, CompletionPriority priority = CompletionPriority.Medium, Container<string>? commitCharacters = null) =>
            new CompletionItem
            {
                Kind = kind,
                Label = label,
                InsertTextFormat = InsertTextFormat.Snippet,
                InsertText = snippet,
                Detail = detail,
                Preselect = preselect,
                SortText = GetSortText(label, priority),
                CommitCharacters = commitCharacters
            };

        public static CompletionItem CreateKeywordCompletion(string keyword, string detail, bool preselect = false, CompletionPriority priority = CompletionPriority.Medium) => CreatePlaintextCompletion(CompletionItemKind.Keyword, keyword, detail, preselect, priority);

        public static CompletionItem CreateTypeCompletion(TypeSymbol type, bool preselect = false, CompletionPriority priority = CompletionPriority.Medium) => CreatePlaintextCompletion(CompletionItemKind.Class, type.Name, type.Name, preselect, priority);

        /// <summary>
        /// Creates a completion with a contextual snippet. This will look like a snippet to the user.
        /// </summary>
        public static CompletionItem CreateContextualSnippetCompletion(string label, string detail, string snippet, bool preselect = false, CompletionPriority priority = CompletionPriority.Medium) =>
            new CompletionItem
            {
                Kind = CompletionItemKind.Snippet,
                Label = label,
                InsertTextFormat = InsertTextFormat.Snippet,
                InsertText = snippet,
                Detail = detail,
                Documentation = new StringOrMarkupContent(new MarkupContent
                {
                    Kind = MarkupKind.Markdown,
                    Value = $"```bicep\n{new Snippet(snippet).FormatDocumentation()}\n```"
                }),
                Preselect = preselect,
                SortText = GetSortText(label, priority)
            };

        public static CompletionItem CreateSymbolCompletion(Symbol symbol, bool preselect = false, string? insertText = null)
        {
            insertText ??= symbol.Name;
            var kind = GetCompletionItemKind(symbol);
            var priority = GetCompletionPriority(symbol);

            if (symbol is FunctionSymbol function)
            {
                // for functions withouth any parameters on all the overloads, we should be placing the cursor after the parentheses
                // for all other functions, the cursor should land between the parentheses so the user can specify the arguments
                bool hasParameters = function.Overloads.Any(overload => overload.HasParameters);
                var snippet = hasParameters
                    ? $"{insertText}($0)"
                    : $"{insertText}()$0";

                return CreateSnippetCompletion(kind, insertText, snippet, $"{insertText}()", preselect, priority);
            }

            return CreatePlaintextCompletion(kind, insertText, insertText, preselect, priority);
        }

        private static CompletionPriority GetCompletionPriority(Symbol symbol) =>
            symbol.Kind switch
            {
                SymbolKind.Function => CompletionPriority.Low,
                SymbolKind.Namespace => CompletionPriority.Low,
                _ => CompletionPriority.Medium
            };

        private static CompletionItemKind GetCompletionItemKind(Symbol symbol) =>
            symbol.Kind switch
            {
                SymbolKind.Function => CompletionItemKind.Function,
                SymbolKind.File => CompletionItemKind.File,
                SymbolKind.Variable => CompletionItemKind.Variable,
                SymbolKind.Namespace => CompletionItemKind.Folder,
                SymbolKind.Output => CompletionItemKind.Value,
                SymbolKind.Parameter => CompletionItemKind.Field,
                SymbolKind.Resource => CompletionItemKind.Interface,
                SymbolKind.Module => CompletionItemKind.Module,
                _ => CompletionItemKind.Text
            };

        private static bool IsPropertyNameEscapingRequired(TypeProperty property) => 
            !Lexer.IsValidIdentifier(property.Name) || LanguageConstants.Keywords.ContainsKey(property.Name);

        private static string FormatPropertyDetail(TypeProperty property) =>
            property.Flags.HasFlag(TypePropertyFlags.Required)
                ? $"{property.Name} (Required)"
                : property.Name;

        private static string FormatPropertyDocumentation(TypeProperty property)
        {
            var buffer = new StringBuilder();

            buffer.Append($"Type: `{property.TypeReference.Type}`{MarkdownNewLine}");

            if (property.Flags.HasFlag(TypePropertyFlags.ReadOnly))
            {
                // this case will be used for dot property access completions
                // this flag is not possible in property name completions
                buffer.Append($"Read-only property{MarkdownNewLine}");
            }

            if (property.Flags.HasFlag(TypePropertyFlags.WriteOnly))
            {
                buffer.Append($"Write-only property{MarkdownNewLine}");
            }

            if (property.Flags.HasFlag(TypePropertyFlags.Constant))
            {
                buffer.Append($"Requires a compile-time constant value.{MarkdownNewLine}");
            }

            return buffer.ToString();
        }

        // the priority must be a number in the sort text
        private static string GetSortText(string label, CompletionPriority priority) => $"{(int)priority}_{label}";
    }
}
