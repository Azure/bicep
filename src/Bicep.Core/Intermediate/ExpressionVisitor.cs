// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;

namespace Bicep.Core.Intermediate;

public abstract class ExpressionVisitor : IExpressionVisitor
{
    public virtual void VisitAccessChainExpression(AccessChainExpression expression)
    {
        Visit(expression.FirstLink);
        Visit(expression.AdditionalProperties);
    }

    public virtual void VisitArrayAccessExpression(ArrayAccessExpression expression)
    {
        Visit(expression.Access);
        Visit(expression.Base);
    }

    public virtual void VisitArrayExpression(ArrayExpression expression)
    {
        Visit(expression.Items);
    }

    public virtual void VisitBinaryExpression(BinaryExpression expression)
    {
        Visit(expression.Left);
        Visit(expression.Right);
    }

    public virtual void VisitBooleanLiteralExpression(BooleanLiteralExpression expression)
    {
    }

    public virtual void VisitCopyIndexExpression(CopyIndexExpression expression)
    {
    }

    public virtual void VisitForLoopExpression(ForLoopExpression expression)
    {
        Visit(expression.Expression);
        Visit(expression.Body);
    }

    public virtual void VisitConditionExpression(ConditionExpression expression)
    {
        Visit(expression.Expression);
        Visit(expression.Body);
    }

    public virtual void VisitFunctionCallExpression(FunctionCallExpression expression)
    {
        Visit(expression.Parameters);
    }

    public virtual void VisitResourceFunctionCallExpression(ResourceFunctionCallExpression expression)
    {
        Visit(expression.Resource);
        Visit(expression.Parameters);
    }

    public virtual void VisitIntegerLiteralExpression(IntegerLiteralExpression expression)
    {
    }

    public virtual void VisitInterpolatedStringExpression(InterpolatedStringExpression expression)
    {
        Visit(expression.Expressions);
    }

    public virtual void VisitLambdaExpression(LambdaExpression expression)
    {
        Visit(expression.ParameterTypes);
        Visit(expression.Body);
        Visit(expression.OutputType);
    }

    public virtual void VisitLambdaVariableReferenceExpression(LambdaVariableReferenceExpression expression)
    {
    }

    public virtual void VisitModuleOutputPropertyAccessExpression(ModuleOutputPropertyAccessExpression expression)
    {
        Visit(expression.Base);
    }

    public virtual void VisitModuleReferenceExpression(ModuleReferenceExpression expression)
    {
    }

    public virtual void VisitStackReferenceExpression(StackReferenceExpression expression)
    {
    }

    public virtual void VisitNullLiteralExpression(NullLiteralExpression expression)
    {
    }

    public virtual void VisitObjectExpression(ObjectExpression expression)
    {
        Visit(expression.Properties);
    }

    public virtual void VisitObjectPropertyExpression(ObjectPropertyExpression expression)
    {
        Visit(expression.Key);
        Visit(expression.Value);
    }

    public virtual void VisitParametersReferenceExpression(ParametersReferenceExpression expression)
    {
    }

    public virtual void VisitParametersAssignmentReferenceExpression(ParametersAssignmentReferenceExpression expression)
    {
    }

    public virtual void VisitPropertyAccessExpression(PropertyAccessExpression expression)
    {
        Visit(expression.Base);
    }

    public virtual void VisitResourceReferenceExpression(ResourceReferenceExpression expression)
    {
    }

    public virtual void VisitStringLiteralExpression(StringLiteralExpression expression)
    {
    }

    public virtual void VisitTernaryExpression(TernaryExpression expression)
    {
        Visit(expression.Condition);
        Visit(expression.True);
        Visit(expression.False);
    }

    public virtual void VisitUnaryExpression(UnaryExpression expression)
    {
        Visit(expression.Expression);
    }

    public virtual void VisitVariableReferenceExpression(VariableReferenceExpression expression)
    {
    }

    public virtual void VisitSynthesizedVariableReferenceExpression(SynthesizedVariableReferenceExpression expression)
    {
    }

    public virtual void VisitDeclaredMetadataExpression(DeclaredMetadataExpression expression)
    {
        VisitDescribableExpression(expression);
        Visit(expression.Value);
    }

    private void VisitDescribableExpression(DescribableExpression expression)
    {
        Visit(expression.Description);
    }

