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

    void VisitArrayAccessExpression(ArrayAccessExpression expression);

    void VisitPropertyAccessExpression(PropertyAccessExpression expression);

    void VisitResourceReferenceExpression(ResourceReferenceExpression expression);

    void VisitModuleReferenceExpression(ModuleReferenceExpression expression);

    void VisitModuleOutputPropertyAccessExpression(ModuleOutputPropertyAccessExpression expression);

    void VisitVariableReferenceExpression(VariableReferenceExpression expression);

    void VisitSynthesizedVariableReferenceExpression(SynthesizedVariableReferenceExpression expression);

    void VisitParametersReferenceExpression(ParametersReferenceExpression expression);

    void VisitLambdaVariableReferenceExpression(LambdaVariableReferenceExpression expression);

    void VisitForLoopExpression(ForLoopExpression expression);

    void VisitCopyIndexExpression(CopyIndexExpression expression);

    void VisitConditionExpression(ConditionExpression expression);

    void VisitLambdaExpression(LambdaExpression expression);

    void VisitDeclaredMetadataExpression(DeclaredMetadataExpression expression);

    void VisitDeclaredImportExpression(DeclaredImportExpression expression);

    void VisitDeclaredParameterExpression(DeclaredParameterExpression expression);

    void VisitDeclaredVariableExpression(DeclaredVariableExpression expression);

    void VisitDeclaredOutputExpression(DeclaredOutputExpression expression);

    void VisitProgramExpression(ProgramExpression expression);

    void VisitAccessChainExpression(AccessChainExpression expression);
}
