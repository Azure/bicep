// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Bicep.Core.Intermediate;

public abstract class ExpressionRewriteVisitor : IExpressionVisitor
{
    void IExpressionVisitor.VisitArrayAccessExpression(ArrayAccessExpression expression) => ReplaceCurrent(expression, ReplaceArrayAccessExpression);
    public virtual Expression ReplaceArrayAccessExpression(ArrayAccessExpression expression)
    {
        var hasChanges = 
            TryRewrite(expression.Base, out var @base) |
            TryRewrite(expression.Access, out var access);

        return hasChanges ? new(@base, access) : expression;
    }

    void IExpressionVisitor.VisitArrayExpression(ArrayExpression expression) => ReplaceCurrent(expression, ReplaceArrayExpression);
    public virtual Expression ReplaceArrayExpression(ArrayExpression expression)
    {
        var hasChanges = 
            TryRewrite(expression.Items, out var items);

        return hasChanges ? new(items) : expression;
    }

    void IExpressionVisitor.VisitBinaryExpression(BinaryExpression expression) => ReplaceCurrent(expression, ReplaceBinaryExpression);
    public virtual Expression ReplaceBinaryExpression(BinaryExpression expression)
    {
        var hasChanges = 
            TryRewrite(expression.Left, out var left) |
            TryRewrite(expression.Right, out var right);

        return hasChanges ? new(expression.Operator, left, right) : expression;
    }

