// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;
using System;
using System.Linq;

namespace Bicep.Core.Emit
{
    public class FunctionPlacementValidatorVisitor : SyntaxVisitor
    {
        private enum VisitedElement
        {
            Module,
            ModuleParams
        }

        private readonly SemanticModel semanticModel;
        private readonly IDiagnosticWriter diagnosticWriter;

        private readonly VisitorRecorder<(SyntaxBase syntax, Symbol? symbol)> syntaxRecorder = new();
        private readonly VisitorRecorder<VisitedElement> elementsRecorder = new();

        private FunctionPlacementValidatorVisitor(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            this.semanticModel = semanticModel;
            this.diagnosticWriter = diagnosticWriter;
        }

        public static void Validate(SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
        {
            var visitor = new FunctionPlacementValidatorVisitor(semanticModel, diagnosticWriter);
            // visiting writes diagnostics in some cases
            visitor.Visit(semanticModel.SourceFile.ProgramSyntax);
        }

        protected override void VisitInternal(SyntaxBase node)
        {
            using var _ = syntaxRecorder.Scope((node, semanticModel.GetSymbolInfo(node)));
            base.VisitInternal(node);
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            if (semanticModel.GetTypeInfo(syntax).IsError())
            {
                return; //suppressing checking this module. it couldn't be read therefore diagnostics emitted might be misleading
            }
            using var _ = elementsRecorder.Scope(VisitedElement.Module);
            base.VisitModuleDeclarationSyntax(syntax);
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            var vistingModuleParams = elementsRecorder.Contains(VisitedElement.Module)
                && !(elementsRecorder.TryPeek(out var head) && head == VisitedElement.ModuleParams)
                && syntax.Key is IdentifierSyntax identifierSyntax
                && string.Equals(identifierSyntax.IdentifierName, LanguageConstants.ModuleParamsPropertyName, LanguageConstants.IdentifierComparison);

            using var _ = vistingModuleParams ? elementsRecorder.Scope(VisitedElement.ModuleParams) : null;
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
            if (semanticModel.GetSymbolInfo(syntax) is FunctionSymbol functionSymbol && functionSymbol.FunctionFlags.HasFlag(FunctionFlags.ModuleSecureParameterOnly))
            {
                // we can check placement only for funtions that were matched and has a proper placement flag
                var (_, levelUpSymbol) = syntaxRecorder.Skip(1).FirstOrDefault();
                if (!(elementsRecorder.TryPeek(out var head) && head == VisitedElement.ModuleParams)
                    || levelUpSymbol is not PropertySymbol propertySymbol
                    || !propertySymbol.Type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).FunctionOnlyValidInModuleSecureParameterAssignment(functionSymbol.Name));
                }
            }
        }
    }
}
