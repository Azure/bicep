// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Visitors
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

            visitedVariableNameStack.Push(variableName);

            base.VisitVariableDeclarationSyntax(syntax);

            // This variable declaration was deployment time constant
            if (visitedVariableNameStack.Pop() is var popped && popped != variableName)
            {
                throw new InvalidOperationException($"{GetType().Name} performed an invalid Stack push/pop: expected popped element to be {variableName} but got {popped}");
            }
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            if (ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(syntax.BaseExpression) is ({ } accessedSymbol, { } accessedBodyType))
            {
                var indexExprTypeInfo = SemanticModel.GetTypeInfo(syntax.IndexExpression);
                if (indexExprTypeInfo is StringLiteralType { RawStringValue: var propertyName })
                {
                    // Validate property access via string literal index (myResource['sku']).
                    FlagIfPropertyNotReadableAtDeployTime(propertyName, accessedSymbol, accessedBodyType);
                }
                else if (indexExprTypeInfo is UnionType { Members: var indexUnionMembers })
                {
                    var unionMemberTypes = indexUnionMembers.Select(m => m.Type).ToList();
                    if (unionMemberTypes.All(t => t is StringLiteralType))
                    {
                        foreach (var unionMemberType in unionMemberTypes.Cast<StringLiteralType>().OrderBy(l => l.RawStringValue))
                        {
                            FlagIfPropertyNotReadableAtDeployTime(unionMemberType.RawStringValue, accessedSymbol, accessedBodyType);
                        }
                    }
                    else
                    {
                        FlagDeployTimeConstantViolationWithVariableDependencies(accessedSymbol, accessedBodyType);
                    }
                }
                else
                {
                    // Flag it as dtc constant violation if we cannot resolve the expression to string literals.
                    FlagDeployTimeConstantViolationWithVariableDependencies(accessedSymbol, accessedBodyType);
                }
            }

            base.VisitArrayAccessSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            if (ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(syntax.BaseExpression) is ({ } accessedSymbol, { } accessedBodyType))
            {
                FlagIfPropertyNotReadableAtDeployTime(syntax.PropertyName.IdentifierName, accessedSymbol, accessedBodyType);
            }

            base.VisitPropertyAccessSyntax(syntax);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (SemanticModel.GetSymbolInfo(syntax) is VariableSymbol variableSymbol &&
                SemanticModel.Binder.TryGetCycle(variableSymbol) is null)
            {
                Visit(variableSymbol.DeclaringSyntax);
            }
            else
            {
                FlagIfAccessingEntireResourceOrModule(syntax);
            }
        }

        public override void VisitResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            FlagIfAccessingEntireResourceOrModule(syntax);
        }

        public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            FlagIfFunctionRequiresInlining(syntax);

            base.VisitFunctionCallSyntax(syntax);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            FlagIfFunctionRequiresInlining(syntax);

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

        private void FlagDeployTimeConstantViolation(DeclaredSymbol? accessedSymbol = null, ObjectType? accessedObjectType = null, IEnumerable<string>? variableDependencyChain = null, string? violatingPropertyName = null)
        {
            // For indirect violations, errorSyntax is always variableDependency.
            FlagDeployTimeConstantViolation(variableDependency, accessedSymbol, accessedObjectType, variableDependencyChain, violatingPropertyName);

            hasError = true;
        }

        private void FlagIfAccessingEntireResourceOrModule(SyntaxBase syntax)
        {
            switch (SemanticModel.Binder.GetParent(syntax))
            {
                // var foo = [for x in [...]: {
                //   bar: myVM <-- accessing an entire resource/module.
                // }]
                case not PropertyAccessSyntax and not ArrayAccessSyntax when
                    ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(syntax) is ({ } accessedSymbol, { } accessedBodyType):
                    {
                        FlagDeployTimeConstantViolationWithVariableDependencies(accessedSymbol, accessedBodyType);
                        break;
                    }
                // var foo = [for x in [...]: {
                //   bar: myVNets[1] <-- accessing an entire resource/module via an array index.
                // }]
                case ArrayAccessSyntax arrayAccessSyntax when
                    arrayAccessSyntax.BaseExpression == syntax &&
                    SemanticModel.Binder.GetParent(arrayAccessSyntax) is not PropertyAccessSyntax and not ArrayAccessSyntax &&
                    ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(arrayAccessSyntax) is ({ } resourceSymbol, { } resourceType):
                    {
                        var arrayIndexExprType = SemanticModel.GetTypeInfo(arrayAccessSyntax.IndexExpression);
                        if (arrayIndexExprType.IsIntegerOrIntegerLiteral()
                            || arrayIndexExprType.TypeKind == TypeKind.Any
                            || arrayIndexExprType is UnionType indexUnionType && indexUnionType.Members.All(m => m.Type.IsIntegerOrIntegerLiteral()))
                        {
                            FlagDeployTimeConstantViolationWithVariableDependencies(resourceSymbol, resourceType);
                        }
                        return;
                    }

                default:
                    break;
            }
        }

        private void FlagDeployTimeConstantViolationWithVariableDependencies(DeclaredSymbol accessedSymbol, ObjectType accessedBodyType, string? violatingPropertyName = null)
        {
            var variableDependencyChain = BuildVariableDependencyChain(accessedSymbol.Name);
            FlagDeployTimeConstantViolation(accessedSymbol, accessedBodyType, variableDependencyChain, violatingPropertyName);
        }

        private void FlagIfPropertyNotReadableAtDeployTime(string propertyName, DeclaredSymbol accessedSymbol, ObjectType accessedBodyType)
        {
            if (accessedBodyType.Properties.TryGetValue(propertyName, out var propertyType) &&
                !propertyType.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime))
            {
                FlagDeployTimeConstantViolationWithVariableDependencies(accessedSymbol, accessedBodyType, propertyName);
            }
        }

        protected void FlagIfFunctionRequiresInlining(FunctionCallSyntaxBase syntax)
        {
            if (SemanticModel.GetSymbolInfo(syntax) is FunctionSymbol functionSymbol &&
                functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining))
            {
                var variableDependencyChain = BuildVariableDependencyChain(functionSymbol.Name);
                FlagDeployTimeConstantViolation(variableDependencyChain: variableDependencyChain);
            }
        }

        private IEnumerable<string> BuildVariableDependencyChain(string tail) => visitedVariableNameStack.ToArray().Reverse().Append(tail);
    }
}
