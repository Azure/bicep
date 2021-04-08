// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Emit
{
    public sealed class ForSyntaxValidatorVisitor : SyntaxVisitor
    {
        // we don't support nesting of property loops right now
        private const int MaximumNestedPropertyLoopCount = 1;

        private readonly IDiagnosticWriter diagnosticWriter;
        private readonly SemanticModel semanticModel;

        private SyntaxBase? activeLoopCapableTopLevelDeclaration = null;

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
            visitor.Visit(semanticModel.SyntaxTree.ProgramSyntax);
        }

        public static bool IsAddingPropertyLoopAllowed(SemanticModel semanticModel, ObjectPropertySyntax property)
        {
            SyntaxBase? current = property;
            int propertyLoopCount = 0;
            while(current is not null)
            {
                var parent = semanticModel.SyntaxTree.Hierarchy.GetParent(current);
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

                case true when this.activeLoopCapableTopLevelDeclaration is VariableDeclarationSyntax variable && InlineDependencyVisitor.ShouldInlineVariable(this.semanticModel, variable, out var variableChain):
                    // this is a loop variable that has a dependency on functions that are not supported in JSON variables
                    // we are initially blocking this because not all cases can be generated in the JSON
                    
                    // unable to get a detailed variable dependency chain
                    // log a generic error instead and put it on the "for" keyword
                    this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax.ForKeyword).VariableLoopsRuntimeDependencyNotAllowed(variableChain));

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
            var symbol = this.semanticModel.GetSymbolInfo(syntax);
            if (symbol is ResourceSymbol { IsCollection: true } || symbol is ModuleSymbol { IsCollection: true })
            {
                // we are inside a dependsOn property and the referenced symbol is a resource/module collection
                var parent = this.semanticModel.Binder.GetParent(syntax);
                if (!this.insideTopLevelDependsOn && parent is not ArrayAccessSyntax)
                {
                    // the parent is not array access, which means that someone is doing a direct reference to the collection
                    // which is not allowed outside of the dependsOn properties
                    this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).DirectAccessToCollectionNotSupported());
                }
            }

            // visit children
            base.VisitVariableAccessSyntax(syntax);
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
            if(!this.IsPropertyLoop(syntax))
            {
                // not a proeprty loop
                return false;
            }

            // possibly allowed - need to check how many property loops we have in the chain
            return null;
        }

        private bool IsPropertyLoop(ForSyntax syntax)
        {
            var parent = this.semanticModel.SyntaxTree.Hierarchy.GetParent(syntax);
            return IsPropertyLoop(parent, syntax);
        }

        private static bool IsPropertyLoop(SyntaxBase? parent, ForSyntax syntax)
        {
            return parent is ObjectPropertySyntax property && ReferenceEquals(property.Value, syntax);
        }

        private bool IsTopLevelLoop(ForSyntax syntax)
        {
            var parent = this.semanticModel.SyntaxTree.Hierarchy.GetParent(syntax);

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
