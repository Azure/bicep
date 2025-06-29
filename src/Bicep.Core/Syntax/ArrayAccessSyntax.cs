// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class ArrayAccessSyntax : AccessExpressionSyntax
    {
        public ArrayAccessSyntax(
            SyntaxBase baseExpression,
            Token openSquare,
            Token? safeAccessMarker,
            Token? fromEndMarker,
            SyntaxBase indexExpression,
            Token closeSquare) : base(
                baseExpression,
                safeAccessMarker)
        {
            AssertTokenType(openSquare, nameof(openSquare), TokenType.LeftSquare);
            AssertTokenType(closeSquare, nameof(closeSquare), TokenType.RightSquare);

            this.OpenSquare = openSquare;
            FromEndMarker = fromEndMarker;
            this.IndexExpression = indexExpression;
            this.CloseSquare = closeSquare;
        }

        public Token OpenSquare { get; }

        public Token? FromEndMarker { get; }

        public SyntaxBase IndexExpression { get; }

        public Token CloseSquare { get; }

        public override ArrayAccessSyntax AsSafeAccess() => SafeAccessMarker is null
            ? new(BaseExpression, OpenSquare, SyntaxFactory.QuestionToken, FromEndMarker, IndexExpression, CloseSquare)
            : this;

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitArrayAccessSyntax(this);

        public override TextSpan Span => TextSpan.Between(BaseExpression, CloseSquare);
    }
}
