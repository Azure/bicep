// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Linq;
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

        // Since we collect errors top down, we will collect and overwrite this variable and emit it in the end
        private SyntaxBase? errorSyntax;
        private string? currentProperty;
        private ObjectType? bodyObj;
        private ObjectType? referencedBodyObj;


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
            if (model.GetSymbolInfo(syntax) is ResourceSymbol resourceSymbol &&
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
            if (model.GetSymbolInfo(syntax) is ModuleSymbol moduleSymbol &&
                moduleSymbol.Type is ModuleType moduleType &&
                moduleType.Body is ObjectType bodyObj)
            {
                this.bodyObj = bodyObj;
            }
            base.VisitModuleDeclarationSyntax(syntax);
            // reset both the current object and referenced object's bodyObj
            this.bodyObj = null;
            this.referencedBodyObj = null;
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
                        if (TypeAssignmentVisitor.UnwrapType(((DeclaredSymbol)baseSymbol).Type) is ObjectType referencedBodyObj)
                        {
                            switch (syntax.IndexExpression)
                            {
                                case StringSyntax stringSyntax:
                                    if (stringSyntax.TryGetLiteralValue() is string literalValue)
                                    {
                                        if (referencedBodyObj.Properties.TryGetValue(literalValue, out var propertyType) &&
                                        !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                                        {
                                            this.errorSyntax = syntax;
                                            this.referencedBodyObj = referencedBodyObj;
                                        }
                                    }
                                    else
                                    {
                                        // we will block referencing module and resource properties using string interpolation 
                                        this.errorSyntax = syntax;
                                        this.referencedBodyObj = referencedBodyObj;
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
                    this.currentProperty = deployTimeIdentifier.Key;
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
                        if (TypeAssignmentVisitor.UnwrapType(((DeclaredSymbol)baseSymbol).Type) is ObjectType referencedBodyObj)
                        {
                            var property = syntax.PropertyName.IdentifierName;
                            if (referencedBodyObj.Properties.TryGetValue(property, out var propertyType) &&
                            !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                            {
                                this.errorSyntax = syntax;
                                this.referencedBodyObj = referencedBodyObj;
                            }
                        }
                        break;
                }
            }
        }
        private void AppendError(SyntaxBase syntax)
        {
            if (this.currentProperty == null)
            {
                throw new NullReferenceException($"{nameof(this.currentProperty)} is null in DeployTimeConstant for syntax {syntax.ToString()}");
            }
            if (this.bodyObj == null)
            {
                throw new NullReferenceException($"{nameof(this.bodyObj)} is null in DeployTimeConstant for syntax {syntax.ToString()}");
            }
            if (this.referencedBodyObj == null)
            {
                throw new NullReferenceException($"{nameof(this.referencedBodyObj)} is null in DeployTimeConstant for syntax {syntax.ToString()}");
            }
            var usableKeys = this.bodyObj.Properties.Where(kv => kv.Value.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant)).Select(kv => kv.Key);
            this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(syntax).RuntimePropertyNotAllowed(this.currentProperty, usableKeys));
        }
    }
}
