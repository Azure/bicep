// Copyright (c) Microsoft Corporation.
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
        private InstanceFunctionCallSyntax? LastInstanceFunctionCallSyntax { get; set; }

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
            var visitingTopExtensionRefThisVisit = !VisitingTopExtensionReference && model.GetSymbolInfo(syntax) is ExtensionNamespaceSymbol;

            if (visitingTopExtensionRefThisVisit)
            {
                VisitingTopExtensionReference = true;

                if ((!InsideModuleExtensionConfigs || !model.Features.ModuleExtensionConfigsEnabled) // Must be in extension configs declaration site
                    && (LastInstanceFunctionCallSyntax is null || !object.ReferenceEquals(syntax, LastInstanceFunctionCallSyntax.BaseExpression))) // except if it's extension reference with an instance function call (ex: az.resourceGroup())
                {
                    diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).ExtensionCannotBeReferenced());
                }
            }

            base.VisitVariableAccessSyntax(syntax);

            if (visitingTopExtensionRefThisVisit)
            {
                VisitingTopExtensionReference = false;
            }
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            LastInstanceFunctionCallSyntax = syntax;
            base.VisitInstanceFunctionCallSyntax(syntax);
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            InsideModuleDeclaration = true;
            base.VisitModuleDeclarationSyntax(syntax);
            InsideModuleDeclaration = false;
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            var insideModuleExtConfigsThisVisit = false;

            if (!InsideModuleExtensionConfigs && InsideModuleDeclaration
                && syntax.TryGetKeyText() == LanguageConstants.ModuleExtensionConfigsPropertyName
                && model.Binder.GetParent(syntax) is ObjectSyntax objectSyntax)
            {
                InsideModuleExtensionConfigs = insideModuleExtConfigsThisVisit = model.Binder.GetParent(objectSyntax) switch
                {
                    ModuleDeclarationSyntax => true,
                    ForSyntax @for when model.Binder.GetParent(@for) is ModuleDeclarationSyntax => true,
                    _ => false
                };
            }

            base.VisitObjectPropertySyntax(syntax);

            if (insideModuleExtConfigsThisVisit)
            {
                InsideModuleExtensionConfigs = false;
            }
        }
    }
}
