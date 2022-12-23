// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class ArrayAccessSyntax : ExpressionSyntax
    {
        public ArrayAccessSyntax(SyntaxBase baseExpression, Token openSquare, Token? safeAccessMarker, SyntaxBase indexExpression, Token closeSquare)
        {
            AssertTokenType(openSquare, nameof(openSquare), TokenType.LeftSquare);
            AssertTokenType(safeAccessMarker, nameof(safeAccessMarker), TokenType.Question);
            AssertTokenType(closeSquare, nameof(closeSquare), TokenType.RightSquare);

            this.BaseExpression = baseExpression;
            this.OpenSquare = openSquare;
            this.SafeAccessMarker = safeAccessMarker;
            this.IndexExpression = indexExpression;
            this.CloseSquare = closeSquare;
        }

        public SyntaxBase BaseExpression { get; }

        public Token OpenSquare { get; }

        public Token? SafeAccessMarker { get; }

        public SyntaxBase IndexExpression { get; }

        public Token CloseSquare { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitArrayAccessSyntax(this);

        public override TextSpan Span => TextSpan.Between(BaseExpression, CloseSquare);
    }
}
