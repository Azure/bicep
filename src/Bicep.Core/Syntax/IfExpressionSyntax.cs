// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class IfExpressionSyntax : ExpressionSyntax
    {
        public IfExpressionSyntax(Token keyword, SyntaxBase conditionExpression, SyntaxBase consequenceExpression)
        {
            this.Keyword = keyword;
            this.ConditionExpression = conditionExpression;
            this.ConsequenceExpression = consequenceExpression;
        }

        public Token Keyword { get; }

        public SyntaxBase ConditionExpression { get; }

        public SyntaxBase ConsequenceExpression { get; }

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.ConsequenceExpression);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitIfExpressionSyntax(this);
    }
}
