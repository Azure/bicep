// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit.CompileTimeImports;

internal class SymbolicReferenceCollector : AstVisitor
{
    private readonly HashSet<DeclaredSymbol> symbolsReferenced = new();
    private readonly SemanticModel model;

    private SymbolicReferenceCollector(SemanticModel model)
    {
        this.model = model;
    }

    internal static IEnumerable<DeclaredSymbol> CollectSymbolsReferenced(SemanticModel model, DeclaredSymbol symbol)
    {
        SymbolicReferenceCollector collector = new(model);
        symbol.DeclaringSyntax.Accept(collector);
        return collector.symbolsReferenced;
    }

    public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
    {
        if (model.GetSymbolInfo(syntax) is DeclaredSymbol symbolReferenced)
        {
            symbolsReferenced.Add(symbolReferenced);
        }
    }
}
