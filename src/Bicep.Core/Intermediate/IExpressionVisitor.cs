// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Intermediate;

public interface IExpressionVisitor
{
    void VisitBooleanLiteralExpression(BooleanLiteralExpression expression);

    void VisitIntegerLiteralExpression(IntegerLiteralExpression expression);

    void VisitStringLiteralExpression(StringLiteralExpression expression);

    void VisitNullLiteralExpression(NullLiteralExpression expression);

    void VisitSyntaxExpression(SyntaxExpression expression);

    void VisitInterpolatedStringExpression(InterpolatedStringExpression expression);

    void VisitObjectExpression(ObjectExpression expression);

    void VisitArrayExpression(ArrayExpression expression);

    void VisitTernaryExpression(TernaryExpression expression);

    void VisitBinaryExpression(BinaryExpression expression);

    void VisitUnaryExpression(UnaryExpression expression);

    void VisitFunctionCallExpression(FunctionCallExpression expression);

    void VisitArrayAccessExpression(ArrayAccessExpression expression);

    void VisitPropertyAccessExpression(PropertyAccessExpression expression);

    void VisitResourceIdExpression(ResourceIdExpression expression);

    void VisitResourceReferenceExpression(ResourceReferenceExpression expression);

    void VisitModuleReferenceExpression(ModuleReferenceExpression expression);
}
