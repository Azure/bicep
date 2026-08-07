// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

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
            if (this.ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(
                SyntaxHelper.UnwrapNonNullAssertion(syntax.BaseExpression)) is ({ } accessedSymbol, { } accessedBodyType))
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
            if (this.ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(
                SyntaxHelper.UnwrapNonNullAssertion(syntax.BaseExpression)) is ({ } accessedSymbol, { } accessedBodyType))
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
            var functionSymbol = this.SemanticModel.GetSymbolInfo(syntax) as FunctionSymbol;

            FlagIfFunctionRequiresInlining(functionSymbol, syntax);
            if (ShouldVisitFunctionArguments(functionSymbol))
            {
                base.VisitFunctionCallSyntax(syntax);
            }
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            var functionSymbol = this.SemanticModel.GetSymbolInfo(syntax) as FunctionSymbol;

            FlagIfFunctionRequiresInlining(functionSymbol, syntax);
            if (ShouldVisitFunctionArguments(functionSymbol))
            {
                base.VisitInstanceFunctionCallSyntax(syntax);
            }
        }

        private void FlagIfAccessingEntireResourceOrModule(SyntaxBase syntax)
        {
            var (parent, immediateChild) = GetParentAndChildIgnoringNonNullAssertions(syntax);
            if (this.DeployTimeConstantContainer is ObjectPropertySyntax property &&
                property.TryGetTypeProperty(this.SemanticModel) is { } typeProperty &&
                typeProperty.TypeReference is ResourceParentType or ResourceScopeType)
            {
                switch (parent)
                {
                    case not PropertyAccessSyntax and not ArrayAccessSyntax when
                        this.ResourceTypeResolver.TryResolveRuntimeExistingResourceSymbolAndBodyType(syntax) is ({ } resourceSymbol, { } resourceType):
                        {
                            this.FlagDeployTimeConstantViolation(immediateChild, resourceSymbol, resourceType);

                            return;
                        }
                    case ArrayAccessSyntax arrayAccessSyntax when
                        arrayAccessSyntax.BaseExpression == immediateChild &&
                        this.SemanticModel.Binder.GetParent(arrayAccessSyntax) is not PropertyAccessSyntax and not ArrayAccessSyntax &&
                        this.ResourceTypeResolver.TryResolveRuntimeExistingResourceSymbolAndBodyType(arrayAccessSyntax) is ({ } resourceSymbol, { } resourceType):
                        {
                            var arrayIndexExprType = this.SemanticModel.GetTypeInfo(arrayAccessSyntax.IndexExpression);
                            if (arrayIndexExprType.IsIntegerOrIntegerLiteral())
                            {
                                this.FlagDeployTimeConstantViolation(immediateChild, resourceSymbol, resourceType);
                            }
                            return;
                        }

                    default:
                        return;
                }
            }

            switch (parent)
            {
                // var foo = [for x in [...]: {
                //   bar: myVM <-- accessing an entire resource/module.
                // }]
                case not PropertyAccessSyntax and not ArrayAccessSyntax when
                    this.ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(immediateChild) is ({ } accessedSymbol, { } accessedBodyType):
                    {
                        this.FlagDeployTimeConstantViolation(immediateChild, accessedSymbol, accessedBodyType);

                        return;
                    }
                // var foo = [for x in [...]: {
                //   bar: myVNets[1] <-- accessing an entire resource/module via an array index.
                // }]
                case ArrayAccessSyntax arrayAccessSyntax when
                    arrayAccessSyntax.BaseExpression == immediateChild && // need this condition because this case is hit both when syntax is myVNets (variable access) or the index expression
                    this.SemanticModel.Binder.GetParent(arrayAccessSyntax) is not PropertyAccessSyntax and not ArrayAccessSyntax &&
                    this.ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(arrayAccessSyntax) is ({ } accessedSymbol, { } accessedBodyType):
                    {
                        var arrayIndexExprType = this.SemanticModel.GetTypeInfo(arrayAccessSyntax.IndexExpression);
                        if (arrayIndexExprType.IsIntegerOrIntegerLiteral()
                            || arrayIndexExprType.TypeKind == TypeKind.Any
                            || (arrayIndexExprType is UnionType indexUnionType && indexUnionType.Members.All(m => m.Type.IsIntegerOrIntegerLiteral())))
                        {
                            this.FlagDeployTimeConstantViolation(immediateChild, accessedSymbol, accessedBodyType);
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

        protected bool ShouldVisitFunctionArguments(FunctionSymbol? functionSymbol)
            => functionSymbol is null || !functionSymbol.FunctionFlags.HasFlag(FunctionFlags.IsArgumentValueIndependent);

        protected void FlagIfFunctionRequiresInlining(FunctionSymbol? functionSymbol, FunctionCallSyntaxBase syntax)
        {
            if (functionSymbol is { } && functionSymbol.FunctionFlags.HasFlag(FunctionFlags.RequiresInlining))
            {
                FlagDeployTimeConstantViolation(syntax);
            }
        }
    }
}
