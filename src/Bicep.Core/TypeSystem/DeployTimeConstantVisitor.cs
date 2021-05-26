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
        private SyntaxBase? deployTimeConstantScopeSyntax;
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
            new DeployTimeConstantVisitor(model, diagnosticWriter).Visit(model.Root.Syntax);
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
            if (model.GetSymbolInfo(syntax) is ResourceSymbol resourceSymbol)
            {
                this.bodyType = resourceSymbol.TryGetBodyObjectType();
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
            if (model.GetSymbolInfo(syntax) is ModuleSymbol moduleSymbol)
            {
                this.bodyType = moduleSymbol.TryGetBodyObjectType();
            }

            base.VisitModuleDeclarationSyntax(syntax);
            this.bodyType = null;
        }

        public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            if (syntax.Value is ForSyntax)
            {
                this.Visit(syntax.Value);
            }
        }
        #endregion

        public override void VisitIfConditionSyntax(IfConditionSyntax syntax)
        {
            this.Visit(syntax.Keyword);
            this.deployTimeConstantScopeSyntax = syntax;
            this.Visit(syntax.ConditionExpression);

            if (this.errorSyntax != null)
            {
                this.AppendError();
            }

            this.deployTimeConstantScopeSyntax = null;
            this.Visit(syntax.Body);
        }

        public override void VisitForSyntax(ForSyntax syntax)
        {
            this.Visit(syntax.OpenSquare);
            this.Visit(syntax.ForKeyword);
            this.Visit(syntax.VariableSection);
            this.Visit(syntax.InKeyword);
            this.deployTimeConstantScopeSyntax = syntax;
            this.Visit(syntax.Expression);

            if (this.errorSyntax != null)
            {
                this.AppendError();
            }

            this.deployTimeConstantScopeSyntax = null;
            this.Visit(syntax.Colon);
            this.Visit(syntax.Body);
            this.Visit(syntax.CloseSquare);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            var currentDeployTimeConstantScopeSyntax = this.deployTimeConstantScopeSyntax;

            if (this.model.Binder.GetSymbolInfo(syntax) is FunctionSymbol functionSymbol &&
                functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining))
            {
                this.deployTimeConstantScopeSyntax = syntax;
            }

            base.VisitFunctionCallSyntax(syntax);

            if (this.errorSyntax is not null)
            {
                this.AppendError();
            }

            
            this.deployTimeConstantScopeSyntax = currentDeployTimeConstantScopeSyntax;
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            if (this.model.Binder.GetParent(syntax) is DecoratorSyntax)
            {
                return;
            }

            var baseType = this.model.TypeManager.GetTypeInfo(syntax.BaseExpression);

            if (TypeAssignmentVisitor.UnwrapType(baseType) is not ObjectType objectType)
            {
                return;
            }

            var symbol = SymbolValidator.ResolveObjectQualifiedFunctionWithoutValidatingFlags(
                objectType.MethodResolver.TryGetSymbol(syntax.Name),
                syntax.Name,
                objectType);

            if (symbol is not FunctionSymbol functionSymbol)
            {
                return;
            }

            var currentDeployTimeConstantScopeSyntax = this.deployTimeConstantScopeSyntax;

            if (functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining))
            {
                this.deployTimeConstantScopeSyntax = syntax;
            }

            base.VisitInstanceFunctionCallSyntax(syntax);

            if (this.errorSyntax is not null)
            {
                this.AppendError();
            }

            
            this.deployTimeConstantScopeSyntax = currentDeployTimeConstantScopeSyntax;
        }

        public override void VisitObjectSyntax(ObjectSyntax syntax)
        {
            if (syntax.HasParseErrors())
            {
                return;
            }

            // Only visit the object properties if they are required to be deploy time constant.
            foreach (var (propertyName, propertySyntax) in syntax.ToNamedPropertyDictionary())
            {
                var isTopLevelDeployTimeConstantProperty =
                    this.bodyType is not null &&
                    this.bodyType.Properties.TryGetValue(propertyName, out var propertyType) &&
                    propertyType.Flags.HasFlag(TypePropertyFlags.DeployTimeConstant);

                if (isTopLevelDeployTimeConstantProperty || this.deployTimeConstantScopeSyntax is not null)
                {
                    // Also need to reset deployTimeConstantScopeSyntax for nested deploy-time constant properties such as "tags.*".
                    this.deployTimeConstantScopeSyntax = propertySyntax;
                }

                // In case there's a nested property whose name matches a deployment constant property name,
                // for example, "properties.replicaSets[0].location", set bodyType to null to avoid flagging that property.
                var currentBodyType = this.bodyType;
                this.bodyType = null;

                this.VisitObjectPropertySyntax(propertySyntax);

                // Restore bodyType for the current level.
                this.bodyType = currentBodyType;

                if (isTopLevelDeployTimeConstantProperty)
                {
                    this.deployTimeConstantScopeSyntax = null;
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
            if (this.deployTimeConstantScopeSyntax is null)
            {
                return;
            }

            if (this.model.GetSymbolInfo(syntax) is VariableSymbol variableSymbol &&
                this.model.Binder.TryGetCycle(variableSymbol) is null)
            {
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

        private void SetState(SyntaxBase errorSyntax, DeclaredSymbol referencedSymbol, ObjectType referencedBodyType, string propertyName)
        {
            if (!referencedBodyType.Properties.TryGetValue(propertyName, out var propertyType))
            {
                return;
            }

            if (this.deployTimeConstantScopeSyntax is null)
            {
                return;
            }

            if (!propertyType.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime) ||
                ReferencedSymbolIsResourceAndPropertyIsAbsent(referencedSymbol, propertyName))
            {
                // Set error state if the property is not a deploy-time constant, or it is a
                // deploy-time constant of a resource, but it does not exist in the resource body.
                this.errorSyntax = errorSyntax;
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
            if (this.deployTimeConstantScopeSyntax == null)
            {
                throw new InvalidOperationException($"{nameof(this.deployTimeConstantScopeSyntax)} is null in {this.GetType().Name} for syntax {this.errorSyntax}");
            }
            if (this.referencedBodyType == null)
            {
                throw new InvalidOperationException($"{nameof(this.referencedBodyType)} is null in {this.GetType().Name} for syntax {this.errorSyntax}");
            }
            if (this.referencedSymbol == null)
            {
                throw new InvalidOperationException($"{nameof(this.referencedSymbol)} is null in {this.GetType().Name} for syntax {this.errorSyntax}");
            }

            var usableProperties = this.referencedBodyType.Properties
                .Where(kv => kv.Value.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime) && !kv.Value.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                .Select(kv => kv.Key);

            if (this.referencedSymbol is ResourceSymbol resourceSymbol &&
                resourceSymbol.DeclaringResource.TryGetBody() is { } bodySyntax)
            {
                var declaredTopLevelPropertyNames = bodySyntax.ToNamedPropertyDictionary().Keys
                    .Append(LanguageConstants.ResourceIdPropertyName)
                    .Append(LanguageConstants.ResourceTypePropertyName)
                    .Append(LanguageConstants.ResourceApiVersionPropertyName);

                usableProperties = usableProperties.Intersect(declaredTopLevelPropertyNames, LanguageConstants.IdentifierComparer);
            }

            var variableDependencyChain = this.variableVisitorStack?.ToArray().Reverse().Select(symbol => symbol.Name);
            var diagnosticBuilder = DiagnosticBuilder.ForPosition(this.errorSyntax);

            diagnosticWriter.Write(this.deployTimeConstantScopeSyntax switch
            {
                ObjectPropertySyntax propertySyntax when propertySyntax.TryGetKeyText() is { } propertyName =>
                    diagnosticBuilder.RuntimePropertyNotAllowedInProperty(propertyName, usableProperties, this.referencedSymbol.Name, variableDependencyChain),
                IfConditionSyntax =>
                    diagnosticBuilder.RuntimePropertyNotAllowedInIfConditionExpression(usableProperties, this.referencedSymbol.Name, variableDependencyChain),
                ForSyntax =>
                    diagnosticBuilder.RuntimePropertyNotAllowedInForExpression(usableProperties, this.referencedSymbol.Name, variableDependencyChain),
                FunctionCallSyntax functionCallSyntax =>
                    diagnosticBuilder.RuntimePropertyNotAllowedInRunTimeFunctionArguments(functionCallSyntax.Name.IdentifierName, usableProperties, this.referencedSymbol.Name, variableDependencyChain),
                InstanceFunctionCallSyntax instanceFunctionCallSyntax =>
                    diagnosticBuilder.RuntimePropertyNotAllowedInRunTimeFunctionArguments(instanceFunctionCallSyntax.Name.IdentifierName, usableProperties, this.referencedSymbol.Name, variableDependencyChain),
                _ =>
                    throw new ArgumentOutOfRangeException($"Expected {nameof(this.deployTimeConstantScopeSyntax)} to be ObjectPropertySyntax with a propertyName, IfConditionSyntax, or ForSyntax."),
            });

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
        public static (DeclaredSymbol?, ObjectType?) ExtractResourceOrModuleSymbolAndBodyType(SemanticModel model, VariableAccessSyntax syntax) => model.GetSymbolInfo(syntax) switch
        {
            ResourceSymbol { IsCollection: false } resourceSymbol => (resourceSymbol, resourceSymbol.TryGetBodyObjectType()),
            ModuleSymbol { IsCollection: false } moduleSymbol => (moduleSymbol, moduleSymbol.TryGetBodyObjectType()),
            _ => (null, null),
        };

        public static (DeclaredSymbol?, ObjectType?) ExtractResourceOrModuleCollectionSymbolAndBodyType(SemanticModel model, VariableAccessSyntax syntax) => model.GetSymbolInfo(syntax) switch
        {
            ResourceSymbol { IsCollection: true } resourceSymbol => (resourceSymbol, resourceSymbol.TryGetBodyObjectType()),
            ModuleSymbol { IsCollection: true } moduleSymbol => (moduleSymbol, moduleSymbol.TryGetBodyObjectType()),
            _ => (null, null),
        };

        public static bool ReferencedSymbolIsResourceAndPropertyIsAbsent(DeclaredSymbol referencedSymbol, string propertyName)
        {
            if (referencedSymbol is not ResourceSymbol { DeclaringResource: ResourceDeclarationSyntax resourceSyntax })
            {
                return false;
            }

            if (LanguageConstants.ReadWriteDeployTimeConstantPropertyNames.Contains(propertyName, LanguageConstants.IdentifierComparer))
            {
                // Read-write deploy-time constant properties (id, name, type, and apiVersion) are always avaiable.
                return false;
            }

            if (resourceSyntax.TryGetBody() is not { } bodySyntax || bodySyntax.HasParseErrors())
            {
                // Bail if there are errors.
                return false;
            }

            return bodySyntax.SafeGetPropertyByName(propertyName) is null;
        }
        #endregion
    }
}
