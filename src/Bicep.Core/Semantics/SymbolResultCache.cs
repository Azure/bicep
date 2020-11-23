// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;

namespace Bicep.Core.Semantics
{
    public class SymbolResultCache<TResult>
    {
        public SymbolResultCache(Func<Symbol, TResult> getResultFunc)
        {
            resultCache = new Dictionary<Symbol, TResult>();
            this.getResultFunc = getResultFunc;
        }

        public TResult Lookup(Symbol symbol)
        {
            if (!resultCache.TryGetValue(symbol, out var result))
            {
                result = getResultFunc(symbol);
                resultCache[symbol] = result;
            }

            return result;
        }

        private readonly IDictionary<Symbol, TResult> resultCache;

        private readonly Func<Symbol, TResult> getResultFunc;
    }
}
