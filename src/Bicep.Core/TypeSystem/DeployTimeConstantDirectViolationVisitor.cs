// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;

namespace Bicep.Core.TypeSystem
{
    public class DeployTimeConstantDirectViolationVisitor : DeployTimeConstantViolationVisitor
    {
        public DeployTimeConstantDirectViolationVisitor(SyntaxBase deployTimeConstantContainer, SemanticModel semanticModel, IDiagnosticWriter diagnosticWriter)
            : base(deployTimeConstantContainer, semanticModel, diagnosticWriter)
        {
        }

        public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            if (this.TryExtractResourceOrModuleSymbolAndBodyType(syntax.BaseExpression) is ({ } accessedSymbol, { } accessedBodyType) &&
                syntax.IndexExpression is StringSyntax stringSyntax)
            {
                if (stringSyntax.TryGetLiteralValue() is { } propertyName) {
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
            if (this.TryExtractResourceOrModuleSymbolAndBodyType(syntax.BaseExpression) is ({ } accessedSymbol, { } accessedBodyType))
            {
                this.FlagIfPropertyNotReadableAtDeployTime(syntax, syntax.PropertyName.IdentifierName, accessedSymbol, accessedBodyType);
            }

            base.VisitPropertyAccessSyntax(syntax);
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

        private void FlagIfPropertyNotReadableAtDeployTime(SyntaxBase errorSyntax, string propertyName, DeclaredSymbol accessedSymbol, ObjectType accessedBodyType)
        {
            if (accessedBodyType.Properties.TryGetValue(propertyName, out var propertyType) &&
                !propertyType.Flags.HasFlag(TypePropertyFlags.ReadableAtDeployTime))
            {
                this.FlagDeployTimeConstantViolation(errorSyntax, accessedSymbol, accessedBodyType);
            }
        }

        protected void FlagIfFunctionRequiresInlining(FunctionCallSyntaxBase syntax)
        {
            if (this.SemanticModel.GetSymbolInfo(syntax) is FunctionSymbol { FunctionFlags: FunctionFlags.RequiresInlining })
            {
                FlagDeployTimeConstantViolation(syntax);
            }
        }
    }
}
