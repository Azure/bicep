// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public class DeployTimeConstantDirectViolationVisitor : DeployTimeConstantViolationVisitor
    {
        public DeployTimeConstantDirectViolationVisitor(SyntaxBase deployTimeConstantContainer, SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter, ResourceTypeResolver resourceTypeResolver)
            : base(deployTimeConstantContainer, semanticModel, diagnosticWriter, resourceTypeResolver)
        {
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            if (this.ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(syntax.BaseExpression) is ({ } accessedSymbol, { } accessedBodyType))
            {
                var indexExprTypeInfo = SemanticModel.GetTypeInfo(syntax.IndexExpression);
                if (indexExprTypeInfo is StringLiteralType { RawStringValue: var propertyName })
                {
                    // Validate property access via string literal index (myResource['sku']).
                    this.FlagIfPropertyNotReadableAtDeployTime(syntax, propertyName, accessedSymbol, accessedBodyType);
                }
                else if (indexExprTypeInfo is UnionType { Members: var indexUnionMembers })
                {
                    var unionMemberTypes = indexUnionMembers.Select(m => m.Type).ToList();
                    if (unionMemberTypes.All(t => t is StringLiteralType))
                    {
                        foreach (var unionMemberType in unionMemberTypes.Cast<StringLiteralType>().OrderBy(l => l.RawStringValue))
                        {
                            this.FlagIfPropertyNotReadableAtDeployTime(syntax, unionMemberType.RawStringValue, accessedSymbol, accessedBodyType);
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
                this.FlagIfPropertyNotReadableAtDeployTime(syntax, syntax.PropertyName.IdentifierName, accessedSymbol, accessedBodyType);
            }

            base.VisitPropertyAccessSyntax(syntax);
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            this.FlagIfAccessingEntireResourceOrModule(syntax);
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

        private void FlagIfAccessingEntireResourceOrModule(SyntaxBase syntax)
        {
            if (this.DeployTimeConstantContainer is ObjectPropertySyntax property &&
                property.TryGetTypeProperty(this.SemanticModel) is { } typeProperty &&
                typeProperty.TypeReference is ResourceParentType or ResourceScopeType)
            {
                switch (this.SemanticModel.Binder.GetParent(syntax))
                {
                    case not PropertyAccessSyntax and not ArrayAccessSyntax when
                        this.ResourceTypeResolver.TryResolveRuntimeExistingResourceSymbolAndBodyType(syntax) is ({ } resourceSymbol, { } resourceType):
                        {
                            this.FlagDeployTimeConstantViolation(syntax, resourceSymbol, resourceType);

                            return;
                        }
                    case ArrayAccessSyntax arrayAccessSyntax when
                        arrayAccessSyntax.BaseExpression == syntax &&
                        this.SemanticModel.Binder.GetParent(arrayAccessSyntax) is not PropertyAccessSyntax and not ArrayAccessSyntax &&
                        this.ResourceTypeResolver.TryResolveRuntimeExistingResourceSymbolAndBodyType(arrayAccessSyntax) is ({ } resourceSymbol, { } resourceType):
                        {
                            var arrayIndexExprType = this.SemanticModel.GetTypeInfo(arrayAccessSyntax.IndexExpression);
                            if (arrayIndexExprType.IsIntegerOrIntegerLiteral())
                            {
                                this.FlagDeployTimeConstantViolation(syntax, resourceSymbol, resourceType);
                            }
                            return;
                        }

                    default:
                        return;
                }
            }

            if (this.DeployTimeConstantContainer is not IfConditionSyntax and not ForSyntax)
            {
                // We can skip validation if we are inside a resource/module body or a function call, because the
                // type checker should be able to produce type errors if the reference is invalid. There are also
                // locations where referencing an entire resource/module module body is expected (e.g., scope).
                return;
            }

            switch (this.SemanticModel.Binder.GetParent(syntax))
            {
                // var foo = [for x in [...]: {
                //   bar: myVM <-- accessing an entire resource/module.
                // }]
                case not PropertyAccessSyntax and not ArrayAccessSyntax when
                    this.ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(syntax) is ({ } accessedSymbol, { } accessedBodyType):
                    {
                        this.FlagDeployTimeConstantViolation(syntax, accessedSymbol, accessedBodyType);

                        return;
                    }
                // var foo = [for x in [...]: {
                //   bar: myVNets[1] <-- accessing an entire resource/module via an array index.
                // }]
                case ArrayAccessSyntax arrayAccessSyntax when
                    arrayAccessSyntax.BaseExpression == syntax && // need this condition because this case is hit both when syntax is myVNets (variable access) or the index expression
                    this.SemanticModel.Binder.GetParent(arrayAccessSyntax) is not PropertyAccessSyntax and not ArrayAccessSyntax &&
                    this.ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(arrayAccessSyntax) is ({ } accessedSymbol, { } accessedBodyType):
                    {
                        var arrayIndexExprType = this.SemanticModel.GetTypeInfo(arrayAccessSyntax.IndexExpression);
                        if (arrayIndexExprType.IsIntegerOrIntegerLiteral()
                            || arrayIndexExprType.TypeKind == TypeKind.Any
                            || (arrayIndexExprType is UnionType indexUnionType && indexUnionType.Members.All(m => m.Type.IsIntegerOrIntegerLiteral())))
                        {
                            this.FlagDeployTimeConstantViolation(syntax, accessedSymbol, accessedBodyType);
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
                this.FlagDeployTimeConstantViolation(syntax, accessedSymbol, accessedBodyType, violatingPropertyName: propertyName);
            }
        }

        protected void FlagIfFunctionRequiresInlining(FunctionCallSyntaxBase syntax)
        {
            if (this.SemanticModel.GetSymbolInfo(syntax) is FunctionSymbol functionSymbol &&
                functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining))
            {
                FlagDeployTimeConstantViolation(syntax);
            }
        }
    }
}
