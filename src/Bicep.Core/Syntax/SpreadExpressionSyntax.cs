// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class SpreadExpressionSyntax : ExpressionSyntax
{
    public SpreadExpressionSyntax(Token ellipsis, SyntaxBase expression)
    {
        AssertTokenType(ellipsis, nameof(ellipsis), TokenType.Ellipsis);

        this.Ellipsis = ellipsis;
        this.Expression = expression;
    }

    public Token Ellipsis { get; }

    public SyntaxBase Expression { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitSpreadExpressionSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.Ellipsis, this.Expression);
}
