// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bicep.Core.Emit
{
    public sealed class FunctionPlacementValidatorVisitor : SyntaxVisitor
    {
        private enum VisitedElement
        {
            Module,
            ModuleParams
        }

        private readonly SemanticModel semanticModel;
        private readonly IDiagnosticWriter diagnosticWriter;

        private readonly Stack<Symbol> visitedSymbolsStack = new();
        private readonly Stack<VisitedElement> visitedElementsStack = new();

        private class VisitorSymbolScope : IDisposable
        {
            private readonly FunctionPlacementValidatorVisitor visitor;
            private readonly Symbol? symbol;

            public VisitorSymbolScope(FunctionPlacementValidatorVisitor visitor, Symbol? symbol)
            {
                this.visitor = visitor;
                if (symbol is not null)
                {
                    this.visitor.visitedSymbolsStack.Push(symbol);
                    this.symbol = symbol;
                }
            }
            public void Dispose()
            {
                if (symbol is not null)
                {
                    var popped = visitor.visitedSymbolsStack.Pop();
                    if (popped != symbol)
                    {
                        //this error is thrown only if we forgot to implement pop in the visitor or we implement it wrong
                        throw new InvalidOperationException($"Unexpected element on visited stack. Expecting symbol {symbol.Name}, got {popped.Name}");
                    }
                }
            }
        }

        private class VisitorElementScope : IDisposable
        {
            private readonly FunctionPlacementValidatorVisitor visitor;
            private readonly VisitedElement element;

            public VisitorElementScope(FunctionPlacementValidatorVisitor visitor, VisitedElement element)
            {
                this.visitor = visitor;
                this.visitor.visitedElementsStack.Push(element);
                this.element = element;

            }
            public void Dispose()
            {
                var popped = visitor.visitedElementsStack.Pop();
                if (popped != element)
                {
                    //this error is thrown only if we forgot to implement pop in the visitor or we implement it wrong
                    throw new InvalidOperationException($"Unexpected element on visited stack. Expecting element {element}, got {popped}");
                }

            }
        }
        private FunctionPlacementValidatorVisitor(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            this.semanticModel = semanticModel;
            this.diagnosticWriter = diagnosticWriter;
        }

        public static void Validate(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            var visitor = new FunctionPlacementValidatorVisitor(semanticModel, diagnosticWriter);

            // visiting writes diagnostics in some cases
            visitor.Visit(semanticModel.SyntaxTree.ProgramSyntax);
        }

        protected override void VisitInternal(SyntaxBase node)
        {
            using var _ = new VisitorSymbolScope(this, semanticModel.GetSymbolInfo(node));
            base.VisitInternal(node);
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            using var _ = new VisitorElementScope(this, VisitedElement.Module);
            base.VisitModuleDeclarationSyntax(syntax);
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            var vistingModuleParams = visitedElementsStack.Contains(VisitedElement.Module)
                && !(visitedElementsStack.TryPeek(out var head) && head == VisitedElement.ModuleParams)
                && syntax.Key is IdentifierSyntax identifierSyntax
                && string.Equals(identifierSyntax.IdentifierName, "params", StringComparison.OrdinalIgnoreCase);

            using var _ = vistingModuleParams ? new VisitorElementScope(this, VisitedElement.ModuleParams) : null;
            base.VisitObjectPropertySyntax(syntax);

        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            VerifyModuleSecureParameterFunctionPlacement(syntax);
            base.VisitInstanceFunctionCallSyntax(syntax);
        }



        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            VerifyModuleSecureParameterFunctionPlacement(syntax);
            base.VisitFunctionCallSyntax(syntax);
        }

        private void VerifyModuleSecureParameterFunctionPlacement(FunctionCallSyntaxBase syntax)
        {
            if (semanticModel.GetSymbolInfo(syntax) is FunctionSymbol functionSymbol)
            {
                var functionOverload = semanticModel.TypeManager.GetMatchedFunctionOverload(functionSymbol);
                if (functionOverload is not null && functionOverload.PlacementFlags.HasFlag(FunctionPlacementFlags.ModuleSecureParameterOnly))
                { // we can check placement only for funtions that were matched and has a proper placement flag
                    var levelUpSymbol = this.visitedSymbolsStack.Skip(1).FirstOrDefault();
                    if (!(visitedElementsStack.TryPeek(out var head) && head == VisitedElement.ModuleParams)
                        || levelUpSymbol is not PropertySymbol propertySymbol
                        || !propertySymbol.Type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).FunctionOnlyValidInModuleSecureParameterAssignment(functionSymbol.Name));
                    }
                }
            }
        }
    }
}
