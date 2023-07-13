// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bicep.Core.Intermediate;

public abstract class ExpressionVisitor : IExpressionVisitor
{
    public void VisitAccessChainExpression(AccessChainExpression expression)
    {
        Visit(expression.FirstLink);
        Visit(expression.AdditionalProperties);
    }

    public void VisitArrayAccessExpression(ArrayAccessExpression expression)
    {
        Visit(expression.Access);
        Visit(expression.Base);
    }

    public void VisitArrayExpression(ArrayExpression expression)
    {
        Visit(expression.Items);
    }

    public void VisitBinaryExpression(BinaryExpression expression)
    {
        Visit(expression.Left);
        Visit(expression.Right);
    }

    public void VisitBooleanLiteralExpression(BooleanLiteralExpression expression)
    {
    }

    public void VisitCopyIndexExpression(CopyIndexExpression expression)
    {
    }

    public void VisitForLoopExpression(ForLoopExpression expression)
    {
        Visit(expression.Expression);
        Visit(expression.Body);
    }

    public void VisitConditionExpression(ConditionExpression expression)
    {
        Visit(expression.Expression);
        Visit(expression.Body);
    }

    public void VisitFunctionCallExpression(FunctionCallExpression expression)
    {
        Visit(expression.Parameters);
    }

    public void VisitResourceFunctionCallExpression(ResourceFunctionCallExpression expression)
    {
        Visit(expression.Resource);
        Visit(expression.Parameters);
    }

    public void VisitIntegerLiteralExpression(IntegerLiteralExpression expression)
    {
    }

    public void VisitInterpolatedStringExpression(InterpolatedStringExpression expression)
    {
        Visit(expression.Expressions);
    }

    public void VisitLambdaExpression(LambdaExpression expression)
    {
        Visit(expression.ParameterTypes);
        Visit(expression.Body);
        Visit(expression.OutputType);
    }

    public void VisitLambdaVariableReferenceExpression(LambdaVariableReferenceExpression expression)
    {
    }

    public void VisitModuleOutputPropertyAccessExpression(ModuleOutputPropertyAccessExpression expression)
    {
        Visit(expression.Base);
    }

    public void VisitModuleReferenceExpression(ModuleReferenceExpression expression)
    {
    }

    public void VisitNullLiteralExpression(NullLiteralExpression expression)
    {
    }

    public void VisitObjectExpression(ObjectExpression expression)
    {
        Visit(expression.Properties);
    }

    public void VisitObjectPropertyExpression(ObjectPropertyExpression expression)
    {
        Visit(expression.Key);
        Visit(expression.Value);
    }

    public void VisitParametersReferenceExpression(ParametersReferenceExpression expression)
    {
    }

    public void VisitParametersAssignmentReferenceExpression(ParametersAssignmentReferenceExpression expression)
    {
    }

    public void VisitPropertyAccessExpression(PropertyAccessExpression expression)
    {
        Visit(expression.Base);
    }

    public void VisitResourceReferenceExpression(ResourceReferenceExpression expression)
    {
    }

    public void VisitStringLiteralExpression(StringLiteralExpression expression)
    {
    }

    public void VisitTernaryExpression(TernaryExpression expression)
    {
        Visit(expression.Condition);
        Visit(expression.True);
        Visit(expression.False);
    }

    public void VisitUnaryExpression(UnaryExpression expression)
    {
        Visit(expression.Expression);
    }

    public void VisitVariableReferenceExpression(VariableReferenceExpression expression)
    {
    }

    public void VisitSynthesizedVariableReferenceExpression(SynthesizedVariableReferenceExpression expression)
    {
    }

    public void VisitDeclaredMetadataExpression(DeclaredMetadataExpression expression)
    {
        Visit(expression.Value);
    }

    public void VisitDeclaredImportExpression(DeclaredImportExpression expression)
    {
        Visit(expression.Config);
    }

    public void VisitDeclaredParameterExpression(DeclaredParameterExpression expression)
    {
        Visit(expression.Type);
        Visit(expression.DefaultValue);
    }

    public void VisitDeclaredVariableExpression(DeclaredVariableExpression expression)
    {
        Visit(expression.Value);
    }

