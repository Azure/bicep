// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.TypeSystem
{
    public class FunctionResolver
    {
        public readonly IReadOnlyList<FunctionOverload> functionOverloads;
        private readonly IReadOnlyList<BannedFunction> bannedFunctions;
        private readonly ObjectType declaringObject;

        public FunctionResolver(ObjectType declaringObject, IEnumerable<FunctionOverload>? functionOverloads = null, IEnumerable<BannedFunction>? bannedFunctions = null)
        {
            this.functionOverloads = functionOverloads.CoalesceEnumerable().ToArray();
            this.bannedFunctions = bannedFunctions.CoalesceEnumerable().ToArray();

            var wildcardOverloads = this.functionOverloads.OfType<FunctionWildcardOverload>();

            // prepopulate cache with all known (non-wildcard) symbols
            // TODO: make cache building logic lazy
            this.FunctionCache = this.functionOverloads
                .Where(fo => fo is not FunctionWildcardOverload)
                .GroupBy(fo => fo.Name, (name, overloads) =>
                {
                    var matchingWildcards = wildcardOverloads.Where(x => x.WildcardRegex.IsMatch(name));

                    return new FunctionSymbol(declaringObject, name, overloads.Concat(matchingWildcards));
                }, LanguageConstants.IdentifierComparer)
                .ToDictionary(s => s.Name, s => s, LanguageConstants.IdentifierComparer);

            this.BannedFunctions = this.bannedFunctions.ToDictionary(bf => bf.Name, LanguageConstants.IdentifierComparer);

            // don't pre-build symbols for wildcard functions, because we don't want to equate two differently-named symbols with each other
            this.FunctionWildcardOverloads = this.functionOverloads
                .OfType<FunctionWildcardOverload>()
                .ToArray();
            this.declaringObject = declaringObject;
        }

        public FunctionResolver CopyToObject(ObjectType declaringObject)
            => new(declaringObject, functionOverloads, bannedFunctions);

        private IReadOnlyDictionary<string, FunctionSymbol> FunctionCache { get; }

        private IReadOnlyDictionary<string, BannedFunction> BannedFunctions { get; }

        private IReadOnlyList<FunctionWildcardOverload> FunctionWildcardOverloads { get; }

        public Symbol? TryGetSymbol(IdentifierSyntax identifierSyntax)
            => TryGetBannedFunction(identifierSyntax) ?? TryGetFunctionSymbol(identifierSyntax.IdentifierName);

        public IReadOnlyDictionary<string, FunctionSymbol> GetKnownFunctions()
            => this.FunctionCache;

        private Symbol? TryGetBannedFunction(IdentifierSyntax identifierSyntax)
        {
            if (BannedFunctions.TryGetValue(identifierSyntax.IdentifierName, out var banned))
            {
                return banned.CreateSymbol(DiagnosticBuilder.ForPosition(identifierSyntax));
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
            var wildcardOverloads = FunctionWildcardOverloads.Where(fo => fo.WildcardRegex.IsMatch(name));

            // create a new symbol for each unique name that matches the wildcard
            return wildcardOverloads.Any() ? new FunctionSymbol(declaringObject, name, wildcardOverloads) : null;
        }

        public static IEnumerable<FunctionOverload> GetMatches(
            IFunctionSymbol function,
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
                        return new[] { overload };

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

            if (potentialMatchOverloads.Any(x => x is not FunctionWildcardOverload))
            {
                // if we have an exact match, prioritize it over the wildcard match
                return potentialMatchOverloads.Where(x => x is not FunctionWildcardOverload);
            }

            return potentialMatchOverloads;
        }
    }
}
