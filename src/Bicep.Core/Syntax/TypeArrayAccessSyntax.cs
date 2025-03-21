// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax;

public class TypeArrayAccessSyntax : TypeSyntax
{
    public TypeArrayAccessSyntax(SyntaxBase baseExpression, Token openSquare, SyntaxBase indexExpression, Token closeSquare)
    {
        AssertTokenType(openSquare, nameof(openSquare), TokenType.LeftSquare);
        AssertTokenType(closeSquare, nameof(closeSquare), TokenType.RightSquare);

        BaseExpression = baseExpression;
        OpenSquare = openSquare;
        IndexExpression = indexExpression;
        CloseSquare = closeSquare;
    }

    public SyntaxBase BaseExpression { get; }

    public Token OpenSquare { get; }

    public SyntaxBase IndexExpression { get; }

    public Token CloseSquare { get; }

    public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTypeArrayAccessSyntax(this);

    public override TextSpan Span => TextSpan.Between(BaseExpression, CloseSquare);
}
