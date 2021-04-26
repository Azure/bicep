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
        private bool withinDeployTimeConstantScope;
        private ObjectType? bodyType;
        private ObjectType? referencedBodyType;
        private Symbol? referencedSymbol;

        private Stack<DeclaredSymbol>? variableVisitorStack;

        private DeployTimeConstantVisitor(SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            this.model = model;
            this.diagnosticWriter = diagnosticWriter;
        }

        // entry point for this visitor. We only iterate through modules and resources
        public static void ValidateDeployTimeConstants(SemanticModel model, IDiagnosticWriter diagnosticWriter)
        {
            var deploymentTimeConstantVisitor = new DeployTimeConstantVisitor(model, diagnosticWriter);
            foreach (var declaredSymbol in model.Root.GetAllResourceDeclarations())
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
            // in certain cases, errors will cause this visitor to short-circuit, 
            // which causes state to be left over after processing a peer declaration
            // let's clean it up
            var previousBodyType = this.bodyType;
            this.bodyType = null;
            ResetState();

            // Once https://github.com/Azure/bicep/issues/1177 is fixed,
            // it should be possible to use 
            // model.GetSymbolInfo(syntax.Body) is ObectType
            // Currently propertyFlags are not propagated
            var symbol = model.GetSymbolInfo(syntax);
            switch(symbol)
            {
                case ResourceSymbol { IsCollection: false, Type: ResourceType { Body: ObjectType bodyObj } }:
                    this.bodyType = bodyObj;
                    break;

                case ResourceSymbol { IsCollection: true, Type: ArrayType { Item: ResourceType { Body: ObjectType bodyObj } } }:
                    this.bodyType = bodyObj;
                    break;
            }

            base.VisitResourceDeclarationSyntax(syntax);
            this.bodyType = previousBodyType;
        }

        public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            // in certain cases, errors will cause this visitor to short-circuit, 
            // which causes state to be left over after processing a peer declaration
            // let's clean it up
            this.bodyType = null;
            ResetState();

            // Once https://github.com/Azure/bicep/issues/1177 is fixed,
            // it should be possible to use 
            // model.GetSymbolInfo(syntax.Body) is ObectType
            // Currently propertyFlags are not propagated
            var symbol = model.GetSymbolInfo(syntax);
            switch(symbol)
            {
                case ModuleSymbol { IsCollection: false, Type: ModuleType { Body: ObjectType bodyType } }:
                    this.bodyType = bodyType;
                    break;

                case ModuleSymbol { IsCollection: true, Type: ArrayType { Item: ModuleType { Body: ObjectType bodyType } } }:
                    this.bodyType = bodyType;
                    break;
            }

            base.VisitModuleDeclarationSyntax(syntax);
            this.bodyType = null;
        }
        #endregion

        public override void VisitObjectSyntax(ObjectSyntax syntax)
        {
            if (this.bodyType == null)
            {
                return;
            }

            if (syntax.HasParseErrors())
            {
                return;
            }

            // Only visit the object properties if they are required to be deploy time constant.
            foreach (var deployTimeIdentifier in ObjectSyntaxExtensions.ToNamedPropertyDictionary(syntax))
            {
                if (!this.bodyType.Properties.TryGetValue(deployTimeIdentifier.Key, out var propertyType))
                {
                    continue;
                }

                if (propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                {
                    this.withinDeployTimeConstantScope = true;
                }

                if (this.withinDeployTimeConstantScope)
                {
                    this.currentProperty = deployTimeIdentifier.Key;
                    this.VisitObjectPropertySyntax(deployTimeIdentifier.Value);
                    this.currentProperty = null;
                }

                if (propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant))
                {
                    this.withinDeployTimeConstantScope = false;
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
                    if (variableVisitor.InvalidReferencedBodyType != null)
                    {
                        this.errorSyntax = syntax;
                        this.referencedBodyType = variableVisitor.InvalidReferencedBodyType;
                        this.variableVisitorStack = variableVisitor.VisitedStack;
                        this.referencedSymbol = variableVisitor.VisitedStack.Peek();
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
                if (ExtractResourceOrModuleSymbolAndBodyType(this.model, variableAccessSyntax) is ({ } referencedSymbol, { } referencedBodyType))
                {
                    switch (syntax.IndexExpression)
                    {
                        case StringSyntax stringSyntax when stringSyntax.TryGetLiteralValue() is string literalValue:
                            SetState(syntax, referencedSymbol, referencedBodyType, literalValue);
                            break;
                        
                        default:
                            // we will block referencing module and resource properties using string interpolation and number indexing
                            this.errorSyntax = syntax;
                            this.referencedSymbol = referencedSymbol;
                            this.referencedBodyType = referencedBodyType;
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
            else
            {
                switch (syntax.BaseExpression)
                {
                    case VariableAccessSyntax variableAccessSyntax:
                        {
                            // This is a non-overlapping error, which means that there's two or more runtime properties being referenced
                            if (this.errorSyntax != null)
                            {
                                this.AppendError();
                            }

                            if(ExtractResourceOrModuleSymbolAndBodyType(this.model, variableAccessSyntax) is ({ } referencedSymbol, { } referencedBodyType))
                            {
                                SetState(syntax, referencedSymbol, referencedBodyType, syntax.PropertyName.IdentifierName);
                            }

                            break;
                        }

                    case ArrayAccessSyntax { BaseExpression: VariableAccessSyntax baseVariableAccess }:
                        {
                            if (this.errorSyntax != null)
                            {
                                this.AppendError();
                            }

                            if (ExtractResourceOrModuleCollectionSymbolAndBodyType(this.model, baseVariableAccess) is ({ } referencedSymbol, { } referencedBodyType))
                            {
                                SetState(syntax, referencedSymbol, referencedBodyType, syntax.PropertyName.IdentifierName);
                            }

                            break;
                        }
                }
            }
        }
        #endregion

        private void SetState(SyntaxBase syntax, DeclaredSymbol referencedSymbol, ObjectType referencedBodyType, string propertyName)
        {
            if (!referencedBodyType.Properties.TryGetValue(propertyName, out var propertyType))
            {
                return;
            }

            if (!propertyType.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime) ||
                ReferencedSymbolIsResourceAndPropertyIsAbsent(referencedSymbol, propertyName))
            {
                // Set error state if the property is not a deploy-time constant, or it is a
                // deploy-time constant of a resource, but it does not exist in the resource body.
                this.errorSyntax = syntax;
                this.referencedSymbol = referencedSymbol;
                this.referencedBodyType = referencedBodyType;
            }
        }

        private void AppendError()
        {
            if (this.errorSyntax == null)
            {
                throw new InvalidOperationException($"{nameof(this.errorSyntax)} is null in {this.GetType().Name}");
            }
            if (this.currentProperty == null)
            {
                throw new InvalidOperationException($"{nameof(this.currentProperty)} is null in {this.GetType().Name} for syntax {this.errorSyntax}");
            }
            if (this.bodyType == null)
            {
                throw new InvalidOperationException($"{nameof(this.bodyType)} is null in {this.GetType().Name} for syntax {this.errorSyntax}");
            }
            if (this.referencedBodyType == null)
            {
                throw new InvalidOperationException($"{nameof(this.referencedBodyType)} is null in {this.GetType().Name} for syntax {this.errorSyntax}");
            }
            if (this.referencedSymbol == null)
            {
                throw new InvalidOperationException($"{nameof(this.referencedSymbol)} is null in {this.GetType().Name} for syntax {this.errorSyntax}");
            }

            var usableKeys = this.referencedBodyType.Properties
                .Where(kv => kv.Value.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime) && !kv.Value.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                .Select(kv => kv.Key);

            if (this.referencedSymbol is ResourceSymbol resourceSymbol &&
                resourceSymbol.DeclaringResource.TryGetBody() is { } bodySyntax)
            {
                var declaredTopLevelPropertyNames = bodySyntax.ToKnownPropertyNames()
                    .Add(LanguageConstants.ResourceIdPropertyName)
                    .Add(LanguageConstants.ResourceTypePropertyName)
                    .Add(LanguageConstants.ResourceApiVersionPropertyName);

                usableKeys = usableKeys.Intersect(declaredTopLevelPropertyNames);
            }

            var variableDependencyChain = this.variableVisitorStack?.ToArray().Reverse().Select(symbol => symbol.Name);
            this.diagnosticWriter.Write(DiagnosticBuilder.ForPosition(this.errorSyntax).RuntimePropertyNotAllowed(this.currentProperty, usableKeys, this.referencedSymbol.Name, variableDependencyChain));

            ResetState();
        }

        private void ResetState()
        {
            this.errorSyntax = null;
            this.referencedBodyType = null;
            this.referencedSymbol = null;
            this.variableVisitorStack = null;
        }

        #region Helpers
        public static (DeclaredSymbol?, ObjectType?) ExtractResourceOrModuleSymbolAndBodyType(SemanticModel model, VariableAccessSyntax syntax)
        {
            var baseSymbol = model.GetSymbolInfo(syntax);
            switch (baseSymbol)
            {
                case ResourceSymbol { IsCollection: false }:
                case ModuleSymbol { IsCollection: false }:
                    var declaredSymbol = (DeclaredSymbol)baseSymbol;
                    var unwrapped = TypeAssignmentVisitor.UnwrapType(declaredSymbol.Type);
                    return (declaredSymbol, unwrapped as ObjectType);
            }
            return (null, null);
        }

        public static (DeclaredSymbol?, ObjectType?) ExtractResourceOrModuleCollectionSymbolAndBodyType(SemanticModel model, VariableAccessSyntax syntax)
        {
            var baseSymbol = model.GetSymbolInfo(syntax);
            switch (baseSymbol)
            {
                case ResourceSymbol { IsCollection: true }:
                case ModuleSymbol { IsCollection: true }:
                    var declaredCollectionSymbol = (DeclaredSymbol)baseSymbol;
                    return (declaredCollectionSymbol, declaredCollectionSymbol.Type is ArrayType arrayType ? TypeAssignmentVisitor.UnwrapType(arrayType.Item.Type) as ObjectType : null);
            }
            return (null, null);
        }

        public static bool ReferencedSymbolIsResourceAndPropertyIsAbsent(DeclaredSymbol referencedSymbol, string propertyName)
        {
            if (referencedSymbol is not ResourceSymbol { DeclaringResource: ResourceDeclarationSyntax resourceSyntax })
            {
                return false;
            }

            if (LanguageConstants.StandardizedResourcePropertyNames.Contains(propertyName, LanguageConstants.IdentifierComparer))
            {
                // Standardized resource properties (id, name, type, and apiVersion) are always avaiable.
                return false;
            }

            if (resourceSyntax.TryGetBody() is not { } bodySyntax || bodySyntax.HasParseErrors())
            {
                return false;
            }

            return bodySyntax.SafeGetPropertyByName(propertyName) is null;
        }
        #endregion
    }
}
