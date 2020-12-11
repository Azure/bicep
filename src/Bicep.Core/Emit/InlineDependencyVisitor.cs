// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
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
        private readonly SemanticModel model;
        private IDictionary<VariableSymbol, bool> shouldInlineCache;
        private VariableSymbol? currentDeclaration;

        public static ImmutableHashSet<VariableSymbol> GetVariablesToInline(SemanticModel model)
        {
            var visitor = new InlineDependencyVisitor(model);
            visitor.Visit(model.Root.Syntax);

            return visitor.shouldInlineCache
                .Where(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToImmutableHashSet();
        }

        private InlineDependencyVisitor(SemanticModel model)
        {
            this.model = model;
            this.shouldInlineCache = new Dictionary<VariableSymbol, bool>();
            this.currentDeclaration = null;
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
            this.shouldInlineCache[variableSymbol] = false;
            base.VisitVariableDeclarationSyntax(syntax);

            // restore previous declaration
            this.currentDeclaration = prevDeclaration;
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            VisitFunctionCallSyntaxInternal(syntax);
            base.VisitFunctionCallSyntax(syntax);
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

            if (shouldInlineCache[currentDeclaration])
            {
                // we've already made a decision
                return;
            }

            switch (model.GetSymbolInfo(syntax))
            {
                case VariableSymbol variableSymbol:
                    if (!shouldInlineCache.TryGetValue(variableSymbol, out var shouldInline))
                    {
                        // recursively visit dependent variables
                        this.Visit(variableSymbol.DeclaringSyntax);

                        shouldInline = shouldInlineCache[variableSymbol];
                    }

                    // if we depend on a variable that requires inlining, then we also require inlining
                    shouldInlineCache[currentDeclaration] |= shouldInline;
                    return;
            }
        }

        private void VisitPropertyAccessSyntaxInternal(PropertyAccessSyntax syntax)
        {
            if (currentDeclaration == null)
            {
                return;
            }

            if (!(syntax.BaseExpression is VariableAccessSyntax variableAccessSyntax))
            {
                // we only care about variable access
                return;
            }

            if (shouldInlineCache[currentDeclaration])
            {
                // we've already made a decision
                return;
            }

            switch (model.GetSymbolInfo(variableAccessSyntax))
            {
                case ResourceSymbol resourceSymbol:
                    if (!(resourceSymbol.Type is ResourceType resourceType && resourceType.Body is ObjectType bodyObjectType))
                    {
                        // Resource or body could be an ErrorType here. We only want to attempt property access on an object body.
                        return;
                    }

                    var property = syntax.PropertyName.IdentifierName;
                    if (!bodyObjectType.Properties.TryGetValue(property, out var propertyType))
                    {
                        // unknown property
                        return;
                    }

                    // update the cache if property can't be skipped for inlining
                    shouldInlineCache[currentDeclaration] |= !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant);
                    return;
            }
        }

        private void VisitFunctionCallSyntaxInternal(FunctionCallSyntax syntax)
        {
            if (currentDeclaration == null)
            {
                return;
            }

            if (shouldInlineCache[currentDeclaration])
            {
                // we've already made a decision
                return;
            }

            switch (model.GetSymbolInfo(syntax))
            {
                case FunctionSymbol functionSymbol:
                    shouldInlineCache[currentDeclaration] |= functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining);
                    return;
            }
        }
    }
}