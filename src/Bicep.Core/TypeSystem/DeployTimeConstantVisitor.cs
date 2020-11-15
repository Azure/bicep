// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Visitor used to collect errors caused by property assignments to run-time variables
    /// </summary>
    public sealed class DeployTimeConstantVisitor : SyntaxVisitor
    {

        private readonly SemanticModel.SemanticModel model;
        private readonly IDiagnosticWriter diagnosticWriter;

        private string currentSymbol;
        private bool runtimeValueAllowed;

        private DeployTimeConstantVisitor(SemanticModel.SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            this.model = model;
            this.diagnosticWriter = diagnosticWriter;
            this.runtimeValueAllowed = true;
            this.currentSymbol = "";
        }

        public static void ValidateDeployTimeConstants(SemanticModel.SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            new DeployTimeConstantVisitor(model, diagnosticWriter).Visit(model.Root.Syntax);
        }
    
        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            if (syntax.Key is IdentifierSyntax keyIdentifier) 
            {
                if (keyIdentifier.IdentifierName.Equals(LanguageConstants.ResourceNamePropertyName, System.StringComparison.Ordinal))
                {
                    this.runtimeValueAllowed = false;
                    this.currentSymbol = keyIdentifier.IdentifierName;
                }
            }
            base.VisitObjectPropertySyntax(syntax);
            this.runtimeValueAllowed = true;
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            if (!this.runtimeValueAllowed)
            {
                if (syntax.BaseExpression is VariableAccessSyntax variableAccessSyntax)
                {
                    // validate only on resource and module symbols
                    // TODO these switch cases are so redundant.....
                    switch (model.GetSymbolInfo(variableAccessSyntax))
                    {
                        case ResourceSymbol resourceSymbol:
                            if ((resourceSymbol.Type is ResourceType resourceType) &&
                            (resourceType.Body is ObjectType resourceBodyObj))
                            {
                                var property = syntax.PropertyName.IdentifierName;
                                if (resourceBodyObj.Properties.TryGetValue(property, out var propertyType) &&
                                !propertyType.Flags.HasFlag(TypePropertyFlags.SkipInlining))
                                {
                                    AppendError(syntax);
                                }
                            }
                            break;
                        case ModuleSymbol moduleSymbol:
                            if ((moduleSymbol.Type is ModuleType moduleType) &&
                                (moduleType.Body is ObjectType moduleBodyObj))
                            {
                                var property = syntax.PropertyName.IdentifierName;
                                if (moduleBodyObj.Properties.TryGetValue(property, out var propertyType) &&
                                !propertyType.Flags.HasFlag(TypePropertyFlags.SkipInlining))
                                {
                                    AppendError(syntax);
                                }
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
                        }
                    }
                }
                base.VisitPropertyAccessSyntax(syntax);
            }
        }

        private void AppendError(SyntaxBase syntax)
        {
            this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).RuntimePropertyNotAllowed(currentSymbol));
        }
    }
}
