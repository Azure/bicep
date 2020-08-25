// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
            // prepopulate cache with all known (non-wildcard) symbols
            this.SymbolCache = functionOverloads
                .Where(fo => !(fo is FunctionWildcardOverload))
                .GroupBy(fo => fo.Name, (name, overloads) => new FunctionSymbol(name, overloads), LanguageConstants.IdentifierComparer)
                .ToDictionary<FunctionSymbol, string, FunctionSymbol?>(s => s.Name, s => s, LanguageConstants.IdentifierComparer);

            // don't pre-build symbols for wildcard functions, because we don't want to equate two differently-named symbols with each other
            this.FunctionWildcardOverloads = functionOverloads
                .OfType<FunctionWildcardOverload>()
                .ToImmutableArray();
        }

        public override IEnumerable<Symbol> Descendants => this.SymbolCache.Values.OfType<Symbol>();

        public override void Accept(SymbolVisitor visitor) => visitor.VisitNamespaceSymbol(this);

        public override SymbolKind Kind => SymbolKind.Namespace;

        private IDictionary<string, FunctionSymbol?> SymbolCache { get; }

        private ImmutableArray<FunctionWildcardOverload> FunctionWildcardOverloads { get; }

        public FunctionSymbol? TryGetFunctionSymbol(string name)
        {
            // symbol comparison relies on object equality; use of this cache ensures that different symbols with the same name are not returned.
            // we also cache negative lookups (null) so that we don't slow down when looking up references to a missing symbol
            if (SymbolCache.TryGetValue(name, out var symbol))
            {
                return symbol;
            }

            // wildcard match (e.g. list*)
            var wildcardOverloads =  FunctionWildcardOverloads.Where(fo => fo.WildcardRegex.IsMatch(name));

            // create a new symbol for each unique name that matches the wildcard
            var cachedSymbol = wildcardOverloads.Any() ? new FunctionSymbol(name, wildcardOverloads) : null;
            SymbolCache[name] = cachedSymbol;

            return cachedSymbol;
        }
    }
}
