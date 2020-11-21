// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.TypeSystem
{
    public class FunctionResolver
    {
        public static FunctionResolver Empty { get; }
            = new FunctionResolver(Enumerable.Empty<FunctionOverload>(), Enumerable.Empty<BannedFunction>());

        public static FunctionResolver Create(IEnumerable<FunctionOverload>? functionOverloads)
            => functionOverloads != null ? new FunctionResolver(functionOverloads, Enumerable.Empty<BannedFunction>()) : FunctionResolver.Empty;

        public FunctionResolver(IEnumerable<FunctionOverload> functionOverloads, IEnumerable<BannedFunction>? bannedFunctions = null)
        {
            bannedFunctions ??= Enumerable.Empty<BannedFunction>();

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

        private IDictionary<string, FunctionSymbol?> FunctionCache { get; }

        private ImmutableDictionary<string, BannedFunction> BannedFunctions { get; }

        private ImmutableArray<FunctionWildcardOverload> FunctionWildcardOverloads { get; }

        public Symbol? TryGetSymbol(IdentifierSyntax identifierSyntax)
            => TryGetBannedFunction(identifierSyntax) ?? TryGetFunctionSymbol(identifierSyntax.IdentifierName);

        public ImmutableDictionary<string, FunctionSymbol> GetKnownFunctions()
            => this.FunctionCache.Values.ToImmutableDictionaryExcludingNullValues(symbol => symbol.Name, LanguageConstants.IdentifierComparer);
        
        private Symbol? TryGetBannedFunction(IdentifierSyntax identifierSyntax)
        {
            if (BannedFunctions.TryGetValue(identifierSyntax.IdentifierName, out var banned))
            {
                return banned.CreateSymbol(DiagnosticBuilder.ForPosition(identifierSyntax));
            }

            return null;
        }

        private FunctionSymbol? TryGetFunctionSymbol(string name)
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

        public static IEnumerable<FunctionOverload> GetMatches(
            FunctionSymbol function,
            IList<TypeSymbol> argumentTypes,
            out IList<ArgumentCountMismatch> argumentCountMismatches,
            out IList<ArgumentTypeMismatch> argumentTypeMismatches)
        {
            argumentCountMismatches = new List<ArgumentCountMismatch>();
            argumentTypeMismatches = new List<ArgumentTypeMismatch>();

            var potentialMatchOverloads = new List<FunctionOverload>();

            foreach (var overload in function.Overloads)
            {
                FunctionMatchResult matchResult = overload.Match(
                    argumentTypes,
                    out ArgumentCountMismatch? countMismatch,
                    out ArgumentTypeMismatch? typeMismatch);

                switch (matchResult)
                {
                    case FunctionMatchResult.Match:
                        // for full match, just return the first one
                        return new [] { overload };

                    case FunctionMatchResult.PotentialMatch:
                        potentialMatchOverloads.Add(overload);
                        break;

                    case FunctionMatchResult.Mismatch:
                        if (countMismatch != null)
                        {
                            argumentCountMismatches.Add(countMismatch);
                        }

                        if (typeMismatch != null)
                        {
                            argumentTypeMismatches.Add(typeMismatch);
                        }

                        break;
                }
            }

            if (potentialMatchOverloads.Any())
            {
                return potentialMatchOverloads;
            }

            return Enumerable.Empty<FunctionOverload>();
        }
    }
}