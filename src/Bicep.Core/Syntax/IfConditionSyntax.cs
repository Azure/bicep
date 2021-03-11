// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public class IfConditionSyntax : SyntaxBase
    {
        public IfConditionSyntax(Token keyword, SyntaxBase conditionExpression, SyntaxBase body)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.IfKeyword);
            AssertSyntaxType(conditionExpression, nameof(conditionExpression), typeof(ParenthesizedExpressionSyntax), typeof(SkippedTriviaSyntax));
            AssertSyntaxType(body, nameof(body), typeof(SkippedTriviaSyntax), typeof(ObjectSyntax));

            this.Keyword = keyword;
            this.ConditionExpression = conditionExpression;
            this.Body = body;
        }

        public Token Keyword { get; }

        public SyntaxBase ConditionExpression { get; }

        public SyntaxBase Body { get; }

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Body);

        public override void Accept(ISyntaxVisitor visitor) => visitor.VisitIfConditionSyntax(this);
    }
}
