// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Bicep.Core.Intermediate;

public abstract class ExpressionRewriteVisitor : IExpressionVisitor
{
    void IExpressionVisitor.VisitAccessChainExpression(AccessChainExpression expression) => ReplaceCurrent(expression, ReplaceAccessChainExpression);
    public virtual Expression ReplaceAccessChainExpression(AccessChainExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.FirstLink, out var firstLink) |
            TryRewriteStrict(expression.AdditionalProperties, out var additionalProperties);

        return hasChanges ? expression with { FirstLink = firstLink, AdditionalProperties = additionalProperties } : expression;
    }

    void IExpressionVisitor.VisitArrayAccessExpression(ArrayAccessExpression expression) => ReplaceCurrent(expression, ReplaceArrayAccessExpression);
    public virtual Expression ReplaceArrayAccessExpression(ArrayAccessExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Base, out var @base) |
            TryRewrite(expression.Access, out var access);

        return hasChanges ? expression with { Base = @base, Access = access } : expression;
    }

    void IExpressionVisitor.VisitArrayExpression(ArrayExpression expression) => ReplaceCurrent(expression, ReplaceArrayExpression);
    public virtual Expression ReplaceArrayExpression(ArrayExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Items, out var items);

        return hasChanges ? expression with { Items = items } : expression;
    }

    void IExpressionVisitor.VisitBinaryExpression(BinaryExpression expression) => ReplaceCurrent(expression, ReplaceBinaryExpression);
    public virtual Expression ReplaceBinaryExpression(BinaryExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Left, out var left) |
            TryRewrite(expression.Right, out var right);

        return hasChanges ? expression with { Left = left, Right = right } : expression;
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

    void IExpressionVisitor.VisitForLoopExpression(ForLoopExpression expression) => ReplaceCurrent(expression, ReplaceForLoopExpression);
    public virtual Expression ReplaceForLoopExpression(ForLoopExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Expression, out var newExpression) |
            TryRewrite(expression.Body, out var body);

        return hasChanges ? expression with { Expression = newExpression, Body = body } : expression;
    }

    void IExpressionVisitor.VisitConditionExpression(ConditionExpression expression) => ReplaceCurrent(expression, ReplaceConditionExpression);
    public virtual Expression ReplaceConditionExpression(ConditionExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Expression, out var newExpression) |
            TryRewrite(expression.Body, out var body);

        return hasChanges ? expression with { Expression = newExpression, Body = body } : expression;
    }

    void IExpressionVisitor.VisitFunctionCallExpression(FunctionCallExpression expression) => ReplaceCurrent(expression, ReplaceFunctionCallExpression);
    public virtual Expression ReplaceFunctionCallExpression(FunctionCallExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Parameters, out var parameters);

        return hasChanges ? expression with { Parameters = parameters } : expression;
    }

    void IExpressionVisitor.VisitResourceFunctionCallExpression(ResourceFunctionCallExpression expression) => ReplaceCurrent(expression, ReplaceResourceFunctionCallExpression);
    public virtual Expression ReplaceResourceFunctionCallExpression(ResourceFunctionCallExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.Resource, out var resource) |
            TryRewrite(expression.Parameters, out var parameters);

        return hasChanges ? expression with { Resource = resource, Parameters = parameters } : expression;
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

        return hasChanges ? expression with { Expressions = expressions } : expression;
    }

    void IExpressionVisitor.VisitLambdaExpression(LambdaExpression expression) => ReplaceCurrent(expression, ReplaceLambdaExpression);
    public virtual Expression ReplaceLambdaExpression(LambdaExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Body, out var body);

        return hasChanges ? expression with { Body = body } : expression;
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

        return hasChanges ? expression with { Base = @base } : expression;
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
        var hasChanges =
            TryRewriteStrict(expression.Properties, out var properties);

        return hasChanges ? expression with { Properties = properties } : expression;
    }

    void IExpressionVisitor.VisitObjectPropertyExpression(ObjectPropertyExpression expression) => ReplaceCurrent(expression, ReplaceObjectPropertyExpression);
    public virtual Expression ReplaceObjectPropertyExpression(ObjectPropertyExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Key, out var key) |
            TryRewrite(expression.Value, out var value);

        return hasChanges ? expression with { Key = key, Value = value } : expression;
    }

    void IExpressionVisitor.VisitParametersReferenceExpression(ParametersReferenceExpression expression) => ReplaceCurrent(expression, ReplaceParametersReferenceExpression);
    public virtual Expression ReplaceParametersReferenceExpression(ParametersReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitParametersAssignmentReferenceExpression(ParametersAssignmentReferenceExpression expression) => ReplaceCurrent(expression, ReplaceParametersAssignmentReferenceExpression);
    public virtual Expression ReplaceParametersAssignmentReferenceExpression(ParametersAssignmentReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitPropertyAccessExpression(PropertyAccessExpression expression) => ReplaceCurrent(expression, ReplacePropertyAccessExpression);
    public virtual Expression ReplacePropertyAccessExpression(PropertyAccessExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Base, out var @base);

        return hasChanges ? expression with { Base = @base } : expression;
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

    void IExpressionVisitor.VisitTernaryExpression(TernaryExpression expression) => ReplaceCurrent(expression, ReplaceTernaryExpression);
    public virtual Expression ReplaceTernaryExpression(TernaryExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Condition, out var condition) |
            TryRewrite(expression.True, out var @true) |
            TryRewrite(expression.False, out var @false);

        return hasChanges ? expression with { Condition = condition, True = @true, False = @false } : expression;
    }

    void IExpressionVisitor.VisitUnaryExpression(UnaryExpression expression) => ReplaceCurrent(expression, ReplaceUnaryExpression);
    public virtual Expression ReplaceUnaryExpression(UnaryExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Expression, out var newExpression);

        return hasChanges ? expression with { Expression = newExpression } : expression;
    }

    void IExpressionVisitor.VisitVariableReferenceExpression(VariableReferenceExpression expression) => ReplaceCurrent(expression, ReplaceVariableReferenceExpression);
    public virtual Expression ReplaceVariableReferenceExpression(VariableReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitSynthesizedVariableReferenceExpression(SynthesizedVariableReferenceExpression expression) => ReplaceCurrent(expression, ReplaceSynthesizedVariableReferenceExpression);
    public virtual Expression ReplaceSynthesizedVariableReferenceExpression(SynthesizedVariableReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitDeclaredMetadataExpression(DeclaredMetadataExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredMetadataExpression);
    public virtual Expression ReplaceDeclaredMetadataExpression(DeclaredMetadataExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Value, out var value);

        return hasChanges ? expression with { Value = value } : expression;
    }

    void IExpressionVisitor.VisitDeclaredImportExpression(DeclaredImportExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredImportExpression);
    public virtual Expression ReplaceDeclaredImportExpression(DeclaredImportExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Config, out var config);

        return hasChanges ? expression with { Config = config } : expression;
    }

    void IExpressionVisitor.VisitDeclaredParameterExpression(DeclaredParameterExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredParameterExpression);
    public virtual Expression ReplaceDeclaredParameterExpression(DeclaredParameterExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.DefaultValue, out var defaultValue);

        return hasChanges ? expression with { DefaultValue = defaultValue } : expression;
    }

    void IExpressionVisitor.VisitDeclaredVariableExpression(DeclaredVariableExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredVariableExpression);
    public virtual Expression ReplaceDeclaredVariableExpression(DeclaredVariableExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Value, out var value);

        return hasChanges ? expression with { Value = value } : expression;
    }

    void IExpressionVisitor.VisitDeclaredOutputExpression(DeclaredOutputExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredOutputExpression);
    public virtual Expression ReplaceDeclaredOutputExpression(DeclaredOutputExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Value, out var value);

        return hasChanges ? expression with { Value = value } : expression;
    }

    void IExpressionVisitor.VisitDeclaredResourceExpression(DeclaredResourceExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredResourceExpression);
    public virtual Expression ReplaceDeclaredResourceExpression(DeclaredResourceExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Body, out var body) |
            TryRewriteStrict(expression.DependsOn, out var dependsOn);

        return hasChanges ? expression with { Body = body, DependsOn = dependsOn } : expression;
    }

    void IExpressionVisitor.VisitDeclaredModuleExpression(DeclaredModuleExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredModuleExpression);
    public virtual Expression ReplaceDeclaredModuleExpression(DeclaredModuleExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Body, out var body) |
            TryRewrite(expression.Parameters, out var parameters) |
            TryRewriteStrict(expression.DependsOn, out var dependsOn);

        return hasChanges ? expression with { Body = body, Parameters = parameters, DependsOn = dependsOn } : expression;
    }

    void IExpressionVisitor.VisitResourceDependencyExpression(ResourceDependencyExpression expression) => ReplaceCurrent(expression, ReplaceResourceDependencyExpression);
    public virtual Expression ReplaceResourceDependencyExpression(ResourceDependencyExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Reference, out var reference);

        return hasChanges ? expression with { Reference = reference } : expression;
    }

    void IExpressionVisitor.VisitProgramExpression(ProgramExpression expression) => ReplaceCurrent(expression, ReplaceProgramExpression);
    public virtual Expression ReplaceProgramExpression(ProgramExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.Metadata, out var metadata) |
            TryRewriteStrict(expression.Imports, out var imports) |
            TryRewriteStrict(expression.Parameters, out var parameters) |
            TryRewriteStrict(expression.Variables, out var variables) |
            TryRewriteStrict(expression.Resources, out var resources) |
            TryRewriteStrict(expression.Modules, out var modules) |
            TryRewriteStrict(expression.Outputs, out var outputs);

        return hasChanges ? expression with { Metadata = metadata, Imports = imports, Parameters = parameters, Variables = variables, Resources = resources, Modules = modules, Outputs = outputs } : expression;
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

    private bool TryRewriteStrict<TExpression>(TExpression? expression, [NotNullIfNotNull("expression")] out TExpression? newExpression)
        where TExpression : Expression
    {
        if (expression is null)
        {
            newExpression = null;
            return false;
        }

        var newExpressionUntyped = Replace(expression);
        var hasChanges = !object.ReferenceEquals(expression, newExpressionUntyped);

        if (newExpressionUntyped is not TExpression newExpressionTyped)
        {
            throw new InvalidOperationException($"Expected {nameof(newExpression)} to be of type {typeof(TExpression)}");
        }

        newExpression = newExpressionTyped;
        return hasChanges;
    }

    private bool TryRewrite(Expression? expression, [NotNullIfNotNull("expression")] out Expression? rewritten)
        => TryRewriteStrict<Expression>(expression, out rewritten);

    private bool TryRewriteStrict<TExpression>(ImmutableArray<TExpression> expressions, out ImmutableArray<TExpression> newExpressions)
        where TExpression : Expression
    {
        var hasChanges = false;
        var newExpressionList = new List<TExpression>();
        foreach (var expression in expressions)
        {
            hasChanges |= TryRewriteStrict(expression, out var newExpression);
            newExpressionList.Add(newExpression);
        }

        newExpressions = hasChanges ? newExpressionList.ToImmutableArray() : expressions;
        return hasChanges;
    }

    private bool TryRewrite(ImmutableArray<Expression> expressions, out ImmutableArray<Expression> newExpressions)
        => TryRewriteStrict(expressions, out newExpressions);

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
