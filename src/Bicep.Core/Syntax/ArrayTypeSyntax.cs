// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class ArrayTypeSyntax : TypeSyntax
{
    public ArrayTypeSyntax(ArrayTypeMemberSyntax item, Token openBracket, Token closeBracket)
    {
        AssertTokenType(openBracket, nameof(openBracket), TokenType.LeftSquare);
        AssertTokenType(closeBracket, nameof(closeBracket), TokenType.RightSquare);

        this.Item = item;
        this.OpenBracket = openBracket;
        this.CloseBracket = closeBracket;
    }

    public ArrayTypeMemberSyntax Item { get; }

    public Token OpenBracket { get; }

    public Token CloseBracket { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitArrayTypeSyntax(this);

    public override TextSpan Span => TextSpan.Between(this.Item, this.CloseBracket);
}
