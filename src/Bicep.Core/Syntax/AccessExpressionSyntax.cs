// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

/// <summary>
/// Represents a syntax node that accesses a property or index of a base value.
/// </summary>
public abstract class AccessExpressionSyntax : ExpressionSyntax
{
    protected AccessExpressionSyntax(SyntaxBase baseExpression, Token? safeAccessMarker)
    {
        AssertTokenType(safeAccessMarker, nameof(safeAccessMarker), TokenType.Question);

        BaseExpression = baseExpression;
        SafeAccessMarker = safeAccessMarker;
    }

    public SyntaxBase BaseExpression { get; }

    public Token? SafeAccessMarker { get; }

    public abstract SyntaxBase IndexExpression { get; }

    public bool IsSafeAccess => SafeAccessMarker is not null;

    public abstract AccessExpressionSyntax AsSafeAccess();
}
