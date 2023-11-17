// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem.Visitors
{
    public class DeployTimeConstantDirectViolationVisitor : DeployTimeConstantViolationVisitor
    {
        public DeployTimeConstantDirectViolationVisitor(SyntaxBase deployTimeConstantContainer, SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter, ResourceTypeResolver resourceTypeResolver)
            : base(deployTimeConstantContainer, semanticModel, diagnosticWriter, resourceTypeResolver)
        {
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            if (ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(syntax.BaseExpression) is ({ } accessedSymbol, { } accessedBodyType))
            {
                var indexExprTypeInfo = SemanticModel.GetTypeInfo(syntax.IndexExpression);
                if (indexExprTypeInfo is StringLiteralType { RawStringValue: var propertyName })
                {
                    // Validate property access via string literal index (myResource['sku']).
                    FlagIfPropertyNotReadableAtDeployTime(syntax, propertyName, accessedSymbol, accessedBodyType);
                }
                else if (indexExprTypeInfo is UnionType { Members: var indexUnionMembers })
                {
                    var unionMemberTypes = indexUnionMembers.Select(m => m.Type).ToList();
                    if (unionMemberTypes.All(t => t is StringLiteralType))
                    {
                        foreach (var unionMemberType in unionMemberTypes.Cast<StringLiteralType>().OrderBy(l => l.RawStringValue))
                        {
                            FlagIfPropertyNotReadableAtDeployTime(syntax, unionMemberType.RawStringValue, accessedSymbol, accessedBodyType);
                        }
                    }
                    else
                    {
                        FlagDeployTimeConstantViolation(syntax, accessedSymbol, accessedBodyType);
                    }
                }
                else
                {
                    // Flag it as dtc constant violation if we cannot resolve the expression to string literals.
                    FlagDeployTimeConstantViolation(syntax, accessedSymbol, accessedBodyType);
                }
            }

            base.VisitArrayAccessSyntax(syntax);
        }

        public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            if (ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(syntax.BaseExpression) is ({ } accessedSymbol, { } accessedBodyType))
            {
                FlagIfPropertyNotReadableAtDeployTime(syntax, syntax.PropertyName.IdentifierName, accessedSymbol, accessedBodyType);
            }

            base.VisitPropertyAccessSyntax(syntax);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            FlagIfAccessingEntireResourceOrModule(syntax);
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

        private void FlagIfAccessingEntireResourceOrModule(SyntaxBase syntax)
        {
            if (DeployTimeConstantContainer is ObjectPropertySyntax property &&
                property.TryGetTypeProperty(SemanticModel) is { } typeProperty &&
                typeProperty.TypeReference is ResourceParentType or ResourceScopeType)
            {
                switch (SemanticModel.Binder.GetParent(syntax))
                {
                    case not PropertyAccessSyntax and not ArrayAccessSyntax when
                        ResourceTypeResolver.TryResolveRuntimeExistingResourceSymbolAndBodyType(syntax) is ({ } resourceSymbol, { } resourceType):
                        {
                            FlagDeployTimeConstantViolation(syntax, resourceSymbol, resourceType);

                            return;
                        }
                    case ArrayAccessSyntax arrayAccessSyntax when
                        arrayAccessSyntax.BaseExpression == syntax &&
                        SemanticModel.Binder.GetParent(arrayAccessSyntax) is not PropertyAccessSyntax and not ArrayAccessSyntax &&
                        ResourceTypeResolver.TryResolveRuntimeExistingResourceSymbolAndBodyType(arrayAccessSyntax) is ({ } resourceSymbol, { } resourceType):
                        {
                            var arrayIndexExprType = SemanticModel.GetTypeInfo(arrayAccessSyntax.IndexExpression);
                            if (arrayIndexExprType.IsIntegerOrIntegerLiteral())
                            {
                                FlagDeployTimeConstantViolation(syntax, resourceSymbol, resourceType);
                            }
                            return;
                        }

                    default:
                        return;
                }
            }

            switch (SemanticModel.Binder.GetParent(syntax))
            {
                // var foo = [for x in [...]: {
                //   bar: myVM <-- accessing an entire resource/module.
                // }]
                case not PropertyAccessSyntax and not ArrayAccessSyntax when
                    ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(syntax) is ({ } accessedSymbol, { } accessedBodyType):
                    {
                        FlagDeployTimeConstantViolation(syntax, accessedSymbol, accessedBodyType);

                        return;
                    }
                // var foo = [for x in [...]: {
                //   bar: myVNets[1] <-- accessing an entire resource/module via an array index.
                // }]
                case ArrayAccessSyntax arrayAccessSyntax when
                    arrayAccessSyntax.BaseExpression == syntax && // need this condition because this case is hit both when syntax is myVNets (variable access) or the index expression
                    SemanticModel.Binder.GetParent(arrayAccessSyntax) is not PropertyAccessSyntax and not ArrayAccessSyntax &&
                    ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(arrayAccessSyntax) is ({ } accessedSymbol, { } accessedBodyType):
                    {
                        var arrayIndexExprType = SemanticModel.GetTypeInfo(arrayAccessSyntax.IndexExpression);
                        if (arrayIndexExprType.IsIntegerOrIntegerLiteral()
                            || arrayIndexExprType.TypeKind == TypeKind.Any
                            || arrayIndexExprType is UnionType indexUnionType && indexUnionType.Members.All(m => m.Type.IsIntegerOrIntegerLiteral()))
                        {
                            FlagDeployTimeConstantViolation(syntax, accessedSymbol, accessedBodyType);
                        }
                        return;
                    }

                default:
                    return;
            }
        }

        private void FlagIfPropertyNotReadableAtDeployTime(SyntaxBase syntax, string propertyName, DeclaredSymbol accessedSymbol, ObjectType accessedBodyType)
        {
            if (accessedBodyType.Properties.TryGetValue(propertyName, out var propertyType) &&
                !propertyType.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime))
            {
                FlagDeployTimeConstantViolation(syntax, accessedSymbol, accessedBodyType, violatingPropertyName: propertyName);
            }
        }

        protected void FlagIfFunctionRequiresInlining(FunctionCallSyntaxBase syntax)
        {
            if (SemanticModel.GetSymbolInfo(syntax) is FunctionSymbol functionSymbol &&
                functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining))
            {
                FlagDeployTimeConstantViolation(syntax);
            }
        }
    }
}
