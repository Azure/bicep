// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.UnitTests.Utils
{
    public static class SymbolCollector
    {
        public static IList<Symbol> CollectSymbols(SemanticModel model)
        {
            var symbols = new List<Symbol>();
            var visitor = new SymbolCollectorVisitor(symbols);
            visitor.Visit(model.Root);

            return symbols;
        }

        private class SymbolCollectorVisitor(IList<Symbol> symbols) : SymbolVisitor
        {
            private readonly IList<Symbol> symbols = symbols;

            protected override void VisitInternal(Symbol symbol)
            {
                this.symbols.Add(symbol);
                base.VisitInternal(symbol);
            }
        }
    }
}

