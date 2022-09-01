// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
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

        public static IList<Symbol> CollectSymbols(ParamsSemanticModel model)
        {
            var symbols = new List<Symbol>();
            var visitor = new SymbolCollectorVisitor(symbols);
            visitor.Visit(model.Root);

            return symbols;
        }

        private class SymbolCollectorVisitor : SymbolVisitor
        {
            private readonly IList<Symbol> symbols;

            public SymbolCollectorVisitor(IList<Symbol> symbols)
            {
                this.symbols = symbols;
            }

            protected override void VisitInternal(Symbol symbol)
            {
                this.symbols.Add(symbol);
                base.VisitInternal(symbol);
            }
        }
    }
}