    public void VisitDeclaredOutputExpression(DeclaredOutputExpression expression)
    {
        Visit(expression.Type);
        Visit(expression.Value);
    }

    public void VisitDeclaredResourceExpression(DeclaredResourceExpression expression)
    {
        Visit(expression.Body);
        Visit(expression.DependsOn);
    }

    public void VisitDeclaredModuleExpression(DeclaredModuleExpression expression)
    {
        Visit(expression.Body);
        Visit(expression.Parameters);
        Visit(expression.DependsOn);
    }

    public void VisitResourceDependencyExpression(ResourceDependencyExpression expression)
    {
        Visit(expression.Reference);
    }

    public void VisitDeclaredFunctionExpression(DeclaredFunctionExpression expression)
    {
        Visit(expression.Lambda);
    }

    public void VisitUserDefinedFunctionCallExpression(UserDefinedFunctionCallExpression expression)
    {
        Visit(expression.Parameters);
    }

    public void VisitDeclaredTypeExpression(DeclaredTypeExpression expression)
    {
        Visit(expression.Value);
    }

    public void VisitAmbientTypeReferenceExpression(AmbientTypeReferenceExpression expression)
    {
    }

    public void VisitFullyQualifiedAmbientTypeReferenceExpression(FullyQualifiedAmbientTypeReferenceExpression expression)
    {
    }

    public void VisitTypeAliasReferenceExpression(TypeAliasReferenceExpression expression)
    {
    }

    public void VisitStringLiteralTypeExpression(StringLiteralTypeExpression expression)
    {
    }

    public void VisitIntegerLiteralTypeExpression(IntegerLiteralTypeExpression expression)
    {
    }

    public void VisitBooleanLiteralTypeExpression(BooleanLiteralTypeExpression expression)
    {
    }

    public void VisitNullLiteralTypeExpression(NullLiteralTypeExpression expression)
    {
    }

    public void VisitResourceTypeExpression(ResourceTypeExpression expression)
    {
    }

    public void VisitObjectTypePropertyExpression(ObjectTypePropertyExpression expression)
    {
        Visit(expression.Value);
    }

    public void VisitObjectTypeAdditionalPropertiesExpression(ObjectTypeAdditionalPropertiesExpression expression)
    {
        Visit(expression.Value);
    }

    public void VisitObjectTypeExpression(ObjectTypeExpression expression)
    {
        Visit(expression.PropertyExpressions);
        Visit(expression.AdditionalPropertiesExpression);
    }

    public void VisitTupleTypeItemExpression(TupleTypeItemExpression expression)
    {
        Visit(expression.Value);
    }

    public void VisitTupleTypeExpression(TupleTypeExpression expression)
    {
        Visit(expression.ItemExpressions);
    }

    public void VisitArrayTypeExpression(ArrayTypeExpression expression)
    {
        Visit(expression.BaseExpression);
    }

    public void VisitNullableTypeExpression(NullableTypeExpression expression)
    {
        Visit(expression.BaseExpression);
    }

    public void VisitNonNullableTypeExpression(NonNullableTypeExpression expression)
    {
        Visit(expression.BaseExpression);
    }

    public void VisitUnionTypeExpression(UnionTypeExpression expression)
    {
        Visit(expression.MemberExpressions);
    }

    public void VisitProgramExpression(ProgramExpression expression)
    {
        Visit(expression.Metadata);
        Visit(expression.Imports);
        Visit(expression.Types);
        Visit(expression.Parameters);
        Visit(expression.Variables);
        Visit(expression.Functions);
        Visit(expression.Resources);
        Visit(expression.Modules);
        Visit(expression.Outputs);
    }

    public void Visit(Expression? expression)
    {
        if (expression is null)
        {
            return;
        }

        RuntimeHelpers.EnsureSufficientExecutionStack();

        VisitInternal(expression);
    }

    protected void Visit(IEnumerable<Expression> expressions)
    {
        foreach (var expression in expressions)
        {
            this.Visit(expression);
        }
    }

    protected virtual void VisitInternal(Expression expression)
    {
        expression.Accept(this);
    }
}
