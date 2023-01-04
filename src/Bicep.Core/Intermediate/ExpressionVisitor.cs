// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bicep.Core.Intermediate;

public abstract class ExpressionVisitor : IExpressionVisitor
{
    public void VisitArrayAccessExpression(ArrayAccessExpression expression)
    {
        Visit(expression.Access);
        Visit(expression.Base);
    }

    public void VisitArrayExpression(ArrayExpression expression)
    {
        VisitMultiple(expression.Items);
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
        VisitMultiple(expression.Parameters);
    }

    public void VisitIntegerLiteralExpression(IntegerLiteralExpression expression)
    {
    }

    public void VisitInterpolatedStringExpression(InterpolatedStringExpression expression)
    {
        VisitMultiple(expression.Expressions);
    }

    public void VisitLambdaExpression(LambdaExpression expression)
    {
        Visit(expression.Body);
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
        VisitMultiple(expression.Properties);
    }

    public void VisitObjectPropertyExpression(ObjectPropertyExpression expression)
    {
        Visit(expression.Key);
        Visit(expression.Value);
    }

    public void VisitParametersReferenceExpression(ParametersReferenceExpression expression)
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
        Visit(expression.DefaultValue);
    }

    public void VisitDeclaredVariableExpression(DeclaredVariableExpression expression)
    {
        Visit(expression.Value);
    }

    public void VisitDeclaredOutputExpression(DeclaredOutputExpression expression)
    {
        Visit(expression.Value);
    }

    public void VisitProgramExpression(ProgramExpression expression)
    {
        VisitMultiple(expression.Metadata);
        VisitMultiple(expression.Imports);
        VisitMultiple(expression.Parameters);
        VisitMultiple(expression.Variables);
        VisitMultiple(expression.Outputs);
    }

    public void Visit(Expression? expression)
    {
        if (expression is null)
        {
            return;
        }

        RuntimeHelpers.EnsureSufficientExecutionStack();

        expression.Accept(this);
    }

    protected void VisitMultiple(IEnumerable<Expression> expressions)
    {
        foreach (var expression in expressions)
        {
            this.Visit(expression);
        }
    }
}
