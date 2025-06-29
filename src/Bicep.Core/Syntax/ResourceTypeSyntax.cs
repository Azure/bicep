// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Text;

namespace Bicep.Core.Syntax
{
    public class ResourceTypeSyntax : TypeSyntax
    {
        public ResourceTypeSyntax(Token keyword, SyntaxBase? type)
        {
            AssertKeyword(keyword, nameof(keyword), LanguageConstants.ResourceKeyword);
            AssertSyntaxType(type, nameof(type), typeof(StringSyntax), typeof(SkippedTriviaSyntax));

            this.Keyword = keyword;
            this.Type = type;
        }

        public Token Keyword { get; }

        public SyntaxBase? Type { get; }

        public StringSyntax? TypeString => Type as StringSyntax;

        public override void Accept(ISyntaxVisitor visitor)
        {
            visitor.VisitResourceTypeSyntax(this);
        }

        public override TextSpan Span => TextSpan.Between(this.Keyword, this.Type ?? this.Keyword);
    }
}
