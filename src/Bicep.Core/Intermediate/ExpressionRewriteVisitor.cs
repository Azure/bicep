// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
            TryRewriteCollectionOfNullablesStrict(expression.ParameterTypes, out var parameterTypes) |
            TryRewrite(expression.Body, out var body) |
            TryRewriteStrict(expression.OutputType, out var outputType);

        return hasChanges ? expression with { Body = body, ParameterTypes = parameterTypes, OutputType = outputType } : expression;
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

    void IExpressionVisitor.VisitStackReferenceExpression(StackReferenceExpression expression) => ReplaceCurrent(expression, ReplaceStackReferenceExpression);
    public virtual Expression ReplaceStackReferenceExpression(StackReferenceExpression expression)
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
            TryRewrite(expression.Value, out var value) |
            TryRewriteDescription(expression, out var description);

        return hasChanges ? expression with { Value = value, Description = description } : expression;
    }

    void IExpressionVisitor.VisitExtensionExpression(ExtensionExpression expression) => ReplaceCurrent(expression, ReplaceExtensionExpression);
    public virtual Expression ReplaceExtensionExpression(ExtensionExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Config, out var config) |
            TryRewriteDescription(expression, out var description);

        return hasChanges ? expression with { Config = config, Description = description } : expression;
    }

    void IExpressionVisitor.VisitExtensionReferenceExpression(ExtensionReferenceExpression expression) => ReplaceCurrent(expression, ReplaceExtensionReferenceExpression);

    public virtual Expression ReplaceExtensionReferenceExpression(ExtensionReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitExtensionConfigAssignmentReferenceExpression(ExtensionConfigAssignmentReferenceExpression expression) => ReplaceCurrent(expression, ReplaceExtensionConfigAssignmentReferenceExpression);

    public virtual Expression ReplaceExtensionConfigAssignmentReferenceExpression(ExtensionConfigAssignmentReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitDeclaredParameterExpression(DeclaredParameterExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredParameterExpression);
    public virtual Expression ReplaceDeclaredParameterExpression(DeclaredParameterExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.DefaultValue, out var defaultValue) |
            TryRewriteStrict(expression.Type, out var type) |
            TryRewriteTypeDecorators(expression, out var description, out var metadata, out var secure, out var minLength, out var maxLength, out var minValue, out var maxValue, out var @sealed) |
            TryRewrite(expression.AllowedValues, out var allowedValues);

        return hasChanges
            ? expression with
            {
                DefaultValue = defaultValue,
                Type = type,
                Description = description,
                Metadata = metadata,
                Secure = secure,
                MinLength = minLength,
                MaxLength = maxLength,
                MinValue = minValue,
                MaxValue = maxValue,
                Sealed = @sealed,
                AllowedValues = allowedValues,
            }
            : expression;
    }

    void IExpressionVisitor.VisitDeclaredVariableExpression(DeclaredVariableExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredVariableExpression);
    public virtual Expression ReplaceDeclaredVariableExpression(DeclaredVariableExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Value, out var value) |
            TryRewriteStrict(expression.Type, out var type) |
            TryRewriteDescription(expression, out var description) |
            TryRewrite(expression.Exported, out var exported);

        return hasChanges ? expression with { Value = value, Type = type, Description = description, Exported = exported } : expression;
    }

    void IExpressionVisitor.VisitDeclaredFunctionExpression(DeclaredFunctionExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredFunctionExpression);
    public virtual Expression ReplaceDeclaredFunctionExpression(DeclaredFunctionExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Lambda, out var lambda) |
            TryRewriteDescription(expression, out var description) |
            TryRewrite(expression.Exported, out var exported);

        return hasChanges ? expression with { Lambda = lambda, Description = description, Exported = exported } : expression;
    }

    void IExpressionVisitor.VisitUserDefinedFunctionCallExpression(UserDefinedFunctionCallExpression expression) => ReplaceCurrent(expression, ReplaceUserDefinedFunctionCallExpression);
    public virtual Expression ReplaceUserDefinedFunctionCallExpression(UserDefinedFunctionCallExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Parameters, out var parameters);

        return hasChanges ? expression with { Parameters = parameters } : expression;
    }

    void IExpressionVisitor.VisitSynthesizedUserDefinedFunctionCallExpression(SynthesizedUserDefinedFunctionCallExpression expression) => ReplaceCurrent(expression, ReplaceSynthesizedUserDefinedFunctionCallExpression);
    public virtual Expression ReplaceSynthesizedUserDefinedFunctionCallExpression(SynthesizedUserDefinedFunctionCallExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Parameters, out var parameters);

        return hasChanges ? expression with { Parameters = parameters } : expression;
    }

    void IExpressionVisitor.VisitImportedUserDefinedFunctionCallExpression(ImportedUserDefinedFunctionCallExpression expression) => ReplaceCurrent(expression, ReplaceImportedUserDefinedFunctionCallExpression);
    public virtual Expression ReplaceImportedUserDefinedFunctionCallExpression(ImportedUserDefinedFunctionCallExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Parameters, out var parameters);

        return hasChanges ? expression with { Parameters = parameters } : expression;
    }

    void IExpressionVisitor.VisitWildcardImportInstanceFunctionCallExpression(WildcardImportInstanceFunctionCallExpression expression) => ReplaceCurrent(expression, ReplaceWildcardImportInstanceFunctionCallExpression);
    public virtual Expression ReplaceWildcardImportInstanceFunctionCallExpression(WildcardImportInstanceFunctionCallExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Parameters, out var parameters);

        return hasChanges ? expression with { Parameters = parameters } : expression;
    }

    void IExpressionVisitor.VisitDeclaredOutputExpression(DeclaredOutputExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredOutputExpression);
    public virtual Expression ReplaceDeclaredOutputExpression(DeclaredOutputExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Value, out var value) |
            TryRewriteStrict(expression.Type, out var type) |
            TryRewriteTypeDecorators(expression, out var description, out var metadata, out var secure, out var minLength, out var maxLength, out var minValue, out var maxValue, out var @sealed);

        return hasChanges
            ? expression with
            {
                Value = value,
                Type = type,
                Description = description,
                Metadata = metadata,
                Secure = secure,
                MinLength = minLength,
                MaxLength = maxLength,
                MinValue = minValue,
                MaxValue = maxValue,
                Sealed = @sealed,
            }
            : expression;
    }

    void IExpressionVisitor.VisitDeclaredAssertExpression(DeclaredAssertExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredAssertExpression);
    public virtual Expression ReplaceDeclaredAssertExpression(DeclaredAssertExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Value, out var value) |
            TryRewriteDescription(expression, out var description);

        return hasChanges ? expression with { Value = value, Description = description } : expression;
    }

    void IExpressionVisitor.VisitDeclaredResourceExpression(DeclaredResourceExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredResourceExpression);
    public virtual Expression ReplaceDeclaredResourceExpression(DeclaredResourceExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Body, out var body) |
            TryRewriteStrict(expression.DependsOn, out var dependsOn) |
            TryRewriteDescription(expression, out var description) |
            TryRewriteDictionaryStrict(expression.DecoratorConfig, out var decoratorConfig);

        return hasChanges ? expression with { Body = body, DependsOn = dependsOn, Description = description, DecoratorConfig = decoratorConfig } : expression;
    }

    void IExpressionVisitor.VisitDeclaredModuleExpression(DeclaredModuleExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredModuleExpression);
    public virtual Expression ReplaceDeclaredModuleExpression(DeclaredModuleExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Body, out var body) |
            TryRewrite(expression.Parameters, out var parameters) |
            TryRewrite(expression.ExtensionConfigs, out var extensionConfigs) |
            TryRewriteStrict(expression.DependsOn, out var dependsOn) |
            TryRewriteDescription(expression, out var description);

        return hasChanges ? expression with { Body = body, Parameters = parameters, ExtensionConfigs = extensionConfigs, DependsOn = dependsOn, Description = description } : expression;
    }

    void IExpressionVisitor.VisitDeclaredStackExpression(DeclaredStackExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredStackExpression);
    public virtual Expression ReplaceDeclaredStackExpression(DeclaredStackExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Body, out var body) |
            TryRewriteStrict(expression.DependsOn, out var dependsOn) |
            TryRewriteDescription(expression, out var description);

        return hasChanges ? expression with { Body = body, DependsOn = dependsOn, Description = description } : expression;
    }

    void IExpressionVisitor.VisitDeclaredRuleExpression(DeclaredRuleExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredRuleExpression);
    public virtual Expression ReplaceDeclaredRuleExpression(DeclaredRuleExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Body, out var body) |
            TryRewriteDescription(expression, out var description);

        return hasChanges ? expression with { Body = body, Description = description } : expression;
    }

    void IExpressionVisitor.VisitResourceDependencyExpression(ResourceDependencyExpression expression) => ReplaceCurrent(expression, ReplaceResourceDependencyExpression);
    public virtual Expression ReplaceResourceDependencyExpression(ResourceDependencyExpression expression)
    {
        var hasChanges =
            TryRewrite(expression.Reference, out var reference);

        return hasChanges ? expression with { Reference = reference } : expression;
    }

    void IExpressionVisitor.VisitDeclaredTypeExpression(DeclaredTypeExpression expression) => ReplaceCurrent(expression, ReplaceDeclaredTypeExpression);
    public virtual Expression ReplaceDeclaredTypeExpression(DeclaredTypeExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.Value, out var value) |
            TryRewriteTypeDecorators(expression, out var description, out var metadata, out var secure, out var minLength, out var maxLength, out var minValue, out var maxValue, out var @sealed) |
            TryRewrite(expression.Exported, out var exported);

        return hasChanges
            ? expression with
            {
                Value = value,
                Description = description,
                Metadata = metadata,
                Secure = secure,
                MinLength = minLength,
                MaxLength = maxLength,
                MinValue = minValue,
                MaxValue = maxValue,
                Sealed = @sealed,
                Exported = exported,
            }
            : expression;
    }

    void IExpressionVisitor.VisitAmbientTypeReferenceExpression(AmbientTypeReferenceExpression expression) => ReplaceCurrent(expression, ReplaceAmbientTypeReferenceExpression);
    public virtual Expression ReplaceAmbientTypeReferenceExpression(AmbientTypeReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitFullyQualifiedAmbientTypeReferenceExpression(FullyQualifiedAmbientTypeReferenceExpression expression) => ReplaceCurrent(expression, ReplaceFullyQualifiedAmbientTypeReferenceExpression);
    public virtual Expression ReplaceFullyQualifiedAmbientTypeReferenceExpression(FullyQualifiedAmbientTypeReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitTypeAliasReferenceExpression(TypeAliasReferenceExpression expression) => ReplaceCurrent(expression, ReplaceTypeAliasReferenceExpression);
    public virtual Expression ReplaceTypeAliasReferenceExpression(TypeAliasReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitSynthesizedTypeAliasReferenceExpression(SynthesizedTypeAliasReferenceExpression expression) => ReplaceCurrent(expression, ReplaceSynthesizedTypeAliasReferenceExpression);
    public virtual Expression ReplaceSynthesizedTypeAliasReferenceExpression(SynthesizedTypeAliasReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitStringLiteralTypeExpression(StringLiteralTypeExpression expression) => ReplaceCurrent(expression, ReplaceStringLiteralTypeExpression);
    public virtual Expression ReplaceStringLiteralTypeExpression(StringLiteralTypeExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitIntegerLiteralTypeExpression(IntegerLiteralTypeExpression expression) => ReplaceCurrent(expression, ReplaceIntegerLiteralTypeExpression);
    public virtual Expression ReplaceIntegerLiteralTypeExpression(IntegerLiteralTypeExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitBooleanLiteralTypeExpression(BooleanLiteralTypeExpression expression) => ReplaceCurrent(expression, ReplaceBooleanLiteralTypeExpression);
    public virtual Expression ReplaceBooleanLiteralTypeExpression(BooleanLiteralTypeExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitNullLiteralTypeExpression(NullLiteralTypeExpression expression) => ReplaceCurrent(expression, ReplaceNullLiteralTypeExpression);
    public virtual Expression ReplaceNullLiteralTypeExpression(NullLiteralTypeExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitResourceTypeExpression(ResourceTypeExpression expression) => ReplaceCurrent(expression, ReplaceResourceTypeExpression);
    public virtual Expression ReplaceResourceTypeExpression(ResourceTypeExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitObjectTypePropertyExpression(ObjectTypePropertyExpression expression) => ReplaceCurrent(expression, ReplaceObjectTypePropertyExpression);
    public virtual Expression ReplaceObjectTypePropertyExpression(ObjectTypePropertyExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.Value, out var value) |
            TryRewriteTypeDecorators(expression, out var description, out var metadata, out var secure, out var minLength, out var maxLength, out var minValue, out var maxValue, out var @sealed);

        return hasChanges
            ? expression with
            {
                Value = value,
                Description = description,
                Metadata = metadata,
                Secure = secure,
                MinLength = minLength,
                MaxLength = maxLength,
                MinValue = minValue,
                MaxValue = maxValue,
                Sealed = @sealed,
            }
            : expression;
    }

    void IExpressionVisitor.VisitObjectTypeAdditionalPropertiesExpression(ObjectTypeAdditionalPropertiesExpression expression) => ReplaceCurrent(expression, ReplaceObjectTypeAdditionalPropertiesExpression);
    public virtual Expression ReplaceObjectTypeAdditionalPropertiesExpression(ObjectTypeAdditionalPropertiesExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.Value, out var value) |
            TryRewriteTypeDecorators(expression, out var description, out var metadata, out var secure, out var minLength, out var maxLength, out var minValue, out var maxValue, out var @sealed);

        return hasChanges
            ? expression with
            {
                Value = value,
                Description = description,
                Metadata = metadata,
                Secure = secure,
                MinLength = minLength,
                MaxLength = maxLength,
                MinValue = minValue,
                MaxValue = maxValue,
                Sealed = @sealed,
            }
            : expression;
    }

    void IExpressionVisitor.VisitObjectTypeExpression(ObjectTypeExpression expression) => ReplaceCurrent(expression, ReplaceObjectTypeExpression);
    public virtual Expression ReplaceObjectTypeExpression(ObjectTypeExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.PropertyExpressions, out var propertyExpressions) |
            TryRewriteStrict(expression.AdditionalPropertiesExpression, out var additionalPropertiesExpression);

        return hasChanges
            ? expression with { PropertyExpressions = propertyExpressions, AdditionalPropertiesExpression = additionalPropertiesExpression }
            : expression;
    }

    void IExpressionVisitor.VisitTupleTypeItemExpression(TupleTypeItemExpression expression) => ReplaceCurrent(expression, ReplaceTupleTypeItemExpression);
    public virtual Expression ReplaceTupleTypeItemExpression(TupleTypeItemExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.Value, out var value) |
            TryRewriteTypeDecorators(expression, out var description, out var metadata, out var secure, out var minLength, out var maxLength, out var minValue, out var maxValue, out var @sealed);

        return hasChanges
            ? expression with
            {
                Value = value,
                Description = description,
                Metadata = metadata,
                Secure = secure,
                MinLength = minLength,
                MaxLength = maxLength,
                MinValue = minValue,
                MaxValue = maxValue,
                Sealed = @sealed,
            }
            : expression;
    }

    void IExpressionVisitor.VisitTupleTypeExpression(TupleTypeExpression expression) => ReplaceCurrent(expression, ReplaceTupleTypeExpression);
    public virtual Expression ReplaceTupleTypeExpression(TupleTypeExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.ItemExpressions, out var itemExpressions);

        return hasChanges ? expression with { ItemExpressions = itemExpressions } : expression;
    }

    void IExpressionVisitor.VisitArrayTypeExpression(ArrayTypeExpression expression) => ReplaceCurrent(expression, ReplaceArrayTypeExpression);
    public virtual Expression ReplaceArrayTypeExpression(ArrayTypeExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.BaseExpression, out var baseExpression);

        return hasChanges ? expression with { BaseExpression = baseExpression } : expression;
    }

    void IExpressionVisitor.VisitNullableTypeExpression(NullableTypeExpression expression) => ReplaceCurrent(expression, ReplaceNullableTypeExpression);
    public virtual Expression ReplaceNullableTypeExpression(NullableTypeExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.BaseExpression, out var baseExpression);

        return hasChanges ? expression with { BaseExpression = baseExpression } : expression;
    }

    void IExpressionVisitor.VisitNonNullableTypeExpression(NonNullableTypeExpression expression) => ReplaceCurrent(expression, ReplaceNonNullableTypeExpression);
    public virtual Expression ReplaceNonNullableTypeExpression(NonNullableTypeExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.BaseExpression, out var baseExpression);

        return hasChanges ? expression with { BaseExpression = baseExpression } : expression;
    }

    void IExpressionVisitor.VisitUnionTypeExpression(UnionTypeExpression expression) => ReplaceCurrent(expression, ReplaceUnionTypeExpression);
    public virtual Expression ReplaceUnionTypeExpression(UnionTypeExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.MemberExpressions, out var memberExpressions);

        return hasChanges ? expression with { MemberExpressions = memberExpressions } : expression;
    }

    void IExpressionVisitor.VisitImportedTypeReferenceExpression(ImportedTypeReferenceExpression expression) => ReplaceCurrent(expression, ReplaceImportedTypeReferenceExpression);
    public virtual Expression ReplaceImportedTypeReferenceExpression(ImportedTypeReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitWildcardImportTypePropertyReferenceExpression(WildcardImportTypePropertyReferenceExpression expression) => ReplaceCurrent(expression, ReplaceWildcardImportPropertyReferenceExpression);
    public virtual Expression ReplaceWildcardImportPropertyReferenceExpression(WildcardImportTypePropertyReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitDiscriminatedObjectTypeExpression(DiscriminatedObjectTypeExpression expression) => ReplaceCurrent(expression, ReplaceDiscriminatedObjectTypeExpression);

    public virtual Expression ReplaceDiscriminatedObjectTypeExpression(DiscriminatedObjectTypeExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.MemberExpressions, out var memberExpressions);

        return hasChanges ? expression with { MemberExpressions = memberExpressions } : expression;
    }

    void IExpressionVisitor.VisitParameterKeyVaultReferenceExpression(ParameterKeyVaultReferenceExpression expression) => ReplaceCurrent(expression, ReplaceParameterKeyVaultReferenceExpression);

    public virtual Expression ReplaceParameterKeyVaultReferenceExpression(ParameterKeyVaultReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitImportedVariableReferenceExpression(ImportedVariableReferenceExpression expression) => ReplaceCurrent(expression, ReplaceImportedVariableReferenceExpression);
    public virtual Expression ReplaceImportedVariableReferenceExpression(ImportedVariableReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitWildcardImportVariablePropertyReferenceExpression(WildcardImportVariablePropertyReferenceExpression expression) => ReplaceCurrent(expression, ReplaceWildcardImportVariablePropertyReferenceExpression);
    public virtual Expression ReplaceWildcardImportVariablePropertyReferenceExpression(WildcardImportVariablePropertyReferenceExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitResourceDerivedTypeExpression(ResourceDerivedTypeExpression expression) => ReplaceCurrent(expression, ReplaceResourceDerivedTypeExpression);
    public virtual Expression ReplaceResourceDerivedTypeExpression(ResourceDerivedTypeExpression expression)
    {
        return expression;
    }

    void IExpressionVisitor.VisitTypeReferencePropertyAccessExpression(TypeReferencePropertyAccessExpression expression) => ReplaceCurrent(expression, ReplaceTypeReferencePropertyAccessExpression);
    public virtual Expression ReplaceTypeReferencePropertyAccessExpression(TypeReferencePropertyAccessExpression expression)
    {
        var hasChanges = TryRewriteStrict(expression.BaseExpression, out var baseExpression);

        return hasChanges ? expression with { BaseExpression = baseExpression } : expression;
    }

    void IExpressionVisitor.VisitTypeReferenceAdditionalPropertiesAccessExpression(TypeReferenceAdditionalPropertiesAccessExpression expression) => ReplaceCurrent(expression, ReplaceTypeReferenceAdditionalPropertiesAccessExpression);
    public virtual Expression ReplaceTypeReferenceAdditionalPropertiesAccessExpression(TypeReferenceAdditionalPropertiesAccessExpression expression)
    {
        var hasChanges = TryRewriteStrict(expression.BaseExpression, out var baseExpression);

        return hasChanges ? expression with { BaseExpression = baseExpression } : expression;
    }

    void IExpressionVisitor.VisitTypeReferenceIndexAccessExpression(TypeReferenceIndexAccessExpression expression) => ReplaceCurrent(expression, ReplaceTypeReferenceIndexAccessExpression);
    public virtual Expression ReplaceTypeReferenceIndexAccessExpression(TypeReferenceIndexAccessExpression expression)
    {
        var hasChanges = TryRewriteStrict(expression.BaseExpression, out var baseExpression);

        return hasChanges ? expression with { BaseExpression = baseExpression } : expression;
    }

    void IExpressionVisitor.VisitTypeReferenceItemsAccessExpression(TypeReferenceItemsAccessExpression expression) => ReplaceCurrent(expression, ReplaceTypeReferenceItemsAccessExpression);
    public virtual Expression ReplaceTypeReferenceItemsAccessExpression(TypeReferenceItemsAccessExpression expression)
    {
        var hasChanges = TryRewriteStrict(expression.BaseExpression, out var baseExpression);

        return hasChanges ? expression with { BaseExpression = baseExpression } : expression;
    }

    void IExpressionVisitor.VisitProgramExpression(ProgramExpression expression) => ReplaceCurrent(expression, ReplaceProgramExpression);
    public virtual Expression ReplaceProgramExpression(ProgramExpression expression)
    {
        var hasChanges =
            TryRewriteStrict(expression.Metadata, out var metadata) |
            TryRewriteStrict(expression.Extensions, out var extensions) |
            TryRewriteStrict(expression.Parameters, out var parameters) |
            TryRewriteStrict(expression.Types, out var types) |
            TryRewriteStrict(expression.Variables, out var variables) |
            TryRewriteStrict(expression.Functions, out var functions) |
            TryRewriteStrict(expression.Resources, out var resources) |
            TryRewriteStrict(expression.Modules, out var modules) |
            TryRewriteStrict(expression.Stacks, out var stacks) |
            TryRewriteStrict(expression.Outputs, out var outputs);

        return hasChanges ? expression with { Metadata = metadata, Extensions = extensions, Parameters = parameters, Types = types, Variables = variables, Functions = functions, Resources = resources, Modules = modules, Stacks = stacks, Outputs = outputs } : expression;
    }

    protected virtual Expression Replace(Expression expression) => ReplaceInternal(expression);

    private Expression ReplaceInternal(Expression expression)
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

    private bool TryRewriteDescription(DescribableExpression expression, out Expression? description)
        => TryRewrite(expression.Description, out description);

    private bool TryRewriteTypeDecorators(TypeDeclaringExpression expression,
        out Expression? description,
        out Expression? metadata,
        out Expression? secure,
        out Expression? minLength,
        out Expression? maxLength,
        out Expression? minValue,
        out Expression? maxValue,
        out Expression? @sealed) =>
            TryRewriteDescription(expression, out description) |
            TryRewrite(expression.Metadata, out metadata) |
            TryRewrite(expression.Secure, out secure) |
            TryRewrite(expression.MinLength, out minLength) |
            TryRewrite(expression.MaxLength, out maxLength) |
            TryRewrite(expression.MinValue, out minValue) |
            TryRewrite(expression.MaxValue, out maxValue) |
            TryRewrite(expression.Sealed, out @sealed);

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
        var newExpressionList = ImmutableArray.CreateBuilder<TExpression>(expressions.Length);
        foreach (var expression in expressions)
        {
            hasChanges |= TryRewriteStrict(expression, out var newExpression);
            newExpressionList.Add(newExpression);
        }

        newExpressions = hasChanges ? newExpressionList.ToImmutable() : expressions;
        return hasChanges;
    }

    private bool TryRewriteDictionaryStrict<TExpression>(ImmutableDictionary<string, TExpression> dictionary, out ImmutableDictionary<string, TExpression> newDictionary)
        where TExpression : ArrayExpression
    {
        var hasChanges = false;
        var newDictionaryList = ImmutableDictionary.CreateBuilder<string, TExpression>(dictionary.KeyComparer, dictionary.ValueComparer);
        foreach (var (key, expression) in dictionary)
        {
            hasChanges |= TryRewriteStrict(expression, out var newExpression);
            newDictionaryList.Add(key, newExpression);
        }

        newDictionary = hasChanges ? newDictionaryList.ToImmutable() : dictionary;
        return hasChanges;
    }


    private bool TryRewrite(ImmutableArray<Expression> expressions, out ImmutableArray<Expression> newExpressions)
        => TryRewriteStrict(expressions, out newExpressions);

    private bool TryRewriteCollectionOfNullablesStrict<TExpression>(ImmutableArray<TExpression?> expressions, out ImmutableArray<TExpression?> newExpressions)
        where TExpression : Expression
    {
        var hasChanges = false;
        var newExpressionList = ImmutableArray.CreateBuilder<TExpression?>(expressions.Length);
        foreach (var expression in expressions)
        {
            var modified = expression;
            if (expression is not null)
            {
                hasChanges |= TryRewriteStrict(expression, out modified);
            }
            newExpressionList.Add(modified);
        }

        newExpressions = hasChanges ? newExpressionList.ToImmutable() : expressions;
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