    public virtual void VisitExtensionExpression(ExtensionExpression expression)
    {
        VisitDescribableExpression(expression);
        Visit(expression.Config);
    }

    public void VisitExtensionReferenceExpression(ExtensionReferenceExpression expression)
    {
    }

    public void VisitExtensionConfigAssignmentReferenceExpression(ExtensionConfigAssignmentReferenceExpression extensionConfigAssignmentReferenceExpression)
    {
    }

    public virtual void VisitDeclaredParameterExpression(DeclaredParameterExpression expression)
    {
        VisitTypeDeclaringExpression(expression);
        Visit(expression.Type);
        Visit(expression.DefaultValue);
    }

    private void VisitTypeDeclaringExpression(TypeDeclaringExpression expression)
    {
        VisitDescribableExpression(expression);
        Visit(expression.Metadata);
        Visit(expression.Secure);
        Visit(expression.MinLength);
        Visit(expression.MaxLength);
        Visit(expression.MinValue);
        Visit(expression.MaxValue);
        Visit(expression.Sealed);
    }

    public virtual void VisitDeclaredVariableExpression(DeclaredVariableExpression expression)
    {
        VisitDescribableExpression(expression);
        Visit(expression.Exported);
        Visit(expression.Type);
        Visit(expression.Value);
    }

    public virtual void VisitDeclaredOutputExpression(DeclaredOutputExpression expression)
    {
        VisitTypeDeclaringExpression(expression);
        Visit(expression.Type);
        Visit(expression.Value);
    }

    public virtual void VisitDeclaredResourceExpression(DeclaredResourceExpression expression)
    {
        VisitDescribableExpression(expression);
        Visit(expression.Body);
        Visit(expression.DependsOn);
        Visit(expression.DecoratorConfig);
    }

    public virtual void VisitDeclaredAssertExpression(DeclaredAssertExpression expression)
    {
        VisitDescribableExpression(expression);
        Visit(expression.Value);
    }

    public virtual void VisitDeclaredModuleExpression(DeclaredModuleExpression expression)
    {
        VisitDescribableExpression(expression);
        Visit(expression.Body);
        Visit(expression.Parameters);
        Visit(expression.DependsOn);
        Visit(expression.ExtensionConfigs);
    }

    public virtual void VisitDeclaredStackExpression(DeclaredStackExpression expression)
    {
        VisitDescribableExpression(expression);
        Visit(expression.Body);
        Visit(expression.DependsOn);
    }

    public virtual void VisitDeclaredRuleExpression(DeclaredRuleExpression expression)
    {
        VisitDescribableExpression(expression);
        Visit(expression.Body);
    }

    public virtual void VisitResourceDependencyExpression(ResourceDependencyExpression expression)
    {
        Visit(expression.Reference);
    }

    public virtual void VisitDeclaredFunctionExpression(DeclaredFunctionExpression expression)
    {
        VisitDescribableExpression(expression);
        Visit(expression.Exported);
        Visit(expression.Lambda);
    }

    public virtual void VisitUserDefinedFunctionCallExpression(UserDefinedFunctionCallExpression expression)
    {
        Visit(expression.Parameters);
    }

    public virtual void VisitSynthesizedUserDefinedFunctionCallExpression(SynthesizedUserDefinedFunctionCallExpression expression)
    {
        Visit(expression.Parameters);
    }

    public virtual void VisitImportedUserDefinedFunctionCallExpression(ImportedUserDefinedFunctionCallExpression expression)
    {
        Visit(expression.Parameters);
    }

    public virtual void VisitWildcardImportInstanceFunctionCallExpression(WildcardImportInstanceFunctionCallExpression expression)
    {
        Visit(expression.Parameters);
    }

    public virtual void VisitDeclaredTypeExpression(DeclaredTypeExpression expression)
    {
        VisitTypeDeclaringExpression(expression);
        Visit(expression.Exported);
        Visit(expression.Value);
    }

    public virtual void VisitAmbientTypeReferenceExpression(AmbientTypeReferenceExpression expression)
    {
    }

    public virtual void VisitFullyQualifiedAmbientTypeReferenceExpression(FullyQualifiedAmbientTypeReferenceExpression expression)
    {
    }

    public virtual void VisitTypeAliasReferenceExpression(TypeAliasReferenceExpression expression)
    {
    }

