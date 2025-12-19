// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;

namespace Bicep.Core.Emit
{
    public class FunctionPlacementValidatorVisitor : AstVisitor
    {
        private enum VisitedElement
        {
            Module,
            ModuleParams,
            ModuleExtensionConfigs
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
            VisitedElement? scope = null;

            if (elementsRecorder.Contains(VisitedElement.Module) && syntax.Key is IdentifierSyntax identifierSyntax && elementsRecorder.TryPeek(out var head))
            {
                if (head != VisitedElement.ModuleParams && string.Equals(identifierSyntax.IdentifierName, LanguageConstants.ModuleParamsPropertyName, LanguageConstants.IdentifierComparison))
                {
                    scope = VisitedElement.ModuleParams;
                }
                else if (head != VisitedElement.ModuleExtensionConfigs && string.Equals(identifierSyntax.IdentifierName, LanguageConstants.ModuleExtensionConfigsPropertyName, LanguageConstants.IdentifierComparison))
                {
                    scope = VisitedElement.ModuleExtensionConfigs;
                }
            }

            using var _ = scope is not null ? elementsRecorder.Scope(scope.Value) : null;

            base.VisitObjectPropertySyntax(syntax);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            VerifyModuleSecureParameterFunctionPlacement(syntax);
            VerifyDeclaredFunctionLambdaFunctionPlacement(syntax);
            base.VisitInstanceFunctionCallSyntax(syntax);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            VerifyModuleSecureParameterFunctionPlacement(syntax);
            VerifyDeclaredFunctionLambdaFunctionPlacement(syntax);
            base.VisitFunctionCallSyntax(syntax);
        }

        private void VerifyModuleSecureParameterFunctionPlacement(FunctionCallSyntaxBase syntax)
        {
            if (semanticModel.GetSymbolInfo(syntax) is FunctionSymbol functionSymbol)
            {
                if (functionSymbol.FunctionFlags.HasFlag(FunctionFlags.ModuleSecureParameterOnly))
                {
                    // we can check placement only for functions that were matched and has a proper placement flag
                    var (_, levelUpSymbol) = syntaxRecorder.Skip(1).SkipWhile(x => x.syntax is TernaryOperationSyntax).FirstOrDefault();
                    if (!(elementsRecorder.TryPeek(out var head) && head is VisitedElement.ModuleParams or VisitedElement.ModuleExtensionConfigs)
                        || levelUpSymbol is not PropertySymbol propertySymbol
                        || !(TypeHelper.TryRemoveNullability(propertySymbol.Type) ?? propertySymbol.Type).ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax)
                            .FunctionOnlyValidInModuleSecureParameterAndExtensionConfigAssignment(functionSymbol.Name, semanticModel.Features.ModuleExtensionConfigsEnabled));
                    }
                }

                if (functionSymbol.FunctionFlags.HasFlag(FunctionFlags.DirectAssignment))
                {
                    var (_, levelUpSymbol) = syntaxRecorder.Skip(1).SkipWhile(x => x.syntax is TernaryOperationSyntax).FirstOrDefault();
                    if (levelUpSymbol is null)
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).FunctionOnlyValidWithDirectAssignment(functionSymbol.Name));
                    }
                }
            }
        }

        private void VerifyDeclaredFunctionLambdaFunctionPlacement(FunctionCallSyntaxBase syntax)
        {
            if (semanticModel.SourceFile.FileKind != BicepSourceFileKind.BicepFile)
            {
                return; // only apply this rule to .bicep files
            }

            if (semanticModel.GetSymbolInfo(syntax) is FunctionSymbol functionSymbol)
            {
                if (functionSymbol.FunctionFlags.HasFlag(FunctionFlags.DeclaredFunctionLambdaOnly))
                {
                    if (!syntaxRecorder.Any(x => x.symbol is DeclaredFunctionSymbol))
                    {
                        diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).FunctionOnlyValidWithinDeclaredFunctionLambda(functionSymbol.Name));
                    }
                }
            }
        }
    }
}
