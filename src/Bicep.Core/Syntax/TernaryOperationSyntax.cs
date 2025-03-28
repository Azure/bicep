// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class TernaryOperationSyntax : ExpressionSyntax
    {
        public TernaryOperationSyntax(
            SyntaxBase conditionExpression,
            ImmutableArray<Token> newlinesBeforeQuestion,
            Token question,
            SyntaxBase trueExpression,
            ImmutableArray<Token> newlinesBeforeColon,
            SyntaxBase colon,
            SyntaxBase falseExpression)
        {
            AssertTokenType(question, nameof(question), TokenType.Question);
            AssertSyntaxType(colon, nameof(colon), typeof(Token), typeof(SkippedTriviaSyntax));
            AssertTokenType(colon as Token, nameof(colon), TokenType.Colon);

            this.ConditionExpression = conditionExpression;
            NewlinesBeforeQuestion = newlinesBeforeQuestion;
            this.Question = question;
            this.TrueExpression = trueExpression;
            NewlinesBeforeColon = newlinesBeforeColon;
            this.Colon = colon;
            this.FalseExpression = falseExpression;
        }

        public SyntaxBase ConditionExpression { get; }

        public ImmutableArray<Token> NewlinesBeforeQuestion { get; }

        public Token Question { get; }

        public SyntaxBase TrueExpression { get; }

        public ImmutableArray<Token> NewlinesBeforeColon { get; }

        public SyntaxBase Colon { get; }

        public SyntaxBase FalseExpression { get; }

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitTernaryOperationSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.ConditionExpression, this.FalseExpression);
    }
}

