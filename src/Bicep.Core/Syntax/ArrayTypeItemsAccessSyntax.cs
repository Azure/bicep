// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax;

public class ArrayTypeItemsAccessSyntax : TypeSyntax
{
    public ArrayTypeItemsAccessSyntax(SyntaxBase baseExpression, Token openBracket, Token asterisk, Token closeBracket)
    {
        AssertTokenType(openBracket, nameof(openBracket), TokenType.LeftSquare);
        AssertTokenType(asterisk, nameof(asterisk), TokenType.Asterisk);
        AssertTokenType(closeBracket, nameof(closeBracket), TokenType.RightSquare);

        BaseExpression = baseExpression;
        OpenBracket = openBracket;
        Asterisk = asterisk;
        CloseBracket = closeBracket;
    }

    public SyntaxBase BaseExpression { get; }

    public Token OpenBracket { get; }

    public Token CloseBracket { get; }

    public Token Asterisk { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitArrayTypeItemsAccessSyntax(this);

    public override TextSpan Span => TextSpan.Between(BaseExpression, CloseBracket);
}
