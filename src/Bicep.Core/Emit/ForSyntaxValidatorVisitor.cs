// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;

namespace Bicep.Core.Emit
{
    public sealed class ForSyntaxValidatorVisitor : SyntaxVisitor
    {
        // we don't support nesting of property loops right now
        private const int MaximumNestedPropertyLoopCount = 1;

        private readonly IDiagnosticWriter diagnosticWriter;
        private readonly SemanticModel semanticModel;

        private SyntaxBase? activeLoopCapableTopLevelDeclaration = null;

        private enum PropertyLoopCapability
        {
            /// <summary>
            /// Node is not derived from expression syntax so property loop capability depends on other nodes in the hierarchy.
            /// </summary>
            Inconclusive,

            /// <summary>
            /// Property loops are not allowed due to nodes that prevent such usage.
            /// </summary>
            DisallowedInExpression,

            /// <summary>
            /// Property loops may be allowed.
            /// </summary>
            PotentiallyAllowed
        }

        // this is used to block property for-expression usages in object properties that are children of operators or function calls
        // null - not in an expression
        // true - property loop allowed IF other rules are satisfied as well
        // false - property loop definitely NOT allowed
        private PropertyLoopCapability isPropertyLoopPotentiallyAllowed = PropertyLoopCapability.Inconclusive;

        private int propertyLoopCount = 0;

        // points to the top level dependsOn property in the resource/module declaration currently being processed
        private ObjectPropertySyntax? currentDependsOnProperty = null;

        private bool insideTopLevelDependsOn = false;

        private ForSyntaxValidatorVisitor(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            this.semanticModel = semanticModel;
            this.diagnosticWriter = diagnosticWriter;
        }

        public static void Validate(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            var visitor = new ForSyntaxValidatorVisitor(semanticModel, diagnosticWriter);

            // visiting writes diagnostics in some cases
            visitor.Visit(semanticModel.SourceFile.ProgramSyntax);
        }

        public static bool IsAddingPropertyLoopAllowed(SemanticModel semanticModel, ObjectPropertySyntax property)
        {
            SyntaxBase? current = property;
            int propertyLoopCount = 0;
            while(current is not null)
            {
                var parent = semanticModel.SourceFile.Hierarchy.GetParent(current);
                if (current is ForSyntax @for && IsPropertyLoop(parent, @for))
                {
                    ++propertyLoopCount;
                }

                current = parent;
            }

            // adding a new property loop is only allowed if we're under the limit
            return propertyLoopCount < MaximumNestedPropertyLoopCount;
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            // This check is separate from IsLoopAllowedHere because this is about the appearance of a
            // nested resource **inside** a loop.
            if (this.semanticModel.Binder.GetNearestAncestor<ForSyntax>(syntax) is ForSyntax)
            {
                this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax.Span).NestedResourceNotAllowedInLoop());
            }

            // Resources can be nested, support recursion of resource declarations
            var previousLoopCapableTopLevelDeclaration = this.activeLoopCapableTopLevelDeclaration;
            this.activeLoopCapableTopLevelDeclaration = syntax;

            // stash the body (handles loops and conditions as well)
            var previousDependsOnProperty = this.currentDependsOnProperty;
            this.currentDependsOnProperty = TryGetDependsOnProperty(syntax.TryGetBody());

            base.VisitResourceDeclarationSyntax(syntax);

            // restore state
            this.currentDependsOnProperty = previousDependsOnProperty;
            this.activeLoopCapableTopLevelDeclaration = previousLoopCapableTopLevelDeclaration;
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            this.activeLoopCapableTopLevelDeclaration = syntax;

            // stash the body (handles loops and conditions as well)
            this.currentDependsOnProperty = TryGetDependsOnProperty(syntax.TryGetBody());

            base.VisitModuleDeclarationSyntax(syntax);

            // clear the stash
            this.currentDependsOnProperty = null;

