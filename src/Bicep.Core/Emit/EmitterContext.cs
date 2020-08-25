// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.SemanticModel;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Emit
{
    public class EmitterContext
    {
        private readonly SymbolResultCache<bool> variableRequiresInliningCache;

        public EmitterContext(SemanticModel.SemanticModel semanticModel)
        {
            this.SemanticModel = semanticModel;
            this.variableRequiresInliningCache = new SymbolResultCache<bool>(RequiresInliningInternal);
        }

        public SemanticModel.SemanticModel SemanticModel { get; }

        public bool RequiresInlining(VariableSymbol symbol) => variableRequiresInliningCache.Lookup(symbol);
        private bool RequiresInliningInternal(Symbol symbol)
        {
            if (SemanticModel.SymbolGraph.GetResourceDependencies(symbol).Any())
            {
                return true;
            }

            if (SemanticModel.SymbolGraph.GetFunctionDependencies(symbol).Any(f => f.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining)))
            {
                return true;
            }

            return false;
        }
    }
}
