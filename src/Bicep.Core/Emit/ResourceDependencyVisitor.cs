// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.DataFlow;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    public class ResourceDependencyVisitor : SyntaxVisitor
    {
        private readonly SemanticModel model;
        private readonly IDictionary<DeclaredSymbol, HashSet<ResourceDependency>> resourceDependencies;
        private DeclaredSymbol? currentDeclaration;

        public static ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> GetResourceDependencies(SemanticModel model)
        {
            var visitor = new ResourceDependencyVisitor(model);
            visitor.Visit(model.Root.Syntax);

            var output = new Dictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>>();
            foreach (var kvp in visitor.resourceDependencies)
            {
                if (kvp.Key is ResourceSymbol || kvp.Key is ModuleSymbol)
                {
                    output[kvp.Key] = OptimizeDependencies(kvp.Value);
                }
            }
            return output.ToImmutableDictionary();
        }

        private static ImmutableHashSet<ResourceDependency> OptimizeDependencies(HashSet<ResourceDependency> dependencies) =>
            dependencies
                .GroupBy(dep => dep.Resource)
                .SelectMany(group => @group.FirstOrDefault(dep => dep.IndexExpression == null) is { } dependencyWithoutIndex
                    ? dependencyWithoutIndex.AsEnumerable()
                    : @group)
                .ToImmutableHashSet();

        private ResourceDependencyVisitor(SemanticModel model)
        {
            this.model = model;
            this.resourceDependencies = new Dictionary<DeclaredSymbol, HashSet<ResourceDependency>>();
            this.currentDeclaration = null;
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            if (!(this.model.GetSymbolInfo(syntax) is ResourceSymbol resourceSymbol))
            {
                throw new InvalidOperationException("Unbound declaration");
            }

            // Resource ancestors are always dependencies.
            var ancestors = this.model.ResourceAncestors.GetAncestors(resourceSymbol);

            // save previous declaration as we may call this recursively
            var prevDeclaration = this.currentDeclaration;

            this.currentDeclaration = resourceSymbol;
            this.resourceDependencies[resourceSymbol] = new HashSet<ResourceDependency>(ancestors.Select(a => new ResourceDependency(a.Resource, a.IndexExpression)));
            base.VisitResourceDeclarationSyntax(syntax);

            // restore previous declaration
            this.currentDeclaration = prevDeclaration;
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            if (!(this.model.GetSymbolInfo(syntax) is ModuleSymbol moduleSymbol))
            {
                throw new InvalidOperationException("Unbound declaration");
            }

            // save previous declaration as we may call this recursively
            var prevDeclaration = this.currentDeclaration;

            this.currentDeclaration = moduleSymbol;
            this.resourceDependencies[moduleSymbol] = new HashSet<ResourceDependency>();
            base.VisitModuleDeclarationSyntax(syntax);

            // restore previous declaration
            this.currentDeclaration = prevDeclaration;
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            if (!(this.model.GetSymbolInfo(syntax) is VariableSymbol variableSymbol))
            {
                throw new InvalidOperationException("Unbound declaration");
            }

            // save previous declaration as we may call this recursively
            var prevDeclaration = this.currentDeclaration;

            this.currentDeclaration = variableSymbol;
            this.resourceDependencies[variableSymbol] = new HashSet<ResourceDependency>();
            base.VisitVariableDeclarationSyntax(syntax);

            // restore previous declaration
            this.currentDeclaration = prevDeclaration;
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (currentDeclaration is null)
            {
                return;
            }

            switch (model.GetSymbolInfo(syntax))
            {
                case VariableSymbol variableSymbol:
                    if (!resourceDependencies.TryGetValue(variableSymbol, out var dependencies))
                    {
                        // recursively visit dependent variables
                        this.Visit(variableSymbol.DeclaringSyntax);

                        dependencies = resourceDependencies[variableSymbol];
                    }

                    foreach (var dependency in dependencies)
                    {
                        resourceDependencies[currentDeclaration].Add(dependency);
                    }

                    return;

                case ResourceSymbol resourceSymbol:
                    resourceDependencies[currentDeclaration].Add(new ResourceDependency(resourceSymbol, GetIndexExpression(syntax, resourceSymbol.IsCollection)));
                    return;

                case ModuleSymbol moduleSymbol:
                    resourceDependencies[currentDeclaration].Add(new ResourceDependency(moduleSymbol, GetIndexExpression(syntax, moduleSymbol.IsCollection)));
                    return;
            }
        }

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            if (currentDeclaration is null)
            {
                return;
            }

            switch (model.GetSymbolInfo(syntax))
            {
                case ResourceSymbol resourceSymbol:
                    resourceDependencies[currentDeclaration].Add(new ResourceDependency(resourceSymbol, GetIndexExpression(syntax, resourceSymbol.IsCollection)));
                    return;

                case ModuleSymbol moduleSymbol:
                    resourceDependencies[currentDeclaration].Add(new ResourceDependency(moduleSymbol, GetIndexExpression(syntax, moduleSymbol.IsCollection)));
                    return;
            }
        }

            
        private SyntaxBase? GetIndexExpression(SyntaxBase syntax, bool isCollection)
        {
            SyntaxBase? candidateIndexExpression = isCollection && this.model.Binder.GetParent(syntax) is ArrayAccessSyntax arrayAccess && ReferenceEquals(arrayAccess.BaseExpression, syntax)
                ? arrayAccess.IndexExpression
                : null;

            if(candidateIndexExpression is null)
            {
                // there is no index expression
                // depend on the entire collection instead
                return null;
            }

            // the index expression we just obtained could be in the scope of a property loop
            // when dependsOn properties are generated, this would mean that a local would be taken outside of its expected scope
            // which would result in runtime errors
            // to avoid this we will run data flow analysis to determine if such locals are present in the index expression
            var dfa = new DataFlowAnalyzer(this.model);

            var context = this.currentDeclaration switch
            {
                ResourceSymbol resourceSymbol => resourceSymbol.DeclaringResource.GetBody(),
                ModuleSymbol moduleSymbol => moduleSymbol.DeclaringModule.GetBody(),
                VariableSymbol variableSymbol => variableSymbol.DeclaringVariable.Value,
                _ => throw new NotImplementedException($"Unexpected current declaration type '{this.currentDeclaration?.GetType().Name}'.")
            };
            
            // using the resource/module body as the context to allow indexed depdnencies relying on the resource/module loop index to work as expected
            var inaccessibleLocals = dfa.GetInaccessibleLocalsAfterSyntaxMove(candidateIndexExpression, context);
            if(inaccessibleLocals.Any())
            {
                // some local will become inaccessible
                // depend on the entire collection instead
                return null;
            }

            return candidateIndexExpression;
        }
    }
}