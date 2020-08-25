// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.SemanticModel;

namespace Bicep.Core.TypeSystem
{
    public static class FunctionResolver
    {
        // TODO: Can potentially remove this
        public static IEnumerable<FunctionOverload> GetMatches(
            NamespaceSymbol @namespace,
            string name,
            IList<TypeSymbol> argumentTypes,
            out IList<ArgumentCountMismatch> argumentCountMismatches,
            out IList<ArgumentTypeMismatch> argumentTypeMismatches)
        {
            var function = @namespace.TryGetFunctionSymbol(name);
            if (function != null)
            {
                return GetMatches(function, argumentTypes, out argumentCountMismatches, out argumentTypeMismatches);
            }

            argumentCountMismatches = new List<ArgumentCountMismatch>();
            argumentTypeMismatches = new List<ArgumentTypeMismatch>();

            // function does not exist
            return Enumerable.Empty<FunctionOverload>();
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

