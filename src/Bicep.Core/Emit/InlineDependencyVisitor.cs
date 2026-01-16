// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Emit
{
    public class InlineDependencyVisitor : AstVisitor
    {
        private enum Decision
        {
            NotInline,
            Inline,
            SkipInline //decision to not inline, however it might be overridden if outer syntax requires inlining
        }
        private readonly SemanticModel model;
        private readonly IDictionary<DeclaredSymbol, Decision> shouldInlineCache;

        private DeclaredSymbol? currentDeclaration;

        // the following variables are only used when processing a single variable
        private readonly VariableDeclarationSyntax? targetVariable;
        private ImmutableStack<string>? currentStack;
        private ImmutableStack<string>? capturedSequence;

        private InlineDependencyVisitor(SemanticModel model, VariableDeclarationSyntax? targetVariable)
        {
            this.model = model;
            this.shouldInlineCache = new Dictionary<DeclaredSymbol, Decision>();
            this.targetVariable = targetVariable;
            this.currentDeclaration = null;

            if (targetVariable is not null)
            {
                // the functionality
                this.currentStack = [];
                this.capturedSequence = null;
            }
        }

        public record SymbolsToInline(
            IReadOnlySet<VariableSymbol> VariablesToInline,
            IReadOnlySet<ParameterAssignmentSymbol> ParameterAssignmentsToInline,
            IReadOnlySet<ResourceSymbol> ExistingResourcesToInline);

        /// <summary>
        /// Gets a set of variables that must be inlined due to runtime limitations.
        /// </summary>
        /// <param name="model">The semantic model</param>
        public static SymbolsToInline GetSymbolsToInline(SemanticModel model)
        {
            var visitor = new InlineDependencyVisitor(model, null);
            visitor.VisitNodes(model.Root.VariableDeclarations.Select(d => d.DeclaringSyntax)
                .Concat(model.Root.ParameterAssignments.Select(d => d.DeclaringSyntax))
                .Concat(model.Root.ResourceDeclarations.Select(d => d.DeclaringSyntax)));

            List<VariableSymbol> variablesToInline = new();
            List<ParameterAssignmentSymbol> parameterAssignmentsToInline = new();
            List<ResourceSymbol> existingResourcesToInline = new();

            foreach (var kvp in visitor.shouldInlineCache.Where(kvp => kvp.Value == Decision.Inline))
            {
                switch (kvp.Key)
                {
                    case VariableSymbol variable:
                        variablesToInline.Add(variable);
                        break;
                    case ParameterAssignmentSymbol parameterAssignment:
                        parameterAssignmentsToInline.Add(parameterAssignment);
                        break;
                    case ResourceSymbol resource when resource.DeclaringResource.IsExistingResource():
                        existingResourcesToInline.Add(resource);
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Expected shouldInlineCache to only contain variables, parameter assignments, and existing resources but found {kvp.Key.Type}");
                }
            }

            return new(variablesToInline.ToFrozenSet(), parameterAssignmentsToInline.ToFrozenSet(), existingResourcesToInline.ToFrozenSet());
        }

        /// <summary>
        /// Checks if the specified variable needs to be inlined due to runtime limitations. In cases where the inlining is caused by accessing a variable that must be inlined,
        /// the variable access chain is returned. Otherwise, an empty chain is returned.
        /// </summary>
        /// <param name="model">The semantic model</param>
        /// <param name="variable">The variable to check</param>
        /// <param name="variableAccessChain">The variable access chain that leads to inlining or empty if not available.</param>
        public static bool ShouldInlineVariable(SemanticModel model, VariableDeclarationSyntax variable, out ImmutableArray<string> variableAccessChain)
        {
            variableAccessChain = [];
            if (model.GetSymbolInfo(variable) is not VariableSymbol variableSymbol)
            {
                // we have errors - assume this is not meant to be inlined
                return false;
            }

            var visitor = new InlineDependencyVisitor(model, variable);
            visitor.Visit(variable);

            if (!visitor.shouldInlineCache.TryGetValue(variableSymbol, out var shouldInline) || shouldInline != Decision.Inline)
            {
                return false;
            }

            variableAccessChain = visitor.capturedSequence?.Reverse().ToImmutableArray() ?? [];
            return true;
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            if (this.model.GetSymbolInfo(syntax) is not VariableSymbol variableSymbol)
            {
                // we have errors that prevent further validation
                // skip this part of the tree
                return;
            }

            if (shouldInlineCache.ContainsKey(variableSymbol))
            {
                // we've already analyzed this variable
                return;
            }

            // save previous declaration as we may call this recursively
            var prevDeclaration = this.currentDeclaration;

            this.currentDeclaration = variableSymbol;
            this.shouldInlineCache[variableSymbol] = Decision.NotInline;
            base.VisitVariableDeclarationSyntax(syntax);

            // restore previous declaration
            this.currentDeclaration = prevDeclaration;
        }

        public override void VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax)
        {
            if (this.model.GetSymbolInfo(syntax) is not ParameterAssignmentSymbol parameterSymbol)
            {
                // we have errors that prevent further validation
                // skip this part of the tree
                return;
            }

            if (shouldInlineCache.ContainsKey(parameterSymbol))
            {
                // we've already analyzed this parameter
                return;
            }

            // save previous declaration as we may call this recursively
            var prevDeclaration = this.currentDeclaration;

            this.currentDeclaration = parameterSymbol;
            this.shouldInlineCache[parameterSymbol] = Decision.NotInline;
            base.VisitParameterAssignmentSyntax(syntax);

            // restore previous declaration
            this.currentDeclaration = prevDeclaration;
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            if (model.ResourceMetadata.TryLookup(syntax) is not DeclaredResourceMetadata resourceMetadata ||
                model.GetSymbolInfo(syntax) is not ResourceSymbol resourceSymbol ||
                shouldInlineCache.ContainsKey(resourceSymbol))
            {
                // if:
                //   * the resource is not an `existing` resource, or
                //   * errors prevent us from recognizing the statement as declaring a resource,
                //   * errors prevent us from recognizing the statement as declaring a resource symbol, or
                //   * we have already analyzed this symbol,
                // then skip this part of the tree
                return;
            }

            if (!syntax.IsExistingResource() ||
                resourceSymbol.TryGetResourceType()?.IsAzResource() is not true)
            {
                // deployed and extensibility resources can never be inlined; only existing az resources can
                shouldInlineCache[resourceSymbol] = Decision.SkipInline;
            }
            else
            {
                // here we know that the resource is an existing resource that can also be targeted directly by
                // resource ID. We will need to "inline" (**not** emit an `"existing": true` resource in the ARM JSON)
                // any resource whose `name` property contains runtime expressions or is scoped to- or a child of an
                // inlined resource.
                var hasInlinedAncestor = false;
                foreach (var ancestor in model.ResourceAncestors.GetAncestors(resourceMetadata))
                {
                    if (ancestor.Resource.IsExistingResource)
                    {
                        this.Visit(ancestor.Resource.Symbol.DeclaringSyntax);
                        hasInlinedAncestor |= shouldInlineCache[ancestor.Resource.Symbol] == Decision.Inline;
                    }
                    else
                    {
                        hasInlinedAncestor = false;
                    }
                }

                var scopedToInlinedResource = false;
                if (model.ResourceScopeData.TryGetValue(resourceMetadata, out var resourceScope) &&
                    resourceScope.ResourceScope is { } scopedToResource)
                {
                    this.Visit(scopedToResource.Symbol.DeclaringSyntax);
                    scopedToInlinedResource = shouldInlineCache[scopedToResource.Symbol] == Decision.Inline;
                }

                if (hasInlinedAncestor || scopedToInlinedResource)
                {
                    shouldInlineCache[resourceSymbol] = Decision.Inline;
                }
                else
                {
                    var prevDeclaration = currentDeclaration;
                    currentDeclaration = resourceSymbol;
                    shouldInlineCache[resourceSymbol] = Decision.NotInline;
                    foreach (var property in (syntax.Value as ObjectSyntax)?.Properties ?? [])
                    {
                        switch (property.TryGetKeyText())
                        {
                            // dependsOn has a different check performed in EmitLimitationCalculator
                            case LanguageConstants.ResourceDependsOnPropertyName:
                            // `parent` and `scope` were checked above
                            case LanguageConstants.ResourceParentPropertyName:
                            case LanguageConstants.ResourceScopePropertyName:
                                continue;
                            default:
                                this.Visit(property);
                                break;
                        }
                    }

                    currentDeclaration = prevDeclaration;
                }
            }

            foreach (var child in (syntax.Value as ObjectSyntax)?.Resources ?? [])
            {
                VisitResourceDeclarationSyntax(child);
            }
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            var functionSymbol = model.GetSymbolInfo(syntax) as FunctionSymbol;
            VisitFunctionCallSyntaxBaseInternal(functionSymbol, syntax);

            if (ShouldVisitFunctionArguments(functionSymbol))
            {
                base.VisitFunctionCallSyntax(syntax);
            }
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            var functionSymbol = model.GetSymbolInfo(syntax) as FunctionSymbol;
            VisitFunctionCallSyntaxBaseInternal(functionSymbol, syntax);

            if (ShouldVisitFunctionArguments(functionSymbol))
            {
                base.VisitInstanceFunctionCallSyntax(syntax);
            }
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (currentDeclaration == null)
            {
                return;
            }

            if (shouldInlineCache[currentDeclaration] == Decision.Inline)
            {
                // we've already made a decision to inline
                return;
            }

            void ProcessInlinableSymbol(DeclaredSymbol symbol, string identifierName)
            {
                var previousStack = this.currentStack;
                if (!shouldInlineCache.TryGetValue(symbol, out var shouldInline))
                {
                    this.currentStack = this.currentStack?.Push(identifierName);

                    // recursively visit dependent symbols
                    this.Visit(symbol.DeclaringSyntax);

                    shouldInline = shouldInlineCache[symbol];

                    if (shouldInline == Decision.Inline && this.targetVariable is not null && this.capturedSequence is null)
                    {
                        // this point is where the decision is made to inline the variable
                        // the variable access stack will be the deepest here
                        // (once captured, we will not reset because the visitor will be short-circuiting and
                        //  unrolling the recursion which would produce shorter and inaccurate paths)

                        // capture the sequence of variable accesses
                        this.capturedSequence = this.currentStack;
                    }
                }

                // if we depend on a symbol that requires inlining, then we also require inlining
                var newValue = shouldInlineCache[currentDeclaration] == Decision.Inline || shouldInline == Decision.Inline;
                SetInlineCache(newValue);

                this.currentStack = previousStack;
            }

            switch (model.GetSymbolInfo(syntax))
            {
                case VariableSymbol variableSymbol:
                    ProcessInlinableSymbol(variableSymbol, syntax.Name.IdentifierName);
                    return;

                case ParameterAssignmentSymbol parameterAssignmentSymbol:
                    ProcessInlinableSymbol(parameterAssignmentSymbol, syntax.Name.IdentifierName);
                    return;

                case ResourceSymbol:
                case ModuleSymbol:
                case ParameterSymbol parameterSymbol when model.ResourceMetadata.TryLookup(syntax) is ResourceMetadata:
                    if (this.currentDeclaration is not null && shouldInlineCache[currentDeclaration] != Decision.SkipInline)
                    {
                        //inline only if declaration wasn't explicitly excluded from inlining, to avoid inlining usages which are permitted
                        SetInlineCache(true);
                    }
                    return;
            }
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            // This solution works on the assumption that all deploy-time constants are top-level properties on
            // resources and modules (id, name, type, apiVersion). Once https://github.com/Azure/bicep/issues/1177 is fixed,
            // it should be possible to make this more robust to handle nested deploy-time constants.

            if (currentDeclaration == null)
            {
                return;
            }

            if (shouldInlineCache[currentDeclaration] == Decision.Inline)
            {
                // we've already made a decision to inline
                return;
            }

            bool ShouldSkipInlining(ObjectType objectType, string propertyName, ResourceSymbol? resourceSymbol = null)
            {
                if (!objectType.Properties.TryGetValue(propertyName, out var propertyType))
                {
                    // unknown property - we should assume it's not inlinable
                    return true;
                }

                if (propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                {
                    if (resourceSymbol is not null && resourceSymbol.TryGetResourceType()?.IsAzResource() is true)
                    {
                        switch (propertyName)
                        {
                            case AzResourceTypeProvider.ResourceIdPropertyName:
                            case AzResourceTypeProvider.ResourceNamePropertyName:
                                this.Visit(resourceSymbol.DeclaringSyntax);
                                return shouldInlineCache[resourceSymbol] != Decision.Inline;
                            default:
                                return AzResourceTypeProvider.ReadWriteDeployTimeConstantPropertyNames.Contains(propertyName, LanguageConstants.IdentifierComparer);
                        }
                    }

                    return true;
                }

                return false;
            }

            switch (this.TryResolveSymbol(SyntaxHelper.UnwrapNonNullAssertion(syntax.BaseExpression)))
            {
                // Note - there's a limitation here that we're using the 'declared' type and not the 'assigned' type.
                // This means that we may encounter a DiscriminatedObjectType. For now we should accept this limitation,
                // and move to using the assigned type once https://github.com/Azure/bicep/issues/1177 is fixed.
                case ResourceSymbol resourceSymbol when resourceSymbol.TryGetBodyObjectType() is { } bodyObjectType:
                    SetSkipInlineCache(ShouldSkipInlining(bodyObjectType, syntax.PropertyName.IdentifierName, resourceSymbol));
                    return;
                case ModuleSymbol moduleSymbol when moduleSymbol.TryGetBodyObjectType() is { } bodyObjectType:
                    SetSkipInlineCache(ShouldSkipInlining(bodyObjectType, syntax.PropertyName.IdentifierName));
                    return;
                case ParameterSymbol parameterSymbol when parameterSymbol.TryGetBodyObjectType() is { } bodyObjectType:
                    SetSkipInlineCache(ShouldSkipInlining(bodyObjectType, syntax.PropertyName.IdentifierName));
                    return;
                default:
                    base.VisitPropertyAccessSyntax(syntax);
                    return;
            }
        }

        private Symbol? TryResolveSymbol(SyntaxBase syntax) => syntax switch
        {
            ArrayAccessSyntax { BaseExpression: var baseSyntax } => TryResolveSymbol(baseSyntax),
            _ => this.model.GetSymbolInfo(syntax)
        };

        private void VisitFunctionCallSyntaxBaseInternal(FunctionSymbol? functionSymbol, FunctionCallSyntaxBase syntax)
        {
            if (currentDeclaration == null)
            {
                return;
            }

            if (shouldInlineCache[currentDeclaration] == Decision.Inline)
            {
                // we've already made a decision to inline
                return;
            }

            if (functionSymbol is { })
            {
                var shouldInline = functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining) ||
                                   functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresExternalInput);
                SetInlineCache(shouldInline);
            }
        }

        private void SetInlineCache(bool shouldInline)
        {
            if (this.currentDeclaration is not null)
            {
                this.shouldInlineCache[this.currentDeclaration] = shouldInline ? Decision.Inline : Decision.NotInline;
            }
        }
        private void SetSkipInlineCache(bool shouldNotInline)
        {
            if (this.currentDeclaration is not null)
            {
                this.shouldInlineCache[this.currentDeclaration] = shouldNotInline ? Decision.SkipInline : Decision.Inline;
            }
        }

        private static bool ShouldVisitFunctionArguments(FunctionSymbol? functionSymbol)
            => functionSymbol is null || !functionSymbol.FunctionFlags.HasFlag(FunctionFlags.IsArgumentValueIndependent);
    }
}
