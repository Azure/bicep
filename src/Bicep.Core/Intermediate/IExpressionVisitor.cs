// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Intermediate;

public interface IExpressionVisitor
{
    void VisitBooleanLiteralExpression(BooleanLiteralExpression expression);

    void VisitIntegerLiteralExpression(IntegerLiteralExpression expression);

    void VisitStringLiteralExpression(StringLiteralExpression expression);

    void VisitNullLiteralExpression(NullLiteralExpression expression);

    void VisitInterpolatedStringExpression(InterpolatedStringExpression expression);

    void VisitObjectExpression(ObjectExpression expression);

    void VisitObjectPropertyExpression(ObjectPropertyExpression expression);

    void VisitArrayExpression(ArrayExpression expression);

    void VisitTernaryExpression(TernaryExpression expression);

    void VisitBinaryExpression(BinaryExpression expression);

    void VisitUnaryExpression(UnaryExpression expression);

    void VisitFunctionCallExpression(FunctionCallExpression expression);

    void VisitResourceFunctionCallExpression(ResourceFunctionCallExpression expression);

    void VisitArrayAccessExpression(ArrayAccessExpression expression);

    void VisitPropertyAccessExpression(PropertyAccessExpression expression);

    void VisitResourceReferenceExpression(ResourceReferenceExpression expression);

    void VisitModuleReferenceExpression(ModuleReferenceExpression expression);

    void VisitModuleOutputPropertyAccessExpression(ModuleOutputPropertyAccessExpression expression);

    void VisitVariableReferenceExpression(VariableReferenceExpression expression);

    void VisitSynthesizedVariableReferenceExpression(SynthesizedVariableReferenceExpression expression);

    void VisitParametersReferenceExpression(ParametersReferenceExpression expression);

    void VisitParametersAssignmentReferenceExpression(ParametersAssignmentReferenceExpression expression);

    void VisitLambdaVariableReferenceExpression(LambdaVariableReferenceExpression expression);

    void VisitForLoopExpression(ForLoopExpression expression);

    void VisitCopyIndexExpression(CopyIndexExpression expression);

    void VisitConditionExpression(ConditionExpression expression);

    void VisitLambdaExpression(LambdaExpression expression);

    void VisitDeclaredMetadataExpression(DeclaredMetadataExpression expression);

    void VisitDeclaredProviderExpression(DeclaredProviderExpression expression);

    void VisitDeclaredParameterExpression(DeclaredParameterExpression expression);

    void VisitDeclaredVariableExpression(DeclaredVariableExpression expression);

    void VisitDeclaredOutputExpression(DeclaredOutputExpression expression);

    void VisitDeclaredAssertExpression(DeclaredAssertExpression expression);

    void VisitDeclaredResourceExpression(DeclaredResourceExpression expression);

    void VisitDeclaredModuleExpression(DeclaredModuleExpression expression);

    void VisitResourceDependencyExpression(ResourceDependencyExpression expression);

    void VisitProgramExpression(ProgramExpression expression);

    void VisitAccessChainExpression(AccessChainExpression expression);

    void VisitDeclaredFunctionExpression(DeclaredFunctionExpression expression);

    void VisitUserDefinedFunctionCallExpression(UserDefinedFunctionCallExpression expression);

    void VisitDeclaredTypeExpression(DeclaredTypeExpression expression);

    void VisitAmbientTypeReferenceExpression(AmbientTypeReferenceExpression expression);

    void VisitFullyQualifiedAmbientTypeReferenceExpression(FullyQualifiedAmbientTypeReferenceExpression expression);

    void VisitTypeAliasReferenceExpression(TypeAliasReferenceExpression expression);

    void VisitStringLiteralTypeExpression(StringLiteralTypeExpression expression);

    void VisitIntegerLiteralTypeExpression(IntegerLiteralTypeExpression expression);

    void VisitBooleanLiteralTypeExpression(BooleanLiteralTypeExpression expression);

    void VisitNullLiteralTypeExpression(NullLiteralTypeExpression expression);

    void VisitResourceTypeExpression(ResourceTypeExpression expression);

    void VisitObjectTypePropertyExpression(ObjectTypePropertyExpression expression);

    void VisitObjectTypeAdditionalPropertiesExpression(ObjectTypeAdditionalPropertiesExpression expression);

    void VisitObjectTypeExpression(ObjectTypeExpression expression);

    void VisitTupleTypeItemExpression(TupleTypeItemExpression expression);

    void VisitTupleTypeExpression(TupleTypeExpression expression);

    void VisitArrayTypeExpression(ArrayTypeExpression expression);

    void VisitNullableTypeExpression(NullableTypeExpression expression);

    void VisitNonNullableTypeExpression(NonNullableTypeExpression expression);

    void VisitUnionTypeExpression(UnionTypeExpression expression);

    void VisitImportedTypeReferenceExpression(ImportedTypeReferenceExpression expression);

    void VisitWildcardImportPropertyReferenceExpression(WildcardImportPropertyReferenceExpression expression);

    void VisitDiscriminatedObjectTypeExpression(DiscriminatedObjectTypeExpression expression);
}
