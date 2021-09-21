// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;

namespace Bicep.Core.TypeSystem
{
    public class FunctionResolver
    {
        private readonly ImmutableArray<FunctionOverload> functionOverloads;
        private readonly ImmutableArray<BannedFunction> bannedFunctions;
        private readonly ObjectType declaringObject;

        public FunctionResolver(ObjectType declaringObject, IEnumerable<FunctionOverload>? functionOverloads = null, IEnumerable<BannedFunction>? bannedFunctions = null)
        {
            this.functionOverloads = functionOverloads?.ToImmutableArray() ?? ImmutableArray<FunctionOverload>.Empty;
            this.bannedFunctions = bannedFunctions?.ToImmutableArray() ?? ImmutableArray<BannedFunction>.Empty;
            
            // prepopulate cache with all known (non-wildcard) symbols
            // TODO: make cache building logic lazy
            this.FunctionCache = this.functionOverloads
                .Where(fo => fo is not FunctionWildcardOverload)
                .GroupBy(fo => fo.Name, (name, overloads) => new FunctionSymbol(declaringObject, name, overloads), LanguageConstants.IdentifierComparer)
                .ToDictionary<FunctionSymbol, string, FunctionSymbol?>(s => s.Name, s => s, LanguageConstants.IdentifierComparer);

            this.BannedFunctions = this.bannedFunctions.ToImmutableDictionary(bf => bf.Name, LanguageConstants.IdentifierComparer);

            // don't pre-build symbols for wildcard functions, because we don't want to equate two differently-named symbols with each other
            this.FunctionWildcardOverloads = this.functionOverloads
                .OfType<FunctionWildcardOverload>()
                .ToImmutableArray();
            this.declaringObject = declaringObject;
        }

        public FunctionResolver CopyToObject(ObjectType declaringObject)
            => new(declaringObject, functionOverloads, bannedFunctions);

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
            return wildcardOverloads.Any() ? new FunctionSymbol(declaringObject, name, wildcardOverloads) : null;
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
