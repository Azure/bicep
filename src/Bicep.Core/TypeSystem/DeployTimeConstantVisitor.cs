// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Visitor used to collect errors caused by property assignments to run-time values when that is not allowed
    /// </summary>
    public sealed class DeployTimeConstantVisitor : SyntaxVisitor
    {

        private readonly SemanticModel model;
        private readonly IDiagnosticWriter diagnosticWriter;

        private ObjectType? bodyObj;

        // Since we collect errors top down, we will collect and overwrite this variable and emit it in the end
        private SyntaxBase? errorSyntax;
        private string? currentSymbol;


        private DeployTimeConstantVisitor(SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            this.model = model;
            this.diagnosticWriter = diagnosticWriter;
        }

        // entry point for this visitor. We only iterate through modules and resources
        public static void ValidateDeployTimeConstants(SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            var deploymentTimeConstantVisitor = new DeployTimeConstantVisitor(model, diagnosticWriter);
            foreach (var declaredSymbol in model.Root.ResourceDeclarations)
            {
                deploymentTimeConstantVisitor.Visit(declaredSymbol.DeclaringSyntax);
            }
            foreach (var declaredSymbol in model.Root.ModuleDeclarations)
            {
                deploymentTimeConstantVisitor.Visit(declaredSymbol.DeclaringSyntax);
            }
        }

        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            if (model.GetSymbolInfo(syntax.Body) is ResourceSymbol resourceSymbol &&
                resourceSymbol.Type is ResourceType resourceType &&
                resourceType.Body is ObjectType bodyObj)
            {
                this.bodyObj = bodyObj;
            }
            base.VisitResourceDeclarationSyntax(syntax);
            this.bodyObj = null;
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            if (model.GetSymbolInfo(syntax.Body) is ModuleSymbol moduleSymbol &&
                moduleSymbol.Type is ModuleType moduleType &&
                moduleType.Body is ObjectType bodyObj)
            {
                this.bodyObj = bodyObj;
            }
            base.VisitModuleDeclarationSyntax(syntax);
            this.bodyObj = null;
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
                                case StringSyntax stringSyntax:
                                    if (stringSyntax.TryGetLiteralValue() is string literalValue)
                                    {
                                        if (bodyObj.Properties.TryGetValue(literalValue, out var propertyType) &&
                                        !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                                        {
                                            this.errorSyntax = syntax;
                                        }
                                    }
                                    else
                                    {
                                        // we will block string interpolation on module and resource properties
                                        this.errorSyntax = syntax;
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax)
        {
            if (this.bodyObj == null)
            {
                return;
            }

            foreach (var deployTimeIdentifier in ObjectSyntaxExtensions.ToNamedPropertyDictionary(syntax))
            {
                if (this.bodyObj.Properties.TryGetValue(deployTimeIdentifier.Key, out var propertyType) && propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                {
                    this.currentSymbol = deployTimeIdentifier.Key;
                    this.VisitObjectPropertySyntax(deployTimeIdentifier.Value);
                }
            }
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            base.VisitObjectPropertySyntax(syntax);
            // if an error is found at the end we emit it and move on to the next key of this object declaration.
            if (this.errorSyntax != null)
            {
                this.AppendError(this.errorSyntax);
                this.errorSyntax = null;
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
                            !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                            {
                                this.errorSyntax = syntax;
                            }
                        }
                        break;
                }
            }
        }
        private void AppendError(SyntaxBase syntax)
        {
            if (this.currentSymbol == null)
            {
                throw new NullReferenceException($"current symbol is null in DeployTimeConstant for syntax {syntax.ToString()}");
            }
            this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).DeployTimeConstantRequired(this.currentSymbol));
        }
    }
}
