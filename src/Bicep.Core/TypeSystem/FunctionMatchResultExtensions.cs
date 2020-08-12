using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public static class FunctionMatchResultExtensions
    {
        public static FunctionMatchResult BestOf(this IEnumerable<FunctionMatchResult> source)
        {
            return source.Aggregate(FunctionMatchResult.Mismatch, (current, item) => current.BestOf(item));
        }

        public static FunctionMatchResult BestOf(this FunctionMatchResult one, FunctionMatchResult two) => (FunctionMatchResult) Math.Max((int) one, (int) two);
    }
}
