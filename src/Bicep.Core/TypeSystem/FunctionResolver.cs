// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class FunctionResolver
    {
        public FunctionResolver(IEnumerable<FunctionOverload> functionOverloads, IEnumerable<BannedFunction>? bannedFunctions = null)
        {
            bannedFunctions ??= Enumerable.Empty<BannedFunction>();

            this.BannedFunctions = bannedFunctions
                .ToImmutableDictionary(bf => bf.Name, LanguageConstants.IdentifierComparer);

            this.FunctionOverloads = functionOverloads
                .Where(fo => fo is not FunctionWildcardOverload)
                .GroupBy(fo => fo.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(g => g.Key, g => g.ToImmutableArray(), LanguageConstants.IdentifierComparer);

            // don't pre-build symbols for wildcard functions, because we don't want to equate two differently-named symbols with each other
            this.FunctionWildcardOverloads = functionOverloads
                .OfType<FunctionWildcardOverload>()
                .ToImmutableArray();
        }

        private ImmutableDictionary<string, BannedFunction> BannedFunctions { get; }

        private ImmutableDictionary<string, ImmutableArray<FunctionOverload>> FunctionOverloads {get; }

        private ImmutableArray<FunctionWildcardOverload> FunctionWildcardOverloads { get; }

        public Symbol? TryGetSymbol(ObjectType declaringType, IdentifierSyntax identifierSyntax)
            => TryGetBannedFunction(identifierSyntax) ?? TryGetFunctionSymbol(declaringType, identifierSyntax.IdentifierName);

        public ImmutableDictionary<string, FunctionSymbol> GetKnownFunctions(ObjectType owner)
            => this.FunctionOverloads.ToImmutableDictionary(
                kvp => kvp.Key,
                kvp => new FunctionSymbol(owner, kvp.Key, kvp.Value),
                LanguageConstants.IdentifierComparer);

        private Symbol? TryGetBannedFunction(IdentifierSyntax identifierSyntax)
        {
            if (BannedFunctions.TryGetValue(identifierSyntax.IdentifierName, out var banned))
            {
                return banned.CreateSymbol(DiagnosticBuilder.ForPosition(identifierSyntax));
            }

            return null;
        }

        private FunctionSymbol? TryGetFunctionSymbol(ObjectType owner, string name)
        {
            // symbol comparison relies on object equality; use of this cache ensures that different symbols with the same name are not returned.
            // we also cache negative lookups (null) so that we don't slow down when looking up references to a missing symbol
            if (FunctionOverloads.TryGetValue(name, out var overloads))
            {
                return new FunctionSymbol(owner, name, overloads);
            }

            // wildcard match (e.g. list*)
            var wildcardOverloads = FunctionWildcardOverloads.Where(fo => fo.WildcardRegex.IsMatch(name));
            if (wildcardOverloads.Any())
            {
                // create a new symbol for each unique name that matches the wildcard
                return new FunctionSymbol(owner, name, wildcardOverloads);
            }

            return null;
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