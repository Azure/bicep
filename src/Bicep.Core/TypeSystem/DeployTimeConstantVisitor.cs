// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Collections.Generic;
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

        private Stack<string>? variableVisitorStack;


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

        #region DeclarationSyntax
        // these need to be kept synchronized
        public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            // Once https://github.com/Azure/bicep/issues/1177 is fixed,
            // it should be possible to use 
            // model.GetSymbolInfo(syntax.Body) is ObectType
            // Currently propertyFlags are not propagated
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
            // Once https://github.com/Azure/bicep/issues/1177 is fixed,
            // it should be possible to use 
            // model.GetSymbolInfo(syntax.Body) is ObectType
            // Currently propertyFlags are not propagated
            if (model.GetSymbolInfo(syntax) is ModuleSymbol moduleSymbol &&
                moduleSymbol.Type is ModuleType moduleType &&
                moduleType.Body is ObjectType bodyObj)
            {
                this.bodyObj = bodyObj;
            }
            base.VisitModuleDeclarationSyntax(syntax);
            this.bodyObj = null;
        }
        #endregion

        public override void VisitObjectSyntax(ObjectSyntax syntax)
        {
            if (this.bodyObj == null)
            {
                return;
            }
            // Only visit the object properties if they are required to be deploy time constant.
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
            // The last error of this property access will be emitted here.
            if (this.errorSyntax != null)
            {
                this.AppendError();
            }
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            var baseSymbol = model.GetSymbolInfo(syntax);
            switch (baseSymbol)
            {
                case VariableSymbol variableSymbol:
                    // emit any error that has already been triggered previously in the value assignment
                    if (this.errorSyntax != null)
                    {
                        this.AppendError();
                    }
                    var variableVisitor = new DeployTimeConstantVariableVisitor(this.model);
                    variableVisitor.Visit(variableSymbol.DeclaringSyntax);
                    if (variableVisitor.invalidReferencedBodyObj != null)
                    {
                        this.errorSyntax = syntax;
                        this.referencedBodyObj = variableVisitor.invalidReferencedBodyObj;
                        this.variableVisitorStack = variableVisitor.visitedStack;
                        this.accessedSymbol = variableVisitor.visitedStack.Peek();
                    }
                    break;
            }
        }

        #region AccessSyntax
        // these need to be kept synchronized.
        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            base.VisitArrayAccessSyntax(syntax);
            if (this.errorSyntax != null && TextSpan.AreOverlapping(this.errorSyntax, syntax))
            {
                // Due to the nature of visitPropertyAccessSyntax, we have to propagate the
                // nested errorSyntax up the stack.
                this.errorSyntax = syntax;
            }
            else if (syntax.BaseExpression is VariableAccessSyntax variableAccessSyntax)
            {
                // This is a non-overlapping error, which means that there's two or more runtime properties being referenced
                if (this.errorSyntax != null)
                {
                    this.AppendError();
                }

                // validate only on resource and module symbols
                if (ExtractResourceOrModuleSymbolAndBodyObj(this.model, variableAccessSyntax) is ({ } declaredSymbol, { } referencedBodyObj))
                {
                    switch (syntax.IndexExpression)
                    {
                        case StringSyntax stringSyntax when stringSyntax.TryGetLiteralValue() is string literalValue:
                            if (referencedBodyObj.Properties.TryGetValue(literalValue, out var propertyType) &&
                            !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                            {
                                this.errorSyntax = syntax;
                                this.accessedSymbol = declaredSymbol.Name;
                                this.referencedBodyObj = referencedBodyObj;
                            }
                            break;
                        default:
                            // we will block referencing module and resource properties using string interpolation and number indexing
                            this.errorSyntax = syntax;
                            this.accessedSymbol = declaredSymbol!.Name;
                            this.referencedBodyObj = referencedBodyObj;
                            break;
                    }
                }
            }
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            base.VisitPropertyAccessSyntax(syntax);
            if (this.errorSyntax != null && TextSpan.AreOverlapping(this.errorSyntax, syntax))
            {
                // Due to the nature of visitPropertyAccessSyntax, we have to propagate the
                // nested errorSyntax up the stack.
                this.errorSyntax = syntax;
            }
            else if (syntax.BaseExpression is VariableAccessSyntax variableAccessSyntax)
            {
                // This is a non-overlapping error, which means that there's two or more runtime properties being referenced
                if (this.errorSyntax != null)
                {
                    this.AppendError();
                }
                if (ExtractResourceOrModuleSymbolAndBodyObj(this.model, variableAccessSyntax) is ({ } declaredSymbol, { } referencedBodyObj) &&
                    referencedBodyObj.Properties.TryGetValue(syntax.PropertyName.IdentifierName, out var propertyType) &&
                    !propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                {
                    this.errorSyntax = syntax;
                    this.accessedSymbol = declaredSymbol.Name;
                    this.referencedBodyObj = referencedBodyObj;
                }
            }
        }
        #endregion

        private void AppendError()
        {
            if (this.errorSyntax == null)
            {
                throw new NullReferenceException($"{nameof(this.errorSyntax)} is null in {this.GetType().Name}");
            }
            if (this.currentProperty == null)
            {
                throw new NullReferenceException($"{nameof(this.currentProperty)} is null in {this.GetType().Name} for syntax {this.errorSyntax.ToString()}");
            }
            if (this.bodyObj == null)
            {
                throw new NullReferenceException($"{nameof(this.bodyObj)} is null in {this.GetType().Name} for syntax {this.errorSyntax.ToString()}");
            }
            if (this.referencedBodyObj == null)
            {
                throw new NullReferenceException($"{nameof(this.referencedBodyObj)} is null in {this.GetType().Name} for syntax {this.errorSyntax.ToString()}");
            }
            if (this.accessedSymbol == null)
            {
                throw new NullReferenceException($"{nameof(this.accessedSymbol)} is null in {this.GetType().Name} for syntax {this.errorSyntax.ToString()}");
            }
            var usableKeys = this.referencedBodyObj.Properties.Where(kv => kv.Value.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant)).Select(kv => kv.Key);
            this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(this.errorSyntax).RuntimePropertyNotAllowed(this.currentProperty, usableKeys, this.accessedSymbol, this.variableVisitorStack?.ToArray().Reverse()));

            this.errorSyntax = null;
            this.referencedBodyObj = null;
            this.accessedSymbol = null;
            this.variableVisitorStack = null;
        }

        #region Helpers
        public static (DeclaredSymbol?, ObjectType?) ExtractResourceOrModuleSymbolAndBodyObj(SemanticModel model, VariableAccessSyntax syntax)
        {
            var baseSymbol = model.GetSymbolInfo(syntax);
            switch (baseSymbol)
            {
                case ResourceSymbol:
                case ModuleSymbol:
                    var declaredSymbol = (DeclaredSymbol)baseSymbol;
                    var unwrapped = TypeAssignmentVisitor.UnwrapType(declaredSymbol.Type);
                    return (declaredSymbol, unwrapped is ObjectType ? (ObjectType)unwrapped : null);
            }
            return (null, null);
        }
        #endregion
    }
}
