// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    public class ResourceDependencyVisitor : SyntaxVisitor
    {
        private Dictionary<VariableSymbol, ImmutableHashSet<DeclaredSymbol>> variableDependencies;
        private readonly SemanticModel model;
        // resource dependencies specific to each ResourceDependencyVisitor
        private HashSet<DeclaredSymbol> resourceDependencies;

        public static ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<DeclaredSymbol>> GetAllResourceDependencies(SemanticModel model) 
        {
            var allResourceDependencies = new Dictionary<DeclaredSymbol, ImmutableHashSet<DeclaredSymbol>>();
            var declaredSymbols = model.Root.AllDeclarations.ToList(); // gets all declaredSymbols in the document (entry point)
            var visitor = new ResourceDependencyVisitor(model);

            // we need to precompute variable dependencies to reuse them when traversing resources or modules
            foreach(var variableSymbol in model.Root.VariableDeclarations) 
            {
                visitor.variableDependencies[variableSymbol] = GetResourceDependencies(model, variableSymbol.DeclaringSyntax);
            }

            foreach(var declaredSymbol in declaredSymbols.Where(symbol => symbol is ModuleSymbol || symbol is ResourceSymbol))
            {
                allResourceDependencies[declaredSymbol] = GetResourceDependencies(model, declaredSymbol.DeclaringSyntax);
            }
            return allResourceDependencies.ToImmutableDictionary();
        }

        public static ImmutableHashSet<DeclaredSymbol> GetResourceDependencies(SemanticModel model, SyntaxBase syntax)
        {
            var visitor = new ResourceDependencyVisitor(model);
            visitor.Visit(syntax);
            return visitor.resourceDependencies.ToImmutableHashSet();
        }

        private ResourceDependencyVisitor(SemanticModel model)
        {
            this.model = model;
            this.resourceDependencies = new HashSet<DeclaredSymbol>();
            this.variableDependencies = new Dictionary<VariableSymbol, ImmutableHashSet<DeclaredSymbol>>();
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            VisitVariableAccessSyntaxInternal(syntax);
            base.VisitVariableAccessSyntax(syntax);
        }

        private void VisitVariableAccessSyntaxInternal(VariableAccessSyntax syntax)
        {
            switch (model.GetSymbolInfo(syntax))
            {
                // recursively visit dependent variables until we have convered all first level (Non transitive)
                //  resources and modules
                case VariableSymbol variableSymbol:
                    if (!variableDependencies.TryGetValue(variableSymbol, out var dependencies))
                    {
                        this.Visit(variableSymbol.DeclaringSyntax);
                    } 
                    else 
                    {
                        resourceDependencies.UnionWith(variableDependencies[variableSymbol]);
                    }
                    return;
                // note: no recursion for transitive dependencies for resources and modules 
                case ResourceSymbol resourceSymbol:
                    resourceDependencies.Add(resourceSymbol);
                    return;
                case ModuleSymbol moduleSymbol:
                    resourceDependencies.Add(moduleSymbol);
                    return;
            }
        }
    }
}