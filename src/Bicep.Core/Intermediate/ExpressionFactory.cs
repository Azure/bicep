// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;

namespace Bicep.Core.Intermediate;

public static class ExpressionFactory
{
    public static ObjectPropertyExpression CreateObjectProperty(string name, Expression value, SyntaxBase? sourceSyntax = null)
        => new(
            sourceSyntax,
            new StringLiteralExpression(sourceSyntax, name),
            value);

    public static ObjectExpression CreateObject(IEnumerable<ObjectPropertyExpression> properties, SyntaxBase? sourceSyntax = null)
        => new(sourceSyntax, properties.ToImmutableArray());

    public static ArrayExpression CreateArray(IEnumerable<Expression> items, SyntaxBase? sourceSyntax = null)
        => new(sourceSyntax, items.ToImmutableArray());

    public static StringLiteralExpression CreateStringLiteral(string value, SyntaxBase? sourceSyntax = null)
        => new(sourceSyntax, value);

    public static BooleanLiteralExpression CreateBooleanLiteral(bool value, SyntaxBase? sourceSyntax = null)
        => new(sourceSyntax, value);

    public static IntegerLiteralExpression CreateIntegerLiteral(long value, SyntaxBase? sourceSyntax = null)
        => new(sourceSyntax, value);

    public static PropertyAccessExpression CreatePropertyAccess(Expression baseExpression, string propertyName, SyntaxBase? sourceSyntax = null, AccessExpressionFlags flags = AccessExpressionFlags.None)
        => new(sourceSyntax ?? baseExpression.SourceSyntax, baseExpression, propertyName, flags);

    public static PropertyAccessExpression CreateResourcePropertyAccess(ResourceMetadata metadata, IndexReplacementContext? indexContext, string propertyName, PropertyAccessSyntax? sourceSyntax = null, AccessExpressionFlags flags = AccessExpressionFlags.None)
        => CreatePropertyAccess(
            new ResourceReferenceExpression(sourceSyntax?.BaseExpression, metadata, indexContext),
            propertyName,
            sourceSyntax,
            flags);

    public static FunctionCallExpression CreateFunctionCall(string functionName, SyntaxBase? sourceSyntax = null, params Expression[] parameters)
        => CreateFunctionCall(functionName, parameters, sourceSyntax);

    public static FunctionCallExpression CreateFunctionCall(string functionName, IEnumerable<Expression> parameters, SyntaxBase? sourceSyntax = null)
        => new(sourceSyntax, functionName, parameters.ToImmutableArray());
}
