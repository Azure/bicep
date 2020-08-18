using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.SemanticModel
{
    public class NamespaceSymbol : Symbol
    {
        public NamespaceSymbol(string name, IEnumerable<FunctionOverload> functionOverloads)
            : base(name)
        {
            this.Symbols = CreateFunctions(functionOverloads.OfType<FunctionOverload>());
            this.FunctionWildcardOverloads = functionOverloads.OfType<FunctionWildcardOverload>().ToImmutableArray();
        }

        public override IEnumerable<Symbol> Descendants => this.Symbols.Values;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitNamespaceSymbol(this);

        public override SymbolKind Kind => SymbolKind.Namespace;

        private ImmutableDictionary<string, FunctionSymbol> Symbols { get; }

        private ImmutableArray<FunctionWildcardOverload> FunctionWildcardOverloads { get; }

        public FunctionSymbol? TryGetFunctionSymbol(string name)
        {
            // exact match
            if (Symbols.TryGetValue(name, out var symbol))
            {
                return symbol;
            }

            // wildcard match (e.g. list*)
            var wildcardOverloads =  FunctionWildcardOverloads.Where(fo => fo.WildcardRegex.IsMatch(name));
            if (!wildcardOverloads.Any())
            {
                return null;
            }

            // build a new symbol with the correct name. we don't want to return a symbol with name 'list*'.
            return new FunctionSymbol(name, wildcardOverloads);
        }

        private static ImmutableDictionary<string, FunctionSymbol> CreateFunctions(IEnumerable<FunctionOverload> functionOverloads)
        {
            return functionOverloads
                .GroupBy(fo => fo.Name, (name, overloads) => new FunctionSymbol(name, overloads), LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(f => f.Name, LanguageConstants.IdentifierComparer);
        }
    }
}