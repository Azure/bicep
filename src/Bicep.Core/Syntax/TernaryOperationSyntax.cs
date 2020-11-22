// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class TernaryOperationSyntax : ExpressionSyntax
    {
        public TernaryOperationSyntax(SyntaxBase conditionExpression, Token question, SyntaxBase trueExpression, Token colon, SyntaxBase falseExpression)
        {
            AssertTokenType(question,nameof(question), TokenType.Question);
            AssertTokenType(colon, nameof(colon), TokenType.Colon);

            this.ConditionExpression = conditionExpression;
            this.Question = question;
            this.TrueExpression = trueExpression;
            this.Colon = colon;
            this.FalseExpression = falseExpression;
        }

        public SyntaxBase ConditionExpression { get; }

        public Token Question { get; }

        public SyntaxBase TrueExpression { get; }

        public Token Colon { get; }

        public SyntaxBase FalseExpression { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTernaryOperationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.ConditionExpression, this.FalseExpression);
    }
}

