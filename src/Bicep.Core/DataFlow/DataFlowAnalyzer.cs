﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.DataFlow
{
    public class DataFlowAnalyzer
    {
        private readonly SemanticModel semanticModel;
        private readonly SymbolHierarchy hierarchy;

        public DataFlowAnalyzer(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
            
            this.hierarchy = new();
            this.hierarchy.AddRoot(this.semanticModel.Root);
        }

        public IList<LocalVariableSymbol> GetInaccessibleLocalsAfterSyntaxMove(SyntaxBase syntax, SyntaxBase newParent)
        {
            var symbolDependencies = LocalSymbolDependencyVisitor.GetLocalSymbolDependencies(this.semanticModel, syntax);

            return symbolDependencies
                .Where(symbol => !IsLocalAccessibleAfterMove(symbol, newParent))
                .ToList();
        }

        private bool IsLocalAccessibleAfterMove(LocalVariableSymbol symbol, SyntaxBase newParent)
        {
            // find out the scope where the local symbol starts being accessible
            // (the declaration is outside of that node)
            var bindingContainer = GetScopeBindingContainer(symbol);

            // the symbol remains accessible IFF the newParent is below the binding container
            return this.semanticModel.SyntaxTree.Hierarchy.IsDescendant(node: newParent, potentialAncestor: bindingContainer);
        }

        /// <summary>
        /// Gets the syntax in which the specified symbol starts being accessible.
        /// </summary>
        /// <param name="symbol">the local variable symbol</param>
        private SyntaxBase GetScopeBindingContainer(LocalVariableSymbol symbol)
        {
            var parent = this.hierarchy.GetParent(symbol);
            if(parent is LocalScope scope)
            {
                return scope.BindingSyntax;
            }

            throw new InvalidOperationException($"Unexpected local variable parent type '{parent?.GetType().Name}'");
        }
    }
}
