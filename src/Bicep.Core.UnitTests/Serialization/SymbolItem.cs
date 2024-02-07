// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Newtonsoft.Json;

namespace Bicep.Core.UnitTests.Serialization
{
    [method: JsonConstructor]
    public class SymbolItem(string name, SymbolKind kind, TextSpan? span, TextSpan? nameSpan)
    {
        public SymbolItem(Symbol symbol) : this(symbol.Name, symbol.Kind, (symbol as DeclaredSymbol)?.DeclaringSyntax.Span, (symbol as DeclaredSymbol)?.NameSource.Span)
        {
        }

        public string Name { get; } = name;

        public SymbolKind Kind { get; } = kind;

        [JsonConverter(typeof(TextSpanConverter))]
        public TextSpan? Span { get; } = span;

        [JsonConverter(typeof(TextSpanConverter))]
        public TextSpan? NameSpan { get; } = nameSpan;
    }
}

