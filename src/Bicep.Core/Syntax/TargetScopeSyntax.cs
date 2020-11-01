// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parser;

namespace Bicep.Core.Syntax
{
    public class TargetScopeSyntax : SyntaxBase
    {
        public TargetScopeSyntax(Token keyword, Token assignment, SyntaxBase value)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.TargetScopeKeyword);
            AssertTokenType(assignment, nameof(assignment), TokenType.Assignment);

            this.Keyword = keyword;
            this.Assignment = assignment;
            this.Value = value;
        }

        public Token Keyword { get; }

        public Token Assignment { get; }

        public SyntaxBase Value { get; }

        public override void Accept(SyntaxVisitor visitor) => visitor.VisitTargetScopeSyntax(this);

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Value);
    }
}