            this.activeLoopCapableTopLevelDeclaration = null;
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            this.activeLoopCapableTopLevelDeclaration = syntax;
            base.VisitVariableDeclarationSyntax(syntax);
            this.activeLoopCapableTopLevelDeclaration = null;
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            this.activeLoopCapableTopLevelDeclaration = syntax;
            base.VisitOutputDeclarationSyntax(syntax);
            this.activeLoopCapableTopLevelDeclaration = null;
        }

        public override void VisitForSyntax(ForSyntax syntax)
        {
            // save previous property loop count on the call stack
            var previousPropertyLoopCount = this.propertyLoopCount;

            switch(this.IsLoopAllowedHere(syntax))
            {
                case false:
                    // this loop was used incorrectly
                    this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax.ForKeyword).ForExpressionsNotSupportedHere());
                    break;

                case null:
                    // this is a property loop
                    this.propertyLoopCount += 1;

                    if(this.propertyLoopCount > MaximumNestedPropertyLoopCount)
                    {
                        // too many property loops
                        this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax.ForKeyword).TooManyPropertyForExpressions());
                    }

                    break;
            }
            
            // visit children
            base.VisitForSyntax(syntax);

            // restore previous property loop count
            this.propertyLoopCount = previousPropertyLoopCount;
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            if (syntax.TryGetKeyText() == null && syntax.Value is ForSyntax)
            {
                // block loop usage with properties whose names are expressions
                this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax.Key).ExpressionedPropertiesNotAllowedWithLoops());
            }

            bool insideDependsOnInThisScope = ReferenceEquals(this.currentDependsOnProperty, syntax);

            // set this to true if the current property is the top-level dependsOn property
            // leave it true if already set to true
            this.insideTopLevelDependsOn = this.insideTopLevelDependsOn || insideDependsOnInThisScope;

            // visit children
            base.VisitObjectPropertySyntax(syntax);

            // clear the flag after we leave the dependsOn property
            if (insideDependsOnInThisScope)
            {
                this.insideTopLevelDependsOn = false;
            }
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            this.ValidateDirectAccessToResourceOrModuleCollection(syntax);

            // visit children
            base.VisitVariableAccessSyntax(syntax);
        }

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            this.ValidateDirectAccessToResourceOrModuleCollection(syntax);

            // visit children
            base.VisitResourceAccessSyntax(syntax);
        }

        protected override void VisitInternal(SyntaxBase node)
        {
            var previousIsPropertyLoopPotentiallyAllowed = this.isPropertyLoopPotentiallyAllowed;

            var isAllowed = IsPropertyLoopUsagePossibleInside(node);
            this.isPropertyLoopPotentiallyAllowed = (previousIsPropertyLoopPotentiallyAllowed, isAllowed) switch
            {
                // expression nodes live inside non-expression nodes, so any value replaces null
                (PropertyLoopCapability.Inconclusive, _) => isAllowed,

                // once it's not allowed, it can't be allowed deeper in the tree
                (PropertyLoopCapability.DisallowedInExpression, _) => PropertyLoopCapability.DisallowedInExpression,

                // once we are in an expression, the children can only be expression nodes, so true or false can replace the true value
                (PropertyLoopCapability.PotentiallyAllowed, not PropertyLoopCapability.Inconclusive) => isAllowed,

                // non-expression nodes (like tokens) can live inside an expression node, but do not alter the decision
                (PropertyLoopCapability.PotentiallyAllowed, PropertyLoopCapability.Inconclusive) => previousIsPropertyLoopPotentiallyAllowed,

                _ => throw new NotImplementedException("Unexpected value of property loop capability.")
            };

            base.VisitInternal(node);

            this.isPropertyLoopPotentiallyAllowed = previousIsPropertyLoopPotentiallyAllowed;
        }

        private void ValidateDirectAccessToResourceOrModuleCollection(SyntaxBase variableOrResourceAccessSyntax)
        {
            var symbol = this.semanticModel.GetSymbolInfo(variableOrResourceAccessSyntax);
            if (symbol is ResourceSymbol { IsCollection: true } || symbol is ModuleSymbol { IsCollection: true })
            {
                // we are inside a dependsOn property and the referenced symbol is a resource/module collection
                var parent = this.semanticModel.Binder.GetParent(variableOrResourceAccessSyntax);
                if (!this.insideTopLevelDependsOn && parent is not ArrayAccessSyntax)
                {
                    // the parent is not array access, which means that someone is doing a direct reference to the collection
                    // which is not allowed outside of the dependsOn properties
                    this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(variableOrResourceAccessSyntax).DirectAccessToCollectionNotSupported());
                }
            }
        }

        private static ObjectPropertySyntax? TryGetDependsOnProperty(ObjectSyntax? body) => body?.SafeGetPropertyByName("dependsOn");

        private bool? IsLoopAllowedHere(ForSyntax syntax)
        {
            if(this.activeLoopCapableTopLevelDeclaration is null)
            {
                // we're not in a loop capable declaration
                return false;
            }

            if(this.IsTopLevelLoop(syntax))
            {
                // this is a loop in a resource, module, variable, or output value
                return true;
            }

            // not a top-level loop
            if(this.activeLoopCapableTopLevelDeclaration is OutputDeclarationSyntax || this.activeLoopCapableTopLevelDeclaration is VariableDeclarationSyntax)
            {
                // output and variable loops are only supported in the values due to runtime limitations
                return false;
            }

            // could be a property loop
            if(this.isPropertyLoopPotentiallyAllowed == PropertyLoopCapability.DisallowedInExpression || !this.IsPropertyLoop(syntax))
            {
                // not a property loop or property loop inside an operator or function call
                return false;
            }

            // possibly allowed - need to check how many property loops we have in the chain
            return null;
        }

        private bool IsPropertyLoop(ForSyntax syntax)
        {
            var parent = this.semanticModel.SourceFile.Hierarchy.GetParent(syntax);
            return IsPropertyLoop(parent, syntax);
        }

        private static bool IsPropertyLoop(SyntaxBase? parent, ForSyntax syntax)
        {
            return parent is ObjectPropertySyntax property && ReferenceEquals(property.Value, syntax);
        }

        /// <summary>
        /// We cannot compile for-expressions when they are inside function calls or operators. This function
        /// checks if the specified node allows for-expression usage.
        /// </summary>
        /// <param name="syntax">The node to check</param>
        /// <returns></returns>
        private static PropertyLoopCapability IsPropertyLoopUsagePossibleInside(SyntaxBase syntax)
        {
            // property loops can be used as long as the path to get to the property is constructed via object or array literals
            // operators or function calls prevent usage of property loops inside
            switch(syntax)
            {
                case not ExpressionSyntax:
                    return PropertyLoopCapability.Inconclusive;

                case ObjectSyntax:
                case ObjectPropertySyntax:
                case ArraySyntax:
                case ArrayItemSyntax:
                    return PropertyLoopCapability.PotentiallyAllowed;

                default:
                    return PropertyLoopCapability.DisallowedInExpression;
            }
        }

        private bool IsTopLevelLoop(ForSyntax syntax)
        {
            var parent = this.semanticModel.SourceFile.Hierarchy.GetParent(syntax);

            switch(parent)
            {
                case ResourceDeclarationSyntax resource when ReferenceEquals(resource.Value, syntax):
                case ModuleDeclarationSyntax module when ReferenceEquals(module.Value, syntax):
                case OutputDeclarationSyntax output when ReferenceEquals(output.Value, syntax):
                case VariableDeclarationSyntax variable when ReferenceEquals(variable.Value, syntax):
                    return true;

                default:
                    return false;
            }
        }
    }
}
