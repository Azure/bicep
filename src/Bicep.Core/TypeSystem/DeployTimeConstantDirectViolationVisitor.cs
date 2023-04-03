// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

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
            if (syntax.IndexExpression is StringSyntax stringSyntax &&
                this.ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(syntax.BaseExpression) is ({ } accessedSymbol, { } accessedBodyType))
            {
                if (stringSyntax.TryGetLiteralValue() is { } propertyName)
                {
                    // Validate property access via string literal index (myResource['sku']).
                    this.FlagIfPropertyNotReadableAtDeployTime(syntax, propertyName, accessedSymbol, accessedBodyType);
                }
                else
                {
                    // Block property access via interpolated string index (myResource['${myParam}']),
                    // since we we cannot tell whether the property is readable at deploy-time or not.
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
                    case ArrayAccessSyntax { IndexExpression: IntegerLiteralSyntax } arrayAccessSyntax when
                        this.SemanticModel.Binder.GetParent(arrayAccessSyntax) is not PropertyAccessSyntax and not ArrayAccessSyntax &&
                        this.ResourceTypeResolver.TryResolveRuntimeExistingResourceSymbolAndBodyType(arrayAccessSyntax) is ({ } resourceSymbol, { } resourceType):
                        {
                            this.FlagDeployTimeConstantViolation(syntax, resourceSymbol, resourceType);

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
                case ArrayAccessSyntax { IndexExpression: IntegerLiteralSyntax } arrayAccessSyntax when
                    this.SemanticModel.Binder.GetParent(arrayAccessSyntax) is not PropertyAccessSyntax and not ArrayAccessSyntax &&
                    this.ResourceTypeResolver.TryResolveResourceOrModuleSymbolAndBodyType(arrayAccessSyntax) is ({ } accessedSymbol, { } accessedBodyType):
                    {
                        this.FlagDeployTimeConstantViolation(syntax, accessedSymbol, accessedBodyType);

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
                this.FlagDeployTimeConstantViolation(syntax, accessedSymbol, accessedBodyType);
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
