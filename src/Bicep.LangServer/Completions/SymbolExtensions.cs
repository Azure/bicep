// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.SemanticModel;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SymbolKind = Bicep.Core.SemanticModel.SymbolKind;

namespace Bicep.LanguageServer.Completions
{
    public static class SymbolExtensions
    {
        public static CompletionItem ToCompletionItem(this Symbol symbol)
        {
            return new CompletionItem
            {
                Label = symbol.Name,
                Kind = GetKind(symbol),
                Detail = symbol.Name,
                InsertText = symbol.Name,
                InsertTextFormat = InsertTextFormat.PlainText
            };
        }

        private static CompletionItemKind GetKind(this Symbol symbol)
        {
            return symbol.Kind switch
            {
                SymbolKind.Function => CompletionItemKind.Function,
                SymbolKind.File => CompletionItemKind.File,
                SymbolKind.Variable => CompletionItemKind.Variable,
                SymbolKind.Namespace => CompletionItemKind.Reference,
                SymbolKind.Output => CompletionItemKind.Value,
                SymbolKind.Parameter => CompletionItemKind.Field,
                SymbolKind.Resource => CompletionItemKind.Interface,
                _ => CompletionItemKind.Text
            };
        }
    }
}
