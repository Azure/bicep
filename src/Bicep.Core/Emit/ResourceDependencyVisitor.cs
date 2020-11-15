// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    /// <summary> Class <c>ResourceDependencyVisitor</c>  provides two public methods: <c>GetAllResourceDependencies, GetResourceDependencies</c>
    /// The dependency graph currently tracks dependencies between three types of declared symbols: Resource, Module, (indirectly) Variables
    /// Modules and Resources only give immediate dependencies (NOT NESTED). Given three resources/modules a,b,c, 
    /// i.e. if a depends on b, and b depends on c,
    /// the graph would be {a: {b}, b: {c}}
    /// However, variables need to be evaluated for NESTED dependencies:
    /// i.e. if a (module/resource) depends on b (variable), and b depends c(module/resource)
    /// the graph would be {a: {c}} 
    /// </summary>
    public class ResourceDependencyVisitor : SyntaxVisitor
    {
        // shared data structure across all ResourceDependencyVisitors; our only "state" during traversals
        // Instead of traversing through variables repeatedly, we pre-calculate variable-resource dependencies before we build the graph
        private static Dictionary<VariableSymbol, ImmutableHashSet<DeclaredSymbol>> variableDependencies = new Dictionary<VariableSymbol, ImmutableHashSet<DeclaredSymbol>>();
        private readonly SemanticModel.SemanticModel model;
        // resource dependencies specific to each ResourceDependencyVisitor
        private HashSet<DeclaredSymbol> resourceDependencies;

        public static ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<DeclaredSymbol>> GetAllResourceDependencies(SemanticModel.SemanticModel model) 
        {
            var allResourceDependencies = new Dictionary<DeclaredSymbol, ImmutableHashSet<DeclaredSymbol>>();
            var declaredSymbols = model.Root.AllDeclarations.ToList(); // gets all declaredSymbols in the document (entry point)

            // we need to precompute variable dependencies to reuse them when traversing resources or modules
            for(int i = 0; i < declaredSymbols.Count; i++) 
            {
                if (declaredSymbols[i] is VariableSymbol variableSymbol)
                {
                    variableDependencies[variableSymbol] = GetResourceDependencies(model, variableSymbol.DeclaringSyntax);
                }
            }

            for (int i = 0; i < declaredSymbols.Count; i++)
            {
                switch(declaredSymbols[i])
                {
                    case ModuleSymbol moduleSymbol:
                        allResourceDependencies[moduleSymbol] = GetResourceDependencies(model, moduleSymbol.DeclaringSyntax);
                        continue;
                    case ResourceSymbol resourceSymbol:
                        allResourceDependencies[resourceSymbol] = GetResourceDependencies(model, resourceSymbol.DeclaringSyntax);
                        continue;
                }
            }
            return allResourceDependencies.ToImmutableDictionary();
        }

        public static ImmutableHashSet<DeclaredSymbol> GetResourceDependencies(SemanticModel.SemanticModel model, SyntaxBase syntax)
        {
            var visitor = new ResourceDependencyVisitor(model);
            visitor.Visit(syntax);
            return visitor.resourceDependencies.ToImmutableHashSet();
        }

        private ResourceDependencyVisitor(SemanticModel.SemanticModel model)
        {
            this.model = model;
            this.resourceDependencies = new HashSet<DeclaredSymbol>();
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            if (!(this.model.GetSymbolInfo(syntax) is ResourceSymbol resourceSymbol))
            {
                throw new InvalidOperationException("Unbound declaration");
            }
            base.VisitResourceDeclarationSyntax(syntax);
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            if (!(this.model.GetSymbolInfo(syntax) is ModuleSymbol moduleSymbol))
            {
                throw new InvalidOperationException("Unbound declaration");
            }
            base.VisitModuleDeclarationSyntax(syntax);
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            if (!(this.model.GetSymbolInfo(syntax) is VariableSymbol variableSymbol))
            {
                throw new InvalidOperationException("Unbound declaration");
            }
            base.VisitVariableDeclarationSyntax(syntax);
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