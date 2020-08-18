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
            this.Symbols = CreateFunctions(functionOverloads);
            this.WildcardSymbols = CreateWildcardFunctions(functionOverloads);
        }

        public override IEnumerable<Symbol> Descendants => this.Symbols.Values;

        public override void Accept(SymbolVisitor visitor) => visitor.VisitNamespaceSymbol(this);

        public override SymbolKind Kind => SymbolKind.Namespace;

        public ImmutableDictionary<string, FunctionSymbol> Symbols { get; }

        public ImmutableArray<FunctionWildcardSymbol> WildcardSymbols { get; }

        private static ImmutableDictionary<string, FunctionSymbol> CreateFunctions(IEnumerable<FunctionOverload> functionOverloads)
        {
            return functionOverloads
                .Where(fo => fo.RegexName == null)
                .GroupBy(fo => fo.Name, (name, overloads) => new FunctionSymbol(name, overloads), LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(f => f.Name, LanguageConstants.IdentifierComparer);
        }

        private static ImmutableArray<FunctionWildcardSymbol> CreateWildcardFunctions(IEnumerable<FunctionOverload> functionOverloads)
        {
            return functionOverloads
                .Where(fo => fo.RegexName != null)
                .GroupBy(fo => fo.Name, (name, overloads) => CreateWildCardFunction(name, overloads))
                .ToImmutableArray();
        }

        private static FunctionWildcardSymbol CreateWildCardFunction(string name, IEnumerable<FunctionOverload> overloads)
        {
            var regexName = overloads.First().RegexName;
            if (regexName == null || overloads.Skip(1).Any(fo => fo.RegexName?.ToString() != regexName.ToString()))
            {
                throw new ArgumentException("Found multiple overloads with different RegexName values");
            }

            return new FunctionWildcardSymbol(name, regexName, overloads);
        }
    }
}
