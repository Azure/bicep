// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Bicep.Core.Semantics
{
    public class SymbolResultCache<TResult>(Func<Symbol, TResult> getResultFunc)
    {
        public TResult Lookup(Symbol symbol)
        {
            if (!resultCache.TryGetValue(symbol, out var result))
            {
                result = getResultFunc(symbol);
                resultCache[symbol] = result;
            }

            return result;
        }

        private readonly IDictionary<Symbol, TResult> resultCache = new Dictionary<Symbol, TResult>();

        private readonly Func<Symbol, TResult> getResultFunc = getResultFunc;
    }
}
