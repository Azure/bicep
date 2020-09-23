﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SymbolKind = Bicep.Core.SemanticModel.SymbolKind;

namespace Bicep.LangServer.UnitTests.Completions
{
    public static class SymbolKindTestExtensions
    {
        public static CompletionItemKind ToCompletionItemKind(this SymbolKind symbolKind) =>
            // this method is intentionally a duplicate of a similar one in core
            symbolKind switch
            {
                SymbolKind.Parameter => CompletionItemKind.Field,
                SymbolKind.Variable => CompletionItemKind.Variable,
                SymbolKind.Resource => CompletionItemKind.Interface,
                SymbolKind.Output => CompletionItemKind.Value,
                SymbolKind.Namespace => CompletionItemKind.Reference,
                SymbolKind.Function => CompletionItemKind.Function,
                _ => throw new AssertFailedException($"Unexpected symbol kind '{symbolKind}'.")
            };
    }
}
