// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Visitor used to collect errors caused by property assignments to run-time variables,
    /// currently, we only run this on the "name" property of resources and modules
    /// </summary>
    public sealed class DeployTimeConstantVisitor : SyntaxVisitor
    {

        private readonly SemanticModel model;
        private readonly IDiagnosticWriter diagnosticWriter;

        // used for logging the property when we detect a run time property
        private string currentSymbol;
        // whether a runtimeValue is allowed for the currentSymbol
        private bool runtimeValueAllowed;

        private DeployTimeConstantVisitor(SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            this.model = model;
            this.diagnosticWriter = diagnosticWriter;
            this.runtimeValueAllowed = true;
            this.currentSymbol = "";
        }

        // entry point for this visitor
        public static void ValidateDeployTimeConstants(SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            new DeployTimeConstantVisitor(model, diagnosticWriter).Visit(model.Root.Syntax);
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            this.runtimeValueAllowed = false;
            base.VisitResourceDeclarationSyntax(syntax);
            this.runtimeValueAllowed = true;
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            this.runtimeValueAllowed = false;
            base.VisitModuleDeclarationSyntax(syntax);
            this.runtimeValueAllowed = true;
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            // if (syntax.IndexExpression.)
            // base.VisitArrayAccessSyntax(syntax);
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            bool priorRuntimeValueAllowed = this.runtimeValueAllowed;
            // runtimeValueAllowed should only be false if we are going through a module or resource's objectPropertySyntax
            if (!this.runtimeValueAllowed && syntax.Key is IdentifierSyntax keyIdentifier) 
            {
                this.currentSymbol = keyIdentifier.IdentifierName;
                // right now we only check for the name property (using resourceNamePropertyName but the name is same for modules)
                if (!LanguageConstants.IdentifierComparer.Equals(keyIdentifier.IdentifierName, LanguageConstants.ResourceNamePropertyName))
                {
                    this.runtimeValueAllowed = true;
                }
            }
            base.VisitObjectPropertySyntax(syntax);
            // restore the value prior to the visit
            this.runtimeValueAllowed = priorRuntimeValueAllowed;
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            base.VisitPropertyAccessSyntax(syntax);
            if (!this.runtimeValueAllowed)
            {
                if (syntax.BaseExpression is VariableAccessSyntax variableAccessSyntax)
                {
                    // validate only on resource and module symbols
                    var baseSymbol = model.GetSymbolInfo(variableAccessSyntax);
                    switch (baseSymbol)
                    {
                        case ResourceSymbol resourceSymbol:
                        case ModuleSymbol moduleSymbol:
                            if (TypeAssignmentVisitor.UnwrapType(((DeclaredSymbol) baseSymbol).Type) is ObjectType bodyObj)
                            {
                                var property = syntax.PropertyName.IdentifierName;
                                if (bodyObj.Properties.TryGetValue(property, out var propertyType) &&
                                !propertyType.Flags.HasFlag(TypePropertyFlags.SkipInlining))
                                {
                                    AppendError(syntax);
                                }
                            }
                            else if (TypeAssignmentVisitor.UnwrapType(((DeclaredSymbol) baseSymbol).Type) is DiscriminatedObjectType discriminatedBodyObj)
                            {
                                // TODO
                            }
                            break;
                    }
                }
                else if (syntax.BaseExpression is PropertyAccessSyntax propertyAccessSyntax) 
                {
                    while (propertyAccessSyntax.BaseExpression is PropertyAccessSyntax nestedPropertyAccessSyntax)
                    {
                        propertyAccessSyntax = nestedPropertyAccessSyntax;
                    }
                    if (propertyAccessSyntax.BaseExpression is VariableAccessSyntax nestedVariableAccessSyntax)
                    {
                        if (model.GetSymbolInfo(nestedVariableAccessSyntax) is ResourceSymbol resourceSymbol ||
                            model.GetSymbolInfo(nestedVariableAccessSyntax) is ModuleSymbol moduleSymbol)
                        {
                            AppendError(syntax);
                            this.runtimeValueAllowed = true;
                        }
                    }
                }
            }
        }

        private void AppendError(SyntaxBase syntax)
        {
            this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).RuntimePropertyNotAllowed(currentSymbol));
        }

    }
}
