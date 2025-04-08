﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.Emit
{
    public class ExtensionReferenceValidatorVisitor : AstVisitor
    {
        private readonly SemanticModel model;
        private readonly IDiagnosticWriter diagnosticWriter;

        private bool VisitingTopExtensionReference { get; set; }
        private bool InsideModuleDeclaration { get; set; }
        private bool InsideModuleExtensionConfigs { get; set; }

        public static void Validate(SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            var visitor = new ExtensionReferenceValidatorVisitor(model, diagnosticWriter);
            visitor.Visit(model.SourceFile.ProgramSyntax);
        }

        private ExtensionReferenceValidatorVisitor(SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            this.model = model;
            this.diagnosticWriter = diagnosticWriter;
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            var visitingTopExtensionReference = !VisitingTopExtensionReference && model.GetSymbolInfo(syntax) is ExtensionNamespaceSymbol;

            if (visitingTopExtensionReference)
            {
                VisitingTopExtensionReference = true;

                if (!InsideModuleExtensionConfigs || model.Features is not { ExtensibilityEnabled: true, ModuleExtensionConfigsEnabled: true })
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).ExtensionCannotBeReferenced());
                }
            }

            base.VisitVariableAccessSyntax(syntax);

            if (visitingTopExtensionReference)
            {
                VisitingTopExtensionReference = false;
            }
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            InsideModuleDeclaration = true;

            base.VisitModuleDeclarationSyntax(syntax);

            InsideModuleDeclaration = false;
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            var insideExtensionConfigs = false;

            if (InsideModuleDeclaration && syntax.TryGetKeyText() == LanguageConstants.ModuleExtensionConfigsPropertyName
                && model.Binder.GetParent(syntax) is ObjectSyntax extensionConfigObjSyntax)
            {
                InsideModuleExtensionConfigs = insideExtensionConfigs = model.Binder.GetParent(extensionConfigObjSyntax) switch
                {
                    ModuleDeclarationSyntax => true,
                    ForSyntax @for when model.Binder.GetParent(@for) is ModuleDeclarationSyntax => true,
                    _ => false
                };
            }

            base.VisitObjectPropertySyntax(syntax);

            if (insideExtensionConfigs)
            {
                InsideModuleExtensionConfigs = false;
            }
        }
    }
}
