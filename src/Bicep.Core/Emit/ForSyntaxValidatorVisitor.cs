// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    public sealed class ForSyntaxValidatorVisitor : SyntaxVisitor
    {
        private readonly IDiagnosticWriter diagnosticWriter;

        private readonly SemanticModel semanticModel;

        private readonly Stack<LoopValidationItem> loopParents;

        private SyntaxBase? currentDependsOnPropertyValue = null;

        private ForSyntaxValidatorVisitor(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            this.semanticModel = semanticModel;
            this.diagnosticWriter = diagnosticWriter;

            this.loopParents = new Stack<LoopValidationItem>();
        }

        public static void Validate(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            var visitor = new ForSyntaxValidatorVisitor(semanticModel, diagnosticWriter);
            
            // visiting writes diagnostics in some cases
            visitor.Visit(semanticModel.SyntaxTree.ProgramSyntax);
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            if (syntax.TryGetKeyText() == null && syntax.Value is ForSyntax)
            {
                // block loop usage with properties whose names are expressions
                this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax.Key).ExpressionedPropertiesNotAllowedWithLoops());
            }

            base.VisitObjectPropertySyntax(syntax);
        }

        public override void VisitForSyntax(ForSyntax syntax)
        {
            var item = CreateValidationItem(syntax);

            if (!item.IsValidParent)
            {
                // this loop was used incorrectly
                this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax.ForKeyword).LoopsNotSupported());
            }
            else if (item.PropertyLoopCount > 1)
            {
                // too many property loops
                this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax.ForKeyword).TooManyPropertyLoops());
            }

            // push the parent to the stack
            this.loopParents.Push(item);

            // visit children
            base.VisitForSyntax(syntax);

            // pop the parent
            var lastPopped = this.loopParents.Pop();
            Debug.Assert(ReferenceEquals(lastPopped, item), "ReferenceEquals(lastPopped, item)");
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            var symbol = this.semanticModel.GetSymbolInfo(syntax);
            if (symbol is ResourceSymbol { IsCollection: true } || symbol is ModuleSymbol { IsCollection: true })
            {
                // we are inside a dependsOn property and the referenced symbol is a resource/module collection
                var parent = this.semanticModel.Binder.GetParent(syntax);
                if (this.currentDependsOnPropertyValue is null && parent is not ArrayAccessSyntax)
                {
                    // the parent is not array access, which means that someone is doing a direct reference to the collection
                    // which is not allowed outside of the dependsOn properties
                    this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).DirectAccessToCollectionNotSupported());
                }
            }

            // visit children
            base.VisitVariableAccessSyntax(syntax);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            // stash the body (handles loops and conditions as well)
            this.currentDependsOnPropertyValue = TryGetDependsOnPropertyValue(syntax.TryGetBody());

            // visit children
            base.VisitResourceDeclarationSyntax(syntax);
            
            // clear the stash
            this.currentDependsOnPropertyValue = null;
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            // stash the body (handles loops and conditions as well)
            this.currentDependsOnPropertyValue = TryGetDependsOnPropertyValue(syntax.TryGetBody());

            // visit children
            base.VisitModuleDeclarationSyntax(syntax);

            // clear the stash
            this.currentDependsOnPropertyValue = null;
        }

        private LoopValidationItem CreateValidationItem(ForSyntax syntax)
        {
            var (lastParentValid, lastPropertyLoopCount) = this.loopParents.TryPeek(out var lastStatus)
                ? (lastStatus.IsValidParent, lastStatus.PropertyLoopCount)
                : (true, 0);

            var parent = this.semanticModel.SyntaxTree.Hierarchy.GetParent(syntax);

            // keep the cases in sync with the error message in the default case
            switch (parent)
            {
                // loops are allowed in top-level module/resource values
                case ResourceDeclarationSyntax resource when ReferenceEquals(resource.Value, syntax):
                case ModuleDeclarationSyntax module when ReferenceEquals(module.Value, syntax):
                    return new LoopValidationItem(parent, lastParentValid, lastPropertyLoopCount);

                // loops are generally allowed in property values
                case ObjectPropertySyntax property when ReferenceEquals(property.Value, syntax):
                    return new LoopValidationItem(parent, lastParentValid, lastPropertyLoopCount + 1);

                default:
                    return new LoopValidationItem(parent, false, lastPropertyLoopCount);
            }
        }

        private SyntaxBase? TryGetDependsOnPropertyValue(ObjectSyntax? body) => body?.SafeGetPropertyByName("dependsOn")?.Value;

        private class LoopValidationItem
        {
            public LoopValidationItem(SyntaxBase? parent, bool isValidParent, int propertyLoopCount)
            {
                this.Parent = parent;
                this.IsValidParent = isValidParent;
                this.PropertyLoopCount = propertyLoopCount;
            }

            public SyntaxBase? Parent { get; }

            public bool IsValidParent { get; }

            public int PropertyLoopCount { get; }
        }
    }
}
