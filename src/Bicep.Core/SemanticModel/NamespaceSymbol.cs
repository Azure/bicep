using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.SemanticModel
{
    public class NamespaceSymbol : Symbol
    {
        public NamespaceSymbol(string name, IEnumerable<Symbol> functions)
            : base(name)
        {
            this.Symbols = functions.ToImmutableDictionary(f => f.Name, LanguageConstants.IdentifierComparer);
        }

        public NamespaceSymbol(string name, IEnumerable<FunctionOverload> functionOverloads)
            : this(name, CreateFunctions(functionOverloads))
        {
        }

        public override IEnumerable<Symbol> Descendants => this.Symbols.Values;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitNamespaceSymbol(this);

        public override SymbolKind Kind => SymbolKind.Namespace;

        public ImmutableDictionary<string, Symbol> Symbols { get; }

        private static ImmutableArray<FunctionSymbol> CreateFunctions(IEnumerable<FunctionOverload> functionOverloads)
        {
            return functionOverloads
                .GroupBy(fo => fo.Name, (name, overloads) => new FunctionSymbol(name, overloads), LanguageConstants.IdentifierComparer)
                .ToImmutableArray();
        }
    }
}
