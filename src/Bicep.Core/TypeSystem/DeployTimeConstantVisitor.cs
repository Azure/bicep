// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
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
        private string? accessedSymbol;
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
            this.bodyObj = null;
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax)
        {
            if (this.bodyObj == null)
            {
                return;
            }

            foreach (var deployTimeIdentifier in ObjectSyntaxExtensions.ToNamedPropertyDictionary(syntax))
            {
                if (this.bodyObj.Properties.TryGetValue(deployTimeIdentifier.Key, out var propertyType) &&
                    propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                {
                    this.currentProperty = deployTimeIdentifier.Key;
                    this.VisitObjectPropertySyntax(deployTimeIdentifier.Value);
                    this.currentProperty = null;
                }
            }
        }

        public override void VisitObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            base.VisitObjectPropertySyntax(syntax);
            // if an error is found at the end we emit it and move on to the next key of this object declaration.
            if (this.errorSyntax != null)
            {
                this.AppendError();
            }
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            base.VisitArrayAccessSyntax(syntax);
            if (this.errorSyntax != null && TextSpan.AreOverlapping(this.errorSyntax, syntax))
            {
                // Due to the nature of visitPropertyAccessSyntax, we have to visit every level of this
                // nested errorSyntax recursively. The last one will be shown to the user.
                this.errorSyntax = syntax;
            }
            else if (syntax.BaseExpression is VariableAccessSyntax variableAccessSyntax)
            {
                if (this.errorSyntax != null)
                {
                    this.AppendError();
                }
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
                                            this.accessedSymbol = baseSymbol.Name;
                                            this.referencedBodyObj = referencedBodyObj;
                                        }
                                    }
                                    else
                                    {
                                        // we will block referencing module and resource properties using string interpolation 
                                        this.errorSyntax = syntax;
                                        this.accessedSymbol = baseSymbol.Name;
                                        this.referencedBodyObj = referencedBodyObj;
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            base.VisitPropertyAccessSyntax(syntax);
            if (this.errorSyntax != null && TextSpan.AreOverlapping(this.errorSyntax, syntax))
            {
                // Due to the nature of visitPropertyAccessSyntax, we have to visit every level of this
                // nested errorSyntax recursively. The last one will be shown to the user.
                this.errorSyntax = syntax;
            }
            else if (syntax.BaseExpression is VariableAccessSyntax variableAccessSyntax)
            {
                if (this.errorSyntax != null)
                {
                    this.AppendError();
                }
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
                                this.accessedSymbol = baseSymbol.Name;
                                this.referencedBodyObj = referencedBodyObj;
                            }
                        }
                        break;
                }
            }
        }

        private void AppendError()
        {
            if (this.errorSyntax == null)
            {
                throw new NullReferenceException($"{nameof(this.errorSyntax)} is null in DeployTimeConstant");
            }
            if (this.currentProperty == null)
            {
                throw new NullReferenceException($"{nameof(this.currentProperty)} is null in DeployTimeConstant for syntax {this.errorSyntax.ToString()}");
            }
            if (this.bodyObj == null)
            {
                throw new NullReferenceException($"{nameof(this.bodyObj)} is null in DeployTimeConstant for syntax {this.errorSyntax.ToString()}");
            }
            if (this.referencedBodyObj == null)
            {
                throw new NullReferenceException($"{nameof(this.referencedBodyObj)} is null in DeployTimeConstant for syntax {this.errorSyntax.ToString()}");
            }
            if (this.accessedSymbol == null)
            {
                throw new NullReferenceException($"{nameof(this.accessedSymbol)} is null in DeployTimeConstant for syntax {this.errorSyntax.ToString()}");
            }
            var usableKeys = this.referencedBodyObj.Properties.Where(kv => kv.Value.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant)).Select(kv => kv.Key);
            this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(this.errorSyntax).RuntimePropertyNotAllowed(this.currentProperty, usableKeys, this.accessedSymbol));

            this.errorSyntax = null;
            this.referencedBodyObj = null;
            this.accessedSymbol = null;
        }
    }
}
