// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Visitor used to collect errors caused by property assignments to run-time values when that is not allowed
    /// </summary>
    public sealed class DeployTimeConstantVisitor : SyntaxVisitor
    {

        private readonly SemanticModel model;
        private readonly IDiagnosticWriter diagnosticWriter;

        // used for logging the property when we detect a run time property
        private string currentSymbol;

        // Since we collect errors top down, we will collect and overwrite this variable and emit it in the end
        private SyntaxBase? errorSyntax;

        private DeployTimeConstantVisitor(SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            this.model = model;
            this.diagnosticWriter = diagnosticWriter;
            this.currentSymbol = "";
        }

        // entry point for this visitor. We only iterate through modules and resources
        public static void ValidateDeployTimeConstants(SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            var deploymentTimeConstantVisitor = new DeployTimeConstantVisitor(model, diagnosticWriter);
            foreach (var declaredSymbol in model.Root.AllDeclarations.Where(symbol => symbol is ModuleSymbol || symbol is ResourceSymbol))
            {
                deploymentTimeConstantVisitor.Visit(declaredSymbol.DeclaringSyntax);
            }
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            base.VisitArrayAccessSyntax(syntax);
            if (this.errorSyntax != null)
            {
                // Due to the nature of visitPropertyAccessSyntax, we have to visit every level of this
                // nested errorSyntax recursively. The last one will be shown to the user.
                this.errorSyntax = syntax;
            }
            else if (syntax.BaseExpression is VariableAccessSyntax variableAccessSyntax)
            {
                // validate only on resource and module symbols
                var baseSymbol = model.GetSymbolInfo(variableAccessSyntax);
                switch (baseSymbol)
                {
                    case ResourceSymbol:
                    case ModuleSymbol:
                        if (TypeAssignmentVisitor.UnwrapType(((DeclaredSymbol)baseSymbol).Type) is ObjectType bodyObj)
                        {
                            switch (syntax.IndexExpression)
                            {
                                case StringSyntax stringSyntax when stringSyntax.TryGetLiteralValue() is { } literalValue:
                                    if (bodyObj.Properties.TryGetValue(literalValue, out var propertyType) &&
                                    !propertyType.Flags.HasFlag(TypePropertyFlags.SkipInlining))
                                    {
                                        this.errorSyntax = syntax;
                                    }
                                    // TODO this doesn't handle string interpolation
                                    break;
                            }
                            
                        }
                        else if (TypeAssignmentVisitor.UnwrapType(((DeclaredSymbol)baseSymbol).Type) is DiscriminatedObjectType discriminatedBodyObj)
                        {
                            // TODO
                        }
                        break;
                }
            };
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            // runtimeValueAllowed should only be false if we are going through a module or resource's objectPropertySyntax
            if (syntax.Key is IdentifierSyntax keyIdentifier)
            {
                this.currentSymbol = keyIdentifier.IdentifierName;
                // right now we only check for the name property (using resourceNamePropertyName but the name is same for modules)
                if (LanguageConstants.IdentifierComparer.Equals(keyIdentifier.IdentifierName, LanguageConstants.ResourceNamePropertyName))
                {
                    base.VisitObjectPropertySyntax(syntax);
                    // if an error is found at the end we emit it and move on to the next key of this object declaration.
                    if (this.errorSyntax != null)
                    {
                        this.AppendError(this.errorSyntax);
                        this.errorSyntax = null;
                    }
                }
            }
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            base.VisitPropertyAccessSyntax(syntax);
            if (this.errorSyntax != null)
            {
                // Due to the nature of visitPropertyAccessSyntax, we have to visit every level of this
                // nested errorSyntax recursively. The last one will be shown to the user.
                this.errorSyntax = syntax;
            }
            else if (syntax.BaseExpression is VariableAccessSyntax variableAccessSyntax)
            {
                // validate only on resource and module symbols
                var baseSymbol = model.GetSymbolInfo(variableAccessSyntax);
                switch (baseSymbol)
                {
                    case ResourceSymbol:
                    case ModuleSymbol:
                        if (TypeAssignmentVisitor.UnwrapType(((DeclaredSymbol)baseSymbol).Type) is ObjectType bodyObj)
                        {
                            var property = syntax.PropertyName.IdentifierName;
                            if (bodyObj.Properties.TryGetValue(property, out var propertyType) &&
                            !propertyType.Flags.HasFlag(TypePropertyFlags.SkipInlining))
                            {
                                this.errorSyntax = syntax;
                            }
                        }
                        else if (TypeAssignmentVisitor.UnwrapType(((DeclaredSymbol)baseSymbol).Type) is DiscriminatedObjectType discriminatedBodyObj)
                        {
                            // TODO
                        }
                        break;
                }
            }
        }
        private void AppendError(SyntaxBase syntax)
        {
            this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).RuntimePropertyNotAllowed(this.currentSymbol));
        }
    }
}
