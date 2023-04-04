// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class DeployTimeConstantIndirectViolationVisitor : DeployTimeConstantViolationVisitor
    {
        private readonly VariableAccessSyntax variableDependency;

        private readonly Stack<string> visitedVariableNameStack = new();

        private bool hasError = false;

        public DeployTimeConstantIndirectViolationVisitor(SyntaxBase deployTimeConstantContainer, VariableAccessSyntax variableDependency, SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter, ResourceTypeResolver resourceTypeResolver)
            : base(deployTimeConstantContainer, semanticModel, diagnosticWriter, resourceTypeResolver)
        {
            this.variableDependency = variableDependency;
        }

        public override void VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            var variableName = syntax.Name.IdentifierName;

            this.visitedVariableNameStack.Push(variableName);

            base.VisitVariableDeclarationSyntax(syntax);

            // This variable declaration was deployment time constant
            if (this.visitedVariableNameStack.Pop() is var popped && popped != variableName)
            {
                throw new InvalidOperationException($"{this.GetType().Name} performed an invalid Stack push/pop: expected popped element to be {variableName} but got {popped}");
            }
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            if (this.ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(syntax.BaseExpression) is ({ } accessedSymbol, { } accessedBodyType))
            {
                var indexExprTypeInfo = SemanticModel.GetTypeInfo(syntax.IndexExpression);
                if (indexExprTypeInfo is StringLiteralType { RawStringValue: var propertyName })
                {
                    // Validate property access via string literal index (myResource['sku']).
                    this.FlagIfPropertyNotReadableAtDeployTime(propertyName, accessedSymbol, accessedBodyType);
                }
                else if (indexExprTypeInfo is UnionType { Members: var indexUnionMembers })
                {
                    var unionMemberTypes = indexUnionMembers.Select(m => m.Type).ToList();
                    if (unionMemberTypes.All(t => t is StringLiteralType))
                    {
                        foreach (var unionMemberType in unionMemberTypes.Cast<StringLiteralType>())
                        {
                            if (this.FlagIfPropertyNotReadableAtDeployTime(unionMemberType.RawStringValue, accessedSymbol, accessedBodyType))
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        this.FlagDeployTimeConstantViolation(syntax, accessedSymbol, accessedBodyType);
                    }
                }
                else
                {
                    // Flag it as dtc constant violation if we cannot resolve the expression to string literals.
                    this.FlagDeployTimeConstantViolation(syntax, accessedSymbol, accessedBodyType);
                }
            }

            base.VisitArrayAccessSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            if (this.ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(syntax.BaseExpression) is ({ } accessedSymbol, { } accessedBodyType))
            {
                this.FlagIfPropertyNotReadableAtDeployTime(syntax.PropertyName.IdentifierName, accessedSymbol, accessedBodyType);
            }

            base.VisitPropertyAccessSyntax(syntax);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (this.SemanticModel.GetSymbolInfo(syntax) is VariableSymbol variableSymbol &&
                this.SemanticModel.Binder.TryGetCycle(variableSymbol) is null)
            {
                this.Visit(variableSymbol.DeclaringSyntax);
            }
            else
            {
                this.FlagIfAccessingEntireResourceOrModule(syntax);
            }
        }

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            this.FlagIfAccessingEntireResourceOrModule(syntax);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            this.FlagIfFunctionRequiresInlining(syntax);

            base.VisitFunctionCallSyntax(syntax);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            this.FlagIfFunctionRequiresInlining(syntax);

            base.VisitInstanceFunctionCallSyntax(syntax);
        }

        protected override void VisitInternal(SyntaxBase node)
        {
            if (hasError)
            {
                // Short circuiting: we only show one violation at a time per variable.
                return;
            }

            base.VisitInternal(node);
        }

        private void FlagDeployTimeConstantViolation(DeclaredSymbol? accessedSymbol = null, ObjectType? accessedObjectType = null, IEnumerable<string>? variableDependencyChain = null)
        {
            // For indirect violations, errorSyntax is always variableDependency.
            this.FlagDeployTimeConstantViolation(this.variableDependency, accessedSymbol, accessedObjectType, variableDependencyChain);

            this.hasError = true;
        }

        private void FlagIfAccessingEntireResourceOrModule(SyntaxBase syntax)
        {
            switch (this.SemanticModel.Binder.GetParent(syntax))
            {
                // var foo = [for x in [...]: {
                //   bar: myVM <-- accessing an entire resource/module.
                // }]
                case not PropertyAccessSyntax and not ArrayAccessSyntax when
                    this.ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(syntax) is ({ } accessedSymbol, { } accessedBodyType):
                    {
                        var variableDependencyChain = this.BuildVariablDependencyChain(accessedSymbol.Name);
                        this.FlagDeployTimeConstantViolation(accessedSymbol, accessedBodyType, variableDependencyChain);

                        break;
                    }
                // var foo = [for x in [...]: {
                //   bar: myVNets[1] <-- accessing an entire resource/module via an array index.
                // }]
                case ArrayAccessSyntax arrayAccessSyntax when
                    arrayAccessSyntax.BaseExpression == syntax &&
                    this.SemanticModel.Binder.GetParent(arrayAccessSyntax) is not PropertyAccessSyntax and not ArrayAccessSyntax &&
                    this.ResourceTypeResolver.TryResolveRuntimeExistingResourceSymbolAndBodyType(arrayAccessSyntax) is ({ } resourceSymbol, { } resourceType):
                {
                    var arrayIndexExprType = this.SemanticModel.GetTypeInfo(arrayAccessSyntax.IndexExpression);
                    if (arrayIndexExprType.IsIntegerOrIntegerLiteral())
                    {
                        var variableDependencyChain = this.BuildVariablDependencyChain(resourceSymbol.Name);
                        this.FlagDeployTimeConstantViolation(resourceSymbol, resourceType, variableDependencyChain);
                    }

                    return;
                }

                default:
                    break;
            }
        }

        private bool FlagIfPropertyNotReadableAtDeployTime(string propertyName, DeclaredSymbol accessedSymbol, ObjectType accessedBodyType)
        {
            if (accessedBodyType.Properties.TryGetValue(propertyName, out var propertyType) &&
                !propertyType.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime))
            {
                var variableDependencyChain = this.BuildVariablDependencyChain(accessedSymbol.Name);
                this.FlagDeployTimeConstantViolation(accessedSymbol, accessedBodyType, variableDependencyChain);
                return true;
            }

            return false;
        }

        protected void FlagIfFunctionRequiresInlining(FunctionCallSyntaxBase syntax)
        {
            if (this.SemanticModel.GetSymbolInfo(syntax) is FunctionSymbol functionSymbol &&
                functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining))
            {
                var variableDependencyChain = this.BuildVariablDependencyChain(functionSymbol.Name);
                FlagDeployTimeConstantViolation(variableDependencyChain: variableDependencyChain);
            }
        }

        private IEnumerable<string> BuildVariablDependencyChain(string tail) => this.visitedVariableNameStack.ToArray().Reverse().Append(tail);
    }
}