    void IExpressionVisitor.VisitBooleanLiteralExpression(BooleanLiteralExpression expression) => ReplaceCurrent(expression, ReplaceBooleanLiteralExpression);
    public virtual Expression ReplaceBooleanLiteralExpression(BooleanLiteralExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitCopyIndexExpression(CopyIndexExpression expression) => ReplaceCurrent(expression, ReplaceCopyIndexExpression);
    public virtual Expression ReplaceCopyIndexExpression(CopyIndexExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitFunctionCallExpression(FunctionCallExpression expression) => ReplaceCurrent(expression, ReplaceFunctionCallExpression);
    public virtual Expression ReplaceFunctionCallExpression(FunctionCallExpression expression)
    {
        var hasChanges = 
            TryRewrite(expression.Parameters, out var parameters);

        return hasChanges ? new(expression.Name, parameters) : expression;
    }

    void IExpressionVisitor.VisitIntegerLiteralExpression(IntegerLiteralExpression expression) => ReplaceCurrent(expression, ReplaceIntegerLiteralExpression);
    public virtual Expression ReplaceIntegerLiteralExpression(IntegerLiteralExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitInterpolatedStringExpression(InterpolatedStringExpression expression) => ReplaceCurrent(expression, ReplaceInterpolatedStringExpression);
    public virtual Expression ReplaceInterpolatedStringExpression(InterpolatedStringExpression expression)
    {
        var hasChanges = 
            TryRewrite(expression.Expressions, out var expressions);

        return hasChanges ? new(expression.SegmentValues, expressions) : expression;
    }

    void IExpressionVisitor.VisitLambdaExpression(LambdaExpression expression) => ReplaceCurrent(expression, ReplaceLambdaExpression);
    public virtual Expression ReplaceLambdaExpression(LambdaExpression expression)
    {
        var hasChanges = 
            TryRewrite(expression.Body, out var body);

        return hasChanges ? new(expression.Parameters, body) : expression;
    }

    void IExpressionVisitor.VisitLambdaVariableReferenceExpression(LambdaVariableReferenceExpression expression) => ReplaceCurrent(expression, ReplaceLambdaVariableReferenceExpression);
    public virtual Expression ReplaceLambdaVariableReferenceExpression(LambdaVariableReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitModuleOutputPropertyAccessExpression(ModuleOutputPropertyAccessExpression expression) => ReplaceCurrent(expression, ReplaceModuleOutputPropertyAccessExpression);
    public virtual Expression ReplaceModuleOutputPropertyAccessExpression(ModuleOutputPropertyAccessExpression expression)
    {
        var hasChanges = 
            TryRewrite(expression.Base, out var @base);

        return hasChanges ? new(@base, expression.PropertyName) : expression;
    }

    void IExpressionVisitor.VisitModuleReferenceExpression(ModuleReferenceExpression expression) => ReplaceCurrent(expression, ReplaceModuleReferenceExpression);
    public virtual Expression ReplaceModuleReferenceExpression(ModuleReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitNullLiteralExpression(NullLiteralExpression expression) => ReplaceCurrent(expression, ReplaceNullLiteralExpression);
    public virtual Expression ReplaceNullLiteralExpression(NullLiteralExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitObjectExpression(ObjectExpression expression) => ReplaceCurrent(expression, ReplaceObjectExpression);
    public virtual Expression ReplaceObjectExpression(ObjectExpression expression)
    {
        var hasChanges = false;
        var newPropertyList = new List<ObjectProperty>();
        foreach (var property in expression.Properties)
        {
            var hasPropertyChanges = TryRewrite(property.Key, out var key) | TryRewrite(property.Value, out var value);
            hasChanges |= hasPropertyChanges;

            newPropertyList.Add(hasPropertyChanges ? new(key, value) : property);
        }

        var newProperties = hasChanges ? newPropertyList.ToImmutableArray() : expression.Properties;
        return hasChanges ? new(newProperties) : expression;
    }

    void IExpressionVisitor.VisitParametersReferenceExpression(ParametersReferenceExpression expression) => ReplaceCurrent(expression, ReplaceParametersReferenceExpression);
    public virtual Expression ReplaceParametersReferenceExpression(ParametersReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitPropertyAccessExpression(PropertyAccessExpression expression) => ReplaceCurrent(expression, ReplacePropertyAccessExpression);
    public virtual Expression ReplacePropertyAccessExpression(PropertyAccessExpression expression)
    {
        var hasChanges = 
            TryRewrite(expression.Base, out var @base);

        return hasChanges ? new(@base, expression.PropertyName) : expression;
    }

    void IExpressionVisitor.VisitResourceReferenceExpression(ResourceReferenceExpression expression) => ReplaceCurrent(expression, ReplaceResourceReferenceExpression);
    public virtual Expression ReplaceResourceReferenceExpression(ResourceReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitStringLiteralExpression(StringLiteralExpression expression) => ReplaceCurrent(expression, ReplaceStringLiteralExpression);
    public virtual Expression ReplaceStringLiteralExpression(StringLiteralExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitSyntaxExpression(SyntaxExpression expression) => ReplaceCurrent(expression, ReplaceSyntaxExpression);
    public virtual Expression ReplaceSyntaxExpression(SyntaxExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitTernaryExpression(TernaryExpression expression) => ReplaceCurrent(expression, ReplaceTernaryExpression);
    public virtual Expression ReplaceTernaryExpression(TernaryExpression expression)
    {
        var hasChanges = 
            TryRewrite(expression.Condition, out var condition) |
            TryRewrite(expression.True, out var @true) |
            TryRewrite(expression.False, out var @false);

        return hasChanges ? new(condition, @true, @false) : expression;
    }

    void IExpressionVisitor.VisitUnaryExpression(UnaryExpression expression) => ReplaceCurrent(expression, ReplaceUnaryExpression);
    public virtual Expression ReplaceUnaryExpression(UnaryExpression expression)
    {
        var hasChanges = 
            TryRewrite(expression.Expression, out var newExpression);

        return hasChanges ? new(expression.Operator, newExpression) : expression;
    }

    void IExpressionVisitor.VisitVariableReferenceExpression(VariableReferenceExpression expression) => ReplaceCurrent(expression, ReplaceVariableReferenceExpression);
    public virtual Expression ReplaceVariableReferenceExpression(VariableReferenceExpression expression)
    {
        return expression;
    }

    protected Expression Replace(Expression expression)
    {
        RuntimeHelpers.EnsureSufficientExecutionStack();

        current = null;
        expression.Accept(this);

        if (current is null)
        {
            throw new InvalidOperationException($"Expected {nameof(current)} to not be null");
        }

        return current;
    }

    private Expression? current;

    private bool TryRewrite(Expression expression, out Expression rewritten)
    {
        rewritten = Replace(expression);

        return !object.ReferenceEquals(expression, rewritten);
    }

    private bool TryRewrite(ImmutableArray<Expression> expressions, out ImmutableArray<Expression> newExpressions)
    {
        var hasChanges = false;
        var newExpressionList = new List<Expression>();
        foreach (var expression in expressions)
        {
            hasChanges |= TryRewrite(expression, out var newExpression);
            newExpressionList.Add(newExpression);
        }

        newExpressions = hasChanges ? newExpressionList.ToImmutableArray() : expressions;
        return hasChanges;
    }

    private void ReplaceCurrent<TExpression>(TExpression expression, Func<TExpression, Expression> replaceFunc)
        where TExpression : Expression
    {
        if (current is not null)
        {
            throw new InvalidOperationException($"Expected {nameof(current)} to be null");
        }

        current = replaceFunc(expression);
    }
}
