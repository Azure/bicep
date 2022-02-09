// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.DataFlow;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    public class ResourceDependencyVisitor : SyntaxVisitor
    {
        private readonly SemanticModel model;
        private Options? options;
        private readonly IDictionary<DeclaredSymbol, HashSet<ResourceDependency>> resourceDependencies;
        private DeclaredSymbol? currentDeclaration;

        public struct Options
        {
            // If true, only inferred dependencies will be returned, not those declared explicitly by dependsOn entries
            public bool? IgnoreExplicitDependsOn;
        }

        /// <summary>
        /// Determines resource dependencies between all resources, returning it as a map of resource -> dependencies.
        /// Consider usage in expressions, parent/child relationships and (by default) dependsOn entries
        /// </summary>
        /// <returns></returns>
        public static ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> GetResourceDependencies(SemanticModel model, Options? options = null)
        {
            var visitor = new ResourceDependencyVisitor(model, options);
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

        private ResourceDependencyVisitor(SemanticModel model, Options? options)
        {
            this.model = model;
            this.options = options;
            this.resourceDependencies = new Dictionary<DeclaredSymbol, HashSet<ResourceDependency>>();
            this.currentDeclaration = null;
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            if (model.ResourceMetadata.TryLookup(syntax) is not DeclaredResourceMetadata resource)
            {
                // When invoked by BicepDeploymentGraphHandler, it's possible that the declaration is unbound.
                return;
            }

            // Resource ancestors are always dependencies.
            var ancestors = this.model.ResourceAncestors.GetAncestors(resource);

            // save previous declaration as we may call this recursively
            var prevDeclaration = this.currentDeclaration;

            this.currentDeclaration = resource.Symbol;
            this.resourceDependencies[resource.Symbol] = new HashSet<ResourceDependency>(ancestors.Select(a => new ResourceDependency(a.Resource.Symbol, a.IndexExpression)));
            base.VisitResourceDeclarationSyntax(syntax);

            // restore previous declaration
            this.currentDeclaration = prevDeclaration;
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            if (this.model.GetSymbolInfo(syntax) is not ModuleSymbol moduleSymbol)
            {
                return;
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
            if (this.model.GetSymbolInfo(syntax) is not VariableSymbol variableSymbol)
            {
                return;
            }

            // save previous declaration as we may call this recursively
            var prevDeclaration = this.currentDeclaration;

            this.currentDeclaration = variableSymbol;
            this.resourceDependencies[variableSymbol] = new HashSet<ResourceDependency>();
            base.VisitVariableDeclarationSyntax(syntax);

            // restore previous declaration
            this.currentDeclaration = prevDeclaration;
        }

        private IEnumerable<ResourceDependency> GetResourceDependencies(DeclaredSymbol declaredSymbol)
        {
            if (!resourceDependencies.TryGetValue(declaredSymbol, out var dependencies))
            {
                // recursively visit dependent variables
                this.Visit(declaredSymbol.DeclaringSyntax);

                if (!resourceDependencies.TryGetValue(declaredSymbol, out dependencies))
                {
                    return Enumerable.Empty<ResourceDependency>();
                }
            }

            return dependencies;
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (currentDeclaration is null)
            {
                return;
            }

            if (!this.resourceDependencies.TryGetValue(currentDeclaration, out HashSet<ResourceDependency>? currentResourceDependencies))
            {
                Debug.Fail("currentDeclaration should be guaranteed to be contained in this.resourceDependencies in VisitResourceDeclarationSyntax");
                return;
            }

            switch (model.GetSymbolInfo(syntax))
            {
                case VariableSymbol variableSymbol:
                    var varDependencies = GetResourceDependencies(variableSymbol);

                    currentResourceDependencies.UnionWith(varDependencies);
                    return;

                case ResourceSymbol resourceSymbol:
                    if (resourceSymbol.DeclaringResource.IsExistingResource())
                    {
                        var existingDependencies = GetResourceDependencies(resourceSymbol);

                        currentResourceDependencies.UnionWith(existingDependencies);
                        return;
                    }

                    currentResourceDependencies.Add(new ResourceDependency(resourceSymbol, GetIndexExpression(syntax, resourceSymbol.IsCollection)));
                    return;

                case ModuleSymbol moduleSymbol:
                    currentResourceDependencies.Add(new ResourceDependency(moduleSymbol, GetIndexExpression(syntax, moduleSymbol.IsCollection)));
                    return;
            }
        }

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            if (currentDeclaration is null)
            {
                return;
            }

            if (!this.resourceDependencies.TryGetValue(currentDeclaration, out HashSet<ResourceDependency>? currentResourceDependencies))
            {
                Debug.Fail("currentDeclaration should be guaranteed to be in this.resourceDependencies in VisitResourceDeclarationSyntax");
                return;
            }

            switch (model.GetSymbolInfo(syntax))
            {
                case ResourceSymbol resourceSymbol:
                    if (resourceSymbol.DeclaringResource.IsExistingResource())
                    {
                        var existingDependencies = GetResourceDependencies(resourceSymbol);

                        currentResourceDependencies.UnionWith(existingDependencies);
                        return;
                    }

                    currentResourceDependencies.Add(new ResourceDependency(resourceSymbol, GetIndexExpression(syntax, resourceSymbol.IsCollection)));
                    return;

                case ModuleSymbol moduleSymbol:
                    currentResourceDependencies.Add(new ResourceDependency(moduleSymbol, GetIndexExpression(syntax, moduleSymbol.IsCollection)));
                    return;
            }
        }

        private SyntaxBase? GetIndexExpression(SyntaxBase syntax, bool isCollection)
        {
            SyntaxBase? candidateIndexExpression = isCollection && this.model.Binder.GetParent(syntax) is ArrayAccessSyntax arrayAccess && ReferenceEquals(arrayAccess.BaseExpression, syntax)
                ? arrayAccess.IndexExpression
                : null;

            if (candidateIndexExpression is null)
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
            if (inaccessibleLocals.Any())
            {
                // some local will become inaccessible
                // depend on the entire collection instead
                return null;
            }

            return candidateIndexExpression;
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax propertySyntax)
        {
            if (options?.IgnoreExplicitDependsOn == true)
            {
                // Is it a property named "dependsOn"?
                if (propertySyntax.Key is IdentifierSyntax key && key.NameEquals(LanguageConstants.ResourceDependsOnPropertyName))
                {
                    // ... that is the a top-level resource or module property?
                    if (this.IsTopLevelPropertyOfCurrentDeclaration(propertySyntax))
                    {
                        // Yes - don't include dependencies from this property value
                        return;
                    }
                }
            }

            base.VisitObjectPropertySyntax(propertySyntax);
        }

        private bool IsTopLevelPropertyOfCurrentDeclaration(ObjectPropertySyntax propertySyntax)
        {
            SyntaxBase? declaringSyntax = this.currentDeclaration switch
            {
                ResourceSymbol resourceSymbol => (resourceSymbol.DeclaringSyntax as ResourceDeclarationSyntax)?.TryGetBody(),
                ModuleSymbol moduleSymbol => (moduleSymbol.DeclaringSyntax as ModuleDeclarationSyntax)?.TryGetBody(),
                _ => null
            };
            IEnumerable<ObjectPropertySyntax>? currentDeclarationProperties = (declaringSyntax as ObjectSyntax)?.Properties;

            return currentDeclarationProperties?.Contains(propertySyntax) ?? false;
        }
    }
}
