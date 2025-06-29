// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class TypeItemsAccessSyntax : TypeSyntax
{
    public TypeItemsAccessSyntax(SyntaxBase baseExpression, Token openSquare, Token asterisk, Token closeSquare)
    {
        AssertTokenType(openSquare, nameof(openSquare), TokenType.LeftSquare);
        AssertTokenType(asterisk, nameof(asterisk), TokenType.Asterisk);
        AssertTokenType(closeSquare, nameof(closeSquare), TokenType.RightSquare);

        BaseExpression = baseExpression;
        OpenSquare = openSquare;
        Asterisk = asterisk;
        CloseSquare = closeSquare;
    }

    public SyntaxBase BaseExpression { get; }

    public Token OpenSquare { get; }

    public Token Asterisk { get; }

    public Token CloseSquare { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypeItemsAccessSyntax(this);

    public override TextSpan Span => TextSpan.Between(BaseExpression, CloseSquare);
}
