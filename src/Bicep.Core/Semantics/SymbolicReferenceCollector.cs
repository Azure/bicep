// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics;

internal class SymbolicReferenceCollector : AstVisitor
{
    private readonly HashSet<DeclaredSymbol> symbolsReferenced = new();
    private readonly IBinder binder;

    private SymbolicReferenceCollector(IBinder binder)
    {
        this.binder = binder;
    }

    internal static IEnumerable<DeclaredSymbol> CollectSymbolsReferenced(IBinder binder, DeclaredSymbol symbol)
    {
        SymbolicReferenceCollector collector = new(binder);
        symbol.DeclaringSyntax.Accept(collector);
        return collector.symbolsReferenced;
    }

    internal static IEnumerable<DeclaredSymbol> CollectSymbolsReferencedRecursive(IBinder binder, DeclaredSymbol symbol)
    {
        Queue<DeclaredSymbol> searchQueue = new(new[] { symbol });
        HashSet<DeclaredSymbol> searched = new();
        SymbolicReferenceCollector collector = new(binder);

        while (searchQueue.TryDequeue(out var toSearch))
        {
            searched.Add(toSearch);
            toSearch.DeclaringSyntax.Accept(collector);

            foreach (var referenced in collector.symbolsReferenced.Where(r => !searched.Contains(r)))
            {
                searchQueue.Enqueue(referenced);
            }
        }

        return collector.symbolsReferenced;
    }

    public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
    {
        if (binder.GetSymbolInfo(syntax) is DeclaredSymbol symbolReferenced)
        {
            symbolsReferenced.Add(symbolReferenced);
        }
    }
}
