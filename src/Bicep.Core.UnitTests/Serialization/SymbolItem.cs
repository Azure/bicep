using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Newtonsoft.Json;

namespace Bicep.Core.UnitTests.Serialization
{
    public class SymbolItem
    {
        [JsonConstructor]
        public SymbolItem(string name, SymbolKind kind, TextSpan? span, TextSpan? nameSpan)
        {
            this.Name = name;
            this.Kind = kind;
            this.Span = span;
            this.NameSpan = nameSpan;
        }

        public SymbolItem(Symbol symbol) : this(symbol.Name, symbol.Kind, (symbol as DeclaredSymbol)?.DeclaringSyntax.Span, (symbol as DeclaredSymbol)?.NameSyntax.Span)
        {
        }

        public string Name { get; }

        public SymbolKind Kind { get; }

        [JsonConverter(typeof(TextSpanConverter))]
        public TextSpan? Span { get; }

        [JsonConverter(typeof(TextSpanConverter))]
        public TextSpan? NameSpan { get; }
    }
}
