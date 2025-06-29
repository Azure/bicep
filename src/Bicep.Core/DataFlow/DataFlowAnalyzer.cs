// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.DataFlow;

public class DataFlowAnalyzer
{
    private readonly SemanticModel semanticModel;

    public DataFlowAnalyzer(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
    }

    public IList<LocalVariableSymbol> GetInaccessibleLocalsAfterSyntaxMove(SyntaxBase syntax, SyntaxBase newParent)
    {
        var symbolDependencies = LocalSymbolDependencyVisitor.GetLocalSymbolDependencies(this.semanticModel, syntax);

        return [.. symbolDependencies.Where(symbol => !IsLocalAccessibleAfterMove(symbol, oldParent: syntax, newParent: newParent))];
    }

    private bool IsLocalAccessibleAfterMove(LocalVariableSymbol symbol, SyntaxBase oldParent, SyntaxBase newParent)
    {
        // find out the scope where the local symbol starts being accessible
        // (the declaration is outside of that node)
        var scope = GetScope(symbol);

        if (semanticModel.SourceFile.Hierarchy.IsEqualOrDescendent(node: newParent, potentialAncestor: scope.BindingSyntax))
        {
            // if the new parent is a child node of or equal to the binding syntax, the same scoping rules should apply.
            return true;
        }

        if (semanticModel.SourceFile.Hierarchy.IsEqualOrDescendent(node: scope.DeclaringSyntax, potentialAncestor: oldParent))
        {
            // if the scope declaration is a child or equal to the old parent, we aren't moving the bound syntax outside of its original scope.
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the syntax in which the specified symbol starts being accessible.
    /// </summary>
    /// <param name="symbol">the local variable symbol</param>
    private LocalScope GetScope(LocalVariableSymbol symbol)
    {
        var parent = this.semanticModel.GetSymbolParent(symbol);
        if (parent is LocalScope scope)
        {
            return scope;
        }

        throw new InvalidOperationException($"Unexpected local variable parent type '{parent?.GetType().Name}'");
    }
}
