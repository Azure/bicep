using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.SemanticModel;

namespace Bicep.Core.TypeSystem
{
    public static class FunctionResolver
    {
        // TODO: Can potentially remove this
        public static IEnumerable<FunctionOverload> GetMatches(NamespaceSymbol @namespace, string name, IList<TypeSymbol> argumentTypes)
        {
            var function = @namespace.TryGetFunctionSymbol(name);
            if (function != null)
            {
                return GetMatches(function, argumentTypes);
            }

            // function does not exist
            return Enumerable.Empty<FunctionOverload>();
        }

        public static IEnumerable<FunctionOverload> GetMatches(FunctionSymbol function, IList<TypeSymbol> argumentTypes)
        {
            // lookup function overload matches by result type
            var candidateLookup = function.Overloads.ToLookup(fo => fo.Match(argumentTypes));

            if (candidateLookup.Contains(FunctionMatchResult.Match))
            {
                // for full match, just return the first one
                return candidateLookup[FunctionMatchResult.Match].Take(1);
            }

            if (candidateLookup.Contains(FunctionMatchResult.PotentialMatch))
            {
                // for partial matches, return all of them
                return candidateLookup[FunctionMatchResult.PotentialMatch];
            }

            // mismatches are not helpful
            return Enumerable.Empty<FunctionOverload>();
        }
    }
}