    public virtual void VisitSynthesizedTypeAliasReferenceExpression(SynthesizedTypeAliasReferenceExpression expression)
    {
    }

    public virtual void VisitStringLiteralTypeExpression(StringLiteralTypeExpression expression)
    {
    }

    public virtual void VisitIntegerLiteralTypeExpression(IntegerLiteralTypeExpression expression)
    {
    }

    public virtual void VisitBooleanLiteralTypeExpression(BooleanLiteralTypeExpression expression)
    {
    }

    public virtual void VisitNullLiteralTypeExpression(NullLiteralTypeExpression expression)
    {
    }

    public virtual void VisitResourceTypeExpression(ResourceTypeExpression expression)
    {
    }

    public virtual void VisitObjectTypePropertyExpression(ObjectTypePropertyExpression expression)
    {
        VisitTypeDeclaringExpression(expression);
        Visit(expression.Value);
    }

    public virtual void VisitObjectTypeAdditionalPropertiesExpression(ObjectTypeAdditionalPropertiesExpression expression)
    {
        VisitTypeDeclaringExpression(expression);
        Visit(expression.Value);
    }

    public virtual void VisitObjectTypeExpression(ObjectTypeExpression expression)
    {
        Visit(expression.PropertyExpressions);
        Visit(expression.AdditionalPropertiesExpression);
    }

    public virtual void VisitTupleTypeItemExpression(TupleTypeItemExpression expression)
    {
        VisitTypeDeclaringExpression(expression);
        Visit(expression.Value);
    }

    public virtual void VisitTupleTypeExpression(TupleTypeExpression expression)
    {
        Visit(expression.ItemExpressions);
    }

    public virtual void VisitArrayTypeExpression(ArrayTypeExpression expression)
    {
        Visit(expression.BaseExpression);
    }

    public virtual void VisitNullableTypeExpression(NullableTypeExpression expression)
    {
        Visit(expression.BaseExpression);
    }

    public virtual void VisitNonNullableTypeExpression(NonNullableTypeExpression expression)
    {
        Visit(expression.BaseExpression);
    }

    public virtual void VisitUnionTypeExpression(UnionTypeExpression expression)
    {
        Visit(expression.MemberExpressions);
    }

    public virtual void VisitImportedTypeReferenceExpression(ImportedTypeReferenceExpression expression)
    {
    }

    public virtual void VisitWildcardImportTypePropertyReferenceExpression(WildcardImportTypePropertyReferenceExpression expression)
    {
    }

    public virtual void VisitDiscriminatedObjectTypeExpression(DiscriminatedObjectTypeExpression expression)
    {
        Visit(expression.MemberExpressions);
    }

    public virtual void VisitParameterKeyVaultReferenceExpression(ParameterKeyVaultReferenceExpression expression)
    {
    }

    public virtual void VisitImportedVariableReferenceExpression(ImportedVariableReferenceExpression expression)
    {
    }

    public virtual void VisitWildcardImportVariablePropertyReferenceExpression(WildcardImportVariablePropertyReferenceExpression expression)
    {
    }

    public virtual void VisitResourceDerivedTypeExpression(ResourceDerivedTypeExpression expression)
    {
    }

    public virtual void VisitTypeReferencePropertyAccessExpression(TypeReferencePropertyAccessExpression expression)
    {
        Visit(expression.BaseExpression);
    }

    public virtual void VisitTypeReferenceAdditionalPropertiesAccessExpression(TypeReferenceAdditionalPropertiesAccessExpression expression)
    {
        Visit(expression.BaseExpression);
    }

    public virtual void VisitTypeReferenceIndexAccessExpression(TypeReferenceIndexAccessExpression expression)
    {
        Visit(expression.BaseExpression);
    }

    public virtual void VisitTypeReferenceItemsAccessExpression(TypeReferenceItemsAccessExpression expression)
    {
        Visit(expression.BaseExpression);
    }

    public virtual void VisitProgramExpression(ProgramExpression expression)
    {
        Visit(expression.Metadata);
        Visit(expression.Extensions);
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

    protected void Visit(IReadOnlyDictionary<string, ArrayExpression> dictionary)
    {
        foreach (var (key, expression) in dictionary)
        {
            this.Visit(expression);
        }
    }

    protected virtual void VisitInternal(Expression expression)
    {
        expression.Accept(this);
    }
}
