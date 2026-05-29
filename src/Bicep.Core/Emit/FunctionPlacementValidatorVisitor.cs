// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
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
                if (functionSymbol.FunctionFlags.HasFlag(FunctionFlags.ModuleSecureParameterOnly))
                {
                    // we can check placement only for functions that were matched and has a proper placement flag
                    var (_, levelUpSymbol) = syntaxRecorder.Skip(1).SkipWhile(x => x.syntax is TernaryOperationSyntax).FirstOrDefault();
                    
                    // Check if getSecret is nested inside an object structure (not a direct child of params/extensionConfigs)
                    // Invalid for params: params: { config: { secret: kv.getSecret(...) } } <- ObjectSyntax between function and parameter property
                    // Valid for params: params: { secret: kv.getSecret(...) } <- No ObjectSyntax between  
                    // Valid for params: params: { secret: cond ? kv.getSecret(...) : 'x' } <- Ternaries are skipped
                    // Valid for extensionConfigs: extensionConfigs: { alias: { prop: kv.getSecret(...) } } <- 1 ObjectSyntax (alias) is OK
                    // Invalid for extensionConfigs: extensionConfigs: { alias: { obj: { prop: kv.getSecret(...) } } } <- 2+ ObjectSyntax is invalid
                    
                    // Count ObjectSyntax nodes between the immediate property and the params/extensionConfigs value object
                    // The params value object is the ObjectSyntax that immediately follows the params ObjectPropertySyntax
                    var ancestors = syntaxRecorder
                        .Skip(1) // Skip the function call
                        .SkipWhile(x => x.syntax is TernaryOperationSyntax) // Skip ternary operators
                        .Skip(1) // Skip the immediate ObjectPropertySyntax (the property containing getSecret, e.g., mySecret, certificate)
                        .ToList();
                    
                    // Find the params/extensionConfigs property
                    var paramsPropertyIndex = ancestors.FindIndex(x => 
                        x.syntax is ObjectPropertySyntax ops &&
                        ops.TryGetKeyText() is string key &&
                        (string.Equals(key, LanguageConstants.ModuleParamsPropertyName, LanguageConstants.IdentifierComparison) ||
                         string.Equals(key, LanguageConstants.ModuleExtensionConfigsPropertyName, LanguageConstants.IdentifierComparison)));
                    
                    // Count ObjectSyntax nodes before the params property (excluding the params value object)
                    // Stop one element BEFORE the params property (since the element before params property is the params value object)
                    var objectSyntaxCount = paramsPropertyIndex >= 1
                        ? ancestors.Take(paramsPropertyIndex - 1).Count(x => x.syntax is ObjectSyntax)
                        : 0;
                    
                    var isInExtensionConfigs = elementsRecorder.TryPeek(out var head) && head is VisitedElement.ModuleExtensionConfigs;
                    var maxAllowedNesting = isInExtensionConfigs ? 1 : 0; // Extension configs allow 1 level (the alias), params allow 0
                    var isNestedInObject = levelUpSymbol is PropertySymbol && objectSyntaxCount > maxAllowedNesting;
                    
                    if (!(elementsRecorder.TryPeek(out head) && head is VisitedElement.ModuleParams or VisitedElement.ModuleExtensionConfigs)
                        || levelUpSymbol is not PropertySymbol propertySymbol
                        || !(TypeHelper.TryRemoveNullability(propertySymbol.Type) ?? propertySymbol.Type).ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure)
                        || isNestedInObject)
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
    }
}
