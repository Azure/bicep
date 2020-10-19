// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;

namespace Bicep.Core.SemanticModel
{
    public class NamespaceSymbol : Symbol
    {
        public NamespaceSymbol(string name, IEnumerable<FunctionOverload> functionOverloads, IEnumerable<BannedFunction> bannedFunctions)
            : base(name)
        {
            // prepopulate cache with all known (non-wildcard) symbols
            this.FunctionCache = functionOverloads
                .Where(fo => !(fo is FunctionWildcardOverload))
                .GroupBy(fo => fo.Name, (name, overloads) => new FunctionSymbol(name, overloads), LanguageConstants.IdentifierComparer)
                .ToDictionary<FunctionSymbol, string, FunctionSymbol?>(s => s.Name, s => s, LanguageConstants.IdentifierComparer);

            this.BannedFunctions = bannedFunctions.ToImmutableDictionary(bf => bf.Name, LanguageConstants.IdentifierComparer);

            // don't pre-build symbols for wildcard functions, because we don't want to equate two differently-named symbols with each other
            this.FunctionWildcardOverloads = functionOverloads
                .OfType<FunctionWildcardOverload>()
                .ToImmutableArray();
        }

        public override IEnumerable<Symbol> Descendants => this.FunctionCache.Values.OfType<Symbol>();

        public override void Accept(SymbolVisitor visitor) => visitor.VisitNamespaceSymbol(this);

        public override SymbolKind Kind => SymbolKind.Namespace;

        private IDictionary<string, FunctionSymbol?> FunctionCache { get; }

        private ImmutableDictionary<string, BannedFunction> BannedFunctions { get; }

        private ImmutableArray<FunctionWildcardOverload> FunctionWildcardOverloads { get; }

        public Symbol? TryGetBannedFunction(string name, TextSpan nameSpan)
        {
            if (BannedFunctions.TryGetValue(name, out var banned))
            {
                return banned.CreateSymbol(DiagnosticBuilder.ForPosition(nameSpan));
            }

            return null;
        }

        public FunctionSymbol? TryGetFunctionSymbol(string name)
        {
            // symbol comparison relies on object equality; use of this cache ensures that different symbols with the same name are not returned.
            // we also cache negative lookups (null) so that we don't slow down when looking up references to a missing symbol
            if (FunctionCache.TryGetValue(name, out var symbol))
            {
                return symbol;
            }

            // wildcard match (e.g. list*)
            var wildcardOverloads =  FunctionWildcardOverloads.Where(fo => fo.WildcardRegex.IsMatch(name));

            // create a new symbol for each unique name that matches the wildcard
            var cachedSymbol = wildcardOverloads.Any() ? new FunctionSymbol(name, wildcardOverloads) : null;
            FunctionCache[name] = cachedSymbol;

            return cachedSymbol;
        }
    }
}
