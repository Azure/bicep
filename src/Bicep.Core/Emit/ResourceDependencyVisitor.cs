// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.DataFlow;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Emit
{
    public class ResourceDependencyVisitor : AstVisitor
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

            Queue<VariableSymbol> nonInlinedVariables = new(visitor.resourceDependencies
                .Where(kvp => kvp.Value.Count == 0)
                .Select(kvp => kvp.Key)
                .OfType<VariableSymbol>());

            while (nonInlinedVariables.TryDequeue(out var nonInlinedVariable))
            {
                foreach (var kvp in visitor.resourceDependencies)
                {
                    if (kvp.Value.RemoveWhere(d => ReferenceEquals(nonInlinedVariable, d.Resource)) > 0 &&
                        kvp.Value.Count == 0 &&
                        kvp.Key is VariableSymbol variable)
                    {
                        nonInlinedVariables.Enqueue(variable);
                    }
                }
            }

            return visitor.resourceDependencies.ToImmutableDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToImmutableHashSet());
        }

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

            HashSet<ResourceDependency> dependencies = new();
            if (model.ResourceAncestors.GetAncestors(resource).LastOrDefault() is { } parent)
            {
                // Resource ancestors are always weak references.
                dependencies.Add(new(parent.Resource.Symbol, parent.IndexExpression, WeakReference: true));
            }
            resourceDependencies[resource.Symbol] = dependencies;

            // save previous declaration as we may call this recursively
            var prevDeclaration = this.currentDeclaration;
            this.currentDeclaration = resource.Symbol;

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
                    currentResourceDependencies.Add(new(variableSymbol, GetIndexExpression(syntax, variableSymbol.IsCopyVariable)));
                    return;

                case ResourceSymbol resourceSymbol:
                    currentResourceDependencies.Add(new(resourceSymbol, GetIndexExpression(syntax, resourceSymbol.IsCollection), IsWeakReference(syntax, resourceSymbol)));
                    return;

                case ModuleSymbol moduleSymbol:
                    currentResourceDependencies.Add(new(moduleSymbol, GetIndexExpression(syntax, moduleSymbol.IsCollection)));
                    return;
            }
        }

        /// <summary>
        /// Determines whether a reference to a resource is weak.
        /// </summary>
        /// <remarks>
        /// A reference is "weak" if it will not read from the body of a resource. For ARM resources, that means that
        /// the reference is only used to read one of the referent's identifiers (id, name, type, apiVersion) or to
        /// call a function. (<code>list*()</code> functions will generate an implicit dependency in the ARM engine,
        /// but any functions that are resolved at compile time (such as <code>keyVault.getSecret()</code>) will not
        /// need to access the resource's body.
        /// </remarks>
        /// <param name="syntax">The referencing syntax.</param>
        /// <param name="resourceSymbol">The referenced resource.</param>
        private bool IsWeakReference(SyntaxBase syntax, ResourceSymbol resourceSymbol)
            => resourceSymbol.TryGetResourceType()?.IsAzResource() is true && (
                IsResourceInfoAccessBase(syntax, resourceSymbol) ||
                IsResourceFunctionCallBase(syntax) ||
                IsWithinExplicitParentDeclaration(syntax) ||
                IsWithinScopeDeclaration(syntax));

        private bool IsResourceInfoAccessBase(SyntaxBase syntax, ResourceSymbol resource)
            => model.Binder.GetParent(syntax) switch
            {
                NonNullAssertionSyntax nonNullAssertion => IsResourceInfoAccessBase(nonNullAssertion, resource),
                PropertyAccessSyntax propertyAccess
                    => IsResourceInfoAccessBase(resource, propertyAccess.PropertyName.IdentifierName),
                ArrayAccessSyntax arrayAccess => model.GetTypeInfo(arrayAccess.IndexExpression) switch
                {
                    // array access can be used to dereference properties, e.g., resourceSymbolicRef['id']
                    StringLiteralType indexStr => IsResourceInfoAccessBase(resource, indexStr.RawStringValue),
                    // it can also dereference a member of a resource collection, e.g., resourceSymbolicRef[0].id
                    var t when resource.IsCollection && TypeValidator.AreTypesAssignable(t, LanguageConstants.Int)
                        => IsResourceInfoAccessBase(arrayAccess, resource),
                    _ => false,
                },
                _ => false,
            };

        private static bool IsResourceInfoAccessBase(ResourceSymbol resource, string propertyName)
            // if specific top-level properties of an ARM resource are accessed, the compiler will migrate syntax from
            // the resource declaration or emit a `resourceInfo()` function
            => EmitConstants.ResourceInfoProperties.Contains(propertyName);

        private bool IsResourceFunctionCallBase(SyntaxBase syntax) => model.Binder.GetParent(syntax) switch
        {
            NonNullAssertionSyntax nonNullAssertion => IsResourceFunctionCallBase(nonNullAssertion),
            InstanceFunctionCallSyntax => true,
            ArrayAccessSyntax arrayAccess when model.GetSymbolInfo(arrayAccess.BaseExpression) is ResourceSymbol r &&
                r.IsCollection &&
                TypeValidator.AreTypesAssignable(model.GetTypeInfo(arrayAccess.IndexExpression), LanguageConstants.Int)
                => IsResourceFunctionCallBase(arrayAccess),
            _ => false,
        };

        private bool IsWithinExplicitParentDeclaration(SyntaxBase syntax)
            => TryGetCurrentDeclarationTopLevelProperty(LanguageConstants.ResourceParentPropertyName) is { } nonNull &&
                model.Binder.IsDescendant(syntax, nonNull);

        private bool IsWithinScopeDeclaration(SyntaxBase syntax)
            => TryGetCurrentDeclarationTopLevelProperty(LanguageConstants.ResourceScopePropertyName) is { } nonNull &&
                model.Binder.IsDescendant(syntax, nonNull);

        private ObjectPropertySyntax? TryGetCurrentDeclarationTopLevelProperty(string propertyName)
        {
            ObjectSyntax? declaringSyntax = this.currentDeclaration switch
            {
                ResourceSymbol resourceSymbol => (resourceSymbol.DeclaringSyntax as ResourceDeclarationSyntax)?.TryGetBody(),
                ModuleSymbol moduleSymbol => (moduleSymbol.DeclaringSyntax as ModuleDeclarationSyntax)?.TryGetBody(),
                _ => null
            };

            return declaringSyntax?.TryGetPropertyByName(propertyName);
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
                    currentResourceDependencies.Add(new(resourceSymbol, GetIndexExpression(syntax, resourceSymbol.IsCollection), IsWeakReference(syntax, resourceSymbol)));
                    return;

                case ModuleSymbol moduleSymbol:
                    currentResourceDependencies.Add(new(moduleSymbol, GetIndexExpression(syntax, moduleSymbol.IsCollection)));
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
                VariableSymbol variableSymbol => variableSymbol.DeclaringVariable.GetBody(),
                _ => throw new NotImplementedException($"Unexpected current declaration type '{this.currentDeclaration?.GetType().Name}'.")
            };

            // using the resource/module body as the context to allow indexed dependencies relying on the resource/module loop index to work as expected
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
                    if (ReferenceEquals(TryGetCurrentDeclarationTopLevelProperty(key.IdentifierName), propertySyntax))
                    {
                        // Yes - don't include dependencies from this property value
                        return;
                    }
                }
            }

            base.VisitObjectPropertySyntax(propertySyntax);
        }
    }
}
