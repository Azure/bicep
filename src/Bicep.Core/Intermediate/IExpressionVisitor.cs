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
}
