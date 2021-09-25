// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Emit
{
    public class InlineDependencyVisitor : SyntaxVisitor
    {
        private enum Decision
        {
            NotInline,
            Inline,
            SkipInline //decision to not inline, however it might be overriden if outer syntax requires inlining
        }
        private readonly SemanticModel model;
        private readonly IDictionary<VariableSymbol, Decision> shouldInlineCache;

        private VariableSymbol? currentDeclaration;

        // the following variables are only used when processing a single variable
        private readonly VariableDeclarationSyntax? targetVariable;
        private ImmutableStack<string>? currentStack;
        private ImmutableStack<string>? capturedSequence;

        private InlineDependencyVisitor(SemanticModel model, VariableDeclarationSyntax? targetVariable)
        {
            this.model = model;
            this.shouldInlineCache = new Dictionary<VariableSymbol, Decision>();
            this.targetVariable = targetVariable;
            this.currentDeclaration = null;

            if (targetVariable is not null)
            {
                // the functionality 
                this.currentStack = ImmutableStack.Create<string>();
                this.capturedSequence = null;
            }
        }

        /// <summary>
        /// Gets a set of variables that must be inlined due to runtime limitations.
        /// </summary>
        /// <param name="model">The semantic model</param>
        public static ImmutableHashSet<VariableSymbol> GetVariablesToInline(SemanticModel model)
        {
            var visitor = new InlineDependencyVisitor(model, null);
            visitor.Visit(model.Root.Syntax);

            return visitor.shouldInlineCache
                .Where(kvp => kvp.Value == Decision.Inline)
                .Select(kvp => kvp.Key)
                .ToImmutableHashSet();
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
            variableAccessChain = ImmutableArray<string>.Empty;
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

            variableAccessChain = visitor.capturedSequence?.Reverse().ToImmutableArray() ?? ImmutableArray<string>.Empty;
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

            // save previous declaration as we may call this recursively
            var prevDeclaration = this.currentDeclaration;

            this.currentDeclaration = variableSymbol;
            this.shouldInlineCache[variableSymbol] = Decision.NotInline;
            base.VisitVariableDeclarationSyntax(syntax);

            // restore previous declaration
            this.currentDeclaration = prevDeclaration;
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            VisitFunctionCallSyntaxBaseInternal(syntax);
            base.VisitFunctionCallSyntax(syntax);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            VisitFunctionCallSyntaxBaseInternal(syntax);
            base.VisitInstanceFunctionCallSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            VisitPropertyAccessSyntaxInternal(syntax);
            base.VisitPropertyAccessSyntax(syntax);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            VisitVariableAccessSyntaxInternal(syntax);
            base.VisitVariableAccessSyntax(syntax);
        }

        private void VisitVariableAccessSyntaxInternal(VariableAccessSyntax syntax)
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

            switch (model.GetSymbolInfo(syntax))
            {
                case VariableSymbol variableSymbol:
                    var previousStack = this.currentStack;
                    if (!shouldInlineCache.TryGetValue(variableSymbol, out var shouldInline))
                    {
                        this.currentStack = this.currentStack?.Push(syntax.Name.IdentifierName);

                        // recursively visit dependent variables
                        this.Visit(variableSymbol.DeclaringSyntax);

                        shouldInline = shouldInlineCache[variableSymbol];

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

                    // if we depend on a variable that requires inlining, then we also require inlining
                    var newValue = shouldInlineCache[currentDeclaration] == Decision.Inline || shouldInline == Decision.Inline;
                    SetInlineCache(newValue);

                    this.currentStack = previousStack;
                    return;

                case ResourceSymbol:
                case ModuleSymbol:
                    if (this.currentDeclaration is not null && shouldInlineCache[currentDeclaration] != Decision.SkipInline)
                    {
                        //inline only if declaration wasn't explicitly excluded from inlining, to avoid inlining usages which are permitted
                        SetInlineCache(true);
                    }
                    return;
            }
        }

        private void VisitPropertyAccessSyntaxInternal(PropertyAccessSyntax syntax)
        {
            // This solution works on the assumption that all deploy-time constants are top-level properties on
            // resources and modules (id, name, type, apiVersion). Once https://github.com/Azure/bicep/issues/1177 is fixed,
            // it should be possible to make this more robust to handle nested deploy-time constants.

            if (currentDeclaration == null)
            {
                return;
            }

            var variableAccessSyntax = syntax.BaseExpression switch
            {
                VariableAccessSyntax variableAccess => variableAccess,
                ArrayAccessSyntax { BaseExpression: VariableAccessSyntax variableAccess } => variableAccess,
                _ => null
            };

            if (variableAccessSyntax is null)
            {
                return;
            }

            if (shouldInlineCache[currentDeclaration] == Decision.Inline)
            {
                // we've already made a decision to inline
                return;
            }

            static bool ShouldSkipInlining(ObjectType objectType, string propertyName, ResourceSymbol? resourceSymbol = null)
            {
                if (!objectType.Properties.TryGetValue(propertyName, out var propertyType))
                {
                    // unknown property - we should assume it's not inlinable
                    return true;
                }

                if (propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                {
                    if (resourceSymbol is not null &&
                        !LanguageConstants.ReadWriteDeployTimeConstantPropertyNames.Contains(propertyName, LanguageConstants.IdentifierComparer))
                    {
                        // The property is not declared in the resource - we should inline event it is a deploy-time constant.
                        // We skip standardized properties (id, name, type, and apiVersion) since their values are always known
                        // and emitted if there are not syntactic and semantic errors (see ConvertResourcePropertyAccess in ExpressionConverter).
                        return false;
                    }

                    return true;
                }

                return false;
            }

            switch (model.GetSymbolInfo(variableAccessSyntax))
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
            }
        }

        private void VisitFunctionCallSyntaxBaseInternal(FunctionCallSyntaxBase syntax)
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

            switch (model.GetSymbolInfo(syntax))
            {
                case FunctionSymbol functionSymbol:
                    SetInlineCache(functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining));
                    return;
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
    }
}
